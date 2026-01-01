using System.Collections.Generic;
using UnityEngine;

public class ObjectMaster : MonoBehaviour
{
    public enum Phase
    {
        Phase1,
        Phase2,
        Phase3
    }

    [SerializeField] private HardObjectConfig _hardObjectConfig;
    [SerializeField] private BoxCollider[] _spawnAreas;
    [SerializeField] private Transform _center;

#if true // 仮。GameMasterなどで管理する予定
    public static float GameProgressTime => _gameProgressTime;
    private static float _gameProgressTime = 0;
    private bool _isGameActive = false;
#endif
    private List<HardObject> _objects = new List<HardObject>();
    private Phase _currentPhase = Phase.Phase1;
    private float _spawnTimer = 0;
    public void Setup()
    {
        if (_hardObjectConfig == null)
        {
            Debug.LogError(this.name + " HardObjectConfig is not assigned.");
            return;
        }
        if (_spawnAreas == null || _spawnAreas.Length == 0)
        {
            Debug.LogError(this.name + " SpawnAreas are not assigned.");
            return;
        }
        if (_center == null)
        {
            Debug.LogError(this.name + " Center transform is not assigned.");
            return;
        }
        _isGameActive = true;
    }

    public void MasterUpdate()
    {
        if (_hardObjectConfig == null)
        {
            Debug.LogError(this.name + " HardObjectConfig is not assigned.");
            return;
        }
        if (!_isGameActive) return;
        _gameProgressTime += Time.deltaTime;
        for (int i = _objects.Count - 1; i >= 0; i--)
        {
            var obj = _objects[i]; 
            if (obj == null || obj.CurrentState == HardObject.HardObjectState.Destroyed)
            {
                _objects.RemoveAt(i);
                Destroy(obj.gameObject); // Todo：削除エフェクトの生成。
                continue;
            }
            obj.ActionUpdate();
        }
        switch (_currentPhase)
        {
            case Phase.Phase1:
                {
                    if (_objects.Count < _hardObjectConfig.MinHardObjectCount1)
                    {
                        for (int i = _objects.Count; i < _hardObjectConfig.MinHardObjectCount1; i++)
                        {
                            SpawnObjects();
                        }
                        break;
                    }
                    if (_objects.Count < _hardObjectConfig.MaxHardObjectCount1)
                    {
                        if (_spawnTimer < _hardObjectConfig.MaxSpawnInterval)
                        {
                            _spawnTimer += Time.deltaTime;

                        }
                        else
                        {
                            SpawnObjects();
                            _spawnTimer = 0;
                        }
                    }
                }
                break;
            case Phase.Phase2:
                // Phase 2 logic here
                break;
            case Phase.Phase3:
                // Phase 3 logic here
                break;
        }
    }

    private void SpawnObjects()
    {
        if (_spawnAreas == null || _spawnAreas.Length == 0)
        {
            Debug.LogError(this.name + " SpawnAreas are not assigned.");
            return;
        }
        var spawnArea = _spawnAreas[Random.Range(0, _spawnAreas.Length)];
        var spawnPositionLocal = new Vector3(
            Random.Range(-spawnArea.size.x * 0.5f, spawnArea.size.x * 0.5f),
            Random.Range(-spawnArea.size.y * 0.5f, spawnArea.size.y * 0.5f),
            Random.Range(-spawnArea.size.z * 0.5f, spawnArea.size.z * 0.5f));
        var spawnPositionWorld = spawnArea.transform.TransformPoint(spawnArea.center + spawnPositionLocal);
        spawnPositionWorld.y = 1; // 地面に合わせる
        if (_hardObjectConfig == null || _hardObjectConfig.HardObjectPrefabs.Length == 0)
        {
            Debug.LogError(this.name + " HardObjectPrefabs are not assigned.");
            return;
        }
        if (_center == null)
        {
            Debug.LogError(this.name + " Center transform is not assigned.");
            return;
        }
        var instantiateIndex = Random.Range(0, _hardObjectConfig.HardObjectPrefabs.Length);
        var hardObjectPrefab = Instantiate(_hardObjectConfig.HardObjectPrefabs[instantiateIndex], Vector3.zero, Quaternion.identity);
        var hardObject = new GameObject("HardObject").AddComponent<HardObject>();
        hardObjectPrefab.transform.SetParent(hardObject.transform);
        hardObjectPrefab.transform.localPosition = Vector3.zero;
        hardObject.transform.position = spawnPositionWorld;
        var originDir = _center.position - spawnPositionWorld;
        originDir.y = 0;
        _objects.Add(hardObject);
        hardObject.StartAction(originDir.normalized);
    }
}

