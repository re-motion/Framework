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
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class BindableObjectWithIdentityBaseTest
  {
    private ClassDerivedFromBindableObjectWithIdentityBase _instance;
    private Mock<IBindableObjectWithIdentityBaseImplementation> _implementationMock;
    private Mock<IBusinessObjectProperty> _propertyFake;
    private Mock<IBusinessObjectClass> _businessObjectClassFake;

    [SetUp]
    public void SetUp ()
    {
      _implementationMock = new Mock<IBindableObjectWithIdentityBaseImplementation>();
      _instance = new ClassDerivedFromBindableObjectWithIdentityBase(_implementationMock.Object);

      _propertyFake = new Mock<IBusinessObjectProperty>();
      _businessObjectClassFake = new Mock<IBusinessObjectClass>();
    }

    [Test]
    public void BindableObjectProviderAttribute ()
    {
      Assert.That(typeof(BindableObjectWithIdentityBase).IsDefined(typeof(BindableObjectWithIdentityProviderAttribute), false), Is.True);
    }

    [Test]
    public void BindableObjectBaseClassAttribute ()
    {
      Assert.That(typeof(BindableObjectWithIdentityBase).IsDefined(typeof(BindableObjectBaseClassAttribute), false), Is.True);
    }

    [Test]
    public void CreateImplementation ()
    {
      var instance = new ClassDerivedFromBindableObjectWithIdentityBase();
      Assert.That(PrivateInvoke.GetNonPublicField(instance, "_implementation"), Is.InstanceOf(typeof(BindableObjectWithIdentityBaseImplementation)));
    }

    [Test]
    public void Implementation_IsInitialized ()
    {
      var instance = new ClassDerivedFromBindableObjectWithIdentityBase();
      var mixin = (BindableObjectWithIdentityBaseImplementation)PrivateInvoke.GetNonPublicField(instance, "_implementation");
      Assert.That(mixin.BusinessObjectClass, Is.Not.Null);
    }

    [Test]
    public void GetProperty ()
    {
      _implementationMock.Setup(mock => mock.GetProperty(_propertyFake.Object)).Returns(12).Verifiable();

      Assert.That(_instance.GetProperty(_propertyFake.Object), Is.EqualTo(12));
      _implementationMock.Verify();
    }

    [Test]
    public void SetProperty ()
    {
      _implementationMock.Setup(mock => mock.SetProperty(_propertyFake.Object, 174)).Verifiable();

      _instance.SetProperty(_propertyFake.Object, 174);
      _implementationMock.Verify();
    }

    [Test]
    public void GetPropertyString ()
    {
      _implementationMock.Setup(mock => mock.GetPropertyString(_propertyFake.Object, "gj")).Returns("yay").Verifiable();

      Assert.That(_instance.GetPropertyString(_propertyFake.Object, "gj"), Is.EqualTo("yay"));
      _implementationMock.Verify();
    }

    [Test]
    public void DisplayName ()
    {
      _implementationMock.Setup(mock => mock.BaseDisplayName).Returns("Philips").Verifiable();

      Assert.That(_instance.DisplayName, Is.EqualTo("Philips"));
      _implementationMock.Verify();
    }

    [Test]
    public void BusinessObjectClass ()
    {
      _implementationMock.Setup(mock => mock.BusinessObjectClass).Returns(_businessObjectClassFake.Object).Verifiable();

      Assert.That(_instance.BusinessObjectClass, Is.SameAs(_businessObjectClassFake.Object));
      _implementationMock.Verify();
    }

    [Test]
    public void UniqueIdentifier ()
    {
      _instance.SetUniqueIdentifier("uniqueID");
      Assert.That(_instance.UniqueIdentifier, Is.EqualTo("uniqueID"));
    }
  }
}
