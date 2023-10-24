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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries.Configuration.Loader
{
  /// <summary>
  /// Abstract base class for <see cref="IQueryFileFinder"/> implementations that want to search for a single query file.
  /// </summary>
  public abstract class BaseDirectoryBasedQueryFileFinder : IQueryFileFinder
  {
    private readonly IAppContextProvider _appContextProvider;
    private readonly string _queryFile;

    protected BaseDirectoryBasedQueryFileFinder (IAppContextProvider appContextProvider, string queryFile)
    {
      ArgumentUtility.CheckNotNull(nameof(appContextProvider), appContextProvider);
      ArgumentUtility.CheckNotNullOrEmpty(nameof(queryFile), queryFile);

      _appContextProvider = appContextProvider;
      _queryFile = queryFile;
    }

    /// <inheritdoc />
    public IEnumerable<string> GetQueryFilePaths ()
    {
      var queryFilePath = Path.IsPathRooted(_queryFile)
          ? _queryFile
          : Path.Combine(_appContextProvider.BaseDirectory, _queryFile);

      return File.Exists(queryFilePath)
          ? EnumerableUtility.Singleton(queryFilePath)
          : throw new ConfigurationException($"The query file '{queryFilePath}' does not exist.");
    }
  }
}
