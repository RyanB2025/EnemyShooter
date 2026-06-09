using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes

public class SceneLoader : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Type the exact name of the scene you want to load here.")]
    public string targetSceneName; // This will appear in the Unity Inspector

    /// <summary>
    /// Loads the scene specified in the targetSceneName variable.
    /// Hook this method up to your Button's OnClick event.
    /// </summary>
    public void LoadTargetScene()
    {
        // A quick safety check to ensure the field isn't left blank
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogWarning("Target Scene Name is empty! Please fill it out in the Inspector.");
        }
    }
}
