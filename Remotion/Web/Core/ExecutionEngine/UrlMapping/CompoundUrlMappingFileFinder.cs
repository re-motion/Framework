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
using System.Linq;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.UrlMapping
{
  /// <summary>
  /// Creates a <see cref="IUrlMappingFileFinder"/> from the list of <see cref="IUrlMappingFileFinder"/> implementations combining their URL mapping files.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  [ImplementationFor(typeof(IUrlMappingFileFinder), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Compound)]
  public class CompoundUrlMappingFileFinder : IUrlMappingFileFinder
  {
    public CompoundUrlMappingFileFinder (IEnumerable<IUrlMappingFileFinder> urlMappingFileFinders)
    {
      ArgumentUtility.CheckNotNull(nameof(urlMappingFileFinders), urlMappingFileFinders);

      UrlMappingFileFinders = urlMappingFileFinders.ToList().AsReadOnly();
    }

    public IReadOnlyList<IUrlMappingFileFinder> UrlMappingFileFinders { get; }

    /// <inheritdoc />
    public IEnumerable<string> GetUrlMappingFilePaths ()
    {
      return UrlMappingFileFinders
          .SelectMany(e => e.GetUrlMappingFilePaths())
          .Distinct(StringComparer.OrdinalIgnoreCase);
    }
  }
}
