﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.ComponentModel.Design;

// ReSharper disable once CheckNamespace

namespace Remotion.Reflection.TypeDiscovery
{
  /// <threadsafety static="true" instance="false"/>
  [Obsolete("Use Remotion.Reflection.ContextAwareTypeUtility instead. (Version 1.15.30.0)", true)]
  public static class ContextAwareTypeDiscoveryUtility
  {
    [Obsolete("Use Remotion.Reflection.ContextAwareTypeUtility.GetTypeDiscoveryService() instead. (Version 1.15.30.0)", true)]
    public static ITypeDiscoveryService GetTypeDiscoveryService ()
    {
      // NOTE: This method must remain until Mixin XRef has been updated to use ContextAwareTypeUtility.GetTypeDiscoveryService() instead.
      return ContextAwareTypeUtility.GetTypeDiscoveryService();
    }
  }
}
