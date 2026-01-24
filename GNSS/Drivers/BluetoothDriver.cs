using System.IO.Ports;

namespace GNSS.Drivers;

public class BluetoothDriver : IDriver
{
    public event IDriver.OnNmeaRecievedEventHandler? OnNmeaRecieved;

    private SerialPort SerialPort { get; set; }

    public BluetoothDriver(string comPort, int baudRate)
    {
        SerialPort = new SerialPort(comPort, baudRate);
    }

    private void DataRecieved(object sender, SerialDataReceivedEventArgs e)
    {
        string data = SerialPort.ReadLine();
        if (data.StartsWith('$'))
        {
            OnNmeaRecieved?.Invoke(data);
        }
    }

    public void Start(Dictionary<string, string>? ntripOptions)
    {
        SerialPort.DataReceived += DataRecieved;
        SerialPort.Open();
    }

    public void Stop()
    {
        SerialPort.Close();
        SerialPort.DataReceived -= DataRecieved;
    }
}
