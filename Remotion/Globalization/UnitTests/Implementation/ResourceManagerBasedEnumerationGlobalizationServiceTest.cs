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
using Remotion.Globalization.UnitTests.TestDomain;
using Remotion.Reflection;

namespace Remotion.Globalization.UnitTests.Implementation
{
  [TestFixture]
  public class ResourceManagerBasedEnumerationGlobalizationServiceTest
  {
    private ResourceManagerBasedEnumerationGlobalizationService _service;
    private Mock<IGlobalizationService> _globalizationServiceStub;
    private Mock<IMemberInformationNameResolver> _memberInformationNameResolverStub;

    [SetUp]
    public void SetUp ()
    {
      _globalizationServiceStub = new Mock<IGlobalizationService>();
      _memberInformationNameResolverStub = new Mock<IMemberInformationNameResolver>();
      _service = new ResourceManagerBasedEnumerationGlobalizationService (_globalizationServiceStub.Object, _memberInformationNameResolverStub.Object);
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_WithResourceManager ()
    {
      var resourceManagerStub = new Mock<IResourceManager>();
      var outValue = "expected";
      resourceManagerStub.Setup (_ => _.IsNull).Returns (false);
      resourceManagerStub.Setup (_ => _.TryGetString ("enumName", out outValue)).Returns (true);

      _globalizationServiceStub.Setup (_ => _.GetResourceManager (TypeAdapter.Create (typeof (EnumWithResources)))).Returns (resourceManagerStub.Object);
      _memberInformationNameResolverStub.Setup (_ => _.GetEnumName (EnumWithResources.Value1)).Returns ("enumName");

      string resourceValue;
      Assert.That (_service.TryGetEnumerationValueDisplayName (EnumWithResources.Value1, out resourceValue), Is.True);
      Assert.That (resourceValue, Is.EqualTo ("expected"));
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_WithResourceManager_ResourceIDIsUnknown ()
    {
      var resourceManagerStub = new Mock<IResourceManager>();
      string outValue = null;
      resourceManagerStub.Setup (_ => _.IsNull).Returns (false);
      resourceManagerStub.Setup (_ => _.TryGetString ("enumName", out outValue)).Returns (false);

      _globalizationServiceStub.Setup (_ => _.GetResourceManager (TypeAdapter.Create (typeof (EnumWithResources)))).Returns (resourceManagerStub.Object);
      _memberInformationNameResolverStub.Setup (_ => _.GetEnumName (EnumWithResources.Value1)).Returns ("enumName");

      string resourceValue;
      Assert.That (_service.TryGetEnumerationValueDisplayName (EnumWithResources.Value1, out resourceValue), Is.False);
      Assert.That (resourceValue, Is.Null);
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_WithoutResourceManager_ReturnsNull ()
    {
      _globalizationServiceStub.Setup (_ => _.GetResourceManager (TypeAdapter.Create (typeof (EnumWithResources)))).Returns (NullResourceManager.Instance);

      string resourceValue;
      Assert.That (_service.TryGetEnumerationValueDisplayName (EnumWithResources.Value1, out resourceValue), Is.False);
      Assert.That (resourceValue, Is.Null);
    }
  }
}