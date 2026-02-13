using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Zenject;
using TMPro;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _currAmmoText;
    [SerializeField] private TMP_Text _maxAmmoText;
    [SerializeField] private PistolBehaviour _pistol;

    private void OnEnable()
    {
        _pistol.OnShoot.AddListener(UpdateAmmoText);
        _pistol.OnReloadEnd.AddListener(UpdateAmmoText);
        _maxAmmoText.text = _pistol.MaxAmmo.ToString();
        _currAmmoText.text = _pistol.CurrAmmo.ToString();
    }

    private void OnDisable()
    {
        _pistol.OnShoot.RemoveListener(UpdateAmmoText);
        _pistol.OnReloadEnd.RemoveListener(UpdateAmmoText);
    }

    private void UpdateAmmoText()
    {
        _currAmmoText.text = _pistol.CurrAmmo.ToString();
    }
}
