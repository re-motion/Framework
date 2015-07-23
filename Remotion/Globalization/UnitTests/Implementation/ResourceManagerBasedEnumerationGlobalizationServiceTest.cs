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
using Remotion.Globalization.UnitTests.TestDomain;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Globalization.UnitTests.Implementation
{
  [TestFixture]
  public class ResourceManagerBasedEnumerationGlobalizationServiceTest
  {
    private ResourceManagerBasedEnumerationGlobalizationService _service;
    private IGlobalizationService _globalizationServiceStub;
    private IMemberInformationNameResolver _memberInformationNameResolverStub;

    [SetUp]
    public void SetUp ()
    {
      _globalizationServiceStub = MockRepository.GenerateStub<IGlobalizationService>();
      _memberInformationNameResolverStub = MockRepository.GenerateStub<IMemberInformationNameResolver>();
      _service = new ResourceManagerBasedEnumerationGlobalizationService (_globalizationServiceStub, _memberInformationNameResolverStub);
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_WithResourceManager ()
    {
      var resourceManagerStub = MockRepository.GenerateStub<IResourceManager>();
      resourceManagerStub.Stub (_ => _.IsNull).Return (false);
      resourceManagerStub.Stub (_ => _.TryGetString (Arg.Is ("enumName"), out Arg<string>.Out ("expected").Dummy)).Return (true);

      _globalizationServiceStub.Stub (_ => _.GetResourceManager (TypeAdapter.Create (typeof (EnumWithResources)))).Return (resourceManagerStub);
      _memberInformationNameResolverStub.Stub (_ => _.GetEnumName (EnumWithResources.Value1)).Return ("enumName");

      string resourceValue;
      Assert.That (_service.TryGetEnumerationValueDisplayName (EnumWithResources.Value1, out resourceValue), Is.True);
      Assert.That (resourceValue, Is.EqualTo ("expected"));
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_WithResourceManager_ResourceIDIsUnknown ()
    {
      var resourceManagerStub = MockRepository.GenerateStub<IResourceManager>();
      resourceManagerStub.Stub (_ => _.IsNull).Return (false);
      resourceManagerStub.Stub (_ => _.TryGetString (Arg.Is ("enumName"), out Arg<string>.Out (null).Dummy)).Return (false);

      _globalizationServiceStub.Stub (_ => _.GetResourceManager (TypeAdapter.Create (typeof (EnumWithResources)))).Return (resourceManagerStub);
      _memberInformationNameResolverStub.Stub (_ => _.GetEnumName (EnumWithResources.Value1)).Return ("enumName");

      string resourceValue;
      Assert.That (_service.TryGetEnumerationValueDisplayName (EnumWithResources.Value1, out resourceValue), Is.False);
      Assert.That (resourceValue, Is.Null);
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_WithoutResourceManager_ReturnsNull ()
    {
      _globalizationServiceStub.Stub (_ => _.GetResourceManager (TypeAdapter.Create (typeof (EnumWithResources)))).Return (NullResourceManager.Instance);

      string resourceValue;
      Assert.That (_service.TryGetEnumerationValueDisplayName (EnumWithResources.Value1, out resourceValue), Is.False);
      Assert.That (resourceValue, Is.Null);
    }
  }
}