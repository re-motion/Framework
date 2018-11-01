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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Design.BindableObject
{
  public class BindableObjectTypeFinder
  {
    private readonly IServiceProvider _serviceProvider;

    public BindableObjectTypeFinder (IServiceProvider serviceProvider)
    {
      ArgumentUtility.CheckNotNull ("serviceProvider", serviceProvider);
      _serviceProvider = serviceProvider;
    }

    public IServiceProvider ServiceProvider
    {
      get { return _serviceProvider; }
    }

    public List<Type> GetTypes (bool includeGac)
    {
      ICollection types = GetAllDesignerTypes (includeGac);
      MixinConfiguration applicationContext = GetMixinConfiguration (types);

      List<Type> bindableTypes = new List<Type>();
      using (applicationContext.EnterScope ())
      {
        foreach (Type type in types)
        {
          if (!MixinTypeUtility.IsGeneratedByMixinEngine (type) && BindableObjectProvider.IsBindableObjectImplementation (type))
            bindableTypes.Add (type);
        }
      }
      return bindableTypes;
    }

    public MixinConfiguration GetMixinConfiguration (bool includeGac)
    {
      ICollection typesToBeAnalyzed = GetAllDesignerTypes (includeGac);
      return GetMixinConfiguration(typesToBeAnalyzed);
    }

    private MixinConfiguration GetMixinConfiguration (ICollection typesToBeAnalyzed)
    {
      DeclarativeConfigurationBuilder builder = new DeclarativeConfigurationBuilder (null);
      foreach (Type type in typesToBeAnalyzed)
        builder.AddType (type);

      return builder.BuildConfiguration ();
    }

    private ICollection GetAllDesignerTypes (bool includeGac)
    {
      ITypeDiscoveryService typeDiscoveryService = (ITypeDiscoveryService) _serviceProvider.GetService (typeof (ITypeDiscoveryService));
      if (typeDiscoveryService == null)
        return Type.EmptyTypes;
      else
        return typeDiscoveryService.GetTypes (typeof (object), !includeGac);
    }
  }
}
