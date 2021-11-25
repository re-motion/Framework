
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
using System.IO;
using System.Linq;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.UnitTests
{
  [TestFixture]
  public class ScreenshotRecorderPathUtilityTest
  {
    [Test]
    public void GetFullScreenshotFilePath_ShouldReturnCorrectPath ()
    {
      var randomPath = "C:\\temp";
      var randomFilename = "IAmaFilename";
      var randomSuffix = "Browser";
      var randomExtension = "png";

      var fullScreenshotFilePath = CallGetFullScreenshotFilePath(randomPath, randomFilename, randomSuffix, randomExtension);

      var expectedPath = string.Format("{0}\\{1}.{2}.{3}", randomPath, randomFilename, randomSuffix, randomExtension);

      Assert.That(fullScreenshotFilePath, Is.EqualTo(expectedPath));
    }

    [Test]
    public void GetFullScreenshotFilePath_ShouldReplaceInvalidFilenameChars ()
    {
      var randomPath = "C:\\temp";
      var randomFilename = Path.GetInvalidFileNameChars().Aggregate("", (current, invalidFileNameChar) => current + invalidFileNameChar);

      var randomSuffix = "Browser";
      var randomExtension = "png";

      var fullScreenshotFilePath = CallGetFullScreenshotFilePath(randomPath, randomFilename, randomSuffix, randomExtension);

      var fileNameWithInvalidCharsReplaced = new String('_', randomFilename.Length);
      var expectedPath = string.Format("{0}\\{1}.{2}.{3}", randomPath, fileNameWithInvalidCharsReplaced, randomSuffix, randomExtension);

      Assert.That(fullScreenshotFilePath, Is.EqualTo(expectedPath));
    }

    [Test]
    public void GetFullScreenshotFilePath_ShouldNotShortenPathWith259Characters ()
    {
      var randomPath = "C:\\temp";
      var randomSuffix = "Browser";
      var randomExtension = "png";
      var currentPathLength = randomPath.Length + randomSuffix.Length + randomExtension.Length;

      // + 3 is for the extra 3 chars in the final Path to have an resulting path of exactly 259 chars
      var largeFileName = new String('A', 259 - (currentPathLength + 3));

      var fullScreenshotFilePath = CallGetFullScreenshotFilePath(randomPath, largeFileName, randomSuffix, randomExtension);

      var expectedPath = string.Format("{0}\\{1}.{2}.{3}", randomPath, largeFileName, randomSuffix, randomExtension);

      Assert.That(expectedPath.Length, Is.EqualTo(259));
      Assert.That(fullScreenshotFilePath.Length, Is.EqualTo(259));
    }

    [Test]
    public void GetFullScreenshotFilePath_ShouldShortenFileNameWhenPathIsLongerThan259Characters ()
    {
      var randomSuffix = "Browser";
      var randomExtension = "png";

      // + 5 so the FileName gets reduced to 5 chars
      var largePath = new String('A', 259 - (randomSuffix.Length+ randomExtension.Length + 3 + 5));

      var randomFilename = "0123456789";

      var fullScreenshotFilePath = CallGetFullScreenshotFilePath(largePath, randomFilename, randomSuffix, randomExtension);

      var reducedRandomFilename = "01234";
      var expectedPath = string.Format("{0}\\{1}.{2}.{3}", largePath, reducedRandomFilename, randomSuffix, randomExtension);

      Assert.That(fullScreenshotFilePath.Length, Is.EqualTo(259));
      Assert.That(fullScreenshotFilePath, Is.EqualTo(expectedPath));
    }

    [Test]
    public void GetFullScreenshotFilePath_ShouldThrowExceptionWhenFileNameWouldBeReducedToZero ()
    {
      var randomSuffix = "Browser";
      var randomExtension = "png";
      var largePath = new String('A', 259);
      var randomFilename = "IAmaFilename";

      var fullFilePath = string.Format("{0}\\{1}.{2}.{3}", largePath, randomFilename, randomSuffix, randomExtension);

      Assert.That(
          () => CallGetFullScreenshotFilePath(largePath, randomFilename, randomSuffix, randomExtension),
          Throws.Exception.TypeOf<PathTooLongException>().With.Message.EqualTo(
              string.Format("Could not save screenshot to '{0}', the file path is too long and cannot be reduced to 259 characters.", fullFilePath)));
    }

    private string CallGetFullScreenshotFilePath (string screenshotDirectory, string baseFileName, string suffix, string extension)
    {
      var webtestingAssembly = typeof (TestExecutionScreenshotRecorder).Assembly;
      var screenshotRecorderPathUtilityType = webtestingAssembly.GetType("Remotion.Web.Development.WebTesting.Utilities.ScreenshotRecorderPathUtility");

      return (string) PrivateInvoke.InvokePublicStaticMethod(
        screenshotRecorderPathUtilityType,
        "GetFullScreenshotFilePath",
        screenshotDirectory,
        baseFileName,
        suffix,
        extension);
    }
  }
}
