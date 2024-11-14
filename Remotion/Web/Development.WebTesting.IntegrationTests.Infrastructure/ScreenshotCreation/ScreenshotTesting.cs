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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;

namespace Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.ScreenshotCreation
{
  /// <summary>
  /// Provides helper methods for testing the screenshot infrastructure.
  /// </summary>
  public static class ScreenshotTesting
  {
    private class SubTestResult
    {
      public bool HasSucceeded { get; }

      public string Message { get; }

      public string ImageSource { get; }

      public string ResourceName { get; }

      public string Name { get; }

      public SubTestResult (bool hasSucceeded, string message, string imageSource, string resourceName, string testName)
      {
        HasSucceeded = hasSucceeded;
        Message = message;
        ImageSource = imageSource;
        ResourceName = resourceName;
        Name = testName;
      }
    }

    private const string c_savedScreenshotsFolder = "SavedTestScreenshots";
    private const int c_allowedPixelVariance = 10;
    private const double c_maxUnequalPixelThresholdRatio = 0.01;

    /// <summary>
    /// Runs the specific <paramref name="test"/> delegate as screenshot setup and compares the resulting image with the stored image.
    /// <paramref name="type"/> controls which screenshot-types will be used as input.
    /// </summary>
    /// <typeparam name="TValue">Type of the value which will be passed to the test delegate.</typeparam>
    /// <typeparam name="TTarget">The containing type which will be used with resource name resolution.</typeparam>
    /// <param name="helper">The <see cref="WebTestHelper"/> for the test session.</param>
    /// <param name="test">The test delegate that will annotated the <see cref="ScreenshotBuilder"/>.</param>
    /// <param name="type">The types of tests that will be tested on the <paramref name="test"/> delegate.</param>
    /// <param name="testName">The name of the test which will be used in resource name resolution.</param>
    /// <param name="value">The value that will be passed to the test delegate.</param>
    /// <param name="maxAllowedPixelVariance">The maximum amount of RGBA values that are allowed before two pixels are considered different.</param>
    /// <param name="unequalPixelThreshold">The maximum amount of pixels that are allowed to have changed for the two images to be considered equal (measured in %).</param>
    public static void RunTest<TValue, TTarget> (
        WebTestHelper helper,
        ScreenshotTestingDelegate<TValue> test,
        ScreenshotTestingType type,
        string testName,
        TValue value,
        int? maxAllowedPixelVariance = null,
        double? unequalPixelThreshold = null)
    {
      var maxVariance = maxAllowedPixelVariance ?? c_allowedPixelVariance;
      var maxRatio = unequalPixelThreshold ?? c_maxUnequalPixelThresholdRatio;

      var savePath = helper.TestInfrastructureConfiguration.ScreenshotDirectory ?? Path.GetTempPath();

      var results = new List<SubTestResult>();

      if (type.HasFlag(ScreenshotTestingType.Desktop) && !helper.MainBrowserSession.Headless)
      {
        using (var diagnosticScreenshotBuilder = DiagnosticScreenshotBuilder.CreateDesktopScreenshot(helper.BrowserConfiguration.Locator, helper.LoggerFactory))
        {
          results.Add(RunSubTest<TValue, TTarget>(helper, diagnosticScreenshotBuilder, test, value, "Desktop", testName, savePath, maxVariance, maxRatio));
        }
      }

      if (type.HasFlag(ScreenshotTestingType.Browser))
      {
        using (var diagnosticScreenshotBuilder = DiagnosticScreenshotBuilder.CreateBrowserScreenshot(
                   helper.BrowserConfiguration.Locator,
                   helper.MainBrowserSession,
                   helper.LoggerFactory))
        {
          results.Add(
              RunSubTest<TValue, TTarget>(
                  helper,
                  diagnosticScreenshotBuilder,
                  test,
                  value,
                  "Browser",
                  testName,
                  savePath,
                  maxVariance,
                  maxRatio));
        }
      }

      var stringBuilder = new StringBuilder();
      var failed = results.Count(t => !t.HasSucceeded);
      if (failed == 0)
        stringBuilder.AppendLine(string.Format("All {0} tests completed successfully.", results.Count));
      else
        stringBuilder.AppendLine(string.Format("{0} out of {1} sub tests failed:", failed, results.Count));

      var hasFailed = false;
      foreach (var testResult in results)
      {
        stringBuilder.AppendLine();
        if (!testResult.HasSucceeded)
        {
          hasFailed = true;
          stringBuilder.AppendLine(string.Format("Test '{0}' failed:", testResult.Name));
          stringBuilder.AppendLine(string.Format("message: {0}", testResult.Message));
          stringBuilder.AppendLine(string.Format("source: {0}", testResult.ImageSource));
        }
        else
        {
          stringBuilder.AppendLine(string.Format("Test '{0}' succeeded:", testResult.Name));
        }
        stringBuilder.AppendLine(string.Format("resource(s): {0}", testResult.ResourceName));
      }

      if (hasFailed)
        Assert.Fail(stringBuilder.ToString());
    }

