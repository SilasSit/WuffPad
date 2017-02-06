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
using System.Windows.Shapes;

namespace XML_Editor_WuffPad
{
    /// <summary>
    /// Interaktionslogik für CommentWarningDialog.xaml
    /// </summary>
    public partial class CommentWarningDialog : Window
    {
        public CommentWarningDialog()
        {
            InitializeComponent();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        public bool DoNotShowAgain
        {
            get
            {
                return checkBoxDontShowAgain.IsChecked == true;
            }
        }
    }
}
