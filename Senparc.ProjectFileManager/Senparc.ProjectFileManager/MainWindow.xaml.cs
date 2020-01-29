using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Senparc.CO2NET.Extensions;
using Senparc.CO2NET.Trace;
using Senparc.ProjectFileManager.Helpers;
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


        private void btnSearch_Click(object sender, RoutedEventArgs e)
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
                        SenparcTrace.SendCustomLog("Task Falid", $"{file} is not a valid xml.csproj file.");
                        continue;
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

            if (lbFiles.Items.Count > 0)
            {
                lbFiles.SelectedIndex = 0;//default select the first item.
            }

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
            if (e.AddedItems.Count == 0)
            {
                return;//when re-research the files, the list will be cleared at first.
            }
            var selectedData = (PropertyGroup)e.AddedItems[0];
            SelectedFile = selectedData;

            tbFilePath.DataContext = SelectedFile;

            #region TabItems

            //Version
            txtVersion.DataContext = SelectedFile;

            //PackageReleaseNotes
            txtPackageReleaseNotes.DataContext = SelectedFile;

            //Introductions
            txtTitle.DataContext = SelectedFile;
            txtCopyright.DataContext = SelectedFile;
            txtAuthors.DataContext = SelectedFile;
            txtDescription.DataContext = SelectedFile;
            txtOwners.DataContext = SelectedFile;
            txtSummary.DataContext = SelectedFile;

            //Package
            txtPackageTags.DataContext = SelectedFile;
            txtPackageLicenseUrl.DataContext = SelectedFile;
            txtProjectUrl.DataContext = SelectedFile;
            txtPackageProjectUrl.DataContext = SelectedFile;
            txtPackageIconUrl.DataContext = SelectedFile;
            txtRepositoryUrl.DataContext = SelectedFile;

            //Assembly
            txtTargetFramework.DataContext = SelectedFile;
            txtTargetFramework.IsEnabled = lblTargetFramework.IsEnabled = !SelectedFile.TargetFramework.IsNullOrEmpty();

            txtTargetFrameworks.DataContext = SelectedFile;
            txtTargetFrameworks.IsEnabled = lblTargetFrameworks.IsEnabled = !SelectedFile.TargetFrameworks.IsNullOrEmpty();

            txtAssemblyName.DataContext = SelectedFile;
            txtRootNamespace.DataContext = SelectedFile;

            #endregion

            tabPropertyGroup.Visibility = Visibility.Visible;
        }

        private void linkSourceCode_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Hyperlink link = sender as Hyperlink;
            System.Diagnostics.Process.Start(new ProcessStartInfo(link.NavigateUri.AbsoluteUri));
            e.Handled = true;
        }

        #region Change Version
        private void ChangeFileVersion(PropertyGroup propertyGroup, Action<VersionObject> versionOperate)
        {
            try
            {
                var version = VersionHelper.GetVersionObject(propertyGroup.Version);
                versionOperate(version);
                propertyGroup.Version = version.ToString();

            }
            catch (Exception ex)
            {
                //some projects many not have a invalid verion number.
                SenparcTrace.SendCustomLog("version not changed", ex.Message);
            }
            finally
            {
                txtVersion.DataContext = SelectedFile;
                txtVersion.Dispatcher.Invoke(() => txtVersion.Text = SelectedFile.Version);
            }

        }

        #region Current Project


        private void btnCurrentMajorVersionPlus_Click(object sender, RoutedEventArgs e)
        {
            ChangeFileVersion(SelectedFile, pg => pg.MajorVersion++);
            //SelectedFile.Version = "changed";
        }

        private void btnCurrentMinorVersionPlus_Click(object sender, RoutedEventArgs e)
        {
            ChangeFileVersion(SelectedFile, pg => pg.MinorVersion++);
        }

        private void btnCurrentIncrementalVersionPlus_Click(object sender, RoutedEventArgs e)
        {
            ChangeFileVersion(SelectedFile, pg => pg.RevisionVersion++);
        }

        private void btnCurrenBuildVersionPlus_Click(object sender, RoutedEventArgs e)
        {
            ChangeFileVersion(SelectedFile, pg => pg.BuildNumberVersion++);
        }
        #endregion

        #region All Projects

        private void btnAllMajorVersionPlus_Click(object sender, RoutedEventArgs e)
        {
            ProjectFiles.ToList().ForEach(pgFile => ChangeFileVersion(pgFile, pg => pg.MajorVersion++));
        }


        private void btnAllMinorVersionPlus_Click(object sender, RoutedEventArgs e)
        {
            ProjectFiles.ToList().ForEach(pgFile => ChangeFileVersion(pgFile, pg => pg.MinorVersion++));
        }

        private void btnAllIncrementalVersionPlus_Click(object sender, RoutedEventArgs e)
        {
            ProjectFiles.ToList().ForEach(pgFile => ChangeFileVersion(pgFile, pg => pg.RevisionVersion++));
        }

        private void btnAllBuildVersionPlus_Click(object sender, RoutedEventArgs e)
        {
            ProjectFiles.ToList().ForEach(pgFile => ChangeFileVersion(pgFile, pg => pg.BuildNumberVersion++));
        }

        #endregion

        #endregion

        private void menuSearch_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            var result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                string filePath = dialog.FileName;
                txtPath.Text = filePath;
                btnSearch_Click(btnSearch, e);
            }
        }

        private void menuSourceCode_Click(object sender, RoutedEventArgs e)
        {
            var iePath = Environment.ExpandEnvironmentVariables(
         @"%PROGRAMFILES%\Internet Explorer\iexplore.exe");
            System.Diagnostics.Process.Start(iePath, "https://github.com/Senparc/ProjectFileManager");
            e.Handled = true;
        }

        private void menuAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(@"      Senparc.ProjectFileManager can help developers to manage .csproj files under the certain path.
      You can use this tool to modify project file information or manage version information individually or in bulk.", "About Senparc.ProjectFileManager");
        }

        #region Save

        private void menuSaveOne_Click(object sender, RoutedEventArgs e)
        {
            txtPath.Focus();
            if (SelectedFile==null)
            {
                MessageBox.Show("Please choose one project!");
            }

            SelectedFile.Save();

            MessageBox.Show($"File saved:\r\n{SelectedFile.FullFilePath}");
        }

        private void menuSaveAll_Click(object sender, RoutedEventArgs e)
        {
            txtPath.Focus();

            int i = 0;
            foreach (var projectFile in ProjectFiles)
            {
                try
                {
                    projectFile.Save();
                    i++;
                }
                catch 
                {

                }
            }
            MessageBox.Show($"All files saved: {i}/{ProjectFiles.Count}");
        }

        #endregion


    }
}
