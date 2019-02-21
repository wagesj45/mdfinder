using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mdfinder
{
    /// <summary> Scans directories, logging files and their attributes. </summary>
    public class Scanner : PropertyChangedAlerter
    {
        #region Members

        /// <summary> Event queue for all listeners interested in FilesFound events. </summary>
        public event EventHandler<FilesFoundEventArgs> FilesFound;

        /// <summary> Event queue for all listeners interested in DirectoryFound events. </summary>
        public event EventHandler<DirectoryFoundEventArgs> DirectoryFound;

        /// <summary> Event queue for all listeners interested in ReportProgress events. </summary>
        public event EventHandler<ProgressReportEventArgs> ReportProgress;

        private uint processed;

        private uint total;

        private bool isScanning;

        #endregion

        #region Properties

        public uint Processed
        {
            get
            {
                return this.processed;
            }
            private set
            {
                this.processed = value;
                OnPropertyChanged();
            }
        }

        public uint Total
        {
            get
            {
                return this.total;
            }
            private set
            {
                this.total = value;
                OnPropertyChanged();
            }
        }

        public bool IsScanning
        {
            get
            {
                return this.isScanning;
            }
            private set
            {
                this.isScanning = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Methods

        public void Scan(string path)
        {
            this.Processed = 0;
            this.Total = 0;

            var scanPath = new DirectoryInfo(path);
            if (scanPath.Exists)
            {
                Discover(scanPath);
                Scan(scanPath);
            }
        }

        private void Discover(DirectoryInfo directory)
        {
            try
            {
                this.Total += (uint)directory.EnumerateFiles().Count();
                foreach (var subdirectory in directory.GetDirectories())
                {
                    OnDirectoryFound(subdirectory);
                    Discover(subdirectory);
                }
            }
            catch (UnauthorizedAccessException unauthorizedAccessException)
            {
                //Ignore and just continue.
            }
            catch (DirectoryNotFoundException directoryNotFoundException)
            {
                //Ignore and continue.
            }
        }

        private void Scan(DirectoryInfo directory)
        {
            try
            {
                var fileBatches = directory.EnumerateFiles().Bin(Properties.Settings.Default.FilesFoundAlert);
                var subdirectories = directory.GetDirectories();

                foreach (var subdirectory in subdirectories)
                {
                    Scan(subdirectory);
                }

                foreach (var batch in fileBatches)
                {
                    OnFilesFound(batch);

                    this.Processed += (uint)batch.Count();

                    OnReportProgress(this.Processed, this.Total);
                }
            }
            catch (UnauthorizedAccessException unauthorizedAccessException)
            {
                //Ignore and just continue.
            }
        }

        /// <summary> Executes the files found action. </summary>
        /// <param name="files"> The files. </param>
        private void OnFilesFound(IEnumerable<FileInfo> files)
        {
            this.FilesFound?.Invoke(this, new FilesFoundEventArgs(files));
        }

        /// <summary> Executes the directory found action. </summary>
        /// <param name="directory"> Pathname of the directory. </param>
        private void OnDirectoryFound(DirectoryInfo directory)
        {
            this.DirectoryFound?.Invoke(this, new DirectoryFoundEventArgs(directory));
        }

        /// <summary> Executes the report progress action. </summary>
        /// <param name="processed"> The processed. </param>
        /// <param name="total">     Number of. </param>
        private void OnReportProgress(uint processed, uint total)
        {
            this.ReportProgress?.Invoke(this, new ProgressReportEventArgs(processed, total));
        }

        #endregion

        #region Subclasses

        /// <summary> Event arguments describing the state of the <see cref="Scanner"/> when it has found files. </summary>
        public class FilesFoundEventArgs : EventArgs
        {
            #region Properties

            /// <summary> Gets or sets the files. </summary>
            /// <value> The files. </value>
            public IEnumerable<FileInfo> Files { get; private set; }

            #endregion

            #region Constructors

            /// <summary> Constructor. </summary>
            /// <param name="files"> The files. </param>
            public FilesFoundEventArgs(IEnumerable<FileInfo> files)
            {
                this.Files = files;
            }

            #endregion
        }

        /// <summary> Event arguments describing the state of the <see cref="Scanner"/> when it has found a directory. </summary>
        public class DirectoryFoundEventArgs : EventArgs
        {
            #region Properties

            /// <summary> Gets or sets the pathname of the directory. </summary>
            /// <value> The pathname of the directory. </value>
            public DirectoryInfo Directory { get; private set; }

            #endregion

            #region Constructors

            /// <summary> Constructor. </summary>
            /// <param name="directory"> The pathname of the directory. </param>
            public DirectoryFoundEventArgs(DirectoryInfo directory)
            {
                this.Directory = directory;
            }

            #endregion
        }

        public class ProgressReportEventArgs : EventArgs
        {
            #region Properties

            /// <summary> Gets or sets the progress as a percentage. </summary>
            /// <value> The percentage. </value>
            public double Percentage
            {
                get
                {
                    return ((double)this.Processed / (double)this.Total);
                }
            }

            /// <summary> Gets or sets the number of processed items. </summary>
            /// <value> The processed. </value>
            public uint Processed { get; private set; }

            /// <summary> Gets or sets the number of items discovered to process.  </summary>
            /// <value> The total. </value>
            public uint Total { get; private set; }

            #endregion

            #region Constructors

            /// <summary> Constructor. </summary>
            /// <param name="processed"> The processed item count. </param>
            /// <param name="total">     The total discovereditem count. </param>
            public ProgressReportEventArgs(uint processed, uint total)
            {
                this.Processed = processed;
                this.Total = total;
            }

            #endregion
        }

        #endregion
    }
}
