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
using Remotion.Globalization.Implementation;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Globalization.UnitTests.Implementation
{
  [TestFixture]
  public class ResourceManagerBasedMemberInformationGlobalizationServiceTest
  {
    private IGlobalizationService _globalizationServiceMock;
    private ResourceManagerBasedMemberInformationGlobalizationService _service;
    private ITypeInformation _typeInformationForResourceResolutionStub;
    private ITypeInformation _typeInformationStub;
    private IPropertyInformation _propertyInformationStub;
    private IResourceManager _resourceManagerMock;
    private IMemberInformationNameResolver _memberInformationNameResolverStub;
    private string _shortPropertyResourceID;
    private string _longPropertyResourceID;
    private string _shortTypeResourceID;
    private string _longTypeResourceID;

    [SetUp]
    public void SetUp ()
    {
      _globalizationServiceMock = MockRepository.GenerateStub<IGlobalizationService>();
      _resourceManagerMock = MockRepository.GenerateStrictMock<IResourceManager>();
      _resourceManagerMock.Stub (stub => stub.IsNull).Return (false);
      _resourceManagerMock.Stub (stub => stub.Name).Return ("RM1");

      _typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      _typeInformationStub.Stub (stub => stub.Name).Return ("TypeName");

      _typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();
      _typeInformationForResourceResolutionStub.Stub (stub => stub.Name).Return ("TypeNameForResourceResolution");

      _propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      _propertyInformationStub.Stub (stub => stub.Name).Return ("PropertyName");

      _memberInformationNameResolverStub = MockRepository.GenerateStub<IMemberInformationNameResolver>();
      _memberInformationNameResolverStub.Stub (stub => stub.GetPropertyName (_propertyInformationStub)).Return ("FakePropertyFullName");
      _memberInformationNameResolverStub.Stub (stub => stub.GetTypeName (_typeInformationStub)).Return ("FakeTypeFullName");

      _shortPropertyResourceID = "property:PropertyName";
      _longPropertyResourceID = "property:FakePropertyFullName";
      _shortTypeResourceID = "type:TypeName";
      _longTypeResourceID = "type:FakeTypeFullName";

      _service = new ResourceManagerBasedMemberInformationGlobalizationService (_globalizationServiceMock, _memberInformationNameResolverStub);
    }

    [Test]
    public void TryGetPropertyDisplayName_NoResourceFound ()
    {
      _globalizationServiceMock.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManagerMock);

      _resourceManagerMock.Expect (mock => mock.TryGetString (Arg.Is (_longPropertyResourceID), out Arg<string>.Out (null).Dummy)).Return (false);
      _resourceManagerMock.Expect (mock => mock.TryGetString (Arg.Is (_shortPropertyResourceID), out Arg<string>.Out (null).Dummy)).Return (false);

      string resourceValue;
      var result = _service.TryGetPropertyDisplayName (_propertyInformationStub, _typeInformationStub, out resourceValue);

      _globalizationServiceMock.VerifyAllExpectations();
      _resourceManagerMock.VerifyAllExpectations();
      Assert.That (result, Is.False);
      Assert.That (resourceValue, Is.Null);
    }

    [Test]
    public void TryGetPropertyDisplayName_ResourceFoundByLongResourceID ()
    {
      _globalizationServiceMock.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManagerMock);

      _resourceManagerMock.Expect (mock => mock.TryGetString (Arg.Is (_longPropertyResourceID), out Arg<string>.Out ("Test").Dummy)).Return (true);

      string resourceValue;
      var result = _service.TryGetPropertyDisplayName (_propertyInformationStub, _typeInformationStub, out resourceValue);

      _globalizationServiceMock.VerifyAllExpectations();
      _resourceManagerMock.VerifyAllExpectations();
      Assert.That (result, Is.True);
      Assert.That (resourceValue, Is.EqualTo ("Test"));
    }

    [Test]
    public void TryGetPropertyDisplayName_ResourceFoundByShortResourceID ()
    {
      _globalizationServiceMock.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManagerMock);

      _resourceManagerMock.Expect (mock => mock.TryGetString (Arg.Is (_longPropertyResourceID), out Arg<string>.Out (null).Dummy)).Return (false);
      _resourceManagerMock.Expect (mock => mock.TryGetString (Arg.Is (_shortPropertyResourceID), out Arg<string>.Out ("Test").Dummy)).Return (true);

      string resourceValue;
      var result = _service.TryGetPropertyDisplayName (_propertyInformationStub, _typeInformationStub, out resourceValue);

      _globalizationServiceMock.VerifyAllExpectations();
      _resourceManagerMock.VerifyAllExpectations();
      Assert.That (result, Is.True);
      Assert.That (resourceValue, Is.EqualTo ("Test"));
    }

    [Test]
    public void TryGetPropertyDisplayName_ResourceFoundByLong_DoesNotUseShortResourceID ()
    {
      _globalizationServiceMock.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManagerMock);

      _resourceManagerMock.Expect (mock => mock.TryGetString (Arg.Is (_longPropertyResourceID), out Arg<string>.Out ("TestLong").Dummy)).Return (true);
      _resourceManagerMock.Expect (mock => mock.TryGetString (Arg.Is (_shortPropertyResourceID), out Arg<string>.Out ("TestShort").Dummy)).Repeat.Never();

      string resourceValue;
      var result = _service.TryGetPropertyDisplayName (_propertyInformationStub, _typeInformationStub, out resourceValue);

      _globalizationServiceMock.VerifyAllExpectations();
      _resourceManagerMock.VerifyAllExpectations();
      Assert.That (result, Is.True);
      Assert.That (resourceValue, Is.EqualTo ("TestLong"));
    }

    [Test]
    public void TryGetTypeDisplayName_NoResourceFound ()
    {
      _globalizationServiceMock.Expect (mock => mock.GetResourceManager (_typeInformationForResourceResolutionStub)).Return (_resourceManagerMock);

      _resourceManagerMock.Expect (mock => mock.TryGetString (Arg.Is (_longTypeResourceID), out Arg<string>.Out (null).Dummy)).Return (false);
      _resourceManagerMock.Expect (mock => mock.TryGetString (Arg.Is (_shortTypeResourceID), out Arg<string>.Out (null).Dummy)).Return (false);

      string resourceValue;
      var result = _service.TryGetTypeDisplayName (_typeInformationStub, _typeInformationForResourceResolutionStub, out resourceValue);

      _globalizationServiceMock.VerifyAllExpectations();
      _resourceManagerMock.VerifyAllExpectations();
      Assert.That (result, Is.False);
      Assert.That (resourceValue, Is.Null);
    }

    [Test]
    public void TryGetTypeDisplayName_ResourceFoundByLongResourceID ()
    {
      _globalizationServiceMock.Expect (mock => mock.GetResourceManager (_typeInformationForResourceResolutionStub)).Return (_resourceManagerMock);

      _resourceManagerMock.Expect (mock => mock.TryGetString (Arg.Is (_longTypeResourceID), out Arg<string>.Out ("Test").Dummy)).Return (true);

      string resourceValue;
      var result = _service.TryGetTypeDisplayName (_typeInformationStub, _typeInformationForResourceResolutionStub, out resourceValue);

      _globalizationServiceMock.VerifyAllExpectations();
      _resourceManagerMock.VerifyAllExpectations();
      Assert.That (result, Is.True);
      Assert.That (resourceValue, Is.EqualTo ("Test"));
    }

    [Test]
    public void TryGetTypeDisplayName_ResourceFoundByShortResourceID ()
    {
      _globalizationServiceMock.Expect (mock => mock.GetResourceManager (_typeInformationForResourceResolutionStub)).Return (_resourceManagerMock);

      _resourceManagerMock.Expect (mock => mock.TryGetString (Arg.Is (_longTypeResourceID), out Arg<string>.Out (null).Dummy)).Return (false);
      _resourceManagerMock.Expect (mock => mock.TryGetString (Arg.Is (_shortTypeResourceID), out Arg<string>.Out ("Test").Dummy)).Return (true);

      string resourceValue;
      var result = _service.TryGetTypeDisplayName (_typeInformationStub, _typeInformationForResourceResolutionStub, out resourceValue);

      _globalizationServiceMock.VerifyAllExpectations();
      _resourceManagerMock.VerifyAllExpectations();
      Assert.That (result, Is.True);
      Assert.That (resourceValue, Is.EqualTo ("Test"));
    }

    [Test]
    public void TryGetTypeDisplayName_ResourceFoundByLong_DoesNotUseShortResourceID ()
    {
      _globalizationServiceMock.Expect (mock => mock.GetResourceManager (_typeInformationForResourceResolutionStub)).Return (_resourceManagerMock);

      _resourceManagerMock.Expect (mock => mock.TryGetString (Arg.Is (_longTypeResourceID), out Arg<string>.Out ("TestLong").Dummy)).Return (true);
      _resourceManagerMock.Expect (stub => stub.TryGetString (Arg.Is (_shortTypeResourceID), out Arg<string>.Out ("TestShort").Dummy)).Repeat.Never();

      string resourceValue;
      var result = _service.TryGetTypeDisplayName (_typeInformationStub, _typeInformationForResourceResolutionStub, out resourceValue);

      _globalizationServiceMock.VerifyAllExpectations();
      _resourceManagerMock.VerifyAllExpectations();
      Assert.That (result, Is.True);
      Assert.That (resourceValue, Is.EqualTo ("TestLong"));
    }
  }
}