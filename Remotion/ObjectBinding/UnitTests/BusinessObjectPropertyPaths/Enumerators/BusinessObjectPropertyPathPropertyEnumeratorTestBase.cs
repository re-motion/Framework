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
using Moq;

namespace Remotion.ObjectBinding.UnitTests.BusinessObjectPropertyPaths.Enumerators
{
  public class BusinessObjectPropertyPathPropertyEnumeratorTestBase
  {
    protected static Mock<IBusinessObjectClass> CreateClassStub ()
    {
      var classStub = new Mock<IBusinessObjectClass>();
      classStub.Setup (_ => _.BusinessObjectProvider).Returns (new Mock<IBusinessObjectProvider>().Object);
      Mock.Get (classStub.Object.BusinessObjectProvider).Setup (_ => _.GetPropertyPathSeparator()).Returns (':');
      return classStub;
    }

    protected Mock<IBusinessObjectProperty> CreatePropertyStub (Mock<IBusinessObjectClass> classStub, string propertyIdentifier)
    {
      var propertyStub = new Mock<IBusinessObjectProperty>();
      propertyStub.Setup (_ => _.Identifier).Returns (propertyIdentifier);
      classStub.Setup (_ => _.GetPropertyDefinition (propertyIdentifier)).Returns (propertyStub.Object);
      return propertyStub;
    }

    protected Mock<IBusinessObjectReferenceProperty> CreateReferencePropertyStub (
        Mock<IBusinessObjectClass> classStub,
        string propertyIdentifier,
        Mock<IBusinessObjectClass> referenceClassStub)
    {
      var propertyStub = new Mock<IBusinessObjectReferenceProperty>();
      propertyStub.Setup (_ => _.Identifier).Returns (propertyIdentifier);
      propertyStub.Setup (_ => _.ReferenceClass).Returns (referenceClassStub.Object);
      classStub.Setup (_ => _.GetPropertyDefinition (propertyIdentifier)).Returns (propertyStub.Object);
      return propertyStub;
    }
  }
}