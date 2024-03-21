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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Base class for reflecting on the relations of a class.</summary>
  public abstract class RelationReflectorBase<T> : MemberReflectorBase where T: BidirectionalRelationAttribute
  {
    protected RelationReflectorBase (
        TypeDefinition typeDefinition,
        IPropertyInformation propertyInfo,
        IMemberInformationNameResolver nameResolver,
        IPropertyMetadataProvider propertyMetadataProvider)
        : base(typeDefinition, propertyInfo, nameResolver, propertyMetadataProvider)
    {
      BidirectionalRelationAttribute = PropertyInfo.GetCustomAttribute<T>(true);
    }

    public T? BidirectionalRelationAttribute { get; private set; }

    [MemberNotNullWhen(true, nameof(BidirectionalRelationAttribute))]
    protected bool IsBidirectionalRelation
    {
      get { return BidirectionalRelationAttribute != null; }
    }

    protected IPropertyInformation? GetOppositePropertyInfo ()
    {
      Assertion.IsNotNull(BidirectionalRelationAttribute, "Property '{0}' is not part of a bi-directional relation.", PropertyInfo.Name);

      var type = ReflectionUtility.GetRelatedObjectTypeFromRelationProperty(PropertyInfo);
      if (type == null)
        return null;

      var propertyFinder = new NameBasedPropertyFinder(
          BidirectionalRelationAttribute.OppositeProperty,
          type,
          true,
          true,
          NameResolver,
          new PersistentMixinFinder(type, true),
          PropertyMetadataProvider);

      return propertyFinder.FindPropertyInfos().LastOrDefault();
    }
  }
}
