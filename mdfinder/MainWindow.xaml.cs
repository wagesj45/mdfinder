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

namespace mdfinder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties

        public DBHelper Database { get; set; }

        public Scanner Scanner { get; set; }

        #endregion

        #region Constructors

        /// <summary> Default constructor for the main window of the application. </summary>
        public MainWindow()
        {
            this.Database = new DBHelper();
            this.Scanner = new Scanner();

            this.Scanner.DirectoryFound += (sender, args) => Dispatcher.Invoke(() => txtScanLocation.Text = args.Directory.Name);
            //this.Scanner.FilesFound += (sender, args) => args.Files;
            this.Scanner.ReportProgress += (sender, args) => Dispatcher.Invoke(() => { if (args.Processed > 0) { this.progressBar.Value = args.Percentage * 100; } });

            InitializeComponent();
        }

        #endregion

        #region Methods

        private void btnFilePicker_Click(object sender, RoutedEventArgs e)
        {
            var fbd = new FolderBrowserDialog();

            if(fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtScanLocation.Text = fbd.SelectedPath;
            }
        }

        private void btnScan_Click(object sender, RoutedEventArgs e)
        {
            var location = txtScanLocation.Text;
            if (!this.Scanner.IsScanning)
            {
                new Thread(() =>
                {
                    this.Scanner.Scan(location);
                }).Start();
            }
        }

        #endregion
    }
}
