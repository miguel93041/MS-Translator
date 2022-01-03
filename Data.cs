using System;
using System.Collections.Generic;

namespace MS_translator
{
  public class Data
  {
    public int StartingPosition { get; }
    public List<Memory<object>> Memories { get; } = new();

    public Data(int startingPosition)
    {
      Console.WriteLine($"Creando conjunto de .data {startingPosition}");
      StartingPosition = startingPosition;
    }
  }
}
