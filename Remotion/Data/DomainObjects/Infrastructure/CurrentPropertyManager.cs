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
using System.Collections.Generic;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Manages a stack of property names per thread.
  /// </summary>
  public static class CurrentPropertyManager
  {
    [ThreadStatic]
    private static Stack<string> _currentPropertyNames;

    private static Stack<string> CurrentPropertyNames
    {
      get
      {
        if (_currentPropertyNames == null)
          _currentPropertyNames = new Stack<string>();
        return _currentPropertyNames;
      }
    }

    /// <summary>
    /// Returns the property name last put on this thread's stack, or null if the stack is empty.
    /// </summary>
    public static string CurrentPropertyName
    {
      get
      {
        if (CurrentPropertyNames.Count == 0)
          return null;
        else
          return _currentPropertyNames.Peek();
      }
    }

    /// <summary>
    /// Retrieves the current property name and throws an exception if there is no property name on this thread's property name stack.
    /// </summary>
    /// <returns>The current property name.</returns>
    /// <remarks>Retrieves the current property name previously initialized via. Domain objects created with 
    /// interception support automatically initialize their virtual properties without needing any further work.</remarks>
    /// <exception cref="InvalidOperationException">There is no current property or it hasn't been properly initialized.</exception>
    public static string GetAndCheckCurrentPropertyName ()
    {
      string propertyName = CurrentPropertyName;
      if (propertyName == null)
      {
        throw new InvalidOperationException (
            "There is no current property or it hasn't been properly initialized. Is the surrounding property virtual?");
      }
      else
        return propertyName;
    }

    /// <summary>
    /// Prepares access to the <see cref="PropertyValue"/> of the given name by pushing its name on top of the current thread's stack of property 
    /// names.
    /// </summary>
    /// <param name="propertyName">The name of the property to be accessed.</param>
    /// <remarks>This method prepares the given property for access via <see cref="DomainObject.CurrentProperty"/>.
    /// It is automatically invoked for virtual properties in domain objects created with interception support and thus doesn't
    /// have to be called manually for these objects. If you choose to invoke <see cref="PreparePropertyAccess"/> and
    /// <see cref="PropertyAccessFinished"/> yourself, be sure to finish the property access with exactly one call to 
    /// <see cref="PropertyAccessFinished"/> from a finally-block.</remarks>
    /// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    ///   <paramref name="propertyName"/> is an empty string.<br /> -or- <br />
    ///   The <paramref name="propertyName"/> parameter does not denote a valid property.
    /// </exception>
    public static void PreparePropertyAccess (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      CurrentPropertyNames.Push (propertyName);
    }

    /// <summary>
    /// Indicates that access to the property of the given name is finished by removing it from the current thread's stack of property names.
    /// </summary>
    /// <remarks>This method must be executed after a property previously prepared via <see cref="PreparePropertyAccess"/> has been accessed as needed.
    /// It is automatically invoked for virtual properties in domain objects created with interception suppport and thus doesn't
    /// have to be called manually for these objects. If you choose to invoke <see cref="PreparePropertyAccess"/> and
    /// <see cref="PropertyAccessFinished"/> yourself, be sure to invoke this method in a finally-block in order to guarantee its execution.</remarks>
    /// <exception cref="InvalidOperationException">There is no property to be finished. There is likely a mismatched number of calls to
    /// <see cref="PreparePropertyAccess"/> and <see cref="PropertyAccessFinished"/>.</exception>
    public static void PropertyAccessFinished ()
    {
      if (CurrentPropertyNames.Count == 0)
        throw new InvalidOperationException ("There is no property to finish.");
      CurrentPropertyNames.Pop();
    }
  }
}
