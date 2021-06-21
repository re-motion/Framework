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
using NUnit.Framework;
using Remotion.ObjectBinding.UnitTests.BusinessObjectReferenceDataSourceBaseTests.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.BusinessObjectReferenceDataSourceBaseTests
{
#pragma warning disable 612,618
  [TestFixture]
  public class BusinessObject
  {
    private Mock<IBusinessObjectReferenceProperty> _referencePropertyStub;
    private Mock<IBusinessObjectDataSource> _referencedDataSourceStub;

    [SetUp]
    public void SetUp ()
    {
      _referencedDataSourceStub = new Mock<IBusinessObjectDataSource>();
      _referencedDataSourceStub.SetupProperty (_ => _.BusinessObject);
      _referencedDataSourceStub.Object.BusinessObject = new Mock<IBusinessObject>().Object;
      _referencedDataSourceStub.Setup (_ => _.BusinessObjectClass).Returns (new Mock<IBusinessObjectClass>().Object);
      _referencePropertyStub = new Mock<IBusinessObjectReferenceProperty>();
      _referencePropertyStub.Setup (_ => _.ReflectedClass).Returns (new Mock<IBusinessObjectClass>().Object);
    }

    [Test]
    public void SetValue_OldValueIsNewlyCreatedDefaultValue ()
    {
      Mock.Get (_referencedDataSourceStub.Object.BusinessObject).SetupSequence (stub => stub.GetProperty (_referencePropertyStub.Object))
          .Returns ((object) null)
          .Throws (new InvalidOperationException ("Method is supposed to be called only once!"));
      _referencePropertyStub.Setup (stub => stub.SupportsDefaultValue).Returns (true);
      _referencePropertyStub.Setup (stub => stub.SupportsDelete).Returns (true);
      var oldValue = new Mock<IBusinessObject>();
      _referencePropertyStub.SetupSequence (stub => stub.CreateDefaultValue (_referencedDataSourceStub.Object.BusinessObject))
          .Returns (oldValue.Object)
          .Throws (new InvalidOperationException ("Method is supposed to be called only once!"));

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.Mode = DataSourceMode.Edit;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.True);
      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (oldValue.Object));

      referenceDataSource.BusinessObject = null;

      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.False);
      Assert.That (referenceDataSource.BusinessObject, Is.Null);
      _referencePropertyStub.Verify (stub => stub.CreateDefaultValue (_referencedDataSourceStub.Object.BusinessObject), Times.Once());
    }
  }
#pragma warning restore 612,618
}