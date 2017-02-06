﻿using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
using System.Xml.Serialization;
using XML_Editor_WuffPad.Commands;
using XML_Editor_WuffPad.XMLClasses;

namespace XML_Editor_WuffPad
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables and constants
        private readonly string fileScratchPath = Environment.CurrentDirectory + "\\language.xml";
        private readonly string dictFilePath = Environment.CurrentDirectory + "\\descriptions.dict";
        private readonly string defaultKeysFilePath = Environment.CurrentDirectory + "\\defaultKeys.list";
        private readonly string settingsDbFilePath = Environment.CurrentDirectory + "\\settings.db";
        private bool fileIsOpen = false;
        private bool textHasChanged = false;
        private bool itemIsOpen = false;
        private bool valueIsOpen = false;
        private bool valueHasChanged = false;
        private string saveDirectory = Environment.CurrentDirectory + "language.xml";
        private string loadDirectory = Environment.CurrentDirectory + "language.xml";
        private XmlStrings loadedFile = new XmlStrings();
        private ObservableCollection<XmlString> currentStringsList = new ObservableCollection<XmlString>();
        private ObservableCollection<string> currentValuesList = new ObservableCollection<string>();
        private XmlString currentString = new XmlString();
        private int currentStringIndex;
        private string currentValue;
        private int currentValueIndex;
        private bool directoryChosen = false;
        private Dictionary<string, string> descriptionDic = new Dictionary<string, string>();
        private List<string> defaultKeysList = new List<string>();
        private int lastClicked = -1;
        private const int clickedItems = 0;
        private const int clickedValues = 1;
        private List<string> commentLines = new List<string>();
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            checkSettingsDb();
            getDictAndDefaultKeys();
            listItemsView.ItemsSource = currentStringsList;
            listValuesView.ItemsSource = currentValuesList;
            updateStatus();
        }

        #region Functionable Methods
        private void setSetting(string key, bool value)
        {
            Dictionary<string, bool> dict = JsonConvert.DeserializeObject<Dictionary<string, bool>>(
                File.ReadAllText(settingsDbFilePath));
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
            File.WriteAllText(settingsDbFilePath, JsonConvert.SerializeObject(dict));
        }

        private bool checkSetting(string key)
        {
            string settingsString = File.ReadAllText(settingsDbFilePath);
            Dictionary<string, bool> settingsDic = 
                JsonConvert.DeserializeObject<Dictionary<string, bool>>(settingsString);
            if (settingsDic.ContainsKey(key))
            {
                return settingsDic[key];
            }
            return false;
        }

        private void checkSettingsDb()
        {
            if (!File.Exists(settingsDbFilePath))
            {
                File.WriteAllText(settingsDbFilePath, 
                    JsonConvert.SerializeObject(new Dictionary<string, bool>()));
            }
        }

        private string getDefaultMissingKey()
        {
            foreach (string s in defaultKeysList)
            {
                bool isPresent = false;
                foreach (XmlString xs in loadedFile.Strings)
                {
                    if (xs.Key == s) isPresent = true;
                }
                if (!isPresent)
                {
                    return s;
                }
            }
            return "";
        }

        private void getDictAndDefaultKeys()
        {
            if (File.Exists(dictFilePath))
            {
                string input = File.ReadAllText(dictFilePath);
                descriptionDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(input);
            }
            if (File.Exists(defaultKeysFilePath))
            {
                string input = File.ReadAllText(defaultKeysFilePath);
                string[] inputs = input.Split('\n');
                foreach (string s in inputs)
                {
                    defaultKeysList.Add(s);
                }
            }
        }

        private void newFile()
        {
            MessageBoxResult result = MessageBox.Show(
                    "Use the english file as a blueprint?", "New File", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                getFileFromScratch();
                currentStringsList.Clear();
                foreach (XmlString s in loadedFile.Strings)
                {
                    s.Description = getDescription(s.Key);
                    loadedFile.Strings.Add(s);
                }
                currentStringsList = loadedFile.Strings;
            }
            fileIsOpen = true;
            textHasChanged = true;
            LanguagePropertyDialog lpd = new LanguagePropertyDialog(firstTime: true);
            if (lpd.ShowDialog() == true)
            {
                loadedFile.Language.Base = lpd.LanguageBase;
                loadedFile.Language.Name = lpd.LanguageName;
                loadedFile.Language.Owner = lpd.LanguageOwner;
                loadedFile.Language.Variant = lpd.LanguageVariant;
            }
            updateStatus();
        }

        private void openFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = loadDirectory;
            ofd.DefaultExt = ".xml";
            ofd.Filter = "XML Documents (.xml)|*.xml";
            bool? result = ofd.ShowDialog();
            if (result == true)
            {
                loadDirectory = ofd.FileName;
                saveDirectory = ofd.FileName;
                directoryChosen = true;
                try
                {
                    loadFile();
                }
                catch
                {
                    //Message should've already popped up
                }
            }
            updateStatus();
        }

        private void loadFile()
        {
            loadedFile = readXmlString(File.ReadAllText(loadDirectory));
            fileIsOpen = true;
            currentStringsList.Clear();
            foreach (XmlString s in loadedFile.Strings)
            {
                s.Description = getDescription(s.Key);
                currentStringsList.Add(s);
            }
            updateStatus();
        }

        private void openLanguageDialog()
        {
            MessageBoxResult res = MessageBox.Show(
                "Only change the language properties if you know what you are doing" +
                " or the language you are translating was never uploaded before!", "Proceed?",
                MessageBoxButton.OKCancel);
            if (res == MessageBoxResult.OK)
            {
                LanguagePropertyDialog lpd = new LanguagePropertyDialog(name: loadedFile.Language.Name, 
                    owner: loadedFile.Language.Owner, _base: loadedFile.Language.Base, 
                    variant: loadedFile.Language.Variant);
                if (lpd.ShowDialog() == true)
                {
                    loadedFile.Language.Base = lpd.LanguageBase;
                    loadedFile.Language.Name = lpd.LanguageName;
                    loadedFile.Language.Owner = lpd.LanguageOwner;
                    loadedFile.Language.Variant = lpd.LanguageVariant;
                }
            }
        }

        #region Big Delete Method
        private void executeDeleteCommand()
        {
            if (lastClicked >= 0)
            {
                switch (lastClicked)
                {
                    case clickedItems:
                        List<XmlString> ls = new List<XmlString>();
                        loadedFile.Strings.Remove((XmlString)listItemsView.SelectedItem);
                        currentStringsList = loadedFile.Strings;
                        currentValuesList = new ObservableCollection<string>();
                        currentString = null;
                        valueHasChanged = true;
                        textBox.Text = "";
                        break;
                    case clickedValues:
                        List<string> ls2 = new List<string>();
                        foreach (string s in currentString.Values)
                        {
                            if (s == currentValue)
                            {
                                ls2.Add(s);
                            }
                        }
                        foreach (string s in ls2)
                        {
                            currentString.Values.Remove(s);
                            currentValuesList = currentString.Values;
                            loadedFile.Strings[currentStringIndex] = currentString;
                            currentStringsList = loadedFile.Strings;
                            valueHasChanged = true;
                            textBox.Text = "";
                        }
                        listItemsView.SelectedIndex = -1;
                        break;
                }
                currentStringsList.Clear();
                foreach (XmlString s in loadedFile.Strings)
                {
                    s.Description = getDescription(s.Key);
                    currentStringsList.Add(s);
                }
                textHasChanged = true;
            }
        }
        #endregion

        #endregion

        #region Status Updating
        public void updateStatus()
        {
            if (fileIsOpen)
            {
                fileCloseMenuItem.IsEnabled = true;
                fileSaveMenuItem.IsEnabled = true;
                listItemsView.IsEnabled = true;
                editLanguageMenuItem.IsEnabled = true;
            }
            else
            {
                fileCloseMenuItem.IsEnabled = false;
                fileSaveMenuItem.IsEnabled = false;
                listItemsView.IsEnabled = false;
                editLanguageMenuItem.IsEnabled = false;
            }
            if (itemIsOpen)
            {
                listValuesView.IsEnabled = true;
                contentColumn.Width = 500;
                contentColumn.Width = double.NaN;
            }
            else
            {
                listValuesView.IsEnabled = false;
                contentColumn.Width = 500;
            }
            if (valueIsOpen)
            {
                textBox.IsEnabled = true;
            }
            else
            {
                textBox.IsEnabled = false;
            }
        }
        #endregion

        #region File and Xml Methods
        private void chooseDirectory()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = saveDirectory;
            sfd.DefaultExt = ".xml";
            sfd.Filter = "XML Documents (.xml)|*.xml";
            bool? result = sfd.ShowDialog();
            if (result == true)
            {
                loadDirectory = sfd.FileName;
                saveDirectory = sfd.FileName;
                directoryChosen = true;
            }
        }

        private XmlStrings readXmlString(string fileString)
        {
            string[] splitted = fileString.Split('\n');
            foreach (string s in splitted)
            {
                if (s.StartsWith("<!--"))
                {
                    commentLines.Add(s);
                }
                else if (s.StartsWith("  <language"))
                {
                    break;
                }
            }
            XmlStrings result;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(XmlStrings));
                using (TextReader tr = new StringReader(fileString))
                {
                    result = (XmlStrings)serializer.Deserialize(tr);
                }
                return result;
            }
            catch
            {
                MessageBox.Show("Failed to load file");
                return null;
            }
        }

        private string serializeXmlToString()
        {

            XmlSerializer serializer = new XmlSerializer(typeof(XmlStrings));
            using (TextWriter tw = new StringWriter())
            {
                serializer.Serialize(tw, loadedFile);
                string[] results = tw.ToString().Split('\n');
                string result = results[0];
                foreach (string s in commentLines)
                {
                    result += "\n" + s;
                }
                bool firstString = true;
                foreach (string s in results)
                {
                    if (!firstString)
                    {
                        result += "\n" + s;
                    }
                    firstString = false;
                }
                return result;
            }
        }

        private void saveXmlFile()
        {
            if (!directoryChosen)
            {
                chooseDirectory();
            }
            string path = saveDirectory;
            string toWrite = serializeXmlToString();
            try
            {
                File.WriteAllText(saveDirectory, toWrite);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            textHasChanged = false;
        }

        private string getDescription(string key)
        {
            if (key != null)
            {
                if (descriptionDic.ContainsKey(key))
                {
                    return descriptionDic[key];
                }
                else
                {
                    return "No description yet.";
                }
            }
            return "No description yet.";
        }

        private void checkForSaved()
        {
            if (textHasChanged)
            {
                MessageBoxResult result = MessageBox.Show("File not saved!\nSave?", "", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    saveXmlFile();
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
        }

        private void getFileFromScratch()
        {
            loadedFile = readXmlString(File.ReadAllText(fileScratchPath));
        }
        #endregion

        #region XAML Stuff
        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (valueIsOpen && !valueHasChanged)
            {
                textHasChanged = true;
                currentString.Values[currentValueIndex] = textBox.Text;
                currentValuesList = currentString.Values;
                loadedFile.Strings[currentStringIndex] = currentString;
                currentStringsList = loadedFile.Strings;
            }
            else if (valueHasChanged)
            {
                valueHasChanged = false;
            }
            updateStatus();
        }

        private void wuffPadWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (textHasChanged)
            {
                MessageBoxResult result = MessageBox.Show("File not saved!\nSave?", "", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    saveXmlFile();
                }
                else if(result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Open)
            {
                if (!checkSetting("commentWarningDisable"))
                {
                    CommentWarningDialog cwd = new CommentWarningDialog();
                    if (cwd.ShowDialog() == true)
                    {
                        setSetting("commentWarningDisable", cwd.DoNotShowAgain);
                    }
                }
                bool closed = true;
                if (fileIsOpen)
                {
                    closed = closeFile();
                }
                if (closed) openFile();
            }
            else if(e.Command == ApplicationCommands.Save)
            {
                saveXmlFile();
            }
            else if(e.Command == ApplicationCommands.Close)
            {
                closeFile();
            }
            else if(e.Command == ApplicationCommands.New)
            {
                if(closeFile()) newFile();
            }
            else if (e.Command == ApplicationCommands.Delete)
            {
                executeDeleteCommand();
            }
            else if (e.Command == CustomCommands.LanguageProperties)
            {
                openLanguageDialog();
            }
            else
            {
                throw new NotImplementedException(
                    "Tried to call a command that was not implemented");
            }
            updateStatus();
        }

        private void listItemsView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = listItemsView.SelectedIndex;
            if (index >= 0)
            {
                currentStringIndex = index;
                currentString = loadedFile.Strings[index];
                currentValuesList = currentString.Values;
                listValuesView.ItemsSource = currentValuesList;
                itemIsOpen = true;
            }
            else
            {
                itemIsOpen = false;
            }
            updateStatus();
            lastClicked = 0;
        }

        private void listValuesView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = listValuesView.SelectedIndex;
            if (index >= 0)
            {
                showValue(currentString.Values[index]);
                currentValueIndex = index;
                valueIsOpen = true;
                valueHasChanged = true;
            }
            updateStatus();
            lastClicked = 1;
        }

        private void cmItemsAdd_Click(object sender, RoutedEventArgs e)
        {
            NewStringDialog nsd = new NewStringDialog(getDefaultMissingKey());
            if (nsd.ShowDialog() == true)
            {
                string key = nsd.Key;
                bool isPresent = false;
                foreach (XmlString s in loadedFile.Strings)
                {
                    if (s.Key == key) isPresent = true;
                }
                if (!isPresent)
                {
                    XmlString xs = new XmlString();
                    xs.Key = key;
                    loadedFile.Strings.Add(xs);
                    currentStringsList = loadedFile.Strings;
                    currentString = xs;
                    currentValue = null;
                    textHasChanged = true;
                    listItemsView.SelectedIndex = loadedFile.Strings.Count - 1;
                    listItemsView.ScrollIntoView(loadedFile.Strings[loadedFile.Strings.Count - 1]);
                }
                else
                {
                    MessageBox.Show("A string with that key is already present");
                }
            }
        }

        private void notList_MouseDown(object sender, MouseButtonEventArgs e)
        {
            lastClicked = -1;
        }

        private void cmValuesAdd_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Create a new value?", "", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                foreach (XmlString s in loadedFile.Strings)
                {
                    if (s.Key == currentString.Key)
                    {
                        s.Values.Add("Add text here");
                        break;
                    }
                }
                currentValuesList = currentString.Values;
                textHasChanged = true;
                showValues(currentString);
                showValue(currentValue);
                listValuesView.SelectedIndex = currentString.Values.Count - 1;
                listValuesView.ScrollIntoView(currentString.Values[currentString.Values.Count - 1]);
            }
        }
        #endregion
        
        #region Display Control
        private void showValues(XmlString s)
        {
            currentString = s;
        }

        private void showValue(string s)
        {
            textBox.Text = s;
            currentValue = s;
        }

        private bool closeFile()
        {
            if (textHasChanged)
            {
                MessageBoxResult result = MessageBox.Show("File not saved!\nSave?", "", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    saveXmlFile();
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return false;
                }
            }
            textBox.Clear();
            textHasChanged = false;
            currentString = new XmlString();
            currentStringIndex = -1;
            loadedFile.Strings = new ObservableCollection<XmlString>();
            currentStringsList = loadedFile.Strings;
            currentValue = null;
            currentValueIndex = -1;
            currentString.Values.Clear();
            fileIsOpen = false;
            textHasChanged = false;
            itemIsOpen = false;
            valueIsOpen = false;
            directoryChosen = false;
            lastClicked = -1;
            valueHasChanged = false;
            return true;
        }
        #endregion
    }
}