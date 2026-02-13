using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RankedGameManager : MonoBehaviourPunCallbacks
{
    public void EndMatch(Player loser)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Player winner = PhotonNetwork.PlayerListOthers[0];

        int mmrLoser = loser.CustomProperties.ContainsKey("MMR") ? (int)loser.CustomProperties["MMR"] : 0;
        int mmrWinner = winner.CustomProperties.ContainsKey("MMR") ? (int)winner.CustomProperties["MMR"] : 0;

        int newWinnerMMR = MMRSystem.Calculate(mmrWinner, mmrLoser, true);
        int newLoserMMR = MMRSystem.Calculate(mmrLoser, mmrWinner, false);

        Hashtable winnerProps = new Hashtable { ["MMR"] = newWinnerMMR };
        Hashtable loserProps = new Hashtable { ["MMR"] = newLoserMMR };

        winner.SetCustomProperties(winnerProps);
        loser.SetCustomProperties(loserProps);

        photonView.RPC(nameof(RPC_UpdateMMRAndReturn), winner, newWinnerMMR);
        photonView.RPC(nameof(RPC_UpdateMMRAndReturn), loser, newLoserMMR);
    }


    [PunRPC]
    private void RPC_UpdateMMRAndReturn(int newMMR)
    {
        PlayerMMR.Save(newMMR);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("MainMenu");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {

        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Player remaining = PhotonNetwork.LocalPlayer;
            int mmrWinner = PlayerMMR.MMR;
            int mmrLoser = otherPlayer.CustomProperties.ContainsKey("MMR")
                ? (int)otherPlayer.CustomProperties["MMR"]
                : mmrWinner;

            int newWinnerMMR = MMRSystem.Calculate(mmrWinner, mmrLoser, true);
            RPC_UpdateMMRAndReturn(newWinnerMMR);
        }
    }

}
