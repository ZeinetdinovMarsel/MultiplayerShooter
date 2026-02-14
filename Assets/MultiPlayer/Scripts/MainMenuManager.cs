using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using ExitGames.Client.Photon;

public class MainMenuManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField _nameInput;
    [SerializeField] private Button _playButton;
    [SerializeField] private TMP_Text _mmrText;

    [Inject] private RankedMatchmaking _rankedMatchmaking;

    private void Start()
    {
        string savedName = PlayerPrefs.GetString("PLAYER_NAME", "");
        _nameInput.text = savedName;

        _mmrText.text = "ММР: " + PlayerMMR.MMR;

        _playButton.onClick.AddListener(StartMatchmaking);
    }

    private void StartMatchmaking()
    {
        if (PhotonNetwork.InRoom) return;

        _playButton.interactable = false;

        string nickname = _nameInput.text;
        if (string.IsNullOrEmpty(nickname))
            nickname = "Игрок" + Random.Range(0, 9999);

        PhotonNetwork.NickName = nickname;
        PlayerPrefs.SetString("PLAYER_NAME", nickname);

        Hashtable hash = new Hashtable
        {
            { "MMR", PlayerMMR.MMR }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        _rankedMatchmaking.StartRankedSearch();
    }

	public override void OnConnectedToMaster()
	{
		base.OnConnectedToMaster();
		Hashtable hash = new Hashtable { { "MMR", PlayerMMR.MMR } };
		PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
	}
}
