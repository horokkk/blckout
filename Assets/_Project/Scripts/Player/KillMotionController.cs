using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class KillMotionController : MonoBehaviour
{
    public static KillMotionController instance;

    [Header("연결할 UI")]
    public GameObject killMotionPanel; // 반투명 배경 패널 전체
    public Animator killAnimator;      // KillMotion_Image에 달린 애니메이터

    private void Awake()
    {
        instance = this;
        // 시작할 때는 무조건 꺼두기
        if (killMotionPanel != null) killMotionPanel.SetActive(false);
    }

    // 누군가 죽거나 죽일 때 호출
    public void ShowKillMotion()
    {
        killMotionPanel.SetActive(true); // UI 켜기
        killAnimator.SetTrigger("PlayKill"); // 킬 애니메이션 재생
        
        // 애니메이션이 끝난 후 UI를 다시 끄기 위한 코루틴 실행
        StartCoroutine(HideKillMotionRoutine());
    }

    private IEnumerator HideKillMotionRoutine()
    {
        // 킬 애니메이션의 길이만큼 기다리기
        yield return new WaitForSeconds(1.5f);

        killMotionPanel.SetActive(false); // 킬 모션 끝난 후 UI 끄기
    }
}