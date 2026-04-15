using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

public class UDPReceive : MonoBehaviour
{

    Thread receiveThread;
    UdpClient client; 
    public int port = 5052;
    public bool startRecieving = true;
    public bool printToConsole = false;
    public string data;


    public void Start()
    {
        receiveThread = new Thread(
            new ThreadStart(ReceiveDataAsync));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    // receive thread
    private void ReceiveDataAsync()
    {

        client = new UdpClient(port);
        while (startRecieving)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] dataByte = client.Receive(ref anyIP);
                data = Encoding.UTF8.GetString(dataByte);

                if (printToConsole) print(data);
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }
    public void OnDisable()
    {
        if (receiveThread != null)
            receiveThread.Abort();
        if (client != null)
            client.Close();
    }
}
