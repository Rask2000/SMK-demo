using UnityEngine;
using UnityEngine.SceneManagement;

public class InputController : MonoBehaviour
{
    public string IntroScene;

    public void OnSwitch_to_intro()
    {
        SceneManager.LoadScene(IntroScene);
    }
}
