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
using System.Reflection;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  [AttributeUsage (AttributeTargets.Property, AllowMultiple = false)]
  public class WxeParameterAttribute : Attribute
  {
    public static WxeParameterAttribute GetAttribute (PropertyInfo property)
    {
      ArgumentUtility.CheckNotNull ("property", property);

      WxeParameterAttribute attribute = AttributeUtility.GetCustomAttribute<WxeParameterAttribute> (property, false);
      if (attribute == null)
        return null;

      if (!property.IsOriginalDeclaration())
      {
        throw new WxeException (
            string.Format (
                "Property '{0}', overridden by '{1}', has a WxeParameterAttribute applied. The WxeParameterAttribute may only be applied to the original declaration of a property.",
                property.Name,
                property.DeclaringType));
      }

      if (!attribute._required.HasValue)
        attribute._required = property.PropertyType.IsValueType;
      return attribute;
    }

    private readonly int _index;
    private bool? _required;
    private readonly WxeParameterDirection _direction;

    public WxeParameterAttribute (int index, WxeParameterDirection direction)
        : this (index, null, direction)
    {
    }

    public WxeParameterAttribute (int index, bool required)
        : this (index, required, WxeParameterDirection.In )
    {
    }

    public WxeParameterAttribute (int index)
        : this (index, null, WxeParameterDirection.In)
    {
    }

    /// <summary>
    /// Declares a property as WXE function parameter.
    /// </summary>
    /// <param name="index"> Index of the parameter within the function's parameter list. </param>
    /// <param name="required"> Speficies whether this parameter must be specified (an not 
    ///     be <see langword="null"/>). Default is <see langword="true"/> for value types
    ///     and <see langword="false"/> for reference types. </param>
    /// <param name="direction"> Declares the parameter as input or output parameter, or both. </param>
    public WxeParameterAttribute (int index , bool required, WxeParameterDirection direction)
        : this (index, (bool?) required, direction)
    {
    }

    private WxeParameterAttribute (int index, bool? required, WxeParameterDirection direction)
    {
      _index = index;
      _required = required;
      _direction = direction;
    }

    public int Index
    {
      get { return _index; }
    }

    public bool Required
    {
      get { return _required.Value; }
    }

    public WxeParameterDirection Direction
    {
      get { return _direction; }
    }
  }
}
