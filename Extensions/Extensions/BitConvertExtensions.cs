using System;

namespace Extensions
{
    #region Enum

    public enum EByteOrder
    {
        Intel,
        Motorola
    }

    public enum EValueType
    {
        Signed,
        Unsigned
    }

    #endregion

    public static class BitConvertExtensions
    {
        #region Private Field

        private const int BIAS = 127;
        private const int IEEE754_LENGTH = 32;
        private const int EXPONENT_LENGTH = 8;
        private const int FRACTION_LENGTH = 23;

        #endregion

        #region Public Methods

        /// <summary>
        /// 전달하는 인자가 2의 승수인지 판별하여 반환합니다.
        /// </summary>
        public static bool CheckNumberPowerOfTwo(int num)
        {
            if(num < 0)
            {
                return false;
            }

            if (((num - 1) & num) == 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 현재 바이트 배열에서 시작비트와 길이, 바이트 오더를 인자로 inputNum을 삽입하고 바뀐 바이트 배열을 반환합니다.
        /// </summary>  
        public static byte[] SetIntToByteArr(this byte[] origNumArr, int startBit, int length, int inputNum, EByteOrder byteOrder)
        {
            byte[] changedByteArr = new byte[origNumArr.Length];

            for (int i = 0; i < changedByteArr.Length; i++)
            {
                changedByteArr[i] = origNumArr[i];
            }

            int inputCount = 0;
            int currentIndex = startBit / 8;
            int bitCount = startBit;

            while(inputCount < length)
            {
                if((currentIndex != bitCount / 8) && byteOrder == EByteOrder.Motorola)
                {
                    bitCount -= 16;
                }

                currentIndex = bitCount / 8;

                int targetPoint = 1 << inputCount++;
                int tempBitResult = inputNum & targetPoint;

                if (tempBitResult == targetPoint)
                {
                    changedByteArr[currentIndex] |= (byte)(1 << bitCount - (currentIndex * 8));
                }
                else
                {
                    changedByteArr[currentIndex] &= (byte)~(1 << bitCount - (currentIndex * 8));
                }

                bitCount++;
            }

            return changedByteArr;
        }

        /// <summary>
        /// 현재 32비트 정수 자료형에 담긴 비트를 바꾸고 데이터를 반환합니다.
        /// </summary>
        public static int SetIntToInt(this int num, int startBit, int length, int inputNum, EByteOrder byteOrder)
        {
            // Early Exit
            ValidateOverflowForInt(startBit, length);

            int changedNum = num;

            int inputCount = 0;
            int bitCount = startBit;

            while (inputCount < length)
            {
                if (inputCount != 0 && bitCount % 8 == 0 && byteOrder == EByteOrder.Motorola)
                {
                    bitCount -= 16;
                }

                int targetPoint = 1 << inputCount++;
                int resultNum = inputNum & targetPoint;

                if (resultNum == targetPoint)
                {
                    // put 1
                    changedNum |= 1 << bitCount;
                }
                else
                {
                    // put 0
                    changedNum &= ~(1 << bitCount);
                }

                bitCount++;
            }

            return changedNum;
        }

        /// <summary>
        /// 현재 바이트 배열에서 시작비트와 길이 만큼 비트를 가져와 32비트 int 자료형으로 반환합니다.
        /// valueType이 Signed일 경우, MSB를 기준으로 양수 또는 음수를 반환 할 수 있습니다.
        /// </summary>
        public static int GetIntFromByteArr(this byte[] origNumArr, int startBit, int length, EValueType valueType, EByteOrder byteOrder)
        {
            int readCount = 0;
            int currentIndex = startBit / 8;
            int bitCount = startBit;

            int resultNum = 0;

            while (readCount < length)
            {
                if ((currentIndex != bitCount / 8) && byteOrder == EByteOrder.Motorola)
                {
                    bitCount -= 16;
                }

                currentIndex = bitCount / 8;

                int targetPoint = 1 << bitCount % 8;
                int tempBitResult = origNumArr[currentIndex] & targetPoint;

                if (tempBitResult == targetPoint)
                {
                    resultNum |= 1 << readCount;
                }
                else
                {
                    resultNum &= ~(1 << readCount);
                }

                bitCount++;
                readCount++;
            }

            if (valueType == EValueType.Signed && length > 1)
            {
                // MSB 체크
                int pointMSB = 1 << length - 1;
                int resultMSB = resultNum & pointMSB;

                if (pointMSB == resultMSB)
                {
                    //int negative = -1;
                    //var a = -1 & ~resultNum;

                    //resultNum = negative.SetIntToInt(0, length, resultNum, byteOrder);
                    int negative = -1 << length;
                    resultNum |= negative;
                }
            }

            return resultNum;

        }

        /// <summary>
        /// 현재 바이트 배열에서 시작 비트를 기준으로 32비트를 가져와 IEEE754 표준의 float 값을 반환합니다.
        /// </summary>
        public static float GetFloatFromByteArr(this byte[] origNumArr, int startBit, EByteOrder byteOrder)
        {
            int exponentCount = origNumArr.GetIntFromByteArr(startBit + FRACTION_LENGTH, EXPONENT_LENGTH, EValueType.Unsigned, byteOrder) - BIAS;

            int integerPart = origNumArr.GetIntFromByteArr(startBit + FRACTION_LENGTH - exponentCount, exponentCount, EValueType.Unsigned, byteOrder);

            integerPart |= (1 << exponentCount);

            int fractionNum = origNumArr.GetIntFromByteArr(startBit, FRACTION_LENGTH - exponentCount, EValueType.Unsigned, byteOrder);

            float fractionPart = 0f;
            float tempNumber = 1f;

            for (int i = startBit + FRACTION_LENGTH - exponentCount; i >= startBit; i--)
            {
                tempNumber /= 2f;

                int targetPoint = 1 << i - 1;
                int tempBitResult = fractionNum & targetPoint;

                if (tempBitResult == targetPoint)
                {
                    fractionPart += tempNumber;
                }
            }

            float finalResult = integerPart + fractionPart;

            // MSB Check
            if(origNumArr.GetIntFromByteArr(startBit + IEEE754_LENGTH - 1, 1, EValueType.Signed, EByteOrder.Intel) == 1)
            {
                finalResult *= -1;
            }

            return finalResult;
        }

        public static float GetFloatFromByteArrDemo(this byte[] origNumArr, int startBit, EByteOrder byteOrder)
        {
            uint convertedNum = (uint) origNumArr.GetIntFromByteArr(startBit, IEEE754_LENGTH, EValueType.Unsigned, byteOrder);

            //uint convertedNum = (uint) convertedNum;

            int currentBitIndex = IEEE754_LENGTH - 1;

            bool bNegative = false;
            
            if((convertedNum & 0x80000000) == 0x80000000)
            {
                bNegative = true;
            }

            uint exponentCount = ((convertedNum & 0x7F800000) >> FRACTION_LENGTH) - BIAS;

            uint integerPart = convertedNum >> (int)(FRACTION_LENGTH - exponentCount);
            integerPart <<= (int)(IEEE754_LENGTH - exponentCount);
            integerPart >>= (int)(IEEE754_LENGTH - exponentCount);

            integerPart |= (uint)(1 << (int)exponentCount);

            return 0;


        }
        #endregion

        #region Private Helpers

        private static void ValidateOverflowForInt(int startBit, int length)
        {
            if(startBit > 32 || length > 32 || startBit + length > 32)
            {
                throw new StackOverflowException($"Stack Over Flow Exception startBit : {startBit} / length : {length}");
            }
        }

        #endregion

        #region legacy

        #region int to byte[]

            //int inputByteLength = (length / 8) + 1;

            //byte[] inputNumArr = new byte[inputByteLength];

            //for (int i = 0; i < inputNumArr.Length; i++)
            //{
            //    inputNumArr[i] = (byte)((inputNum >> ((inputNumArr.Length - i - 1) * 8)) & 0xFF);
            //}

            #endregion

        private static byte[] GetBitChangedByteArr(this byte[] origNumArr, int startBit, int length, int inputNum)
        {
            byte[] changedByteArr = new byte[origNumArr.Length];

            for (int i = 0; i < changedByteArr.Length; i++)
            {
                changedByteArr[i] = origNumArr[i];
            }

            int totalBits = startBit + length;

            int inputBitCount = 0;

            for (int i = startBit; i < totalBits; i++)
            {
                int targetPoint = 1 << inputBitCount++;
                int tempResultNum = inputNum & targetPoint;
                int currentIndex = i / 8;

                if (tempResultNum == targetPoint)
                {
                    changedByteArr[currentIndex] = (byte)(changedByteArr[currentIndex] | (1 << i - (currentIndex * 8)));
                }
                else
                {
                    changedByteArr[currentIndex] = (byte)(changedByteArr[currentIndex] & ~(1 << i - (currentIndex * 8)));
                }
            }

            return changedByteArr;
        }
      
        private static int GetBitChangedInteger(this int num, int startBit, int length, int inputNum)
        {
            // Early Exit
            ValidateOverflowForInt(startBit, length);

            int changedNum = num;

            for (int i = 0; i < length; i++)
            {
                int targetPoint = 1 << i;
                int resultNum = inputNum & targetPoint;

                if (resultNum == targetPoint)
                {
                    // put 1
                    changedNum |= 1 << (startBit + i);
                }
                else
                {
                    // put 0
                    changedNum &= ~(1 << (startBit + i));
                }
            }

            return changedNum;
        }

        private static int GetIntegerFromByteArr(this byte[] origNumArr, int startBit, int length, EValueType valueType)
        {
            int totalBits = startBit + length;

            int inputIndex = 0;
            int resultNum = 0;

            for (int i = startBit; i < totalBits; i++)
            {
                int currentIndex = i / 8;
                int targetPoint = 1 << (i - (currentIndex * 8));
                int tempResultPoint = origNumArr[currentIndex] & targetPoint;

                if (tempResultPoint == targetPoint)
                {
                    resultNum |= (1 << inputIndex);
                }
                else
                {
                    resultNum &= ~(1 << inputIndex);
                }

                inputIndex++;
            }

            if (valueType == EValueType.Signed && length > 1)
            {
                // MSB 체크
                int pointMSB = 1 << length - 1;
                int resultMSB = resultNum & pointMSB;

                if (pointMSB == resultMSB)
                {
                    int negative = -1;

                    resultNum = negative.GetBitChangedInteger(0, length, resultNum);
                }
            }

            return resultNum;

        }

        #endregion

    }
}