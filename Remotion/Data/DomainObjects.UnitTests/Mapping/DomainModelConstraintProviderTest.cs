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
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class DomainModelConstraintProviderTest
  {
    private DomainModelConstraintProvider _domainModelConstraintProvider;
    private Mock<IPropertyInformation> _propertyInformationStub;
    private Mock<INullablePropertyAttribute> _nullablePropertyAttributeStub;
    private Mock<ILengthConstrainedPropertyAttribute> _lengthConstraintPropertyAttributeStub;

    [SetUp]
    public void SetUp ()
    {
      _domainModelConstraintProvider = new DomainModelConstraintProvider();
      _propertyInformationStub = new Mock<IPropertyInformation>();
      _nullablePropertyAttributeStub = new Mock<INullablePropertyAttribute>();
      _lengthConstraintPropertyAttributeStub = new Mock<ILengthConstrainedPropertyAttribute>();
    }

    [Test]
    public void IsNullable_NoAttribute ()
    {
      _propertyInformationStub.Setup (stub => stub.GetCustomAttribute<INullablePropertyAttribute> (true)).Returns ((INullablePropertyAttribute) null);

      var result = _domainModelConstraintProvider.IsNullable(_propertyInformationStub.Object);

      Assert.That(result, Is.True);
    }

    [Test]
    public void IsNullable_NullableFromAttribute ()
    {
      _nullablePropertyAttributeStub.Setup (stub => stub.IsNullable).Returns (true);
      _propertyInformationStub.Setup (stub => stub.GetCustomAttribute<INullablePropertyAttribute> (true)).Returns (_nullablePropertyAttributeStub.Object);

      var result = _domainModelConstraintProvider.IsNullable(_propertyInformationStub.Object);

      Assert.That(result, Is.True);
    }

    [Test]
    public void IsNullable_NotNullableFromAttribute ()
    {
      _nullablePropertyAttributeStub.Setup (stub => stub.IsNullable).Returns (false);
      _propertyInformationStub.Setup (stub => stub.GetCustomAttribute<INullablePropertyAttribute> (true)).Returns (_nullablePropertyAttributeStub.Object);

      var result = _domainModelConstraintProvider.IsNullable(_propertyInformationStub.Object);

      Assert.That(result, Is.False);
    }

    [Test]
    public void GetMaxLength_NoAttribute ()
    {
      _propertyInformationStub.Setup (stub => stub.GetCustomAttribute<ILengthConstrainedPropertyAttribute> (true)).Returns ((ILengthConstrainedPropertyAttribute) null);

      var result = _domainModelConstraintProvider.GetMaxLength(_propertyInformationStub.Object);

      Assert.That(result, Is.Null);
    }

    [Test]
    public void GetMaxLength_MaxLengthFromAttribute ()
    {
      _lengthConstraintPropertyAttributeStub.Setup (stub => stub.MaximumLength).Returns (100);
      _propertyInformationStub.Setup (stub => stub.GetCustomAttribute<ILengthConstrainedPropertyAttribute> (true)).Returns (_lengthConstraintPropertyAttributeStub.Object);

      var result = _domainModelConstraintProvider.GetMaxLength(_propertyInformationStub.Object);

      Assert.That(result, Is.EqualTo(100));
    }

  }
}
