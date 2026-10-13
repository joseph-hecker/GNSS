namespace GNSS.Nmea;

public class NmeaTypeTracker : IDisposable
{
    public static TimeSpan MaxAge => TimeSpan.FromSeconds(5);

    private Timer Timer { get; }
    private Dictionary<NmeaType, DateTime> Tracker { get; set; }

    public NmeaType CurrentNmeaTypes => Tracker.Keys.Aggregate((nmeaTypes, next) => nmeaTypes ^= next);

    public NmeaTypeTracker()
    {
        Tracker = [];
        Timer = new(RemoveStaleNmeaTypes, null, 0, 5000);
    }

    public void Add(NmeaType nmeaType)
    {
        if (!Tracker.TryAdd(nmeaType, DateTime.Now))
        {
            Tracker[nmeaType] = DateTime.Now;
        }
    }

    public bool Remove(NmeaType nmeaType)
    {
        return Tracker.Remove(nmeaType);
    }

    public bool CheckIfActive(NmeaType nmeaType)
    {
        return Tracker.TryGetValue(nmeaType, out DateTime time);
    }

    private void RemoveStaleNmeaTypes(object? state)
    {
        Tracker = Tracker.Where((kvp) => DateTime.Now - kvp.Value < MaxAge).ToDictionary();
    }

    public void Dispose()
    {
        Timer.Dispose();
    }
}
