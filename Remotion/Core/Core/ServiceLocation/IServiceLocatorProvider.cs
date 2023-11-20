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
using System.Collections.ObjectModel;

namespace Remotion.ServiceLocation
{
  /// <summary>
  /// Defines an API for classes providing a <see cref="IServiceLocator"/> implementation. This is used by <see cref="SafeServiceLocator"/> in order 
  /// to retrieve the default <see cref="IServiceLocator"/> if no custom one was set via <see cref="ServiceLocator.SetLocatorProvider"/>.
  /// </summary>
  public interface IServiceLocatorProvider
  {
    /// <summary>
    /// Returns an <see cref="IServiceLocator"/> instance.
    /// </summary>
    /// <param name="bootstrapConfiguration">
    /// A list of registrations made to the <see cref="SafeServiceLocator.BootstrapConfiguration"/>. The implementation should include 
    /// those registrations in the returned <see cref="IServiceLocator"/> if it wants the service locator to include all custom configuration prepared
    /// for the boostrapping service locator.
    /// </param>
    /// <returns>An instance of <see cref="IServiceLocator"/>.</returns>
    IServiceLocator GetServiceLocator (ReadOnlyCollection<ServiceConfigurationEntry> bootstrapConfiguration);
  }
}
