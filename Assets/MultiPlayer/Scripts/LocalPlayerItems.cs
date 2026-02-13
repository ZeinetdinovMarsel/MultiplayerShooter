using Photon.Pun;
using Unity.Cinemachine;
using UnityEngine;

public class LocalPlayerItems : MonoBehaviour
{
    [SerializeField] private Camera _mainCam;
    [SerializeField] private CinemachineCamera _cinemachineCam;
    [SerializeField] private GameObject _inputManager;
    [SerializeField] private Canvas _playerCanvas;

    public void RespawnSetup()
    {
        transform.SetParent(null);
        _mainCam.gameObject.SetActive(true);
        _cinemachineCam.gameObject.SetActive(true);
        _inputManager.gameObject.SetActive(true);
        _playerCanvas.gameObject.SetActive(true);
    }

}
