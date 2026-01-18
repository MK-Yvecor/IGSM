using System;
using System.Collections.Generic;
using System.Text;

namespace IGSM
{
    struct BasicTelemetry
    {
        public double Tempereature;
        public double Pressure;
        public double Voltage;
        public double Altitude;
    }

    struct GNSS_Data
    {
        public int Satelites;
        public string Longitude;
        public string Latitude;
    }

    struct LoRa_Data
    {
        public int RSSI;
    }

    struct IMUTelemetry
    {
        public double AccelX;
        public double AccelY;
        public double AccelZ;
        public double GyroX;
        public double GyroY;
        public double GyroZ;
    }
    struct EX_01_Data
    {
        public double RChannel;
        public double GChannel;
        public double BChannel;
    }
}
