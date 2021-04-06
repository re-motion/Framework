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
using System.Configuration;
using System.Configuration.Provider;
using System.Reflection;
using Remotion.Configuration;

namespace Remotion.UnitTests.Configuration
{
  public class StubProviderHelper : ProviderHelperBase<IFakeProvider>
  {
    private string _wellKnownProviderID;
    private string _defaultProviderName;
    private string _defaultProviderID;
    private string _providerCollectionName;

    public StubProviderHelper (
        ExtendedConfigurationSection configurationSection,
        string wellKnownProviderID,
        string defaultProviderName,
        string defaultProviderID,
        string providerCollectionName)
        : base (configurationSection)
    {
      _wellKnownProviderID = wellKnownProviderID;
      _defaultProviderName = defaultProviderName;
      _defaultProviderID = defaultProviderID;
      _providerCollectionName = providerCollectionName;
    }

    public override void PostDeserialze ()
    {
      base.PostDeserialze();

      CheckForDuplicateWellKownProviderName (_wellKnownProviderID);
    }

    public new Type GetTypeWithMatchingVersionNumber (ConfigurationProperty property, string assemblyName, string typeName)
    {
      return base.GetTypeWithMatchingVersionNumber (property, assemblyName, typeName);
    }

    public new Type GetType (ConfigurationProperty property, AssemblyName assemblyName, string typeName)
    {
      return base.GetType (property, assemblyName, typeName);
    }

    public new void InstantiateProviders (
        ProviderSettingsCollection providerSettingsCollection,
        ProviderCollection providerCollection,
        Type providerType,
        params Type[] providerInterfaces)
    {
      base.InstantiateProviders (providerSettingsCollection, providerCollection, providerType, providerInterfaces);
    }

    public new ExtendedProviderBase InstantiateProvider (ProviderSettings providerSettings, Type providerType, params Type[] providerInterfaces)
    {
      return base.InstantiateProvider (providerSettings, providerType, providerInterfaces);
    }

    protected override void EnsureWellKownProviders (ProviderCollection collection)
    {
      base.EnsureWellKownProviders (collection);

      collection.Add (new FakeWellKnownProvider (_wellKnownProviderID, new NameValueCollection()));
    }

    protected override ConfigurationProperty CreateDefaultProviderNameProperty ()
    {
      return CreateDefaultProviderNameProperty (_defaultProviderName, _defaultProviderID);
    }

    protected override ConfigurationProperty CreateProviderSettingsProperty ()
    {
      return CreateProviderSettingsProperty (_providerCollectionName);
    }
  }
}
