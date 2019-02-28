using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Threading;
using mdfinder.hashprovider;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.IO.Compression;

namespace mdfinder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary> The media extentions. </summary>
        private static readonly string[] MEDIA_EXTENTIONS = new[] { ".AVI", ".MPG", ".MPEG", ".MP3", ".MP4", ".MKV", ".WAV" };

        /// <summary> The image extentions. </summary>
        private static readonly string[] IMAGE_EXTENTIONS = new[] { ".JPG", ".JPEG", ".PNG", ".BMP", ".TIF", ".TIFF", ".ICO", "GIF" };

        /// <summary> The text extentions. </summary>
        private static readonly string[] TEXT_EXTENTIONS = new[] { ".TXT", ".XML", ".HTM", ".HTML", ".JS", ".CSS" };


        #region Properties

        /// <summary> Gets or sets the database. </summary>
        /// <value> The database. </value>
        public DBHelper Database { get; set; }

        /// <summary> Gets or sets the scanner. </summary>
        /// <value> The scanner. </value>
        public Scanner Scanner { get; set; }

        /// <summary> Gets or sets the hash providers. </summary>
        /// <value> The hash providers. </value>
        public IEnumerable<IHashProvider> HashProviders { get; set; }

        /// <summary> Gets or sets the default provider. </summary>
        /// <value> The default provider. </value>
        public IHashProvider DefaultProvider { get; set; }

        /// <summary> Gets or sets the scan results. </summary>
        /// <value> The scan results. </value>
        public ScanResults ScanResults { get; set; }

        #endregion

        #region Constructors

        /// <summary> Default constructor for the main window of the application. </summary>
        public MainWindow()
        {
            this.Database = new DBHelper();
            this.Scanner = new Scanner();
            this.DefaultProvider = new MD5HashProvider();
            this.HashProviders = GetProviderPlugins();
            this.ScanResults = new ScanResults();

            this.Scanner.DirectoryFound += (sender, args) => Dispatcher.Invoke(() => txtProgressLabel.Content = args.Directory.Name);
            this.Scanner.FilesFound += (sender, args) =>
            {
                foreach (var file in args.Files)
                {
                    if (Properties.Settings.Default.SkipEmptyFiles && file.Length == 0)
                    {
                        break;
                    }

                    var provider = this.HashProviders.Where(hp => hp.FileExtensions.Contains(file.Extension.ToUpper())).OrderByDescending(hp => hp.Priority).FirstOr(this.DefaultProvider);
                    this.Database.InsertFileRecord(file.FullName, file.Length, provider.GetHash(file), provider.Name);
                    Dispatcher.Invoke(() => txtProgressLabel.Content = file.FullName);
                }
            };
            this.Scanner.ReportProgress += (sender, args) => Dispatcher.Invoke(() => { if (args.Processed > 0) { this.progressBar.Value = args.Percentage * 100; } });

            InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary> Gets the provider plugins from a folder. </summary>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the provider plugins in this
        /// collection.
        /// </returns>
        private IEnumerable<IHashProvider> GetProviderPlugins()
        {
            if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.ProviderFolder) && Directory.Exists(Properties.Settings.Default.ProviderFolder))
            {
                var directory = new DirectoryInfo(Properties.Settings.Default.ProviderFolder);
                foreach (var pluginFile in directory.GetFiles("*.dll"))
                {
                    var assembly = Assembly.LoadFrom(pluginFile.FullName);
                    foreach (var type in assembly.GetTypes().Where(t => t.GetInterface("IHashProvider") != null))
                    {
                        yield return Activator.CreateInstance(type) as IHashProvider;
                    }
                }
            }
        }

        /// <summary> Sets duplicate file collection. </summary>
        /// <param name="duplicates"> The duplicates. </param>
        private void SetDuplicateFileCollection(IEnumerable<DuplicateFileGroup> duplicates)
        {
            this.ScanResults.DuplicateFiles = duplicates;
        }

        /// <summary> Gets the duplicate files in this collection. </summary>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the duplicate files in this
        /// collection.
        /// </returns>
        private IEnumerable<DuplicateFileGroup> GetDuplicateFiles()
        {
            return this.Database.GetFileRecords().GroupBy(fr => fr.Hash).Where(g => g.Count() > 1).Select(g => new DuplicateFileGroup(g)).ToArray();
        }

        /// <summary> Resets the media preview. </summary>
        private void ResetMediaPreview()
        {
            this.mediaPreview.Stop();

            this.mediaPreview.Source = null;
            this.imagePreview.Source = null;
            this.textPreview.Text = string.Empty;

            this.mediaPreviewContainer.Visibility = Visibility.Hidden;
            this.imagePreview.Visibility = Visibility.Hidden;
            this.textPreview.Visibility = Visibility.Hidden;
            this.stackNoPreview.Visibility = Visibility.Visible;
        }

        /// <summary> Event handler. Called by btnFilePicker for click events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void btnFilePicker_Click(object sender, RoutedEventArgs e)
        {
            var fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtScanLocation.Text = fbd.SelectedPath;
            }
        }

        /// <summary> Event handler. Called by btnScan for click events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void btnScan_Click(object sender, RoutedEventArgs e)
        {
            var location = txtScanLocation.Text;
            ResetMediaPreview();
            if (!this.Scanner.IsScanning)
            {
                new Thread(() =>
                {
                    this.Scanner.Scan(location);
                    this.Dispatcher.Invoke(() => txtProgressLabel.Content = string.Empty);
                    this.Dispatcher.Invoke(() => progressBar.Value = 0);
                    this.Dispatcher.Invoke(() => SetDuplicateFileCollection(GetDuplicateFiles()));
                }).Start();
            }
        }

        /// <summary> Event handler. Called by BtnClear for click events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            this.Database.Clear();
            ResetMediaPreview();
            SetDuplicateFileCollection(Enumerable.Empty<DuplicateFileGroup>());
        }

        /// <summary> Event handler. Called by Hyperlink for click events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = (Hyperlink)e.OriginalSource;
            Process.Start(link.NavigateUri.AbsoluteUri);
        }

        /// <summary> Event handler. Called by MenuOptions for click events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void MenuOptions_Click(object sender, RoutedEventArgs e)
        {
            var optionsWindow = new OptionsWindow();

            optionsWindow.ShowDialog();

            this.HashProviders = GetProviderPlugins();
        }

        /// <summary> Event handler. Called by MenuAbout for click events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow();

            aboutWindow.Show();
        }

        /// <summary> Event handler. Called by ListBoxDupes for initialized events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Event information. </param>
        private void ListBoxDupes_Initialized(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                this.Dispatcher.Invoke(() => SetDuplicateFileCollection(GetDuplicateFiles()));
            }).Start();
        }

        /// <summary> Event handler. Called by ListBoxDupes for selection changed events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Selection changed event information. </param>
        private void ListBoxDupes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                this.ScanResults.SelectedDuplicateFileGroup = e.AddedItems[0] as DuplicateFileGroup;
            }
            else
            {
                this.ScanResults.SelectedDuplicateFileGroup = null;
            }
        }

        /// <summary> Event handler. Called by PerformDuplicateAction for click events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void PerformDuplicateAction_Click(object sender, RoutedEventArgs e)
        {
            var tag = (sender as System.Windows.Controls.Button).Tag.ToString();
            var duplicateFileGroup = this.listBoxDupes.SelectedItem as DuplicateFileGroup;
            var actionableFiles = Enumerable.Empty<FileRecord>();
            var archive = this.checkboxArchiveRemainingFiles.IsChecked ?? false;

            ResetMediaPreview();

            if (duplicateFileGroup != null)
            {
                if(tag == "largest")
                {
                     actionableFiles = duplicateFileGroup.FileRecords.OrderByDescending(fr => fr.Size).Skip(1);
                }
                else if(tag == "smallest")
                {
                    actionableFiles = duplicateFileGroup.FileRecords.OrderBy(fr => fr.Size).Skip(1);
                }
                else
                {
                    actionableFiles = duplicateFileGroup.FileRecords.Where(fr => !fr.Keep);
                }

                ZipArchive zipFile = null;

                if (archive)
                {
                    zipFile = ZipFile.Open(System.IO.Path.Combine(Properties.Settings.Default.ArchiveFolder, duplicateFileGroup.Hash + ".zip"), ZipArchiveMode.Update);
                }

                foreach (var file in actionableFiles)
                {
                    if(archive && zipFile != null)
                    {
                        zipFile.CreateEntryFromFile(file.Path.LocalPath, System.IO.Path.GetFileName(file.Path.LocalPath));
                    }

                    File.Delete(file.Path.LocalPath);

                    this.Database.RemoveFileRecord(file.Id);
                }

                if(zipFile != null)
                {
                    zipFile.Dispose();
                }

                SetDuplicateFileCollection(GetDuplicateFiles());
            }
        }

        /// <summary>
        /// Event handler. Called by DatagridFileList for selection changed events.
        /// </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Selection changed event information. </param>
        private void DatagridFileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                var fileRecord = e.AddedItems[0] as FileRecord;
                if (fileRecord != null)
                {
                    var extension = System.IO.Path.GetExtension(fileRecord.Path.LocalPath).ToUpper();

                    if (MEDIA_EXTENTIONS.Contains(extension))
                    {
                        this.mediaPreview.Source = fileRecord.Path;
                        this.imagePreview.Source = null;
                        this.textPreview.Text = string.Empty;

                        this.mediaPreviewContainer.Visibility = Visibility.Visible;
                        this.imagePreview.Visibility = Visibility.Hidden;
                        this.textPreview.Visibility = Visibility.Hidden;
                        this.stackNoPreview.Visibility = Visibility.Hidden;
                    }
                    else if (IMAGE_EXTENTIONS.Contains(extension))
                    {
                        this.mediaPreview.Source = null;
                        this.imagePreview.Source = new BitmapImage(fileRecord.Path);
                        this.textPreview.Text = string.Empty;

                        this.mediaPreviewContainer.Visibility = Visibility.Hidden;
                        this.imagePreview.Visibility = Visibility.Visible;
                        this.textPreview.Visibility = Visibility.Hidden;
                        this.stackNoPreview.Visibility = Visibility.Hidden;
                    }
                    else if (TEXT_EXTENTIONS.Contains(extension))
                    {
                        this.mediaPreview.Source = null;
                        this.imagePreview.Source = null;
                        this.textPreview.Text = File.ReadAllText(fileRecord.Path.LocalPath);

                        this.mediaPreviewContainer.Visibility = Visibility.Hidden;
                        this.imagePreview.Visibility = Visibility.Hidden;
                        this.textPreview.Visibility = Visibility.Visible;
                        this.stackNoPreview.Visibility = Visibility.Hidden;
                    }
                    else //Can't preivew.
                    {
                        ResetMediaPreview();
                    }
                }
            }
        }
    }
    #endregion


}
