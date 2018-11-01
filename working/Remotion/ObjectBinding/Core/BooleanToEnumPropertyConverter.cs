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

namespace Remotion.ObjectBinding
{
  public class BooleanEnumerationValueInfo : IEnumerationValueInfo
  {
    private bool _value;
    private IBusinessObjectBooleanProperty _property;

    public BooleanEnumerationValueInfo (bool value, IBusinessObjectBooleanProperty property)
    {
      _value = value;
      _property = property;
    }

    public string DisplayName
    {
      get { return _property.GetDisplayName (_value); }
    }

    public string Identifier
    {
      get { return _value.ToString(); }
    }

    public object Value
    {
      get { return _value; }
    }

    public bool IsEnabled
    {
      get { return true; }
    }
  }

  /// <summary>
  ///   Provides implementations for <see cref="IBusinessObjectEnumerationProperty"/> methods that can be used by 
  ///   implementations of <see cref="IBusinessObjectBooleanProperty"/>.
  /// </summary>
  public class BooleanToEnumPropertyConverter
  {
    private readonly IEnumerationValueInfo _enumInfoTrue;
    private readonly IEnumerationValueInfo _enumInfoFalse;

    public BooleanToEnumPropertyConverter (IBusinessObjectBooleanProperty property)
    {
      _enumInfoTrue = new BooleanEnumerationValueInfo (true, property);
      _enumInfoFalse = new BooleanEnumerationValueInfo (false, property);
    }

    /// <summary>Returns the <see cref="IEnumerationValueInfo"/> objects for <see langword="true"/> and <see langword="false"/>.</summary>
    /// <returns> An array of <see cref="IEnumerationValueInfo"/> objects. </returns>
    public IEnumerationValueInfo[] GetValues ()
    {
      return new IEnumerationValueInfo[] {_enumInfoTrue, _enumInfoFalse};
    }

    /// <summary>
    ///   Returns an <see cref="IEnumerationValueInfo"/> if <paramref name="value"/> is <see langword="true"/> or
    ///   <see langword="false"/> and <see langword="null"/> if <paramref name="value"/> is <see langword="null"/>.
    /// </summary>
    /// <param name="value">Can be any object that equals to <see langword="true"/> or <see langword="false"/> and <see langword="null"/>.</param>
    /// <returns> An <see cref="IEnumerationValueInfo"/> or <see langword="null"/>. </returns>
    public IEnumerationValueInfo GetValueInfoByValue (object value)
    {
      if (value == null)
        return null;
      else if (value.Equals (true))
        return _enumInfoTrue;
      else if (value.Equals (false))
        return _enumInfoFalse;
      else
        throw new ArgumentOutOfRangeException ("value");
    }

    /// <summary>
    ///   Returns an <see cref="IEnumerationValueInfo"/> matching the <c>True</c> or <c>False</c> strings or 
    ///   <see langword="null"/> for an empty or null string.
    /// </summary>
    /// <param name="identifier"> Can be <see langword="true"/>, <see langword="false"/>, or an empty string or <see langword="null"/>. </param>
    /// <returns> An <see cref="IEnumerationValueInfo"/> or <see langword="null"/>. </returns>
    public IEnumerationValueInfo GetValueInfoByIdentifier (string identifier)
    {
      if (string.IsNullOrEmpty (identifier))
        return null;
      else if (identifier == _enumInfoTrue.Identifier)
        return _enumInfoTrue;
      else if (identifier == _enumInfoFalse.Identifier)
        return _enumInfoFalse;
      else
        throw new ArgumentOutOfRangeException ("identifier");
    }
  }
}
