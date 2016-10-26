// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.Base32String
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Text;

namespace System.Deployment.Application
{
  internal class Base32String
  {
    protected static char[] charList = new char[32]
    {
      '0',
      '1',
      '2',
      '3',
      '4',
      '5',
      '6',
      '7',
      '8',
      '9',
      'A',
      'B',
      'C',
      'D',
      'E',
      'G',
      'H',
      'J',
      'K',
      'L',
      'M',
      'N',
      'O',
      'P',
      'Q',
      'R',
      'T',
      'V',
      'W',
      'X',
      'Y',
      'Z'
    };

    public static string FromBytes(byte[] bytes)
    {
      int length = bytes.Length;
      if (length <= 0)
        return (string) null;
      int num1 = length << 3;
      int num2 = num1 / 5 << 3;
      if (num1 % 5 != 0)
        num2 += 8;
      StringBuilder stringBuilder = new StringBuilder(num2 >> 3);
      int num3 = 0;
      int num4 = 0;
      for (int index = 0; index < length; ++index)
      {
        num4 = num4 << 8 | (int) bytes[index];
        num3 += 8;
        while (num3 >= 5)
        {
          num3 -= 5;
          stringBuilder.Append(Base32String.charList[num4 >> num3 & 31]);
        }
      }
      if (num3 > 0)
        stringBuilder.Append(Base32String.charList[num4 << 5 - num3 & 31]);
      return stringBuilder.ToString();
    }
  }
}
