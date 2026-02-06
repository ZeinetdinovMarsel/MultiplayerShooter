using PrimeTween;
using System.Threading;
using Unity.Cinemachine;
using UnityEngine;

public enum FOVState
{
    Slow,
    Medium,
    Fast
}
[System.Serializable]
public class CameraController
{
    [SerializeField] private float _fastFOV = 90;
    [SerializeField] private float _mediumFOV = 60;
    [SerializeField] private float _slowFOV = 40;
    [SerializeField] private float _fOVChangeDuration;
    private Transform _playerTransform;
    private CinemachineCamera _playerCamera;
    private Tween _changeFOVTween;
    private Rigidbody _rb;

    public void Init(
        Transform playerTransform,
        Rigidbody rb,
        CinemachineCamera playerCamera)
    {
        _playerTransform = playerTransform;
        _rb = rb;
        _playerCamera = playerCamera;
    }


    public void TryToSyncOrientation()
    {
        float yRotation = _playerCamera.transform.rotation.eulerAngles.y;
        Vector3 initRotation = _playerTransform.rotation.eulerAngles;
        initRotation.y = yRotation;
        _playerTransform.rotation = Quaternion.Euler(initRotation);
    }

    public void SetFov(FOVState fOVState)
    {
        switch (fOVState)
        {
            case FOVState.Slow:
                ChangeFovTween(_playerCamera.Lens.FieldOfView, ref _slowFOV);
                break;
            case FOVState.Medium:
                ChangeFovTween( _playerCamera.Lens.FieldOfView, ref _mediumFOV);
                break;
            case FOVState.Fast:
                ChangeFovTween(_playerCamera.Lens.FieldOfView, ref _fastFOV);
                break;
        }
    }

    private void ChangeFovTween( float initialFOV, ref float targetFOV)
    {
        _changeFOVTween.Stop();
        _changeFOVTween = Tween.Custom(initialFOV, targetFOV, _fOVChangeDuration,
            onValueChange: value => _playerCamera.Lens.FieldOfView = value,
            ease: Ease.InOutQuad);
    }
}
