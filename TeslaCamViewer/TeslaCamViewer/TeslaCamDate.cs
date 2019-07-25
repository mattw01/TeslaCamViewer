using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeslaCamViewer
{
    public class TeslaCamDate
    {
        private const string FileFormatNoSeconds = "yyyy-MM-dd_HH-mm";
        private const string FileFormatWithSeconds = "yyyy-MM-dd_HH-mm-ss";
        private const string DisplayFormat = "M/d/yyyy h:mm tt";

        public string UTCDateString { get; private set; }
        public string DisplayValue
        {
            get
            {

                return LocalTimeStamp.ToString(DisplayFormat);
            }
        }
        public DateTime UTCTimeStamp
        {
            get
            {
                DateTime dt;
                if (DateTime.TryParseExact(UTCDateString, FileFormatNoSeconds, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)) return dt;
                if (DateTime.TryParseExact(UTCDateString, FileFormatWithSeconds, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)) return dt;
                throw new Exception("Invalid date format: " + UTCDateString);
            }
        }
        public DateTime LocalTimeStamp
        {
            get
            {
                return UTCTimeStamp;
            }
        }

        public TeslaCamDate(string DateString)
        {
            UTCDateString = DateString;
        }

    }
}
