using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace TeslaCamViewer
{
    /// <summary>
    /// A set of multiple matched TeslaCamFiles (multiple camera angles)
    /// </summary>
    public class TeslaCamFileSet
    {
        public TeslaCamDate Date { get; private set; }
        public List<TeslaCamFile> Cameras { get; private set; }
        public string Directory { get; private set; }

        public TeslaCamFile ThumbnailVideo
        {
            get
            {
                return Cameras.First(e => e.CameraLocation == TeslaCamFile.CameraType.FRONT);
            }
        }

        public void SetCollection(List<TeslaCamFile> cameras)
        {
            this.Cameras = cameras;
            this.Date = cameras.First().Date;
            this.Directory = cameras.Select(file => file.FileDirectory).Distinct().First();
        }

        private Lazy<EventMetadata> eventMetadata;

        public EventMetadata EventMetadata
        {
            get { return eventMetadata.Value;  }
        }

        public TeslaCamFileSet()
        {
            eventMetadata = new Lazy<EventMetadata>(computeEventMetadata);
        }

        private EventMetadata computeEventMetadata() {
            var eventMetadata = Path.Combine(Directory, "event.json");
            if (!File.Exists(eventMetadata))
            {
                return null;
            }

            var json = File.ReadAllText(eventMetadata);
            return JsonSerializer.Deserialize<EventMetadata>(json);
        }
    }
}
