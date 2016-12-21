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
using Remotion.Mixins;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Validation.Implementation;

namespace Remotion.Validation.Mixins.Implementation
{
  /// <summary>
  /// Implements the <see cref="IInvolvedTypeProvider"/> interface for mixins.
  /// </summary>
  [ImplementationFor (typeof (IInvolvedTypeProvider), RegistrationType = RegistrationType.Decorator)]
  public class MixedInvolvedTypeProviderDecorator : IInvolvedTypeProvider
  {
    private readonly IInvolvedTypeProvider _involvedTypeProvider;
    private readonly IValidationTypeFilter _validationTypeFilter;

    public MixedInvolvedTypeProviderDecorator (IInvolvedTypeProvider involvedTypeProvider, IValidationTypeFilter validationTypeFilter)
    {
      ArgumentUtility.CheckNotNull ("involvedTypeProvider", involvedTypeProvider);
      ArgumentUtility.CheckNotNull ("validationTypeFilter", validationTypeFilter);

      _involvedTypeProvider = involvedTypeProvider;
      _validationTypeFilter = validationTypeFilter;
    }

    public IInvolvedTypeProvider InvolvedTypeProvider
    {
      get { return _involvedTypeProvider; }
    }

    public IEnumerable<IEnumerable<Type>> GetTypes (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      //Note: if the type is a mixin type then the concrete type is passed to the InvolvedTypeProvider. 
      //That is the reason why no compound implementation is possible!
      var concreteOrMixedType = MixinTypeUtility.GetConcreteMixedType (type); 

      var involvedTypes = _involvedTypeProvider.GetTypes (concreteOrMixedType);
      var involvedMixins = GetMixins (type);
      return involvedTypes.Concat (involvedMixins);
    }

    private IEnumerable<IEnumerable<Type>> GetMixins (Type type)
    {
      return
          MixinTypeUtility.GetMixinTypesExact (type)
              .Where (_validationTypeFilter.IsValidatableType)
              .Select (mixinType => new[] { mixinType })
              .ToArray();
    }
  }
}