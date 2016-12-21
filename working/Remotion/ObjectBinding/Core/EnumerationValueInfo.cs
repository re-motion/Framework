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

namespace Remotion.ObjectBinding
{
  /// <summary> Default implementation of the <see cref="IEnumerationValueInfo"/> interface. </summary>
  public class EnumerationValueInfo : IEnumerationValueInfo
  {
    private readonly object _value;
    private readonly string _identifier;
    private readonly string _displayName;
    private readonly bool _isEnabled;

    /// <summary> Initializes a new instance of the <b>EnumerationValueInfo</b> type. </summary>
    public EnumerationValueInfo (object value, string identifier, string displayName, bool isEnabled)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      ArgumentUtility.CheckNotNullOrEmpty ("identifier", identifier);

      _value = value;
      _identifier = identifier;
      _displayName = displayName;
      _isEnabled = isEnabled;
    }

    /// <summary> Gets the object representing the original value, e.g. a System.Enum type. </summary>
    public object Value
    {
      get { return _value; }
    }

    /// <summary> Gets the string presented to the user. </summary>
    public string Identifier
    {
      get { return _identifier; }
    }

    /// <summary> Gets the string presented to the user. </summary>
    public virtual string DisplayName
    {
      get { return _displayName; }
    }

    /// <summary>
    ///   Gets a flag indicating whether this value should be presented as an option to the user. 
    ///   (If not, existing objects might still use this value.)
    /// </summary>
    public bool IsEnabled
    {
      get { return _isEnabled; }
    }
  }
}
