using Senparc.CO2NET.Extensions;
using Senparc.CO2NET.Trace;
using Senparc.ProjectFileManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        public PropertyGroup SelectedFile { get; set; } = new PropertyGroup() { FullFilePath = $"[ no file selectd ] - {SystemTime.Now}" };
        //public ObservableCollection<KeyValuePair<string, string>> BindFileData { get; set; }
        public ObservableCollection<PropertyGroup> ProjectFiles { get; set; } = new ObservableCollection<PropertyGroup>();
        public List<XDocument> ProjectDocuments { get; set; } = new List<XDocument>();

        private bool _inited = false;

        public MainWindow()
        {
            InitializeComponent();
            SenparcTrace.SendCustomLog("System", "Window opened.");

            Init();
        }

        private void Init()
        {
            tabPropertyGroup.Visibility = Visibility.Hidden;
            //BindFileData = new ObservableCollection<KeyValuePair<string, string>>();
            ProjectFiles.Clear();
            ProjectDocuments.Clear();

            lbFiles.ItemsSource = ProjectFiles;

            if (!_inited)
            {
                lbFiles.DataContext = ProjectFiles;

                _inited = true;
            }

            SelectedFile.FullFilePath = $"[ no file selectd ] - {SystemTime.Now}";

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
            if (csprojFiles == null || csprojFiles.Length == 0)
            {
                MessageBox.Show("No valiable .csproj file！", "error");
                return;
            }

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

            //lbFiles.DataContext = ProjectFiles;
            //lbFiles.ItemsSource = ProjectFiles;

            //foreach (var projectFile in ProjectFiles)
            //{
            //    BindFileData.Add(new KeyValuePair<string, string>(projectFile.FileName, projectFile.FullFilePath));
            //}
            //lbFiles.ItemsSource = BindFileData;
        }

        private void lbFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedData = (PropertyGroup)e.AddedItems[0];
            SelectedFile = selectedData;

            tbFilePath.DataContext = SelectedFile;

            #region TabItems

            //Assemble
            txtTargetFramework.DataContext = SelectedFile;
            txtTargetFramework.IsEnabled = lblTargetFramework.IsEnabled = !SelectedFile.TargetFramework.IsNullOrEmpty();

            txtTargetFrameworks.DataContext = SelectedFile;
            txtTargetFrameworks.IsEnabled =  lblTargetFrameworks.IsEnabled =!SelectedFile.TargetFrameworks.IsNullOrEmpty();

            txtVersion.DataContext = SelectedFile;
            txtAssemblyName.DataContext = SelectedFile;
            txtRootNamespace.DataContext = SelectedFile;

            //Introductions
            txtTitle.DataContext = SelectedFile;
            txtCopyright.DataContext = SelectedFile;
            txtDescription.DataContext = SelectedFile;
            txtAuthors.DataContext = SelectedFile;
            txtOwners.DataContext = SelectedFile;
            txtSummary.DataContext = SelectedFile;

            //Package
            txtPackageTags.DataContext = SelectedFile;
            txtPackageLicenseUrl.DataContext = SelectedFile;
            txtProjectUrl.DataContext = SelectedFile;
            txtPackageProjectUrl.DataContext = SelectedFile;
            txtPackageIconUrl.DataContext = SelectedFile;
            txtRepositoryUrl.DataContext = SelectedFile;

            //PackageReleaseNotes
            txtPackageReleaseNotes.DataContext = SelectedFile;

            #endregion


            //lblFilePath.DataContext = SelectedFile;
            //lblFilePath.Content = SelectedFile.FullFilePath;
        }

        private void linkSourceCode_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Hyperlink link = sender as Hyperlink;
            Process.Start(new ProcessStartInfo(link.NavigateUri.AbsoluteUri));
        }

        private void btnCurrentMajorVersionPlus_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
