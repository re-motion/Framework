// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.IO;
using JetBrains.Annotations;
using NUnit.Framework;

namespace Remotion.Web.Development.WebTesting.UnitTests;

public static class SavedTestScreenshots
{
  private const string c_savedScreenshotsFolder = "SavedTestScreenshots";

  public static byte[] GetSavedScreenshotForTest ([CanBeNull] string name = null)
  {
    var test = TestContext.CurrentContext.Test;
    return GetSavedScreenshotForTest(test.Parent?.Name ?? "", name ?? test.Name);
  }

  public static byte[] GetSavedScreenshotForTest (string folder, string fileName)
  {
    var assembly = typeof(SavedTestScreenshots).Assembly;

    var resourceName = string.Join(
        ".",
        assembly.GetName().Name,
        c_savedScreenshotsFolder,
        folder,
        fileName);

    var stream = assembly.GetManifestResourceStream(resourceName);
    if (stream == null)
      throw new InvalidOperationException($"Cannot find a saved screenshot for resource '{folder}/{fileName}'.");

    var memoryStream = new MemoryStream();
    stream.CopyTo(memoryStream);

    return memoryStream.ToArray();
  }
}
