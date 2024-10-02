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
using System.Drawing.Imaging;
using System.IO;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.BrowserSession;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;

namespace Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure
{
  /// <summary>
  /// Provides means of annotating or cropping a <see cref="Screenshot"/>, and saving the annotation independently.
  /// </summary>
  public class DiagnosticScreenshotBuilder : ScreenshotBuilder
  {
    [NotNull]
    public static DiagnosticScreenshotBuilder CreateDesktopScreenshot ([NotNull] IBrowserContentLocator contentLocator, [NotNull] ILoggerFactory loggerFactory)
    {
      ArgumentUtility.CheckNotNull("contentLocator", contentLocator);
      ArgumentUtility.CheckNotNull("loggerFactory", loggerFactory);

      return new DiagnosticScreenshotBuilder(Screenshot.TakeDesktopScreenshot(), contentLocator, loggerFactory);
    }

    [NotNull]
    public static DiagnosticScreenshotBuilder CreateBrowserScreenshot (
        [NotNull] IBrowserContentLocator contentLocator,
        [NotNull] IBrowserSession browserSession,
        [NotNull] ILoggerFactory loggerFactory)
    {
      ArgumentUtility.CheckNotNull("contentLocator", contentLocator);
      ArgumentUtility.CheckNotNull("browserSession", browserSession);
      ArgumentUtility.CheckNotNull("loggerFactory", loggerFactory);

      return new DiagnosticScreenshotBuilder(
          Screenshot.TakeBrowserScreenshot(browserSession, contentLocator),
          contentLocator,
          loggerFactory);
    }

    private DiagnosticScreenshotBuilder ([NotNull] Screenshot screenshot, [NotNull] IBrowserContentLocator locator, ILoggerFactory loggerFactory)
        : base(screenshot, locator, loggerFactory)
    {
    }

    public void SaveAnnotation ([NotNull] string path, bool overwriteFileIfExists = false)
    {
      ArgumentUtility.CheckNotNullOrEmpty("path", path);

      if (!overwriteFileIfExists && File.Exists(path))
        throw new InvalidOperationException(string.Format("A screenshot with the file name '{0}' does already exist.", path));

      var directory = Path.GetDirectoryName(path);
      Directory.CreateDirectory(directory);

      using (var annotationImage = AnnotationLayer.CloneImage())
      {
        annotationImage.Save(path, ImageFormat.Png);
      }
    }
  }
}
