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
using FluentValidation.Validators;
using JetBrains.Annotations;

namespace Remotion.Validation.Globalization
{
  public interface IErrorMessageGlobalizationService
  {
    /// <summary>
    /// Returns the error message for a given <see cref="IPropertyValidator"/>.
    /// </summary>
    /// <param name="propertyValidator">The <see cref="IPropertyValidator"/> to get the error message for.</param>
    /// <returns>The error message for the <see cref="IPropertyValidator"/>. Please note that the method can return null if no error message is specified.</returns>
    [CanBeNull]
    string GetErrorMessage (IPropertyValidator propertyValidator);
  }
}