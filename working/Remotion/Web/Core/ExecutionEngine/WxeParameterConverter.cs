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
using System.Globalization;
using Remotion.Collections;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine
{

public class WxeParameterConverter
{
  private WxeParameterDeclaration _parameter;
  private static readonly ITypeConversionProvider s_typeConversionProvider= SafeServiceLocator.Current.GetInstance<ITypeConversionProvider>();

  public WxeParameterConverter (WxeParameterDeclaration parameter)
  {
    ArgumentUtility.CheckNotNull ("parameter", parameter);
    _parameter = parameter;
  }
  
  protected WxeParameterDeclaration Parameter
  {
    get { return _parameter; }
  }

  /// <summary> Converts a parameter's value to its string representation. </summary>
  /// <param name="value"> The value to be converted. Must be of assignable to the <see cref="Type"/>. </param>
  /// <param name="callerVariables"> 
  ///   The optional list of caller variables. Used to dereference a <see cref="WxeVariableReference"/>.
  /// </param>
  /// <returns> 
  ///   A <see cref="string"/> or <see langword="null"/> if the conversion is not possible but the parameter is not
  ///   required.
  /// </returns>
  /// <exception cref="WxeException"> Thrown if the <paramref name="value"/> could not be converted. </exception>
  public string ConvertToString (object value, NameObjectCollection callerVariables)
  {
    CheckForRequiredOutParameter();

    WxeVariableReference varRef = value as WxeVariableReference;
    if (varRef != null)
      return ConvertVarRefToString (varRef, callerVariables);

    return ConvertObjectToString (value);
  }

  /// <summary> Converts a <see cref="WxeVariableReference"/>'s value to its string representation. </summary>
  /// <param name="varRef"> 
  ///   The <see cref="WxeVariableReference"/> to be converted. The referenced value must be of assignable to the 
  ///   <see cref="WxeParameterDeclaration"/>'s <see cref="Type"/>. Must not be <see langword="null"/>.
  /// </param>
  /// <param name="callerVariables">
  ///   The optional list of caller variables. Used to dereference a <see cref="WxeVariableReference"/>.
  /// </param>
  /// <returns> 
  ///   A <see cref="string"/> or <see langword="null"/> if the conversion is not possible but the parameter is not
  ///   required.
  /// </returns>
  /// <exception cref="WxeException"> Thrown if the value referenced by the <paramref name="varRef"/> could not be converted. </exception>
  protected string ConvertVarRefToString (WxeVariableReference varRef, NameObjectCollection callerVariables)
  {
    ArgumentUtility.CheckNotNull ("varRef", varRef);

    if (callerVariables == null)
    {
      if (_parameter.Required)
      {
        throw new WxeException (string.Format (
            "Required IN parameter '{0}' is a Variable Reference but no caller variables have been provided.", 
            _parameter.Name));
      }
      return null;
    }

    object value = callerVariables[_parameter.Name];
    
    if (value is WxeVariableReference)
    {
      if (_parameter.Required)
      {
        throw new WxeException (string.Format (
            "Required IN parameter '{0}' is a Variable Reference but no caller variables have been provided.", 
            _parameter.Name));
      }
      return null;
    }

    return ConvertObjectToString (value);
  }

  /// <summary> Converts a parameter's value to its string representation. </summary>
  /// <param name="value"> The value to be converted. Must be of assignable to the <see cref="Type"/>. </param>
  /// <returns> 
  ///   A <see cref="string"/> or <see langword="null"/> if the conversion is not possible but the parameter is not
  ///   required.
  /// </returns>
  /// <exception cref="WxeException"> Thrown if the <paramref name="value"/> could not be converted. </exception>
  protected string ConvertObjectToString (object value)
  {
    if (value != null && ! _parameter.Type.IsAssignableFrom (value.GetType()))
      throw ArgumentUtility.CreateArgumentTypeException ("value", value.GetType(), _parameter.Type);

    if (! _parameter.Required && value == null)
      return null;

    value = TryConvertObjectToString (value);
    if (value is string)
      return (string) value;

    if (_parameter.Required)
    {
      throw new WxeException (string.Format (
          "Only parameters that can be restored from their string representation may be converted to a string. Parameter: '{0}'.",
          _parameter.Name));
    }
    return null;
  }

  /// <summary> Tries to convert a parameter's value to its string representation. </summary>
  /// <param name="value"> The value to be converted. </param>
  /// <returns> A <see cref="string"/> or the <paramref name="value"/> if the conversion is not possible. </returns>
  protected object TryConvertObjectToString (object value)
  {
    Type sourceType = _parameter.Type;
    Type destinationType = typeof (string);

    //TODO: #if DEBUG
    if (! s_typeConversionProvider.CanConvert (sourceType, destinationType))
      return value;

    return s_typeConversionProvider.Convert (null, CultureInfo.InvariantCulture, sourceType, destinationType, value);
  }

  protected void CheckForRequiredOutParameter ()
  {
    if (_parameter.Required && _parameter.Direction == WxeParameterDirection.Out)
    {
      throw new WxeException (string.Format (
          "Required OUT parameters cannot be converted to a string. Parameter: '{0}'", _parameter.Name));
    }
  }
}

}
