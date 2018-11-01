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
  /// Provides an entry point for registering and triggering events for an <see cref="IDomainObject"/>. This is not related to the 
  /// mixin-based composition pattern and included for the sake of the example only.
  /// </summary>
  public class DomainObjectEventSource
  {
    private readonly object _sender;

    public event EventHandler Committing;

    public DomainObjectEventSource (object sender)
    {
      _sender = sender;
    }

    public void OnCommitting ()
    {
      var handler = Committing;
      if (handler != null)
        handler (_sender, EventArgs.Empty);
    }
  }
}