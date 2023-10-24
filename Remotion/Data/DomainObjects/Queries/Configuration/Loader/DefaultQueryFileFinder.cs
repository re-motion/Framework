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
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries.Configuration.Loader
{
  /// <summary>
  /// Default implementation of <see cref="IQueryFileFinder"/> that locates a "queries.xml" file in <see cref="IAppContextProvider"/>.<see cref="IAppContextProvider.BaseDirectory"/>.
  /// </summary>
  [ImplementationFor(typeof(IQueryFileFinder), Position = 0, RegistrationType = RegistrationType.Multiple)]
  public class DefaultQueryFileFinder : IQueryFileFinder
  {
    private const string c_defaultConfigurationFile = "queries.xml";
    private readonly string _queryFile;

    public DefaultQueryFileFinder (IAppContextProvider appContextProvider)
    {
      ArgumentUtility.CheckNotNull(nameof(appContextProvider), appContextProvider);

      _queryFile = Path.Combine(appContextProvider.BaseDirectory, c_defaultConfigurationFile);
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
