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
using Remotion.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
/// <summary>
/// Represents the common information all configuration classes provide.
/// </summary>
public class ConfigurationBase
{
  // types

  // static members and constants

  // member fields

  private bool _resolveTypes;

  // construction and disposing

  /// <summary>
  /// Initializes a new instance of the <b>ConfigurationBase</b> class from the specified <see cref="BaseFileLoader"/>.
  /// </summary>
  /// <param name="loader">The <see cref="BaseFileLoader"/> to be used for reading the configuration. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="loader"/> is <see langword="null"/>.</exception>
  protected ConfigurationBase (BaseFileLoader loader)
  {
    ArgumentUtility.CheckNotNull ("loader", loader);

    _resolveTypes = loader.ResolveTypes;
  }

  // methods and properties

  /// <summary>
  /// Gets a flag whether type names in the configuration file should be resolved to their corresponding .NET <see cref="Type"/>.
  /// </summary>
  public bool ResolveTypes
  {
    get { return _resolveTypes; }
  }
}
}
