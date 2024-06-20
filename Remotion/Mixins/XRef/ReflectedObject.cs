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
using System.Linq;
using System.Reflection;
using Remotion.Mixins.CrossReferencer.Utility;
using Remotion.Utilities;

namespace Remotion.Mixins.CrossReferencer
{
  public class ReflectedObject : IEnumerable<ReflectedObject>
  {
    private static readonly FastMemberInvokerCache s_cache = new();

    public object WrappedObject { get; }

    public ReflectedObject (object wrappedObject)
    {
      ArgumentUtility.CheckNotNull("wrappedObject", wrappedObject);

      if (wrappedObject is ReflectedObject)
        throw new ArgumentException("There is no point in wrapping an instance of 'MixinXRef.Reflection.ReflectedObject'.");

      WrappedObject = wrappedObject;
    }

    public static ReflectedObject Create (Assembly assembly, string fullName, params object[] parameters)
    {
      ArgumentUtility.CheckNotNull("assembly", assembly);
      ArgumentUtility.CheckNotNull("fullName", fullName);
      ArgumentUtility.CheckNotNull("parameters", parameters);

      return new ReflectedObject(Activator.CreateInstance(assembly.GetType(fullName, true)!, UnWrapParameters(parameters))!);
    }

    public static ReflectedObject? CallMethod (Type type, string methodName, params object[] parameters)
    {
      ArgumentUtility.CheckNotNull("type", type);
      ArgumentUtility.CheckNotNull("methodName", methodName);
      ArgumentUtility.CheckNotNull("parameters", parameters);

      return CallMethod(type, methodName, new Type[0], parameters);
    }

    public static ReflectedObject? CallMethod (Type type, string methodName, Type[] typeParameters, params object[] parameters)
    {
      ArgumentUtility.CheckNotNull("type", type);
      ArgumentUtility.CheckNotNull("methodName", methodName);
      ArgumentUtility.CheckNotNull("typeParameters", typeParameters);
      ArgumentUtility.CheckNotNull("parameters", parameters);

      var unwrappedParameters = UnWrapParameters(parameters);
      var argumentTypes = unwrappedParameters.Select(obj => obj.GetType()).ToArray();
      var invoker = s_cache.GetOrCreateFastMethodInvoker(type, methodName, typeParameters, argumentTypes, BindingFlags.Public | BindingFlags.Static);

      var returnValue = invoker(null!, unwrappedParameters);
      return returnValue == null ? null : new ReflectedObject(returnValue);
    }

    public ReflectedObject? CallMethod (string methodName, params object[] parameters)
    {
      ArgumentUtility.CheckNotNull("methodName", methodName);
      ArgumentUtility.CheckNotNull("parameters", parameters);

      return CallMethod(methodName, new Type[0], parameters);
    }

    public ReflectedObject? CallMethod (string methodName, Type[] typeParameters, params object[] parameters)
    {
      ArgumentUtility.CheckNotNull("methodName", methodName);
      ArgumentUtility.CheckNotNull("typeParameters", typeParameters);
      ArgumentUtility.CheckNotNull("parameters", parameters);

      var unwrappedParameters = UnWrapParameters(parameters);
      var argumentTypes = unwrappedParameters.Select(obj => obj.GetType()).ToArray();
      var invoker = s_cache.GetOrCreateFastMethodInvoker(WrappedObject.GetType(), methodName, typeParameters, argumentTypes, BindingFlags.Public | BindingFlags.Instance);

      var returnValue = invoker(WrappedObject, unwrappedParameters);
      return returnValue == null ? null : new ReflectedObject(returnValue);
    }

    public ReflectedObject? GetProperty (string propertyName)
    {
      ArgumentUtility.CheckNotNull("propertyName", propertyName);

      return CallMethod("get_" + propertyName);
    }

    public T To<T> ()
    {
      return (T)WrappedObject;
    }

    public IEnumerator<ReflectedObject> GetEnumerator ()
    {
      var wrappedObjectAsEnumerable = WrappedObject as IEnumerable;

      if (wrappedObjectAsEnumerable != null)
      {
        foreach (var item in wrappedObjectAsEnumerable)
          yield return new ReflectedObject(item);
      }
      else
        throw new NotSupportedException(string.Format("The reflected object '{0}' is not enumerable.", WrappedObject.GetType()));
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public IEnumerable AsEnumerable<T> ()
    {
      return this.Select(reflectedObject => reflectedObject.To<T>());
    }

    public override string ToString ()
    {
      return WrappedObject.ToString() ?? string.Empty;
    }

    public override bool Equals (object? obj)
    {
      return obj is ReflectedObject && WrappedObject.Equals(UnWrapInstance(obj));
    }

    public override int GetHashCode ()
    {
      return WrappedObject.GetHashCode();
    }

    private static object UnWrapInstance (object instance)
    {
      var reflectedInstance = instance as ReflectedObject;

      return reflectedInstance == null ? instance : reflectedInstance.To<object>();
    }

    private static object[] UnWrapParameters (object[] parameters)
    {
      if (parameters == null)
        return null!;

      for (int i = 0; i < parameters.Length; i++)
      {
        parameters[i] = UnWrapInstance(parameters[i]);
      }

      return parameters;
    }
  }
}
