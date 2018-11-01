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
using Remotion.Mixins.Samples.CompositionPattern.Core.Domain.Mixins;

namespace Remotion.Mixins.Samples.CompositionPattern.UnitTests.Domain.Mixins
{
  [TestFixture]
  public class DocumentMixinTest
  {
    private DocumentMixin _mixin;
    private ITenantBoundObject _targetStub;

    [SetUp]
    public void SetUp ()
    {
      _mixin = MixinInstanceFactory.CreateDomainObjectMixinWithTargetStub<DocumentMixin, ITenantBoundObject> (out _targetStub);
    }
    
    [Test]
    public void TargetCommitting_ReplacesNullTitle ()
    {
      _targetStub.Tenant = "TheTenant";
      Assert.That (_mixin.Title, Is.Null);

      _mixin.TargetEvents.OnCommitting ();

      Assert.That (_mixin.Title, Is.EqualTo ("(unnamed document of TheTenant)"));
    }

    [Test]
    public void TargetCommitting_ReplacesEmptyTitle ()
    {
      _targetStub.Tenant = "TheTenant";
      _mixin.Title = "";

      _mixin.TargetEvents.OnCommitting ();

      Assert.That (_mixin.Title, Is.EqualTo ("(unnamed document of TheTenant)"));
    }

    [Test]
    public void TargetCommitting_LeavesNonEmptyTitle ()
    {
      _mixin.Title = "Blah";

      _mixin.TargetEvents.OnCommitting ();

      Assert.That (_mixin.Title, Is.EqualTo ("Blah"));
    }
  }
}