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

namespace Remotion
{
  // TODO: Remove finalizer, check derivations whether they really need a finalizer (unmanaged resources only); document the change
  /// <summary>
  /// This class can be used as a base class for any class that needs to implement <see cref="IDisposable"/>, but does not want to 
  /// expose a public <c>Dispose</c> method.
  /// <see cref="DisposableBase"/>
  /// </summary>
  public abstract class DisposableExplicitBase : IDisposable
  {
    private bool _disposed = false;

    protected abstract void Dispose (bool disposing);

    void IDisposable.Dispose ()
    {
      Dispose();
    }

    protected void Dispose ()
    {
      if (! _disposed)
      {
        Dispose(true);
        GC.SuppressFinalize(this);
        _disposed = true;
      }
    }

    ~DisposableExplicitBase()
    {
      Dispose(false);
    }

    protected bool Disposed
    {
      get { return _disposed; }
    }

    protected void AssertNotDisposed ()
    {
      if (_disposed)
        throw new InvalidOperationException("Object disposed.");
    }

    protected void Resurrect ()
    {
      if (_disposed)
      {
        _disposed = false;
        GC.ReRegisterForFinalize(this);
      }
    }
  }

  /// <summary>
  /// This class can be used as a base class for any class that needs to implement <see cref="IDisposable"/>.
  /// <see cref="DisposableExplicitBase"/>
  /// </summary>
  public abstract class DisposableBase: DisposableExplicitBase
  {
    public new void Dispose ()
    {
      base.Dispose();
    }
  }

}
