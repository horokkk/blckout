using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class GameStateManager : MonoBehaviourPunCallbacks, IPunObservable
{   
    #region 인스펙터창 설정 변수
    [Header("UI 연결")]
    public TextMeshProUGUI timerText; //투표 타이머
    public TextMeshProUGUI totalGameTimeText; //전체 게임 타이머
    public GameObject votingPanel;
    public Light globalLight;

    [Header("설정")]
    public float gameTime = 1800.0f;
    public float blackoutDelay = 30.0f;
    public float votingTime = 120.0f;
    #endregion

    #region 내부 변수

    //상태변수
    private GameState currentState = GameState.Playing_OnLight;
    private float currentGameTime;

    //투표용 변수
    private double votingEndTime;
    #endregion
    
    void Start()
    {
        #region 테스트용 코드 후에 지울 것

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinOrCreateRoom("TestRoom", new RoomOptions { MaxPlayers = 4}, TypedLobby.Default);
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        #endregion

        currentGameTime = gameTime;
        if(votingPanel != null) votingPanel.SetActive(false);
        UpdateLightState();
    }

    #region 테스트용 코드 후에 지울 것
    public override void OnConnectedToMaster()
    {
        Debug.Log("서버 접속 성공. 방 참가를 시도합니다");
        PhotonNetwork.JoinOrCreateRoom("TestRoom",new RoomOptions {MaxPlayers = 4}, TypedLobby.Default);
    }
    #endregion

    void Update()
    {   
        //방장 책임: 시간 관리 & 상태 자동 변경
        if (PhotonNetwork.IsMasterClient)
        {
            if (currentState == GameState.Playing_OnLight || currentState == GameState.Playing_OffLight)
            {
                currentGameTime -= Time.deltaTime;

                //게임 시작 30초 후 암전
                float timeElapsed = gameTime - currentGameTime;
                if (currentState == GameState.Playing_OnLight && timeElapsed >= blackoutDelay)
                {
                    Debug.Log("30초 경과 [암전]");
                    photonView.RPC("RPC_SetGameState", RpcTarget.All, GameState.Playing_OffLight,0.0);
                }

                //게임 종료
                if (currentGameTime <= 0)
                {
                    currentGameTime = 0;
                }
            }

            int min = (int)(currentGameTime / 60);
            int sec = (int) (currentGameTime % 60);

            if(totalGameTimeText != null) totalGameTimeText.text = string.Format("{0:00}:{1:00}", min, sec);

            //투표 로직
            if (currentState == GameState.Voting)
            {
                double timeRemaining = votingEndTime - PhotonNetwork.Time;
                if (timeRemaining > 0) timerText.text = ((int)timeRemaining).ToString();
                else timerText.text = "0";

                if (PhotonNetwork.IsMasterClient && PhotonNetwork.Time >= votingEndTime)
                {
                    EndVoting();
                }
            }

            if (PhotonNetwork.IsMasterClient && Input.GetKeyDown(KeyCode.M))
            {
                StartMeeting();
            }  
        }
    }


    //회의 시작&종료 로직
    public void StartMeeting()
    {
        if (currentState == GameState.Voting) return;
        double endTime = PhotonNetwork.Time + votingTime;
        photonView.RPC("RPC_SetGameState", RpcTarget.All, GameState.Voting, endTime);
    }

    [PunRPC]
    void RPC_SetGameState(GameState newState, double endTime)
    {
        currentState = newState;
        votingEndTime = endTime;

        switch (newState)
        {
            case GameState.Playing_OnLight:
            case GameState.Playing_OffLight:
                if (votingPanel != null) votingPanel.SetActive(false);
                timerText.text = "";
                UpdateLightState();
                break;
            case GameState.Voting:
                if (votingPanel != null) votingPanel.SetActive(true);
                globalLight.intensity = 1.0f;
                break;    
        }
    }

    void UpdateLightState()
    {
        if (globalLight == null) return;

        if (currentState == GameState.Playing_OnLight)
        {
            globalLight.intensity = 1.0f;
            globalLight.color = Color.white;
        }
        else if (currentState == GameState.Playing_OffLight)
        {
            globalLight.intensity = 0.2f;
            globalLight.color = Color.darkGray;
        }
    }

    void EndVoting()
    {
        float timeElapsed = gameTime - currentGameTime;

        GameState nextState;
        if (timeElapsed >= blackoutDelay) nextState = GameState.Playing_OffLight;
        else nextState = GameState.Playing_OnLight;

        photonView.RPC("RPC_SetGameState", RpcTarget.All,nextState,0.0);
    }

    public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentGameTime);
            stream.SendNext(currentState);
        }
        else
        {
            currentGameTime = (float)stream.ReceiveNext();
            GameState receivedState = (GameState)stream.ReceiveNext();

            if (currentState != receivedState)
            {
                currentState = receivedState;
                UpdateLightState();
            }
        }
    }
}
