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
using System.Xml.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.MappingExport
{
  /// <summary>
  /// Default implementation for <see cref="IColumnSerializer"/> interface.
  /// </summary>
  public class ColumnSerializer : IColumnSerializer
  {
    public ColumnSerializer ()
    {
    }

    public IEnumerable<XElement> Serialize (PropertyDefinition propertyDefinition, IRdbmsPersistenceModelProvider persistenceModelProvider)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      ArgumentUtility.CheckNotNull ("persistenceModelProvider", persistenceModelProvider);

      var storagePropertyDefinition = persistenceModelProvider.GetStoragePropertyDefinition (propertyDefinition);
      return storagePropertyDefinition.GetColumns().Select (
          columnDefinition =>
              new XElement (
                  Constants.Namespace + "column",
                  new XAttribute ("name", columnDefinition.Name),
                  new XAttribute ("dbType", ((StorageTypeInformation) columnDefinition.StorageTypeInfo).StorageDbType)
                  ));
    }
  }
}