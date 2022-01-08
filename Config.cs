using System;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MS_translator
{
  public class Config
  {
    [JsonIgnore]
    public static string ConfigPath {
      get {
        FileInfo fileInfo = new(Process.GetCurrentProcess().MainModule!.FileName!);
        string configName = $"{fileInfo.Name.Split('.')[0]}.config.json";
        return Path.Combine(AppContext.BaseDirectory, configName);
      }
    }

    [JsonProperty("version")]
    public static string Version => Process.GetCurrentProcess().MainModule!.FileVersionInfo.FileVersion;

    [JsonProperty("instructionSize")]
    public int InstructionSize { get; set; } = 2;

    [JsonProperty("firstOperandSize")]
    public int FirstOperandSize { get; set; } = 7;

    [JsonProperty("secondOperandSize")]
    public int SecondOperandSize { get; set; } = 7;

    [JsonProperty("instructions")]
    public JArray Instructions { get; set; } = new()
    {
      new JObject()
      {
        new JProperty("name", "add"),
        new JProperty("value", 0),
        new JProperty("immediate", false),
      },
      new JObject()
      {
        new JProperty("name", "cmp"),
        new JProperty("value", 1),
        new JProperty("immediate", false),
      },
      new JObject()
      {
        new JProperty("name", "mov"),
        new JProperty("value", 2),
        new JProperty("immediate", false),
      },
      new JObject()
      {
        new JProperty("name", "beq"),
        new JProperty("value", 3),
        new JProperty("immediate", false),
      }
    };

    public string Serialize() => JsonConvert.SerializeObject(this, Formatting.Indented);

    public static Config Deserialize(string configPath)
    {
      Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configPath));
      return config;
    }
  }
}
