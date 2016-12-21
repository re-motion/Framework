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
  /// <summary>
  ///   The <see cref="IBusinessObjectWithIdentity"/> interface provides functionality to uniquely identify a business object 
  ///   within its business object domain.
  /// </summary>
  /// <remarks>
  ///   With the help of the <see cref="IBusinessObjectWithIdentity"/> interface it is possible to persist and later restore 
  ///   a reference to the business object. 
  /// </remarks>
  public interface IBusinessObjectWithIdentity : IBusinessObject
  {
    /// <summary> Gets the programmatic <b>ID</b> of this <see cref="IBusinessObjectWithIdentity"/> </summary>
    /// <value> A <see cref="string"/> uniquely identifying this object. </value>
    /// <remarks> This value must be be unqiue within its business object domain. </remarks>
    string UniqueIdentifier { get; }

  /// <summary> Gets the human readable representation of this <see cref="IBusinessObjectWithIdentity"/>. </summary>
  /// <value> A <see cref="String"/> identifying this object to the user. </value>
  /// <remarks> This value does not have to be unqiue within its business object domain. </remarks>
  string DisplayName { get; }
  }
}
