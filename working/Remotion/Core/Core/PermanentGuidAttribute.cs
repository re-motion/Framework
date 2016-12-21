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

namespace Remotion
{
  /// <summary>
  /// Supplies an identifier that should remain constant even accross refactorings. 
  /// Can be applied to reference types, value types, interfaces, enums, properties, methods, and fields.
  /// </summary>
  [AttributeUsage (
      AttributeTargets.Class
      | AttributeTargets.Struct
      | AttributeTargets.Interface
      | AttributeTargets.Enum
      | AttributeTargets.Property
      | AttributeTargets.Method
      | AttributeTargets.Field,
      AllowMultiple = false,
      Inherited = false)]
  public class PermanentGuidAttribute : Attribute
  {
    private readonly Guid _value;

    /// <summary>
    ///   Initializes a new instance of the <see cref="PermanentGuidAttribute"/> class.
    /// </summary>
    /// <param name="value"> The <see cref="String"/> representation of a <see cref="Guid"/>. </param>
    public PermanentGuidAttribute (string value)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("value", value);

      _value = new Guid (value);
    }

    /// <summary>
    ///   Gets the <see cref="Guid"/> supplied during initialization.
    /// </summary>
    public Guid Value
    {
      get { return _value; }
    }
  }
}