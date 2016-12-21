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
using FluentValidation;

namespace Remotion.Validation
{
  /// <summary>
  /// The <see cref="IValidatorBuilder"/> interface provides an API for creating an <see cref="IValidator"/> for a <see cref="Type"/>.
  /// Use the application's IoC container to retrieve an instance of <see cref="IValidatorBuilder"/>.
  /// </summary>
  public interface IValidatorBuilder
  {
    /// <summary>
    /// Returns a validator for <paramref name="validatedType"/>.
    /// </summary>
    /// <returns>An implementation of <see cref="IValidator"/> which can be used to validate instances of <paramref name="validatedType"/>.</returns>
    IValidator BuildValidator (Type validatedType);
  }
}