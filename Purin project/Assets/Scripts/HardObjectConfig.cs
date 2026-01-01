using UnityEngine;

[CreateAssetMenu(fileName = "HardObjectConfig", menuName = "Scriptable Objects/HardObjectConfig")]
public class HardObjectConfig : ScriptableObject
{
    [Header("障害物のステータス")]
    [SerializeField]private float _minSpeedRandom = 1.0f;
    [SerializeField] private float _maxSpeedRandom = 10.0f;
    [SerializeField] private float _minAccelerationRandom = 1.0f;
    [SerializeField] private float _maxAccelerationRandom = 5.0f;
    public float MinSpeedRandom => _minSpeedRandom;
    public float MaxSpeedRandom => _maxSpeedRandom;
    public float MinAccelerationRandom => _minAccelerationRandom;
    public float MaxAccelerationRandom => _maxAccelerationRandom;

    [Header("障害物の生成")]
    [SerializeField] private float _minSpawnInterval = 2.0f;
    [SerializeField] private float _maxSpawnInterval = 5.0f;
    [SerializeField] private int _maxHardObjectCount1 = 10;
    [SerializeField] private int _minHardObjectCount1 = 3;
    [SerializeField] private int _maxHardObjectCount2 = 15;
    [SerializeField] private int _minHardObjectCount2 = 5;
    [SerializeField] private int _maxHardObjectCount3 = 20;
    [SerializeField] private int _minHardObjectCount3 = 7;
    [SerializeField] private GameObject[] _hardObjectPrefabs = {};

    public float MinSpawnInterval => _minSpawnInterval;
    public float MaxSpawnInterval => _maxSpawnInterval;
    public int MaxHardObjectCount1 => _maxHardObjectCount1;
    public int MinHardObjectCount1 => _minHardObjectCount1;
    public int MaxHardObjectCount2 => _maxHardObjectCount2;
    public int MinHardObjectCount2 => _minHardObjectCount2;
    public int MaxHardObjectCount3 => _maxHardObjectCount3;
    public int MinHardObjectCount3 => _minHardObjectCount3;
    public GameObject[] HardObjectPrefabs => _hardObjectPrefabs;


}
