﻿namespace CaffShop.Interfaces
{
    public interface ICaffParserWrapper
    {
        public void ValidateAndParseCaff(string tempFilePath, string prevFilePath);
    }
}