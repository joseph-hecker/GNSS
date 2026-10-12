namespace GNSS.Nmea;

public class Satellite(int prn, int elevation, int azimuth, int snr) : IEquatable<Satellite>
{
    public int PRN { get; } = prn;
    public int Elevation { get; } = elevation;
    public int Azimuth { get; } = azimuth;
    public int SNR { get; } = snr;

    public Satellite Copy()
    {
        return new(PRN, Elevation, Azimuth, SNR);
    }

    public bool Equals(Satellite? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return PRN == other.PRN &&
               Elevation == other.Elevation &&
               Azimuth == other.Azimuth &&
               SNR == other.SNR;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Satellite);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(PRN, Elevation, Azimuth, SNR);
    }

    public override string ToString()
    {
        return $"(PRN: {PRN}, Elevation: {Elevation}, Azimuth: {Azimuth}, SNR: {SNR})";
    }
}
