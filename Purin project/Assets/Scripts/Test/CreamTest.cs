using UnityEngine;

public class CreamTest : Cream
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Setup(5.0f);
    }

    // Update is called once per frame
    void Update()
    {
        CreamActionUpdate();
    }
}
