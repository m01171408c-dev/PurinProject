using UnityEngine;

[CreateAssetMenu(fileName = "PurinConfig", menuName = "Scriptable Objects/PurinConfig")]
public class PurinConfig : ScriptableObject
{
    [Header("ƒvƒŠƒ“‚Ì‘¬“xÝ’è")]
    [SerializeField] private float _acceleration = 0.1f;
    [SerializeField] private float _maxSpeed = 5.0f;
    [SerializeField] private float _minSpeed = 1.0f;
    [SerializeField] private float _deceleration = 0.1f;
    [SerializeField] private float _turnSpeed = 2.0f;
    [SerializeField] private float _breakSpeed = 0.2f;

    public float Acceleration => _acceleration;
    public float MaxSpeed => _maxSpeed;
    public float MinSpeed => _minSpeed;
    public float Deceleration => _deceleration;
    public float TurnSpeed => _turnSpeed;
    public float BreakSpeed => _breakSpeed;

    [Header("ƒvƒŠƒ“‚ÌÕ“ËÝ’è")]
    [SerializeField] private float _knockbackDistance = 3f;
    [SerializeField] private float _knckbackTime = 0.2f;
    [SerializeField] private float _downTime = 1.0f;
    public float KnockbackDistance => _knockbackDistance;
    public float KnockbackTime => _knckbackTime;
    public float DownTime => _downTime;
}
