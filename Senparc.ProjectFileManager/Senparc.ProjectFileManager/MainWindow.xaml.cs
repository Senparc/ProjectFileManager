using Senparc.CO2NET.Trace;
using Senparc.ProjectFileManager.Models;
using System;
using System.Collections.Generic;
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
        public PropertyGroup SelectedFile { get; set; } = new PropertyGroup() { FullFilePath = "[ no file selectd ]" };
        private List<PropertyGroup> ProjectFiles { get; set; } = new List<PropertyGroup>();
        private List<XDocument> ProjectDocuments { get; set; } = new List<XDocument>(); 

        public MainWindow()
        {
            InitializeComponent();
            SenparcTrace.SendCustomLog("System", "Window opened.");

            //lblFilePath.DataContext = SelectedFile;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var path = txtPath.Text?.Trim();
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                MessageBox.Show("Please input the correct path which includes .csproj files！", "error");
            }

            SenparcTrace.SendCustomLog("Task", "Search .csproj files begin.");

            var csprojFiles = Directory.GetFiles(path, "*.csproj", SearchOption.AllDirectories);
            foreach (var file in csprojFiles)
            {
                try
                {
                    var doc = XDocument.Load(file);
                    var propertyGroup = doc.Root.Elements("PropertyGroup").FirstOrDefault();
                    if (propertyGroup==null)
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

            Dictionary<string, string> lbFilesData = new Dictionary<string, string>();
            foreach (var projectFile in ProjectFiles)
            {
                lbFilesData[projectFile.FileName] = projectFile.FullFilePath;
            }

            lbFiles.ItemsSource = lbFilesData;
        }

    }
}
