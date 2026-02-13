using Cysharp.Threading.Tasks;
using PrimeTween;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class PistolBehaviour : MonoBehaviour
{
    [SerializeField] private float _maxAmmo = 7;
    [SerializeField] private float _currAmmo = 0;
    [SerializeField] private float _shootDelay = 0;
    [SerializeField] private float _reloadTime = 1;
    [SerializeField] private int _damage = 25;
    public float MaxAmmo => _maxAmmo;
    public float CurrAmmo => _currAmmo;
    public float ReloadTime => _reloadTime;
    public int Damage => _damage;

    public bool ReadyToShoot { get; private set; } = true;
    public bool IsReloading { get; private set; } = false;
    public bool EnoughAmmo => _currAmmo > 0;
    public UnityEvent OnShoot { get; private set; } = new();
    public UnityEvent OnReloadStart { get; private set; } = new();
    public UnityEvent OnReloadEnd { get; private set; } = new();

    private CancellationTokenSource _shootCancelationSource;
    private void Awake()
    {
        _currAmmo = _maxAmmo;
        _shootCancelationSource = new();
    }

    public async UniTask Shoot()
    {
        if (!ReadyToShoot) return;
        ReadyToShoot = false;
        _currAmmo = Mathf.Clamp(--_currAmmo, 0, _maxAmmo);
        OnShoot?.Invoke();
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_shootDelay),
                cancellationToken: _shootCancelationSource.Token);
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            ReadyToShoot = true;
        }
    }

    public async UniTask ReloadGun()
    {
        if (IsReloading) return;
        OnReloadStart?.Invoke();
        _shootCancelationSource?.Cancel();
        _shootCancelationSource?.Dispose();
        _shootCancelationSource = new CancellationTokenSource();
        IsReloading = true;
        ReadyToShoot = false;
        await UniTask.WaitUntil(() => !IsReloading);
        _currAmmo = _maxAmmo;
        OnReloadEnd?.Invoke();
        ReadyToShoot = true;
    }

    private void OnDestroy()
    {
        _shootCancelationSource?.Cancel();
        _shootCancelationSource?.Dispose();
    }

    public void StopReload()
    {
        IsReloading = false;
    }
}
