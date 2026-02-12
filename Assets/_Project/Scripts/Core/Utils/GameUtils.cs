using Photon.Pun;
using UnityEngine;

public static class GameUtils
{
    // 내 플레이어의 생존 여부 파악용 전역 함수
    public static bool IsMyPlayerDead
    {
        get
        {
            // 1. 포톤 연결 안 됐으면 살아있는 걸로 처리
            if (PhotonNetwork.LocalPlayer == null) return false;

            // 2. 프로퍼티 검사
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("IsDead", out object isDead))
            {
                return (bool)isDead;
            }

            // 3. 정보 없으면 살아있는 걸로 처리
            return false;
        }
    }

    public static bool IsMyPlayerKiller
    {
        get
        {
            // 1. 포톤 연결 안 됐으면 생존자로 처리
            if (PhotonNetwork.LocalPlayer == null) return false;

            // 2. 프로퍼티 검사
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Job"))
            {
                string job = (string)PhotonNetwork.LocalPlayer.CustomProperties["Job"];

                if (job == "Killer") return true;
            }

            // 3. 정보 없으면 생존자로 처리
            return false;
        }
    }
}