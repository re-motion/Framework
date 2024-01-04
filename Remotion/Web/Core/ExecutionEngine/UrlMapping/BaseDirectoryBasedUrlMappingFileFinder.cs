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
using System.IO;
using Remotion.FunctionalProgramming;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.UrlMapping
{
  /// <summary>
  /// Class for <see cref="IUrlMappingFileFinder"/> implementations that want to search for a single URL mapping file.
  /// Create a derived class to add a new URL mapping file via IoC.
  /// </summary>
  public class BaseDirectoryBasedUrlMappingFileFinder : IUrlMappingFileFinder
  {
    /// <summary>
    /// Creates a new instance using the ambient <see cref="IAppContextProvider"/> and the specified <paramref name="urlMappingFile"/>.
    /// </summary>
    public static BaseDirectoryBasedUrlMappingFileFinder Create (string urlMappingFile)
    {
      return new BaseDirectoryBasedUrlMappingFileFinder(SafeServiceLocator.Current.GetInstance<IAppContextProvider>(), urlMappingFile);
    }

    private readonly IAppContextProvider _appContextProvider;
    private readonly string _urlMappingFile;

    protected BaseDirectoryBasedUrlMappingFileFinder (IAppContextProvider appContextProvider, string urlMappingFile)
    {
      ArgumentUtility.CheckNotNull(nameof(appContextProvider), appContextProvider);
      ArgumentUtility.CheckNotNullOrEmpty(nameof(urlMappingFile), urlMappingFile);

      _appContextProvider = appContextProvider;
      _urlMappingFile = urlMappingFile;
    }

    /// <inheritdoc />
    public IEnumerable<string> GetUrlMappingFilePaths ()
    {
      var urlMappingFilePath = Path.IsPathRooted(_urlMappingFile)
          ? _urlMappingFile
          : Path.Combine(_appContextProvider.BaseDirectory, _urlMappingFile);

      return File.Exists(urlMappingFilePath)
          ? EnumerableUtility.Singleton(urlMappingFilePath)
          : throw new FileNotFoundException($"The URL mapping file '{urlMappingFilePath}' does not exist.");
    }
  }
}
