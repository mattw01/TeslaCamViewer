using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TeslaCamViewer
{
    /// <summary>
    /// A set of multiple matched TeslaCamFiles (multiple camera angles)
    /// </summary>
    public class TeslaCamFileSet
    {
        public TeslaCamDate Date { get; private set; }
        public List<TeslaCamFile> Cameras { get; private set; }

        public TeslaCamFile ThumbnailVideo
        {
            get
            {
                return Cameras.First(e => e.CameraLocation == TeslaCamFile.CameraType.FRONT);
            }
        }

        public void SetCollection(List<TeslaCamFile> Cameras)
        {
            this.Cameras = Cameras;
            this.Date = Cameras.First().Date;
        }
    }
}
