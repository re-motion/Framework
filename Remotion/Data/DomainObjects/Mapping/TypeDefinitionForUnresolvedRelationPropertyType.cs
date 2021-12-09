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
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  public class TypeDefinitionForUnresolvedRelationPropertyType : TypeDefinition
  {
    private readonly IPropertyInformation _relationProperty;

    public TypeDefinitionForUnresolvedRelationPropertyType (Type type, IPropertyInformation relationProperty)
            : base(type, null, DefaultStorageClass.Persistent)
    {
        ArgumentUtility.CheckNotNull("relationProperty", relationProperty);

        _relationProperty = relationProperty;
    }

    public IPropertyInformation RelationProperty
    {
      get { return _relationProperty; }
    }

    public override bool IsTypeResolved
    {
      get { return false; }
    }

    public override bool IsPartOfInheritanceHierarchy
    {
      get { return false; }
    }

    public override bool IsAssignableFrom (TypeDefinition other)
    {
      return ReferenceEquals(this, other);
    }

    public override TypeDefinition[] GetTypeHierarchy ()
    {
      return Array.Empty<TypeDefinition>();
    }

    public override TypeDefinition GetInheritanceRoot ()
    {
      return this;
    }

    protected override void CheckPropertyDefinitions (IEnumerable<PropertyDefinition> propertyDefinitions)
    {
    }

    protected override void CheckRelationEndPointDefinitions (IEnumerable<IRelationEndPointDefinition> relationEndPoints)
    {
    }
  }
}
