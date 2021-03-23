using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace MarsRoverPhotosAPI.Implementation
{
  public static class Extensions
  {
    public static void AddLog( this ILoggerFactory loggerFactory)
    {
      var path = Directory.GetCurrentDirectory();
      string date = DateTime.UtcNow.Ticks.ToString();
      loggerFactory.AddFile($"{path}\\Logs\\{date}Log.txt");

    }
  }
}
