using UnityEngine;

public class HardObjectTest : HardObject
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Foward Direction: " + transform.forward);
        StartAction(Vector3.forward);
    }

    // Update is called once per frame
    void Update()
    {
        ActionUpdate();
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("Hard Object Test");
        GUILayout.Label("Current Speed:" + CurrentSpeed.ToString());
        GUILayout.EndArea();

    }
}
