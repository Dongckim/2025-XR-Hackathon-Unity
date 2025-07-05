using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuizManager : MonoBehaviour
{
    public GameObject[] questionCanvases; // 0, 1, 2번 질문 캔버스
    public GameObject resultCanvas;       // 결과 캔버스
    public Text resultText;               // 결과 텍스트

    public Transform targetObject;              // 이동시킬 오브젝트
    public Vector3 moveOffset = new Vector3(0, 0, 0.5f); // 이동량
    public float moveDuration = 1f;             // 이동에 걸릴 시간

    private int currentIndex = 0;

    // 정답 선택 시
    public void OnCorrect(string message)
    {
        if (currentIndex == 0 && targetObject != null)
        {
            // 오브젝트 이동 후 결과 UI 띄우기
            StartCoroutine(AnimateMoveAndShowResult(targetObject, moveOffset, moveDuration, message));
        }
        else
        {
            ShowResultThenNext(message);
        }
    }

    // 오답 선택 시 (다음 문제로 안 넘기고 결과만 보여주기)
    public void OnWrong(string message)
    {
        ShowResultThenNext(message);
    }

    private IEnumerator AnimateMoveAndShowResult(Transform obj, Vector3 offset, float duration, string message)
    {
        Vector3 startPos = obj.position;
        Vector3 endPos = startPos
            + obj.forward.normalized * offset.z
            + obj.right.normalized * offset.x
            + obj.up.normalized * offset.y;

        float time = 0f;

        while (time < duration)
        {
            obj.position = Vector3.Lerp(startPos, endPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        obj.position = endPos;

        // 이동 후 결과 UI 표시
        ShowResultThenNext(message);
    }


    private void ShowResultThenNext(string message)
    {
        resultCanvas.SetActive(true);
        resultText.text = message;
        Invoke(nameof(GoToNextCanvas), 2f);
    }

    private void GoToNextCanvas()
    {
        resultCanvas.SetActive(false);

        if (currentIndex < questionCanvases.Length)
        {
            questionCanvases[currentIndex].SetActive(false);
            currentIndex++;
        }

        if (currentIndex < questionCanvases.Length)
        {
            questionCanvases[currentIndex].SetActive(true);
        }
        else
        {
            Debug.Log("퀴즈 끝!");
        }
    }
}
