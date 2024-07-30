using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class SceneLoader : MonoBehaviour
{
    // Campo serializable para seleccionar la escena desde el Inspector
    [SerializeField] private SceneAsset sceneAsset;

    void Start()
    {
        LoadSceneAdditive();
    }

    // Método para cargar la escena aditiva
    public void LoadSceneAdditive()
    {
        if (sceneAsset != null)
        {
            string sceneName = sceneAsset.name;
            
            // Verificar si la escena ya está cargada
            if (!SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
                Debug.Log("Escena " + sceneName + " cargada de manera aditiva.");
            }
            else
            {
                Debug.LogWarning("La escena " + sceneName + " ya está cargada.");
            }
        }
        else
        {
            Debug.LogError("El campo de la escena no puede estar vacío.");
        }
    }

    // Método para descargar la escena aditiva
    public void UnloadSceneAdditive()
    {
        if (sceneAsset != null)
        {
            string sceneName = sceneAsset.name;

            // Verificar si la escena está cargada
            if (SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                SceneManager.UnloadSceneAsync(sceneName);
                Debug.Log("Escena " + sceneName + " descargada.");
            }
            else
            {
                Debug.LogWarning("La escena " + sceneName + " no está cargada.");
            }
        }
        else
        {
            Debug.LogError("El campo de la escena no puede estar vacío.");
        }
    }
}