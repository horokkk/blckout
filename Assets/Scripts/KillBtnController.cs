using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class KillBtnController : MonoBehaviourPunCallbacks
{
    [Header("UI 컴포넌트 연결")]
    [SerializeField] private Button killButton; // 킬 버튼
    [SerializeField] private Image hideImage; // 비활성화 역할 스킬 이미지

    [Header("스킬 설정")]
    [SerializeField] private float coolTime = 40f; // 쿨타임 40초(임시)

    private bool isCoolDown = false; // 스킬쿨 세는 중인지


    void Start()
    {
        // 일단 시작하자마자 킬 버튼 비활성화(직업 확인 전이므로)
        hideImage.gameObject.SetActive(false);
        killButton.gameObject.SetActive(false);

        // 킬 버튼은 클릭 이벤트 연결
        killButton.onClick.AddListener(OnClickKillButton);

        // 직업 확인 후 살인자만 KILL button UI 활성화
        CheckJobAndActivateUI();
    }

    void Update()
    {
        // 스페이스바 누르면 스킬 발동 함수 호출
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("스페이스바 눌림!!");
            OnClickKillButton();
        }
    }

    #region [직업 체크 로직]

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        // 내가 + 직업이 배정(변경)되었다면
        if (targetPlayer.IsLocal && changedProps.ContainsKey("Job"))
        {
            CheckJobAndActivateUI();
        }

    }

    public void CheckJobAndActivateUI()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Job"))
        {
            string job = (string)PhotonNetwork.LocalPlayer.CustomProperties["Job"];

            if (job == "Killer") // 직업이 킬러인 사람만 킬 버튼 활성화
            {
                Debug.Log("킬러의 킬 버튼 활성화!");
                killButton.gameObject.SetActive(true);
                killButton.interactable = true;

                // 만약 게임 시작하자마자 스킬 사용 가능 시 hide image는 false
                // 게임 시작 후 쿨타임 똑같이 지나야 스킬 버튼 활성화된다면 true로 변경하기
                hideImage.gameObject.SetActive(false);

            }
        }
    }

    #endregion

    #region [스킬 사용/로직]
    public void OnClickKillButton()
    {
        // 킬 기능: 사거리 내 가장 가까운 플레이어 죽이기
        Debug.Log("킬 버튼 눌림!!");

        if (isCoolDown) return; // 쿨타임 중이라면 무시

        if (Attack()) // 킬 성공 시
        {
            Debug.Log("스킬 사용함. 쿨타임 시작");
            StartCoroutine(StartCoolDown());
        }
        else // 킬 실패 시
        {
            // 아무 처리도 하지 않기.
            Debug.Log("스킬 사용 실패. 아무 일도 일어나지 않음..");
        }
    }

    public bool Attack()
    {
        // 사정거리 내 타겟이 있는지 확인.
    }

    // 스킬 쿨타임 360도 돌아가는 거(UI)용 코루틴 함수
    IEnumerator StartCoolDown()
    {
        isCoolDown = true;
        killButton.interactable = false;

        // 덮는 이미지 켜서 360도 돌리기
        hideImage.gameObject.SetActive(true);
        hideImage.fillAmount = 1.0f; // 처음은 360도 꽉 채워서 시작

        float cur = coolTime; // 쿨타임 40초로 시작해서
        while (cur > 0)
        {
            cur -= Time.deltaTime; // 시간 계속 깍기
            hideImage.fillAmount = cur / coolTime; // 360도에서 점점 돌기

            yield return null; // 1프레임 대기
        }

        // 쿨타임 종료 후 다시 버튼 활성화
        isCoolDown = false;
        killButton.interactable = true;
        hideImage.gameObject.SetActive(false);

        Debug.Log("쿨타임 종료.");
    }
}

    #endregion