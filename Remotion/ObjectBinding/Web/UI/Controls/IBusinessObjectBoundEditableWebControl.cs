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
using Remotion.ObjectBinding.BusinessObjectPropertyConstraints;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary>
  ///   Extends an <see cref="IBusinessObjectBoundWebControl"/> with functionality for validating the control's 
  ///   <see cref="IBusinessObjectBoundControl.Value"/> and writing it back into the bound <see cref="IBusinessObject"/>.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     See <see cref="IBusinessObjectBoundEditableControl.SaveValue"/> for a description of the data binding 
  ///     process.
  ///   </para><para>
  ///     See <see cref="BusinessObjectBoundEditableWebControl"/> for the <see langword="abstract"/> default 
  ///     implementation.
  ///   </para>
  /// </remarks>
  /// <seealso cref="IBusinessObjectBoundWebControl"/>
  /// <seealso cref="IBusinessObjectBoundEditableControl"/>
  /// <seealso cref="IValidatableControl"/>
  /// <seealso cref="IBusinessObjectDataSourceControl"/>
  public interface IBusinessObjectBoundEditableWebControl
      :
          IBusinessObjectBoundWebControl,
          IBusinessObjectBoundEditableControl,
          IValidatableControl,
          IEditableControl
  {
    /// <summary>
    /// Gets a flag if the business object control should generate validators that go beyond the validation of the .NET data type itself.
    /// </summary>
    bool AreOptionalValidatorsEnabled { get; }

    /// <summary>
    /// Gets or sets a flag that indicates if the <see cref="IBusinessObjectBoundWebControl"/> is required via the <see cref="BusinessObjectPropertyValueRequiredConstraint"/>.
    /// </summary>
    bool? RequiredByPropertyConstraint { get; set; }
  }
}
