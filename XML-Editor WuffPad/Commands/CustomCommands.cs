using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XML_Editor_WuffPad.Commands
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand LanguageProperties = new RoutedUICommand(
            "Language properties",
            "Language properties",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.L, ModifierKeys.Control)
            }
            );
    }
}
