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
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Remotion.Data.DomainObjects.Mapping.Configuration
{
  public class MappingLoaderConfiguration: ExtendedConfigurationSection
  {
    private readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
    private readonly ConfigurationProperty _mappingLoaderProperty;

    public MappingLoaderConfiguration()
    {
      _mappingLoaderProperty = new ConfigurationProperty (
          "loader",
          typeof (TypeElement<IMappingLoader, MappingReflector>),
          null,
          ConfigurationPropertyOptions.None);

      _properties.Add (_mappingLoaderProperty);
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }

    public IMappingLoader CreateMappingLoader()
    {
      return MappingLoaderElement.CreateInstance();
    }

    public Type MappingLoaderType
    {
      get { return MappingLoaderElement.Type; }
      set { MappingLoaderElement.Type = value; }
    }

    protected TypeElement<IMappingLoader> MappingLoaderElement
    {
      get { return (TypeElement<IMappingLoader>) this[_mappingLoaderProperty]; }
    }
  }
}
