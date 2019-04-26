using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeslaCamViewer
{
    /// <summary>
    /// A single TeslaCam File
    /// </summary>
    public class TeslaCamFile
    {
        public enum CameraType
        {
            UNKNOWN,
            LEFT_REPEATER,
            FRONT,
            RIGHT_REPEATER
        }
        private readonly string FileNameRegex = "([0-9]{4}-[0-9]{2}-[0-9]{2}_[0-9]{2}-[0-9]{2})-([a-z_]*).mp4";
        public string FilePath { get; private set; }
        public string FileName { get { return System.IO.Path.GetFileName(FilePath); } }
        public TeslaCamDate Date { get; private set; }
        public CameraType CameraLocation { get; private set; }
        public string FileDirectory { get { return System.IO.Path.GetDirectoryName(FilePath); } }
        public Uri FileURI { get { return new Uri(this.FilePath); } }

        public TeslaCamFile(string FilePath)
        {
            this.FilePath = FilePath;
            var m = new System.Text.RegularExpressions.Regex(FileNameRegex).Matches(FileName);
            if (m.Count != 1)
                throw new Exception("Invalid TeslaCamFile '" + FileName + "'");
            this.Date = new TeslaCamDate(m[0].Groups[1].Value);
            string cameraType = m[0].Groups[2].Value;
            if (cameraType == "front")
                CameraLocation = CameraType.FRONT;
            else if (cameraType == "left_repeater")
                CameraLocation = CameraType.LEFT_REPEATER;
            else if (cameraType == "right_repeater")
                CameraLocation = CameraType.RIGHT_REPEATER;
            else
                throw new Exception("Invalid Camera Type: '" + cameraType + "'");
        }

    }
}
