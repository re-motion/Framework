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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// Used to create the <see cref="IRelationEndPointDefinition"/> from a <see cref="IPropertyInformation"/> for types persisted in an <b>RDBMS</b>.
  /// </summary>
  public class RdbmsRelationEndPointReflector : RelationEndPointReflector<DBBidirectionalRelationAttribute>
  {
    public RdbmsRelationEndPointReflector (
        ClassDefinition classDefinition,
        IPropertyInformation propertyInfo,
        IMemberInformationNameResolver nameResolver,
        IPropertyMetadataProvider propertyMetadataProvider,
        IDomainModelConstraintProvider domainModelConstraintProvider)
        : base (classDefinition, propertyInfo, nameResolver, propertyMetadataProvider, domainModelConstraintProvider)
    {
    }

    public override bool IsVirtualEndRelationEndpoint ()
    {
      if (base.IsVirtualEndRelationEndpoint())
        return true;

      return !ContainsKey();
    }

    private bool ContainsKey ()
    {
      if (!IsBidirectionalRelation)
        return true;

      if (BidirectionalRelationAttribute.ContainsForeignKey)
        return true;

      if (ReflectionUtility.IsObjectList (PropertyInfo.PropertyType))  //TODO: RM-7294
        return false;

      var oppositePropertyInfo = GetOppositePropertyInfo();
      if (oppositePropertyInfo == null)
        return true;

      if (ReflectionUtility.IsDomainObject (oppositePropertyInfo.PropertyType))
        return false;

      return true;
    }
  }
}