using PrimeTween;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class WeaponRecoil : MonoBehaviour
{
    [SerializeField] private float _horizontalRecoilAmount = 1f;
    [SerializeField] private float _verticalRecoilAmount = 1f;
    [SerializeField] private float _recoilDuration = 0.2f;
    [SerializeField] private PistolBehaviour _pistol;
    [Inject(Id = "PlayerFPCamera")] private CinemachineCamera _playerCamera;
    private CinemachinePanTilt _cinemachinePanTilt;

    private void Start()
    {
        _cinemachinePanTilt = _playerCamera.GetComponent<CinemachinePanTilt>();
        _pistol.OnShoot.AddListener(AddRecoil);
    }
    private void AddRecoil()
    {
        float startPan = _cinemachinePanTilt.PanAxis.Value;
        float endPan = startPan + _horizontalRecoilAmount * Random.Range(-1f, 1f);

        float startTilt = _cinemachinePanTilt.TiltAxis.Value;
        float endTilt = startTilt - _verticalRecoilAmount * Random.Range(0f, 1f);

        Sequence.Create()
            .Group(Tween.Custom(startPan, endPan, _recoilDuration, (value) => { _cinemachinePanTilt.PanAxis.Value = value; }, Ease.OutBounce))
            .Group(Tween.Custom(startTilt, endTilt, _recoilDuration, (value) => { _cinemachinePanTilt.TiltAxis.Value = value; }, Ease.OutBounce));
    }
}
