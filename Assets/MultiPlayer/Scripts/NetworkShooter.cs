using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Zenject;
using static PlayerInputManager;

public class NetworkShooter : MonoBehaviourPun
{
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _range = 100f;
    [SerializeField] private int _damage = 25;

    [Inject] private PlayerInputManager _playerInput;
    private PhotonView _photonView;

    void Awake() => _photonView = GetComponent<PhotonView>();

    public UnityEvent OnShoot { get; private set; } = new();
    public UnityEvent<Vector3, Vector3> OnHit { get; private set; } = new();
    public UnityEvent<Vector3, Vector3> OnTrailSend { get; private set; } = new();
    private void Start()
    {
        if (!photonView.IsMine) return;
        _playerInput.OnShootEvent.AddListener(ShootAction);
    }

    void ShootAction(PressedStateEventArgs pressedState)
    {
        if (!photonView.IsMine || pressedState.State != PressedState.Started) return;
        Shoot();
    }

    void Shoot()
    {
        double shootTime = PhotonNetwork.Time;

        photonView.RPC(nameof(RPC_Shoot), RpcTarget.MasterClient,
            _firePoint.position,
            _firePoint.forward,
            shootTime);

        photonView.RPC(nameof(RPC_PlayShootAnimation), RpcTarget.Others, shootTime);
    }

    [PunRPC]
    void RPC_PlayShootAnimation(double shootTime)
    {
        PlayShootWithDelay(shootTime).Forget();
    }

    private async UniTask PlayShootWithDelay(double shotTime)
    {
        double delay = Mathf.Max(0f, (float)(shotTime - PhotonNetwork.Time));
        await UniTask.WaitForSeconds((float)delay);

        OnShoot?.Invoke();
    }



    [PunRPC]
    void RPC_Shoot(Vector3 origin, Vector3 direction, double shotTime, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (Physics.Raycast(origin, direction, out var hit, _range))
        {
            photonView.RPC(nameof(RPC_PlayHitEffect), RpcTarget.All, hit.point, hit.normal, shotTime);

            photonView.RPC(nameof(RPC_SpawnBulletTrail), RpcTarget.All, _firePoint.position, hit.point, shotTime);
            if (hit.collider.TryGetComponent<Health>(out var health) && health != null)
            {
                health.ApplyDamage(_damage, info.Sender.ActorNumber);
            }
        }
        else
        {

            photonView.RPC(nameof(RPC_SpawnBulletTrail), RpcTarget.All, _firePoint.position, _firePoint.position + _firePoint.forward * _range, shotTime);
        }
    }

    [PunRPC]
    void RPC_PlayHitEffect(Vector3 hitPoint, Vector3 hitNormal, double shotTime)
    {
        PlayHitEffect(hitPoint, hitNormal, shotTime).Forget();
    }

    private async UniTask PlayHitEffect(Vector3 hitPoint, Vector3 hitNormal, double shotTime)
    {
        double delay = Mathf.Max(0f, (float)(shotTime - PhotonNetwork.Time));
        await UniTask.WaitForSeconds((float)delay);

        OnHit?.Invoke(hitPoint, hitNormal);
    }

    [PunRPC]
    void RPC_SpawnBulletTrail(Vector3 firePoint, Vector3 hitPoint, double shotTime)
    {
        SendTrail(firePoint, hitPoint, shotTime).Forget();
    }

    private async UniTask SendTrail(Vector3 firePoint, Vector3 hitPoint, double shotTime)
    {
        double delay = Mathf.Max(0f, (float)(shotTime - PhotonNetwork.Time));
        await UniTask.WaitForSeconds((float)delay);

        OnTrailSend?.Invoke(firePoint, hitPoint);
    }
}

