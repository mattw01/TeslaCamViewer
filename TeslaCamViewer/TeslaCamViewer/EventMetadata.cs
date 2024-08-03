using System;

namespace TeslaCamViewer
{
    public class EventMetadata
    {
        public DateTime Timestamp { get; set; }

        public string City { get; set; }

        public double EstLat { get; set; }

        public double EstLon { get; set; }

        public string Reason { get; set; }

        public string Camera { get; set; }
    }
}
