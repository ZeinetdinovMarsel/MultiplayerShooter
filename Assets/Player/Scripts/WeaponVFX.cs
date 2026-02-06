using Photon.Pun;
using PrimeTween;
using UnityEngine;

public class WeaponVFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem _muzzleFlash;
    [SerializeField] private ParticleSystem _hitPrefab;
    [SerializeField] private NetworkShooter _shooter;
    [SerializeField] private TrailRenderer _trailPrefab;

    private void OnEnable()
    {
        if (_shooter != null)
        {
            _shooter.OnShoot.AddListener(PlayMuzzleFlash);
            _shooter.OnHit.AddListener(PlayHit);
            _shooter.OnTrailSend.AddListener(PlayTrailEffect);
        }
    }

    private void OnDisable()
    {
        if (_shooter != null)
        {
            _shooter.OnShoot.RemoveListener(PlayMuzzleFlash);
            _shooter.OnHit.RemoveListener(PlayHit);
            _shooter.OnTrailSend.RemoveListener(PlayTrailEffect);
        }
    }
    private void PlayHit(Vector3 point, Vector3 normal)
    {
        if (_hitPrefab != null)
        {
            Quaternion rotation = Quaternion.LookRotation(normal);

            var hitEffect = Instantiate(_hitPrefab, point, rotation);
            hitEffect.Play();

            Destroy(hitEffect.gameObject, 1f);
        }
    }


    private void PlayMuzzleFlash()
    {
        if (_muzzleFlash != null)
            _muzzleFlash.Play();
    }

    private void PlayTrailEffect(Vector3 firePoint, Vector3 hitPoint)
    {
        if (_trailPrefab != null)
        {
            Vector3 toEnd = hitPoint - firePoint;

            var trail = Instantiate(_trailPrefab, firePoint, Quaternion.LookRotation(toEnd.normalized));

            Tween.Position(trail.transform,startValue:firePoint ,endValue: hitPoint, duration: toEnd.magnitude / 600f).OnComplete(() => Destroy(trail.gameObject));
        }
    }
}
