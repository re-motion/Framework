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
using Remotion.ObjectBinding.Web.Services;

namespace Remotion.ObjectBinding.Web.UnitTests.Services
{
  [TestFixture]
  public class BusinessObjectWebServiceContextTest
  {
    private Mock<IBusinessObjectDataSource> _dataSourceStub;
    private Mock<IBusinessObjectClass> _dataSourceBusinessObjectClassStub;
    private Mock<IBusinessObject> _businessObjectStub;
    private Mock<IBusinessObjectClass> _businessObjectBusinessObjectClassStub;
    private Mock<IBusinessObjectWithIdentity> _businessObjectWithIdentityStub;
    private Mock<IBusinessObjectProperty> _propertyStub;

    [SetUp]
    public void SetUp ()
    {
      _dataSourceBusinessObjectClassStub = new Mock<IBusinessObjectClass>();
      _dataSourceBusinessObjectClassStub.Setup(stub => stub.Identifier).Returns("DataSourceBusinessObjectClass");

      _dataSourceStub = new Mock<IBusinessObjectDataSource>();
      _dataSourceStub.Setup(stub => stub.BusinessObjectClass).Returns(_dataSourceBusinessObjectClassStub.Object);

      _businessObjectBusinessObjectClassStub = new Mock<IBusinessObjectClass>();
      _businessObjectBusinessObjectClassStub.Setup(stub => stub.Identifier).Returns("BusinessObjectBusinessObjectClass");

      _businessObjectStub = new Mock<IBusinessObject>();
      _businessObjectStub.Setup(stub => stub.BusinessObjectClass).Returns(_businessObjectBusinessObjectClassStub.Object);

      _businessObjectWithIdentityStub = new Mock<IBusinessObjectWithIdentity>();
      _businessObjectWithIdentityStub.Setup(stub => stub.UniqueIdentifier).Returns("BusinessObjectIdentifier");

      _propertyStub = new Mock<IBusinessObjectProperty>();
      _propertyStub.Setup(stub => stub.Identifier).Returns("BusinessObjectProperty");
    }

    [Test]
    public void Create_DataSourceNull_SetsBusinessObjectClassNull ()
    {
      var serviceContext = BusinessObjectWebServiceContext.Create(null, null, null);

      Assert.That(serviceContext.BusinessObjectClass, Is.Null);
    }

    [Test]
    public void Create_DataSourceNull_SetsBusinessObjectIdentifierNull ()
    {
      var serviceContext = BusinessObjectWebServiceContext.Create(null, null, null);

      Assert.That(serviceContext.BusinessObjectIdentifier, Is.Null);
    }

    [Test]
    public void Create_DataSourceSet_BusinessObjectNull_SetsBusinessObjectClass_FromDataSource_BusinessObjectClass ()
    {
      var serviceContext = BusinessObjectWebServiceContext.Create(_dataSourceStub.Object, null, null);

      Assert.That(serviceContext.BusinessObjectClass, Is.EqualTo("DataSourceBusinessObjectClass"));
    }

    [Test]
    public void Create_BusinessObjectNotNull_SetsBusinessObjectClass_FromDataSource_BusinessObject_BusinessObjectClass ()
    {
      _dataSourceStub.SetupProperty(_ => _.BusinessObject);
      _dataSourceStub.Object.BusinessObject = _businessObjectStub.Object;
      var serviceContext = BusinessObjectWebServiceContext.Create(_dataSourceStub.Object, null, null);

      Assert.That(serviceContext.BusinessObjectClass, Is.EqualTo("BusinessObjectBusinessObjectClass"));
    }

    [Test]
    public void Create_PropertyNull_SetsBusinessObjectPropertyNull ()
    {
      var serviceContext = BusinessObjectWebServiceContext.Create(null, null, null);

      Assert.That(serviceContext.BusinessObjectIdentifier, Is.Null);
    }

    [Test]
    public void Create_PropertySet_SetsBusinessObjectProperty ()
    {
      var serviceContext = BusinessObjectWebServiceContext.Create(null, _propertyStub.Object, null);

      Assert.That(serviceContext.BusinessObjectProperty, Is.EqualTo("BusinessObjectProperty"));
    }

    [Test]
    public void Create_BusinessObjectNotNullAndHasNoIdentity_SetsBusinessObjectIdentifierNull ()
    {
      _dataSourceStub.SetupProperty(_ => _.BusinessObject);
      _dataSourceStub.Object.BusinessObject = _businessObjectStub.Object;
      var serviceContext = BusinessObjectWebServiceContext.Create(_dataSourceStub.Object, null, null);

      Assert.That(serviceContext.BusinessObjectIdentifier, Is.Null);
    }

    [Test]
    public void Create_BusinessObjectNotNullAndHasIdentity_SetsBusinessObjectIdentifier ()
    {
      _dataSourceStub.SetupProperty(_ => _.BusinessObject);
      _dataSourceStub.Object.BusinessObject = _businessObjectWithIdentityStub.Object;
      var serviceContext = BusinessObjectWebServiceContext.Create(_dataSourceStub.Object, null, null);

      Assert.That(serviceContext.BusinessObjectIdentifier, Is.EqualTo("BusinessObjectIdentifier"));
    }

    [Test]
    public void Create_ArgsNull_SetsArgsNull ()
    {
      var serviceContext = BusinessObjectWebServiceContext.Create(null, null, null);

      Assert.That(serviceContext.Arguments, Is.Null);
    }

    [Test]
    public void Create_ArgsEmpty_SetsArgsNull ()
    {
      var serviceContext = BusinessObjectWebServiceContext.Create(null, null, "");

      Assert.That(serviceContext.Arguments, Is.Null);
    }

    [Test]
    public void Create_ArgsNotNull_SetsArgs ()
    {
      var serviceContext = BusinessObjectWebServiceContext.Create(null, null, "Args");

      Assert.That(serviceContext.Arguments, Is.EqualTo("Args"));
    }
  }
}
