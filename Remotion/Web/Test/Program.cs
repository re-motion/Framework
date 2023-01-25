using System;
using CoreForms.Web.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Remotion.Web.Test
{
  public class Program
  {
    public static void Main (string[] args)
    {
      Console.WriteLine(Environment.CurrentDirectory);
      var legacyAspNetInitializationOptions = new LegacyAspNetInitializationOptions("/", Environment.CurrentDirectory);
      LegacyAspNetInitialization.Initialize(legacyAspNetInitializationOptions);

      CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder (string[] args)
    {
      return Host.CreateDefaultBuilder(args)
          .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
  }
}
