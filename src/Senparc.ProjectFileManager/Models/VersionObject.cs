using Senparc.CO2NET.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Senparc.ProjectFileManager.Models
{
    /// <summary>
    /// Version Object
    /// </summary>
    public class VersionObject
    {
        public int MajorVersion { get; set; }
        public int MinorVersion { get; set; }
        public int RevisionVersion { get; set; }
        public int BuildNumberVersion { get; set; }
        public string QualifierVersion { get; set; }

        public VersionObject() { }

        public VersionObject(int majorVersion, int minorVersion, int revisionVersion, int buildNumberVersion, string qualifierVersion)
        {
            MajorVersion = majorVersion;
            MinorVersion = minorVersion;
            RevisionVersion = revisionVersion;
            BuildNumberVersion = buildNumberVersion;
            QualifierVersion = qualifierVersion;
        }

        /// <summary>
        /// Build version string from this object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string versionString = MajorVersion + "." + MinorVersion;//at least keep 2 version segments.
            if (RevisionVersion > 0 || BuildNumberVersion > 0)
            {
                versionString += "." + RevisionVersion;
                if (BuildNumberVersion > 0)
                {
                    versionString += "." + BuildNumberVersion;
                }
            }
            if (!QualifierVersion.IsNullOrEmpty())
            {
                versionString += QualifierVersion;
            }

            return versionString;
        }

    }
}
