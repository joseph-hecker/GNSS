namespace GNSS.Nmea;

[Flags]
public enum NmeaType : uint
{
    Unknown     = 0x00000000,
    GGA         = 0x00000001,
    GPGGA       = GGA,
    GSV         = 0x00000002,
    /// <summary>
    /// Galileo satellites.
    /// </summary>
    GAGSV       = GSV,
    /// <summary>
    /// BeiDou satellites.
    /// </summary>
    GBGSV       = GSV,
    /// <summary>
    /// GLONASS satellites.
    /// </summary>
    GLGSV       = GSV,
    /// <summary>
    /// GPS / SBAS satellites.
    /// </summary>
    GPGSV       = GSV,
    ENAV        = 0x10000000,
}

public enum Fix : int
{
    Invalid  = 0,
    GPS      = 1,
    DGPS     = 2,
    RTKFix   = 4,
    RTKFloat = 5,
}

public enum FixMode : int
{
    None   = 1,
    TwoD   = 2,
    ThreeD = 3,
}

public enum EnavMode : int
{
    GNSS = 0,
    Tilt = 1,
    InvisiblePole = 3,
    ExtensiblePoleTriggerUnpressed = 4,
    ExtensiblePoleTriggerPressed = 5,
}

public enum SmartHandleStatus : int
{
    None = 0,
    StandardHandle = 1,
    SmartHandleOff = 2,
    SmartHandleOn = 3,
}
