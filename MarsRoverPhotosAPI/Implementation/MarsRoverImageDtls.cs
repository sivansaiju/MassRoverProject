using System;
using System.Threading.Tasks;
using RestSharp;
using MarsRoverPhotosAPI.Interfaces;
using MarsRoverPhotosAPI.Models;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.IO;

namespace MarsRoverPhotosAPI.Implementation
{

  public class MarsRoverImageDtls : IMarsRoverImageDtls
  {
    public async Task<Root> GetMarsImages(string roverName, string marsApi, string apiKey, DateTime date)
    {
      StringBuilder strUrl = new StringBuilder(marsApi);
      strUrl.Append(roverName);
      strUrl.AppendFormat("/photos?earth_date={0}{1}{2}", String.Format("{0:yyyy-MM-dd}", date), "&api_key=", apiKey);
      var client = new RestClient(strUrl.ToString());
      var request = new RestRequest(Method.GET);
      var jsonResponse = await client.ExecuteAsync<Root>(request);
      var result = jsonResponse.Data;
      return result;

    }
    public DateTime? ParseDate(string date)
    {
      DateTime dateValue;
      CultureInfo enUS = new CultureInfo("en-US");

      var formatStrings = new string[] { "MM/dd/yy", "MMMM d, yyyy", "MMM-d-yyyy" };

      if (DateTime.TryParseExact(date, formatStrings, enUS, DateTimeStyles.None, out dateValue))
        return dateValue;
      else
        return null;
    }

    public List<DateTime> GetDates(string datefile)
    {
      List<DateTime> dateInputList = new List<DateTime>();
      FileStream dateList = new FileStream(datefile, FileMode.Open);
      using (StreamReader reader = new StreamReader(dateList))
      {
        while (reader.Peek() >= 0)
        {
          string line = reader.ReadLine();

          DateTime? date = ParseDate(line);
          if (date != null)
          {
            dateInputList.Add((DateTime)date);
          }
        }
      }
      return dateInputList;
    }
  }
}

  
