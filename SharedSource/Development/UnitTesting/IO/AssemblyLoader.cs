// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.IO;
using System.Reflection;
using Remotion.Utilities;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.Development.UnitTesting.IO
{
  /// <summary>
  /// Provides functionality for loading an <see cref="Assembly"/> from a file path without locking the file.
  /// </summary>
  static partial class AssemblyLoader
  {
    public static Assembly LoadWithoutLocking (string assemblyFilenameOrPath)
    {
      ArgumentUtility.CheckNotNullOrEmpty("assemblyFilenameOrPath", assemblyFilenameOrPath);

      var bytes = File.ReadAllBytes(assemblyFilenameOrPath);
      return Assembly.Load(bytes);
    }
  }
}
