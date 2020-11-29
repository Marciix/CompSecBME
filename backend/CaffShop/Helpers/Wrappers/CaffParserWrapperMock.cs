using System.IO;
using CaffShop.Interfaces;


namespace CaffShop.Helpers.Wrappers
{
    public class CaffParserWrapperMock : ICaffParserWrapper
    {
        public void ValidateAndParseCaff(string tempFilePath, string prevFilePath, string jsonFilePath)
        {
            var dir = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "TestFiles");

            // Delete uploaded file
            if (File.Exists(tempFilePath))
                File.Delete(tempFilePath);

            // Copy raw CAFF
            File.Copy(Path.Combine(dir, "test.caff"), tempFilePath);

            // Copy preview
            File.Copy(Path.Combine(dir, "test.ppm"), prevFilePath);

            // Copy json
            File.Copy(Path.Combine(dir, "test.json"), jsonFilePath);
        }
    }
}