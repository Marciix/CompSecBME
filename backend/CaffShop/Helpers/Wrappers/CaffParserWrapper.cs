using System;
using System.Runtime.InteropServices;
using CaffShop.Interfaces;
using CaffShop.Models.Exceptions;

namespace CaffShop.Helpers.Wrappers
{
    public class CaffParserWrapper : ICaffParserWrapper
    {
        [DllImport(@"Resources\CaffParser", EntryPoint = "ParseAndValidateCaff")]
        [return: MarshalAs(UnmanagedType.I4)]
        private static extern int ParseAndValidateCaff(
            [MarshalAs(UnmanagedType.LPStr)] string tempFilePath,
            [MarshalAs(UnmanagedType.LPStr)] string validCaffPath,
            [MarshalAs(UnmanagedType.LPStr)] string prevFilePath
        );

        public void ValidateAndParseCaff(string tempFilePath, string caffFilePath, string prevFilePath)
        {
            int returnCode = -1;
            try
            {
                returnCode = ParseAndValidateCaff(tempFilePath, caffFilePath, prevFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception");
                Console.WriteLine(ex);
            }

            if (0 != returnCode)
                throw new InvalidCaffFileException("Failed to parse caff file");
        }
    }
}