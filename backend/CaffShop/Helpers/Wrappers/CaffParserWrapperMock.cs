using CaffShop.Interfaces;

namespace CaffShop.Helpers.Wrappers
{
    public class CaffParserWrapperMock : ICaffParserWrapper
    {
        public bool ValidateAndParseCaff(string tempDir, string validCaffDir, string previewDir, string name)
        {
            return true;
        }
    }
}