    private static SubTestResult RunSubTest<TValue, TTarget> (
        WebTestHelper helper,
        DiagnosticScreenshotBuilder diagnosticScreenshotBuilder,
        ScreenshotTestingDelegate<TValue> test,
        TValue value,
        string testPrefix,
        string testName,
        string savePath,
        int maxVariance,
        double maxRatio)
    {
      diagnosticScreenshotBuilder.DrawMouseCursor = false;

      test(diagnosticScreenshotBuilder, value);

      var typeName = typeof(TTarget).FullName;
      var testAssembly = test.Method.DeclaringType.Assembly;
      var testAssemblyName = testAssembly.GetName().Name;
      var embeddedResources = new HashSet<string>(testAssembly.GetManifestResourceNames());
      if (typeName.StartsWith(testAssemblyName + "."))
        typeName = typeName.Substring(testAssemblyName.Length + 1);

      // Save the screenshot
      string path;
      if (!string.IsNullOrWhiteSpace(testPrefix))
        path = Path.Combine(savePath, string.Join(".", typeName, helper.BrowserConfiguration.BrowserName, testPrefix, testName));
      else
        path = Path.Combine(savePath, string.Join(".", typeName, helper.BrowserConfiguration.BrowserName, testName));

      var screenshotPath = string.Join(".", path, "png");
      var annotationPath = string.Join(".", path, "AnnotationsOnly", "png");

      diagnosticScreenshotBuilder.Save(screenshotPath, true);
      diagnosticScreenshotBuilder.SaveAnnotation(annotationPath, true);

      // Try to find the resource which belongs to the current test
      var resourcePrefixes = GenerateResourcePrefixes(typeName, testAssemblyName, helper.BrowserConfiguration.BrowserName, testPrefix, testName);
      var resourceNames = new List<string>();
      foreach (var resourcePrefix in resourcePrefixes)
      {
        // try to find a/some resource/s with that resource prefix
        var neutralName = string.Join(".", resourcePrefix, "png");
        if (embeddedResources.Contains(neutralName))
          resourceNames.Add(neutralName);
        else
          resourceNames.AddRange(
              Enumerable.Range(0, 10).Select(n => string.Format("{0}{1}.png", resourcePrefix, n)).TakeWhile(embeddedResources.Contains));
      }

      var testNameWithPrefix = string.Join(".", testPrefix, testName);

      if (resourceNames.Count == 0)
        return new SubTestResult(
            false,
            "Can not find a resource image that belongs to the specified test.",
            screenshotPath,
            string.Join(", ", resourcePrefixes),
            testNameWithPrefix);

      var result =
          CompareScreenshots(
              resourceNames.ToArray(),
              testAssembly,
              screenshotPath,
              maxVariance,
              maxRatio,
              testNameWithPrefix);
      if (result.HasSucceeded)
      {
        File.Delete(screenshotPath);
        File.Delete(annotationPath);
      }

      return result;
    }

