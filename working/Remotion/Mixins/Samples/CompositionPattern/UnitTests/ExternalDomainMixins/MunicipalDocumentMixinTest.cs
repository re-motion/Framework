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
  public class MunicipalDocumentMixinTest
  {
    private MunicipalDocumentMixin _mixin;
    private ITarget _targetStub;

    public interface ITarget : ITenantBoundObject, IMunicipalSettlement
    {
    }

    [SetUp]
    public void SetUp ()
    {
      _mixin = MixinInstanceFactory.CreateDomainObjectMixinWithTargetStub<MunicipalDocumentMixin, ITenantBoundObject, ITarget> (out _targetStub);
    }

    [Test]
    public void Title_Set_IncludesMunicipalityID ()
    {
      Assert.That (_mixin.Title, Is.Null);

      _targetStub.MunicipalityID = 13;
      _mixin.Title = "Test";

      Assert.That (_mixin.Title, Is.EqualTo ("Test (for municipality 13)"));
    }
    
    [Test]
    public void TargetCommitting_ReplacesNullTitle ()
    {
      _targetStub.Tenant = "TheTenant";
      _targetStub.MunicipalityID = 13;
      Assert.That (_mixin.Title, Is.Null);

      _mixin.TargetEvents.OnCommitting ();

      Assert.That (_mixin.Title, Is.EqualTo ("(unnamed document of TheTenant) (for municipality 13)"));
    }

    [Test]
    public void TargetCommitting_LeavesNonEmptyTitle ()
    {
      _targetStub.MunicipalityID = 13;
      _mixin.Title = "Blah";

      _mixin.TargetEvents.OnCommitting ();

      Assert.That (_mixin.Title, Is.EqualTo ("Blah (for municipality 13)"));
    }
  }
}