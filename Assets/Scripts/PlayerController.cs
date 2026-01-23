using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviourPun
{
    [Header("플레이어 설정")]
    public float moveSpeed = 5f;

    [Header("컴포넌트 연결")]
    public Animator anim;
    public TextMeshProUGUI playerNameText;

    private Vector3 currentPos;

    void Start()
    {
        if (photonView.Owner != null)
        {
            playerNameText.text = photonView.Owner.NickName;
            playerNameText.color = Color.black;
        }

        // #region 맵 테스트용 임시 코드
        // if (photonView.IsMine)
        // {
        //     CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        //     if (cam != null)
        //     {
        //         cam.target = this.transform;
        //     }
        // }
        // #endregion
    }

    void Update()
    {   
        // 1.내 캐릭터 아니면 조종X
        if (!photonView.IsMine) return;

        // 2.게임 상태 체크
        if (GameStateManager.instance.currentState == GameState.Voting)
        {
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            return;
        }

        if  (photonView.IsMine)
        {
            ProcessInput();
        }
    }

    void ProcessInput()
    {
        float y = Input.GetAxisRaw("Vertical");
        float x = Input.GetAxisRaw("Horizontal");

        Vector3 moveDir = new Vector3(x, y, 0).normalized;

        transform.position += moveDir * moveSpeed * Time.deltaTime;
        UpdateAnimation(moveDir);

    }

    void UpdateAnimation (Vector3 moveDir)
    {
        if (moveDir.magnitude > 0)
        {
            anim.SetBool("IsWalking", true);
            anim.SetFloat("InputX", moveDir.x);
            anim.SetFloat("InputY", moveDir.y);
        }
        else
        {
            anim.SetBool("IsWalking", false);
        }
    }
}
