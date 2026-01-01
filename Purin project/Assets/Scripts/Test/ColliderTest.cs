using UnityEngine;

public class ColliderTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        
        Debug.Log(this.name + " OnTriggerEnter with " + other.name);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(this.name + " OnCollisionEnter with " + collision.gameObject.name);
    }
}
