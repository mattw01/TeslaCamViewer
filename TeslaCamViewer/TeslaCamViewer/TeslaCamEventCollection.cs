using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TeslaCamViewer
{
    /// <summary>
    /// Contains multiple TeslaCam File Sets making up one event
    /// Ex. A single Sentry Mode event
    /// </summary>
    public class TeslaCamEventCollection
    {
        public TeslaCamDate StartDate { get; private set; }
        public TeslaCamDate EndDate { get; private set; }
        public List<TeslaCamFileSet> Recordings { get; set; }
        public EventMetadata EventMetadata { get; set; }
        public TeslaCamFile ThumbnailVideo
        {
            get
            {
                return Recordings.First().ThumbnailVideo;
            }
        }

        public TeslaCamEventCollection()
        {
            this.Recordings = new List<TeslaCamFileSet>();
        }

        public bool BuildFromDirectory(string directory)
        {
            // Get list of raw files
            string[] files = Directory.GetFiles(directory, "*.mp4").OrderBy(x=>x).ToArray();

            // Make sure there's at least one valid file
            if (files.Length < 1) { return false; }

            // Convert raw files to cam files
            List<TeslaCamFile> currentTeslaCams =
                files.Where(file => TeslaCamFile.Matches(file)).Select(file => new TeslaCamFile(file)).ToList();

            // Now get list of only distinct events
            List<string> distinctEvents = currentTeslaCams.Select(e => e.Date.UTCDateString).Distinct().ToList();

            // Find the files that match the distinct event
            foreach (var currentEvent in distinctEvents)
            {
                List<TeslaCamFile> matchedFiles = currentTeslaCams.Where(e => e.Date.UTCDateString == currentEvent).ToList();
                TeslaCamFileSet currentFileSet = new TeslaCamFileSet();
                currentFileSet.SetCollection(matchedFiles);
                this.Recordings.Add(currentFileSet);
            }

            // Set metadata
            this.Recordings = Recordings.OrderBy(e => e.Date.UTCDateString).ToList();
            this.StartDate = Recordings.First().Date;
            this.EndDate = Recordings.Last().Date;

            // Success
            return true;
        }
    }
}
