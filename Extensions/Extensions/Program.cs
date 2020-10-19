﻿using System;
using System.Diagnostics;
using System.Text;

namespace Extensions
{
    class Program
    {
        static void Main(string[] args)
        {
            string result = "I'm happy";
            byte[] utfstr = new byte[] { 0xE4, 0xA4, 0xA7, 0xE6, 0xB4, 0xA0, 0xE6, 0xA1, 0xA1, 0xE7, 0x81, 0xB0, 0x79 };
            string utrstrr = Encoding.UTF8.GetString(utfstr);
            byte[] resArr = Encoding.UTF8.GetBytes(result);
            

            string sdsd = "-35.75";
            byte[] res = new byte[sdsd.Length];

            for (int i = 0; i < sdsd.Length; i++)
            {
                res[i] = (byte)sdsd[i];
            }


            byte c = 0b0100_1111;
            c =(byte) (c << 4);
            c = (byte)(c >> 4);



            byte[] testByteArr = new byte[] { 0xBB, 0xAA, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF};

            byte[] changedByteArr = testByteArr.SetIntToByteArr(8, 16, -444, EByteOrder.Motorola);
            Debug.Assert( changedByteArr.GetIntFromByteArr(8, 16, EValueType.Signed, EByteOrder.Motorola) == -444);

            changedByteArr = changedByteArr.SetIntToByteArr(23, 16, -444, EByteOrder.Motorola);

            Debug.Assert(changedByteArr.GetIntFromByteArr(23, 16, EValueType.Signed, EByteOrder.Motorola) == -444);

            //nt testNum = 0x1FFFFFFF;

            //testNum.GetBitChangedIntegerByEndian(56, 12, 0, true);
            byte[] floatByte = new byte[] { 0xc2, 0xed, 0x40, 0x00 };
            //floatByte = new byte[] { 0x00, 0x40, 0xED, 0xC2 };

            float a = -118.625f;
            float b = floatByte.GetFloatFromByteArrDemo(24, EByteOrder.Motorola);



        }
    }
}

