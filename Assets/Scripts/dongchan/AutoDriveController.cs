// =============================================================================
// AutoReverseController.cs - X축 기반 자동 후진 컨트롤러
// =============================================================================
using UnityEngine;
using System.Collections;

public class AutoReverseController : MonoBehaviour
{
    [Header("References")]
    public WaypointManager waypointManager;
    public Transform car;
    
    [Header("Movement Settings")]
    public float acceleration = 10f;
    public float deceleration = 15f;
    public float turnSpeed = 5f;
    public float stoppingDistance = 2f;
    public float maxSpeed = 20f;
    
    [Header("Reverse Settings")]
    public bool enableReverseMode = true;
    public float reverseSpeedMultiplier = 0.8f; // 후진 속도 배율
    
    [Header("Detection Settings")]
    public float lookAheadDistance = 5f;
    public LayerMask obstacleLayer = -1;
    public bool enableObstacleDetection = true;
    
    [Header("Debug")]
    public bool showDebugInfo = true;
    
    // Private variables
    private int currentWaypointIndex = 0;
    private float currentSpeed = 0f;
    private bool isMoving = false;
    private bool isWaiting = false;
    private Vector3 targetPosition;
    private float targetSpeed;
    private bool isReversing = false;
    private Vector3 previousWaypointPosition;
    
    // 차량 제어를 위한 Rigidbody
    public Rigidbody carRigidbody;
    
    void Start()
    {
        // Rigidbody 컴포넌트 찾기
        if (carRigidbody == null)
        {
            carRigidbody = car.GetComponent<Rigidbody>();
        }
        
        if (carRigidbody == null)
        {
            Debug.LogError("차량에 Rigidbody가 없습니다!");
            return;
        }
        
        if (waypointManager == null)
            waypointManager = FindObjectOfType<WaypointManager>();
            
        if (waypointManager != null && waypointManager.waypoints.Count > 0)
        {
            StartAutoDrive();
        }
        else
        {
            Debug.LogWarning("웨이포인트가 설정되지 않았습니다!");
        }
    }
    
    void Update()
    {
        if (!isMoving || waypointManager == null || waypointManager.waypoints.Count == 0)
            return;
            
        UpdateAutoDrive();
    }
    
    void FixedUpdate()
    {
        if (!isMoving || carRigidbody == null) return;
        
        ApplyMovement();
    }
    
    public void StartAutoDrive()
    {
        if (waypointManager == null || waypointManager.waypoints.Count == 0)
        {
            Debug.LogWarning("웨이포인트가 설정되지 않았습니다!");
            return;
        }
        
        currentWaypointIndex = 0;
        isMoving = true;
        
        // 첫 번째 웨이포인트를 이전 위치로 설정
        previousWaypointPosition = waypointManager.waypoints[0].position;
        
        SetCurrentTarget();
        
        Debug.Log("자동 운전 시작!");
    }
    
    public void StopAutoDrive()
    {
        isMoving = false;
        StopAllCoroutines();
        
        // 차량 정지
        if (carRigidbody != null)
        {
            carRigidbody.linearVelocity = Vector3.zero;
            carRigidbody.angularVelocity = Vector3.zero;
        }
        
        Debug.Log("자동 운전 종료");
    }
    
    void UpdateAutoDrive()
    {
        if (isWaiting) return;
        
        float distanceToTarget = Vector3.Distance(car.position, targetPosition);
        
        // 목표 지점에 도달했는지 확인
        if (distanceToTarget <= stoppingDistance)
        {
            OnWaypointReached();
            return;
        }
        
        // 장애물 감지 (선택사항)
        if (enableObstacleDetection && DetectObstacle())
        {
            // 장애물이 있으면 정지
            currentSpeed = 0f;
            return;
        }
        
        // 속도 계산
        CalculateSpeed(distanceToTarget);
    }
    
    void ApplyMovement()
    {
        if (isWaiting || carRigidbody == null) return;
        
        // 목표 방향 계산
        Vector3 direction = (targetPosition - car.position).normalized;
        
        // 회전 처리
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation;
            
            if (isReversing)
            {
                // 후진 시: 목표 방향의 반대로 차량이 바라보도록 설정
                targetRotation = Quaternion.LookRotation(-direction);
            }
            else
            {
                // 전진 시: 목표 방향으로 차량이 바라보도록 설정
                targetRotation = Quaternion.LookRotation(direction);
            }
            
            car.rotation = Quaternion.Slerp(car.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
        }
        
        // 이동 처리
        if (currentSpeed > 0.1f)
        {
            Vector3 moveDirection;
            
            if (isReversing)
            {
                // 후진 시: 차량의 뒤쪽 방향으로 이동
                moveDirection = -car.forward * currentSpeed;
            }
            else
            {
                // 전진 시: 차량의 앞쪽 방향으로 이동
                moveDirection = car.forward * currentSpeed;
            }
            
            carRigidbody.linearVelocity = new Vector3(moveDirection.x, carRigidbody.linearVelocity.y, moveDirection.z);
        }
        else
        {
            // 정지
            Vector3 velocity = carRigidbody.linearVelocity;
            carRigidbody.linearVelocity = new Vector3(0, velocity.y, 0);
        }
    }
    
