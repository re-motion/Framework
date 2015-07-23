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
  /// When applied to an interface member, defines the visibility of that member when it is introduced into a mixed type. This overrides the value
  /// specified via <see cref="MixinRelationshipAttribute.IntroducedMemberVisibility"/>.
  /// </summary>
  [AttributeUsage (AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Event, AllowMultiple = false, Inherited = false)]
  public class MemberVisibilityAttribute : Attribute
  {
    private readonly MemberVisibility _visibility;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemberVisibilityAttribute"/> class.
    /// </summary>
    /// <param name="visibility">The visibility to be used for the attributed member.</param>
    public MemberVisibilityAttribute (MemberVisibility visibility)
    {
      _visibility = visibility;
    }

    /// <summary>
    /// Gets the visibility set to be used for the attributed member.
    /// </summary>
    /// <value>The visibility to be used.</value>
    public MemberVisibility Visibility
    {
      get { return _visibility; }
    }
  }
}
