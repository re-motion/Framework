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
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration
{
  /// <summary>
  /// Added to a member introduced by a mixin on the generated concrete type.
  /// </summary>
  [AttributeUsage (AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event, AllowMultiple = false, Inherited = true)]
  public class IntroducedMemberAttribute : Attribute
  {
    public IntroducedMemberAttribute (Type mixin, string mixinMemberName, Type introducedInterface, string interfaceMemberName)
    {
      ArgumentUtility.CheckNotNull ("mixin", mixin);
      ArgumentUtility.CheckNotNullOrEmpty ("mixinMemberName", mixinMemberName);
      ArgumentUtility.CheckNotNull ("introducedInterface", introducedInterface);
      ArgumentUtility.CheckNotNullOrEmpty ("interfaceMemberName", interfaceMemberName);

      Mixin = mixin;
      MixinMemberName = mixinMemberName;
      IntroducedInterface = introducedInterface;
      InterfaceMemberName = interfaceMemberName;
    }

    /// <summary>
    /// Gets the mixin that added the member.
    /// </summary>
    /// <value>The mixin that added the member.</value>
    public Type Mixin { get; private set; }

    /// <summary>
    /// Gets the name of the member as it is defined by the <see cref="Mixin"/>.
    /// </summary>
    /// <value>The name of the member on the <see cref="Mixin"/>.</value>
    public string MixinMemberName { get; private set; }

    /// <summary>
    /// Gets the interface defining the introduced member.
    /// </summary>
    /// <value>The interface defining the member.</value>
    public Type IntroducedInterface { get; private set; }

    /// <summary>
    /// Gets the name of the member as it is defined by the <see cref="IntroducedInterface"/>.
    /// </summary>
    /// <value>The name of the member on the <see cref="IntroducedInterface"/>.</value>
    public string InterfaceMemberName { get; private set; }
  }
}
