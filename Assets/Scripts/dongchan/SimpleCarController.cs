using UnityEngine;

public class SimpleCarController : MonoBehaviour
{
    [Header("Car Settings")]
    public float motorTorque = 1500f;
    public float brakeTorque = 3000f;
    public float maxSteerAngle = 30f;
    
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
    
    private float motor;
    private float steering;
    private bool isBraking;
    
    void FixedUpdate()
    {
        // 모터 토크 적용
        float motorForce = motor * motorTorque;
        rearLeftWheel.motorTorque = motorForce;
        rearRightWheel.motorTorque = motorForce;
        
        // 스티어링 적용
        float steerAngle = steering * maxSteerAngle;
        frontLeftWheel.steerAngle = steerAngle;
        frontRightWheel.steerAngle = steerAngle;
        
        // 브레이크 적용
        float brakeForce = isBraking ? brakeTorque : 0f;
        frontLeftWheel.brakeTorque = brakeForce;
        frontRightWheel.brakeTorque = brakeForce;
        rearLeftWheel.brakeTorque = brakeForce;
        rearRightWheel.brakeTorque = brakeForce;
        
        // 휠 메시 업데이트
        UpdateWheelMeshes();
    }
    
    public void SetInputs(float motorInput, float steerInput, bool brakeInput)
    {
        motor = motorInput;
        steering = steerInput;
        isBraking = brakeInput;
    }
    
    void UpdateWheelMeshes()
    {
        UpdateWheelMesh(frontLeftWheel, frontLeftWheelMesh);
        UpdateWheelMesh(frontRightWheel, frontRightWheelMesh);
        UpdateWheelMesh(rearLeftWheel, rearLeftWheelMesh);
        UpdateWheelMesh(rearRightWheel, rearRightWheelMesh);
    }
    
    void UpdateWheelMesh(WheelCollider wheel, Transform wheelMesh)
    {
        if (wheelMesh == null) return;
        
        Vector3 position;
        Quaternion rotation;
        wheel.GetWorldPose(out position, out rotation);
        
        wheelMesh.position = position;
        wheelMesh.rotation = rotation;
    }
}
