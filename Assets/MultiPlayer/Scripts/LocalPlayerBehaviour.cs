using Unity.Cinemachine;
using UnityEngine;

public class LocalPlayerBehaviour : MonoBehaviour
{
    [SerializeField] private Camera _mainCam;
    [SerializeField] private CinemachineCamera _cinemachineCam;
    [SerializeField] private GameObject _inputManager;

    public void IsLocalPlayer()
    {
        _mainCam.gameObject.SetActive(true);
        _mainCam.transform.SetParent(null);
        _cinemachineCam.gameObject.SetActive(true);
        _cinemachineCam.transform.SetParent(null);
        _inputManager.gameObject.SetActive(true);

    }
}
