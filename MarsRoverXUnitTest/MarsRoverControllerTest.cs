using System;
using Xunit;
using MarsRoverPhotosAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MarsRoverPhotos.Controllers;
using Microsoft.Extensions.Configuration;
using Moq;
namespace MarsRoverXUnitTest
{

  public class MarsRoverControllerTest
  {
    ControllerContext controllerContext;
    ILogger<MarsRoverPhotosController> logger;
    MarsRoverPhotosController objmarsRover;
    IConfiguration iConfig;
    private const string apiUrl = "https://api.nasa.gov/mars-photos/api/v1/rovers";
    private const string apiKey = "DEMO_KEY";
    private const string roverName = "Curiosity";
    private DateTime imageDate = new DateTime(2015, 06, 03);
    public MarsRoverControllerTest()
    {
      controllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() };
      var logerFactory = (ILoggerFactory)new LoggerFactory();
      logger = logerFactory.CreateLogger<MarsRoverPhotosController>();
      iConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    }
    [Fact]
    public async void GetMarsPhotos_Test()
    {
      var moqMarsRoverImageDtls = new Mock<IMarsRoverImageDtls>();
      moqMarsRoverImageDtls.Setup(x => x.GetMarsImages(roverName, apiUrl, apiKey, imageDate).Result);
      objmarsRover = new MarsRoverPhotosController(logger, moqMarsRoverImageDtls.Object, iConfig) { ControllerContext = controllerContext };
      var response = await objmarsRover.GetMarsPhotos();
      Assert.NotNull(response);
      Assert.True( ((OkObjectResult)response).StatusCode == 200);
    }
  }
}
