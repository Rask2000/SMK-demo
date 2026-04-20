using UnityEngine;
using FMODUnity;
using FMOD.Studio;


public class FmodFoleyPlayer : MonoBehaviour
{
    [SerializeField] private GameObject trackingObj;

    public string eventPath;

    [SerializeField] private float maxDistance = 5f; // tweak this!

    [SerializeField] private Material debugMaterial;

    [UnityEngine.Range(0f, 1f)]
    public float normalized = 0f;

    private bool isDebugMode;

    private EventInstance lowValenceInstance;

    private GameObject sphere;

    public void Start()
    {
        isDebugMode = DebugManager.isDebugMode;
        if (isDebugMode)
        {
            CreateDebugSphere();
        }

        //has to be last or later code will not be run;
        lowValenceInstance = RuntimeManager.CreateInstance(eventPath);
        lowValenceInstance.setParameterByName("Loudness", 0f);
        lowValenceInstance.start();
    }

    void Update()
    {
        float distance = Vector3.Distance(
            transform.position,
            trackingObj.transform.position
        );
        normalized = Mathf.Clamp01(distance / maxDistance);
        var flippedNormalized = 1 - normalized;

        lowValenceInstance.getPlaybackState(out PLAYBACK_STATE state);
        FMOD.RESULT result = lowValenceInstance.setParameterByName("Loudness", flippedNormalized);
        Debug.Log($"Result: {result}, value: {flippedNormalized}");
        if (state == PLAYBACK_STATE.STOPPED)
        {
            lowValenceInstance.start(); // just restart, no release/recreate
        }


        
    }

    private void CreateDebugSphere()
    {
        sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.localScale = new Vector3(maxDistance * 2, maxDistance * 2, maxDistance * 2);
        debugMaterial = new Material(debugMaterial);
        sphere.GetComponent<Renderer>().material = debugMaterial;
        sphere.transform.SetParent(transform);
        sphere.transform.localPosition = Vector3.zero;
    }

    void OnDestroy()
    {
        lowValenceInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        lowValenceInstance.release();
    }


}