    private static string[] GenerateResourcePrefixes (string typeName, string assemblyName, string browser, string format, string name)
    {
      return new[]
             {
                 string.Join(".", assemblyName, c_savedScreenshotsFolder, typeName, browser, format, name),
                 string.Join(".", assemblyName, c_savedScreenshotsFolder, typeName, browser, "any", name),
                 string.Join(".", assemblyName, c_savedScreenshotsFolder, typeName, format, name),
                 string.Join(".", assemblyName, c_savedScreenshotsFolder, typeName, name)
             };
    }

    private static SubTestResult CompareScreenshots (string[] resourceNames, Assembly testAssembly, string sourcePath, int maxVariance, double maxRatio, string testName)
    {
      var stringBuilder = new StringBuilder();

      using (var source = (Bitmap)Image.FromFile(sourcePath))
      {
        foreach (var resourceName in resourceNames)
        {
          stringBuilder.Append("resource: ");
          stringBuilder.AppendLine(resourceName);

          using (var resourceStream = testAssembly.GetManifestResourceStream(resourceName))
          {
            if (resourceStream == null)
              Assert.Fail($"Could not open saved resource image: '{resourceName}'");

            var resource = (Bitmap)Image.FromStream(resourceStream);

            if (resource.Size != source.Size)
            {
              stringBuilder.AppendLine("Image sizes do not match.");
              stringBuilder.AppendLine(string.Format("source size: {0}x{1}", source.Width, source.Height));
              stringBuilder.AppendLine(string.Format("resource size: {0}x{1}", resource.Width, resource.Height));
              stringBuilder.AppendLine();
              continue;
            }

            var totalPixel = source.Width * source.Height;
            var pixelOverLimit = 0;
            for (var i = 0; i < source.Width; i++)
              for (var j = 0; j < source.Height; j++)
              {
                var sourceColor = source.GetPixel(i, j);
                var resourceColor = resource.GetPixel(i, j);

                int variance;
                if (!AreSameColor(sourceColor, resourceColor, out variance, maxVariance) || variance != 0)
                {
                  pixelOverLimit++;
                }
              }

            var unequalPixelRatio = pixelOverLimit / (double)totalPixel;
            if (unequalPixelRatio > maxRatio)
            {
              stringBuilder.AppendLine("Images are not considered identical.");
              stringBuilder.AppendLine(string.Format("unequal ratio: {0}", unequalPixelRatio));
              stringBuilder.AppendLine(string.Format("max unequal ratio: {0}", maxRatio));
              stringBuilder.AppendLine();
              continue;
            }

            return new SubTestResult(true, null, null, resourceName, testName);
          }
        }

        return new SubTestResult(false, stringBuilder.ToString(), sourcePath, string.Join(", ", resourceNames), testName);
      }
    }

    /// <summary>
    /// Checks that <paramref name="source"/> is the same color as <paramref name="resource"/> allowing 
    /// a certain degree of <paramref name="allowedVariance"/> and returning the <paramref name="variance"/>
    /// between the two colors. If <paramref name="resource"/> is transparent the color check is skipped.
    /// </summary>
    private static bool AreSameColor (Color source, Color resource, out int variance, int allowedVariance = 0)
    {
      var sourceArgb = (uint)source.ToArgb();
      var resourceArgb = (uint)resource.ToArgb();

      if (sourceArgb == resourceArgb || (resourceArgb & 0xFF000000u) == 0)
      {
        variance = 0;
        return true;
      }

      if (allowedVariance == 0)
      {
        variance = 0;
        return false;
      }

      var total = 0;
      for (var i = 0; i < 4; i++)
      {
        var sourceValue = (byte)((sourceArgb >> (8 * i)) & 0xFF);
        var resourceValue = (byte)((resourceArgb >> (8 * i)) & 0xFF);
        if (sourceValue < resourceValue)
          total += resourceValue - sourceValue;
        else
          total += sourceValue - resourceValue;
      }

      variance = total;
      return total < allowedVariance;
    }
  }
}
