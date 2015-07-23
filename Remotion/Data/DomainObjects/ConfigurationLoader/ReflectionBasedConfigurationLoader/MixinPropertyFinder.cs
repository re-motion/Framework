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
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  public class MixinPropertyFinder
  {
    private readonly IPersistentMixinFinder _persistentMixinFinder;
    private readonly bool _includeBaseMixins;
    private readonly Func<Type, bool, bool, PropertyFinderBase> _propertyFinderFactory;

    public MixinPropertyFinder (
        Func<Type, bool, bool, PropertyFinderBase> propertyFinderFactory,
        IPersistentMixinFinder persistentMixinFinder,
        bool includeBaseMixins)
    {
      ArgumentUtility.CheckNotNull ("propertyFinderFactory", propertyFinderFactory);

      _propertyFinderFactory = propertyFinderFactory;
      _persistentMixinFinder = persistentMixinFinder;
      _includeBaseMixins = includeBaseMixins;
    }

    public IEnumerable<IPropertyInformation> FindPropertyInfosOnMixins ()
    {
      if (_persistentMixinFinder != null)
      {
        var processedMixins = new HashSet<Type> ();
        return from mixin in _persistentMixinFinder.GetPersistentMixins ()
               from propertyInfo in FindPropertyInfosOnMixin (mixin, processedMixins)
               select propertyInfo;
      }
      else
      {
        return Enumerable.Empty<IPropertyInformation>();
      }
    }

    private IEnumerable<IPropertyInformation> FindPropertyInfosOnMixin (Type mixin, HashSet<Type> processedMixins)
    {
      Type current = mixin;
      while (current != null && !IsMixinBaseClass (current))
      {
        if (!processedMixins.Contains (current) && (_includeBaseMixins || !_persistentMixinFinder.IsInParentContext (current)))
        {
          // Note: mixins on mixins are not checked
          var mixinPropertyFinder = _propertyFinderFactory (current, false, false);
          foreach (var propertyInfo in mixinPropertyFinder.FindPropertyInfosDeclaredOnThisType ())
            yield return propertyInfo;

          processedMixins.Add (current);
        }
        current = current.BaseType;
      }
    }

    private static bool IsMixinBaseClass (Type type)
    {
      if (!type.IsGenericType)
        return false;
      Type genericTypeDefinition = type.GetGenericTypeDefinition();
      return genericTypeDefinition == typeof (Mixin<>) || genericTypeDefinition == typeof (Mixin<,>);
    }
  }
}