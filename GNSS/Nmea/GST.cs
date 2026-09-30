using GNSS.Nmea.Exceptions;

namespace GNSS.Nmea;

public class GST : NmeaSentence
{
    public TimeOnly Time { get; }
    public double RMSRange { get; }
    public double MajorStd { get; }
    public double MinorStd { get; }
    public double MajorAngle { get; }
    public double LatStd { get; }
    public double LonStd { get; }
    public double AltStd { get; }

    public GST(string message) : base(message)
    {
        try
        {
            Time = ParseTime(_parts[1]);
            RMSRange = double.Parse(_parts[2]);
            MajorStd = double.Parse(_parts[3]);
            MinorStd = double.Parse(_parts[4]);
            MajorAngle = double.Parse(_parts[5]);
            LatStd = double.Parse(_parts[6]);
            LonStd = double.Parse(_parts[7]);
            AltStd = double.Parse(_parts[8]);
        }
        catch (Exception ex)
        {
            throw new NmeaException($"Error parsing GST message. ({_message})", ex);
        }
    }

    public override void UpdateGPSData(GPSData data)
    {
        data.UTCTime = Time;
        data.RMSRange = RMSRange;
        data.MajorStd = MajorStd;
        data.MinorStd = MinorStd;
        data.MajorAngle = MajorAngle;
        data.LatStd = LatStd;
        data.LonStd = LonStd;
        data.AltStd = AltStd;
        data.EstimatedAccuracy = Math.Sqrt(LatStd * LatStd + LonStd * LonStd);
    }
}
