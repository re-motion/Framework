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
using System.Xml.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.MappingExport
{
  /// <summary>
  /// Default implementation for <see cref="ITypeSerializer"/>
  /// </summary>
  public class TypeSerializer : ITypeSerializer
  {
    private readonly ITableSerializer _tableSerializer;

    public TypeSerializer (ITableSerializer tableSerializer)
    {
      ArgumentUtility.CheckNotNull("tableSerializer", tableSerializer);

      _tableSerializer = tableSerializer;
    }

    public XElement Serialize (TypeDefinition typeDefinition)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);

      if (typeDefinition is not ClassDefinition classDefinition)
        throw new NotSupportedException("Only class definitions are supported"); // TODO R2I Mapping: Add support for interfaces

      return new XElement(
          Constants.Namespace + "class",
          GetIdAttribute(classDefinition),
          GetBaseClassAttribute(classDefinition)!,
          GetIsAbstractAttribute(classDefinition),
          GetTableElements(classDefinition)!
          );
    }

    private static XAttribute GetIdAttribute (ClassDefinition classDefinition)
    {
      return new XAttribute("id", classDefinition.ID);
    }

    private static XAttribute GetIsAbstractAttribute (ClassDefinition classDefinition)
    {
      return new XAttribute("isAbstract", classDefinition.IsAbstract);
    }

    private IEnumerable<XElement>? GetTableElements (ClassDefinition classDefinition)
    {
      if (!classDefinition.IsAbstract)
        return _tableSerializer.Serialize(classDefinition);
      return null;
    }

    private XAttribute? GetBaseClassAttribute (ClassDefinition classDefinition)
    {
      if (classDefinition.BaseClass == null)
        return null;
      return new XAttribute("baseClass", classDefinition.BaseClass.ID);
    }
  }
}
