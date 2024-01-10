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
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Schemas;
using Remotion.Xml;

namespace Remotion.Web.ExecutionEngine.UrlMapping
{
  /// <summary>
  /// Provides an API for loading <see cref="UrlMappingEntry"/>s from a specified URL mapping file.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  [ImplementationFor(typeof(IUrlMappingFileLoader), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Single)]
  public class UrlMappingFileLoader : IUrlMappingFileLoader
  {
    public UrlMappingFileLoader ()
    {
    }

    /// <inheritdoc />
    public IReadOnlyList<UrlMappingEntry> LoadUrlMappingEntries (string urlMappingFile)
    {
      ArgumentUtility.CheckNotNullOrEmpty("urlMappingFile", urlMappingFile);

      using var reader = new XmlTextReader(urlMappingFile);

      var xmlSchemaSet = new XmlSchemaSet();
      xmlSchemaSet.Add(new UrlMappingSchema().LoadSchemaSet());

      var urlMappingConfiguration = (UrlMappingConfiguration)XmlSerializationUtility.DeserializeUsingSchema(reader, typeof(UrlMappingConfiguration), xmlSchemaSet);
      return urlMappingConfiguration.Mappings.Cast<UrlMappingEntry>().ToArray();
    }
  }
}
