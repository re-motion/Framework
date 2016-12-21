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
  /// Describes how a mixin influences its target class.
  /// </summary>
  public enum MixinKind
  {
    /// <summary>
    /// The mixin extends the target class from the outside, the target class might not know about being mixed. The mixin therefore has the
    /// possibility to override attributes (with <see cref="AttributeUsageAttribute.AllowMultiple"/> set to false) and interfaces declared
    /// or implemented by the target class.
    /// </summary>
    Extending,
    /// <summary>
    /// The mixin is explicitly used by the target class. The mixin therefore behaves more like a base class, eg. attributes (with 
    /// <see cref="AttributeUsageAttribute.AllowMultiple"/> set to false) and interfaces introduced by the mixin can be overridden by the 
    /// target class.
    /// </summary>
    Used
  }
}
