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
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.BusinessObjectPropertyPaths.Enumerators
{
  public class BusinessObjectPropertyPathPropertyEnumeratorTestBase
  {
    protected static IBusinessObjectClass CreateClassStub ()
    {
      var classStub = MockRepository.GenerateStub<IBusinessObjectClass>();
      classStub.Stub (_ => _.BusinessObjectProvider).Return (MockRepository.GenerateStub<IBusinessObjectProvider>());
      classStub.BusinessObjectProvider.Stub (_ => _.GetPropertyPathSeparator()).Return (':');
      return classStub;
    }

    protected IBusinessObjectProperty CreatePropertyStub (IBusinessObjectClass classStub, string propertyIdentifier)
    {
      var propertyStub = MockRepository.GenerateStub<IBusinessObjectProperty>();
      propertyStub.Stub (_ => _.Identifier).Return (propertyIdentifier);
      classStub.Stub (_ => _.GetPropertyDefinition (propertyIdentifier)).Return (propertyStub);
      return propertyStub;
    }

    protected IBusinessObjectReferenceProperty CreateReferencePropertyStub (
        IBusinessObjectClass classStub,
        string propertyIdentifier,
        IBusinessObjectClass referenceClassStub)
    {
      var propertyStub = MockRepository.GenerateStub<IBusinessObjectReferenceProperty>();
      propertyStub.Stub (_ => _.Identifier).Return (propertyIdentifier);
      propertyStub.Stub (_ => _.ReferenceClass).Return (referenceClassStub);
      classStub.Stub (_ => _.GetPropertyDefinition (propertyIdentifier)).Return (propertyStub);
      return propertyStub;
    }
  }
}