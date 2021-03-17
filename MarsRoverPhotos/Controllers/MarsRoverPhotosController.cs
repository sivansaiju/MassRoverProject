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
      var imageFolder = _iConfig.GetSection("AppSettings:ImageFolder").Get<string>();
      var roverNames = _iConfig.GetSection("AppSettings:RoverName").Get<string>();

      List<string> lstRoverNames = roverNames.Split(',').ToList();
      List<Photo> result = new List<Photo>();
      List<DateTime> lstDate = _roverImageDtls.GetDates(dateFile);
      Root marsPhotos;

      foreach (DateTime date in lstDate)
      {
        if (date != null)
        {
          foreach (string roverName in lstRoverNames)
          {
            marsPhotos = await _roverImageDtls.GetMarsImages(roverName, marsApi, apiKey, date);
            if (marsPhotos == null)
            {
              return NotFound();
            }
            if (marsPhotos.photos != null)
            {
              foreach (Photo varPh in marsPhotos.photos)
              {
                result.Add(varPh);
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
      return Ok(result);
    }

  }
}
