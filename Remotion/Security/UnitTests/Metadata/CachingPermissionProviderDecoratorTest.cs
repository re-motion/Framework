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
using Remotion.Reflection;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.SampleDomain;

namespace Remotion.Security.UnitTests.Metadata
{
  [TestFixture]
  public class CachingPermissionProviderDecoratorTest
  {
    private Mock<IPermissionProvider> _innerProviderStub;
    private CachingPermissionProviderDecorator _cacheDecorator;
    private MethodInfoAdapter _methodInformation;
    private Type _type;

    [SetUp]
    public void SetUp ()
    {
      _innerProviderStub = new Mock<IPermissionProvider>();
      _cacheDecorator = new CachingPermissionProviderDecorator (_innerProviderStub.Object);
      _type = typeof (SecurableObject);
      _methodInformation = MethodInfoAdapter.Create (_type.GetMethod ("Save"));
    }

    [Test]
    public void GetRequiredMethodPermissions_WithoutAccessTypes_ReturnsEmptyFromCache ()
    {
      var expected = new Enum[0];
      _innerProviderStub.Setup (_ => _.GetRequiredMethodPermissions (_type, _methodInformation)).Returns (expected);

      var result = _cacheDecorator.GetRequiredMethodPermissions (_type, _methodInformation);

      Assert.That (result, Is.Empty);
      Assert.That (result, Is.Not.SameAs (expected));
      Assert.That (_cacheDecorator.GetRequiredMethodPermissions (_type, _methodInformation), Is.SameAs (result));
    }

    [Test]
    public void GetRequiredMethodPermissions_WithAccessTypes_ReturnsValueCopyFromCache ()
    {
      var expected = new Enum[] { GeneralAccessTypes.Read };
      _innerProviderStub.Setup (_ => _.GetRequiredMethodPermissions (_type, _methodInformation)).Returns (expected);

      var result = _cacheDecorator.GetRequiredMethodPermissions (_type, _methodInformation);

      Assert.That (result, Is.EquivalentTo (expected));
      Assert.That (result, Is.Not.SameAs (expected));
      Assert.That (_cacheDecorator.GetRequiredMethodPermissions (_type, _methodInformation), Is.SameAs (result));
    }

    [Test]
    public void GetRequiredMethodPermissions_WithNullMethod_ReturnsValueCopyFromCache ()
    {
      _innerProviderStub.Setup (_ => _.GetRequiredMethodPermissions (It.IsAny<Type>(), It.IsAny<IMethodInformation>())).Throws (new InvalidOperationException());

      var methodInformation = new NullMethodInformation();

      var result = _cacheDecorator.GetRequiredMethodPermissions (_type, methodInformation);

      Assert.That (result, Is.Empty);
      Assert.That (_cacheDecorator.GetRequiredMethodPermissions (_type, methodInformation), Is.SameAs (result));
    }
  }
}