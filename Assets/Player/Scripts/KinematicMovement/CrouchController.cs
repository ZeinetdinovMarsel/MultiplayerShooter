using PrimeTween;
using System.Threading;
using UnityEngine;
[System.Serializable]
public class CrouchController
{
    [SerializeField] private float _crouchDuration = 0.5f;
    [SerializeField] private float _crouchHeight = 1;

    private PlayerController _controller;
    private Transform _playerTransform;
    private Transform _playerModelTransform;
    private Rigidbody _rb;
    private CapsuleCollider _playerCollider;
    private bool _isCrouched = false;
    private float _initialTransformY;
    private float _initialColliderHeight;
    public void Init(
        PlayerController controller,
        Transform playerTransform,
        Rigidbody rb,
        CapsuleCollider playerCollider)
    {
        _controller = controller;
        _playerTransform = playerTransform;

        _playerModelTransform = _playerTransform.GetChild(0);
        _initialTransformY = _playerModelTransform.localScale.y;
        _rb = rb;
        _playerCollider = playerCollider;
        _initialColliderHeight = _playerCollider.height;
    }

    public void StartCrouch()
    {
        //if (_isCrouched) return;
        Tween.CompleteAll(_playerModelTransform);
        Sequence.Create()
            .Group(Tween.Custom(_playerCollider.height, _crouchHeight, _crouchDuration, (value) =>
            {
                _playerCollider.height = value;
                _rb.AddForce(Vector3.down, ForceMode.Acceleration);
            }))
            .OnComplete(() => _isCrouched = true);
    }

    public void StartStandUp()
    {
        //if (!_isCrouched) return;
        Tween.CompleteAll(_playerModelTransform);
        Sequence.Create()
            .Group(Tween.Custom(_playerCollider.height, _initialColliderHeight, _crouchDuration, (value) =>
            {
                _playerCollider.height = value;
            }))
            .OnComplete(() => _isCrouched = false);   
    }


}
