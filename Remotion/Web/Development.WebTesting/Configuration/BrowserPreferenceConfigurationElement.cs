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

namespace Remotion.Web.Development.WebTesting.Configuration
{
  /// <summary>
  /// Additionally configured browser preference, see <see cref="WebTestingConfiguration.BrowserPreferences"/> and
  /// <see cref="BrowserPreferencesConfigurationElementCollection"/>.
  /// </summary>
  public class BrowserPreferenceConfigurationElement : ConfigurationElement
  {
    private readonly ConfigurationPropertyCollection _properties;
    private readonly ConfigurationProperty _keyProperty;
    private readonly ConfigurationProperty _valueProperty;

    public BrowserPreferenceConfigurationElement ()
    {
      _keyProperty = new ConfigurationProperty ("key", typeof (string), null, ConfigurationPropertyOptions.IsKey);
      _valueProperty = new ConfigurationProperty ("value", typeof (string));

      _properties = new ConfigurationPropertyCollection
                    {
                        _keyProperty,
                        _valueProperty
                    };
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }

    public string Key
    {
      get { return (string) this[_keyProperty]; }
    }

    public object Value
    {
      get { return this[_valueProperty]; }
    }
  }
}