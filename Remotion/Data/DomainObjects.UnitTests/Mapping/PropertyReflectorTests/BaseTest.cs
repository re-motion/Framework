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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.PropertyReflectorTests
{
  public class BaseTest : MappingReflectionTestBase
  {
    protected PropertyReflector CreatePropertyReflector<T> (
        string property,
        IDomainModelConstraintProvider domainModelConstraintProvider,
        IPropertyDefaultValueProvider propertyDefaultValueProvider)
    {
      ArgumentUtility.CheckNotNullOrEmpty("property", property);

      Type type = typeof(T);
      TypeDefinition typeDefinition;
      if (ReflectionUtility.IsDomainObject(type))
      {
        typeDefinition = TypeDefinitionObjectMother.CreateClassDefinition(classType: type, isAbstract: true);
      }
      else
      {
        typeDefinition = TypeDefinitionObjectMother.CreateClassDefinition(classType: type, isAbstract: false);
      }

      return CreatePropertyReflector<T>(property, typeDefinition, domainModelConstraintProvider, propertyDefaultValueProvider);
    }

    protected PropertyReflector CreatePropertyReflector<T> (
        string property,
        TypeDefinition typeDefinition,
        IDomainModelConstraintProvider domainModelConstraintProvider,
        IPropertyDefaultValueProvider propertyDefaultValueProvider)
    {
      ArgumentUtility.CheckNotNullOrEmpty("property", property);
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);

      Type type = typeof(T);
      var propertyInfo = PropertyInfoAdapter.Create(
          type.GetProperty(property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));

      return new PropertyReflector(
          typeDefinition,
          propertyInfo,
          Configuration.NameResolver,
          PropertyMetadataProvider,
          domainModelConstraintProvider,
          propertyDefaultValueProvider);
    }
  }
}
