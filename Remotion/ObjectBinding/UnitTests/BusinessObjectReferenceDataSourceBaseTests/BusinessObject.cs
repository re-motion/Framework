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
using NUnit.Framework;
using Remotion.ObjectBinding.UnitTests.BusinessObjectReferenceDataSourceBaseTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.BusinessObjectReferenceDataSourceBaseTests
{
#pragma warning disable 612,618
  [TestFixture]
  public class BusinessObject
  {
    private IBusinessObjectReferenceProperty _referencePropertyStub;
    private IBusinessObjectDataSource _referencedDataSourceStub;

    [SetUp]
    public void SetUp ()
    {
      _referencedDataSourceStub = MockRepository.GenerateStub<IBusinessObjectDataSource>();
      _referencedDataSourceStub.BusinessObject = MockRepository.GenerateStub<IBusinessObject>();
      _referencedDataSourceStub.Stub (_ => _.BusinessObjectClass).Return (MockRepository.GenerateStub<IBusinessObjectClass>());
      _referencePropertyStub = MockRepository.GenerateStub<IBusinessObjectReferenceProperty> ();
      _referencePropertyStub.Stub (_ => _.ReflectedClass).Return (MockRepository.GenerateStub<IBusinessObjectClass>());
    }

    [Test]
    public void SetValue_OldValueIsNewlyCreatedDefaultValue ()
    {
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub)).Return (null).Repeat.Once ();
      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      _referencePropertyStub.Stub (stub => stub.SupportsDelete).Return (true);
      var oldValue = MockRepository.GenerateStub<IBusinessObject> ();
      _referencePropertyStub.Stub (stub => stub.CreateDefaultValue (_referencedDataSourceStub.BusinessObject))
          .Return (oldValue)
          .Repeat.Once ();

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.Mode = DataSourceMode.Edit;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.True);
      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (oldValue));

      referenceDataSource.BusinessObject = null;

      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.False);
      Assert.That (referenceDataSource.BusinessObject, Is.Null);
      _referencePropertyStub.AssertWasCalled (stub => stub.Delete (_referencedDataSourceStub.BusinessObject, oldValue));
    }
  }
#pragma warning restore 612,618
}