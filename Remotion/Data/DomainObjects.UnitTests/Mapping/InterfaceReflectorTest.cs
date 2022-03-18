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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class InterfaceReflectorTest : MappingReflectionTestBase
  {
    private interface IInterfaceWithNoStorageGroupAttribute
    {
    }

    private class CustomStorageGroupAttribute : StorageGroupAttribute
    {
      public CustomStorageGroupAttribute ()
          : base(DefaultStorageClass.Transaction)
      {
      }
    }

    [CustomStorageGroup]
    interface IInterfaceWithStorageGroupAttribute
    {
    }

    [Test]
    public void GetMetadata ()
    {
      var implementedInterface = InterfaceDefinitionObjectMother.CreateInterfaceDefinition();
      var interfaceReflector = CreateInterfaceReflector(typeof(IInterfaceWithNoStorageGroupAttribute));

      var interfaceDefinition = interfaceReflector.GetMetadata(new[] { implementedInterface });
      Assert.That(interfaceDefinition.Type, Is.EqualTo(typeof(IInterfaceWithNoStorageGroupAttribute)));
      Assert.That(interfaceDefinition.ExtendedInterfaces, Is.EqualTo(new[] { implementedInterface }));
      Assert.That(interfaceDefinition.StorageGroupType, Is.Null);
      Assert.That(interfaceDefinition.DefaultStorageClass, Is.EqualTo(DefaultStorageClass.Persistent));
      Assert.That(interfaceDefinition.MyPropertyDefinitions, Is.Empty);
      Assert.That(interfaceDefinition.MyRelationEndPointDefinitions, Is.Empty);
    }

    [Test]
    public void GetMetadata_WithStorageGroupAttribute_SetsStorageGroupTypeAndDefaultStorageClass ()
    {
      var interfaceReflector = CreateInterfaceReflector(typeof(IInterfaceWithStorageGroupAttribute));

      var interfaceDefinition = interfaceReflector.GetMetadata(Enumerable.Empty<InterfaceDefinition>());
      Assert.That(interfaceDefinition.Type, Is.EqualTo(typeof(IInterfaceWithStorageGroupAttribute)));
      Assert.That(interfaceDefinition.ExtendedInterfaces, Is.Empty);
      Assert.That(interfaceDefinition.StorageGroupType, Is.EqualTo(typeof(CustomStorageGroupAttribute)));
      Assert.That(interfaceDefinition.DefaultStorageClass, Is.EqualTo(DefaultStorageClass.Transaction));
    }

    private InterfaceReflector CreateInterfaceReflector (Type type)
    {
      return new InterfaceReflector(
          type,
          MappingObjectFactory,
          Configuration.NameResolver,
          PropertyMetadataProvider,
          DomainModelConstraintProviderStub.Object,
          SortExpressionDefinitionProviderStub.Object);
    }
  }
}
