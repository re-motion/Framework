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
using Remotion.Globalization.Implementation;
using Remotion.Reflection;

namespace Remotion.Globalization.UnitTests.Implementation
{
  [TestFixture]
  public class ResourceManagerBasedMemberInformationGlobalizationServiceTest
  {
    private Mock<IGlobalizationService> _globalizationServiceMock;
    private ResourceManagerBasedMemberInformationGlobalizationService _service;
    private Mock<ITypeInformation> _typeInformationForResourceResolutionStub;
    private Mock<ITypeInformation> _typeInformationStub;
    private Mock<IPropertyInformation> _propertyInformationStub;
    private Mock<IResourceManager> _resourceManagerMock;
    private Mock<IMemberInformationNameResolver> _memberInformationNameResolverStub;
    private string _shortPropertyResourceID;
    private string _longPropertyResourceID;
    private string _shortTypeResourceID;
    private string _longTypeResourceID;

    [SetUp]
    public void SetUp ()
    {
      _globalizationServiceMock = new Mock<IGlobalizationService>();
      _resourceManagerMock = new Mock<IResourceManager> (MockBehavior.Strict);
      _resourceManagerMock.Setup (stub => stub.IsNull).Returns (false);
      _resourceManagerMock.Setup (stub => stub.Name).Returns ("RM1");

      _typeInformationStub = new Mock<ITypeInformation>();
      _typeInformationStub.Setup (stub => stub.Name).Returns ("TypeName");

      _typeInformationForResourceResolutionStub = new Mock<ITypeInformation>();
      _typeInformationForResourceResolutionStub.Setup (stub => stub.Name).Returns ("TypeNameForResourceResolution");

      _propertyInformationStub = new Mock<IPropertyInformation>();
      _propertyInformationStub.Setup (stub => stub.Name).Returns ("PropertyName");

      _memberInformationNameResolverStub = new Mock<IMemberInformationNameResolver>();
      _memberInformationNameResolverStub.Setup (stub => stub.GetPropertyName (_propertyInformationStub.Object)).Returns ("FakePropertyFullName");
      _memberInformationNameResolverStub.Setup (stub => stub.GetTypeName (_typeInformationStub.Object)).Returns ("FakeTypeFullName");

      _shortPropertyResourceID = "property:PropertyName";
      _longPropertyResourceID = "property:FakePropertyFullName";
      _shortTypeResourceID = "type:TypeName";
      _longTypeResourceID = "type:FakeTypeFullName";

      _service = new ResourceManagerBasedMemberInformationGlobalizationService (_globalizationServiceMock.Object, _memberInformationNameResolverStub.Object);
    }

    [Test]
    public void TryGetPropertyDisplayName_NoResourceFound ()
    {
      string outValue = null;

      _globalizationServiceMock.Setup (mock => mock.GetResourceManager (_typeInformationStub.Object)).Returns (_resourceManagerMock.Object);

      _resourceManagerMock.Setup (mock => mock.TryGetString (_longPropertyResourceID, out outValue)).Returns (false).Verifiable();
      _resourceManagerMock.Setup (mock => mock.TryGetString (_shortPropertyResourceID, out outValue)).Returns (false).Verifiable();

      string resourceValue;
      var result = _service.TryGetPropertyDisplayName (_propertyInformationStub.Object, _typeInformationStub.Object, out resourceValue);

      _globalizationServiceMock.Verify();
      _resourceManagerMock.Verify();
      Assert.That (result, Is.False);
      Assert.That (resourceValue, Is.Null);
    }

    [Test]
    public void TryGetPropertyDisplayName_ResourceFoundByLongResourceID ()
    {
      var outValue = "Test";

      _globalizationServiceMock.Setup (mock => mock.GetResourceManager (_typeInformationStub.Object)).Returns (_resourceManagerMock.Object);

      _resourceManagerMock.Setup (mock => mock.TryGetString (_longPropertyResourceID, out outValue)).Returns (true).Verifiable();

      string resourceValue;
      var result = _service.TryGetPropertyDisplayName (_propertyInformationStub.Object, _typeInformationStub.Object, out resourceValue);

      _globalizationServiceMock.Verify();
      _resourceManagerMock.Verify();
      Assert.That (result, Is.True);
      Assert.That (resourceValue, Is.EqualTo ("Test"));
    }

    [Test]
    public void TryGetPropertyDisplayName_ResourceFoundByShortResourceID ()
    {
      string nullOutValue = null;
      var testOutValue = "Test";

      _globalizationServiceMock.Setup (mock => mock.GetResourceManager (_typeInformationStub.Object)).Returns (_resourceManagerMock.Object);

      _resourceManagerMock.Setup (mock => mock.TryGetString (_longPropertyResourceID, out nullOutValue)).Returns (false).Verifiable();
      _resourceManagerMock.Setup (mock => mock.TryGetString (_shortPropertyResourceID, out testOutValue)).Returns (true).Verifiable();

      string resourceValue;
      var result = _service.TryGetPropertyDisplayName (_propertyInformationStub.Object, _typeInformationStub.Object, out resourceValue);

      _globalizationServiceMock.Verify();
      _resourceManagerMock.Verify();
      Assert.That (result, Is.True);
      Assert.That (resourceValue, Is.EqualTo ("Test"));
    }

