using System;
using System.Web;
using System.Web.Integration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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

      //app.UsePathBase ("/fx-develop/Web.Test"); // didn't use
      //app.UseHsts();
      //app.UseHttpsRedirection();

      app.UseRouting();

      app.UseLegacyAspNet(LegacyAspNetOptions.Default);
    }
  }
}
