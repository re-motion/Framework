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
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  public class PropertyReference : TypeReference
  {
    private readonly PropertyInfo _property;

    public PropertyReference (PropertyInfo property)
        : base (SelfReference.Self, property.PropertyType)
    {
      _property = property;
    }

    public PropertyReference(Reference owner, PropertyInfo property)
        : base (owner, property.PropertyType)
    {
      _property = property;
    }

    public PropertyInfo Reference
    {
      get { return _property; }
    }

    public override void LoadAddressOfReference (ILGenerator gen)
    {
      throw new NotSupportedException ("A property's address cannot be loaded.");
    }

    public override void LoadReference (ILGenerator gen)
    {
      MethodInfo getMethod = Reference.GetGetMethod(true);
      if (getMethod == null)
      {
        string message = string.Format("The property {0}.{1} cannot be loaded, it has no getter.", Reference.DeclaringType.FullName, Reference.Name);
        throw new InvalidOperationException (message);
      }
      if (getMethod.IsStatic)
      {
        gen.EmitCall (OpCodes.Call, getMethod, null);
      }
      else
      {
        gen.EmitCall (OpCodes.Callvirt, getMethod, null);
      }
    }

    public override void StoreReference (ILGenerator gen)
    {
      MethodInfo setMethod = Reference.GetSetMethod (true);
      if (setMethod == null)
      {
        string message = string.Format ("The property {0}.{1} cannot be stored, it has no setter.", Reference.DeclaringType.FullName, Reference.Name);
        throw new InvalidOperationException (message);
      }
      if (setMethod.IsStatic)
      {
        gen.EmitCall (OpCodes.Call, setMethod, null);
      }
      else
      {
        gen.EmitCall (OpCodes.Callvirt, setMethod, null);
      }
    }
  }
}
