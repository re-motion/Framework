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
using Remotion.Mixins.Context;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  public class PersistentMixinFinder : IPersistentMixinFinder
  {
    public static ClassContext GetMixinConfigurationForDomainObjectType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      // For performance, use the ClassContextCollection rather than ActiveConfiguration.GetClassContext
      // (The former checks whether type is a generated type, which we know isn't the case here.)
      return Mixins.MixinConfiguration.ActiveConfiguration.ClassContexts.GetWithInheritance (type) ?? CreateEmptyClassContext(type);
    }

    public static bool IsPersistenceRelevant (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);

      return TypeExtensions.CanAscribeTo (mixinType, typeof (DomainObjectMixin<,>));
    }

    private static ClassContext CreateEmptyClassContext (Type type)
    {
      return new ClassContext (type, Enumerable.Empty<MixinContext> (), Enumerable.Empty<Type> ());
    }

    private readonly ClassContext _mixinConfiguration;
    private readonly List<ClassContext> _allParentClassContexts;
    private readonly ClassContext _parentClassContext;
    private readonly bool _includeInherited;

    private Type[] _persistentMixins;

    public PersistentMixinFinder (Type type)
      : this (type, false)
    {
    }

    public PersistentMixinFinder (Type type, bool includeInherited)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      Type = type;
      _includeInherited = includeInherited;
      
      _mixinConfiguration = GetMixinConfigurationForDomainObjectType (type); // never null
       
      if (Type.BaseType != null)
        _parentClassContext = GetMixinConfigurationForDomainObjectType (Type.BaseType); // never null

      if (IncludeInherited)
        _allParentClassContexts = GetParentClassContexts();
    }

    private List<ClassContext> GetParentClassContexts ()
    {
      var parentClassContexts = new List<ClassContext> ();
      ClassContext current = MixinConfiguration;
      while (current != null && current.Type.BaseType != null)
      {
        ClassContext parent = Mixins.MixinConfiguration.ActiveConfiguration.GetContext (current.Type.BaseType);
        if (parent != null)
          parentClassContexts.Add (parent);
        current = parent;
      }
      parentClassContexts.Reverse (); // first base is first
      return parentClassContexts;
    }

    public Type Type { get; private set; }

    public ClassContext MixinConfiguration
    {
      get { return _mixinConfiguration; }
    }

    public ClassContext ParentClassContext
    {
      get { return _parentClassContext; }
    }

    public bool IncludeInherited
    {
      get { return _includeInherited; }
    }

    public Type[] GetPersistentMixins ()
    {
      if (_persistentMixins == null)
        _persistentMixins = CalculatePersistentMixins ().ToArray();
      return _persistentMixins;
    }

    public bool IsInParentContext (Type mixinType)
    {
      return ParentClassContext.Mixins.ContainsAssignableMixin (mixinType);
    }

    private IEnumerable<Type> CalculatePersistentMixins ()
    {
      // Generic parameter substitution is disallowed on purpose: the reflection-based mapping considers mixins applied to a base class to be
      // part of the base class definition. Therefore, the mixin applied to the derived class must be exactly the same as that on the base class,
      // otherwise the mapping might be inconsistent with the actual property types. With generic parameter substitution, an inherited mixin
      // might change with the derived class, so we can't allow it.
      // (The need to specify all generic arguments is also consistent with the mapping rule disallowing generic domain object types in the mapping;
      // and Extends and Uses both provide means to explicitly specify generic type arguments.)
      CheckForSuppressedMixins ();

      return from mixin in MixinConfiguration.Mixins
             where IsPersistenceRelevant (mixin.MixinType) && (IncludeInherited || !IsInParentContext (mixin.MixinType))
             select CheckNotOpenGenericMixin (mixin).MixinType;
    }

    private void CheckForSuppressedMixins ()
    {
      var suppressedMixins = from mixin in ParentClassContext.Mixins
                             where IsPersistenceRelevant (mixin.MixinType) && !MixinConfiguration.Mixins.ContainsAssignableMixin (mixin.MixinType)
                             select mixin;

      MixinContext suppressedMixin = suppressedMixins.FirstOrDefault();
      if (suppressedMixin != null)
      {
        string message = string.Format ("Class '{0}' suppresses mixin '{1}' inherited from its base class '{2}'. This is not allowed because "
            + "the mixin adds persistence information to the base class which must also be present in the derived class.", Type.FullName,
            suppressedMixin.MixinType.Name, ParentClassContext.Type.Name);
        throw new MappingException (message);
      }
    }

    private MixinContext CheckNotOpenGenericMixin (MixinContext mixin)
    {
      if (mixin.MixinType.ContainsGenericParameters)
      {
        string message = string.Format ("The persistence-relevant mixin {0} applied to class {1} has open generic type parameters. All type "
            + "parameters of the mixin must be specified when it is applied to a DomainObject.", mixin.MixinType.FullName, Type.FullName);
        throw new MappingException (message);
      }
      else
        return mixin;
    }

    public Type FindOriginalMixinTarget (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);

      Assertion.IsTrue (_allParentClassContexts != null || !IncludeInherited, "If IncludeInherited is set, _allParentClassContexts is never null.");

      if (!IncludeInherited && ParentClassContext.Mixins.ContainsKey (mixinType))
        throw new InvalidOperationException ("The given mixin is inherited from the base class, but includeInherited is not set to true.");

      ClassContext parent = _allParentClassContexts != null ? _allParentClassContexts.FirstOrDefault (c => c.Mixins.ContainsKey (mixinType)) : null;
      if (parent != null)
        return parent.Type;
      else if (MixinConfiguration.Mixins.ContainsKey (mixinType))
        return MixinConfiguration.Type;
      else
        return null;
    }
  }
}
