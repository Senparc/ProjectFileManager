using Microsoft.VisualStudio.TestTools.UnitTesting;
using Senparc.ProjectFileManager.Helpers;

namespace Senparc.ProjectFileManager.Tests.Helpers
{
    [TestClass]
    public class VersionHelperTest
    {
        [TestMethod]
        public void GetVersionObjectTest()
        {
            var versionStr = "1.0-preview2";
            var versionObject = VersionHelper.GetVersionObject(versionStr);
            Assert.AreEqual(versionStr, versionObject.ToString());

            versionStr = "3.0.0.3";
            versionObject = VersionHelper.GetVersionObject(versionStr);
            Assert.AreEqual(versionStr, versionObject.ToString());

            versionStr = "3.0.0.0-preview3";
            versionObject = VersionHelper.GetVersionObject(versionStr);
            Assert.AreEqual("3.0-preview3", versionObject.ToString());//just keep 2 segments

            versionStr = "3.0.3.0_beta1";
            versionObject = VersionHelper.GetVersionObject(versionStr);
            Assert.AreEqual("3.0.3_beta1", versionObject.ToString());

        }
    }
}