    void CalculateSpeed(float distanceToTarget)
    {
        // 목표 속도에 따른 가속/감속
        float desiredSpeed = targetSpeed;
        
        // 후진 시 속도 조정
        if (isReversing)
        {
            desiredSpeed *= reverseSpeedMultiplier;
        }
        
        // 목표 지점 근처에서 감속
        if (distanceToTarget < stoppingDistance * 4f)
        {
            float slowDownFactor = distanceToTarget / (stoppingDistance * 4f);
            desiredSpeed *= slowDownFactor;
        }
        
        // 부드러운 속도 변화
        if (currentSpeed < desiredSpeed)
        {
            currentSpeed += acceleration * Time.deltaTime;
        }
        else if (currentSpeed > desiredSpeed)
        {
            currentSpeed -= deceleration * Time.deltaTime;
        }
        
        currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);
    }
    
    bool DetectObstacle()
    {
        if (!enableObstacleDetection) return false;
        
        // 전방/후방 장애물 감지
        Vector3 rayOrigin = car.position + Vector3.up;
        Vector3 rayDirection = isReversing ? -car.forward : car.forward;
        
        if (Physics.Raycast(rayOrigin, rayDirection, lookAheadDistance, obstacleLayer))
        {
            return true;
        }
        
        return false;
    }
    
    void OnWaypointReached()
    {
        var currentWaypoint = waypointManager.waypoints[currentWaypointIndex];
        
        Debug.Log($"웨이포인트 {currentWaypointIndex} 도달! (후진: {isReversing})");
        
        // 대기 시간이 있으면 대기
        if (currentWaypoint.waitTime > 0f)
        {
            StartCoroutine(WaitAtWaypoint(currentWaypoint.waitTime));
        }
        else
        {
            MoveToNextWaypoint();
        }
    }
    
    IEnumerator WaitAtWaypoint(float waitTime)
    {
        isWaiting = true;
        currentSpeed = 0f;
        
        Debug.Log($"웨이포인트에서 {waitTime}초 대기 중...");
        
        yield return new WaitForSeconds(waitTime);
        
        isWaiting = false;
        MoveToNextWaypoint();
    }
    
    void MoveToNextWaypoint()
    {
        // 현재 웨이포인트 위치를 이전 위치로 저장
        if (currentWaypointIndex < waypointManager.waypoints.Count)
        {
            previousWaypointPosition = waypointManager.waypoints[currentWaypointIndex].position;
        }
        
        currentWaypointIndex++;
        
        // 경로 끝에 도달
        if (currentWaypointIndex >= waypointManager.waypoints.Count)
        {
            if (waypointManager.loopPath)
            {
                currentWaypointIndex = 0; // 처음으로 돌아가기
                // 루프 시 마지막 웨이포인트를 이전 위치로 설정
                previousWaypointPosition = waypointManager.waypoints[waypointManager.waypoints.Count - 1].position;
                Debug.Log("경로 루프 - 처음 웨이포인트로 이동");
            }
            else
            {
                Debug.Log("모든 웨이포인트 완료 - 자동 운전 종료");
                StopAutoDrive();
                return;
            }
        }
        
        SetCurrentTarget();
    }
    
    void SetCurrentTarget()
    {
        if (currentWaypointIndex < waypointManager.waypoints.Count)
        {
            var waypoint = waypointManager.waypoints[currentWaypointIndex];
            targetPosition = waypoint.position;
            targetSpeed = waypoint.speed;
            
            // X축 기반 후진 조건 확인
            CheckReverseCondition();
            
            Debug.Log($"새 목표: 웨이포인트 {currentWaypointIndex}, 속도: {targetSpeed}, 후진: {isReversing}");
            Debug.Log($"현재 X: {targetPosition.x}, 이전 X: {previousWaypointPosition.x}");
        }
    }
    
    void CheckReverseCondition()
    {
        if (!enableReverseMode) 
        {
            isReversing = false;
            return;
        }
        
        // X축 위치 비교: 현재 목표 웨이포인트의 X축이 이전 웨이포인트보다 작으면 후진
        isReversing = targetPosition.x >= previousWaypointPosition.x;
        
        Debug.Log($"후진 조건 확인: 목표 X({targetPosition.x}) < 이전 X({previousWaypointPosition.x}) = {isReversing}");
    }
    
    void OnDrawGizmos()
    {
        if (!showDebugInfo || !isMoving) return;
        
        // 현재 목표 표시
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(targetPosition, 1f);
        
        // 목표까지의 선
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(car.position, targetPosition);
        
        // 이전 웨이포인트 표시
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(previousWaypointPosition, 0.8f);
        
        // 장애물 감지 레이
        if (enableObstacleDetection)
        {
            Gizmos.color = Color.red;
            Vector3 rayOrigin = car.position + Vector3.up;
            Vector3 rayDirection = isReversing ? -car.forward : car.forward;
            Gizmos.DrawRay(rayOrigin, rayDirection * lookAheadDistance);
        }
        
        // 현재 속도 표시 (후진 시 다른 색상)
        Gizmos.color = isReversing ? Color.magenta : Color.cyan;
        Vector3 speedVector = isReversing ? -car.forward * currentSpeed : car.forward * currentSpeed;
        Gizmos.DrawRay(car.position, speedVector);
        
        // 후진 상태 표시
        if (isReversing)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(car.position + Vector3.up * 2f, Vector3.one * 0.5f);
        }
        
        // X축 정보 표시
        Gizmos.color = Color.white;
        Vector3 textPos = car.position + Vector3.up * 3f;
        
        // Unity Editor에서만 텍스트 표시 (Gizmos는 텍스트 지원 안함)
        #if UNITY_EDITOR
        UnityEditor.Handles.color = Color.white;
        UnityEditor.Handles.Label(textPos, $"X: {targetPosition.x:F1} | Prev X: {previousWaypointPosition.x:F1} | Reverse: {isReversing}");
        #endif
    }
}