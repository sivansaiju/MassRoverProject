using System;
using Xunit;
using MarsRoverPhotosAPI.Models;
using MarsRoverPhotosAPI.Implementation;
using Shouldly;
namespace MarsRoverXUnitTest
{
  public class MarsRoverTests
  {
    private const string apiUrl = "https://api.nasa.gov/mars-photos/api/v1/rovers";
    private const string apiKey = "DEMO_KEY";
    private const string roverName = "Curiosity";
    private DateTime pastDate = new DateTime(1776, 01, 01);
    private DateTime futureDate = new DateTime(2999, 01, 01);
    private DateTime imageDate = new DateTime(2015, 06, 03);
    private DateTime invalidDate = new DateTime(2015, 05, 31);
    [Fact]
    public async void pastDateresult_Shouldbe_NULL()
    {
      MarsRoverImageDtls objMarsRoverImageDtls = new MarsRoverImageDtls();
      Root result = await objMarsRoverImageDtls.GetMarsImages(roverName, apiUrl, apiKey, pastDate);
      result.photos.ShouldBeNull();
    }
    [Fact]
    public async void futureDateresult_Shouldbe_NULL()
    {
      MarsRoverImageDtls objMarsRoverImageDtls = new MarsRoverImageDtls();
      Root result = await objMarsRoverImageDtls.GetMarsImages(roverName, apiUrl, apiKey, futureDate);
      result.photos.ShouldBeNull();
    }
    [Fact]
    public async void badRoverName_Shouldbe_NULL()
    {
      MarsRoverImageDtls objMarsRoverImageDtls = new MarsRoverImageDtls();
      Root result = await objMarsRoverImageDtls.GetMarsImages("badRoverName", apiUrl, apiKey, imageDate);
      result.photos.ShouldBeNull();
    }
    [Fact]
    public void invalidDate_Shouldbe_NULL()
    {
      MarsRoverImageDtls objMarsRoverImageDtls = new MarsRoverImageDtls();
      var result = objMarsRoverImageDtls.ParseDate(invalidDate.ToString());
      result.ShouldBeNull();
    }
  }
}
