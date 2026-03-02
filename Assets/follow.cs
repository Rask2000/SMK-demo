using UnityEngine;

public class Follow : MonoBehaviour
{
    void Update()
    {
        int childCount = transform.childCount;
        if (childCount == 0) return;

        Vector3 center = Vector3.zero;

        // Sum positions of all children
        for (int i = 0; i < childCount; i++)
        {
            center += transform.GetChild(i).position;
        }

        // Divide by number of children to get average
        center /= childCount;

        // Set parent position to center
        transform.position = center;
    }
}