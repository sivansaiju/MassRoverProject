using System;
using System.Threading.Tasks;
using RestSharp;
using MarsRoverPhotosAPI.Interfaces;
using MarsRoverPhotosAPI.Models;
using System.Text;

namespace MarsRoverPhotosAPI.Implementation
{

  public class MarsRoverImageDtls : IMarsRoverImageDtls
  {
    public async Task<Root> GetMarsImages(string roverName, string marsApi, string apiKey, DateTime date)
    {
      StringBuilder strUrl = new StringBuilder(marsApi);
      strUrl.Append(roverName);
      strUrl.AppendFormat("/photos?earth_date={0}{1}{2}", string.Format("{0:yyyy-MM-dd}", date), "&api_key=", apiKey);
      var client = new RestClient(strUrl.ToString());
      var request = new RestRequest(Method.GET);
      var jsonResponse = await client.ExecuteAsync<Root>(request);
      var result = jsonResponse.Data;
      return result;
      
    }

  }
}

  
