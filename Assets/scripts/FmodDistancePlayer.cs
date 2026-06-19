using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FmodDistancePlayer : MonoBehaviour
{
    [SerializeField] private GameObject listener;

    [SerializeField] private BoxCollider boxColliderObj;

    [SerializeField] private string parameterName;

    public string eventPath;

    [SerializeField] private float maxDistance = 5f; // tweak this!

    [Range(0f, 1f)]
    public float normalized = 0f;

    private EventInstance instance;

    void Start()
    {
        instance = RuntimeManager.CreateInstance(eventPath);
        instance.start();
    }

    public void FixedUpdate()
    {
        Vector3 ClosestPoint = boxColliderObj.ClosestPoint(listener.transform.position);

        float distance = Vector3.Distance(
            listener.transform.position,
            ClosestPoint
        );
        normalized = Mathf.Clamp01(distance / maxDistance);

        instance.setParameterByName(parameterName, normalized);
    }

    void OnDestroy()
    {
        instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        instance.release();
    }
}