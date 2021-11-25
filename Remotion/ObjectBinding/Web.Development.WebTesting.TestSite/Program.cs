using System;
using System.Diagnostics;
using System.Web.Integration;
using System.Web.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite
{
  public class Program
  {
    public static void Main (string[] args)
    {
      LegacyAspNetInitialization.Init("/", Environment.CurrentDirectory, true);

      CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder (string[] args)
    {
      return Host.CreateDefaultBuilder(args)
          .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
  }
}