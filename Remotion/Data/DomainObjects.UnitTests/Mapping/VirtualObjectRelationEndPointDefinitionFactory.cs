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
using Remotion.Development.UnitTesting;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  public static class VirtualObjectRelationEndPointDefinitionFactory
  {
    public static VirtualObjectRelationEndPointDefinition Create (
        ClassDefinition classDefinition,
        string propertyName,
        bool isMandatory,
        Type propertyType)
    {
      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      propertyInformationStub.Stub (stub => stub.Name).Return (propertyName);
      propertyInformationStub.Stub (stub => stub.PropertyType).Return (propertyType);
      propertyInformationStub.Stub (stub => stub.DeclaringType).Return (TypeAdapter.Create (classDefinition.ClassType));

      return new VirtualObjectRelationEndPointDefinition (
          classDefinition, propertyName, isMandatory, propertyInformationStub);
    }

    public static VirtualObjectRelationEndPointDefinition Create (
        ClassDefinition classDefinition,
        string propertyName,
        bool isMandatory,
        Type propertyType,
        string sortExpression)
    {
      var relationEndPointDefinition = Create (classDefinition, propertyName, isMandatory, propertyType);
      if (sortExpression != null)
        PrivateInvoke.InvokeNonPublicMethod (relationEndPointDefinition, "SetHasSortExpressionFlag");

      return relationEndPointDefinition;
    }

  }
}