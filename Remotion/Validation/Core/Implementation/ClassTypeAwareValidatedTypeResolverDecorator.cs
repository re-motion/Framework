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
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Validation.Attributes;

namespace Remotion.Validation.Implementation
{
  /// <summary>
  /// Implements <see cref="IValidatedTypeResolver"/> and resolves the validated Type via the <see cref="ApplyWithClassAttribute"/>.
  /// </summary>
  [ImplementationFor (typeof (IValidatedTypeResolver), Position = 1, RegistrationType = RegistrationType.Decorator)]
  public class ClassTypeAwareValidatedTypeResolverDecorator : IValidatedTypeResolver
  {
    private readonly IValidatedTypeResolver _resolver;
    private readonly GenericTypeAwareValidatedTypeResolverDecorator _genericResolver;

    public ClassTypeAwareValidatedTypeResolverDecorator (IValidatedTypeResolver resolver)
    {
      ArgumentUtility.CheckNotNull ("resolver", resolver);

      _genericResolver = new GenericTypeAwareValidatedTypeResolverDecorator (new NullValidatedTypeResolver());
      _resolver = resolver;
    }

    public IValidatedTypeResolver InnerResolver
    {
      get { return _resolver; }
    }

    public Type GetValidatedType (Type collectorType)
    {
      ArgumentUtility.CheckNotNull ("collectorType", collectorType);

      var attribute = AttributeUtility.GetCustomAttribute<ApplyWithClassAttribute> (collectorType, false);
      if (attribute == null)
        return _resolver.GetValidatedType (collectorType);

      var validatedType = attribute.ClassType;
      var validatedTypefromGeneric = _genericResolver.GetValidatedType (collectorType);
      if (validatedTypefromGeneric != null)
        CheckValidatedTypeAssignableFromDefinedType (collectorType, validatedTypefromGeneric, validatedType);
      return validatedType;
    }

    private void CheckValidatedTypeAssignableFromDefinedType (Type collectorType, Type validatedType, Type classOrMixinType)
    {
      if (!validatedType.IsAssignableFrom (classOrMixinType))
      {
        throw new InvalidOperationException (
            string.Format (
                "Invalid '{0}'-definition for collector '{1}': type '{2}' is not assignable from '{3}'.",
                typeof (ApplyWithClassAttribute).Name,
                collectorType.FullName,
                validatedType.FullName,
                classOrMixinType.FullName));
      }
    }
  }
}