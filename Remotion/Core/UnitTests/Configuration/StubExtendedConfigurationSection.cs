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

namespace Remotion.UnitTests.Configuration
{
  public class StubExtendedConfigurationSection : ExtendedConfigurationSection
  {
    // constants

    // types

    // static members

    // member fields

    private ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
    private StubProviderHelper _stubProviderHelper;

    // construction and disposing

    public StubExtendedConfigurationSection (
        string wellKnownProviderID,
        string defaultProviderName,
        string defaultProviderID,
        string providerCollectionName
        )
    {
      _stubProviderHelper = new StubProviderHelper (this, wellKnownProviderID, defaultProviderName, defaultProviderID, providerCollectionName);
      _stubProviderHelper.InitializeProperties (_properties);
    }

    // methods and properties

    public StubProviderHelper GetStubProviderHelper ()
    {
      return _stubProviderHelper;
    }

    public ConfigurationPropertyCollection GetProperties ()
    {
      return _properties;
    }

    protected override void PostDeserialize ()
    {
      base.PostDeserialize();

      _stubProviderHelper.PostDeserialze();
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }
  }
}
