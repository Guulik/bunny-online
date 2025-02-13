using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapManager : MonoBehaviour
{
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
        StartCoroutine(LoadScenesAndUnloadCurrent());
    }

    private IEnumerator LoadScenesAndUnloadCurrent()
    {
        // Загрузить сцену "Services"
        AsyncOperation loadServices = SceneManager.LoadSceneAsync("Services", LoadSceneMode.Additive);
        yield return new WaitUntil(() => loadServices.isDone);

        // Загрузить сцену "Menu"
        AsyncOperation loadMenu = SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Additive);
        yield return new WaitUntil(() => loadMenu.isDone);

        // Получить текущую активную сцену (сцену с BootstrapManager)
        Scene currentScene = SceneManager.GetActiveScene();

        // Выгрузить текущую сцену
        AsyncOperation unloadCurrentScene = SceneManager.UnloadSceneAsync(currentScene);
        yield return new WaitUntil(() => unloadCurrentScene.isDone);

        // После выгрузки текущей сцены можно переключиться на новую активную сцену, если это необходимо
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Menu"));
    }
}