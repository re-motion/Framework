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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// Used to create the <see cref="IRelationEndPointDefinition"/> from a <see cref="IPropertyInformation"/> for types persisted in an <b>RDBMS</b>.
  /// </summary>
  public class RdbmsRelationEndPointReflector : RelationEndPointReflector<DBBidirectionalRelationAttribute>
  {
    public RdbmsRelationEndPointReflector (
        TypeDefinition typeDefinition,
        IPropertyInformation propertyInfo,
        IMemberInformationNameResolver nameResolver,
        IPropertyMetadataProvider propertyMetadataProvider,
        IDomainModelConstraintProvider domainModelConstraintProvider,
        ISortExpressionDefinitionProvider sortExpressionDefinitionProvider)
        : base(typeDefinition, propertyInfo, nameResolver, propertyMetadataProvider, domainModelConstraintProvider, sortExpressionDefinitionProvider)
    {
    }

    public override bool IsVirtualEndRelationEndpoint ()
    {
      if (!IsBidirectionalRelation)
        return false;

      if (ReflectionUtility.IsObjectList(PropertyInfo.PropertyType))
        return true;

      if (ReflectionUtility.IsIObjectList(PropertyInfo.PropertyType))
        return true;

      Assertion.DebugIsNotNull(BidirectionalRelationAttribute, "BidirectionalRelationAttribute != null");
      if (BidirectionalRelationAttribute.ContainsForeignKey)
        return false;

      var oppositePropertyInfo = GetOppositePropertyInfo();
      if (oppositePropertyInfo == null)
        return false;

      if (ReflectionUtility.IsDomainObject(oppositePropertyInfo.PropertyType))
        return true;

      return false;
    }
  }
}
