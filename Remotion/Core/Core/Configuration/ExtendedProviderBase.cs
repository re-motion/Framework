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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Provider;
using Remotion.Utilities;

namespace Remotion.Configuration
{
  /// <summary>Base class for all providers.</summary>
  /// <remarks>
  /// <see cref="ExtendedProviderBase"/> changes the protocoll for initializing a configuration provider from using a default constructor
  /// followed by a call to <see cref="Initialize"/> to initialize the provider during construction.
  /// </remarks>
  public abstract class ExtendedProviderBase: ProviderBase
  {
    private readonly NameValueCollection _config;

    /// <summary>Initializes a new instance of the <see cref="ExtendedProviderBase"/>.</summary>
    /// <param name="name">The friendly name of the provider. Must not be <see langword="null" /> or empty.</param>
    /// <param name="config">
    /// A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.
    /// Must not be <see langword="null" />.
    /// </param>
    protected ExtendedProviderBase (string name, NameValueCollection config)
    {
      NameValueCollection configClone = new NameValueCollection (config);
      Initialize (name, config);
      _config = configClone;
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public override sealed void Initialize (string name, NameValueCollection config)
    {
      base.Initialize (name, config);
      Assertion.IsNull (_config, "Initialize can only succeed when called for the first time, from the constructor");
    }

    protected string GetAndRemoveNonEmptyStringAttribute (NameValueCollection config, string attribute, string providerName, bool required)
    {
      ArgumentUtility.CheckNotNull ("config", config);
      ArgumentUtility.CheckNotNullOrEmpty ("attribute", attribute);
      ArgumentUtility.CheckNotNullOrEmpty ("providerName", providerName);

      string value = config.Get (attribute);
      if ((value == null && required) || (value != null && value.Length == 0))
      {
        throw new ConfigurationErrorsException (
            string.Format ("The attribute '{0}' is missing in the configuration of the '{1}' provider.", attribute, providerName));
      }
      config.Remove (attribute);
      
      return value;
    }
  }
}
