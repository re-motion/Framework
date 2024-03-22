// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it
// and/or modify it under the terms of the GNU Lesser General Public License
// as published by the Free Software Foundation; either version 2.1 of the
// License, or (at your option) any later version.
//
// re-motion is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
//
using System;
using System.Net;
using System.Threading;

namespace Remotion.Web.Development.WebTesting.WebDriver.Configuration
{
  internal static class FileDownloadUtility
  {
    private static readonly TimeSpan s_downloadRetrySleepTime = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan s_downloadMaxTime = TimeSpan.FromMinutes(1);

    public static void DownloadFileWithRetry (string downloadUrl, string outputPath)
    {
      RetryWebClientActionWithTimeout(webClient =>
      {
        webClient.DownloadFile(downloadUrl, outputPath);
        return string.Empty; // dummy value since we have to return a value
      });
    }

    public static string DownloadStringWithRetry (string downloadUrl)
    {
      return RetryWebClientActionWithTimeout(webClient => webClient.DownloadString(downloadUrl));
    }

    private static T RetryWebClientActionWithTimeout<T> (Func<WebClient, T> action)
    {
#pragma warning disable SYSLIB0014
      using (var webClient = new WebClient()) // TODO RM-8492: Replace with HttpClient
#pragma warning restore SYSLIB0014
      {
        var startTime = DateTime.Now;
        while (true)
        {
          try
          {
            return action(webClient);
          }
          catch (WebException)
          {
            var elapsedTime = DateTime.Now - startTime;
            if (elapsedTime > s_downloadMaxTime)
              throw;
          }

          Thread.Sleep(s_downloadRetrySleepTime);
        }
      }
    }
  }
}
