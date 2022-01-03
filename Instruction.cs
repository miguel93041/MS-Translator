using System;

namespace MS_translator
{
  public class Instruction
  {
    public string Name { get; }
    public int Operator { get; }
    public int FirstOperand { get; }
    public int SecondOperand { get; }

    public Instruction(string name, int op, int firstOperand, int secondOperand)
    {
      Console.WriteLine($"Creando instrucción: {op} {firstOperand} {secondOperand}");
      Name = name;
      Operator = op;
      FirstOperand = firstOperand;
      SecondOperand = secondOperand;
    }

    public Instruction(string name, int op, int secondOperand)
    {
      Console.WriteLine($"Creando instrucción: {op} 0 {secondOperand}");
      Name = name;
      Operator = op;
      FirstOperand = 0;
      SecondOperand = secondOperand;
    }

    public string ToHex()
    {
      string sBinaryOperator = Common.DecimalToBinary(Operator, Program.Config.InstructionSize);
      string sFirstOperand = Common.DecimalToBinary(FirstOperand, Program.Config.FirstOperandSize);
      string sSecondOperand = Common.DecimalToBinary(SecondOperand, Program.Config.SecondOperandSize);

      return Common.BinaryToHex(string.Concat(sBinaryOperator, sFirstOperand, sSecondOperand));
    }
  }
}
