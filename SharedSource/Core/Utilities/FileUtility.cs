// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.IO;
using System.Threading;
#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.Utilities
{
  /// <summary>
  /// Provides utility operations for File-IO.
  /// </summary>
  /// <remarks>
  /// Deleting files in Windows can be an asynchronous operation if the system is already busy. The following code is an example that can result in 
  /// an exception because the file <c>a.txt</c> has not yet been deleted when the copy-operation is attempted.
  /// <code>
  /// File.Delete (@"C:\temp\a.txt")
  /// File.Copy (@"C:\temp\b.txt", @"C:\temp\a.txt")
  /// </code>
  /// </remarks>
  static partial class FileUtility
  {
    public static void DeleteOnDemandAndWaitForCompletion (string fileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty("fileName", fileName);

      if (File.Exists(fileName))
        DeleteAndWaitForCompletion(fileName);
    }

    public static void DeleteAndWaitForCompletion (string fileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty("fileName", fileName);

      File.Delete(fileName);
      while (File.Exists(fileName))
        Thread.Sleep(10);
    }

    public static void MoveAndWaitForCompletion (string sourceFileName, string destinationFileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty("sourceFileName", sourceFileName);
      ArgumentUtility.CheckNotNullOrEmpty("destinationFileName", destinationFileName);

      File.Move(sourceFileName, destinationFileName);

      if (Path.GetFullPath(sourceFileName) == Path.GetFullPath(destinationFileName))
        return;

      while (File.Exists(sourceFileName) || !File.Exists(destinationFileName))
        Thread.Sleep(10);
    }
  }
}
