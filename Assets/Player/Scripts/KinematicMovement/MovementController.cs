using UnityEngine;
using UnityEngine.EventSystems;
using static PlayerController;
[System.Serializable]
public class MovementController
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _maxYSpeed;
    [SerializeField] private float _walkSpeed = 5;
    [SerializeField] private float _sprintSpeed = 10;
    [SerializeField] private float _maxSlopeAngle = 10;
    [SerializeField] private float _airMultiplier = 0.5f;

    [SerializeField] private float _movementDrag = 10f;
    [SerializeField] private float _airDrag = 0f;

    public float MoveSpeed => _moveSpeed;
    public float WalkSpeed => _walkSpeed;
    public float SprintSpeed => _sprintSpeed;

    private Transform _playerTransform;
    private Rigidbody _rb;
    private PlayerController _controller;

    public bool ExitingSlope
    { get; set; }

    public void Init(
        PlayerController controller,
        Transform playerPos,
        Rigidbody rb
        )
    {
        SetRunSpeed(false);
        _controller = controller;
        _playerTransform = playerPos;
        _rb = rb;
    }

    public void SetRunSpeed(bool isRunning)
    {
        _moveSpeed = isRunning ? _sprintSpeed : _walkSpeed;
    }

    public void Move(Vector2 moveInput)
    {

        if (_controller.State == MovementState.Dashing) return;

        Vector3 moveDir = _playerTransform.forward * moveInput.y + _playerTransform.right * moveInput.x;

        moveDir.Normalize();

        bool isGrounded = _controller.IsGrounded;
        bool onSlope = OnSlope(ref isGrounded) && !ExitingSlope;


        Vector3 desiredVel = (onSlope ? GetSlopeMoveDirection(ref moveDir, _controller.GroundRayHit.normal) : moveDir) * _moveSpeed;
        Vector3 targetVelocity = desiredVel - (onSlope ? _rb.linearVelocity : new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z));

        if (isGrounded)
        {
            if (onSlope)
            {
                _rb.AddForce(targetVelocity, ForceMode.VelocityChange);


                _rb.AddForce(-_controller.GroundRayHit.normal * _moveSpeed, ForceMode.Acceleration);
            }
            else
            {
                _rb.AddForce(targetVelocity, ForceMode.VelocityChange);
            }

            _rb.linearDamping = _movementDrag;
        }
        else
        {
            _rb.AddForce(targetVelocity * _airMultiplier, ForceMode.VelocityChange);
            _rb.linearDamping = _airDrag;
        }

        _rb.useGravity = !onSlope;
        SpeedControl(ref onSlope);
    }

    public void SpeedControl(ref bool onSlope)
    {
        if (onSlope)
        {
            if (_rb.linearVelocity.magnitude > _moveSpeed)
                _rb.linearVelocity = _rb.linearVelocity.normalized * _moveSpeed;
        }
        else
        {
            Vector3 flatVel = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            if (flatVel.magnitude > _moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * _moveSpeed;
                limitedVel.y = _rb.linearVelocity.y;
                _rb.linearVelocity = limitedVel;
            }
        }

        if (_maxYSpeed != 0 && _rb.linearVelocity.y > _maxYSpeed)
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, _maxYSpeed, _rb.linearVelocity.z);
    }

    private bool OnSlope(ref bool isGrounded)
    {
        if (isGrounded)
        {
            float angle = Vector3.Angle(Vector3.up, _controller.GroundRayHit.normal);
            return angle < _maxSlopeAngle && angle != 0;
        }

        return false;
    }
    private Vector3 GetSlopeMoveDirection(ref Vector3 moveDir, Vector3 slopeHitNormal)
    {
        return Vector3.ProjectOnPlane(moveDir, slopeHitNormal).normalized;
    }

}
