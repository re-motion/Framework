using System;
using System.Web.Integration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Remotion.Web.Test
{
  public class Program
  {
    public static void Main (string[] args)
    {
      Console.WriteLine(Environment.CurrentDirectory);
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