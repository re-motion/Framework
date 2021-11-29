﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Validation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Validation
{
  public static class BusinessObjectValidationResultExtensions
  {
    public static IEnumerable<BusinessObjectValidationFailure> GetValidationFailures (
        [NotNull] this IBusinessObjectValidationResult validationResult,
        [NotNull] IBusinessObjectBoundEditableWebControl control,
        bool markAsHandled = true)
    {
      ArgumentUtility.CheckNotNull("validationResult", validationResult);
      ArgumentUtility.CheckNotNull("control", control);

      if (!control.HasValidBinding)
        return Enumerable.Empty<BusinessObjectValidationFailure>();

      var businessObject = control.DataSource?.BusinessObject;
      if (businessObject == null)
        return Enumerable.Empty<BusinessObjectValidationFailure>();

      var businessObjectProperty = control.Property;
      if (businessObjectProperty == null)
        return Enumerable.Empty<BusinessObjectValidationFailure>();

      return validationResult.GetValidationFailures(businessObject, businessObjectProperty, markAsHandled);
    }
  }
}