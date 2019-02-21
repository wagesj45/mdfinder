using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace mdfinder
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        /// <summary> Gets or sets the supported languages. </summary>
        /// <value> The supported languages. </value>
        public List<CultureInfo> SupportedLanguages { get; set; } = new List<CultureInfo>(new[]
        {
            CultureInfo.CreateSpecificCulture("en-US"),
            //CultureInfo.CreateSpecificCulture("es"),
            //CultureInfo.CreateSpecificCulture("ar"),
            //CultureInfo.CreateSpecificCulture("ja"),
            //CultureInfo.CreateSpecificCulture("ru"),
            //CultureInfo.CreateSpecificCulture("zh-CN"),
        });

        /// <summary> Default constructor. </summary>
        public OptionsWindow()
        {
            InitializeComponent();
        }

        /// <summary> Event handler. Called by BtnSave for click events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();

            this.Close();
        }

        /// <summary>
        /// Event handler. Called by BtnProviderLocationDirectory for click events.
        /// </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void BtnProviderLocationDirectory_Click(object sender, RoutedEventArgs e)
        {
            var fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Properties.Settings.Default.ProviderFolder = fbd.SelectedPath;
            }
        }
    }
}
