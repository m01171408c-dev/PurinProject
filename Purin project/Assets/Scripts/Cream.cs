using UnityEngine;

public class Cream : MonoBehaviour
{
    [SerializeField] private Material _creamBlinkMaterial = null;
    private bool _isActive = false;
    public bool IsActive => _isActive;
    private bool _isDestroyed = false;
    public bool IsDestroyed => _isDestroyed;
    private int _useModelIndex = -1;
    private float _lifetime = 5.0f;
    private float _timer = 0.0f;
    private bool _isBlinking = false;
    public void Setup(float lifeTime)
    {
        int childCount = -1;
        _lifetime = lifeTime;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
            childCount++;
        }
        if (childCount < 0)
        {
            Debug.LogError(this.name + " Cream: No child objects found for cream stages.");
            return;
        }
        _useModelIndex = Random.Range(0, childCount + 1);
        transform.GetChild(_useModelIndex).gameObject.SetActive(true);
        _isActive = true;
    }

    public void CreamActionUpdate()
    {
        if (!_isActive || _isDestroyed)
        {
            return;
        }
        if (_timer >= _lifetime)
        {
            DestroyCream();
            return;
        }
        if ((_lifetime - _timer) <= 0.5f)
        {
            if (!_isBlinking)
            {
                var renderer = transform.GetChild(_useModelIndex).GetComponent<Renderer>();
                if (renderer != null && _creamBlinkMaterial != null)
                {
                    renderer.material = _creamBlinkMaterial;
                    _isBlinking = true;
                }
            }
        }
        _timer += Time.deltaTime;
    }

    public int PurinTakeCream()
    {
        var result = -1;
        if (_isActive == false || _isDestroyed)
        {
            return result;
        }
        DestroyCream();
        result = _useModelIndex;
        return result;
    }
    public void DestroyCream()
    {
        _isDestroyed = true;
        _isActive = false;
        _timer = 0.0f;
    }
}
