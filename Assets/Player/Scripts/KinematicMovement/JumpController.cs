using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using static PlayerInputManager;

[System.Serializable]
public class JumpController : IDisposable
{
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _jumpCooldown;

    private Transform _playerTransform;
    private Rigidbody _rb;
    private PlayerController _controller;
    private CancellationTokenSource _cts;
    private bool _isJumping;
    public bool IsJumping

    {
        get => _isJumping;
        private set
        {
            _isJumping = value;
            OnJump?.Invoke(_isJumping);
        }
    }
    public UnityEvent<bool> OnJump { get; private set; } = new();
    public void Init(
        PlayerController controller,
        Transform playerTransform,
        Rigidbody rb)
    {
        _controller = controller;
        _playerTransform = playerTransform;
        _rb = rb;
        _cts = new CancellationTokenSource();
    }

    public void Dispose()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }

    public void TryJump(PressedState pressedState)
    {
         if (pressedState != PressedState.Canceled && !IsJumping && _controller.IsGrounded)
        {
            JumpAsync().Forget();
        }
    }

    private async UniTaskVoid JumpAsync()
    {
        IsJumping = true;
        await UniTask.DelayFrame(1);
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
        _rb.AddForce(_playerTransform.up * _jumpForce, ForceMode.Impulse);

        try
        {
            await UniTask.Delay(
                TimeSpan.FromSeconds(_jumpCooldown),
                cancellationToken: _cts.Token);
        }
        catch (OperationCanceledException) { return; }
        finally
        {
            IsJumping = false;
        }
    }
}