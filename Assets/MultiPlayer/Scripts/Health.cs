using Photon.Pun;
using UnityEngine;
using System;
using UnityEngine.Events;

public class Health : MonoBehaviourPun
{
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _health;

    public UnityEvent<PhotonView> OnPlayerDeath { get; private set; } = new();
    public UnityEvent<PhotonView> OnPlayerRespawn { get; private set; } = new();

    void Awake() => _health = _maxHealth;

    public void ApplyDamage(int damage, int attackerId)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        _health -= damage;

        if (_health <= 0)
        {
            _health = 0;
            if (photonView.IsMine)
            {
                OnPlayerDeath?.Invoke(photonView);
            }
            photonView.RPC(nameof(RPC_SetActive), RpcTarget.All, false);
        }


        photonView.RPC(nameof(RPC_SyncHealth), RpcTarget.All, _health);

    }

    [PunRPC]
    public void RPC_Respawn(Vector3 position, Quaternion rotation)
    {
        _health = _maxHealth;
        transform.position = position;
        transform.rotation = rotation;

        photonView.RPC(nameof(RPC_SetActive), RpcTarget.All, true);

        OnPlayerRespawn?.Invoke(photonView);

        photonView.RPC(nameof(RPC_SyncHealth), RpcTarget.All, _health);
    }

    [PunRPC]
    public void RPC_RequestRespawnFromMaster(int viewID, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        PhotonView playerView = PhotonView.Find(viewID);
        if (playerView != null)
        {
            Vector3 spawnPos = new Vector3(0, 1, 0);
            Quaternion spawnRot = Quaternion.identity;
            playerView.RPC("RPC_Respawn", playerView.Owner, spawnPos, spawnRot);
        }
    }

    [PunRPC]
    void RPC_SyncHealth(int hp)
    {
        _health = hp;
        if (_health <= 0 && photonView.IsMine)
        {
            OnPlayerDeath?.Invoke(photonView);
        }
    }

    [PunRPC]
    void RPC_SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

}
