using UnityEngine;

public class PurinTestPlayer : PlayerObject
{
    private void Start()
    {
        base.Init();
        base.EnableInput();
    }

    private void Update()
    {
        base.PlayerUpdate();
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("Purin Test Player");
        GUILayout.Label("Current Speed: " + GetMyPurin?.CurrentSpeed.ToString("F2"));
        var angle = GetMyPurin?.transform.localEulerAngles.y;
        GUILayout.Label("Current Angle: " + angle?.ToString("F2"));
        GUILayout.Label("Current State: " + GetMyPurin?.CurrentState.ToString());
        GUILayout.Label("Is Landing: " + GetMyPurin?.GetIsLanding().ToString());
        GUILayout.EndArea();
    }
}
