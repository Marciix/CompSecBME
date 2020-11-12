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
            [MarshalAs(UnmanagedType.LPStr)] string prevFilePath,
            [MarshalAs(UnmanagedType.LPStr)] string jsonFilePath
        );

        public void ValidateAndParseCaff(string tempFilePath, string prevFilePath, string jsonFilePath)
        {
            var returnCode = -1;
            try
            {
                returnCode = ParseAndValidateCaff(tempFilePath, prevFilePath, jsonFilePath);
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