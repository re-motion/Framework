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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Provides data for a <b>PropertyChanging</b> event.
  /// </summary>
  public class PropertyChangeEventArgs : ValueChangeEventArgs
  {
    private readonly PropertyDefinition _propertyDefinition;

    /// <summary>
    /// Initializes a new instance of the <b>ValueChangingEventArgs</b> class.
    /// </summary>
    /// <param name="propertyDefinition"></param>
    /// <param name="oldValue">The old value.</param>
    /// <param name="newValue">The new value.</param>
    public PropertyChangeEventArgs (PropertyDefinition propertyDefinition, object oldValue, object newValue)
        : base (oldValue, newValue)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      _propertyDefinition = propertyDefinition;
    }

    /// <summary>
    /// Gets the <see cref="DataManagement.PropertyValue"/> object that is being changed.
    /// </summary>
    public PropertyDefinition PropertyDefinition
    {
      get { return _propertyDefinition; }
    }
  }
}