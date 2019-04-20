using System;
using System.Collections.Generic;
using System.Linq;
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

        public void SetCollection(List<TeslaCamFile> Cameras)
        {
            this.Cameras = Cameras;
            this.Date = Cameras.First().Date;
        }
    }
}
