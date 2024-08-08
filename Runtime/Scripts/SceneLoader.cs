using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class SceneLoader : MonoBehaviour
{
    // Campo serializable para seleccionar la escena desde el Inspector
    [SerializeField] private string SceneName;

    void Start()
    {
        LoadSceneAdditive();
    }

    // Método para cargar la escena aditiva
    public void LoadSceneAdditive()
    {
        if (SceneName != null)
        {
            
            
            // Verificar si la escena ya está cargada
            if (!SceneManager.GetSceneByName(SceneName).isLoaded)
            {
                SceneManager.LoadScene(SceneName, LoadSceneMode.Additive);
                Debug.Log("Escena " + SceneName + " cargada de manera aditiva.");
            }
            else
            {
                Debug.LogWarning("La escena " + SceneName + " ya está cargada.");
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
        if (SceneName != null)
        {
            

            // Verificar si la escena está cargada
            if (SceneManager.GetSceneByName(SceneName).isLoaded)
            {
                SceneManager.UnloadSceneAsync(SceneName);
                Debug.Log("Escena " + SceneName + " descargada.");
            }
            else
            {
                Debug.LogWarning("La escena " + SceneName + " no está cargada.");
            }
        }
        else
        {
            Debug.LogError("El campo de la escena no puede estar vacío.");
        }
    }
}