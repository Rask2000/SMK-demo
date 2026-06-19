using UnityEngine;
using FMODUnity;
using FMOD.Studio;


public class FmodFoleyPlayer : MonoBehaviour
{
    [SerializeField] private GameObject listener;

    public string eventPath;

    [SerializeField] private float maxDistance = 5f; // tweak this!

    [SerializeField] private Material debugMaterial;

    [UnityEngine.Range(0f, 1f)]
    public float normalized = 0f;

    private bool isDebugMode;

    private EventInstance instance;

    private GameObject sphere;

    public void Start()
    {
        isDebugMode = DebugManager.isDebugMode;
        if (isDebugMode)
        {
            CreateDebugSphere();
        }

        //has to be last or later code will not be run;
        instance = RuntimeManager.CreateInstance(eventPath);
        instance.setParameterByName("Loudness", 0f);
        instance.start();
    }

    void Update()
    {
        float distance = Vector3.Distance(
            this.transform.position,
            listener.transform.position
        );
        normalized = Mathf.Clamp01(distance / maxDistance);
        var flippedNormalized = 1 - normalized;

        instance.getPlaybackState(out PLAYBACK_STATE state);
        FMOD.RESULT result = instance.setParameterByName("Loudness", flippedNormalized);
        if (state == PLAYBACK_STATE.STOPPED)
        {
            instance.start(); // just restart, no release/recreate
        }
        if (isDebugMode && sphere != null)
        {
            Color color = Color.Lerp(Color.red, Color.green, flippedNormalized);
            debugMaterial.color = new Color(color.r, color.g, color.b, 0.5f);
            sphere.transform.localScale = new Vector3(maxDistance * 2, maxDistance * 2, maxDistance * 2);
        }
    }

    private void CreateDebugSphere()
    {
        sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.localScale = new Vector3(maxDistance * 2, maxDistance * 2, maxDistance * 2);
        debugMaterial = new Material(debugMaterial);
        sphere.GetComponent<Renderer>().material = debugMaterial;
        sphere.transform.SetParent(this.transform);
        sphere.transform.localPosition = Vector3.zero;
    }

    void OnDestroy()
    {
        instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        instance.release();
    }


}