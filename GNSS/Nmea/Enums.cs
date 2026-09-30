namespace GNSS.Nmea;

[Flags]
public enum NmeaType : uint
{
    Unknown = 0x00000000,

    GGA = 0x00000001,
    GPGGA = GGA,
    GNGGA = GGA,
    GLGGA = GGA,
    GAGGA = GGA,
    GBGGA = GGA,

    GLL = 0x00000002,
    GPGLL = GLL,
    GNGLL = GLL,
    GLGLL = GLL,
    GAGLL = GLL,
    GBGLL = GLL,

    GSA = 0x00000004,
    GPGSA = GSA,
    GNGSA = GSA,
    GLGSA = GSA,
    GAGSA = GSA,
    GBGSA = GSA,

    GST = 0x00000008,
    GPGST = GST,
    GNGST = GST,
    GLGST = GST,
    GAGST = GST,
    GBGST = GST,

    GSV = 0x00000010,
    GPGSV = GSV,
    GNGSV = GSV,
    GLGSV = GSV,
    GAGSV = GSV,
    GBGSV = GSV,

    RMC = 0x00000020,
    GPRMC = RMC,
    GNRMC = RMC,
    GLRMC = RMC,
    GARMC = RMC,
    GBRMC = RMC,

    RME = 0x00000040,
    PGRME = RME,

    VTG = 0x00000080,
    GPVTG = VTG,
    GNVTG = VTG,
    GLVTG = VTG,
    GAVTG = VTG,
    GBVTG = VTG,

    ENAV = 0x00000100,
}

public enum FixQuality : int
{
    Invalid = 0,
    GPS = 1,
    DGPS = 2,
    PPS = 3,
    RTKFix = 4,
    RTKFloat = 5,
    Estimated = 6,
    Manual = 7,
    Simulation = 8,
}

public enum PositionMode
{
    Invalid = 0,
    Autonomous = 1,
    Differential = 2,
    Estimated = 3,
    Manual = 4,
    Simulated = 5,
}

public enum FixMode : int
{
    None = 1,
    TwoD = 2,
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
