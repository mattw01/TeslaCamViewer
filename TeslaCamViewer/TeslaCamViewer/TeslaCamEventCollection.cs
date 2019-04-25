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

        public void BuildFromDirectory(string Directory)
        {
            // Get list of files
            string[] Files = System.IO.Directory.GetFiles(Directory).OrderBy(x=>x).ToArray();

            List<TeslaCamFile> CurrentTeslaCams = new List<TeslaCamFile>();

            foreach (var File in Files)
            {
                try
                {
                    TeslaCamFile f = new TeslaCamFile(File);
                    CurrentTeslaCams.Add(f);
                }
                catch
                {
                }
            }
            List<string> DistinctEvents = CurrentTeslaCams.Select(e => e.Date.UTCDateString).Distinct().ToList();

            foreach (var CurrentEvent in DistinctEvents)
            {
                List<TeslaCamFile> MatchedFiles = CurrentTeslaCams.Where(e => e.Date.UTCDateString == CurrentEvent).ToList();
                TeslaCamFileSet CurrentFileSet = new TeslaCamFileSet();

                CurrentFileSet.SetCollection(MatchedFiles);
                this.Recordings.Add(CurrentFileSet);
            }
            this.Recordings = Recordings.OrderBy(e => e.Date.UTCDateString).ToList();
            this.StartDate = Recordings.First().Date;
            this.EndDate = Recordings.Last().Date;
            
            
        }
    }
}
