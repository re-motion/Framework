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
  /// Indicates that an ordinal name comparison can be used to determine the order between two mixins when these override the same methods
  /// on a target object. Only use this if you don't care about the actual mixin order.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Ordering between mixins is important when two mixins override the same methods on a target object, because without a defined ordering,
  /// it wouldn't be deterministic which of the overrides would be executed first. Usually, orderings between mixins are expressed via dependencies.
  /// Either implicitly, because the mixin has a base call dependency (second type argument of the <see cref="Mixin{TTarget,TNext}"/> base class) to 
  /// an interface implemented by another mixin, or explicitly via  <see cref="MixinRelationshipAttribute.AdditionalDependencies"/>.
  /// </para>
  /// <para>
  /// In some situations, however, a mixin cannot and does not need to specify a specific ordering simply because any override call order would
  /// be sufficient for its purpose. Such a mixin can opt into alphabetic ordering by having this attribute applied to it. Alphabetic ordering is
  /// only applied after all implicit or explicit dependencies have been analyzed. It is also ruled out if two or more of the mixins involved
  /// do not accept alphabetic ordering.
  /// </para>
  /// </remarks>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class AcceptsAlphabeticOrderingAttribute : Attribute
  {
  }
}
