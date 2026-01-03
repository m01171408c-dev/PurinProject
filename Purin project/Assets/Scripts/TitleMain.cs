using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class TitleMain : MonoBehaviour
{
    [SerializeField] private PlayableAsset _opening;
    [SerializeField] private PlayableAsset _pressStart;
    [SerializeField] private PlayableDirector _director;
    [SerializeField] private string _nextSceneName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        await GameStartTask();
    }

    private async UniTask GameStartTask()
    {
        _director.playableAsset = _opening;
        _director.Play();
        await UniTask.WaitUntil(() =>_director.state != PlayState.Playing);
        Debug.Log("Title Main Opening Finish.");
        await UniTask.Delay(500);
        // Todo：ゲームパッドの動作要確認
        await UniTask.WaitUntil(() =>
        (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) ||
        (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) || 
        (Gamepad.current != null && Gamepad.current.allControls.Any(c => c is ButtonControl b && b.wasPressedThisFrame))
        );
        Debug.Log("Title Main Key Pressed.");
        _director.playableAsset = _pressStart;
        _director.Play();
        await UniTask.WaitUntil(() => _director.state != PlayState.Playing);
        Debug.Log("Title Main Start Load Scene.");
        await SceneManager.LoadSceneAsync(_nextSceneName).ToUniTask();
        Debug.Log("Title Main Load Scene Cpmpleted.");
    }
}
