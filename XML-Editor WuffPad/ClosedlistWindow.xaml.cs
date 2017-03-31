using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaktionslogik für ClosedlistWindow.xaml
    /// </summary>
    public partial class ClosedlistWindow : Window
    {
        string html;
        public ClosedlistWindow(string header, string htmlContent)
        {
            html = "<h3>" + header + "</h3>" + htmlContent;
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            webBrowser.NavigateToString(html);
        }
    }
}
