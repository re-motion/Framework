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
using Remotion.ServiceLocation;
using Remotion.TypePipe;

namespace Remotion.Reflection.CodeGeneration.TypePipe
{
  /// <summary>
  /// Provides helper code for initialization of the <see cref="PipelineRegistry"/>. This is required in when deserializating TypePipe-generated objects.
  /// If deserialization takes place before the IoC configuration has completed, the  <see cref="PipelineRegistry"/> initialization must be done during
  /// manually during application startup.
  /// </summary>
  public static class PipelineRegistryInitializer
  {
    /// <summary>
    /// Initializes the <see cref="PipelineRegistry"/> with lookup based on the <see cref="SafeServiceLocator"/>. Call this method from within the 
    /// static constructor of the TypePipe particpants.
    /// </summary>
    public static void InitializeWithServiceLocator ()
    {
      // Note: It is still possible that a manual initialization is required in application code if mixin deserialization takes place 
      //       before TypeFactoryImplementation is requested.

      // Note: No Synchronization is required. Either the setup is already done during single-threaded application startup 
      //       or the operation is idem-potent within the PipelineRegistryInitializer. Race-conditions between this method and 3rd-party code
      //       that does not support SafeServiceLocator cannot be handled from within the infrastructure code.
      if (!PipelineRegistry.HasInstanceProvider)
        PipelineRegistry.SetInstanceProvider (() => SafeServiceLocator.Current.GetInstance<IPipelineRegistry>());
    }
  }
}