## BitConvertExtension.cs
* SetIntToByteArr
* SetIntToInt
* GetIntFromByteArr
* GetFloatFromByteArr

### public static byte[] SetIntToByteArr(this byte[] origNumArr, int startBit, int length, int inputNum, EByteOrder byteOrder);
``` 
현재 바이트 배열에서 시작비트와 길이, 바이트 오더를 인자로 inputNum을 삽입하고 바뀐 바이트 배열을 반환합니다.
```
        
### public static int SetIntToInt(this int num, int startBit, int length, int inputNum, EByteOrder byteOrder);
``` 
현재 32비트 정수 자료형에 담긴 비트를 바꾸고 데이터를 반환합니다. 
```

### public static int GetIntFromByteArr(this byte[] origNumArr, int startBit, int length, EValueType valueType, EByteOrder byteOrder);
``` 
현재 바이트 배열에서 시작비트와 길이 만큼 비트를 가져와 32비트 int 자료형으로 반환합니다.
valueType이 Signed일 경우, MSB를 기준으로 양수 또는 음수를 반환 할 수 있습니다. 
```

### public static float GetFloatFromByteArr(this byte[] origNumArr, int startBit, EByteOrder byteOrder)
```
현재 바이트 배열에서 시작 비트를 기준으로 32비트를 가져와 IEEE754 표준의 float 값을 반환합니다.
```
