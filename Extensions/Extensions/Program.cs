using System;

namespace Extensions
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] testByteArr = new byte[] { 0xBB, 0xAA, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF};

            //testByteArr.GetBitChangedByteArrByEndian(57, 16, 0x5AA5, true);

            //testByteArr.GetIntegerFromByteArr(56, 8, false);


            testByteArr.GetIntegerFromByteArrDemo(8, 16, false, false);

            //nt testNum = 0x1FFFFFFF;

            //testNum.GetBitChangedIntegerByEndian(56, 12, 0, true);



        }
    }
}

