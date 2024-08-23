// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using CoreForms.Web.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Remotion.Data.DomainObjects.Web.Test
{
  public class Program
  {
    public static void Main (string[] args)
    {
      LegacyAspNetInitialization.License = "I hereby confirm that I use CoreForms only for trial purposes and have read and accept the CoreForms Trial License.";

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
