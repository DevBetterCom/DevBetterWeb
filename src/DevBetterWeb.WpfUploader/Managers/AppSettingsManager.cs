using System;
using System.Configuration;
using System.IO;

namespace DevBetterWeb.WpfUploader.Managers;
public class AppSettingsManager
{
  private readonly Configuration _configFile;

  public AppSettingsManager()
  {
    _configFile = GetConfigFile();

  }

  public void AddUpdateAppSettings(string key, string value)
  {
    try
    {
      var settings = GetSettings();
      if (settings[key] == null)
      {
        settings.Add(key, value);
      }
      else
      {
        settings[key].Value = value;
      }

      Save();
    }
    catch (ConfigurationErrorsException)
    {
      Console.WriteLine("Error writing app settings");
    }
  }

  public string GetAppSetting(string name)
  {
    var settings = GetSettings();
    if (settings[name] == null)
    {
      AddUpdateAppSettings(name, string.Empty);
      return string.Empty;
    }

    return settings[name].Value;
  }

  private KeyValueConfigurationCollection GetSettings()
  {
    var settings = _configFile.AppSettings.Settings;

    return settings;
  }

  private Configuration GetConfigFile()
  {
    ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
    fileMap.ExeConfigFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App.config");
    Configuration configFile = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

    return configFile;
  }

  private void Save()
  {
    _configFile.Save(ConfigurationSaveMode.Modified);
    ConfigurationManager.RefreshSection(_configFile.AppSettings.SectionInformation.Name);
  }

}
