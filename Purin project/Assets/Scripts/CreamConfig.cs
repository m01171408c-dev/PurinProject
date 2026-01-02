using UnityEngine;

[CreateAssetMenu(fileName = "CreamProperty", menuName = "Scriptable Objects/CreamConfig")]
public class CreamConfig : ScriptableObject
{
    [Header("クリームアイテムのステータス")]
    [SerializeField] private float _lifetime = 5.0f;
    [SerializeField] private GameObject _creamPrefab = null;
    public float Lifetime => _lifetime;
    public GameObject CreamPrefab => _creamPrefab;

    [Header("クリームアイテムの生成")]
    [SerializeField] private float _minSpawnInterval = 2.0f;
    [SerializeField] private float _maxSpawnInterval = 10.0f;
    [SerializeField] private int _maxCreamItemCount = 3;
    public int MaxCreamItemCount => _maxCreamItemCount;
    public float MaxSpawnInterval => _maxSpawnInterval;
    public float MinSpawnInterval => _minSpawnInterval;
}
