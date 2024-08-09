using System;
using CoreForms.Web.Infrastructure;
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

      services.AddLegacyAspNet();
    }

    public void Configure (IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseDefaultFiles();
      app.UseStaticFiles();

      app.UseRouting();

      app.UseEndpoints(
          endpoints =>
          {
            endpoints.MapLegacyAspNet("/{**rest}");
          });
    }
  }
}
