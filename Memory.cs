namespace MS_translator
{
  public class Memory<T>
  {
    public bool IsVector => Data.GetType().IsArray;
    public string Name { get; }
    public ETypes Type { get; }
    public T Data { get; }

    public Memory(string name, ETypes type, T data)
    {
      Name = name;
      Type = type;
      Data = data;
    }
  }

  public enum ETypes
  {
    Ascii16,
    Dw,
    Rw
  }
}
