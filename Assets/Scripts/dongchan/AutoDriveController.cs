// =============================================================================
// 2. AutoDriveController.cs - 자동 운전 메인 컨트롤러
// =============================================================================
using UnityEngine;
using System.Collections;

public class AutoDriveController : MonoBehaviour
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
    
    // 차량 제어를 위한 Rigidbody
    public Rigidbody carRigidbody;
    
    void Start()
    {
        // Rigidbody 컴포넌트 찾
        
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
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            car.rotation = Quaternion.Slerp(car.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
        }
        
        // 이동 처리
        if (currentSpeed > 0.1f)
        {
            Vector3 moveDirection = car.forward * currentSpeed;
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
        
        // 전방 장애물 감지
        Vector3 rayOrigin = car.position + Vector3.up;
        Vector3 rayDirection = car.forward;
        
        if (Physics.Raycast(rayOrigin, rayDirection, lookAheadDistance, obstacleLayer))
        {
            return true;
        }
        
        return false;
    }
    
    void OnWaypointReached()
    {
        var currentWaypoint = waypointManager.waypoints[currentWaypointIndex];
        
        Debug.Log($"웨이포인트 {currentWaypointIndex} 도달!");
        
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
        currentWaypointIndex++;
        
        // 경로 끝에 도달
        if (currentWaypointIndex >= waypointManager.waypoints.Count)
        {
            if (waypointManager.loopPath)
            {
                currentWaypointIndex = 0; // 처음으로 돌아가기
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
            
            Debug.Log($"새 목표: 웨이포인트 {currentWaypointIndex}, 속도: {targetSpeed}");
        }
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
        
        // 장애물 감지 레이
        if (enableObstacleDetection)
        {
            Gizmos.color = Color.red;
            Vector3 rayOrigin = car.position + Vector3.up;
            Gizmos.DrawRay(rayOrigin, car.forward * lookAheadDistance);
        }
        
        // 현재 속도 표시
        Gizmos.color = Color.cyan;
        Vector3 speedVector = car.forward * currentSpeed;
        Gizmos.DrawRay(car.position, speedVector);
    }
}