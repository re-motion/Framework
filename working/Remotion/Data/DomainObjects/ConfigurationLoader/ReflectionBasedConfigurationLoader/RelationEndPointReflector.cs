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
  /// <summary>Used to create the <see cref="IRelationEndPointDefinition"/> from a <see cref="IPropertyInformation"/>.</summary>
  public static class RelationEndPointReflector
  {
    public static RdbmsRelationEndPointReflector CreateRelationEndPointReflector (
        ClassDefinition classDefinition,
        IPropertyInformation propertyInfo,
        IMemberInformationNameResolver nameResolver,
        IPropertyMetadataProvider propertyMetadataProvider,
        IDomainModelConstraintProvider domainModelConstraintProvider)
    {
      return new RdbmsRelationEndPointReflector (classDefinition, propertyInfo, nameResolver, propertyMetadataProvider, domainModelConstraintProvider);
    }
  }

  /// <summary>Used to create the <see cref="IRelationEndPointDefinition"/> from a <see cref="IPropertyInformation"/>.</summary>
  public abstract class RelationEndPointReflector<T> : RelationReflectorBase<T>
      where T: BidirectionalRelationAttribute
  {
    private readonly IDomainModelConstraintProvider _domainModelConstraintProvider;

    protected RelationEndPointReflector (
        ClassDefinition classDefinition,
        IPropertyInformation propertyInfo,
        IMemberInformationNameResolver nameResolver,
        IPropertyMetadataProvider propertyMetadataProvider,
        IDomainModelConstraintProvider domainModelConstraintProvider)
        : base (classDefinition, propertyInfo, nameResolver, propertyMetadataProvider)
    {
      ArgumentUtility.CheckNotNull ("domainModelConstraintProvider", domainModelConstraintProvider);

      _domainModelConstraintProvider = domainModelConstraintProvider;
    }

    public IRelationEndPointDefinition GetMetadata ()
    {
      if (IsVirtualEndRelationEndpoint())
        return CreateVirtualRelationEndPointDefinition (ClassDefinition);
      else
        return CreateRelationEndPointDefinition (ClassDefinition);
    }

    public virtual bool IsVirtualEndRelationEndpoint ()
    {
      if (!IsBidirectionalRelation)
        return false;
      return ReflectionUtility.IsObjectList (PropertyInfo.PropertyType);
    }

    private IRelationEndPointDefinition CreateRelationEndPointDefinition (ClassDefinition classDefinition)
    {
      var propertyName = GetPropertyName();

      PropertyDefinition propertyDefinition = classDefinition.GetMandatoryPropertyDefinition (propertyName);
      if (!propertyDefinition.IsObjectID)
        return new TypeNotObjectIDRelationEndPointDefinition (classDefinition, propertyName, propertyDefinition.PropertyInfo.PropertyType);
      else
        return new RelationEndPointDefinition (propertyDefinition, IsMandatory ());
    }

    private IRelationEndPointDefinition CreateVirtualRelationEndPointDefinition (ClassDefinition classDefinition)
    {
      return new VirtualRelationEndPointDefinition (
          classDefinition,
          GetPropertyName(),
          IsMandatory(),
          GetCardinality(),
          GetSortExpression(),
          PropertyInfo);
    }

    private bool IsMandatory ()
    {
      return !_domainModelConstraintProvider.IsNullable (PropertyInfo);
    }

    private CardinalityType GetCardinality ()
    {
      return ReflectionUtility.IsObjectList (PropertyInfo.PropertyType) ? CardinalityType.Many : CardinalityType.One;
    }

    private string GetSortExpression ()
    {
      if (!IsBidirectionalRelation)
        return null;

      return BidirectionalRelationAttribute.SortExpression;
    }
  }
}