﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Linq;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries.Configuration.Loader
{
  /// <summary>
  /// Creates a <see cref="IQueryFileFinder"/> from the list of <see cref="IQueryFileFinder"/> implementations combining their query files.
  /// </summary>
  [ImplementationFor(typeof(IQueryFileFinder), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Compound)]
  public class CompoundQueryFileFinder : IQueryFileFinder
  {
    public CompoundQueryFileFinder (IEnumerable<IQueryFileFinder> queryFileFinders)
    {
      ArgumentUtility.CheckNotNull(nameof(queryFileFinders), queryFileFinders);

      QueryFileFinders = queryFileFinders.ToList().AsReadOnly();
    }

    public IReadOnlyList<IQueryFileFinder> QueryFileFinders { get; }

    /// <inheritdoc />
    public IEnumerable<string> GetQueryFilePaths ()
    {
      return QueryFileFinders
          .SelectMany(e => e.GetQueryFilePaths())
          .Distinct(StringComparer.OrdinalIgnoreCase);
    }
  }
}
