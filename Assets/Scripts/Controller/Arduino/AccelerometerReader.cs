using UnityEngine;
using System.IO.Ports;

public class AccelerometerReader : MonoBehaviour
{
    SerialPort serial;
    public string portName = "COM3"; // Arduinoの接続ポートに合わせて変更
    public int baudRate = 115200;

    public Vector3 acceleration;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        serial = new SerialPort(portName, baudRate);
        serial.ReadTimeout = 100;
        try
        {
            serial.Open();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Serial port failed: " + e.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (serial != null && serial.IsOpen)
        {
            try
            {
                string line = serial.ReadLine(); // 例: "0.001,-0.002,0.998"
                string[] values = line.Split(',');
                if (values.Length == 3)
                {
                    float x = float.Parse(values[0]);
                    float y = float.Parse(values[1]);
                    float z = float.Parse(values[2]);
                    acceleration = new Vector3(x, y, z);
                }
            }
            catch (System.Exception) { }
        }
    }
    void OnDestroy()
    {
        if (serial != null && serial.IsOpen)
            serial.Close();
    }
}