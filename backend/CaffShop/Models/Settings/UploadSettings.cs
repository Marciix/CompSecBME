using System;
using System.IO;
using CaffShop.Helpers;

namespace CaffShop.Models.Settings
{
    public class UploadSettings
    {
        public const long UploadSizeLimit = 20971520; // 20 Mib
        public const string PreviewExtension = ".jpg";

        public string UploadBaseDir { get; private set; }
        public string TempDirPath { get; private set; }
        public string PrevDirPath { get; private set; }
        public string CaffDirPath { get; private set; }


        public UploadSettings()
        {
            FillFromEnvironment();
        }

        private void FillFromEnvironment()
        {
            var uploadBasePath = HelperFunctions.GetEnvironmentValueOrException("UP_BASE_DIR_PATH");
            var uploadTempPath = HelperFunctions.GetEnvironmentValue("UP_TMP_DIR_PATH");
            var previewFolderName = HelperFunctions.GetEnvironmentValue("PREVIEW_DIR_NAME");
            var caffFolderName = HelperFunctions.GetEnvironmentValue("CAFF_DIR_NAME");

            UploadBaseDir = uploadBasePath;
            TempDirPath = uploadTempPath.Length != 0 ? uploadTempPath : Path.Combine(uploadBasePath, "Tmp");
            PrevDirPath = Path.Combine(uploadBasePath, previewFolderName.Length != 0 ? previewFolderName : "Previews");
            CaffDirPath = Path.Combine(uploadBasePath, caffFolderName.Length != 0 ? caffFolderName : "Caffs");
        }
    }
}