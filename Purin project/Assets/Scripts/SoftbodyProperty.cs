using UnityEngine;

[CreateAssetMenu(fileName = "SoftbodyProperty", menuName = "Scriptable Objects/SoftbodyProperty")]
public class SoftbodyProperty : ScriptableObject
{
    [Header("アニメーションの速度")]
    [SerializeField] private float _idleSpeed = 2f;
    [SerializeField] private float _moveSpeed = 6f;
    [SerializeField] private float _jumpSpeed = 8f;

    public float IdleSpeed => _idleSpeed;
    public float MoveSpeed => _moveSpeed;
    public float JumpSpeed => _jumpSpeed;

    [Header("スケールの幅")]
    [SerializeField] private float _idleAmount = 0.1f;
    [SerializeField] private float _moveAmount = 0.2f;
    [SerializeField] private float _jumpStretch = 0.4f;
    [SerializeField] private float _landSquash = 0.5f;
    [SerializeField] private float _hitSquash = 0.3f;
    public float IdleAmount => _idleAmount;
    public float MoveAmount => _moveAmount;
    public float JumpStretch => _jumpStretch;
    public float LandSquash => _landSquash;
    public float HitSquash => _hitSquash;
}
