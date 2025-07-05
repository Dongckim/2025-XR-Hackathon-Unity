using System.Collections;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    [Header("Car Controller")]
    public AutoCarController carController;
    
    [Header("Target Cubes (드래그해서 연결)")]
    public Transform cube1;
    public Transform cube2;
    public Transform cube3;
    public Transform cube4;
    
    [Header("Test Settings")]
    public float delayBetweenMoves = 2f;
    public bool autoStart = true;
    public bool loopTest = false;
    
    [Header("Test Controls")]
    public KeyCode startTestKey = KeyCode.T;
    public KeyCode stopTestKey = KeyCode.S;
    public KeyCode resetCarKey = KeyCode.R;
    
    private bool isTestRunning = false;
    
    void Start()
    {
        StartCoroutine(DelayedStart());
    }
    
    void Update()
    {
        HandleInput();
    }
    
    void HandleInput()
    {
        if (Input.GetKeyDown(startTestKey))
        {
            StartTest();
        }
        
        if (Input.GetKeyDown(stopTestKey))
        {
            StopTest();
        }
        
        if (Input.GetKeyDown(resetCarKey))
        {
            ResetCarPosition();
        }
    }
    
    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(1f);
        StartTest();
    }
    
    public void StartTest()
    {
        if (isTestRunning) return;
        
        if (!ValidateSetup())
        {
            Debug.LogError("테스트 설정이 완료되지 않았습니다!");
            return;
        }
        
        Debug.Log("=== 차량 테스트 시작 ===");
        isTestRunning = true;
        StartCoroutine(RunTest());
    }
    
    public void StopTest()
    {
        if (!isTestRunning) return;
        
        Debug.Log("=== 차량 테스트 중지 ===");
        isTestRunning = false;
        StopAllCoroutines();
        carController.StopMoving();
    }
    
    bool ValidateSetup()
    {
        if (carController == null)
        {
            Debug.LogError("AutoCarController가 연결되지 않았습니다!");
            return false;
        }
        
        if (cube1 == null || cube2 == null || cube3 == null || cube4 == null)
        {
            Debug.LogError("모든 Cube가 연결되지 않았습니다!");
            return false;
        }
        
        return true;
    }
    
    IEnumerator RunTest()
    {
        do
        {
            // 테스트 시작 로그
            Debug.Log($"테스트 시작 - 차량 위치: {carController.transform.position}");
            
            // Cube 1로 이동
            yield return StartCoroutine(MoveToTarget("Cube 1", cube1.position));
            
            // Cube 2로 이동
            yield return StartCoroutine(MoveToTarget("Cube 2", cube2.position));
            
            // Cube 3으로 이동
            yield return StartCoroutine(MoveToTarget("Cube 3", cube3.position));
            
            // Cube 4로 이동
            yield return StartCoroutine(MoveToTarget("Cube 4", cube4.position));
            
            Debug.Log("=== 한 사이클 완료 ===");
            
            if (loopTest)
            {
                yield return new WaitForSeconds(delayBetweenMoves);
                Debug.Log("루프 테스트 - 다음 사이클 시작");
            }
            
        } while (loopTest && isTestRunning);
        
        if (!loopTest)
        {
            Debug.Log("=== 모든 테스트 완료 ===");
            isTestRunning = false;
        }
    }
    
    IEnumerator MoveToTarget(string targetName, Vector3 targetPosition)
    {
        Debug.Log($"→ {targetName} 이동 시작: {targetPosition}");
        
        // 차량에게 이동 명령
        carController.MoveTwoPoints(carController.transform.position, targetPosition);
        
        // 이동 완료까지 대기
        while (carController.IsMoving())
        {
            float distance = carController.GetDistanceToTarget();
            Debug.Log($"  {targetName}까지 거리: {distance:F1}m");
            yield return new WaitForSeconds(0.5f);
        }
        
        Debug.Log($"✓ {targetName} 도착 완료!");
        
        // 다음 이동까지 대기
        yield return new WaitForSeconds(delayBetweenMoves);
    }
    
    public void ResetCarPosition()
    {
        if (cube1 != null)
        {
            carController.transform.position = cube1.position + Vector3.up * 0.5f;
            carController.transform.rotation = Quaternion.identity;
            carController.StopMoving();
            Debug.Log("차량 위치를 Cube 1 근처로 리셋했습니다.");
        }
    }
    
    // 개별 Cube로 이동하는 공개 메서드들
    public void MoveToCube1() { MoveToSingleCube("Cube 1", cube1); }
    public void MoveToCube2() { MoveToSingleCube("Cube 2", cube2); }
    public void MoveToCube3() { MoveToSingleCube("Cube 3", cube3); }
    public void MoveToCube4() { MoveToSingleCube("Cube 4", cube4); }
    
    void MoveToSingleCube(string cubeName, Transform cubeTransform)
    {
        if (cubeTransform == null)
        {
            Debug.LogError($"{cubeName}이 연결되지 않았습니다!");
            return;
        }
        
        Debug.Log($"단일 이동: {cubeName}으로 이동");
        carController.MoveTwoPoints(carController.transform.position, cubeTransform.position);
    }
    
    // 모든 Cube를 순서대로 방문
    public void VisitAllCubes()
    {
        if (!ValidateSetup()) return;
        
        Vector3[] allCubes = new Vector3[] {
            cube1.position,
            cube2.position,
            cube3.position,
            cube4.position
        };
        
        Debug.Log("모든 Cube 순차 방문 시작");
        carController.MoveToWaypoints(allCubes);
    }
    
    // UI 표시용 정보
    void OnGUI()
    {
        if (!isTestRunning) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("=== Car Test Status ===");
        GUILayout.Label($"테스트 실행 중: {isTestRunning}");
        GUILayout.Label($"차량 이동 중: {carController.IsMoving()}");
        GUILayout.Label($"현재 목표까지 거리: {carController.GetDistanceToTarget():F1}m");
        GUILayout.Label($"현재 차량 위치: {carController.transform.position}");
        
        GUILayout.Space(10);
        GUILayout.Label("=== 컨트롤 ===");
        GUILayout.Label($"T: 테스트 시작");
        GUILayout.Label($"S: 테스트 중지");
        GUILayout.Label($"R: 차량 리셋");
        GUILayout.EndArea();
    }
}