using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float time;
    private Text text;
    private DelayTimeMain DelayCount;

    void Start()
    {
        DelayCount = GameObject.Find("Canvas").GetComponent<DelayTimeMain>();
        text = this.gameObject.GetComponent<Text>();
    }

    void Update()
    {
        if (DelayCount.DelayCount == 0)
        {
            time += Time.deltaTime;
            text.text = string.Format("{0:F1}", time);
            //Debug.Log("[시간 재기 UI ]Time: " + time);
        }
    }

}