    [Test]
    public void TryGetPropertyDisplayName_ResourceFoundByLong_DoesNotUseShortResourceID ()
    {
      var testLongOutValue = "TestLong";
      var testShortOutValue = "TestShort";

      _globalizationServiceMock.Setup (mock => mock.GetResourceManager (_typeInformationStub.Object)).Returns (_resourceManagerMock.Object);

      _resourceManagerMock.Setup (mock => mock.TryGetString (_longPropertyResourceID, out testLongOutValue)).Returns (true);
      _resourceManagerMock.Setup (mock => mock.TryGetString (_shortPropertyResourceID, out testShortOutValue)).Verifiable();

      string resourceValue;
      var result = _service.TryGetPropertyDisplayName (_propertyInformationStub.Object, _typeInformationStub.Object, out resourceValue);

      _resourceManagerMock.Verify (mock => mock.TryGetString (_longPropertyResourceID, out testLongOutValue), Times.AtLeastOnce);
      _resourceManagerMock.Verify (mock => mock.TryGetString (_shortPropertyResourceID, out testShortOutValue), Times.Never());
      Assert.That (result, Is.True);
      Assert.That (resourceValue, Is.EqualTo ("TestLong"));
    }

    [Test]
    public void TryGetTypeDisplayName_NoResourceFound ()
    {
      string outValue = null;

      _globalizationServiceMock.Setup (mock => mock.GetResourceManager (_typeInformationForResourceResolutionStub.Object)).Returns (_resourceManagerMock.Object);

      _resourceManagerMock.Setup (mock => mock.TryGetString (_longTypeResourceID, out outValue)).Returns (false).Verifiable();
      _resourceManagerMock.Setup (mock => mock.TryGetString (_shortTypeResourceID, out outValue)).Returns (false).Verifiable();

      string resourceValue;
      var result = _service.TryGetTypeDisplayName (_typeInformationStub.Object, _typeInformationForResourceResolutionStub.Object, out resourceValue);

      _resourceManagerMock.Verify();
      Assert.That (result, Is.False);
      Assert.That (resourceValue, Is.Null);
    }

    [Test]
    public void TryGetTypeDisplayName_ResourceFoundByLongResourceID ()
    {
      var outValue = "Test";

      _globalizationServiceMock.Setup (mock => mock.GetResourceManager (_typeInformationForResourceResolutionStub.Object)).Returns (_resourceManagerMock.Object);

      _resourceManagerMock.Setup (mock => mock.TryGetString (_longTypeResourceID, out outValue)).Returns (true).Verifiable();

      string resourceValue;
      var result = _service.TryGetTypeDisplayName (_typeInformationStub.Object, _typeInformationForResourceResolutionStub.Object, out resourceValue);

      _resourceManagerMock.Verify();
      Assert.That (result, Is.True);
      Assert.That (resourceValue, Is.EqualTo ("Test"));
    }

    [Test]
    public void TryGetTypeDisplayName_ResourceFoundByShortResourceID ()
    {
      string nullOutValue = null;
      var testOutValue = "Test";

      _globalizationServiceMock.Setup (mock => mock.GetResourceManager (_typeInformationForResourceResolutionStub.Object)).Returns (_resourceManagerMock.Object);

      _resourceManagerMock.Setup (mock => mock.TryGetString (_longTypeResourceID, out nullOutValue)).Returns (false).Verifiable();
      _resourceManagerMock.Setup (mock => mock.TryGetString (_shortTypeResourceID, out testOutValue)).Returns (true).Verifiable();

      string resourceValue;
      var result = _service.TryGetTypeDisplayName (_typeInformationStub.Object, _typeInformationForResourceResolutionStub.Object, out resourceValue);

      _resourceManagerMock.Verify();
      Assert.That (result, Is.True);
      Assert.That (resourceValue, Is.EqualTo ("Test"));
    }

    [Test]
    public void TryGetTypeDisplayName_ResourceFoundByLong_DoesNotUseShortResourceID ()
    {
      var testLongOutValue = "TestLong";
      var testShortOutValue = "TestShort";

      _globalizationServiceMock.Setup (mock => mock.GetResourceManager (_typeInformationForResourceResolutionStub.Object)).Returns (_resourceManagerMock.Object);

      _resourceManagerMock.Setup (mock => mock.TryGetString (_longTypeResourceID, out testLongOutValue)).Returns (true).Verifiable();
      _resourceManagerMock.Setup (stub => stub.TryGetString (_shortTypeResourceID, out testShortOutValue)).Verifiable();

      string resourceValue;
      var result = _service.TryGetTypeDisplayName (_typeInformationStub.Object, _typeInformationForResourceResolutionStub.Object, out resourceValue);

      _resourceManagerMock.Verify (stub => stub.TryGetString (_longTypeResourceID, out testLongOutValue), Times.AtLeastOnce);
      _resourceManagerMock.Verify (stub => stub.TryGetString (_shortTypeResourceID, out testShortOutValue), Times.Never());
      Assert.That (result, Is.True);
      Assert.That (resourceValue, Is.EqualTo ("TestLong"));
    }
  }
}