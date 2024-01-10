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
using System.Xml;

namespace Remotion.Web.ExecutionEngine.UrlMapping
{
  /// <summary>
  /// Provides an API for loading <see cref="UrlMappingEntry"/>s from a specified URL mapping file.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  /// <seealso cref="UrlMappingFileLoader"/>
  public interface IUrlMappingFileLoader
  {
    /// <summary>
    /// Loads <see cref="UrlMappingEntry"/>s from the specified <paramref name="urlMappingFile"/>.
    /// </summary>
    /// <exception cref="FileNotFoundException">The specified <paramref name="urlMappingFile"/> was not found.</exception>
    /// <exception cref="XmlException">The specified <paramref name="urlMappingFile"/> could not be read.</exception>
    IReadOnlyList<UrlMappingEntry> LoadUrlMappingEntries (string urlMappingFile);
  }
}
