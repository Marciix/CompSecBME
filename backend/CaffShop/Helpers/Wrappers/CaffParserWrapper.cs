using System;
using System.Runtime.InteropServices;
using CaffShop.Interfaces;

namespace CaffShop.Helpers.Wrappers
{
    public class CaffParserWrapper : ICaffParserWrapper
    {
        [DllImport(@"Resources\CaffParser.dll", EntryPoint = "ParseAndValidateCaff")]
        [return: MarshalAs(UnmanagedType.I4)]
        private static extern int ParseAndValidateCaff(
            [MarshalAs(UnmanagedType.LPStr)] string tempDir,
            [MarshalAs(UnmanagedType.LPStr)] string validCaffDir,
            [MarshalAs(UnmanagedType.LPStr)] string previewDir,
            [MarshalAs(UnmanagedType.LPStr)] string name
        );

        public bool ValidateAndParseCaff(string tempDir, string validCaffDir, string previewDir, string name)
        {
            try
            {
                var returnCode = ParseAndValidateCaff(tempDir, validCaffDir, previewDir, name);
                Console.WriteLine(returnCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception");
                Console.WriteLine(ex);
                throw new ArgumentException("Failed to parse");
            }

            return true;
        }
    }
}