using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using JetBrains.Annotations;

public class FmodVariableSound : MonoBehaviour
{
    [SerializeField] private GameObject lowValence;
    [SerializeField] private GameObject trackingObj;

    [SerializeField] private BoxCollider boxColliderObj;

    public string eventPath;

    [SerializeField] private float maxDistance = 5f; // tweak this!

    [Range(0f, 1f)]
    public float normalized = 0f;

    private EventInstance lowValenceInstance;

    void Start()
    {
        lowValenceInstance = RuntimeManager.CreateInstance(eventPath);
        lowValenceInstance.start();
    }

    public void Update()
    {
        Vector3 ClosestPoint = boxColliderObj.ClosestPoint(trackingObj.transform.position);

        float distance = Vector3.Distance(
            trackingObj.transform.position,
            ClosestPoint
        );
        normalized = Mathf.Clamp01(distance / maxDistance);

        lowValenceInstance.setParameterByName("up", normalized);
    }
    void FixedUpdate()
    {

        float distance = Vector3.Distance(
            trackingObj.transform.position,
            lowValence.transform.position
        );

        normalized = Mathf.Clamp01(distance / maxDistance);

        lowValenceInstance.setParameterByName("up", normalized);

    }

    void OnDestroy()
    {
        lowValenceInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        lowValenceInstance.release();
    }


}