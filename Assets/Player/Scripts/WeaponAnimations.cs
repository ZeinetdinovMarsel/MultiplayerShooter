using UnityEngine;

public class WeaponAnimations : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private PistolBehaviour _pistol;

    private void OnEnable()
    {
        if (_pistol != null)
        {
            _pistol.OnShoot.AddListener(PlayShoot);
            _pistol.OnReloadStart.AddListener(StartReload);
        }
    }

    private void OnDisable()
    {
        if (_pistol != null)
        {
            _pistol.OnShoot.RemoveListener(PlayShoot);
            _pistol.OnReloadStart.RemoveListener(StartReload);
        }
    }

    private void PlayShoot()
    {
        _animator?.ResetTrigger("Shoot");
        _animator?.SetTrigger("Shoot");
    }

    private void StartReload()
    {
        _animator?.ResetTrigger("Reload");
        _animator?.SetTrigger("Reload");
    }
}
