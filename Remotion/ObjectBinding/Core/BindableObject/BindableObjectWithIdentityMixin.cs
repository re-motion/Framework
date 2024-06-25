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
using System.Diagnostics;
using Remotion.Mixins;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// Apply this mixin to a type in order to add an <see cref="IBusinessObjectWithIdentity"/> implementation.
  /// </summary>
  [BindableObjectWithIdentityProvider]
  [CopyCustomAttributes(typeof(DebuggerDisplay))]
  public abstract class BindableObjectWithIdentityMixin : BindableObjectMixinBase<object>, IBusinessObjectWithIdentity
  {
    [DebuggerDisplay("{UniqueIdentifier} ({((Remotion.Mixins.IMixinTarget)this).ClassContext.Type.FullName})")]
    private class DebuggerDisplay // the attributes of this class are copied to the target class
    {
    }

    public BindableObjectWithIdentityMixin ()
    {
    }

    public abstract string UniqueIdentifier { get; }

    /// <summary> Gets the human readable representation of this <see cref="IBusinessObjectWithIdentity"/>. </summary>
    /// <value> The default implementation returns the <see cref="BindableObjectMixinBase{T}.BusinessObjectClass"/>'s <see cref="IBusinessObjectClass.Identifier"/>. </value>
    public virtual string DisplayName
    {
      get { return BusinessObjectClass.Identifier; }
    }

    protected override Type GetTypeForBindableObjectClass ()
    {
      return MixinTypeUtility.GetUnderlyingTargetType(Target.GetType());
    }
  }
}
