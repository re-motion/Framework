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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.DomainImplementation.Transport
{
  /// <summary>
  /// Decorates an <see cref="IDisposable"/> and adds a post-<see cref="IDisposable.Dispose"/> <see cref="Action"/>.
  /// </summary>
  public class PostActionDisposableDecorator : IDisposable
  {
    private readonly IDisposable _inner;
    private readonly Action _postDisposeAction;
    private bool _isDisposed = false;

    public PostActionDisposableDecorator (IDisposable inner, Action postDisposeAction)
    {
      ArgumentUtility.CheckNotNull ("inner", inner);

      _inner = inner;
      _postDisposeAction = postDisposeAction;
    }

    public void Dispose ()
    {
      try
      {
        _inner.Dispose();
      }
      finally
      {
        if (!_isDisposed)
        {
          _postDisposeAction();
          _isDisposed = true;
        }
      }
    }
  }
}