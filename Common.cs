using System;

namespace MS_translator
{
  public static class Common
  {
    public static string DecimalToBinary(int @decimal, int size)
    {
      char[] buff = new char[size];

      for (int i = size - 1; i >= 0; i--)
      {
        int mask = 1 << i;
        buff[size - 1 - i] = (@decimal & mask) != 0 ? '1' : '0';
      }

      return new string(buff);
    }

    public static string BinaryToHex(string binary) => Convert.ToInt64(binary, 2).ToString("x");
  }
}
