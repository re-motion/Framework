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
using System.Linq;
using System.Reflection;

namespace Remotion.Scripting.UnitTests
{
 
  public static class TypeExtensions {
    /// <summary>
    /// Same as <see cref="Type.GetMethod(string,System.Reflection.BindingFlags)"/>, but also matches the
    /// passed parameter types.
    /// </summary>
    public static MethodInfo[] GetMethods (this Type type, string name,
                                           BindingFlags bindingFlags, params Type[] parameterTypes)
    {
      return type.GetMethods  (bindingFlags).Where (
          mi => (mi.Name == name) && mi.GetParameters ().Select (pi => pi.ParameterType).SequenceEqual (parameterTypes)).ToArray ();
    }

    public static MethodInfo[] GetPublicInstanceMethods (this Type type, string name, params Type[] parameterTypes)
    {
      return type.GetMethods (name, BindingFlags.Instance | BindingFlags.Public, parameterTypes);
    }

    public static MethodInfo[] GetAllInstanceMethods (this Type type, string name, params Type[] parameterTypes)
    {
      return type.GetMethods (name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, parameterTypes);
    }

    public static MethodInfo[] GetPrivateInstanceMethods (this Type type, string name, params Type[] parameterTypes)
    {
      return type.GetMethods (name, BindingFlags.Instance | BindingFlags.NonPublic, parameterTypes);
    }

    public static MethodInfo[] GetAllMethods (this Type type)
    {
      return type.GetMethods (BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
    }

    public static PropertyInfo[] GetProperties (this Type type, string name, BindingFlags bindingFlags)
    {
      return type.GetProperties (bindingFlags).Where (pi => (pi.Name == name)).ToArray ();
    }

    public static PropertyInfo[] GetAllProperties (this Type type, string name)
    {
      return type.GetProperties (name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
    }

    public static PropertyInfo[] GetAllProperties (this Type type)
    {
      return type.GetProperties (BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
    }

  }
}
