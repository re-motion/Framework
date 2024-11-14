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
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DomainImplementation.Transport
{
  /// <summary>
  /// Represents an export strategy for <see cref="DomainObject"/> instances using XML serialization. This matches <see cref="XmlImportStrategy"/>.
  /// </summary>
  public class XmlExportStrategy : IExportStrategy
  {
    public static readonly XmlExportStrategy Instance = new XmlExportStrategy();

    public void Export (Stream outputStream, TransportItem[] transportedObjects)
    {
      ArgumentUtility.CheckNotNull("outputStream", outputStream);
      ArgumentUtility.CheckNotNull("transportedObjects", transportedObjects);

      var formatter = new XmlSerializer(typeof(XmlTransportItem[]));
      PerformSerialization(XmlTransportItem.Wrap(transportedObjects), outputStream, formatter);
    }

    protected virtual void PerformSerialization (XmlTransportItem[] transportedObjects, Stream dataStream, XmlSerializer formatter)
    {
      ArgumentUtility.CheckNotNull("transportedObjects", transportedObjects);
      ArgumentUtility.CheckNotNull("dataStream", dataStream);
      ArgumentUtility.CheckNotNull("formatter", formatter);

      var xmlWriterSettings = new XmlWriterSettings() { Indent = true };
      using (XmlWriter xmlWriter = XmlWriter.Create(dataStream, xmlWriterSettings))
      {
        formatter.Serialize(xmlWriter, transportedObjects);
      }
    }
  }
}
