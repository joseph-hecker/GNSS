namespace GNSS.Nmea;

public enum NmeaType : uint
{
    Unknown     = 0x00000000,
    GGA         = 0x00000001,
    GPGGA       = GGA,
    GSV         = 0x00000002,
    GPGSV       = GSV,
    //GLGSV       = GSV,
    //GAGSV       = GSV,
    //GBGSV       = GSV,
}

public enum Fix : int
{
    Invalid  = 0,
    GPS      = 1,
    DGPS     = 2,
    RTKFloat = 5,
    RTKFix   = 4,
}

public enum FixMode : int
{
    None   = 1,
    TwoD   = 2,
    ThreeD = 3,
}
