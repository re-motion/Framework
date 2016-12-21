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
using System.Linq;

namespace Remotion.Mixins.Samples.CompositionPattern.Core.Framework
{
  /// <summary>
  /// Acts as a convenience base class for domain objects in the mixin-based composition pattern. Implements the <see cref="IDomainObject"/> interface
  /// and provides a <see cref="ComposedObject{TComposedInterface}.This"/> property that provides access to the composed interface.
  /// </summary>
  /// <typeparam name="TComposedInterface">The composed interface of the derived class.</typeparam>
  public abstract class ComposedDomainObject<TComposedInterface> : ComposedObject<TComposedInterface>, IDomainObject
      where TComposedInterface: class, IDomainObject
  {
    private readonly Guid _id;
    private readonly DomainObjectEventSource _events;

    protected ComposedDomainObject ()
    {
      _id = Guid.NewGuid();
      _events = new DomainObjectEventSource (this);

      // ReSharper disable DoNotCallOverridableMethodsInConstructor
      OnReferenceInitializing();
      // ReSharper restore DoNotCallOverridableMethodsInConstructor
    }

    protected virtual void OnReferenceInitializing ()
    {
      var mixinTarget = this as IMixinTarget;
      if (mixinTarget != null)
      {
        foreach (var mixin in mixinTarget.Mixins.OfType<IDomainObjectMixin>())
          mixin.OnTargetReferenceInitializing();
      }
    }

    public Guid ID
    {
      get { return _id; }
    }

    public DomainObjectEventSource Events
    {
      get { return _events; }
    }
  }
}