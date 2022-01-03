using System;
using System.IO;
using System.Text;

namespace MS_translator
{
  public class Program
  {
    public static Config Config;

    private static void Main(string[] args)
    {
      Console.WriteLine("################################################################################################################");
      Console.WriteLine("#                                                                                                              #");
      Console.WriteLine("#  ███╗   ███╗███████╗   ████████╗██████╗  █████╗ ███╗   ██╗███████╗██╗      █████╗ ████████╗ ██████╗ ██████╗  #");
      Console.WriteLine("#  ████╗ ████║██╔════╝   ╚══██╔══╝██╔══██╗██╔══██╗████╗  ██║██╔════╝██║     ██╔══██╗╚══██╔══╝██╔═══██╗██╔══██╗ #");
      Console.WriteLine("#  ██╔████╔██║███████╗█████╗██║   ██████╔╝███████║██╔██╗ ██║███████╗██║     ███████║   ██║   ██║   ██║██████╔╝ #");
      Console.WriteLine("#  ██║╚██╔╝██║╚════██║╚════╝██║   ██╔══██╗██╔══██║██║╚██╗██║╚════██║██║     ██╔══██║   ██║   ██║   ██║██╔══██╗ #");
      Console.WriteLine("#  ██║ ╚═╝ ██║███████║      ██║   ██║  ██║██║  ██║██║ ╚████║███████║███████╗██║  ██║   ██║   ╚██████╔╝██║  ██║ #");
      Console.WriteLine("#  ╚═╝     ╚═╝╚══════╝      ╚═╝   ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚═══╝╚══════╝╚══════╝╚═╝  ╚═╝   ╚═╝    ╚═════╝ ╚═╝  ╚═╝ #");
      Console.WriteLine("#                                                                                            v1.0.0 03/01/2022 #");
      Console.WriteLine("#                                                                                  Autor: Miguel Granel Ferrer #");
      Console.WriteLine("#                       Con licencia Atribución-NoComercial-CompartirIgual 4.0 Internacional (CC BY-NC-SA 4.0) #");
      Console.WriteLine("#                                                                 https://github.com/miguel93041/MS-Translator #");
      Console.WriteLine("################################################################################################################");

      if (args.Length != 1)
      {
        Console.WriteLine("Arrastra el archivo .asm al .exe");
        Console.WriteLine("Presiona alguna tecla para terminar el programa.");
        Console.ReadLine();
        Environment.Exit(1);
      }

      string path = args[0];
      Console.WriteLine($"Encontrado el archivo {path}");

      try
      {
        if (File.Exists(Config.ConfigPath))
        {
          Config = Config.Deserialize(Config.ConfigPath);
        }
        else
        {
          Config = new Config();
          using StreamWriter configFile = new(Config.ConfigPath, false, Encoding.UTF8) { AutoFlush = true };
          configFile.Write(Config.Serialize());
        }

        MsFile file = new(path);
        Console.WriteLine("Serializando .txt");
        string outputPath = file.Serialize();

        Console.WriteLine($"La creación del archivo .txt ha finalizado, se encuentra en {outputPath}");
        Console.WriteLine("Presiona alguna tecla para terminar el programa.");
        Console.ReadLine();
      } 
      catch (Exception e)
      {
        Console.WriteLine($"Se ha encontrado la siguiente excepción mientras se intentaba convertir el archivo: {e}");
        Console.WriteLine("Presiona alguna tecla para terminar el programa.");
        Console.ReadLine();
        Environment.Exit(1);
      }
    }
  }
}
