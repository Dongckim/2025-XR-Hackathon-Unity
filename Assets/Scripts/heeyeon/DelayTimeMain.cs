using UnityEngine;

public class DelayTimeMain : MonoBehaviour
{
    //딜레이 코루틴
    public int DelayCount = 3;

    private float delayTimer = 0f;

    void Update()
    {
        if (DelayCount > 0)
        {
            // 1초마다 DelayCount 감소
            delayTimer += Time.deltaTime;
            if (delayTimer >= 1f)
            {
                DelayCount--;
                delayTimer = 0f;
            }
        }
    }
}
