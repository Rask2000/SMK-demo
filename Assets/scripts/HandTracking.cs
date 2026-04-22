using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class HandTracking : MonoBehaviour
{
    // Start is called before the first frame update
    public UDPReceive udpReceive;
    public GameObject[] hand1Points;
    public GameObject[] hand2Points;

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

            string[] hands = data.Split('|');
            string hand1Data = hands[0];
            string hand2Data = hands.Length > 1 ? hands[1] : "";
            string[] hand1PointsData = hand1Data.Split(';');
            string[] hand2PointsData = hand2Data.Split(';');

            Vector3[] hand1PointVectors = TrackPointsForHand(hand1Points, hand1PointsData);
            Vector3[] hand2PointVectors = TrackPointsForHand(hand2Points, hand2PointsData);

            Vector3 avgHand1 = CalculateAvargeFingerPosition(0, hand1Data);
            Vector3 avgHand2 = CalculateAvargeFingerPosition(1, hand2Data);
            //lerp towards the average position of the fingers, but if the hands are too far apart, lerp towards the first hand only
            if (Vector3.Distance(avgHand1, avgHand2) > 3f)
            {
                listener.transform.localPosition = Vector3.Lerp(listener.transform.localPosition, avgHand1, Time.deltaTime * 2f);
                return;
            }
            listener.transform.localPosition = Vector3.Lerp(listener.transform.localPosition, (avgHand1 + avgHand2) / 2, Time.deltaTime * 2f);
        }
        catch (System.Exception e)
        {
            // This will tell us EXACTLY what is crashing
            Debug.LogError("EXCEPTION IN FixedUpdate: " + e.GetType().Name + " - " + e.Message + "\n" + e.StackTrace);

        }
    }
    private Vector3[] TrackPointsForHand(GameObject[] handPoints, string[] points)
    {
        if (points.Length < 21 * 3)
        {
            Debug.LogWarning($"Not enough points: got {points.Length}, expected {21 * 3}");
            return new Vector3[21];
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
        return pointVectors;
    }
    private Vector3 CalculateAvargeFingerPosition(int handIndex, string handData)
    {
        var handPoints = handIndex == 0 ? hand1Points : hand2Points;
        int[] averageIndices = { 7, 8, 11, 12, 15, 16, 19, 20 }; //tips of fingers

        Vector3 avgHand1 = Vector3.zero;
        foreach (int idx in averageIndices)
        {
            avgHand1 += handPoints[idx].transform.localPosition;
        }
        avgHand1 /= averageIndices.Length;
        return avgHand1;
    }


    void OnDisable()
    {
        Debug.LogWarning("HandTracking was DISABLED! " + System.Environment.StackTrace);
    }
}