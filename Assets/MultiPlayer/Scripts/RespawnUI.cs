using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Zenject;

public class RespawnUI : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private Button _respawnButton;
    [Inject] private Health _playerHealth;
    private PhotonView _localPlayerView;

    private void Awake()
    {
        _panel.SetActive(false);
        _respawnButton.onClick.AddListener(OnRespawnClicked);
    }

    private void OnEnable()
    {
        _playerHealth.OnPlayerDeath.AddListener( HandlePlayerDeath);
    }

    private void OnDisable()
    {
        _playerHealth.OnPlayerDeath.RemoveListener(HandlePlayerDeath);
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
        _localPlayerView.RPC(nameof(RoomManager.RPC_RequestRespawn), RpcTarget.MasterClient);
        _panel.SetActive(false);
    }
}
