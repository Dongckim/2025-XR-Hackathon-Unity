// =============================================================================
// 1. WaypointManager.cs - 웨이포인트 관리 및 시각화
// =============================================================================
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Waypoint
{
    public Vector3 position;
    public float speed = 10f;
    public float waitTime = 0f;
    
    public Waypoint(Vector3 pos)
    {
        position = pos;
    }
}

public class WaypointManager : MonoBehaviour
{
    [Header("Waypoint Settings")]
    public List<Waypoint> waypoints = new List<Waypoint>();
    public bool loopPath = true;
    public Color waypointColor = Color.red;
    public Color pathColor = Color.yellow;
    public float waypointSize = 1f;
    
    [Header("Editor Tools")]
    public bool showWaypoints = true;
    public bool showPath = true;
    
    void OnDrawGizmos()
    {
        if (!showWaypoints || waypoints == null || waypoints.Count == 0) return;
        
        // 웨이포인트 그리기
        Gizmos.color = waypointColor;
        for (int i = 0; i < waypoints.Count; i++)
        {
            Gizmos.DrawWireSphere(waypoints[i].position, waypointSize);
            
            // 번호 표시 (Scene 뷰에서)
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(waypoints[i].position + Vector3.up * 2, i.ToString());
            #endif
        }
        
        // 경로 그리기
        if (showPath && waypoints.Count > 1)
        {
            Gizmos.color = pathColor;
            for (int i = 0; i < waypoints.Count - 1; i++)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
            
            // 루프 경로인 경우 마지막과 첫 번째 연결
            if (loopPath && waypoints.Count > 2)
            {
                Gizmos.DrawLine(waypoints[waypoints.Count - 1].position, waypoints[0].position);
            }
        }
    }
    
    // 런타임에서 웨이포인트 추가
    public void AddWaypoint(Vector3 position, float speed = 10f, float waitTime = 0f)
    {
        waypoints.Add(new Waypoint(position) { speed = speed, waitTime = waitTime });
    }
    
    // 웨이포인트 제거
    public void RemoveWaypoint(int index)
    {
        if (index >= 0 && index < waypoints.Count)
        {
            waypoints.RemoveAt(index);
        }
    }
}