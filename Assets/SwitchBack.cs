using UnityEngine;
using FMOD;
using FMODUnity;
using FMOD.Studio;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

public class SwitchBack : MonoBehaviour
{
    [SerializeField] private string eventPath;
    [SerializeField] private string scene;
    private EventInstance instance;
    void Start()
    {
        instance = RuntimeManager.CreateInstance(eventPath);
        instance.start();
    }

    public void FixedUpdate()
    {
        instance.getPlaybackState(out PLAYBACK_STATE state);
        if (state == PLAYBACK_STATE.STOPPED)
        {
            SceneManager.LoadScene(scene);
        }
    }
}
