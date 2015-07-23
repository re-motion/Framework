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
using Remotion.Utilities;

namespace Remotion.Mixins
{
  public partial class MixinConfiguration
  {
    /// <summary>
    /// Returns a <see cref="MixinConfigurationBuilder"/> object to build a new <see cref="MixinConfiguration"/>.
    /// </summary>
    /// <returns>A <see cref="MixinConfigurationBuilder"/> for building a new <see cref="MixinConfiguration"/> with a fluent interface.</returns>
    /// <remarks>
    /// <para>
    /// Use this method to build a new <see cref="MixinConfiguration"/> from scratch.
    /// </para>
    /// <para>
    /// If you want to temporarily make the built
    /// <see cref="MixinConfiguration"/> the <see cref="ActiveConfiguration"/>, call the builder's <see cref="MixinConfigurationBuilder.EnterScope"/>
    /// method from within a <c>using</c> statement.
    /// </para>
    /// </remarks>
    public static MixinConfigurationBuilder BuildNew ()
    {
      return new MixinConfigurationBuilder (null);
    }

    /// <summary>
    /// Returns a <see cref="MixinConfigurationBuilder"/> object to build a new <see cref="MixinConfiguration"/> which inherits data from a 
    /// <paramref name="parentConfiguration"/>. This method indirectly creates a copy of the <paramref name="parentConfiguration"/>. Use 
    /// <see cref="BuildNew"/> to avoid the costs of copying.
    /// </summary>
    /// <param name="parentConfiguration">A <see cref="MixinConfiguration"/> whose data should be inherited from the built
    /// <see cref="MixinConfiguration"/>.</param>
    /// <returns>A <see cref="MixinConfigurationBuilder"/> for building a new <see cref="MixinConfiguration"/> with a fluent interface.</returns>
    /// <remarks>
    /// <para>
    /// Use this method to build a new <see cref="MixinConfiguration"/> while taking over the class-mixin bindings from an existing
    /// <see cref="MixinConfiguration"/> object.
    /// </para>
    /// <para>
    /// If you want to temporarily make the built
    /// <see cref="MixinConfiguration"/> the <see cref="ActiveConfiguration"/>, call the builder's <see cref="MixinConfigurationBuilder.EnterScope"/>
    /// method from within a <c>using</c> statement.
    /// </para>
    /// </remarks>
    public static MixinConfigurationBuilder BuildFrom (MixinConfiguration parentConfiguration)
    {
      ArgumentUtility.CheckNotNull ("parentConfiguration", parentConfiguration);
      return new MixinConfigurationBuilder (parentConfiguration);
    }

    /// <summary>
    /// Returns a <see cref="MixinConfigurationBuilder"/> object to build a new <see cref="MixinConfiguration"/> which inherits data from the
    /// <see cref="ActiveConfiguration"/>. This method indirectly creates a copy of the <see cref="ActiveConfiguration"/>. Use <see cref="BuildNew"/> 
    /// to avoid the costs of copying.
    /// </summary>
    /// <returns>A <see cref="MixinConfigurationBuilder"/> for building a new <see cref="MixinConfiguration"/> with a fluent interface.</returns>
    /// <remarks>
    /// <para>
    /// Use this method to build a new <see cref="MixinConfiguration"/> while taking over the class-mixin bindings from the
    /// <see cref="ActiveConfiguration"/>.
    /// </para>
    /// <para>
    /// If you want to temporarily make the built
    /// <see cref="MixinConfiguration"/> the <see cref="ActiveConfiguration"/>, call the builder's <see cref="MixinConfigurationBuilder.EnterScope"/>
    /// method from within a <c>using</c> statement.
    /// </para>
    /// </remarks>
    public static MixinConfigurationBuilder BuildFromActive ()
    {
      return new MixinConfigurationBuilder (ActiveConfiguration);
    }
  }
}
