using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevBetterWeb.UploaderApp
{
  class Program
  {
    static async Task Main(string[] args)
    {
      var argsList = args.ToList();
      if (argsList.Count == 0 || argsList.All( x => x.ToLower() != "-d") || argsList.All(x => x.ToLower() != "-t") || argsList.All(x => x.ToLower() != "-a"))
      {
        Console.WriteLine("Please use -d [destination folder] -t [Vimeo token] -a [api link]");
        return;
      }

      var folderToUpload = GetArgument(argsList, "-d");
      if (string.IsNullOrEmpty(folderToUpload))
      {
        Console.WriteLine("Please use -d [destination folder]");
        return;
      }

      var token = GetArgument(argsList, "-t");
      if (string.IsNullOrEmpty(token))
      {
        Console.WriteLine("Please use -t [Vimeo token]");
        return;
      }

      var apiLink = GetArgument(argsList, "-a");
      if (string.IsNullOrEmpty(apiLink))
      {
        Console.WriteLine("Please use -a [api link]");
        return;
      }

      var uploaderManager = new UploaderManager(token, apiLink);
      await uploaderManager.SyncAsync(folderToUpload);

      Console.WriteLine("Done, press any key to close");
      Console.ReadKey();
    }       
    
    public static string GetArgument(List<string> argsList, string argValue)
    {
      var index  = argsList.FindIndex(x => x.ToLower() == argValue) + 1;
      if (index <= 0)
      {
        return null;
      }

      return argsList[index];
    }
  }
}
