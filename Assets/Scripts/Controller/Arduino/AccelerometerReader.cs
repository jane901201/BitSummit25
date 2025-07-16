using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class AccelerometerReader : MonoBehaviour
{
    private SerialPort serial;
    public string portName = "COM4";
    public int baudRate = 9600;

    public Vector3 latestAcceleration { get; private set; } = Vector3.zero;
    public Vector3 latestGyro { get; private set; } = Vector3.zero;
    public Vector3 rawLatestAcceleration { get; private set; } = Vector3.zero;

    private Thread readThread;
    private bool keepReading = true;

    void Start()
    {
        serial = new SerialPort(portName, baudRate);

        try
        {
            serial.Open();
            serial.ReadTimeout = 500;
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

                if (values.Length == 9)
                {
                    float ax = float.Parse(values[0]);
                    float ay = float.Parse(values[1]);
                    float az = float.Parse(values[2]);
                    float gx = float.Parse(values[3]);
                    float gy = float.Parse(values[4]);
                    float gz = float.Parse(values[5]);

                    // ハイパス無し
                    float ax_raw = float.Parse(values[6]);
                    float ay_raw = float.Parse(values[7]);
                    float az_raw = float.Parse(values[8]);

                    latestAcceleration = new Vector3(ax, ay, az);
                    latestGyro = new Vector3(gx, gy, gz);
                    rawLatestAcceleration = new Vector3(ax_raw, ay_raw, az_raw);
                }
            }
            catch (System.TimeoutException)
            {
                // タイムアウト → loopを続ける
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Serial read error: " + e.Message);
                // 他の例外（切断など）はループを抜ける
                break;
            }
        }
    }

    void OnDestroy()
    {
        keepReading = false;

        if (serial != null && serial.IsOpen)
        {
            try
            {
                serial.BaseStream.Flush(); // 強制 wake-up
            }
            catch { }

            // 明示的に閉じて、ReadLineを失敗させる
            serial.Close();
        }

        if (readThread != null && readThread.IsAlive)
        {
            readThread.Join(1000); // 最大1秒待つ。フリーズ対策
        }
    }
}