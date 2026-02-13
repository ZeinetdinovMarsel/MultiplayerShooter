using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine;

public class PlayerMMR : MonoBehaviourPunCallbacks
{
    public static int MMR { get; private set; }

    private const string KEY = "PLAYER_MMR";

    private void Awake()
    {
        MMR = PlayerPrefs.GetInt(KEY, 1000);
    }

    public static void Save(int newMMR)
    {
        MMR = newMMR;
        PlayerPrefs.SetInt(KEY, MMR);
        PlayerPrefs.Save();

        Hashtable hash = new Hashtable { { "MMR", MMR } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public override void OnJoinedRoom()
    {
        Hashtable hash = new Hashtable { { "MMR", MMR } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }
}
