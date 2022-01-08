using System;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Text;

namespace MS_translator
{
  public class Serializer
  {
    private FileInfo FileInfo { get; }
    private string OutputPath { get; }

    public Serializer(string filePath)
    {
      FileInfo = new FileInfo(filePath);
      string fileName = $"{FileInfo.Name.Split('.')[0]}.txt";
      OutputPath = Path.Combine(FileInfo.Directory!.FullName, fileName);
    }

    public string Serialize()
    {
      string content = CreateContent();
      File.WriteAllText(OutputPath, content);
      return OutputPath;
    }

    private string CreateContent()
    {
      string content = "";
      content += WriteHeader() + Environment.NewLine;

      int columnPos = 0;
      int currentMemoryPosition = 0;
      content = MsFile.Instructions.Select(instruction =>
        {
          Console.WriteLine($"Serializando instrucción {instruction.Name}: {instruction.Operator} {instruction.FirstOperand} {instruction.SecondOperand}");
          return instruction.ToHex();
        }).
        Aggregate(content, (current, hexInstruction) => current + WriteColumn(hexInstruction, ref currentMemoryPosition, ref columnPos));

      Data[] datas = MsFile.Datas.OrderBy(d => d.StartingPosition).ToArray();
      foreach (Data data in datas)
      {
        Console.WriteLine($"Serializando .data {data.StartingPosition}");
        if (currentMemoryPosition < data.StartingPosition)
        {
          string blankHexPositions = $"{data.StartingPosition - currentMemoryPosition}*0";
          content += WriteColumn(blankHexPositions, ref currentMemoryPosition, ref columnPos, data.StartingPosition);
        }

        foreach (Memory<object> memory in data.Memories)
        {
          Console.WriteLine($"Serializando {memory.Name}: .{memory.Type.ToString().ToLower()} {memory.Data}");
          if (memory.Type != ETypes.Rw)
          {
            if (!memory.IsVector)
            {
              string hex = GetMemoryDataHex(memory.Data);
              content += WriteColumn(hex, ref currentMemoryPosition, ref columnPos);
            }
            else
            {
              Array elements = (Array)memory.Data;
              content = (from object element in elements select GetMemoryDataHex(element))
                .Aggregate(content, (current, hex) => current + WriteColumn(hex, ref currentMemoryPosition, ref columnPos));
            }
          }
          else
          {
            for (int i = 0; i < (int)memory.Data; i++)
            {
              content += WriteColumn("0", ref currentMemoryPosition, ref columnPos);
            }
          }
        }
      }

      return content;
    }

    private string GetMemoryDataHex(object element)
    {
      string hex = "";
      switch (element)
      {
        case char:
          {
            byte[] asciiBytes = Encoding.ASCII.GetBytes(element.ToString()!);
            hex = asciiBytes.Aggregate("", (current, b) => current + b.ToString("x"));
            break;
          }
        case int dec:
          {
            string binaryDecimal = Convert.ToString(dec, 2);
            hex = Common.BinaryToHex(binaryDecimal);
            break;
          }
      }

      return hex;
    }

    private static string WriteColumn(string newContent, ref int currentMemoryPosition, ref int columnPos, int newMemoryPosition = -1)
    {
      columnPos++;

      if (columnPos > 7)
      {
        columnPos = 0;
        newContent += Environment.NewLine;
      }
      else
      {
        newContent += " ";
      }

      currentMemoryPosition = (newMemoryPosition != -1) ? newMemoryPosition : currentMemoryPosition + 1;
      return newContent;
    }

    private string WriteHeader() => "v2.0 raw";
  }
}
