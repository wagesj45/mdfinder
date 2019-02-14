using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mdfinder
{
    /// <summary> Scans directories, logging files and their attributes. </summary>
    public static class Scanner
    {
        #region Members

        /// <summary> Event queue for all listeners interested in FilesFound events. </summary>
        public static event EventHandler<FilesFoundEventArgs> FilesFound;

        /// <summary> Event queue for all listeners interested in DirectoryFound events. </summary>
        public static event EventHandler<DirectoryFoundEventArgs> DirectoryFound;

        /// <summary> Event queue for all listeners interested in ReportProgress events. </summary>
        public static event EventHandler<ProgressReportEventArgs> ReportProgress;

        #endregion

        #region Properties

        public static uint Processed { get; private set; }

        public static uint Total { get; private set; }

        public static bool IsScanning { get; private set; }

        #endregion

        #region Methods

        public static void Scan(string path)
        {
            Processed = 0;
            Total = 0;

            var scanPath = new DirectoryInfo(path);
            if (scanPath.Exists)
            {
                Scan(scanPath);
            }
        }

        private static void Scan(DirectoryInfo directory)
        {
            var files = directory.GetFiles();
            var fileBatches = files.Bin(Properties.Settings.Default.FilesFoundAlert);
            var subdirectories = directory.GetDirectories();

            Total += (uint)files.Count();

            foreach (var subdirectory in subdirectories)
            {
                OnDirectoryFound(subdirectory);

                Scan(subdirectory);
            }

            foreach (var batch in fileBatches)
            {
                OnFilesFound(batch);

                Processed += (uint)batch.Count();

                OnReportProgress(Processed, Total);
            }
        }

        /// <summary> Executes the files found action. </summary>
        /// <param name="files"> The files. </param>
        private static void OnFilesFound(IEnumerable<FileInfo> files)
        {
            FilesFound?.Invoke(null, new FilesFoundEventArgs(files));
        }

        /// <summary> Executes the directory found action. </summary>
        /// <param name="directory"> Pathname of the directory. </param>
        private static void OnDirectoryFound(DirectoryInfo directory)
        {
            DirectoryFound?.Invoke(null, new DirectoryFoundEventArgs(directory));
        }

        /// <summary> Executes the report progress action. </summary>
        /// <param name="processed"> The processed. </param>
        /// <param name="total">     Number of. </param>
        private static void OnReportProgress(uint processed, uint total)
        {
            ReportProgress?.Invoke(null, new ProgressReportEventArgs(processed, total));
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

            }

            #endregion
        }

        #endregion
    }
}
