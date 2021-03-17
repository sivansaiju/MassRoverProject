using System;
using MarsRoverPhotosAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MarsRoverPhotosAPI.Interfaces
{
  public interface IMarsRoverImageDtls
  {
    Task<Root> GetMarsImages( string roverName, string marsApi, string apiKey, DateTime date );
    DateTime? ParseDate(string date);
    List<DateTime> GetDates(string datefile);
  }
}
