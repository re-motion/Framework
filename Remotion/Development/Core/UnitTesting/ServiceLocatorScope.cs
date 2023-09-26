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
using System.Collections.Generic;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting
{
  /// <summary>
  /// Sets <see cref="ServiceLocator.Current"/> to a given <see cref="IServiceLocator"/> temporarily, resetting it, when <see cref="Dispose"/>
  /// is called.
  /// </summary>
  public class ServiceLocatorScope : IDisposable
  {
    private static DefaultServiceLocator CreateServiceLocator (IEnumerable<ServiceConfigurationEntry> configuration)
    {
      ArgumentUtility.CheckNotNull("configuration", configuration);

      var defaultServiceLocator = DefaultServiceLocator.Create();
      foreach (var stubbedRegistration in configuration)
        defaultServiceLocator.Register(stubbedRegistration);
      return defaultServiceLocator;
    }

    private readonly ServiceLocatorProvider? _previousLocatorProvider;

    public ServiceLocatorScope (IServiceLocator temporaryServiceLocator)
    {
      if (ServiceLocator.IsLocationProviderSet)
      {
        var previousLocator = ServiceLocator.Current;
        _previousLocatorProvider = () => previousLocator;
      }
      else
      {
        _previousLocatorProvider = null;
      }

      ServiceLocator.SetLocatorProvider(() => temporaryServiceLocator);
    }

    public ServiceLocatorScope (params ServiceConfigurationEntry[] temporaryConfiguration)
      : this(CreateServiceLocator(temporaryConfiguration))
    {
    }

    public void Dispose ()
    {
      ServiceLocator.SetLocatorProvider(_previousLocatorProvider);
    }
  }
}
