using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadScene : MonoBehaviour
{
    [SerializeField] private string SceneToLoad;

    public void Execute()
    {
        SceneManager.LoadScene(sceneName: SceneToLoad, LoadSceneMode.Single);
    }
}
