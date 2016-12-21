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
using Remotion.Globalization.ExtensibleEnums.Implementation;
using Remotion.Globalization.ExtensibleEnums.UnitTests.TestDomain;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Globalization.ExtensibleEnums.UnitTests.Implementation
{
  [TestFixture]
  public class ResourceManagerBasedExtensibleEnumGlobalizationServiceTest
  {
    private ResourceManagerBasedExtensibleEnumGlobalizationService _service;
    private IGlobalizationService _globalizationServiceStub;

    [SetUp]
    public void SetUp ()
    {
      _globalizationServiceStub = MockRepository.GenerateStub<IGlobalizationService>();
      _service = new ResourceManagerBasedExtensibleEnumGlobalizationService (_globalizationServiceStub);
    }

    [Test]
    public void TryGetExtensibleEnumValueDisplayName_WithResourceManager_ReturnsLocalizedValue ()
    {
      var resourceManagerStub = MockRepository.GenerateStub<IResourceManager>();
      resourceManagerStub.Stub (_ => _.IsNull).Return (false);
      _globalizationServiceStub
          .Stub (_ => _.GetResourceManager (TypeAdapter.Create (typeof (ColorExtensions))))
          .Return (resourceManagerStub);
      resourceManagerStub
          .Stub (
              _ => _.TryGetString (
                  Arg.Is ("Color.Red"),
                  out Arg<string>.Out ("expected").Dummy))
          .Return (true);

      string resourceValue;
      Assert.That (_service.TryGetExtensibleEnumValueDisplayName (Color.Values.Red(), out resourceValue), Is.True);
      Assert.That (resourceValue, Is.EqualTo ("expected"));
    }

    [Test]
    public void TryGetExtensibleEnumValueDisplayName_WithoutResourceManager_ReturnsFalse ()
    {
      _globalizationServiceStub.Stub (_ => _.GetResourceManager (Arg<ITypeInformation>.Is.NotNull)).Return (NullResourceManager.Instance);

      string resourceValue;
      Assert.That (_service.TryGetExtensibleEnumValueDisplayName (Color.Values.Red(), out resourceValue), Is.False);
      Assert.That (resourceValue, Is.Null);
    }
  }
}