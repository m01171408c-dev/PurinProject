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

#if true // ‰¼BGameMaster‚È‚Ç‚ÅŠÇ—‚·‚é—\’è
    public static float GameProgressTime => _gameProgressTime;
    private static float _gameProgressTime = 0;
    private bool _isGameActive = false;
#endif
    private List<HardObject> _objects = new List<HardObject>();
    private Phase _currentPhase = Phase.Phase1;
    public void Setup()
    {
        if (_hardObjectConfig == null)
        {
            Debug.LogError(this.name + " HardObjectConfig is not assigned.");
            return;
        }
        if(_spawnAreas == null || _spawnAreas.Length == 0)
        {
            Debug.LogError(this.name + " SpawnAreas are not assigned.");
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
        switch(_currentPhase)
        {
            case Phase.Phase1:
                // Phase 1 logic here
                {
                    if(_objects.Count < _hardObjectConfig.MinHardObjectCount1)
                    {
                        // Spawn new HardObject
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
        spawnPositionWorld.y = 0; // ’n–Ê‚É‡‚í‚¹‚é
    }
}

