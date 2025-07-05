using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCarController : MonoBehaviour
{
    [Header("Car Settings")]
    public float motorTorque = 1500f;
    public float brakeTorque = 3000f;
    public float maxSteerAngle = 30f;
    public float downForce = 100f;
    public float maxSpeed = 10f;
    
    [Header("Wheels")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;
    
    [Header("Wheel Meshes")]
    public Transform frontLeftWheelMesh;
    public Transform frontRightWheelMesh;
    public Transform rearLeftWheelMesh;
    public Transform rearRightWheelMesh;
    
    [Header("Auto Drive Settings")]
    public float stoppingDistance = 2f;
    public float turnSpeed = 2f;
    public float waypointTolerance = 1f;
    
    [Header("Debug")]
    public bool showPath = true;
    public Color pathColor = Color.green;
    
    private Rigidbody rb;
    private List<Vector3> waypoints = new List<Vector3>();
    private int currentWaypointIndex = 0;
    private bool isMoving = false;
    
    private float motor;
    private float steering;
    private float brake;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        if (isMoving && waypoints.Count > 0)
        {
            AutoDrive();
        }
        UpdateWheelMeshes();
    }
    
    void FixedUpdate()
    {
        ApplyMotor();
        ApplySteering();
        ApplyBrake();
        ApplyDownforce();
    }
    
    // 두 좌표를 받아서 자동 이동 시작
    public void MoveTwoPoints(Vector3 startPos, Vector3 endPos)
    {
        waypoints.Clear();
        waypoints.Add(startPos);
        waypoints.Add(endPos);
        currentWaypointIndex = 0;
        isMoving = true;
        
        Debug.Log($"Moving from {startPos} to {endPos}");
    }
    
    // 다중 좌표를 받아서 순차적으로 이동
    public void MoveToWaypoints(Vector3[] positions)
    {
        waypoints.Clear();
        waypoints.AddRange(positions);
        currentWaypointIndex = 0;
        isMoving = true;
        
        Debug.Log($"Moving through {positions.Length} waypoints");
    }
    
    // 이동 중지
    public void StopMoving()
    {
        isMoving = false;
        motor = 0;
        steering = 0;
        brake = brakeTorque;
        Debug.Log("Car stopped");
    }
    
    void AutoDrive()
    {
        if (currentWaypointIndex >= waypoints.Count)
        {
            // 모든 waypoint 도달 완료
            StopMoving();
            Debug.Log("All waypoints reached!");
            return;
        }
        
        Vector3 targetPosition = waypoints[currentWaypointIndex];
        Vector3 direction = (targetPosition - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, targetPosition);
        
        // 목표 지점에 도달했는지 확인
        if (distance < waypointTolerance)
        {
            currentWaypointIndex++;
            Debug.Log($"Waypoint {currentWaypointIndex} reached!");
            return;
        }
        
        // 스티어링 계산
        Vector3 localDirection = transform.InverseTransformDirection(direction);
        float steerAngle = Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg;
        steering = Mathf.Clamp(steerAngle * turnSpeed, -maxSteerAngle, maxSteerAngle);
        
        // 속도 제어
        float currentSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);
        
        if (distance < stoppingDistance)
        {
            // 목표 지점 근처에서 감속
            brake = brakeTorque * 0.5f;
            motor = 0;
        }
        else if (currentSpeed < maxSpeed)
        {
            // 가속
            brake = 0;
            motor = motorTorque;
        }
        else
        {
            // 최대 속도 유지
            brake = 0;
            motor = 0;
        }
    }
    
    void ApplyMotor()
    {
        rearLeftWheel.motorTorque = motor;
        rearRightWheel.motorTorque = motor;
    }
    
    void ApplySteering()
    {
        frontLeftWheel.steerAngle = steering;
        frontRightWheel.steerAngle = steering;
    }
    
    void ApplyBrake()
    {
        frontLeftWheel.brakeTorque = brake;
        frontRightWheel.brakeTorque = brake;
        rearLeftWheel.brakeTorque = brake;
        rearRightWheel.brakeTorque = brake;
    }
    
    void ApplyDownforce()
    {
        rb.AddForce(-transform.up * downForce * rb.linearVelocity.magnitude);
    }
    
    void UpdateWheelMeshes()
    {
        UpdateWheelMesh(frontLeftWheel, frontLeftWheelMesh);
        UpdateWheelMesh(frontRightWheel, frontRightWheelMesh);
        UpdateWheelMesh(rearLeftWheel, rearLeftWheelMesh);
        UpdateWheelMesh(rearRightWheel, rearRightWheelMesh);
    }
    
    void UpdateWheelMesh(WheelCollider wheelCollider, Transform wheelMesh)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelMesh.position = pos;
        wheelMesh.rotation = rot;
    }
    
    // 경로 시각화 (Scene 뷰에서)
    void OnDrawGizmos()
    {
        if (!showPath || waypoints.Count < 2) return;
        
        Gizmos.color = pathColor;
        
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            Gizmos.DrawLine(waypoints[i], waypoints[i + 1]);
            Gizmos.DrawWireSphere(waypoints[i], 0.5f);
        }
        
        // 마지막 waypoint
        if (waypoints.Count > 0)
        {
            Gizmos.DrawWireSphere(waypoints[waypoints.Count - 1], 0.5f);
        }
        
        // 현재 목표 지점 강조
        if (isMoving && currentWaypointIndex < waypoints.Count)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(waypoints[currentWaypointIndex], 1f);
        }
    }
    
    // 공개 메서드들 - 다른 스크립트에서 호출 가능
    public bool IsMoving() { return isMoving; }
    public int GetCurrentWaypointIndex() { return currentWaypointIndex; }
    public float GetDistanceToTarget()
    {
        if (!isMoving || currentWaypointIndex >= waypoints.Count) return 0f;
        return Vector3.Distance(transform.position, waypoints[currentWaypointIndex]);
    }
}