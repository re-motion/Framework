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
using Remotion.Web.Services;

namespace Remotion.ObjectBinding.Web.Services
{
  /// <summary>
  /// The <see cref="IBusinessObjectIconWebService"/> interface defines the API for retrieving the icon 
  /// for an <see cref="IBusinessObjectWithIdentity"/> via a web-service call.
  /// </summary>
  public interface IBusinessObjectIconWebService
  {
    /// <summary>
    /// Retrieves the icon for the specified <see cref="IBusinessObjectWithIdentity"/>.
    /// </summary>
    /// <param name="businessObjectClass">
    /// The <see cref="IBusinessObjectClass.Identifier"/> of the <see cref="IBusinessObjectClass"/> the control is bound to or <see langword="null" />.
    /// This value is either the <see cref="IBusinessObject.BusinessObjectClass"/> of the bound <see cref="IBusinessObject"/> or the 
    /// <see cref="IBusinessObjectDataSource.BusinessObjectClass"/> of the <see cref="IBusinessObjectDataSource"/>. 
    /// If no <paramref name="businessObjectClass"/> is specified, the method will return <see langword="null"/>.
    /// </param>
    /// <param name="businessObject">
    ///   The <see cref="IBusinessObjectWithIdentity.UniqueIdentifier"/> of the bound <see cref="IBusinessObjectWithIdentity"/> or <see langword="null" />
    ///   if no business object is set, i.e. the control's value is <see langword="null"/>.
    /// </param>
    /// <param name="arguments">Additional arguments required for retrieving the icon.</param>
    /// <returns>An <see cref="IconProxy"/> object or <see langword="null" />.</returns>
    IconProxy GetIcon (string businessObjectClass, string businessObject, string arguments);
  }
}