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
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  public abstract class ClassDefinitionBase : IAttributableDefinition, IVisitableDefinition
  {
    private readonly UniqueDefinitionCollection<MethodInfo, MethodDefinition> _methods =
        new UniqueDefinitionCollection<MethodInfo, MethodDefinition> (m => m.MethodInfo);
    private readonly UniqueDefinitionCollection<PropertyInfo, PropertyDefinition> _properties =
        new UniqueDefinitionCollection<PropertyInfo, PropertyDefinition> (p => p.PropertyInfo);
    private readonly UniqueDefinitionCollection<EventInfo, EventDefinition> _events =
        new UniqueDefinitionCollection<EventInfo, EventDefinition> (p => p.EventInfo);
    private readonly MultiDefinitionCollection<Type, AttributeDefinition> _customAttributes =
        new MultiDefinitionCollection<Type, AttributeDefinition> (a => a.AttributeType);

    private readonly Type _type;
    private readonly HashSet<Type> _implementedInterfaces;

    protected ClassDefinitionBase (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      if (type.ContainsGenericParameters)
        throw new ArgumentException (string.Format ("The type {0} contains generic parameters, which is not allowed.", type), "type");
      _type = type;
      _implementedInterfaces = new HashSet<Type> (_type.GetInterfaces());
    }

    public Type Type
    {
      get { return _type; }
    }

    public string Name
    {
      get { return Type.Name; }
    }

    public string FullName
    {
      get {
        if (Type.IsGenericType)
          return Type.GetGenericTypeDefinition ().FullName;
        else
          return Type.FullName;
      }
    }

    /// <summary>
    /// Gets the result of <see cref="System.Type.GetInterfaceMap"/> for <see cref="Type"/>, adjusting the <see cref="MethodInfo"/> instances returned
    /// so that they match those stored in the <see cref="MethodDefinition"/> objects freturned by <see cref="Methods"/> and <see cref="GetAllMembers"/>.
    /// </summary>
    /// <param name="interfaceType">The interface to create a mapping for.</param>
    /// <remarks>When calling <see cref="System.Type.GetInterfaceMap"/>, the <see cref="MemberInfo.ReflectedType"/> properties of the returned 
    /// <see cref="MethodInfo"/> objects do not match those of the <see cref="MethodInfo"/> objects stored by this <see cref="ClassDefinitionBase"/>.
    /// This method remedies this by aligning the returned <see cref="MethodInfo"/> instances with the stored ones.</remarks>
    public InterfaceMapping GetAdjustedInterfaceMap(Type interfaceType)
    {
      InterfaceMapping mapping = Type.GetInterfaceMap (interfaceType);
      for (int i = 0; i < mapping.InterfaceMethods.Length; ++i)
      {
        MethodInfo targetMethod = mapping.TargetMethods[i];
        
        if (targetMethod.DeclaringType != Type)
        {
          // The MethodInfo objects returned by the Methods property has the ReflectedType == DeclaringType; the interface map must reflect this.
          Assertion.IsFalse (targetMethod.ReflectedType == targetMethod.DeclaringType);

          var newTargetMethod = (MethodInfo) MethodBase.GetMethodFromHandle (targetMethod.MethodHandle, targetMethod.DeclaringType.TypeHandle);
          Assertion.IsTrue (newTargetMethod.ReflectedType == newTargetMethod.DeclaringType);

          mapping.TargetMethods[i] = newTargetMethod;
        }
      }
      return mapping;
    }

    public abstract IVisitableDefinition Parent { get; }

    public HashSet<Type> ImplementedInterfaces
    {
      get { return _implementedInterfaces; }
    }

    public MultiDefinitionCollection<Type, AttributeDefinition> CustomAttributes
    {
      get { return _customAttributes; }
    }

    public ICustomAttributeProvider CustomAttributeProvider
    {
      get { return Type; }
    }

    public UniqueDefinitionCollection<MethodInfo, MethodDefinition> Methods
    {
      get { return _methods; }
    }

    public UniqueDefinitionCollection<PropertyInfo, PropertyDefinition> Properties
    {
      get { return _properties; }
    }

    public UniqueDefinitionCollection<EventInfo, EventDefinition> Events
    {
      get { return _events; }
    }

    public IEnumerable<MemberDefinitionBase> GetAllMembers()
    {
      foreach (MethodDefinition method in _methods)
        yield return method;
      foreach (PropertyDefinition property in _properties)
        yield return property;
      foreach (EventDefinition eventDefinition in _events)
        yield return eventDefinition;
    }

    public IEnumerable<MethodDefinition> GetAllMethods ()
    {
      foreach (MethodDefinition method in _methods)
        yield return method;

      foreach (PropertyDefinition property in _properties)
      {
        if (property.GetMethod != null)
          yield return property.GetMethod;
        if (property.SetMethod != null)
          yield return property.SetMethod;
      }

      foreach (EventDefinition eventDef in _events)
      {
        yield return eventDef.AddMethod;
        yield return eventDef.RemoveMethod;
      }
    }

    public void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      ChildSpecificAccept (visitor);

      _methods.Accept (visitor);
      _properties.Accept (visitor);
      _events.Accept (visitor);
      CustomAttributes.Accept (visitor);
    }

    protected abstract void ChildSpecificAccept (IDefinitionVisitor visitor);

    public bool HasOverriddenMembers ()
    {
      foreach (MemberDefinitionBase member in GetAllMembers ())
      {
        if (member.Overrides.Count > 0)
          return true;
      }
      return false;
    }

    public bool HasProtectedOverriders ()
    {
      return GetProtectedOverriders ().Any ();
    }

    public IEnumerable<MethodDefinition> GetProtectedOverriders ()
    {
      return from m in GetAllMethods()
             where m.Base != null && m.MethodInfo.IsFamily || m.MethodInfo.IsFamilyOrAssembly
             select m;
    }

    public override string ToString ()
    {
      return FullName;
    }
  }
}
