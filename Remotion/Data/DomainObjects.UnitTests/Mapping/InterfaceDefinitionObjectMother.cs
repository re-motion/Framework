﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  public static class InterfaceDefinitionObjectMother
  {
    public static InterfaceDefinition CreateInterfaceDefinition (
        Type type = null,
        IEnumerable<InterfaceDefinition> extendedInterfaces = null,
        Type storageGroupType = null,
        DefaultStorageClass? defaultStorageClass = null)
    {
      type ??= typeof(IOrder);
      extendedInterfaces ??= Array.Empty<InterfaceDefinition>();
      defaultStorageClass ??= DefaultStorageClass.Persistent;

      return new InterfaceDefinition(
          type,
          extendedInterfaces,
          storageGroupType,
          defaultStorageClass.Value);
    }

    public static InterfaceDefinition CreateInterfaceDefinitionWithDefaultProperties (
        Type type = null,
        IEnumerable<InterfaceDefinition> extendedInterfaces = null,
        Type storageGroupType = null,
        DefaultStorageClass? defaultStorageClass = null)
    {
      var interfaceDefinition = CreateInterfaceDefinition(type, extendedInterfaces, storageGroupType, defaultStorageClass);

      interfaceDefinition.SetStorageEntity(new FakeStorageEntityDefinition(new UnitTestStorageProviderStubDefinition("stub"), "stub"));
      interfaceDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection());
      interfaceDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection());
      interfaceDefinition.SetExtendingInterfaces(Array.Empty<InterfaceDefinition>());
      interfaceDefinition.SetImplementingClasses(Array.Empty<ClassDefinition>());

      return interfaceDefinition;
    }
  }
}