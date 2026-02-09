// =============================================================================
// SteeringWheelController.cs - 차체 움직임에 따른 핸들 회전
// =============================================================================
using UnityEngine;

public class SteeringWheelController : MonoBehaviour
{
    [Header("Car References")]
    public Transform carTransform; // 차체 Transform
    public Rigidbody carRigidbody; // 차체 Rigidbody
    
    [Header("Steering Settings")]
    public float maxSteeringAngle = 540f; // 최대 핸들 회전 각도 (1.5바퀴)
    public float steeringSpeed = 5f; // 핸들 회전 속도
    public float returnSpeed = 3f; // 핸들 복귀 속도
    public bool smoothSteering = true; // 부드러운 핸들 회전
    
    [Header("Sensitivity Settings")]
    public float velocitySensitivity = 1f; // 속도 민감도
    public float angularSensitivity = 2f; // 각속도 민감도
    public float deadZone = 0.1f; // 데드존 (작은 움직임 무시)
    
    [Header("Steering Axis")]
    public Vector3 steeringAxis = Vector3.right ; // 핸들 회전 축 (Z축)
    
    [Header("Debug")]
    public bool showDebugInfo = true;
    
    // Private variables
    private float targetSteeringAngle = 0f;
    private float currentSteeringAngle = 0f;
    private Vector3 lastPosition;
    private Vector3 lastForward;
    private float lastAngularVelocity = 0f;
    
    void Start()
    {
        // 자동으로 차체 찾기
        if (carTransform == null)
        {
            // 부모에서 차체 찾기
            Transform parent = transform.parent;
            while (parent != null)
            {
                if (parent.GetComponent<Rigidbody>() != null)
                {
                    carTransform = parent;
                    break;
                }
                parent = parent.parent;
            }
        }
        
        // Rigidbody 자동 설정
        if (carRigidbody == null && carTransform != null)
        {
            carRigidbody = carTransform.GetComponent<Rigidbody>();
        }
        
        // 초기 위치 저장
        if (carTransform != null)
        {
            lastPosition = carTransform.position;
            lastForward = carTransform.forward;
        }
        
        if (carTransform == null)
        {
            Debug.LogError("SteeringWheelController: 차체 Transform을 찾을 수 없습니다!");
        }
    }
    
    void Update()
    {
        if (carTransform == null) return;
        
        // 핸들 각도 계산
        CalculateSteeringAngle();
        
        // 핸들 회전 적용
        ApplySteeringRotation();
        
        // 디버그 정보 출력
        if (showDebugInfo)
        {
            ShowDebugInfo();
        }
    }
    
    void CalculateSteeringAngle()
    {
        float steeringInput = 0f;
        
        // 방법 1: Rigidbody 각속도 사용 (권장)
        if (carRigidbody != null)
        {
            steeringInput = CalculateSteeringFromRigidbody();
        }
        else
        {
            // 방법 2: Transform 변화량 사용
            steeringInput = CalculateSteeringFromTransform();
        }
        
        // 데드존 적용
        if (Mathf.Abs(steeringInput) < deadZone)
        {
            steeringInput = 0f;
        }
        
        // 목표 핸들 각도 설정
        targetSteeringAngle = steeringInput * maxSteeringAngle;
    }
    
    float CalculateSteeringFromRigidbody()
    {
        if (carRigidbody == null) return 0f;
        
        // 차체의 Y축 각속도 (회전 속도)
        float angularVelocityY = carRigidbody.angularVelocity.y;
        
        // 속도에 따른 민감도 조정
        float speedFactor = carRigidbody.linearVelocity.magnitude / 10f; // 속도 정규화
        speedFactor = Mathf.Clamp(speedFactor, 0.1f, 2f);
        
        // 핸들 입력 계산
        float steeringInput = angularVelocityY * angularSensitivity * speedFactor;
        
        return Mathf.Clamp(steeringInput, -1f, 1f);
    }
    
    float CalculateSteeringFromTransform()
    {
        // 현재 전방 방향
        Vector3 currentForward = carTransform.forward;
        
        // 방향 변화량 계산
        float angleDifference = Vector3.SignedAngle(lastForward, currentForward, Vector3.up);
        
        // 시간 기반 각속도 계산
        float angularVelocity = angleDifference / Time.deltaTime;
        
        // 속도 고려
        Vector3 velocity = (carTransform.position - lastPosition) / Time.deltaTime;
        float speedFactor = velocity.magnitude * velocitySensitivity;
        
        // 핸들 입력 계산
        float steeringInput = (angularVelocity * angularSensitivity * speedFactor) / 100f;
        
        // 이전 값 저장
        lastPosition = carTransform.position;
        lastForward = currentForward;
        
        return Mathf.Clamp(steeringInput, -1f, 1f);
    }
    
    void ApplySteeringRotation()
    {
        if (smoothSteering)
        {
            // 부드러운 핸들 회전
            float speed = Mathf.Abs(targetSteeringAngle - currentSteeringAngle) > 0.1f ? steeringSpeed : returnSpeed;
            currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, targetSteeringAngle, speed * Time.deltaTime);
        }
        else
        {
            // 즉시 핸들 회전
            currentSteeringAngle = targetSteeringAngle;
        }
        
