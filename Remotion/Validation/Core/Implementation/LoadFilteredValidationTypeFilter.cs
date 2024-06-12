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
using System.Diagnostics.CodeAnalysis;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Validation.Implementation
{
  /// <summary>
  /// Implements the <see cref="IValidationTypeFilter"/> interface and filters <see cref="object"/>.
  /// </summary>
  [ImplementationFor(typeof(IValidationTypeFilter), Lifetime = LifetimeKind.Singleton, Position = 0, RegistrationType = RegistrationType.Multiple)]
  public class LoadFilteredValidationTypeFilter : IValidationTypeFilter
  {
    private List<Type> _filterTypes;

    public LoadFilteredValidationTypeFilter ()
    {
      Initialize();
    }

    public bool IsValidatableType (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);

      return !_filterTypes.Contains(type);
    }

    [MemberNotNull(nameof(_filterTypes))]
    private void Initialize ()
    {
      _filterTypes = new List<Type>(
          new[]
          {
              typeof(object)
          });
    }
  }
}
