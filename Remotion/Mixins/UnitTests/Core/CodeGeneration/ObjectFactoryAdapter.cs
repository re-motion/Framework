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
using Remotion.Mixins.CodeGeneration;
using Remotion.TypePipe;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration
{
  /// <summary>
  /// Provides an instantiable wrapper for <see cref="IObjectFactoryImplementation"/> with an API matching <see cref="ObjectFactory"/>.
  /// </summary>
  /// <remarks>
  ///  The <see cref="ObjectFactoryAdapter"/> is needed because the <see cref="CodeGenerationBaseTest"/> reset the current IoC container,
  ///  invalidating the association of <see cref="ObjectFactory"/> and <see cref="IPipelineRegistry"/> as resolved from the IoC container.
  /// </remarks>
  public class ObjectFactoryAdapter
  {
    private readonly IObjectFactoryImplementation _objectFactoryImplementation;

    public ObjectFactoryAdapter (IObjectFactoryImplementation objectFactoryImplementation)
    {
      _objectFactoryImplementation = objectFactoryImplementation;
    }

    public T Create<T> ()
    {
      return Create<T>(ParamList.Empty);
    }

    public T Create<T> (ParamList constructorParameters, params object[] preparedMixins)
    {
      return Create<T>(false, constructorParameters, preparedMixins);
    }

    public object Create (Type targetOrConcreteType)
    {
      return Create(false, targetOrConcreteType, ParamList.Empty);
    }

    public object Create (Type targetOrConcreteType, ParamList constructorParameters, params object[] preparedMixins)
    {
      return Create(false, targetOrConcreteType, constructorParameters, preparedMixins);
    }

    public T Create<T> (bool allowNonPublicConstructors, ParamList constructorParameters, params object[] preparedMixins)
    {
      return (T) Create(allowNonPublicConstructors, typeof(T), constructorParameters, preparedMixins);
    }

    public object Create (
        bool allowNonPublicConstructors,
        Type targetOrConcreteType,
        ParamList constructorParameters,
        params object[] preparedMixins)
    {
      return _objectFactoryImplementation.CreateInstance(
          allowNonPublicConstructors,
          targetOrConcreteType,
          constructorParameters,
          preparedMixins);
    }
  }
}