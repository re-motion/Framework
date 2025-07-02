// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Remotion.Data.DomainObjects.UnitTests.Tracing;

public class TracingTestHelper
{
  public static void AssertFinalizerImplemented (Type type)
  {
    var finalizer = type.GetMethod("Finalize", BindingFlags.Instance | BindingFlags.NonPublic);
    Assert.That(finalizer?.DeclaringType, Is.Not.EqualTo(typeof(object)));
  }

  public static void AssertNoFinalizerImplemented (Type type)
  {
    var finalizer = type.GetMethod("Finalize", BindingFlags.Instance | BindingFlags.NonPublic);
    Assert.That(finalizer?.DeclaringType, Is.EqualTo(typeof(object)));
  }

  public static void AssertAllVirtualMethodsOverridden (Type derivedType)
  {
    var baseType = derivedType.BaseType!;
    var virtualMethods = baseType.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic)
        .Where(m => m.IsVirtual && !m.IsFinal).ToList();

    foreach (var virtualMethod in virtualMethods)
    {
      var parameterTypes = virtualMethod.GetParameters().Select(p => p.ParameterType).ToArray();
      var overriddenMethod = derivedType.GetMethod(
          virtualMethod.Name,
          BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
          null,
          parameterTypes,
          null);
      Assert.That(
          overriddenMethod?.DeclaringType,
          Is.Not.Null.And.EqualTo(derivedType),
          $"Virtual method '{virtualMethod}' declared on base class was not re-implemented by derived type '{derivedType.Name}'. "
          + $"To ensure proper forwarding of the API calls to the decorated instance, all virtual members must be overridden and forward the call to the decorated instance.");
    }


    var virtualProperties = baseType.GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic)
        .Where(p => p.GetMethod is { IsVirtual: true, IsFinal: false } || p.SetMethod is { IsVirtual: true, IsFinal: false })
        .ToList();

    foreach (var virtualProperty in virtualProperties)
    {
      PropertyInfo overriddenProperty;
      if (virtualProperty.GetIndexParameters().Length > 0)
      {
        var parameterTypes = virtualProperty.GetIndexParameters().Select(p => p.ParameterType).ToArray();
        overriddenProperty = derivedType.GetProperty(
            virtualProperty.Name,
            BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null,
            virtualProperty.PropertyType,
            parameterTypes,
            null);
      }
      else
      {
        overriddenProperty = derivedType.GetProperty(
            virtualProperty.Name,
            BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      }

      Assert.That(
          overriddenProperty?.DeclaringType,
          Is.Not.Null.And.EqualTo(derivedType),
          $"Virtual property '{virtualProperty.Name}' declared on base class was not re-implemented by derived type '{derivedType.Name}'. "
          + $"To ensure proper forwarding of the API calls to the decorated instance, all virtual members must be overridden and forward the call to the decorated instance.");
    }
  }
}
