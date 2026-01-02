using UnityEngine;

public class EffectTest : MonoBehaviour
{
    [SerializeField] private GameObject _effect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime);
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Play Effect"))
        {
            _effect.SetActive(true);
        }

    }
}
