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
using System.Collections.Concurrent;
using System.Threading;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Validation
{
  [ImplementationFor (typeof (IValidatorProvider), Position = Position, RegistrationType = RegistrationType.Single, Lifetime = LifetimeKind.Singleton)]
  public class CachingValidatorProvider : IValidatorProvider
  {
    public const int Position = 0;

    private readonly ConcurrentDictionary<Type, Lazy<IValidator>> _cache = new ConcurrentDictionary<Type, Lazy<IValidator>>();
    private readonly IValidatorBuilder _validatorBuilder;

    public CachingValidatorProvider (IValidatorBuilder validatorBuilder)
    {
      _validatorBuilder = validatorBuilder;
    }

    public IValidator GetValidator (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      var cachedResult = _cache.GetOrAdd (
          type,
          key => new Lazy<IValidator> (() => _validatorBuilder.BuildValidator (key), LazyThreadSafetyMode.ExecutionAndPublication));

      return cachedResult.Value;
    }
  }
}
