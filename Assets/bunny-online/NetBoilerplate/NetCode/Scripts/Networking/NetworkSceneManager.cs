using FishNet;
using FishNet.Managing.Scened;

public class NetworkSceneManager : SingletonMonoBehavior<NetworkSceneManager>
{
    public void LoadScene(string sceneName)
    {
        SceneLoadData sld = new SceneLoadData(sceneName);
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
    }

    public void UnloadScene(string sceneName)
    {
        SceneUnloadData sld = new SceneUnloadData(sceneName);
        InstanceFinder.SceneManager.UnloadGlobalScenes(sld);
    }
}