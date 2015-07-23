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

namespace Remotion.Mixins
{
  /// <summary>
  /// Indicates that a generic parameter of a mixin should be bound to the mixin's target type (unless the generic parameter type is explicitly
  /// specified when the mixin is configured).
  /// </summary>
  /// <remarks>
  /// <para>
  /// Apply this attribute to a generic parameter of a generic mixin when the mixin engine should be able to automatically close the mixin type.
  /// Without the attribute, an exception will be thrown.
  /// </para>
  /// <para>
  /// For example, consider the following code:
  /// <code>
  /// public class TargetClass&lt;T&gt; { }
  /// 
  /// [Extends (typeof (TargetClass&lt;&gt;))]
  /// public class MyMixin&lt;T&gt; { }
  /// </code>
  /// To bind <c>T</c> to <c>TargetClass&lt;T&gt;</c>, use a binding specification:
  /// <code>
  /// public class TargetClass&lt;T&gt; { }
  /// 
  /// [Extends (typeof (TargetClass&lt;&gt;))]
  /// public class MyMixin&lt;[BindToTargetType] T&gt; { }
  /// </code>
  /// </para>
  /// <note type="inotes">When a type parameter is reused for the generic parameter of the <see cref="Mixin{TTarget,TNext}"/>
  /// or <see cref="Mixin{TTarget}"/> base classes, the type parameter must satisfy several constraints. See <see cref="Mixin{TTarget,TNext}"/> and
  /// <see cref="Mixin{TTarget}"/> for more information.</note>
  /// </remarks>
  /// <seealso cref="BindToConstraintsAttribute"/>
  /// <seealso cref="BindToGenericTargetParameterAttribute"/>
  [AttributeUsage (AttributeTargets.GenericParameter, Inherited = false)]
  public class BindToTargetTypeAttribute : Attribute
  {
  }
}
