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
using System.Runtime.Serialization.Formatters.Binary;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DomainImplementation.Transport
{
  /// <summary>
  /// Represents an export strategy for <see cref="DomainObject"/> instances using binary serialization. This matches <see cref="BinaryImportStrategy"/>.
  /// </summary>
#pragma warning disable SYSLIB0011
  public class BinaryExportStrategy : IExportStrategy
  {
    public static readonly BinaryExportStrategy Instance = new BinaryExportStrategy();

    public void Export (Stream outputStream, TransportItem[] transportedItems)
    {
      ArgumentUtility.CheckNotNull("outputStream", outputStream);
      ArgumentUtility.CheckNotNull("transportedItems", transportedItems);

      var formatter = new BinaryFormatter();
      KeyValuePair<string, Dictionary<string, object?>>[] versionIndependentItems = GetVersionIndependentItems(transportedItems);
      PerformSerialization(versionIndependentItems, outputStream, formatter);
    }

    protected virtual void PerformSerialization (
        KeyValuePair<string, Dictionary<string, object?>>[] versionIndependentItems,
        Stream dataStream,
        BinaryFormatter formatter)
    {
      ArgumentUtility.CheckNotNull("versionIndependentItems", versionIndependentItems);
      ArgumentUtility.CheckNotNull("dataStream", dataStream);
      ArgumentUtility.CheckNotNull("formatter", formatter);

      formatter.Serialize(dataStream, versionIndependentItems);
    }

    private KeyValuePair<string, Dictionary<string, object?>>[] GetVersionIndependentItems (TransportItem[] transportItems)
    {
      return Array.ConvertAll(transportItems,
                               item => new KeyValuePair<string, Dictionary<string, object?>>(item.ID.ToString(), item.Properties));
    }
  }
#pragma warning restore SYSLIB0011
}
