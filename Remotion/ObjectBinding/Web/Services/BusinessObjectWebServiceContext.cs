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
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.Services
{
  /// <summary>
  /// Contains context information required by the web services used for business object controls.
  /// </summary>
  public class BusinessObjectWebServiceContext
  {
    public static BusinessObjectWebServiceContext Create (
        [CanBeNull] IBusinessObjectDataSource dataSource,
        [CanBeNull] IBusinessObjectProperty property,
        [CanBeNull] string arguments)
    {
      return new BusinessObjectWebServiceContext (
          dataSource?.BusinessObject?.BusinessObjectClass?.Identifier ?? dataSource?.BusinessObjectClass?.Identifier,
          property?.Identifier,
          (dataSource?.BusinessObject as IBusinessObjectWithIdentity)?.UniqueIdentifier,
          StringUtility.EmptyToNull (arguments));
    }

    /// <summary>
    /// Gets the <see cref="IBusinessObjectClass.Identifier"/> of the <see cref="IBusinessObjectClass"/> 
    /// the <see cref="BusinessObjectBoundWebControl"/> is bound to.
    /// </summary>
    public string BusinessObjectClass { get; }

    /// <summary>
    /// Gets the <see cref="IBusinessObjectProperty.Identifier"/> of the <see cref="IBusinessObjectReferenceProperty"/> 
    /// the <see cref="BusinessObjectBoundWebControl"/> is bound to.
    /// </summary>
    public string BusinessObjectProperty { get; }

    /// <summary>
    /// Gets the <see cref="IBusinessObjectWithIdentity.UniqueIdentifier"/> of the <see cref="IBusinessObjectWithIdentity"/> 
    /// the <see cref="BusinessObjectBoundWebControl"/> is bound to.
    /// </summary>
    public string BusinessObjectIdentifier { get; }

    /// <summary>
    /// Gets the additional arguments specified.
    /// </summary>
    public string Arguments { get; }

    private BusinessObjectWebServiceContext (
        string businessObjectClass,
        string businessObjectProperty,
        string businessObjectIdentifier,
        string arguments)
    {
      BusinessObjectClass = businessObjectClass;
      BusinessObjectProperty = businessObjectProperty;
      BusinessObjectIdentifier = businessObjectIdentifier;
      Arguments = arguments;
    }
  }
}