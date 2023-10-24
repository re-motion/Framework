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
using System.Linq;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries.ConfigurationLoader
{
  /// <summary>
  /// Abstract base class for <see cref="IQueryFileFinder"/> implementations that want to search for a single optional query file.
  /// </summary>
  public abstract class BaseDirectoryBasedOptionalQueryFileFinder : IQueryFileFinder
  {
    private readonly string _queryFile;

    protected BaseDirectoryBasedOptionalQueryFileFinder (IAppContextProvider appContextProvider, string queryFile)
    {
      ArgumentUtility.CheckNotNull(nameof(appContextProvider), appContextProvider);
      ArgumentUtility.CheckNotNullOrEmpty(nameof(queryFile), queryFile);

      _queryFile = Path.IsPathRooted(queryFile)
          ? queryFile
          : Path.Combine(appContextProvider.BaseDirectory, queryFile);
    }

    /// <inheritdoc />
    public IEnumerable<string> GetQueryFilePaths ()
    {
      return File.Exists(_queryFile)
          ? EnumerableUtility.Singleton(_queryFile)
          : Enumerable.Empty<string>();
    }
  }
}
