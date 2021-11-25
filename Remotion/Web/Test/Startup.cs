using System;
using System.Web;
using System.Web.Integration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Remotion.Web.Test
{
  public class Startup
  {
    public void ConfigureServices (IServiceCollection services)
    {
      services.AddRouting();
    }

    public void Configure (IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      //app.UseHsts();
      //app.UseHttpsRedirection();

      app.UseRouting();

      app.UseLegacyAspNet(LegacyAspNetOptions.Default);
    }
  }
}