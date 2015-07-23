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
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  public static class VirtualRelationEndPointDefinitionFactory
  {
    public static VirtualRelationEndPointDefinition Create (
        ClassDefinition classDefinition,
        string propertyName,
        bool isMandatory,
        CardinalityType cardinality,
        Type propertyType,
        string sortExpressionString)
    {
      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      propertyInformationStub.Stub (stub => stub.Name).Return (propertyName);
      propertyInformationStub.Stub (stub => stub.PropertyType).Return (propertyType);
      propertyInformationStub.Stub (stub => stub.DeclaringType).Return (TypeAdapter.Create (classDefinition.ClassType));

      return new VirtualRelationEndPointDefinition (
          classDefinition, propertyName, isMandatory, cardinality, sortExpressionString, propertyInformationStub);
    }

    public static VirtualRelationEndPointDefinition Create (
        ClassDefinition classDefinition,
        string propertyName,
        bool isMandatory,
        CardinalityType cardinality,
        Type propertyType)
    {
      return Create (classDefinition, propertyName, isMandatory, cardinality, propertyType, null);
    }
  }
}