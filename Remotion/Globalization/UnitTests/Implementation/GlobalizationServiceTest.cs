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
  public class GlobalizationServiceTest
  {
    private class ResourceTarget
    {
    }

    private GlobalizationService _globalizationService;
    private IResourceManagerResolver _resolverStub;
    private IResourceManager _resourceManagerStub;
    private ResolvedResourceManagerResult _resolvedResourceManagerResult;

    [SetUp]
    public void SetUp ()
    {
      _resourceManagerStub = MockRepository.GenerateStub<IResourceManager>();
      _resolvedResourceManagerResult = ResolvedResourceManagerResult.Create (_resourceManagerStub, NullResourceManager.Instance);

      _resolverStub = MockRepository.GenerateStub<IResourceManagerResolver>();
      _globalizationService = new GlobalizationService (_resolverStub);
    }

    [Test]
    public void GetResourceManager_WithTypeWithoutResources()
    {
      var type = typeof (ResourceTarget);
      var typeInformation = TypeAdapter.Create (type);

      _resourceManagerStub.Stub (stub=>stub.IsNull).Return (true);
      _resolverStub.Stub (stub => stub.Resolve (type)).Return (_resolvedResourceManagerResult);

      var resourceManager = _globalizationService.GetResourceManager (typeInformation);

      string value;
      Assert.That (resourceManager.TryGetString ("property:Value1", out value), Is.False);
      Assert.That (value, Is.Null);
    }

    [Test]
    public void GetResourceManager_WithExpectedType ()
    {
      var type = typeof (ResourceTarget);
      var typeInformation = TypeAdapter.Create (type);

      _resourceManagerStub
          .Stub (stub => stub.TryGetString (Arg.Is ("property:Value1"), out Arg<string>.Out ("TheValue").Dummy))
          .Return (true);
      _resolverStub.Stub (stub => stub.Resolve (type)).Return (_resolvedResourceManagerResult);

      var resourceManager = _globalizationService.GetResourceManager (typeInformation);

      string value;
      Assert.That (resourceManager.TryGetString ("property:Value1", out value), Is.True);
      Assert.That (value, Is.EqualTo ("TheValue"));
    }

    [Test]
    public void GetResourceManager_WithTypeNotSupportingConversionFromITypeInformationToType ()
    {
      var typeInformation = MockRepository.GenerateStub<ITypeInformation>();

      var result = _globalizationService.GetResourceManager (typeInformation);

      _resolverStub.AssertWasNotCalled (stub => stub.Resolve (Arg<Type>.Is.Anything));
      Assert.That (result, Is.EqualTo (NullResourceManager.Instance));
    }

    [Test]
    public void GetResourceManagerTwice_SameFromCache ()
    {
      var type = typeof (ResourceTarget);
      var typeInformation = TypeAdapter.Create (type);

      _resourceManagerStub
          .Stub (stub => stub.TryGetString (Arg.Is ("property:Value1"), out Arg<string>.Out ("TheValue").Dummy))
          .Return (true);

      _resolverStub.Stub (stub => stub.Resolve (type)).Return (_resolvedResourceManagerResult);

      var resourceManager1 = _globalizationService.GetResourceManager (typeInformation);
      var resourceManager2 = _globalizationService.GetResourceManager (typeInformation);

      Assert.That (resourceManager1, Is.SameAs (resourceManager2));
      _resolverStub.AssertWasCalled (stub => stub.Resolve (Arg<Type>.Is.Anything), options=>options.Repeat.Once());
    }
  }
}