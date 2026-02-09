using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float time;
    public Text text;
    public DelayTimeMain DelayCount;
    public Text endtext;

    void Start()
    {
        // 직접 연결했다면 이건 생략 가능
        // DelayCount = GameObject.Find("Canvas").GetComponent<DelayTimeMain>();
    }

    void Update()
    {
        if (DelayCount != null && DelayCount.DelayCount == 0)
        {
            time += Time.deltaTime;
            text.text = string.Format("{0:F1}", time);
        }
    }

    public void saveTime ()
    {
        endtext.text = string.Format("Time: {0:F1} seconds", time);
        Debug.Log("Time saved: " + time + " seconds");
    }
}
