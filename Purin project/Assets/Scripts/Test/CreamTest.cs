using UnityEngine;

public class CreamTest : Cream
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Setup();
    }

    // Update is called once per frame
    void Update()
    {
        CreamActionUpdate();
    }
}
