using TMPro;
using UnityEngine;

public class DebugMovementText : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    private Rigidbody _rb;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _text.text = $"Лин. скорость: {_rb.linearVelocity.magnitude:F2}";
    }
}
