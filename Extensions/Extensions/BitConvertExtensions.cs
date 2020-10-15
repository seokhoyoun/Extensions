namespace Extensions
{
    public static class BitConvertExtensions
    {
        /// <summary>
        /// 현재 바이트 배열에서 시작비트와 길이를 인자로 inputNum을 삽입하여 바뀐 바이트 배열을 반환합니다.
        /// </summary>  
        public static byte[] GetBitChangedByteArr(this byte[] origNumArr, int startBit, int length, int inputNum)
        {            

            #region int to byte[]

            //int inputByteLength = (length / 8) + 1;

            //byte[] inputNumArr = new byte[inputByteLength];

            //for (int i = 0; i < inputNumArr.Length; i++)
            //{
            //    inputNumArr[i] = (byte)((inputNum >> ((inputNumArr.Length - i - 1) * 8)) & 0xFF);
            //}

            #endregion

            byte[] changedByteArr = new byte[origNumArr.Length];

            for (int i = 0; i < changedByteArr.Length; i++)
            {
                changedByteArr[i] = origNumArr[i];
            }

            int totalBits = startBit + length;

            int inputCount = 0;

            for (int i = startBit; i < totalBits; i++)
            {
                int targetPoint = 1 << inputCount++;
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

        /// <summary>
        /// 현재 int형 값에서 시작비트와 길이를 인자로 inputNum을 삽입하여 바뀐 int 형 값을 반환합니다.
        /// </summary>
        public static int GetBitChangedInteger(this int num, int startBit, int length, int inputNum)
        {
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
                int tempResultNum = origNumArr[currentIndex] & targetPoint;

                if (tempResultNum == targetPoint)
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
            const int bias = 127;
            const int length = 32;
            const int exponentLength = 8;
            const int fractionLength = 23;

            int exponent = origNumArr.GetIntegerFromByteArr(startBit + fractionLength, exponentLength, false) - bias;

            return 0;
        }
    }
}