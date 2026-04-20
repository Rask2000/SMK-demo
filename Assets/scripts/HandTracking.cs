using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class HandTracking : MonoBehaviour
{
    // Start is called before the first frame update
    public UDPReceive udpReceive;
    public GameObject[] handPoints;
    [SerializeField]
    private GameObject listener;
    [SerializeField] float movementScale = 5f;

    // Update is called once per frame
    public void FixedUpdate()
    {
        try
        {
            if (udpReceive == null)
            {
                Debug.LogError("udpReceive is null!");
                return;
            }

            string data = udpReceive.data;

            if (string.IsNullOrEmpty(data) || data.Length < 2)
            {
                Debug.LogWarning("Empty or too-short data received.");
                return;
            }

            data = data.Substring(1, data.Length - 2);
            string[] points = data.Split(',');

            if (points.Length < 21 * 3)
            {
                Debug.LogWarning($"Not enough points: got {points.Length}, expected {21 * 3}");
                return;
            }

            Vector3[] pointVectors = new Vector3[21];
            for (int i = 0; i < 21; i++)
            {
                float x = (7 - float.Parse(points[i * 3]) / 100) * movementScale;
                float y = (float.Parse(points[i * 3 + 1]) / 100) * movementScale;
                float z = (float.Parse(points[i * 3 + 2]) / 100) * movementScale;
                handPoints[i].transform.localPosition = new Vector3(x, -1, y);
                pointVectors[i] = new Vector3(x, -1, z);
            }

            int[] averageIndices = { 7, 8, 11, 12, 15, 16, 19, 20 }; //tips of fingers

            Vector3 avg = Vector3.zero;
            foreach (int idx in averageIndices)
            {
                avg += handPoints[idx].transform.localPosition;
            }
            avg /= averageIndices.Length;

            listener.transform.localPosition = avg;


        }
        catch (System.Exception e)
        {
            // This will tell us EXACTLY what is crashing
            Debug.LogError("EXCEPTION IN FixedUpdate: " + e.GetType().Name + " - " + e.Message + "\n" + e.StackTrace);
        }
    }
    void OnDisable()
    {
        Debug.LogWarning("HandTracking was DISABLED! " + System.Environment.StackTrace);
    }
}