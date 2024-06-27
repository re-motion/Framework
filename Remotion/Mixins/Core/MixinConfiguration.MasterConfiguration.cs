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
using Microsoft.Extensions.Logging;
using Remotion.Mixins.Context;
using Remotion.Utilities;

namespace Remotion.Mixins
{
  public partial class MixinConfiguration
  {
    private static readonly DoubleCheckedLockingContainer<MixinConfiguration> s_masterConfiguration =
        new DoubleCheckedLockingContainer<MixinConfiguration>(BuildMasterConfiguration);

    /// <summary>
    /// Gets the master <see cref="MixinConfiguration"/>. The master configuration is the default <see cref="MixinConfiguration"/> used whenever a 
    /// thread first accesses its <see cref="ActiveConfiguration"/>. If no master configuration has been set via <see cref="SetMasterConfiguration"/>,
    /// a default configuration will be built by <see cref="DeclarativeConfigurationBuilder.BuildDefaultConfiguration"/>.
    /// </summary>
    /// <returns>The master <see cref="MixinConfiguration"/>.</returns>
    /// <seealso cref="SetMasterConfiguration"/>
    public static MixinConfiguration GetMasterConfiguration ()
    {
      return s_masterConfiguration.Value;
    }

    /// <summary>
    /// Sets the master <see cref="MixinConfiguration"/>. The master configuration is the default <see cref="MixinConfiguration"/> used whenever a 
    /// thread first accesses its <see cref="ActiveConfiguration"/>. If the master configuration is set to <see langword="null" />, the next call
    /// to <see cref="GetMasterConfiguration"/> (or the next thread first accessing its <see cref="ActiveConfiguration"/>) will trigger a new
    /// default configuration to be built.
    /// </summary>
    /// <param name="newMasterConfiguration">The <see cref="MixinConfiguration"/> to be used as the new master configuration.</param>
    /// <remarks>
    /// Changes made to the master configuration will affect any thread accessing its mixin configuration for the
    /// first time after this method has been called.
    /// </remarks>
    /// <seealso cref="GetMasterConfiguration"/>
    public static void SetMasterConfiguration (MixinConfiguration newMasterConfiguration)
    {
      s_masterConfiguration.Value = newMasterConfiguration;
    }

    /// <summary>
    /// Causes the master mixin configuration to be rebuilt from scratch the next time a thread accesses its mixin configuration for the first time.
    /// </summary>
    /// <remarks>
    /// The master mixin configuration is the default mixin configuration used whenever a thread first accesses
    /// <see cref="ActiveConfiguration"/>. Changes made to it will affect any thread accessing its mixin configuration for the
    /// first time after this method has been called.
    /// </remarks>
    public static void ResetMasterConfiguration ()
    {
      s_masterConfiguration.Value = null!;
    }

    private static MixinConfiguration BuildMasterConfiguration ()
    {
      s_logger.LogInformation("Building mixin master configuration...");
      using (StopwatchScope.CreateScope(s_logger, LogLevel.Information, "Time needed to build mixin master configuration: {elapsed}."))
      {
        return DeclarativeConfigurationBuilder.BuildDefaultConfiguration();
      }
    }
  }
}
