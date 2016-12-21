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
using System.Threading;
using Remotion.Context;
using Remotion.Mixins.Context;

namespace Remotion.Mixins.Utilities
{
  /// <summary>
  /// Allows users to specify configuration settings when a mixed type is instantiated.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Usually, the mixin types configured in the <see cref="ClassContext"/> of a target class are simply instantiated when the mixed
  /// instance is constructed. Using this scope class, a user can supply pre-instantiated mixins instead.
  /// </para>
  /// <para>
  /// This is mainly for internal purposes, users should use the <see cref="ObjectFactory"/>
  /// class to instantiate mixed types.
  /// </para>
  /// <para>
  /// This class is a singleton bound to the current <see cref="Thread"/>. It does not use the <see cref="SafeContext"/> for performance reasons.
  /// </para>
  /// </remarks>
  public class MixedObjectInstantiationScope : IDisposable
  {
    [ThreadStatic]
    private static MixedObjectInstantiationScope s_current;

    private static readonly MixedObjectInstantiationScope s_empty = new MixedObjectInstantiationScope (false, new object[0]);

    public static MixedObjectInstantiationScope Current
    {
      get { return s_current ?? s_empty; }
    }

    public static bool HasCurrent
    {
      get { return s_current != null; }
    }

    public static void SetCurrent (MixedObjectInstantiationScope value)
    {
      s_current = value;
    }

    private readonly object[] _suppliedMixinInstances;

    private MixedObjectInstantiationScope _previous;
    private bool _isDisposed = false;

    private MixedObjectInstantiationScope (bool setCurrent, object[] suppliedMixinInstances)
    {
      if (setCurrent)
        StorePreviousAndSetCurrent ();

      _suppliedMixinInstances = suppliedMixinInstances;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MixedObjectInstantiationScope"/> class, setting it as the
    /// <see cref="Current"/> scope object. The previous scope is restored when this scope's <see cref="Dispose"/>
    /// method is called, e.g. from a <c>using</c> statement. The new scope will not contain any pre-created mixin instances.
    /// </summary>
    public MixedObjectInstantiationScope () : this (true, new object[0])
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MixedObjectInstantiationScope"/> class, setting it as the
    /// <see cref="Current"/> scope object. The previous scope is restored when this scope's <see cref="Dispose"/>
    /// method is called, e.g. from a <c>using</c> statement. The new scope contains the specified pre-created mixin instances.
    /// </summary>
    /// <param name="suppliedMixinInstances">The mixin instances to be used when a mixed type is instantiated from within the scope. The objects
    /// specified must fit the mixin types specified in the mixed type's configuration. Users can also specify instances for a subset of the mixin
    /// types, the remaining ones will be created on demand.</param>
    public MixedObjectInstantiationScope (params object[] suppliedMixinInstances) : this (true, suppliedMixinInstances)
    {
    }

    public bool IsDisposed
    {
      get { return _isDisposed; }
    }

    /// <summary>
    /// The mixin instances to be used when a mixed class is instantiated from within the scope.
    /// </summary>
    public object[] SuppliedMixinInstances
    {
      get { return _suppliedMixinInstances; }
    }

    /// <summary>
    /// When called for the first time, restores the <see cref="MixedObjectInstantiationScope"/> that was in effect when this scope was created.
    /// </summary>
    public void Dispose ()
    {
      if (!_isDisposed)
      {
        RestorePrevious ();
        _isDisposed = true;
      }
    }

    private void StorePreviousAndSetCurrent ()
    {
      _previous = s_current;
      s_current = this;
    }

    private void RestorePrevious ()
    {
      s_current = _previous;
    }
  }
}
