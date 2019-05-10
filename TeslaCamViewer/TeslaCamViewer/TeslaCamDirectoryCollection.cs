using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace TeslaCamViewer
{
    /// <summary>
    /// A collection of multiple TeslaCam events
    /// Ex. All Sentry Mode recordings
    /// </summary>
    public class TeslaCamDirectoryCollection
    {
        public string DisplayName { get; private set; }
        public ObservableCollection<TeslaCamEventCollection> Events { get; set; }

        public TeslaCamDirectoryCollection()
        {
            this.Events = new ObservableCollection<TeslaCamEventCollection>();
        }
        public void SetDisplayName(string Name)
        {
            this.DisplayName = Name;
        }
        public void BuildFromBaseDirectory(string Directory)
        {
            string[] Directories = System.IO.Directory.GetDirectories(Directory);
            foreach (var Dir in Directories)
            {
                var e = new TeslaCamEventCollection();
                if (e.BuildFromDirectory(Dir))
                {
                    this.Events.Add(e);
                }
            }
            string[] BaseFiles = System.IO.Directory.GetFiles(Directory);
            if (BaseFiles.Count() > 0)
            {
                var baseCollection = new TeslaCamEventCollection();
                baseCollection.BuildFromDirectory(Directory);
                this.Events.Add(baseCollection);
            }
            this.Events = new ObservableCollection<TeslaCamEventCollection>(Events.OrderBy(e => e.StartDate.UTCDateString));
            this.DisplayName = new System.IO.DirectoryInfo(Directory).Name;
        }
    }
}
