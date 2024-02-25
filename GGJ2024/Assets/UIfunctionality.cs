using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIfunctionality : MonoBehaviour
{
    public void LoadScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    public void QuitApplication()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}