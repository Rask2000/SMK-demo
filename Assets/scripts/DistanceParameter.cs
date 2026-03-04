using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class DistanceParameter : MonoBehaviour
{
    [Header("FMOD")]
    public EventReference fmodEvent;   // ← NEW way
    public string parameterName = "up";

    [Header("Distance Settings")]
    public Transform target;
    public float maxDistance = 10f;

    private EventInstance eventInstance;

    void Start()
    {
        eventInstance = RuntimeManager.CreateInstance(fmodEvent);
        eventInstance.start();
    }

    void Update()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        float normalized = Mathf.Clamp01(distance / maxDistance);

        eventInstance.setParameterByName(parameterName, normalized);

        // Important if using 3D spatialization
        eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
    }

    void OnDestroy()
    {
        eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        eventInstance.release();
    }
}