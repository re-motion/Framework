using System;
using System.Collections.Generic;
using System.Reflection;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

public class ValuePropertyInformation : IPropertyInformation
{
  public ValuePropertyInformation (Type propertyType)
  {
    ArgumentUtility.CheckNotNull(nameof(propertyType), propertyType);
    PropertyType = propertyType;
    DeclaringType = TypeAdapter.Create(propertyType);
  }

  public bool IsNull => false;
  public string Name => "Value";
  public ITypeInformation? DeclaringType { get; }
  public ITypeInformation? GetOriginalDeclaringType ()
  {
    return DeclaringType;
  }

  public T? GetCustomAttribute<T> (bool inherited)
      where T : class
  {
    return null;
  }

  public T[] GetCustomAttributes<T> (bool inherited)
      where T : class
  {
    return [];
  }

  public bool IsDefined<T> (bool inherited)
      where T : class
  {
    return false;
  }

  public Type PropertyType { get; }
  public bool CanBeSetFromOutside { get; }
  public object? GetValue (object? instance, object[]? indexParameters)
  {
    return instance;
  }

  public void SetValue (object? instance, object? value, object[]? indexParameters)
  {
    throw new NotSupportedException($"Cannot set the value of a {nameof(ValuePropertyInformation)}.");
  }

  public IMethodInformation? GetGetMethod (bool nonPublic)
  {
    throw new NotSupportedException($"Cannot get an accessor method for a {nameof(ValuePropertyInformation)}.");
  }

  public IMethodInformation? GetSetMethod (bool nonPublic)
  {
    throw new NotSupportedException($"Cannot get an accessor method for a {nameof(ValuePropertyInformation)}.");
  }

  public IPropertyInformation? FindInterfaceImplementation (Type implementationType)
  {
    return null;
  }

  public IEnumerable<IPropertyInformation> FindInterfaceDeclarations ()
  {
    return [];
  }

  public ParameterInfo[] GetIndexParameters ()
  {
    return [];
  }

  public IMethodInformation[] GetAccessors (bool nonPublic)
  {
    return [];
  }

  public IPropertyInformation GetOriginalDeclaration ()
  {
    return this;
  }
}
