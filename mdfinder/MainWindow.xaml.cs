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

namespace mdfinder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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

        #endregion

        #region Constructors

        /// <summary> Default constructor for the main window of the application. </summary>
        public MainWindow()
        {
            this.Database = new DBHelper();
            this.Scanner = new Scanner();
            this.DefaultProvider = new MD5HashProvider();
            this.HashProviders = GetProviderPlugins();

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
            if (!this.Scanner.IsScanning)
            {
                new Thread(() =>
                {
                    this.Scanner.Scan(location);
                    this.Dispatcher.Invoke(() => txtProgressLabel.Content = string.Empty);
                    this.Dispatcher.Invoke(() => datagridFileRecords.ItemsSource = this.Database.GetFileRecords());
                }).Start();
            }
        }

        /// <summary> Event handler. Called by DatagridFileRecords for initialized events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Event information. </param>
        private void DatagridFileRecords_Initialized(object sender, EventArgs e)
        {
            this.datagridFileRecords.ItemsSource = this.Database.GetFileRecords();
        }

        /// <summary> Event handler. Called by BtnFilterDuplicates for click events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void BtnFilterDuplicates_Click(object sender, RoutedEventArgs e)
        {
            this.datagridFileRecords.ItemsSource = new ListCollectionView(this.Database.GetFileRecords().GroupBy(fr => fr.Hash).Where(g => g.Count() > 1).SelectMany(g => g).ToList());
            ((ListCollectionView)this.datagridFileRecords.ItemsSource).GroupDescriptions.Add(new PropertyGroupDescription("Hash"));
        }

        /// <summary> Event handler. Called by BtnFilterShowAll for click events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void BtnFilterShowAll_Click(object sender, RoutedEventArgs e)
        {
            this.datagridFileRecords.ItemsSource = this.Database.GetFileRecords();
        }

        /// <summary> Event handler. Called by BtnClear for click events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            this.Database.Clear();
            this.datagridFileRecords.ItemsSource = Enumerable.Empty<FileRecord>();
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

        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow();

            aboutWindow.Show();
        }
        #endregion


    }
}
