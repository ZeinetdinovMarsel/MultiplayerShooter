using Photon.Pun;
using UnityEngine;
using Zenject;

public class PlayerStats : MonoBehaviourPun
{
    private int _deaths = 0;
    private const int MAX_DEATHS = 3;

    [SerializeField] private Health _health;
    private RankedGameManager _rankedGameManager;

    public void Start()
    {
        _rankedGameManager = FindAnyObjectByType<RankedGameManager>();
    }
    private void OnEnable()
    {
        _health.OnPlayerDeath.AddListener(OnDeath);
    }

    private void OnDisable()
    {
        _health.OnPlayerDeath.RemoveListener(OnDeath);
    }

    private void OnDeath(PhotonView view)
    {
        if (!view.IsMine) return;

        _deaths++;

        if (_deaths >= MAX_DEATHS)
        {
            photonView.RPC(nameof(RPC_ReportLossToMaster), RpcTarget.MasterClient, photonView.Owner);
        }
    }

    [PunRPC]
    private void RPC_ReportLossToMaster(Photon.Realtime.Player loser)
    {

        _rankedGameManager.EndMatch(loser);

    }
}
