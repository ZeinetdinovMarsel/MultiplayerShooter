using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RankedGameManager : MonoBehaviourPun
{
    public void EndMatch(bool localPlayerWon)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        Player player1 = PhotonNetwork.PlayerList[0];
        Player player2 = PhotonNetwork.PlayerList[1];

        int mmr1 = (int)player1.CustomProperties["MMR"];
        int mmr2 = (int)player2.CustomProperties["MMR"];

        int newMMR1 = MMRSystem.Calculate(mmr1, mmr2, localPlayerWon);
        int newMMR2 = MMRSystem.Calculate(mmr2, mmr1, !localPlayerWon);

        photonView.RPC(nameof(RPC_UpdateMMR), player1, newMMR1);
        photonView.RPC(nameof(RPC_UpdateMMR), player2, newMMR2);
    }

    [PunRPC]
    private void RPC_UpdateMMR(int newMMR)
    {
        PlayerMMR.Save(newMMR);
        Debug.Log("New MMR: " + newMMR);
    }
}
