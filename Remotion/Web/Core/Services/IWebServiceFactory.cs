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

namespace Remotion.Web.Services
{
  /// <summary>
  /// Defines the API for a factory capable of instantiating web services
  /// </summary>
  public interface IWebServiceFactory
  {
    /// <summary>
    /// Instantiates the web service located at the <paramref name="virtualPath"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of the instantiated web service. Can be an interface.</typeparam>
    /// <param name="virtualPath">The virtual path where the web service is located. Must not be <see langword="null" /> or empty.</param>
    T CreateWebService<T> (string virtualPath) where T: class;

    /// <summary>
    /// Instantiates the script web service located at the <paramref name="virtualPath"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of the instantiated script web service. Can be an interface.</typeparam>
    /// <param name="virtualPath">The virtual path where the web service is located. Must not be <see langword="null" /> or empty.</param>
    T CreateScriptService<T> (string virtualPath) where T: class;

    /// <summary>
    /// Instantiates the JSON web service located at the <paramref name="virtualPath"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of the instantiated JSON web service. Can be an interface.</typeparam>
    /// <param name="virtualPath">The virtual path where the web service is located. Must not be <see langword="null" /> or empty.</param>
    T CreateJsonService<T> (string virtualPath) where T: class;
  }
}