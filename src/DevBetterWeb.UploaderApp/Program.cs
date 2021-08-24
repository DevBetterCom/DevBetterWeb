using System;
using System.Linq;
using System.Threading.Tasks;

namespace DevBetterWeb.UploaderApp
{
  class Program
  {
    static async Task Main(string[] args)
    {
      var argsList = args.ToList();
      if (argsList.Count == 0 || argsList.All( x => x.ToLower() != "-d") || argsList.All(x => x.ToLower() != "-t"))
      {
        Console.WriteLine("PLease use -d [destination folder] -t [Vimeo token]");
        return;
      }

      var folderToUploadIndex = argsList.FindIndex(x => x.ToLower() == "-d") + 1;
      if(folderToUploadIndex <= 0)
      {
        Console.WriteLine("PLease use -d [destination folder]");
        return;
      }
      var folderToUpload = argsList[folderToUploadIndex];

      var tokenIndex = argsList.FindIndex(x => x.ToLower() == "-t") + 1;
      if (tokenIndex <= 0)
      {
        Console.WriteLine("PLease use -t [Vimeo token]");
        return;
      }      
      var token = argsList[tokenIndex];

      var uploaderManager = new UploaderManager(token);
      await uploaderManager.SyncAsync(folderToUpload);

      Console.WriteLine("Done, press any key to close");
      Console.ReadKey();
    }        
  }
}
