using UnityEngine;

public class GameSystemTest : MonoBehaviour
{
    [SerializeField] private ObjectMaster _objectMaster;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _objectMaster.Setup();
    }

    // Update is called once per frame
    void Update()
    {
        _objectMaster.MasterUpdate();
    }
}
