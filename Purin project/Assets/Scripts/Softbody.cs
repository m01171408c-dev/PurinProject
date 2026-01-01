using UnityEngine;

public class Softbody : MonoBehaviour
{
    private Vector3 _baseScale = new Vector3(1, 1, 1);
    private SoftbodyProperty _softbodyProp = null;

    private float _animTimer = 0f;
    private bool _isMoving = false;
    private bool _isJumping = false;
    private bool _isLanding = false;
    private bool _isHit = false;

    private float _squashTimer = 0f;
    private float _squashDuration = 0.15f;


    public void SetUp(ref SoftbodyProperty softbodyProperty)
    {
        if(softbodyProperty == null)
        {
            Debug.LogError(this.name + " ref SoftbodyProperty is null");
            return;
        }
        _softbodyProp = softbodyProperty;
        _baseScale = transform.localScale;
    }

    public void ResetAnimation()
    {
        _animTimer = 0f;
        _isMoving = false;
        _isJumping = false;
        _isLanding = false;
        _isHit = false;
        _squashTimer = 0f;
        transform.localScale = _baseScale;
    }

    public void AnimationUpdate()
    {
        if (_softbodyProp == null)
        {
            Debug.LogError(this.name + " SoftbodyProperty is null");
            return;
        }
        
        _animTimer += Time.deltaTime;

        Vector3 scale = _baseScale;

        // --- 衝突アニメ（最優先） ---
        if (_isHit)
        {
            float t = _squashTimer / _squashDuration;
            float squash = Mathf.Lerp(_softbodyProp.HitSquash, 0, t);

            scale = new Vector3(
                _baseScale.x + squash,
                _baseScale.y - squash,
                _baseScale.z + squash
            );

            _squashTimer += Time.deltaTime;
            if (_squashTimer >= _squashDuration)
                _isHit = false;

            transform.localScale = scale;
            return;
        }

        // --- 着地アニメ ---
        if (_isLanding)
        {
            float t = _squashTimer / _squashDuration;
            float squash = Mathf.Lerp(_softbodyProp.LandSquash, 0, t);

            scale = new Vector3(
                _baseScale.x + squash,
                _baseScale.y - squash,
                _baseScale.z + squash
            );

            _squashTimer += Time.deltaTime;
            if (_squashTimer >= _squashDuration)
                _isLanding = false;

            transform.localScale = scale;
            return;
        }

        // --- ジャンプ中 ---
        if (_isJumping)
        {
            float stretch = Mathf.Sin(_animTimer * _softbodyProp.JumpSpeed) * _softbodyProp.JumpStretch;

            scale = new Vector3(
                _baseScale.x - stretch * 0.5f,
                _baseScale.y + stretch,
                _baseScale.z - stretch * 0.5f
            );

            transform.localScale = scale;
            return;
        }

        // --- 移動中 ---
        if (_isMoving)
        {
            float t = Mathf.Sin(_animTimer * _softbodyProp.MoveSpeed) * _softbodyProp.MoveAmount;

            scale = new Vector3(
                _baseScale.x + t,
                _baseScale.y - t,
                _baseScale.z + t
            );

            transform.localScale = scale;
            return;
        }

        // --- 待機中 ---
        float idle = Mathf.Sin(_animTimer * _softbodyProp.IdleSpeed) * _softbodyProp.IdleAmount;

        scale = new Vector3(
            _baseScale.x + idle,
            _baseScale.y - idle,
            _baseScale.z + idle
        );

        transform.localScale = scale;
    }

    public void SetMoving(bool value)
    {
        _isMoving = value;
    }

    public void Jump()
    {
        ResetAnimation();
        _isJumping = true;
    }

    public void Land()
    {
        ResetAnimation();
        _isJumping = false;
        _isLanding = true;
        _squashTimer = 0f;
    }

    public void Hit()
    {
        ResetAnimation();
        _isHit = true;
        _squashTimer = 0f;
    }
}
