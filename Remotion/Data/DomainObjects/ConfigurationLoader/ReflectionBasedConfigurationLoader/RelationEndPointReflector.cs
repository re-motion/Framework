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
using System.Threading;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
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
        IDomainModelConstraintProvider domainModelConstraintProvider,
        ISortExpressionDefinitionProvider sortExpressionDefinitionProvider)
    {
      return new RdbmsRelationEndPointReflector(
          classDefinition,
          propertyInfo,
          nameResolver,
          propertyMetadataProvider,
          domainModelConstraintProvider,
          sortExpressionDefinitionProvider);
    }
  }

  /// <summary>Used to create the <see cref="IRelationEndPointDefinition"/> from a <see cref="IPropertyInformation"/>.</summary>
  public abstract class RelationEndPointReflector<T> : RelationReflectorBase<T>
      where T: BidirectionalRelationAttribute
  {
    private class DeferredRelationEndPointDefinition
    {
      public IRelationEndPointDefinition? Value;
    }

    private readonly IDomainModelConstraintProvider _domainModelConstraintProvider;
    private readonly ISortExpressionDefinitionProvider _sortExpressionDefinitionProvider;

    protected RelationEndPointReflector (
        ClassDefinition classDefinition,
        IPropertyInformation propertyInfo,
        IMemberInformationNameResolver nameResolver,
        IPropertyMetadataProvider propertyMetadataProvider,
        IDomainModelConstraintProvider domainModelConstraintProvider,
        ISortExpressionDefinitionProvider sortExpressionDefinitionProvider)
        : base(classDefinition, propertyInfo, nameResolver, propertyMetadataProvider)
    {
      ArgumentUtility.CheckNotNull("domainModelConstraintProvider", domainModelConstraintProvider);
      ArgumentUtility.CheckNotNull("sortExpressionDefinitionProvider", sortExpressionDefinitionProvider);

      _domainModelConstraintProvider = domainModelConstraintProvider;
      _sortExpressionDefinitionProvider = sortExpressionDefinitionProvider;
    }

    public abstract bool IsVirtualEndRelationEndpoint ();

    public IRelationEndPointDefinition GetMetadata ()
    {
      if (IsVirtualEndRelationEndpoint())
        return CreateVirtualRelationEndPointDefinition(ClassDefinition);
      else
        return CreateRelationEndPointDefinition(ClassDefinition);
    }

    private IRelationEndPointDefinition CreateRelationEndPointDefinition (ClassDefinition classDefinition)
    {
      var propertyName = GetPropertyName();

      PropertyDefinition propertyDefinition = classDefinition.GetMandatoryPropertyDefinition(propertyName);
      if (!propertyDefinition.IsObjectID)
        return new TypeNotObjectIDRelationEndPointDefinition(classDefinition, propertyName, propertyDefinition.PropertyInfo.PropertyType);
      else
        return new RelationEndPointDefinition(propertyDefinition, IsMandatory());
    }

    private IRelationEndPointDefinition CreateVirtualRelationEndPointDefinition (ClassDefinition classDefinition)
    {
      if (ReflectionUtility.IsDomainObject(PropertyInfo.PropertyType))
      {
        var relationEndPointDefinition = new VirtualObjectRelationEndPointDefinition(
            classDefinition,
            GetPropertyName(),
            IsMandatory(),
            PropertyInfo);

        Assertion.DebugIsNotNull(BidirectionalRelationAttribute, "Cannot call {0}(...) with for unidirectional relations.", nameof(CreateVirtualRelationEndPointDefinition));
        if (BidirectionalRelationAttribute.SortExpression != null)
          relationEndPointDefinition.SetHasSortExpressionFlag();

        return relationEndPointDefinition;
      }
      else if (ReflectionUtility.IsObjectList(PropertyInfo.PropertyType))
      {
        var deferredRelationEndPointDefinition = new DeferredRelationEndPointDefinition();
        var relationEndPointDefinition = new DomainObjectCollectionRelationEndPointDefinition(
            classDefinition,
            GetPropertyName(),
            IsMandatory(),
            GetSortExpressionDefinition(deferredRelationEndPointDefinition),
            PropertyInfo);
        deferredRelationEndPointDefinition.Value = relationEndPointDefinition;

        return relationEndPointDefinition;
      }
      else if (ReflectionUtility.IsIObjectList(PropertyInfo.PropertyType))
      {
        var deferredRelationEndPointDefinition = new DeferredRelationEndPointDefinition();
        var relationEndPointDefinition = new VirtualCollectionRelationEndPointDefinition(
            classDefinition,
            GetPropertyName(),
            IsMandatory(),
            GetSortExpressionDefinition(deferredRelationEndPointDefinition),
            PropertyInfo);
        deferredRelationEndPointDefinition.Value = relationEndPointDefinition;

        return relationEndPointDefinition;
      }
      else
      {
        return new TypeNotCompatibleWithVirtualRelationEndPointDefinition(classDefinition, GetPropertyName(), PropertyInfo.PropertyType);
      }
    }

    private bool IsMandatory ()
    {
      return !_domainModelConstraintProvider.IsNullable(PropertyInfo);
    }

    private Lazy<SortExpressionDefinition?> GetSortExpressionDefinition (DeferredRelationEndPointDefinition deferredRelationEndPointDefinition)
    {
      Assertion.DebugIsNotNull(BidirectionalRelationAttribute, "Cannot call {0}(...) with for unidirectional relations.", nameof(GetSortExpressionDefinition));

      var propertyInfo = PropertyInfo;
      var sortExpressionText = BidirectionalRelationAttribute.SortExpression;
      var sortExpressionDefinitionProvider = _sortExpressionDefinitionProvider;

      return new Lazy<SortExpressionDefinition?>(
          () =>
          {
            var relationEndPointDefinition = deferredRelationEndPointDefinition.Value;
            Assertion.IsNotNull(relationEndPointDefinition, "RelationEndPointDefinition was not initialized.");
            var oppositeClassDefinition = relationEndPointDefinition.GetOppositeEndPointDefinition().ClassDefinition;
            return sortExpressionDefinitionProvider.GetSortExpression(propertyInfo, oppositeClassDefinition, sortExpressionText);
          }, LazyThreadSafetyMode.ExecutionAndPublication);
    }
  }
}