        // 핸들 회전 적용
        Vector3 rotationEuler = steeringAxis * currentSteeringAngle;
        transform.localRotation = Quaternion.Euler(rotationEuler);
    }
    
    void ShowDebugInfo()
    {
        if (carRigidbody != null)
        {
            Debug.Log($"핸들 각도: {currentSteeringAngle:F1}° | 목표: {targetSteeringAngle:F1}° | 차체 각속도: {carRigidbody.angularVelocity.y:F2}");
        }
    }
    
    // 수동으로 핸들 각도 설정
    public void SetSteeringAngle(float angle)
    {
        targetSteeringAngle = Mathf.Clamp(angle, -maxSteeringAngle, maxSteeringAngle);
    }
    
    // 핸들 중앙 복귀
    public void ResetSteering()
    {
        targetSteeringAngle = 0f;
    }
    
    // 현재 핸들 각도 반환
    public float GetCurrentSteeringAngle()
    {
        return currentSteeringAngle;
    }
    
    // 정규화된 핸들 입력 반환 (-1 ~ 1)
    public float GetSteeringInput()
    {
        return currentSteeringAngle / maxSteeringAngle;
    }
}

// =============================================================================
// 고급 핸들 컨트롤러 (더 정교한 계산)
// =============================================================================

public class AdvancedSteeringWheelController : MonoBehaviour
{
    [Header("Car References")]
    public Transform carTransform;
    public Rigidbody carRigidbody;
    
    [Header("Wheel References")]
    public Transform[] frontWheels; // 앞바퀴들 (스티어링 휠)
    
    [Header("Steering Settings")]
    public float maxSteeringAngle = 540f;
    public float steeringSpeed = 5f;
    public float returnSpeed = 3f;
    public bool useWheelAngle = true; // 바퀴 각도 기반 계산
    
    [Header("Steering Axis")]
    public Vector3 steeringAxis = Vector3.forward;
    
    private float targetSteeringAngle = 0f;
    private float currentSteeringAngle = 0f;
    
    void Start()
    {
        // 자동으로 앞바퀴 찾기
        if (frontWheels.Length == 0)
        {
            // "FrontWheel" 태그나 이름으로 찾기
            GameObject[] wheels = GameObject.FindGameObjectsWithTag("FrontWheel");
            if (wheels.Length > 0)
            {
                frontWheels = new Transform[wheels.Length];
                for (int i = 0; i < wheels.Length; i++)
                {
                    frontWheels[i] = wheels[i].transform;
                }
            }
        }
    }
    
    void Update()
    {
        if (carTransform == null) return;
        
        CalculateAdvancedSteeringAngle();
        ApplySteeringRotation();
    }
    
    void CalculateAdvancedSteeringAngle()
    {
        float steeringInput = 0f;
        
        if (useWheelAngle && frontWheels.Length > 0)
        {
            // 앞바퀴 각도 기반 계산
            steeringInput = CalculateSteeringFromWheelAngle();
        }
        else if (carRigidbody != null)
        {
            // 차체 각속도 기반 계산
            steeringInput = CalculateSteeringFromAngularVelocity();
        }
        
        targetSteeringAngle = steeringInput * maxSteeringAngle;
    }
    
    float CalculateSteeringFromWheelAngle()
    {
        if (frontWheels.Length == 0) return 0f;
        
        float averageWheelAngle = 0f;
        
        foreach (Transform wheel in frontWheels)
        {
            if (wheel != null)
            {
                // 바퀴의 Y축 회전 각도
                float wheelAngle = wheel.localEulerAngles.y;
                
                // 각도 정규화 (-180 ~ 180)
                if (wheelAngle > 180f) wheelAngle -= 360f;
                
                averageWheelAngle += wheelAngle;
            }
        }
        
        averageWheelAngle /= frontWheels.Length;
        
        // 바퀴 각도를 핸들 입력으로 변환 (일반적으로 바퀴 최대 각도는 30도)
        float maxWheelAngle = 30f;
        return Mathf.Clamp(averageWheelAngle / maxWheelAngle, -1f, 1f);
    }
    
    float CalculateSteeringFromAngularVelocity()
    {
        float angularVelocityY = carRigidbody.angularVelocity.y;
        float speedFactor = Mathf.Clamp(carRigidbody.linearVelocity.magnitude / 10f, 0.1f, 2f);
        
        return Mathf.Clamp(angularVelocityY * 2f * speedFactor, -1f, 1f);
    }
    
    void ApplySteeringRotation()
    {
        float speed = Mathf.Abs(targetSteeringAngle - currentSteeringAngle) > 0.1f ? steeringSpeed : returnSpeed;
        currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, targetSteeringAngle, speed * Time.deltaTime);
        
        Vector3 rotationEuler = steeringAxis * currentSteeringAngle;
        transform.localRotation = Quaternion.Euler(rotationEuler);
    }
}