using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications.Runner;
using Microsoft.SmartDevice.Connectivity;
using ICSharpCode.SharpZipLib.Zip;
using System.Xml.Linq;

namespace Machine.Specifications.ConsoleRunner
{
    class Wp7DeviceRunner
    {
        static readonly ObjectId WP7_PLATFORM_ID = new ObjectId("7E6B29E6-AAE1-41dc-9849-049507CBA2B0");
        private readonly Device _device;

        public Wp7DeviceRunner(bool RunOnEmulator)
        {
            var dsm = new DatastoreManager(1033);
            var platform = dsm.GetPlatform(WP7_PLATFORM_ID);

            _device = platform.GetDevices().Where(d => RunOnEmulator == d.IsEmulator()).FirstOrDefault();
        }

        public void RunXap(string xapPath)
        {
            if (_device == null)
                throw new ApplicationException("No WP7 device found to deploy to");

            _device.Connect();
            _device.Activate();

            ManifestInfo manifestInfo = GetXapManifestInfo(xapPath);

            if (_device.IsApplicationInstalled(manifestInfo.ProductId))
                _device.GetApplication(manifestInfo.ProductId).Uninstall();

            RemoteApplication app = _device.InstallApplication(manifestInfo.ProductId, Guid.NewGuid(), manifestInfo.Genre, manifestInfo.IconPath, xapPath);
            app.Launch();
        }

        private static ManifestInfo GetXapManifestInfo(string xapPath)
        {
                var xap = new ZipFile(xapPath);
                int manifestId = xap.FindEntry("WMAppManifest.xml", true);

                if (manifestId < 0)
                    throw new ApplicationException(string.Format("WMAppManifest.xml not found in \"{0}\"", xapPath));

                XDocument manifest;
                using (var s = xap.GetInputStream(manifestId))
                    manifest = XDocument.Load(s);

                xap.Close();

                var appNode = manifest.Descendants("App").First();
                return new ManifestInfo()
                {
                    ProductId = Guid.Parse((string)appNode.Attribute("ProductID")),
                    Genre = (string)appNode.Attribute("Genre"),
                    IconPath = appNode.Descendants("IconPath").First().Value,
                };
        }
    }

    class ManifestInfo
    {
        public Guid ProductId { get; set; }
        public string Genre { get; set; }
        public string IconPath { get; set; }
    }
}
