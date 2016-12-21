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

namespace Remotion.ServiceLocation
{
  /// <summary>
  /// Defines an API for registering a <see cref="ServiceConfigurationEntry"/>.
  /// </summary>
  public interface IServiceConfigurationRegistry
  {
    /// <summary>
    /// Registers a concrete implementation.
    /// </summary>
    /// <param name="serviceConfigurationEntry">A <see cref="ServiceConfigurationEntry"/> describing the concrete implementation to be registered.</param>
    /// <exception cref="InvalidOperationException">An instance of the service type described by the <paramref name="serviceConfigurationEntry"/>
    /// has already been retrieved. Registering factories or concrete implementations can only be done before any instances are retrieved.</exception>
    void Register (ServiceConfigurationEntry serviceConfigurationEntry);
  }
}