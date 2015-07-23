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

namespace Remotion.Mixins.Samples.CompositionPattern.Core.Framework
{
  /// <summary>
  /// Provides a base class for mixins extending domain objects. Provides a default implementation of <see cref="IDomainObject"/> that simply
  /// delegates to the mixin's target <see cref="IDomainObject"/>. This default implementation is useful because domain object mixins will
  /// usually introduce interfaces extending <see cref="IDomainObject"/>. Also provides infrastructure members useful to domain object mixins.
  /// </summary>
  /// <typeparam name="TTarget">The minimum type of the mixin's target class. This must at least implement <see cref="IDomainObject"/>.</typeparam>
  [NonIntroduced (typeof (IDomainObjectMixin))]
  public abstract class DomainObjectMixin<TTarget> : Mixin<TTarget>, IDomainObject, IDomainObjectMixin
      where TTarget : class, IDomainObject
  {
    public Guid TargetID
    {
      get { return Target.ID; }
    }

    public DomainObjectEventSource TargetEvents
    {
      get { return Target.Events; }
    }

    protected virtual void OnTargetReferenceInitializing()
    {
    }

    Guid IDomainObject.ID
    {
      get { return TargetID; }
    }

    DomainObjectEventSource IDomainObject.Events
    {
      get { return TargetEvents; }
    }

    void IDomainObjectMixin.OnTargetReferenceInitializing ()
    {
      OnTargetReferenceInitializing ();
    }
  }
}