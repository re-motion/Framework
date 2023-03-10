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
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Used to create the <see cref="PropertyDefinition"/> from a <see cref="PropertyInfo"/>.</summary>
  //TODO: Validation: check that only non-virtual relation endpoints are returned as propertydefinition.
  //TODO: Test for null or empty StorageSpecificIdentifier
  public class PropertyReflector : MemberReflectorBase
  {
    private readonly IDomainModelConstraintProvider _domainModelConstraintProvider;
    private readonly IPropertyDefaultValueProvider _propertyDefaultValueProvider;

    public PropertyReflector (
        ClassDefinition classDefinition,
        IPropertyInformation propertyInfo,
        IMemberInformationNameResolver nameResolver,
        IPropertyMetadataProvider propertyMetadataProvider,
        IDomainModelConstraintProvider domainModelConstraintProvider,
        IPropertyDefaultValueProvider propertyDefaultValueProvider)
        : base(classDefinition, propertyInfo, nameResolver, propertyMetadataProvider)
    {
      ArgumentUtility.CheckNotNull("domainModelConstraintProvider", domainModelConstraintProvider);
      ArgumentUtility.CheckNotNull("propertyDefaultValueProvider", propertyDefaultValueProvider);

      _domainModelConstraintProvider = domainModelConstraintProvider;
      _propertyDefaultValueProvider = propertyDefaultValueProvider;
    }

    public PropertyDefinition GetMetadata ()
    {
      var propertyDefinition = new PropertyDefinition(
          ClassDefinition,
          PropertyInfo,
          GetPropertyName(),
          IsDomainObject(),
          IsNullable(),
          _domainModelConstraintProvider.GetMaxLength(PropertyInfo),
          GetStorageClass(),
          _propertyDefaultValueProvider.GetDefaultValue(PropertyInfo, IsNullable()));
      return propertyDefinition;
    }

    private bool IsDomainObject ()
    {
      return ReflectionUtility.IsDomainObject(PropertyInfo.PropertyType);
    }

    private bool IsNullable ()
    {
      if (PropertyInfo.PropertyType.IsValueType)
        return Nullable.GetUnderlyingType(PropertyInfo.PropertyType) != null;

      if (ReflectionUtility.IsDomainObject(PropertyInfo.PropertyType))
        return true;

      return _domainModelConstraintProvider.IsNullable(PropertyInfo);
    }
  }
}
