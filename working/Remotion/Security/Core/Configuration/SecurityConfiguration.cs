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
using System.Configuration;
using Remotion.Configuration;

namespace Remotion.Security.Configuration
{
  /// <summary> The configuration section for <see cref="Remotion.Security"/>. </summary>
  /// <threadsafety static="true" instance="true" />
  public class SecurityConfiguration : ExtendedConfigurationSection
  {
    #region Obsolete

    [Obsolete ("Use the application's IoC container to retrieve the configured ISecurityProvier implementation. (Version 1.15.20.0)", true)]
    public ISecurityProvider SecurityProvider
    {
      get
      {
        throw new NotImplementedException (
            "Use the application's IoC container to retrieve the configured ISecurityProvier implementation. (Version 1.15.20.0)");
      }
      set
      {
        throw new NotImplementedException (
            "Use the application's IoC container to configure the ISecurityProvier implementation. (Version 1.15.20.0)");
      }
    }

    [Obsolete ("Use the application's IoC container to retrieve the configured IPrincipalProvider implementation. (Version 1.15.20.0)", true)]
    public IPrincipalProvider PrincipalProvider
    {
      get
      {
        throw new NotImplementedException (
            "Use the application's IoC container to retrieve the configured IPrincipalProvider implementation. (Version 1.15.20.0)");
      }
      set
      {
        throw new NotImplementedException (
            "Use the application's IoC container to configure the IPrincipalProvider implementation. (Version 1.15.20.0)");
      }
    }

    #endregion

    private static readonly Lazy<SecurityConfiguration> s_current;

    static SecurityConfiguration ()
    {
      s_current = new Lazy<SecurityConfiguration> (
        () => (SecurityConfiguration) ConfigurationManager.GetSection ("remotion.security") ?? new SecurityConfiguration());
    }

    public static SecurityConfiguration Current
    {
      get { return s_current.Value; }
    }

    private readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
    private readonly ConfigurationProperty _disableAccessChecksProperty;

    public SecurityConfiguration ()
    {
      _disableAccessChecksProperty = new ConfigurationProperty ("disableAccessChecks", typeof (bool), false, ConfigurationPropertyOptions.None);

      _properties.Add (_disableAccessChecksProperty);

      _properties.Add (new ConfigurationProperty ("xmlns", typeof (string), null, ConfigurationPropertyOptions.None));
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }

    /// <summary>
    /// Gets or sets a flag that controls if <see cref="SecurityClient"/>.<see cref="SecurityClient.CreateSecurityClientFromConfiguration"/>()
    /// creates a working <see cref="SecurityClient"/> or a null-implementation.
    /// </summary>
    public bool DisableAccessChecks
    {
      get { return (bool) this[_disableAccessChecksProperty]; }
      set { this[_disableAccessChecksProperty] = value; }
    }
  }
}