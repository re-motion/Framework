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
using Remotion.Data.DomainObjects.DataManagement;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Provides data for change events of <see cref="PropertyValue"/> instances.
  /// </summary>
  public class ValueChangeEventArgs : EventArgs
  {
    private readonly object _oldValue;
    private readonly object _newValue;

    /// <summary>
    /// Initializes a new instance of the <b>ValueChangingEventArgs</b>.
    /// </summary>
    /// <param name="oldValue">The old value.</param>
    /// <param name="newValue">The new value.</param>
    public ValueChangeEventArgs (object oldValue, object newValue)
    {
      _oldValue = oldValue;
      _newValue = newValue;
    }

    /// <summary>
    /// Gets the old value.
    /// </summary>
    public object OldValue
    {
      get { return _oldValue; }
    }

    /// <summary>
    /// Gets the new value.
    /// </summary>
    public object NewValue
    {
      get { return _newValue; }
    }
  }
}