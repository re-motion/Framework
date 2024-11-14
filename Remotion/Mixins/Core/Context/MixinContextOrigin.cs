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
using System.Diagnostics;
using System.Reflection;
using Remotion.Mixins.Context.Serialization;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Context
{
  /// <summary>
  /// Describes the code artifact (custom attribute, method, etc.) a <see cref="MixinContext"/> was configured by.
  /// </summary>
  public class MixinContextOrigin : IEquatable<MixinContextOrigin>
  {
    public static MixinContextOrigin CreateForCustomAttribute (Attribute attribute, MemberInfo target)
    {
      ArgumentUtility.CheckNotNull("attribute", attribute);
      ArgumentUtility.CheckNotNull("target", target);

      return new MixinContextOrigin(attribute.GetType().Name, target.Module.Assembly, target.ToString()!);
    }

    public static MixinContextOrigin CreateForCustomAttribute (Attribute attribute, Assembly assembly)
    {
      ArgumentUtility.CheckNotNull("attribute", attribute);
      ArgumentUtility.CheckNotNull("assembly", assembly);

      return new MixinContextOrigin(attribute.GetType().Name, assembly, "assembly");
    }

    public static MixinContextOrigin CreateForMethod (MethodBase methodBase)
    {
      ArgumentUtility.CheckNotNull("methodBase", methodBase);

      var location = String.Format("{0}, declaring type: {1}", methodBase, methodBase.DeclaringType);
      return new MixinContextOrigin("Method", methodBase.Module.Assembly, location);
    }

    public static MixinContextOrigin CreateForStackFrame (StackFrame stackFrame)
    {
      ArgumentUtility.CheckNotNull("stackFrame", stackFrame);
      return CreateForMethod(stackFrame.GetMethod()!);
    }

    public static MixinContextOrigin Deserialize (IMixinContextOriginDeserializer deserializer)
    {
      ArgumentUtility.CheckNotNull("deserializer", deserializer);

      return new MixinContextOrigin(deserializer.GetKind(), deserializer.GetAssembly(), deserializer.GetLocation());
    }

    private readonly string _kind; // e.g., UsesAttribute or Imperative
    private readonly Assembly _assembly;
    private readonly string _location; // e.g., a type name, or a fully qualified method name

    public MixinContextOrigin (string kind, Assembly assembly, string location)
    {
      ArgumentUtility.CheckNotNullOrEmpty("kind", kind);
      ArgumentUtility.CheckNotNull("assembly", assembly);
      ArgumentUtility.CheckNotNullOrEmpty("location", location);

      _kind = kind;
      _assembly = assembly;
      _location = location;
    }

    public string Kind
    {
      get { return _kind; }
    }

    public Assembly Assembly
    {
      get { return _assembly; }
    }

    public string Location
    {
      get { return _location; }
    }

    public override string ToString ()
    {
      var assemblyName = Assembly.GetName();
      return string.Format("{0}, Location: '{1}' (assembly: '{2}', location: {3})", Kind, Location, assemblyName.GetNameSafe(), Assembly.Location);
    }

    public void Serialize (IMixinContextOriginSerializer serializer)
    {
      ArgumentUtility.CheckNotNull("serializer", serializer);

      serializer.AddKind(_kind);
      serializer.AddAssembly(_assembly);
      serializer.AddLocation(_location);
    }

    public bool Equals (MixinContextOrigin? other)
    {
      if (ReferenceEquals(null, other))
        return false;
      if (ReferenceEquals(this, other))
        return true;

      return
          GetType() == other.GetType()
          && Equals(other._kind, _kind)
          && Equals(other._assembly, _assembly)
          && Equals(other._location, _location);
    }

    public override bool Equals (object? obj)
    {
      return Equals(obj as MixinContextOrigin);
    }

    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode(_kind, _assembly, _location);
    }
  }
}
