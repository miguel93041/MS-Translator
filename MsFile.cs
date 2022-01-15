using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace MS_translator
{
  public class MsFile
  {
    public static List<Data> Datas = new();
    public static List<Instruction> Instructions = new();
    private Serializer Serializer { get; }

    public MsFile(string filePath)
    {
      Console.WriteLine("Deserializando archivo ...");
      List<string> fileLines = GetLines(filePath);

      int codeStartPos = fileLines.FindIndex(line => line.Contains(".code"));
      int codeEndPos = fileLines.FindIndex(codeStartPos, line => line.Contains(".end"));

      if (codeStartPos == -1 || codeEndPos == -1)
      {
        throw new Exception("El archivo .asm esta mal formado");
      }

      ExtractAllData(fileLines, codeStartPos);
      ExtractCode(fileLines, codeStartPos, codeEndPos);

      Serializer = new Serializer(filePath);
    }

    private void ExtractCode(List<string> fileLines, int codeStartPos, int codeEndPos)
    {
      Console.WriteLine("Extrayendo .code ...");

      string[] codeLines = fileLines.GetRange(codeStartPos + 1, codeEndPos - codeStartPos - 1).ToArray();
      foreach (string line in codeLines)
      {
        Instruction instruction = ExtractInstruction(codeLines, line);
        Instructions.Add(instruction);
      }
    }

    private Instruction ExtractInstruction(string[] codeLines, string line)
    {
      Console.WriteLine($"Extrayendo instruccion: {line}");
      string name = ExtractName(line);
      string formattedLine = FormatLine(line);
      string[] sInstructionLine = formattedLine.Split(' ', ',').Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
      string sInstruction = sInstructionLine[0];
      string sFirstOperand = sInstructionLine[1];

      JObject jInstruction = (JObject)Program.Config.Instructions.FirstOrDefault(jToken => ((JObject)jToken).GetValue("name")?.ToString() == sInstruction);
      if (jInstruction == null)
      {
        throw new Exception($"La instrucción {sInstruction} no se encuentra definida en la configuración del programa");
      }
      int op = (int)jInstruction.GetValue("value");

      int firstOperand = sFirstOperand.Any(char.IsLetter)
        ? GetMemoryPos(codeLines, sFirstOperand)
        : int.Parse(sFirstOperand);

      Instruction instruction;
      if (sInstructionLine.Length > 2) //Instrucción de 2 operandos
      {
        string sSecondOperand = sInstructionLine[2];
        int secondOperand = (sSecondOperand).Any(char.IsLetter)
          ? GetMemoryPos(codeLines, sSecondOperand)
          : int.Parse(sSecondOperand);
        instruction = new Instruction(name, op, firstOperand, secondOperand);
      }
      else //Instrucción inmediata
      {
        bool fillFirstOperand = (bool)jInstruction.GetValue("immediate");
        if (!fillFirstOperand)
        {
          instruction = new Instruction(name, op, firstOperand);
        }
        else
        {
          string binaryDecimal = Convert.ToString(op, 2);
          string opBits = binaryDecimal[..Program.Config.InstructionSize];
          string firstOperandBits = binaryDecimal[Program.Config.InstructionSize..];
          for (int i = firstOperandBits.Length; i < Program.Config.FirstOperandSize; i++)
          {
            firstOperandBits += "0";
          }
          int secondOperand = firstOperand;
          op = Convert.ToInt32(opBits, 2);
          firstOperand = Convert.ToInt32(firstOperandBits, 2);
          instruction = new Instruction(name, op, firstOperand, secondOperand);
        }
      }

      return instruction;
    }

    private void ExtractAllData(List<string> fileLines, int codeStartPos)
    {
      Console.WriteLine("Extrayendo .data ...");

      List<int> dataPositions = new();
      int dataPosition = fileLines.FindIndex(line => line.Contains(".data"));
      int lastIndex = dataPosition + 1;
      while (dataPosition != -1)
      {
        dataPositions.Add(dataPosition);

        dataPosition = fileLines.FindIndex(lastIndex, line => line.Contains(".data"));
        lastIndex = dataPosition + 1;
      }

      for (int i = 0; i < dataPositions.Count; i++)
      {
        int position = dataPositions[i];
        int startingPosition = int.Parse(new string(FormatLine(fileLines[position]).Replace(".data", "").Where(c => !char.IsWhiteSpace(c)).ToArray()));
        bool isLast = (i + 1) == dataPositions.Count;
        int count = !isLast ? dataPositions[i + 1] - position - 1 : codeStartPos - position - 1;
        string[] memoryLines = fileLines.GetRange(position + 1, count).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();

        Data data = new(startingPosition);

        foreach (string line in memoryLines)
        {
          Memory<object> memory = ExtractMemory(line);
          data.Memories.Add(memory);
        }

        Datas.Add(data);
      }
    }

    private Memory<object> ExtractMemory(string line)
    {
      Console.WriteLine($"Extrayendo memoria: {line}");
      string name = ExtractName(line);
      string formattedLine = FormatLine(line);
      int firstSpacePosition = formattedLine.IndexOf(' ');
      string sType = formattedLine.Substring(1, firstSpacePosition - 1);
      string sData = formattedLine[(firstSpacePosition + 1)..];

      ETypes type = (ETypes)Enum.Parse(typeof(ETypes), sType, true);

      Memory<object> memory;
      if (sData.Contains("\"") || sData.Contains("\'")) //Es de tipo string
      {
        const string pattern = "(\"|\')(.|\\.|\\|\\{\\[)(\"|\'),?\\s?";
        if (sData.Length > 3)
        {
          MatchCollection regexCoincidences = Regex.Matches(sData, pattern);
          if (regexCoincidences.Count > 0)
          {
            char[] chars = regexCoincidences.Select(r =>
            {
              string value = r.Value;
              if (value[^1] == ',')
              {
                value = value[..^1];
              }

              value = value.Substring(1, 1);
              return char.Parse(value);
            }).ToArray();
            memory = new Memory<object>(name, type, chars);
          }
          else
          {
            memory = new Memory<object>(name, type, sData[1..^1].ToCharArray());
          }
        }
        else
        {
          memory = new Memory<object>(name, type, Convert.ToChar(sData[1..^1]));
        }
      }
      else //Data es un int
      {
        memory = new Memory<object>(name, type, sData.Contains(',') ?
          Array.ConvertAll(string.Concat(sData.Where(c => !char.IsWhiteSpace(c))).Split(',').ToArray(), int.Parse) :
          int.Parse(sData));
      }

      return memory;
    }

    private int GetMemoryPos(string[] codeLines, string name)
    {

      int codePosition = codeLines.ToList().FindIndex(l => ExtractName(l) == name);
      if (codePosition != -1)
      {
        return codePosition;
      }

      int desiredPosition = -1;
      foreach (Data data in Datas)
      {
        int memoryIndex = data.Memories.FindIndex(memory => memory.Name == name);
        if (memoryIndex == -1)
        {
          continue;
        }

        int increment = 0;
        for (int i = 0; i < memoryIndex; i++)
        {
          Memory<object> memory = data.Memories[i];
          increment += memory.IsVector ? ((Array)memory.Data).Length : 1;
        }

        desiredPosition = increment + data.StartingPosition;
        break;
      }

      if (desiredPosition == -1)
      {
        throw new Exception($"{name} no esta definido ni en .data ni en .code, no se puede encontrar la memoria asociada");
      }

      return desiredPosition;
    }

    private string ExtractName(string line)
    {
      int namedLine = line.IndexOf(':');

      if (namedLine == -1)
      {
        return "";
      }

      line = line[..namedLine];

      return line;
    }

    private string FormatLine(string line)
    {
      string formattedLine = RemoveName(line);
      formattedLine = RemoveComment(formattedLine);
      formattedLine = RemoveJumps(formattedLine);

      return formattedLine.Trim();
    }

    private string RemoveName(string line)
    {
      int namedLine = line.IndexOf(':');
      if (namedLine != -1)
      {
        line = line[(namedLine + 1)..];
      }

      return line;
    }

    private string RemoveComment(string line)
    {
      int commentedLine = line.IndexOf(';');
      if (commentedLine != -1)
      {
        line = line[..commentedLine];
      }

      return line;
    }

    private string RemoveJumps(string line) =>
        line.Replace("\r", "", true, CultureInfo.InvariantCulture)
            .Replace("\t", "", true, CultureInfo.InvariantCulture)
            .Replace("\n", "", true, CultureInfo.InvariantCulture)
            .Replace("\\", "", true, CultureInfo.InvariantCulture);

    private List<string> GetLines(string filePath) => File.ReadAllLines(filePath).ToList();

    public string Serialize() => Serializer.Serialize();
  }
}
