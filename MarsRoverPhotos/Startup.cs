using MarsRoverPhotosAPI.Implementation;
using MarsRoverPhotosAPI.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MarsRoverPhotos
{
  public class Startup
  {
    public IConfiguration Configuration { get; }
    public ILoggerFactory LoggerFactory { get; }
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }



    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllers();
      services.AddCors();
      services.AddScoped<IMarsRoverImageDtls, MarsRoverImageDtls>();
      services.AddSingleton(Configuration);
    
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
      loggerFactory.AddLog();
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

      //app.UseHttpsRedirection();

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
