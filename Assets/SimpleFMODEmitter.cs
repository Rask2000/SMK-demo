using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class SimpleFMODEmitter : MonoBehaviour
{
    [Header("Event")]
    public EventReference eventReference;

    [Header("Attenuation Override")]
    public bool overrideAttenuation = true;
    public float minDistance = 1f;
    public float maxDistance = 3f;

    private EventInstance instance;

    void Start()
    {
        instance = RuntimeManager.CreateInstance(eventReference);

        // Force 3D attributes
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

        if (overrideAttenuation)
        {
            instance.setProperty(EVENT_PROPERTY.MINIMUM_DISTANCE, minDistance);
            instance.setProperty(EVENT_PROPERTY.MAXIMUM_DISTANCE, maxDistance);
        }

        instance.start();
    }

    void Update()
    {
        // Continuously update position
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
    }

    void OnDestroy()
    {
        instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        instance.release();
    }
}