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

namespace Remotion.Data.DomainObjects
{
/// <summary>
/// The exception that is thrown when a PropertyValue is set to a value of wrong type.
/// </summary>
public class InvalidTypeException : DomainObjectException
{
  // types

  // static members and constants

  // member fields

  private string _propertyName;
  private Type _expectedType;
  private Type _actualType;

  // construction and disposing

  public InvalidTypeException (string propertyName, Type expectedType, Type actualType) : this(
      string.Format(
          "Actual type '{0}' of property '{1}' does not match expected type '{2}'.",
          actualType, propertyName, expectedType),
      propertyName,
      expectedType,
      actualType)
  {
  }

  public InvalidTypeException (string message, string propertyName, Type expectedType, Type actualType, Exception? innerException = null)
      : base(message, innerException)
  {
    ArgumentUtility.CheckNotNullOrEmpty("propertyName", propertyName);
    ArgumentUtility.CheckNotNull("expectedType", expectedType);
    ArgumentUtility.CheckNotNull("actualType", actualType);

    _propertyName = propertyName;
    _expectedType = expectedType;
    _actualType = actualType;
  }

  // methods and properties

  /// <summary>
  /// The name of the property that caused the exception.
  /// </summary>
  public string PropertyName
  {
    get { return _propertyName; }
  }

  /// <summary>
  /// The type that was expected for the property value.
  /// </summary>
  public Type ExpectedType
  {
    get { return _expectedType; }
  }

  /// <summary>
  /// The type that was provided for the property value.
  /// </summary>
  public Type ActualType
  {
    get { return _actualType; }
  }
}
}
