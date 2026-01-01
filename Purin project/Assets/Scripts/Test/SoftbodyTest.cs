using UnityEngine;

public class SoftbodyTest : Softbody
{
    [SerializeField] private SoftbodyProperty _softbodyPropTest;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_softbodyPropTest == null)
        {
            Debug.LogError(this.name + " SoftbodyProperty is not assigned in the inspector");
            return;
        }
        SetUp(ref _softbodyPropTest);
    }

    // Update is called once per frame
    void Update()
    {
        if (_softbodyPropTest == null)
        {
            Debug.LogError(this.name + " SoftbodyProperty is null");
            return;
        }
        AnimationUpdate();
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("Softbody Test");
        if (GUI.Button(new Rect(10, 30, 100, 30), "Jump"))
        {
            Debug.Log("Jump Button Pressed");
            // Trigger jump animation
            Jump();
        }
        if (GUI.Button(new Rect(10, 70, 100, 30), "Land"))
        {
            Debug.Log("Land Button Pressed");
            // Trigger land animation
            Land();
        }
        if (GUI.Button(new Rect(10, 110, 100, 30), "Hit"))
        {
            Debug.Log("Hit Button Pressed");
            // Trigger hit animation
            Hit();
        }
        if (GUI.Button(new Rect(10, 150, 100, 30), "Move"))
        {
            Debug.Log("Move Button Pressed");
            // Trigger move animation
            SetMoving(true);
        }
        if (GUI.Button(new Rect(120, 150, 100, 30), "Stop"))
        {
            Debug.Log("Stop Button Pressed");
            // Stop move animation
            SetMoving(false);
        }
        GUILayout.EndArea();
    }
}
