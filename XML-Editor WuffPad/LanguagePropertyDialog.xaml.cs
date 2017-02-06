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
    /// Interaktionslogik für LanguagePropertyDialog.xaml
    /// </summary>
    public partial class LanguagePropertyDialog : Window
    {
        private bool canCancel;
        public LanguagePropertyDialog(bool firstTime = false, string name = "", string owner = "", string _base = "", string variant = "")
        {
            InitializeComponent();
            canCancel = !firstTime;
            textBoxBase.Text = _base;
            textBoxName.Text = name;
            textBoxOwner.Text = owner;
            textBoxVariant.Text = variant;
        }

        #region Fields
        public string LanguageName
        {
            get
            {
                return textBoxName.Text;
            }
        }

        public string LanguageOwner
        {
            get
            {
                return textBoxOwner.Text;
            }
        }

        public string LanguageBase
        {
            get
            {
                return textBoxBase.Text;
            }
        }

        public string LanguageVariant
        {
            get
            {
                return textBoxVariant.Text;
            }
        }
        #endregion

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            if (canCancel) DialogResult = false;
            else MessageBox.Show("You need to enter a complete set of values to proceed!");
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            execOkay();
        }

        private void execOkay()
        {
            if (textBoxBase.Text == "" || textBoxName.Text == "" || textBoxVariant.Text == "")
            {
                MessageBox.Show("Please fill all required boxes!");
                return;
            }
            DialogResult = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            textBoxName.Focus();
        }
    }
}
