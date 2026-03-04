using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTracking : MonoBehaviour
{
    // Start is called before the first frame update
    public UDPReceive udpReceive;
    public GameObject[] handPoints;
    [SerializeField]  float movementScale = 5f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        string data = udpReceive.data;

        data = data.Remove(0, 1);
        data = data.Remove(data.Length-1, 1);
        print(data);
        string[] points = data.Split(',');
        print(points[0]);

        //0        1*3      2*3
        //x1,y1,z1,x2,y2,z2,x3,y3,z3

        for ( int i = 0; i<21; i++)
        {

            float x = (7 - float.Parse(points[i * 3]) / 100) * movementScale;
float y = (float.Parse(points[i * 3 + 1]) / 100) * movementScale;
float z = (float.Parse(points[i * 3 + 2]) / 100) * movementScale;

            handPoints[i].transform.localPosition = new Vector3(x, z, y);

        }


    }
}