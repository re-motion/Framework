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
using Remotion.Mixins.Samples.CompositionPattern.Core.Framework;

namespace Remotion.Mixins.Samples.CompositionPattern.Core.Domain.Mixins
{
  /// <summary>
  /// Implements the <see cref="ITenantBoundObject"/> interface as a mixin agnostic of its specific target type. Requires the target type to (directly
  /// or indirectly) implement <see cref="ITenantBoundObject"/>. Attaches itself to the <see cref="DomainObjectEventSource.Committing"/> event
  /// of the target object in order to implement business logic validation to be executed before the commit.
  /// </summary>
  public class TenantBoundMixin : DomainObjectMixin<IDomainObject>, ITenantBoundObject
  {
    public string Tenant { get; set; }

    protected override void OnTargetReferenceInitializing ()
    {
      base.OnTargetReferenceInitializing ();
      TargetEvents.Committing += Target_Committing;
    }

    private void Target_Committing (object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty (Tenant))
        throw new InvalidOperationException ("Cannot commit tenant-bound object " + TargetID + " without a tenant.");
    }
  }
}