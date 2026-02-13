using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Zenject;
using UnityEngine.SceneManagement;

public class RespawnUI : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private Button _respawnButton;
    [SerializeField] private Button _menuButton;
    [Inject] private Health _playerHealth;
    private PhotonView _localPlayerView;

    private void Start()
    {
        _respawnButton.onClick.AddListener(OnRespawnClicked);
        _menuButton.onClick.AddListener(() =>
        {
           SceneManager.LoadScene("MainMenu");
        });
       
    }

    [Inject]
    private void Construct()
    {
        _playerHealth.OnPlayerDeath.AddListener(HandlePlayerDeath);
    }


    private void HandlePlayerDeath(PhotonView playerView)
    {
        if (playerView.IsMine)
        {
            _localPlayerView = playerView;
            _panel.SetActive(true);
        }
    }

    private void OnRespawnClicked()
    {
        if (_localPlayerView == null) return;

        if (PhotonNetwork.IsMasterClient)
        {

            Vector3 spawnPos = new Vector3(0, 1, 0);
            Quaternion spawnRot = Quaternion.identity;
            _localPlayerView.RPC("RPC_Respawn", _localPlayerView.Owner, spawnPos, spawnRot);
        }
        else
        {
            _localPlayerView.RPC("RPC_RequestRespawnFromMaster", RpcTarget.MasterClient, _localPlayerView.ViewID);
        }

        _panel.SetActive(false);
    }


}
