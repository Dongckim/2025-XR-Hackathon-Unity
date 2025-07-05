using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEvent : MonoBehaviour
{
    [Header("결과 UI")]
    public GameObject resultPanel;      // ResultCanvas 내 결과 패널
    public Text resultText;         // 결과 텍스트

    [Header("표시할 텍스트")]
    public string displayMessage = "정답입니다!";

    public void OnButtonClicked()
    {
        resultPanel.SetActive(true);             // 결과 패널 보여주기
        resultText.text = displayMessage;        // 결과 메시지 출력
        Debug.Log("버튼이 눌려 결과가 표시됨");
    }
    //버튼 클릭 이벤트 작성 예정 
    public void ButtonEventLog () {
        Debug.Log("[버튼 클릭 이벤트 호출] Button clicked!");
    }
}
