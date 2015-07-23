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
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Remotion.Utilities;

namespace Remotion.Validation.Implementation
{
  /// <summary>
  /// Implements the <see cref="IValidatorBuilder"/> interface as a composite of one or more builders.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  public class CompoundValidatorBuilder : IValidatorBuilder
  {
    private readonly IReadOnlyCollection<IValidatorBuilder> _builders;

    public CompoundValidatorBuilder (IEnumerable<IValidatorBuilder> builders)
    {
      ArgumentUtility.CheckNotNull ("builders", builders);
      
      _builders = builders.ToList().AsReadOnly();
    }

    public IReadOnlyCollection<IValidatorBuilder> Builders
    {
      get { return _builders; }
    }

    public IValidator BuildValidator (Type validatedType)
    {
      ArgumentUtility.CheckNotNull ("validatedType", validatedType);

      var validators = _builders.Select (b => b.BuildValidator (validatedType));
      return new CompoundValidator (validators, validatedType);
    }
  }
}