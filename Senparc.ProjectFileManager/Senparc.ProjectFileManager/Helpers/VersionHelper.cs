using Senparc.CO2NET.Extensions;
using Senparc.ProjectFileManager.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Senparc.ProjectFileManager.Helpers
{
    /// <summary>
    /// Version Helper
    /// </summary>
    public static class VersionHelper
    {
        public static VersionObject GetVersionObject(string versionString)
        {
            //var systemVersion = Version.Parse(versionString);
            var versionArr = versionString.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            var versionObject = new VersionObject();

            for (int i = 0; i < versionArr.Length; i++)
            {
                var currentVersion = versionArr[i];
                int versionNumber;
                if (!int.TryParse(currentVersion, out versionNumber))
                {
                    //may contains QualifierVersion

                    var regex = Regex.Match(currentVersion, @"(\d+)([_\-\+]{0,1})([\S\w]*)");
                    if (regex.Success)
                    {
                        if (!int.TryParse(regex.Groups[1].Value, out versionNumber))
                        {
                            throw new Exception($"invalid version format: {currentVersion} / {regex.Groups[1].Value}");
                        }

                        if (!regex.Groups[2].Value.IsNullOrEmpty() /*like: "-" */ && !regex.Groups[3].Value.IsNullOrEmpty() /* like: "preview3" */)
                        {
                            versionObject.QualifierVersion = regex.Groups[2].Value + regex.Groups[3].Value;//like: "-preview3"
                        }
                    }
                    else
                    {
                        throw new Exception("invalid version format: " + currentVersion);
                    }
                }

                switch (i)
                {
                    case 0:
                        versionObject.MajorVersion = versionNumber;
                        break;
                    case 1:
                        versionObject.MinorVersion = versionNumber;
                        break;
                    case 2:
                        versionObject.RevisionVersion = versionNumber;
                        break;
                    case 3:
                        versionObject.BuildNumberVersion = versionNumber;
                        break;
                    default:
                        break;
                }
            }


            //ver version = new VersionObject(versionMatch)

            return versionObject;
        }
    }
}
