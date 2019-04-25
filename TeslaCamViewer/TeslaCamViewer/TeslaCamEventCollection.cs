using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public bool BuildFromDirectory(string Directory)
        {
            // Get list of raw files
            string[] Files = System.IO.Directory.GetFiles(Directory, "*.mp4").OrderBy(x=>x).ToArray();

            // Make sure there's at least one valid file
            if (Files.Length < 1) { return false; }

            // Create a list of cam files
            List<TeslaCamFile> CurrentTeslaCams = new List<TeslaCamFile>(Files.Length);

            // Convert raw file to cam file
            foreach (var File in Files)
            {
                TeslaCamFile f = new TeslaCamFile(File);
                CurrentTeslaCams.Add(f);
            }

            // Now get list of only distinct events
            List<string> DistinctEvents = CurrentTeslaCams.Select(e => e.Date.UTCDateString).Distinct().ToList();

            // Find the files that match the distinct event
            foreach (var CurrentEvent in DistinctEvents)
            {
                List<TeslaCamFile> MatchedFiles = CurrentTeslaCams.Where(e => e.Date.UTCDateString == CurrentEvent).ToList();
                TeslaCamFileSet CurrentFileSet = new TeslaCamFileSet();

                CurrentFileSet.SetCollection(MatchedFiles);
                this.Recordings.Add(CurrentFileSet);
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
