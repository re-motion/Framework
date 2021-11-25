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
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// Provides a way to create a valid file path where a screenshot can be saved.
  /// Used by <see cref="TestExecutionScreenshotRecorder"/>
  /// </summary>
  internal static class ScreenshotRecorderPathUtility
  {
    /// <summary>
    /// Combines the given <paramref name="screenshotDirectory"/> with the <paramref name="baseFileName"/>, the suffix <paramref name="suffix"/> and
    /// the file <paramref name="extension"/>. If the full file path would be longer than 259 characters, the <paramref name="baseFileName"/> is
    /// shortened accordingly.
    /// </summary>
    /// <exception cref="PathTooLongException">
    /// If the resulting file path would be longer than 259 characters (despite shortening of the <paramref name="baseFileName"/>).
    /// </exception>
    public static string GetFullScreenshotFilePath ([NotNull] string screenshotDirectory, [NotNull] string baseFileName, [NotNull] string suffix, [NotNull] string extension)
    {
      ArgumentUtility.CheckNotNull("screenshotDirectory", screenshotDirectory);
      ArgumentUtility.CheckNotNull("baseFileName", baseFileName);
      ArgumentUtility.CheckNotNull("suffix", suffix);
      ArgumentUtility.CheckNotNull("extension", extension);

      var sanitizedBaseFileName = SanitizeFileName(baseFileName);
      var filePath = GetFullScreenshotFilePathInternal(screenshotDirectory, sanitizedBaseFileName, suffix, extension);
      if (filePath.Length > 259)
      {
        var shortenedSanitizedBaseFileName = ShortenBaseFileName(filePath, sanitizedBaseFileName);
        filePath = GetFullScreenshotFilePathInternal(screenshotDirectory, shortenedSanitizedBaseFileName, suffix, extension);
      }

      return filePath;
    }

    /// <summary>
    /// Combines the given <paramref name="screenshotDirectory"/> with the <paramref name="baseFileName"/>, the suffix <paramref name="suffix"/> and
    /// the file <paramref name="extension"/>.
    /// </summary>
    private static string GetFullScreenshotFilePathInternal (string screenshotDirectory, string baseFileName, string suffix, string extension)
    {
      var fileName = string.Format("{0}.{1}.{2}", baseFileName, suffix, extension);
      return Path.Combine(screenshotDirectory, fileName);
    }

    /// <summary>
    /// Removes all invalid file name characters from the given <paramref name="fileName"/>.
    /// </summary>
    private static string SanitizeFileName (string fileName)
    {
      foreach (var c in Path.GetInvalidFileNameChars())
        fileName = fileName.Replace(c, '_');

      return fileName;
    }

    /// <summary>
    /// Reduces the <paramref name="baseFileName"/> length such that the <paramref name="fullFilePath"/> is no longer than 259 characters. Throws an
    /// <see cref="PathTooLongException"/> if the <paramref name="baseFileName"/> would have to be reduced to zero characters.
    /// </summary>
    private static string ShortenBaseFileName (string fullFilePath, string baseFileName)
    {
      var overflow = fullFilePath.Length - 259;
      if (overflow > baseFileName.Length - 1)
      {
        throw new PathTooLongException(
            string.Format("Could not save screenshot to '{0}', the file path is too long and cannot be reduced to 259 characters.", fullFilePath));
      }

      return baseFileName.Substring(0, baseFileName.Length - overflow);
    }
  }
}