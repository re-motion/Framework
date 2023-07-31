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
using Remotion.Reflection;

namespace Remotion.Validation.Results
{
  [Obsolete("Use ValidationFailure instead. (Version 5.0.0)", true)]
  public class PropertyValidationFailure : ValidationFailure
  {
    [Obsolete("Use ValidationFailure.ValidatedProperties[i].ValidatedProperty instead. (Version 5.0.0)", true)]
    public IPropertyInformation ValidatedProperty
    {
      get { throw new NotSupportedException("Use ValidationFailure.ValidatedProperties[i].ValidatedProperty instead. (Version 5.0.0)"); }
    }

    [Obsolete("Use ValidationFailure.ValidatedProperties[i].ValidatedPropertyValue instead. (Version 5.0.0)", true)]
    public object? ValidatedPropertyValue
    {
      get { throw new NotSupportedException("Use ValidationFailure.ValidatedProperties[i].ValidatedPropertyValue instead. (Version 5.0.0)"); }
    }

    [Obsolete("Use ValidationFailure.CreatePropertyValidationFailure(object, IPropertyInformation, object?, string, string) instead. (Version 5.0.0)", true)]
    public PropertyValidationFailure (
        [NotNull] object validatedObject,
        [NotNull] IPropertyInformation validatedProperty,
        [CanBeNull] object? validatedPropertyValue,
        [NotNull] string errorMessage,
        [NotNull] string localizedValidationMessage)
        : base(null!, null!, null!, null!)
    {
      throw new NotSupportedException("Use ValidationFailure.CreatePropertyValidationFailure(object, IPropertyInformation, object?, string, string) instead. (Version 5.0.0)");
    }
  }
}
