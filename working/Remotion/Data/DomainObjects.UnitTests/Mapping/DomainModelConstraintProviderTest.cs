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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class DomainModelConstraintProviderTest
  {
    private DomainModelConstraintProvider _domainModelConstraintProvider;
    private IPropertyInformation _propertyInformationStub;
    private INullablePropertyAttribute _nullablePropertyAttributeStub;
    private ILengthConstrainedPropertyAttribute _lengthConstraintPropertyAttributeStub;

    [SetUp]
    public void SetUp ()
    {
      _domainModelConstraintProvider = new DomainModelConstraintProvider();
      _propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      _nullablePropertyAttributeStub = MockRepository.GenerateStub<INullablePropertyAttribute>();
      _lengthConstraintPropertyAttributeStub = MockRepository.GenerateStub<ILengthConstrainedPropertyAttribute>();
    }

    [Test]
    public void IsNullable_NoAttribute ()
    {
      _propertyInformationStub.Stub (stub => stub.GetCustomAttribute<INullablePropertyAttribute> (true)).Return (null);

      var result = _domainModelConstraintProvider.IsNullable (_propertyInformationStub);

      Assert.That (result, Is.True);
    }

    [Test]
    public void IsNullable_NullableFromAttribute ()
    {
      _nullablePropertyAttributeStub.Stub (stub => stub.IsNullable).Return (true);
      _propertyInformationStub.Stub (stub => stub.GetCustomAttribute<INullablePropertyAttribute> (true)).Return (_nullablePropertyAttributeStub);

      var result = _domainModelConstraintProvider.IsNullable (_propertyInformationStub);

      Assert.That (result, Is.True);
    }

    [Test]
    public void IsNullable_NotNullableFromAttribute ()
    {
      _nullablePropertyAttributeStub.Stub (stub => stub.IsNullable).Return (false);
      _propertyInformationStub.Stub (stub => stub.GetCustomAttribute<INullablePropertyAttribute> (true)).Return (_nullablePropertyAttributeStub);

      var result = _domainModelConstraintProvider.IsNullable (_propertyInformationStub);

      Assert.That (result, Is.False);
    }

    [Test]
    public void GetMaxLength_NoAttribute ()
    {
      _propertyInformationStub.Stub (stub => stub.GetCustomAttribute<ILengthConstrainedPropertyAttribute> (true)).Return (null);

      var result = _domainModelConstraintProvider.GetMaxLength (_propertyInformationStub);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetMaxLength_MaxLengthFromAttribute ()
    {
      _lengthConstraintPropertyAttributeStub.Stub (stub => stub.MaximumLength).Return (100);
      _propertyInformationStub.Stub (stub => stub.GetCustomAttribute<ILengthConstrainedPropertyAttribute> (true)).Return (_lengthConstraintPropertyAttributeStub);

      var result = _domainModelConstraintProvider.GetMaxLength (_propertyInformationStub);

      Assert.That (result, Is.EqualTo(100));
    }

  }
}