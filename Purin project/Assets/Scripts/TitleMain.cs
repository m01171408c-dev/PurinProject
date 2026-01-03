using Cysharp.Threading.Tasks;
using System.Linq;
using System.Threading;
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

    private CancellationTokenSource _taskcancelToken;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        _taskcancelToken = new CancellationTokenSource();
        await GameStartTask();
    }

    private async UniTask GameStartTask()
    {
        _director.playableAsset = _opening;
        _director.Play();
        await UniTask.WaitUntil(() =>_director.state != PlayState.Playing, cancellationToken: _taskcancelToken.Token);
        Debug.Log("Title Main Opening Finish.");
        await UniTask.Delay(500, cancellationToken: _taskcancelToken.Token);
        // Todo：ゲームパッドの動作要確認
        await UniTask.WaitUntil(() =>
        (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) ||
        (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) || 
        (Gamepad.current != null && Gamepad.current.allControls.Any(c => c is ButtonControl b && b.wasPressedThisFrame))
        , cancellationToken: _taskcancelToken.Token);
        Debug.Log("Title Main Key Pressed.");
        _director.playableAsset = _pressStart;
        _director.Play();
        await UniTask.WaitUntil(() => _director.state != PlayState.Playing, cancellationToken: _taskcancelToken.Token);
        Debug.Log("Title Main Start Load Scene.");
        await SceneManager.LoadSceneAsync(GameSceneName.Main).ToUniTask();
        Debug.Log("Title Main Load Scene Cpmpleted.");
    }

    private void OnDestroy()
    {
        _taskcancelToken.Cancel();
    }
}
