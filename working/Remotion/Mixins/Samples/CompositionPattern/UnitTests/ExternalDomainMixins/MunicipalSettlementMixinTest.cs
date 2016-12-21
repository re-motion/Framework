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
using Remotion.Mixins.Samples.CompositionPattern.Core.Domain;
using Remotion.Mixins.Samples.CompositionPattern.Core.ExternalDomainMixins;
using Remotion.Mixins.Samples.CompositionPattern.UnitTests.Domain.Mixins;

namespace Remotion.Mixins.Samples.CompositionPattern.UnitTests.ExternalDomainMixins
{
  [TestFixture]
  public class MunicipalSettlementMixinTest
  {
    private MunicipalSettlementMixin _mixin;
    private ISettlement _targetStub;

    [SetUp]
    public void SetUp ()
    {
      _mixin = MixinInstanceFactory.CreateDomainObjectMixinWithTargetStub<MunicipalSettlementMixin, ISettlement> (out _targetStub);
    }

    [Test]
    public void OnTargetReferenceInitializing ()
    {
      Assert.That (_mixin.MunicipalityID, Is.EqualTo (12));
    }

    [Test]
    public void GetDescriptionForMayors ()
    {
      _targetStub.Title = "Title";
      _targetStub.SettlementKind = "Kind";

      var result = _mixin.GetDescriptionForMayors();

      Assert.That (result, Is.EqualTo ("MunicipalSettlement: Title (Kind), 12"));
    }
  }
}