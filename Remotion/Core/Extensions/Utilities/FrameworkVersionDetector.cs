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
using System.Transactions;

namespace Remotion.Utilities
{
  /// <summary>
  /// Allows for the testing of a specific .NET Framework version. This can be used to ensure a certain .NET Framework has been installed.
  /// Otherwise, runtime expections can occur, e.g. a <see cref="MissingMethodException"/> can ben thrown.
  /// </summary>
  public static class FrameworkVersionDetector
  {
    private static readonly Lazy<bool> s_isNet_4_6_Supported = new Lazy<bool> (IsNet_4_6_Installed);

    private static readonly Lazy<bool> s_isNet_4_5_2_Supported = new Lazy<bool> (IsNet_4_5_2_Installed);

    private static readonly Lazy<bool> s_isNet_4_5_1_Supported = new Lazy<bool> (IsNet_4_5_1_Installed);

    private static readonly Lazy<bool> s_isNet_4_5_Supported = new Lazy<bool> (IsNet_4_5_Installed);

    private static readonly Lazy<bool> s_isNet_4_0_Supported = new Lazy<bool> (() => Environment.Version.Major >= 4);

    /// <summary>
    /// Returns <see langword="true" /> if the <paramref name="frameworkVersion"/> is supported by the current system/runtime.
    /// </summary>
    public static bool IsVersionSupported (FrameworkVersion frameworkVersion)
    {
      if (frameworkVersion >= FrameworkVersion.Net_4_6)
        return s_isNet_4_6_Supported.Value;

      if (frameworkVersion >= FrameworkVersion.Net_4_5_2)
        return s_isNet_4_5_2_Supported.Value;

      if (frameworkVersion >= FrameworkVersion.Net_4_5_1)
        return s_isNet_4_5_1_Supported.Value;

      if (frameworkVersion >= FrameworkVersion.Net_4_5)
        return s_isNet_4_5_Supported.Value;

      if (frameworkVersion == FrameworkVersion.Net_4_0)
        return s_isNet_4_0_Supported.Value;

      throw new ArgumentException (string.Format("'{0}' is not a valid FrameworkVersion.", frameworkVersion));
    }

    private static bool IsNet_4_6_Installed ()
    {
      // https://msdn.microsoft.com/en-US/library/ms171868.aspx#v46
      return Type.GetType ("System.AppContext", throwOnError: false) != null;
    }

    private static bool IsNet_4_5_2_Installed ()
    {
      // http://msdn.microsoft.com/en-us/library/ms171868.aspx#v452
      var transactionType = typeof (Transaction);
      return transactionType.GetMethod (
          "PromoteAndEnlistDurable",
          new[]
          {
              typeof (Guid),
              typeof (IPromotableSinglePhaseNotification),
              typeof (ISinglePhaseNotification),
              typeof (EnlistmentOptions)
          }) != null;
    }

    private static bool IsNet_4_5_1_Installed ()
    {
      // http://msdn.microsoft.com/en-us/library/ms171868.aspx#v451
      return Type.GetType ("System.Runtime.GCLargeObjectHeapCompactionMode", throwOnError: false, ignoreCase: false) != null;
    }

    private static bool IsNet_4_5_Installed ()
    {
      // http://msdn.microsoft.com/en-us/library/ms171868.aspx#v45
      return Type.GetType ("System.Reflection.ReflectionContext", throwOnError: false) != null;
    }
  }
}