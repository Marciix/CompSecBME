using CaffShop.Interfaces;
using CaffShop.Models.Exceptions;

namespace CaffShop.Helpers.Wrappers
{
    public class CaffParserWrapperFailMock : ICaffParserWrapper
    {
        public void ValidateAndParseCaff(string tempFilePath, string prevFilePath, string jsonFilePath)
        {
            throw new InvalidCaffFileException("MOCK: Failed to parse caff file");
        }
    }
}