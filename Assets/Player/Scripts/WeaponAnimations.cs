using UnityEngine;

public class WeaponAnimations : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private NetworkShooter _shooter;

    private void OnEnable()
    {
        if (_shooter != null)
            _shooter.OnShoot.AddListener(PlayShoot);
    }

    private void OnDisable()
    {
        if (_shooter != null)
            _shooter.OnShoot.RemoveListener(PlayShoot);
    }

    private void PlayShoot()
    {
        _animator?.SetTrigger("Shoot");
    }
}
