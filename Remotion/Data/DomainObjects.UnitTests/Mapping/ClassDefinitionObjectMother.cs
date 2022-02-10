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
using System.Linq;
using Moq;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  public static class ClassDefinitionObjectMother
  {
    public static ClassDefinition CreateClassDefinition (
        string id = null,
        Type classType = null,
        bool isAbstract = false,
        ClassDefinition baseClass = null,
        IEnumerable<InterfaceDefinition> implementedInterfaces = null,
        Type storageGroupType = null,
        DefaultStorageClass? defaultStorageClass = null,
        IPersistentMixinFinder persistentMixinFinder = null,
        IDomainObjectCreator instanceCreator = null)
    {
      id = id ?? "Test";
      classType = classType ?? typeof(Order);
      implementedInterfaces = implementedInterfaces ?? Enumerable.Empty<InterfaceDefinition>();
      var defaultStorageClassNotNullable = defaultStorageClass ?? DefaultStorageClass.Persistent;
      persistentMixinFinder = persistentMixinFinder ?? new PersistentMixinFinderStub(classType, Type.EmptyTypes);
      instanceCreator = instanceCreator ?? new Mock<IDomainObjectCreator>(MockBehavior.Strict).Object;

      return new ClassDefinition(
              id,
              classType,
              isAbstract,
              baseClass,
              implementedInterfaces,
              storageGroupType,
              defaultStorageClassNotNullable,
              persistentMixinFinder,
              instanceCreator);
    }

    public static ClassDefinition CreateClassDefinitionWithDefaultProperties (
        string id = null,
        Type classType = null,
        bool isAbstract = false,
        ClassDefinition baseClass = null,
        IEnumerable<InterfaceDefinition> implementedInterfaces = null,
        Type storageGroupType = null,
        DefaultStorageClass? defaultStorageClass = null,
        IPersistentMixinFinder persistentMixinFinder = null,
        IDomainObjectCreator instanceCreator = null)
    {
      var classDefinition = CreateClassDefinition(id, classType, isAbstract, baseClass, implementedInterfaces, storageGroupType, defaultStorageClass, persistentMixinFinder, instanceCreator);

      classDefinition.SetStorageEntity(new FakeStorageEntityDefinition(new UnitTestStorageProviderStubDefinition("stub"), "stub"));
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection());
      classDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection());
      classDefinition.SetDerivedClasses(Array.Empty<ClassDefinition>());

      return classDefinition;
    }

    public static ClassDefinition CreateClassDefinition_WithEmptyMembers_AndDerivedClasses (string id = null, Type classType = null, ClassDefinition baseClass = null)
    {
      var classDefinition = CreateClassDefinition(id: id, classType: classType, baseClass: baseClass);

      classDefinition.SetDerivedClasses(new ClassDefinition[0]);
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection());
      classDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection());

      return classDefinition;
    }

    public static ClassDefinition CreateClassDefinitionWithTable (
        StorageProviderDefinition storageProviderDefinition,
        string id = null,
        Type classType = null,
        bool isAbstract = false,
        ClassDefinition baseClass = null,
        IEnumerable<InterfaceDefinition> implementedInterfaces = null,
        Type storageGroupType = null,
        DefaultStorageClass? defaultStorageClass = null,
        IPersistentMixinFinder persistentMixinFinder = null,
        IDomainObjectCreator instanceCreator = null)
    {
        var classDefinition = CreateClassDefinition(
                id,
                classType,
                isAbstract,
                baseClass,
                implementedInterfaces,
                storageGroupType,
                defaultStorageClass,
                persistentMixinFinder,
                instanceCreator);
      classDefinition.SetStorageEntity(TableDefinitionObjectMother.Create(storageProviderDefinition));
      return classDefinition;
    }

    public static ClassDefinition CreateClassDefinitionWithMixins (Type type, params Type[] mixins)
    {
      return CreateClassDefinition(classType: type, persistentMixinFinder: new PersistentMixinFinderStub(type, mixins));
    }

    public static ClassDefinition CreateClassDefinitionWithMixinsWithDefaultProperties (Type type, params Type[] mixins)
    {
      return CreateClassDefinitionWithDefaultProperties(classType: type, persistentMixinFinder: new PersistentMixinFinderStub(type, mixins));
    }
  }
}
