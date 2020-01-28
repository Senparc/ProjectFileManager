using Senparc.CO2NET.Trace;
using Senparc.ProjectFileManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Xml.Linq;

namespace Senparc.ProjectFileManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public PropertyGroup SelectedFile { get; set; }
        public ObservableCollection<KeyValuePair<string, string>> BindFileData { get; set; }
        private List<PropertyGroup> ProjectFiles { get; set; }
        private List<XDocument> ProjectDocuments { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            SenparcTrace.SendCustomLog("System", "Window opened.");

            Init();
            //lbFiles.ItemsSource = BindFileData;

            //lblFilePath.DataContext = SelectedFile;
        }

        private void Init()
        {
            tabPropertyGroup.Visibility = Visibility.Hidden;
            BindFileData = new ObservableCollection<KeyValuePair<string, string>>();
            ProjectFiles = new List<PropertyGroup>();
            ProjectDocuments = new List<XDocument>();
            SelectedFile = new PropertyGroup() { FullFilePath = $"[ no file selectd ] - {SystemTime.Now}" };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Init();

            var path = txtPath.Text?.Trim();
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                MessageBox.Show("Please input the correct path which includes .csproj files！", "error");
                return;
            }

            SenparcTrace.SendCustomLog("Task", "Search .csproj files begin.");

            var csprojFiles = Directory.GetFiles(path, "*.csproj", SearchOption.AllDirectories);
            foreach (var file in csprojFiles)
            {
                try
                {
                    var doc = XDocument.Load(file);
                    var propertyGroup = doc.Root.Elements("PropertyGroup").FirstOrDefault();
                    if (propertyGroup == null)
                    {
                        throw new Exception($"{file} is not a valid xml.csproj file.");
                    }

                    var projectFile = PropertyGroup.GetObjet(propertyGroup, file);
                    ProjectFiles.Add(projectFile);
                    ProjectDocuments.Add(doc);

                    SenparcTrace.SendCustomLog("Task", $"[Success] Load file:{file}");
                }
                catch (Exception ex)
                {
                    SenparcTrace.SendCustomLog("Task", $"[Faild] Load file:{file}");
                    SenparcTrace.SendCustomLog("Error", ex.Message);
                }
            }

            if (ProjectFiles.Count == 0)
            {
                MessageBox.Show("No valiable .csproj file！", "error");
                return;
            }

            tabPropertyGroup.Visibility = Visibility.Visible;

            foreach (var projectFile in ProjectFiles)
            {
                BindFileData.Add(new KeyValuePair<string, string>(projectFile.FileName, projectFile.FullFilePath));
            }
            //lbFiles.ItemsSource = BindFileData;
        }

        private void lbFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedData = (KeyValuePair<string, string>)e.AddedItems[0];
            SelectedFile = ProjectFiles.First(z => z.FullFilePath == selectedData.Value);
        }
    }
}
