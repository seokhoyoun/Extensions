using System;

namespace Extensions
{
    public static class BitConvertExtensions
    {
        private const int BIAS = 127;
        private const int IEEE754_LENGTH = 32;
        private const int EXPONENT_LENGTH = 8;
        private const int FRACTION_LENGTH = 23;

        /// <summary>
        /// 현재 바이트 배열에서 시작비트와 길이, 바이트 오더를 인자로 inputNum을 삽입하여 바뀐 바이트 배열을 반환합니다.
        /// </summary>  
        public static byte[] GetBitChangedByteArrByEndian(this byte[] origNumArr, int startBit, int length, int inputNum, bool bBigEndian)
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
                if((currentIndex != bitCount / 8) && bBigEndian)
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

            #region int to byte[]

            //int inputByteLength = (length / 8) + 1;

            //byte[] inputNumArr = new byte[inputByteLength];

            //for (int i = 0; i < inputNumArr.Length; i++)
            //{
            //    inputNumArr[i] = (byte)((inputNum >> ((inputNumArr.Length - i - 1) * 8)) & 0xFF);
            //}

            #endregion

        /// <summary>
        /// 현재 바이트 배열에서 시작비트와 길이를 인자로 inputNum을 삽입하여 바뀐 바이트 배열을 반환합니다.
        /// </summary>  
        public static byte[] GetBitChangedByteArr(this byte[] origNumArr, int startBit, int length, int inputNum)
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

        public static int GetBitChangedIntegerByEndian(this int num, int startBit, int length, int inputNum, bool bBigEndian)
        {
            // Early Exit
            ValidateOverflowForInt(startBit, length);

            int changedNum = num;

            int inputCount = 0;
            int bitCount = startBit;

            while(inputCount < length)
            {
                if(inputCount != 0 && bitCount % 8 == 0 && bBigEndian)
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
        /// 현재 int형 값에서 시작비트와 길이를 인자로 inputNum을 삽입하여 바뀐 int 형 값을 반환합니다.
        /// </summary>
        public static int GetBitChangedInteger(this int num, int startBit, int length, int inputNum)
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

        public static int GetIntegerFromByteArrDemo(this byte[] origNumArr, int startBit, int length, bool bSigned, bool bBigEndian)
        {
            int readCount = 0;
            int currentIndex = startBit / 8;
            int bitCount = startBit;

            int resultNum = 0;

            while (readCount < length)
            {
                if ((currentIndex != bitCount / 8) && bBigEndian)
                {
                    bitCount -= 16;
                }

                currentIndex = bitCount / 8;

                int targetPoint = 1 << (bitCount - (currentIndex * 8));
                int tempBitResult = (origNumArr[currentIndex] << bitCount - (currentIndex * 8)) & targetPoint;

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


            //int totalBits = startBit + length;

            //int inputIndex = 0;
            //int resultNum = 0;

            //for (int i = startBit; i < totalBits; i++)
            //{
            //    int currentIndex = i / 8;
            //    int targetPoint = 1 << (i - (currentIndex * 8));
            //    int tempResultPoint = origNumArr[currentIndex] & targetPoint;

            //    if (tempResultPoint == targetPoint)
            //    {
            //        resultNum |= (1 << inputIndex);
            //    }
            //    else
            //    {
            //        resultNum &= ~(1 << inputIndex);
            //    }

            //    inputIndex++;
            //}

            if (bSigned && length > 1)
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

        /// <summary>
        /// 현재 바이트 배열에서 시작비트와 길이 만큼 비트를 가져와 32비트 int 자료형으로 반환합니다.
        /// bSigned가 true일 경우, MSB를 기준으로 양수 또는 음수를 반환 할 수 있습니다.
        /// </summary>
        public static int GetIntegerFromByteArr(this byte[] origNumArr, int startBit, int length, bool bSigned)
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

            if (bSigned && length > 1)
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

        /// <summary>
        /// 현재 바이트 배열에서 시작 비트를 기준으로 32비트를 가져와 IEEE754 표준의 float 값을 반환합니다.
        /// </summary>
        public static float GetFloatFromByteArr(this byte[] origNumArr, int startBit)
        {


            int exponent = origNumArr.GetIntegerFromByteArr(startBit + FRACTION_LENGTH, EXPONENT_LENGTH, false) - BIAS;

            int integerPart = origNumArr.GetIntegerFromByteArr(startBit + FRACTION_LENGTH - exponent, exponent, false);

            integerPart |= (1 << exponent);

            float fractionPart = 0;
            float tempNumber = 1;

            for (int i = startBit + FRACTION_LENGTH - exponent; i <= startBit; i--)
            {

            }

            return 0;
        }

        private static void ValidateOverflowForInt(int startBit, int length)
        {
            if(startBit > 32 || length > 32 || startBit + length > 32)
            {
                throw new StackOverflowException($"Stack Over Flow Exception startBit : {startBit} / length : {length}");
            }
        }

    }
}