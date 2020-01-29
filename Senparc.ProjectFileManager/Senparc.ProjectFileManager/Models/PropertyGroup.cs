using Senparc.CO2NET.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;

namespace Senparc.ProjectFileManager.Models
{
    /// <summary>
    /// .csproj file items in the first &lt;PropertyGroup&gt; tag
    /// </summary>
    public class PropertyGroup : INotifyPropertyChanged
    {
        public string TargetFramework { get; set; }
        public string TargetFrameworks { get; set; }
        public string Version { get; set; }
        public string AssemblyName { get; set; }
        public string RootNamespace { get; set; }
        public string Description { get; set; }
        public string Copyright { get; set; }
        public string PackageTags { get; set; }
        public string Authors { get; set; }
        public string Owners { get; set; }
        public string PackageLicenseUrl { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string ProjectUrl { get; set; }
        public string PackageProjectUrl { get; set; }
        public string PackageIconUrl { get; set; }
        public string PackageReleaseNotes { get; set; }
        public string RepositoryUrl { get; set; }

        #region Additional Information
        public XElement OriginalElement { get; set; }

        public string FileName { get; set; }
        public string FullFilePath { get; set; }

        #endregion

        #region INotifyPropertyChanged functions

        private void UpdateProperty<T>(ref T properValue, T newValue, [CallerMemberName] string propertyName = "")
        {
            if (object.Equals(properValue, newValue))
            {
                return;
            }
            properValue = newValue;

            OnPropertyChanged(propertyName);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        /// <summary>
        /// Create a PropertyGroup entity from XML.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="fullFilePath"></param>
        /// <returns></returns>
        public static PropertyGroup GetObjet(XElement element, string fullFilePath)
        {
            var projectFile = Senparc.CO2NET.Utilities.XmlUtility.Deserialize<PropertyGroup>(element.ToString()) as PropertyGroup;
            if (projectFile == null)
            {
                throw new Exception("invalid xml file for new .csproj file format!");
            }
            projectFile.OriginalElement = element;
            projectFile.FullFilePath = fullFilePath;
            projectFile.FileName = Path.GetFileName(fullFilePath);
            return projectFile;
        }

        public void Save()
        {
            var doc = XDocument.Load(FullFilePath);
            var propertyGroup = doc.Root.Elements("PropertyGroup").First();
            if (!TargetFramework.IsNullOrEmpty())
            {

            }
        }

        private void FillXml(Expression<Func<object>> obj, XElement element)
        {
           
        }
    }
}
