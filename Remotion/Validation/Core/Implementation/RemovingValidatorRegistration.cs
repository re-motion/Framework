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
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Validation.Implementation
{
  /// <summary>
  /// Represents the information required to remove validators of type <see cref="ValidatorType"/> registered by collector type <see cref="CollectorTypeToRemoveFrom"/>.
  /// </summary>
  public sealed class RemovingValidatorRegistration
  {
    private readonly Type _validatorType;
    private readonly Type? _collectorTypeToRemoveFrom;

    public RemovingValidatorRegistration ([NotNull] Type validatorType, [CanBeNull] Type? collectorTypeToRemoveFrom)
    {
      ArgumentUtility.CheckNotNull("validatorType", validatorType);

      _validatorType = validatorType;
      _collectorTypeToRemoveFrom = collectorTypeToRemoveFrom;
    }

    [NotNull]
    public Type ValidatorType
    {
      get { return _validatorType; }
    }

    [CanBeNull]
    public Type? CollectorTypeToRemoveFrom
    {
      get { return _collectorTypeToRemoveFrom; }
    }
  }
}
