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
using Remotion.Mixins.Context.FluentBuilders;

namespace Remotion.Mixins
{
  /// <summary>
  /// Defines <typeparamref name="TInterface"/> as a composed interface for the target class implementing the 
  /// <see cref="IHasComposedInterface{TInterface}"/> interface.
  /// </summary>
  /// <typeparam name="TInterface">The composed interface type.</typeparam>
  /// <remarks>
  /// <para>
  /// Composed interfaces are interfaces that comprise members implemented on a target class as well as members added by mixins to that target class.
  /// They can be used to access all members on a mixed instance without casting to mixin interfaces.
  /// </para>
  /// <para>
  /// For more information, see <see cref="ComposedInterfaceAttribute"/>. Implementing <see cref="IHasComposedInterface{TInterface}"/> on a target 
  /// class has the same effect as marking the respective <typeparamref name="TInterface"/> with the <see cref="ComposedInterfaceAttribute"/>.
  /// </para>
  /// <para>
  /// <see cref="IHasComposedInterface{TInterface}"/> is automatically taken into account when the declarative mixin configuration is analyzed.
  /// When building a mixin configuration using the fluent mixin building APIs (<see cref="MixinConfiguration.BuildNew()"/> and 
  /// similar), it is not automatically taken into account. Register the interface by hand using 
  /// <see cref="ClassContextBuilder.AddComposedInterface{TInterface}"/>.
  /// </para>
  /// </remarks>
// ReSharper disable UnusedTypeParameter - the type parameter is analyzed via Reflection.
  public interface IHasComposedInterface<TInterface> where TInterface : class
// ReSharper restore UnusedTypeParameter
  {
  }
}