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
using System.Reflection;
using System.Runtime.Serialization;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Forms a bridge between domain objects and mixins by supporting generation and deserialization of mixed domain objects.
  /// </summary>
  /// <remarks>All of the methods in this class are tolerant agains non-mixed types, i.e. they will perform default/no-op actions if a non-mixed
  /// type (or object) is passed to them rather than throwing exceptions.</remarks>
  public static class DomainObjectMixinCodeGenerationBridge
  {
    internal class DummyObjectReference
#pragma warning disable SYSLIB0050
        : IObjectReference
#pragma warning restore SYSLIB0050
    {
      private readonly object _realObject;

      public DummyObjectReference (Type concreteDeserializedType, SerializationInfo info, StreamingContext context)
      {
        try
        {
          Assertion.DebugAssert(
              NullableTypeUtility.IsNullableType(concreteDeserializedType) == false,
              "NullableTypeUtility.IsNullableType (concreteDeserializedType) == false with concreteDeserializedType '{0}'.",
              concreteDeserializedType);

          _realObject = Activator.CreateInstance(
              concreteDeserializedType,
              BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
              null,
              new object[] { info, context },
              null)!;
        }
        catch (MissingMethodException ex)
        {
          throw new MissingMethodException("No deserialization constructor was found on type " + concreteDeserializedType.GetFullNameSafe() + ".", ex);
        }
      }

      public object GetRealObject (StreamingContext context)
      {
        return _realObject;
      }
    }

    public static void OnDomainObjectReferenceInitializing (DomainObject instance)
    {
      ArgumentUtility.CheckNotNull("instance", instance);
      NotifyDomainObjectMixins(instance, mixin => mixin.OnDomainObjectReferenceInitializing());
    }

    public static void OnDomainObjectCreated (DomainObject instance)
    {
      ArgumentUtility.CheckNotNull("instance", instance);
      NotifyDomainObjectMixins(instance, mixin => mixin.OnDomainObjectCreated());
    }

    public static void OnDomainObjectLoaded (DomainObject instance, LoadMode loadMode)
    {
      ArgumentUtility.CheckNotNull("instance", instance);
      NotifyDomainObjectMixins(instance, mixin => mixin.OnDomainObjectLoaded(loadMode));
    }

    private static void NotifyDomainObjectMixins (DomainObject instance, Action<IDomainObjectMixin> notifier)
    {
      var instanceAsMixinTarget = instance as IMixinTarget;
      if (instanceAsMixinTarget != null)
      {
        foreach (object mixin in instanceAsMixinTarget.Mixins)
        {
          var mixinAsDomainObjectMixin = mixin as IDomainObjectMixin;
          if (mixinAsDomainObjectMixin != null)
            notifier(mixinAsDomainObjectMixin);
        }
      }
    }
  }
}
