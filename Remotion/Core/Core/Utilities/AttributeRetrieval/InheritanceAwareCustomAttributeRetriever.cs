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
using System.Linq.Expressions;
using System.Reflection;

namespace Remotion.Utilities.AttributeRetrieval
{
  // Note: This class is currently only tested integratively, via AttributeUtility. When changing it, consider adding tests specifically for this 
  // class.
  /// <summary>
  /// Implements a generic custom attribute retrieval algorithm that knows how to deal with inheritance.
  /// </summary>
  /// <typeparam name="TCustomAttributeProvider">The type of <see cref="ICustomAttributeProvider"/> whose attributes should be retrieved.</typeparam>
  public abstract class InheritanceAwareCustomAttributeRetriever<TCustomAttributeProvider>
      where TCustomAttributeProvider : class, ICustomAttributeProvider
  {
    // Note: We're calling MethodInfo.GetParentDefinition via Reflection here for best performance. If this method is ever removed, we'll need to
    // find a different way of getting the parent method. Note that finding it by iterating base methods and comparing base definitions is too slow.
    // Maybe the implementation of the Attribute class has an idea?
    // ReSharper disable StaticFieldInGenericType - yes, the field will exist once per instantiation, but we don't care.
    private static readonly DoubleCheckedLockingContainer<Func<MethodInfo, MethodInfo>> s_getMethodParentDefinition =
    // ReSharper restore StaticFieldInGenericType
        new DoubleCheckedLockingContainer<Func<MethodInfo, MethodInfo>> (
            () =>
            {
              var runtimeMethodInfoType = Type.GetType ("System.Reflection.RuntimeMethodInfo");
              Assertion.IsNotNull (runtimeMethodInfoType, "The internal type RuntimeMethodInfo has been removed. We need to patch this implementation.");
             
              var method = runtimeMethodInfoType.GetMethod (
                  "GetParentDefinition",
                  BindingFlags.Instance | BindingFlags.NonPublic,
                  null,
                  Type.EmptyTypes,
                  null);
              Assertion.IsNotNull (method, "The internal method RuntimeMethodInfo.GetParentDefinition has been removed. We need to patch this implementation.");

              var parameterExpression = Expression.Parameter (typeof (MethodInfo));
              var lambdaExpression = Expression.Lambda (
                  Expression.Convert (
                      Expression.Call (
                          Expression.Convert (parameterExpression, runtimeMethodInfoType), method),
                      typeof (MethodInfo)),
                  parameterExpression);

              return (Func<MethodInfo, MethodInfo>) lambdaExpression.Compile();
            });

    protected abstract TCustomAttributeProvider GetBaseMember (TCustomAttributeProvider memberInfo);

    public object[] GetCustomAttributes (TCustomAttributeProvider memberInfo, Type attributeType, bool inherit)
    {
      ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);
      ArgumentUtility.CheckNotNull ("attributeType", attributeType);

      var declaredAttributes = memberInfo.GetCustomAttributes (attributeType, false);
      if (!inherit)
        return declaredAttributes;

      var allAttributes = new ArrayList ();
      var attributeUsagesPerType = new Dictionary<Type, AttributeUsageAttribute> ();
      foreach (var declaredAttribute in declaredAttributes)
      {
        allAttributes.Add (declaredAttribute);
        var declaredAttributeType = declaredAttribute.GetType ();
        if (!attributeUsagesPerType.ContainsKey (declaredAttributeType))
          attributeUsagesPerType.Add (declaredAttributeType, AttributeUtility.GetAttributeUsage (declaredAttributeType));
      }

      var currentMember = GetBaseMember (memberInfo);
      while (currentMember != null)
      {
        AddInheritedAttributes (allAttributes, currentMember, attributeType, attributeUsagesPerType);
        currentMember = GetBaseMember (currentMember);
      }
      return (object[]) allAttributes.ToArray (attributeType);
    }

    protected static MethodInfo GetBaseMethod (MethodInfo methodInfo)
    {
      return s_getMethodParentDefinition.Value (methodInfo);
    }

    private void AddInheritedAttributes (ArrayList allAttributes, TCustomAttributeProvider baseMember, Type attributeType, Dictionary<Type, AttributeUsageAttribute> visitedAttributeTypes)
    {
      var currentAttributes = baseMember.GetCustomAttributes (attributeType, false);
      foreach (var currentAttribute in currentAttributes)
      {
        var currentAttributeType = currentAttribute.GetType ();
        AttributeUsageAttribute attributeUsage;
        if (!visitedAttributeTypes.TryGetValue (currentAttributeType, out attributeUsage))
        {
          attributeUsage = AttributeUtility.GetAttributeUsage (currentAttributeType);
          visitedAttributeTypes.Add (currentAttributeType, attributeUsage);
          if (attributeUsage.Inherited)
            allAttributes.Add (currentAttribute);
        }
        else if (attributeUsage.Inherited && attributeUsage.AllowMultiple)
        {
          allAttributes.Add (currentAttribute);
        }
      }
    }
  }
}