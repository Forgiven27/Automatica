using UnityEngine;
using UnityEngine.SceneManagement;

using Cysharp.Threading.Tasks;

public class MenuButtonsController : MonoBehaviour
{

    [SerializeField] private RectTransform _downloadPanel;

    public void Start()
    {
        _downloadPanel.gameObject.SetActive(false);
    }

    public async void StartGame()
    {
        var sceneOp = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        if (sceneOp == null)
        {
            Debug.LogError("Не удалось начать загрузку сцены (операция = null)");
            return;
        }



        sceneOp.allowSceneActivation = false;
        await LoadScene(sceneOp);
    }

    private async UniTask LoadScene(AsyncOperation op)
    {
        _downloadPanel.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        while (op.progress < 0.9f)
        {
            Debug.Log($"Загрузка: {op.progress:P0}");
            await UniTask.Yield(); 
        }
        await UniTask.WaitForSeconds(5);

        Debug.Log("Сцена загружена на 90%, разрешаем активацию...");
        op.allowSceneActivation = true;

        while (!op.isDone)
        {
            Debug.Log($"Активация: {op.progress:P0}");
            await UniTask.Yield();
        }
        Debug.Log("Сцена полностью загружена и активирована");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
