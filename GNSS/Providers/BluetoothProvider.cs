using System.IO.Ports;

namespace GNSS.Providers;

public class BluetoothProvider : ILocationProvider
{
    public event ILocationProvider.OnNmeaRecievedEventHandler? OnNmeaRecieved;

    private SerialPort SerialPort { get; set; }

    public BluetoothProvider(string comPort, int baudRate)
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

    public void Start(Dictionary<string, string>? ntripOptions = null)
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
