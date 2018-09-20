using System;

namespace CheckPrices
{
    static class Tests
    {
        [STAThread]
        static void Main()
        {
            //TestLiftPartsStandard();
            TestLiftPartsDetailed();
        }

        private static void TestLiftPartsStandard()
        {
            string theCode;
            decimal thePrice;
            var url = "https://www.liftpartswarehouse.com/SearchResults.asp?Search=ac4920195&Submit=Submit";

            Engine.ParseLiftParts(url, out theCode, out thePrice);
        }
        private static void TestLiftPartsDetailed()
        {
            string theCode;
            decimal thePrice;
            var url = "https://www.liftpartswarehouse.com/product-p/BJ073507.htm";

            Engine.ParseLiftParts(url, out theCode, out thePrice);
        }

    }
}
