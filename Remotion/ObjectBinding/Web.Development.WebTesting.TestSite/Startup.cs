using System;
using System.Web;
using System.Web.Integration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite
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

      app.UseRouting();

      app.UseLegacyAspNet(LegacyAspNetOptions.Default);
    }
  }
}
