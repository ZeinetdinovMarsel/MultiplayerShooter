using Photon.Pun;
using Unity.Cinemachine;
using UnityEngine;

public class LocalPlayerBehaviour : MonoBehaviour
{
    [SerializeField] private LocalPlayerItems _localPlayerItems;

    public void IsLocalPlayer()
    {
        _localPlayerItems.RespawnSetup();
        _localPlayerItems.gameObject.SetActive(true);
    }
}
