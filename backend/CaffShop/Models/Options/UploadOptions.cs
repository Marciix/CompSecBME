using System.ComponentModel.DataAnnotations;
using System.IO;

namespace CaffShop.Models.Options
{
    public class UploadOptions
    {
        public static readonly string OptionsName = "Upload";

        public const long UploadSizeLimit = 20971520; // 20 Mib
        public const string PreviewExtension = ".jpg";

        private string _tempDirPath = "";
        private string _prevDirPath = "";
        private string _caffDirPath = "";

        public string UploadBaseDir { get; set; }

        public string TempDirPath
        {
            get => _tempDirPath.Length != 0 ? _tempDirPath : Path.Combine(UploadBaseDir, "Tmp");
            set => _tempDirPath = value;
        }

        public string PrevDirPath
        {
            get => Path.Combine(UploadBaseDir, _prevDirPath.Length != 0 ? _prevDirPath : "Previews");
            set => _prevDirPath = value;
        }

        public string CaffDirPath
        {
            get => Path.Combine(UploadBaseDir, _caffDirPath.Length != 0 ? _caffDirPath : "Caffs");
            set => _caffDirPath = value;
        }
    }
}