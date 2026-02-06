using Photon.Pun;
using UnityEngine;

public class Health : MonoBehaviourPun
{
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _health;

    void Awake() => _health = _maxHealth;

    public void ApplyDamage(int damage, int attackerId)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        _health -= damage;
        if (_health <= 0)
        {
            photonView.RPC(nameof(RPC_Destroy), photonView.Owner);
        }

        photonView.RPC(nameof(RPC_SyncHealth), RpcTarget.All, _health);
    }

    [PunRPC]
    void RPC_SyncHealth(int hp)
    {
        _health = hp;
    }

    [PunRPC]
    void RPC_Destroy()
    {
        PhotonNetwork.Destroy(gameObject);
    }


    void Die()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(RPC_Destroy), photonView.Owner);
        }
    }


}
