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
using Moq.Protected;
using Rhino.Mocks;
using MockRepository = Rhino.Mocks.MockRepository;

namespace Remotion.ObjectBinding.UnitTests.BusinessObjectPropertyPaths.BusinessObjectPropertyPathBaseTests
{
  public class BusinessObjectPropertyPathTestHelper
  {
    // types

    // static members

    public const string NotAccessible = "Not Accessible";

    private Mock<IBusinessObjectProperty> _mockProperty;
    private Mock<IBusinessObjectReferenceProperty> _mockReferenceProperty;
    private Mock<IBusinessObjectReferenceProperty> _mockReferenceListProperty;

    private Mock<IBusinessObjectClass> _mockBusinessObjectClass;
    private Mock<IBusinessObjectClassWithIdentity> _mockBusinessObjectClassWithIdentity;

    private Mock<IBusinessObject> _mockBusinessObject;
    private Mock<IBusinessObjectWithIdentity> _mockBusinessObjectWithIdentity;
    private IBusinessObjectWithIdentity[] _businessObjectWithIdentityList;

    private Mock<IBusinessObjectProvider> _mockBusinessObjectProvider;

    // construction and disposing

    public BusinessObjectPropertyPathTestHelper ()
    {
      _mocks = new MockRepository ();

      _mockProperty = new Mock<IBusinessObjectProperty> (MockBehavior.Strict);
      _mockReferenceProperty = new Mock<IBusinessObjectReferenceProperty> (MockBehavior.Strict);
      _mockReferenceListProperty = new Mock<IBusinessObjectReferenceProperty> (MockBehavior.Strict);

      _mockBusinessObjectClass = new Mock<IBusinessObjectClass> (MockBehavior.Strict);
      _mockBusinessObjectClassWithIdentity = new Mock<IBusinessObjectClassWithIdentity> (MockBehavior.Strict);

      _mockBusinessObject = new Mock<IBusinessObject> (MockBehavior.Strict);
      _mockBusinessObjectWithIdentity = new Mock<IBusinessObjectWithIdentity> (MockBehavior.Strict);
      _businessObjectWithIdentityList = new[] { _mockBusinessObjectWithIdentity.Object, new Mock<IBusinessObjectWithIdentity> (MockBehavior.Strict).Object };

      _mockBusinessObjectProvider = new Mock<IBusinessObjectProvider> (MockBehavior.Strict);

      _mockBusinessObject.Setup (_ => _.BusinessObjectClass).Returns (_mockBusinessObjectClass.Object).Verifiable();
      _mockBusinessObjectWithIdentity.Setup (_ => _.BusinessObjectClass).Returns (_mockBusinessObjectClassWithIdentity.Object).Verifiable();
      _mockReferenceProperty.Setup (_ => _.IsList).Returns (false).Verifiable();
      _mockReferenceListProperty.Setup (_ => _.IsList).Returns (true).Verifiable();

      _mockProperty.Setup (_ => _.Identifier).Returns ("Property").Verifiable();
      _mockReferenceProperty.Setup (_ => _.Identifier).Returns ("ReferenceProperty").Verifiable();
      _mockReferenceListProperty.Setup (_ => _.Identifier).Returns ("ReferenceListProperty").Verifiable();

      _mockProperty.Setup (_ => _.BusinessObjectProvider).Returns (_mockBusinessObjectProvider.Object).Verifiable();
      _mockReferenceProperty.Setup (_ => _.BusinessObjectProvider).Returns (_mockBusinessObjectProvider.Object).Verifiable();
      _mockReferenceListProperty.Setup (_ => _.BusinessObjectProvider).Returns (_mockBusinessObjectProvider.Object).Verifiable();

      _mockBusinessObjectClass.Setup (_ => _.BusinessObjectProvider).Returns (_mockBusinessObjectProvider.Object).Verifiable();
      _mockBusinessObjectClassWithIdentity.Setup (_ => _.BusinessObjectProvider).Returns (_mockBusinessObjectProvider.Object).Verifiable();

      _mockBusinessObjectProvider.Setup (_ => _.GetPropertyPathSeparator ()).Returns ('.').Verifiable();
      _mockBusinessObjectProvider.Setup (_ => _.GetNotAccessiblePropertyStringPlaceHolder ()).Returns (BusinessObjectPropertyPathTestHelper.NotAccessible).Verifiable();
    }

    // methods and properties

    public IDisposable Ordered ()
    {
      return _mocks.Ordered ();
    }

    public void ReplayAll ()
    {
    }

    public void VerifyAll ()
    {
      _mockProperty.Verify();
      _mockReferenceProperty.Verify();
      _mockReferenceListProperty.Verify();
      _mockBusinessObjectClass.Verify();
      _mockBusinessObjectClassWithIdentity.Verify();
      _mockBusinessObject.Verify();
      _mockBusinessObjectWithIdentity.Verify();
      _mockBusinessObjectProvider.Verify();
    }

    public IBusinessObjectProperty Property
    {
      get { return _mockProperty.Object; }
    }

    public IBusinessObjectReferenceProperty ReferenceProperty
    {
      get { return _mockReferenceProperty.Object; }
    }

    public IBusinessObjectReferenceProperty ReferenceListProperty
    {
      get { return _mockReferenceListProperty.Object; }
    }

    public IBusinessObjectClass BusinessObjectClass
    {
      get { return _mockBusinessObjectClass.Object; }
    }

    public IBusinessObjectClassWithIdentity BusinessObjectClassWithIdentity
    {
      get { return _mockBusinessObjectClassWithIdentity.Object; }
    }

    public IBusinessObject BusinessObject
    {
      get { return _mockBusinessObject.Object; }
    }

    public IBusinessObjectWithIdentity BusinessObjectWithIdentity
    {
      get { return _mockBusinessObjectWithIdentity.Object; }
    }

    public IBusinessObjectWithIdentity[] BusinessObjectWithIdentityList
    {
      get { return _businessObjectWithIdentityList; }
    }

    public void ExpectOnceOnGetProperty (IBusinessObject businessObject, IBusinessObjectProperty property, object returnValue)
    {
      businessObject.Setup (_ => _.GetProperty (property)).Returns (returnValue).Verifiable();
    }

    public void ExpectThrowOnGetProperty (IBusinessObject businessObject, IBusinessObjectProperty property, Exception exception)
    {
      businessObject.Setup (_ => _.GetProperty (property)).Throws (exception).Verifiable();
    }

    public void ExpectOnceOnIsAccessible (
        IBusinessObjectClass businessObjectClass, 
        IBusinessObject businessObject, 
        IBusinessObjectProperty property, 
        bool returnValue)
    {
      property.Setup (_ => _.IsAccessible (businessObject)).Returns (returnValue).Verifiable();
    }
  }
}
