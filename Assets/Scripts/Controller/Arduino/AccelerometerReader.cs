using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class AccelerometerReader : MonoBehaviour
{
    private SerialPort serial;
    public string portName = "COM3";
    public int baudRate = 115200;

    public Vector3 latestAcceleration { get; private set; } = Vector3.zero;
    public Vector3 latestGyro { get; private set; } = Vector3.zero;

    private Thread readThread;
    private bool keepReading = true;

    void Start()
    {
        serial = new SerialPort(portName, baudRate);
        serial.ReadTimeout = 100;

        try
        {
            serial.Open();
            readThread = new Thread(ReadSerial);
            readThread.Start();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Serial port failed: " + e.Message);
        }
    }

    private void ReadSerial()
    {
        while (keepReading && serial != null && serial.IsOpen)
        {
            try
            {
                string line = serial.ReadLine();
                string[] values = line.Split(',');

                if (values.Length == 6)
                {
                    float ax = float.Parse(values[0]);
                    float ay = float.Parse(values[1]);
                    float az = float.Parse(values[2]);
                    float gx = float.Parse(values[3]);
                    float gy = float.Parse(values[4]);
                    float gz = float.Parse(values[5]);

                    latestAcceleration = new Vector3(ax, ay, az);
                    latestGyro = new Vector3(gx, gy, gz);
                }
            }
            catch { /* skip invalid lines */ }
        }
    }

    void OnDestroy()
    {
        keepReading = false;
        if (readThread != null && readThread.IsAlive) readThread.Join();
        if (serial != null && serial.IsOpen) serial.Close();
    }
}