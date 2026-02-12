using Photon.Pun;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;
using static PlayerInputManager;

public class PlayerController : MonoBehaviourPun
{
    [Inject] private PlayerInputManager _playerInput;
    [Inject(Id = "PlayerTransform")] private Transform _playerTransform;
    [Inject(Id = "PlayerFPCamera")] private CinemachineCamera _playerCamera;

    [SerializeField] private float _groundCheckRadius = 1;
    [SerializeField] private float _groundCheckTopPadding = 0;
    [SerializeField] private float _groundCheckBottomPadding = 0;
    [SerializeField] private LayerMask _groundMask;

    private Rigidbody _rb;
    [SerializeField] private MovementController _movementController;
    [SerializeField] private JumpController _jumpController;
    [SerializeField] private CrouchController _crouchController;
    [SerializeField] private CameraController _cameraController;

    private Collider _playerCollider;
    public Collider PlayerCollider
    {
        get
        {
            if (_playerCollider == null)
            {
                _playerCollider = GetComponent<Collider>();
            }
            return _playerCollider;
        }
    }
    public MovementState State;
    public enum MovementState
    {
        Walking,
        Sprinting,
        Crouching,
        Dashing,
        Air
    }

    public bool IsGrounded => Physics.SphereCast(
         transform.position + Vector3.down * _groundCheckTopPadding,
        _groundCheckRadius,
        Vector3.down,
        out _groundRayHit,
        PlayerCollider.bounds.size.y * 0.5f + _groundCheckBottomPadding,
        _groundMask);

    private RaycastHit _groundRayHit;
    public RaycastHit GroundRayHit => _groundRayHit;

    private void Start()
    {
        if (!photonView.IsMine)
        {
            enabled = false;
            return;
        }
        _rb = GetComponent<Rigidbody>();
        _playerCollider = GetComponent<Collider>();
        GetComponent<LocalPlayerBehaviour>().IsLocalPlayer();
        _movementController.Init(this, _playerTransform, _rb);
        _jumpController.Init(this, _playerTransform, _rb);
        _jumpController.OnJump.AddListener((value) => _movementController.ExitingSlope = value);
        _crouchController.Init(this, _playerTransform, _rb, _playerCollider as CapsuleCollider);
        _cameraController.Init(_playerTransform, _rb, _playerCamera);

        _playerInput.OnRunEvent.AddListener(PlayerMoveFOV);
        _playerInput.OnRunEvent.AddListener(PlayerRun);
        _playerInput.OnJumpEvent.AddListener(PlayerJump);
        _playerInput.OnCrouchEvent.AddListener(PlayerCrouch);
    }



    private void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        _movementController.Move(_playerInput.ContiniousActions.MoveInput);
        _cameraController.TryToSyncOrientation();
    }
    private void PlayerRun(PressedStateEventArgs pressedState)
    {
        _movementController.SetRunSpeed(pressedState.State == PressedState.Performed);
    }

    private void PlayerJump(PressedStateEventArgs pressedState)
    {
        _jumpController.TryJump(pressedState.State);
    }

    private void PlayerCrouch(PressedStateEventArgs pressedState)
    {
        if (pressedState.State == PressedState.Started)
        {
            _crouchController.StartCrouch();
        }
        else if (pressedState.State == PressedState.Canceled)
        {
            _crouchController.StartStandUp();
        }
    }

    private void PlayerMoveFOV(PressedStateEventArgs pressedState)
    {
        if (pressedState.State == PressedState.Started && _rb.linearVelocity.magnitude > _movementController.WalkSpeed)
        {
            _cameraController.SetFov(FOVState.Fast);
        }
        else if (pressedState.State == PressedState.Canceled)
        {
            _cameraController.SetFov(FOVState.Medium);
        }
    }

    private void OnDestroy()
    {
        _jumpController?.Dispose();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        float rayLength = PlayerCollider.bounds.size.y * 0.5f + _groundCheckBottomPadding;
        Vector3 initPos = transform.position + Vector3.down * _groundCheckTopPadding;


        Color sphereColor = new(255, 0, 0, 100);
        Gizmos.color = sphereColor;
        for (int i = 0; i < 10; i++)
        {
            Gizmos.DrawSphere(initPos + i * (Vector3.down * rayLength) / 10, _groundCheckRadius);
        }

    }
#endif
}
