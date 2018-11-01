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
using System.Reflection;
using Remotion.Utilities;
using ReflectionUtility = Remotion.Mixins.Utilities.ReflectionUtility;

namespace Remotion.Mixins.Definitions.Building
{
  public class MemberDefinitionBuilder
  {
    private const BindingFlags c_bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private readonly ClassDefinitionBase _classDefinition;
    private readonly Predicate<MethodInfo> _methodFilter;
    private readonly HashSet<MethodInfo> _specialMethods = new HashSet<MethodInfo> ();

    public MemberDefinitionBuilder (ClassDefinitionBase classDefinition, Predicate<MethodInfo> methodFilter)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("methodFilter", methodFilter);

      _classDefinition = classDefinition;
      _methodFilter = methodFilter;
    }

    public void Apply (Type type)
    {
      IEnumerable<MethodInfo> methods = ReflectionUtility.RecursiveGetAllMethods (type, c_bindingFlags);
      IEnumerable<PropertyInfo> properties = ReflectionUtility.RecursiveGetAllProperties (type, c_bindingFlags);
      IEnumerable<EventInfo> events = ReflectionUtility.RecursiveGetAllEvents (type, c_bindingFlags);

      var overriddenMemberFilter = new OverriddenMemberFilter ();

      methods = overriddenMemberFilter.RemoveOverriddenMembers (methods);
      properties = overriddenMemberFilter.RemoveOverriddenMembers (properties);
      events = overriddenMemberFilter.RemoveOverriddenMembers (events);

      AnalyzeProperties (properties);
      AnalyzeEvents (events);
      AnalyzeMethods (methods);
    }

    private void AnalyzeProperties (IEnumerable<PropertyInfo> properties)
    {
      foreach (PropertyInfo property in properties)
      {
        MethodInfo getMethod = property.GetGetMethod (true);
        MethodInfo setMethod = property.GetSetMethod (true);

        MethodDefinition getMethodDefinition = CreateSpecialMethodDefinition (getMethod);
        MethodDefinition setMethodDefinition = CreateSpecialMethodDefinition (setMethod);

        if (getMethodDefinition != null || setMethodDefinition != null)
        {
          var definition = new PropertyDefinition (property, _classDefinition, getMethodDefinition, setMethodDefinition);
          var attributeBuilder = new AttributeDefinitionBuilder (definition);
          attributeBuilder.Apply (property);
          _classDefinition.Properties.Add (definition);
        }
      }
    }

    private void AnalyzeEvents (IEnumerable<EventInfo> events)
    {
      foreach (EventInfo eventInfo in events)
      {
        MethodInfo addMethod = eventInfo.GetAddMethod (true);
        MethodInfo removeMethod = eventInfo.GetRemoveMethod (true);

        MethodDefinition addMethodDefinition = CreateSpecialMethodDefinition (addMethod);
        MethodDefinition removeMethodDefinition = CreateSpecialMethodDefinition (removeMethod);

        if (addMethodDefinition != null || removeMethodDefinition != null)
        {
          var definition = new EventDefinition (eventInfo, _classDefinition, addMethodDefinition, removeMethodDefinition);
          var attributeBuilder = new AttributeDefinitionBuilder (definition);
          attributeBuilder.Apply (eventInfo);
          _classDefinition.Events.Add (definition);
        }
      }
    }

    private void AnalyzeMethods (IEnumerable<MethodInfo> methods)
    {
      foreach (MethodInfo method in methods)
      {
        if (!_specialMethods.Contains (method) && _methodFilter (method))
        {
          var definition = new MethodDefinition (method, _classDefinition);
          var attributeBuilder = new AttributeDefinitionBuilder (definition);
          attributeBuilder.Apply (method);
          _classDefinition.Methods.Add (definition);
        }
      }
    }

    private MethodDefinition CreateSpecialMethodDefinition (MethodInfo methodInfo)
    {
      if (methodInfo != null && _methodFilter (methodInfo))
      {
        var methodDefinition = new MethodDefinition (methodInfo, _classDefinition);
        var attributeBuilder = new AttributeDefinitionBuilder (methodDefinition);
        attributeBuilder.Apply (methodInfo);
        _specialMethods.Add (methodInfo);
        return methodDefinition;
      }
      else
        return null;
    }
  }
}
