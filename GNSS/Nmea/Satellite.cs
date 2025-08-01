using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNSS.Nmea;

public class Satellite(int prn, int elevation, int azimuth, int snr)
{
    public int PRN { get; set; } = prn;
    public int Elevation { get; set; } = elevation;
    public int Azimuth { get; set; } = azimuth;
    public int SNR { get; set; } = snr;
}
