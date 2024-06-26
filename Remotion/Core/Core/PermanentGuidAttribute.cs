// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using Remotion.Utilities;

namespace Remotion
{
  /// <summary>
  /// Supplies an identifier that should remain constant even accross refactorings. 
  /// Can be applied to reference types, value types, interfaces, enums, properties, methods, and fields.
  /// </summary>
  [AttributeUsage(
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
      ArgumentUtility.CheckNotNullOrEmpty("value", value);

      _value = new Guid(value);
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
