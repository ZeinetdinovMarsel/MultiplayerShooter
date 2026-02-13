using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using Zenject;
using static PlayerInputManager;

public class NetworkShooter : MonoBehaviourPun
{
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _range = 100f;
    [SerializeField] private PistolBehaviour _pistolBehaviour;

    [Inject] private PlayerInputManager _playerInput;

    public UnityEvent<Vector3, Vector3> OnHit { get; private set; } = new();
    public UnityEvent<Vector3, Vector3> OnTrail { get; private set; } = new();

    private void Start()
    {
        if (!photonView.IsMine) return;
        _playerInput.OnShootEvent.AddListener(ShootAction);
        _playerInput.OnReloadEvent.AddListener(ReloadAction);
    }
    private void ShootAction(PressedStateEventArgs pressedState)
    {
        if (pressedState.State != PressedState.Started
            || !_pistolBehaviour.ReadyToShoot
            || _pistolBehaviour.IsReloading
            || !_pistolBehaviour.EnoughAmmo) return;
        Shoot();
    }
    private void ReloadAction(PressedStateEventArgs pressedState)
    {
        if (pressedState.State != PressedState.Started) return;
        _pistolBehaviour.ReloadGun().Forget();
    }

    private void Shoot()
    {
        Vector3 origin = _firePoint.position;
        Vector3 direction = _firePoint.forward;
        double shootTime = PhotonNetwork.Time;
        _pistolBehaviour.Shoot().Forget();
        photonView.RPC(nameof(RPC_ProcessShot),
            RpcTarget.MasterClient,
            origin,
            direction,
            shootTime);
    }

    [PunRPC]
    private void RPC_ProcessShot(Vector3 origin, Vector3 direction, double shotTime, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Vector3 hitPoint;
        Vector3 hitNormal = Vector3.zero;
        bool hasHit = false;

        if (Physics.Raycast(origin, direction, out var hit, _range))
        {
            hasHit = true;
            hitPoint = hit.point;
            hitNormal = hit.normal;

            if (hit.collider.TryGetComponent<Health>(out var health))
            {
                health.ApplyDamage(_pistolBehaviour.Damage, info.Sender.ActorNumber);
            }
        }
        else
        {
            hitPoint = origin + direction * _range;
        }

        photonView.RPC(nameof(RPC_PlayShotEffects),
            RpcTarget.All,
            origin,
            hitPoint,
            hitNormal,
            hasHit,
            shotTime);
    }

    [PunRPC]
    private void RPC_PlayShotEffects(
        Vector3 origin,
        Vector3 hitPoint,
        Vector3 hitNormal,
        bool hasHit,
        double shotTime)
    {
        PlayEffects(origin, hitPoint, hitNormal, hasHit, shotTime).Forget();
    }

    private async UniTask PlayEffects(
        Vector3 origin,
        Vector3 hitPoint,
        Vector3 hitNormal,
        bool hasHit,
        double shotTime)
    {
        double delay = Mathf.Max(0f, (float)(shotTime - PhotonNetwork.Time));
        await UniTask.WaitForSeconds((float)delay);

        OnTrail?.Invoke(origin, hitPoint);

        if (hasHit)
            OnHit?.Invoke(hitPoint, hitNormal);
    }
}
