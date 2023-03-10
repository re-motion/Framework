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
    protected PropertyReflector CreatePropertyReflector<T> (string property, IDomainModelConstraintProvider domainModelConstraintProvider)
    {
      ArgumentUtility.CheckNotNullOrEmpty("property", property);

      Type type = typeof(T);
      ClassDefinition classDefinition;
      if (ReflectionUtility.IsDomainObject(type))
      {
        classDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: type, isAbstract: true);
      }
      else
      {
        classDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: type, isAbstract: false);
      }

      return CreatePropertyReflector<T>(property, classDefinition, domainModelConstraintProvider);
    }

    protected PropertyReflector CreatePropertyReflector<T> (
        string property,
        ClassDefinition classDefinition,
        IDomainModelConstraintProvider domainModelConstraintProvider)
    {
      ArgumentUtility.CheckNotNullOrEmpty("property", property);
      ArgumentUtility.CheckNotNull("classDefinition", classDefinition);

      Type type = typeof(T);
      var propertyInfo = PropertyInfoAdapter.Create(
          type.GetProperty(property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));

      return new PropertyReflector(
          classDefinition,
          propertyInfo,
          Configuration.NameResolver,
          PropertyMetadataProvider,
          domainModelConstraintProvider,
          new LegacyPropertyDefaultValueProvider());
    }
  }
}
