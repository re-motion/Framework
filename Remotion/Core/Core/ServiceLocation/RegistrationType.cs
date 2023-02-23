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
namespace Remotion.ServiceLocation
{
  /// <summary>
  /// Defines whether an implementation is registered as a single value or as a sequence, a compound type, or a decorator.
  /// </summary>
  /// <remarks>
  /// When using the <see cref="ServiceLocator"/>, <see cref="Single"/> and <see cref="Compound"/>
  /// registrations must be resolved via <see cref="IServiceLocator.GetInstance(System.Type)"/> and <see cref="Multiple"/> 
  /// registrations must be resolved via <see cref="IServiceLocator.GetAllInstances(System.Type)"/>. Otherwise, an <see cref="ActivationException"/> is thrown.
  /// </remarks>
  public enum RegistrationType
  {
    /// <summary>
    /// Instances registered as <see cref="Single"/> indicate that there is only one instance registered
    /// and the <see cref="Multiple"/> or <see cref="Compound"/> registration types are not allowed.
    /// The <see cref="Single"/> instance can be decorated by instances registered with the <see cref="Decorator"/> registration type.
    /// </summary>
    Single,
    /// <summary>
    /// Instances registered as <see cref="Multiple"/> indicate that there is a sequence of instances registered and the <see cref="Single"/> registration type is not allowed.
    /// <see cref="Multiple"/> instance can be decorated by instances registered with the <see cref="Decorator"/> registration type.
    /// All <see cref="Multiple"/> instances can be aggregated by an instance registered with the <see cref="Compound"/> registration type.
    /// </summary>
    Multiple,
    /// <summary>
    /// Instances registered as <see cref="Compound"/> will contain all <see cref="Multiple"/> registrations via a constructor parameter of type <see cref="IEnumerable{T}"/>.
    /// Note that the <see cref="LifetimeKind"/> of the compound type will override the lifetime of the injected types, same as all other service compositions.
    /// The <see cref="Single"/> registration type is not allowed when using a <see cref="Compound"/> registration.
    /// </summary>
    Compound,
    /// <summary>
    /// Instances registered as <see cref="Decorator"/> will decorate instances of any registration type.
    /// Note that for <see cref="Compound"/> registrations, only the <see cref="Compound"/> type will be decorated, not the contained <see cref="Multiple"/> types.
    /// </summary>
    Decorator
  }
}
