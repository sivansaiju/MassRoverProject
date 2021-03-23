using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarsRoverPhotosAPI.Models;
using MarsRoverPhotosAPI.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Globalization;

namespace MarsRoverPhotos.Controllers
{
  [ApiController]
  [Route("api/MarsRoverPhotos")]
  public class MarsRoverPhotosController : ControllerBase
  {

    private readonly ILogger<MarsRoverPhotosController> _logger;
    private readonly IMarsRoverImageDtls _roverImageDtls;
    private readonly IConfiguration _iConfig;
    public MarsRoverPhotosController(ILogger<MarsRoverPhotosController> logger,IMarsRoverImageDtls roverImageDtls, IConfiguration iConfig)
    {
      _logger = logger;
      _roverImageDtls = roverImageDtls;
      _iConfig = iConfig;
    }

    [HttpGet]
    [Route("GetMarsPhotos")]
    [ProducesResponseType( typeof(IEnumerable<Photo>), (int)HttpStatusCode.OK )]
    [ProducesResponseType( (int)HttpStatusCode.BadRequest )]
    [ProducesResponseType( (int)HttpStatusCode.NotFound )]
    public async Task<IActionResult> GetMarsPhotos()
    {
      var marsApi = _iConfig.GetSection("AppSettings:MarsRoverBaseApiUrl").Get<string>();
      var apiKey = _iConfig.GetSection("AppSettings:MarsRoverApiKey").Get<string>();
      var dateFile = _iConfig.GetSection("AppSettings:DatesFile").Get<string>();
      var imageLoc = _iConfig.GetSection("AppSettings:ImageFolder").Get<string>();
      var roverNames = _iConfig.GetSection("AppSettings:RoverName").Get<string>();

      List<string> lstRoverNames = roverNames.Split(',').ToList();
      List<DateTime> lstDate = GetDates(dateFile);
      List<imageDtls> imageDtls = new List<imageDtls>();
      Root marsPhotos;
      var imageFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imageLoc);
      if (!Directory.Exists(imageFolder))
      {
        Directory.CreateDirectory(imageFolder);
      }


      foreach (DateTime date in lstDate)
      {
        if (date != null)
        {

          foreach (string roverName in lstRoverNames)
          {
            string message = string.Format("Retrieving photos on date {0} for the Rover : {1}", date.ToString(), roverName);
            _logger.LogInformation(message);
            marsPhotos = await _roverImageDtls.GetMarsImages(roverName, marsApi, apiKey, date);

            if ( marsPhotos!=null && marsPhotos.photos != null )
            {
              foreach (Photo varPh in marsPhotos.photos)
              {
                imageDtls.Add(new imageDtls { earth_date = varPh.earth_date, img_src = varPh.img_src });
                if (!string.IsNullOrEmpty(varPh.img_src))
                {
                  using (WebClient client = new WebClient())
                  {
                    StringBuilder strImage = new StringBuilder(imageFolder);
                    strImage.AppendFormat("{0}{1}{2}{3}{4}{5}{6}", "LandingDate_", varPh.rover.landing_date, "-launchDate_", varPh.rover.launch_date, "-", DateTime.UtcNow.Ticks.ToString(), ".jpg");
                    client.DownloadFile(varPh.img_src, strImage.ToString());
                  }
                }
              }
            }
          }


        }
      }
      return Ok(imageDtls);
    }
    public DateTime? ParseDate(string date)
    {
      DateTime dateValue;
      CultureInfo enUS = new CultureInfo("en-US");

      string[] formatStrings = {"dd/MM/yyyy", "dd/M/yyyy", "d/M/yyyy", "d/MM/yyyy","dd/MM/yy", "dd/M/yy", "d/M/yy", "d/MM/yy"};

      if (DateTime.TryParseExact(date, formatStrings, enUS, DateTimeStyles.None, out dateValue))
        return dateValue;
      else
      {
        string message = string.Format("Date :{0} is not a valid date ", date.ToString());
        _logger.LogInformation(message);
        return null;
      }
        
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
