namespace CaffShop.Interfaces
{
    public interface ICaffParserWrapper
    {
        public bool ValidateAndParseCaff(string tempDir, string validCaffDir, string previewDir, string name);
    }
}