using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace Senparc.ProjectFileManager.Models
{
    /// <summary>
    /// .csproj file items in the first &lt;PropertyGroup&gt; tag
    /// </summary>
    public class PropertyGroup
    {
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
        public string PackageProjectUrl { get; set; }
        public string PackageIconUrl { get; set; }
        public string PackageReleaseNotes { get; set; }
        public string RepositoryUrl { get; set; }

        #region 补充信息
        public XElement OriginalElement { get; set; }

        public string FileName => Path.GetFileName(FullFilePath);
        public string FullFilePath { get; set; }
        #endregion

        public static PropertyGroup GetObjet(XElement element,string fullFilePath)
        {
            var projectFile = Senparc.CO2NET.Utilities.XmlUtility.Deserialize<PropertyGroup>(element.ToString()) as PropertyGroup;
            if (projectFile == null)
            {
                throw new Exception("invalid xml file for new .csproj file format!");
            }
            projectFile.OriginalElement = element;
            projectFile.FullFilePath = fullFilePath;
            return projectFile;
        }
    }
}
