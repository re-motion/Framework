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
using Remotion.Mixins.Samples.CompositionPattern.Core.Domain.Mixins;
using Remotion.Mixins.Samples.CompositionPattern.Core.Framework;

namespace Remotion.Mixins.Samples.CompositionPattern.UnitTests.Domain.Mixins
{
  [TestFixture]
  public class TenantBoundMixinTest
  {
    private TenantBoundMixin _mixin;
    private IDomainObject _targetStub;

    [SetUp]
    public void SetUp ()
    {
      _mixin = MixinInstanceFactory.CreateDomainObjectMixinWithTargetStub<TenantBoundMixin, IDomainObject> (out _targetStub);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "Cannot commit tenant-bound object 00000000-0000-0000-0000-000000000000 without a tenant.")]
    public void TargetCommitting_ThrowsOnNullTenant ()
    {
      Assert.That (_mixin.Tenant, Is.Null);

      _mixin.TargetEvents.OnCommitting ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot commit tenant-bound object 00000000-0000-0000-0000-000000000000 without a tenant.")]
    public void TargetCommitting_ThrowsOnEmptyTenant ()
    {
      _mixin.Tenant = "";

      _mixin.TargetEvents.OnCommitting ();
    }

    [Test]
    public void TargetCommitting_LeavesNonEmptyTenant ()
    {
      _mixin.Tenant = "Blah";

      _mixin.TargetEvents.OnCommitting ();

      Assert.That (_mixin.Tenant, Is.EqualTo ("Blah"));
    }
  }
}