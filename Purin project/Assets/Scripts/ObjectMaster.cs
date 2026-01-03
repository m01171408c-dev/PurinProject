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
    [SerializeField] private BoxCollider[] _hardObjectSpawnAreas;
    [SerializeField] private Transform _center;
    [SerializeField] private CreamConfig _creamConfig;
    [SerializeField] private BoxCollider _creamSpawnArea;

    private List<HardObject> _objects = new List<HardObject>();
    private List<Cream> _creams = new List<Cream>();
    private Phase _currentPhase = Phase.Phase1;
    private float _hardObjSpawnTimer = 0;
    private float _creamSpawnTimer = 0;

    public void Setup()
    {
        if (_hardObjectConfig == null)
        {
            Debug.LogError(this.name + " HardObjectConfig is not assigned.");
            return;
        }
        if (_hardObjectSpawnAreas == null || _hardObjectSpawnAreas.Length == 0)
        {
            Debug.LogError(this.name + " SpawnAreas are not assigned.");
            return;
        }
        if (_center == null)
        {
            Debug.LogError(this.name + " Center transform is not assigned.");
            return;
        }
    }

    public void MasterUpdate()
    {
        if (_hardObjectConfig == null)
        {
            Debug.LogError(this.name + " HardObjectConfig is not assigned.");
            return;
        }
        if (GameMain.Instance.State != GameMain.GameState.Active) return;
        for (int i = _objects.Count - 1; i >= 0; i--)
        {
            var obj = _objects[i]; 
            if (obj == null || obj.CurrentState == HardObject.HardObjectState.Destroyed)
            {
                _objects.RemoveAt(i);
                if(obj)
                {
                    Destroy(obj.gameObject); // Todo：削除エフェクトの生成。
                }
                continue;
            }
            obj.ActionUpdate();
        }
        for (int i = _creams.Count - 1; i >= 0; i--)
        {
            var cream = _creams[i];
            if(cream == null || cream.IsDestroyed)
            {
                _creams.RemoveAt(i);
                if(cream)
                {
                    Destroy(cream.gameObject);// Todo：削除エフェクトの生成。
                }
                continue;
            }
            cream.CreamActionUpdate();
        }
            switch (_currentPhase)
        {
            case Phase.Phase1:
                {
                    if(_creams.Count < _creamConfig.MaxCreamItemCount)
                    {
                        if(_objects.Count > _hardObjectConfig.MinHardObjectCount1)
                        {
                            if(_creamSpawnTimer < _creamConfig.MaxSpawnInterval)
                            {
                                _creamSpawnTimer += Time.deltaTime;
                            }
                            else
                            {
                                SpawnCream();
                                _creamSpawnTimer = 0;
                            }
                        }
                    }
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
                        if (_hardObjSpawnTimer < _hardObjectConfig.MaxSpawnInterval)
                        {
                            _hardObjSpawnTimer += Time.deltaTime;

                        }
                        else
                        {
                            SpawnObjects();
                            _hardObjSpawnTimer = 0;
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

    private void SpawnCream()
    {
        if(_creamSpawnArea == null)
        {
            Debug.LogError(this.name + "CreamSpawnArea is not assigned.");
            return;
        }
        var spawnPositionLocal = new Vector3(
           Random.Range(-_creamSpawnArea.size.x * 0.5f, _creamSpawnArea.size.x * 0.5f),
           Random.Range(-_creamSpawnArea.size.y * 0.5f, _creamSpawnArea.size.y * 0.5f),
           Random.Range(-_creamSpawnArea.size.z * 0.5f, _creamSpawnArea.size.z * 0.5f));
        var spawnPositionWorld = _creamSpawnArea.transform.TransformPoint(_creamSpawnArea.center + spawnPositionLocal);
        spawnPositionWorld.y = 1; // 地面に合わせる
        if(_creamConfig ==  null || _creamConfig.CreamPrefab == null)
        {
            Debug.LogError(this.name + " CreamConfig Prefab is not assigned.");
            return;
        }
        var creamPrefab = Instantiate(_creamConfig.CreamPrefab, Vector3.zero, Quaternion.identity);
        creamPrefab.name = "Cream" + _creams.Count.ToString();
        creamPrefab.transform.position = spawnPositionWorld;
        var creamItem = creamPrefab.AddComponent<Cream>();
        _creams.Add(creamItem);
        creamItem.Setup(_creamConfig.Lifetime);
        if(_creamConfig.SpawnEffect != null)
        {
            var spawnEffect = GameObject.Instantiate(_creamConfig.SpawnEffect,
                            creamPrefab.transform.position, _creamConfig.SpawnEffect.transform.rotation);
            spawnEffect.transform.parent = creamItem.transform;
        }
    }
    private void SpawnObjects()
    {
        if (_hardObjectSpawnAreas == null || _hardObjectSpawnAreas.Length == 0)
        {
            Debug.LogError(this.name + " SpawnAreas are not assigned.");
            return;
        }
        var spawnArea = _hardObjectSpawnAreas[Random.Range(0, _hardObjectSpawnAreas.Length)];
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
        var hardObject = new GameObject("HardObject" + _objects.Count.ToString()).AddComponent<HardObject>();
        hardObjectPrefab.transform.SetParent(hardObject.transform);
        hardObjectPrefab.transform.localPosition = Vector3.zero;
        hardObject.transform.position = spawnPositionWorld;
        var originDir = _center.position - spawnPositionWorld;
        originDir.y = 0;
        _objects.Add(hardObject);
        hardObject.StartAction(originDir.normalized);
    }
}

