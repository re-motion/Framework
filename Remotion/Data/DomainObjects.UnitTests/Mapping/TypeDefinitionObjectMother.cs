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
using Moq;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  public static class TypeDefinitionObjectMother
  {
    public static TypeDefinition CreateClassDefinition (
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
      return ClassDefinitionObjectMother.CreateClassDefinition(
          id,
          classType,
          isAbstract,
          baseClass,
          implementedInterfaces,
          storageGroupType,
          defaultStorageClass,
          persistentMixinFinder,
          instanceCreator);
    }

    public static TypeDefinition CreateClassDefinitionWithDefaultProperties (
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
      return ClassDefinitionObjectMother.CreateClassDefinitionWithDefaultProperties(
          id,
          classType,
          isAbstract,
          baseClass,
          implementedInterfaces,
          storageGroupType,
          defaultStorageClass,
          persistentMixinFinder,
          instanceCreator);
    }

    public static TypeDefinition CreateClassDefinition_WithEmptyMembers_AndDerivedClasses (string id = null, Type classType = null, ClassDefinition baseClass = null)
    {
      return ClassDefinitionObjectMother.CreateClassDefinition_WithEmptyMembers_AndDerivedClasses(
          id,
          classType,
          baseClass);
    }

    public static TypeDefinition CreateClassDefinitionWithTable (
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
      return ClassDefinitionObjectMother.CreateClassDefinitionWithTable(
          storageProviderDefinition,
          id,
          classType,
          isAbstract,
          baseClass,
          implementedInterfaces,
          storageGroupType,
          defaultStorageClass,
          persistentMixinFinder,
          instanceCreator);
    }

    public static TypeDefinition CreateClassDefinitionWithMixins (Type type, params Type[] mixins)
    {
      return ClassDefinitionObjectMother.CreateClassDefinitionWithMixins(type, mixins);
    }

    public static TypeDefinition CreateClassDefinitionWithMixinsAndDefaultProperties (Type type, params Type[] mixins)
    {
      return ClassDefinitionObjectMother.CreateClassDefinitionWithMixinsWithDefaultProperties(type, mixins);
    }

    public static TypeDefinition CreateInterfaceDefinition (
        Type type = null,
        IEnumerable<InterfaceDefinition> extendedInterfaces = null,
        Type storageGroupType = null,
        DefaultStorageClass? defaultStorageClass = null)
    {
      return InterfaceDefinitionObjectMother.CreateInterfaceDefinition(type, extendedInterfaces, storageGroupType, defaultStorageClass);
    }

    public static TypeDefinition CreateInterfaceDefinitionWithDefaultProperties (
        Type type = null,
        IEnumerable<InterfaceDefinition> extendedInterfaces = null,
        Type storageGroupType = null,
        DefaultStorageClass? defaultStorageClass = null)
    {
      return InterfaceDefinitionObjectMother.CreateInterfaceDefinitionWithDefaultProperties(type, extendedInterfaces, storageGroupType, defaultStorageClass);
    }
  }
}
