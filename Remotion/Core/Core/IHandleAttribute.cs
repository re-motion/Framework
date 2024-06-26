// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;

namespace Remotion
{
  /// <summary>
  /// Implemented by custom attributes that denote that a type implements the Handle pattern; i.e., its instances represent a pointer to an object 
  /// rather than the object itself.
  /// </summary>
  /// <remarks>
  /// This interface is used by WXE/Security integration in order to allow security evaluation when object handles are passed to a WXE function.
  /// </remarks>
  public interface IHandleAttribute
  {
    /// <summary>
    /// When passed the type to which this <see cref="IHandleAttribute"/> is applied, this method returns the type of objects referenced by the handle.
    /// </summary>
    Type GetReferencedType (Type handleType);

    /// <summary>
    /// When passed an instance of the type to which this <see cref="IHandleAttribute"/> is applied, this method returns the object referenced by 
    /// the handle.
    /// </summary>
    object GetReferencedInstance (object handleInstance);
  }
}
