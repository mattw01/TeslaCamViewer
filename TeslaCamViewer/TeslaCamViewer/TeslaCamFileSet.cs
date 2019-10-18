namespace TeslaCamViewer
{
    using System.Collections.Generic;
    using System.Linq;

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
