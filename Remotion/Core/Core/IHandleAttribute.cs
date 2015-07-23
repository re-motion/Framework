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