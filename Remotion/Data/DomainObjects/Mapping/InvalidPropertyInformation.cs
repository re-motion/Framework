using System;
using System.Collections.Generic;
using System.Reflection;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// Holds information about a mapping property that could not be resolved.
  /// </summary>
  public class InvalidPropertyInformation : IPropertyInformation
  {
    private readonly ITypeInformation _declaringType;
    private readonly string _name;
    private readonly Type _propertyType;

    public InvalidPropertyInformation (ITypeInformation declaringType, string name, Type propertyType)
    {
      ArgumentUtility.CheckNotNull ("declaringType", declaringType);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      _declaringType = declaringType;
      _name = name;
      _propertyType = propertyType;
    }

    public string Name
    {
      get { return _name; }
    }

    public ITypeInformation DeclaringType
    {
      get { return _declaringType; }
    }

    public ITypeInformation GetOriginalDeclaringType ()
    {
      return _declaringType;
    }

    public IPropertyInformation GetOriginalDeclaration ()
    {
      return this;
    }

    public T GetCustomAttribute<T> (bool inherited) where T : class
    {
      return null;
    }

    public T[] GetCustomAttributes<T> (bool inherited) where T : class
    {
      return new T[] { };
    }

    public bool IsDefined<T> (bool inherited) where T : class
    {
      return false;
    }

    public Type PropertyType
    {
      get { return _propertyType; }
    }

    public bool CanBeSetFromOutside
    {
      get { return false; }
    }

    public object GetValue (object instance, object[] indexParameters)
    {
      return null;
    }

    public void SetValue (object instance, object value, object[] indexParameters)
    {
    }

    public IMethodInformation GetGetMethod (bool nonPublic)
    {
      return null;
    }

    public IMethodInformation GetSetMethod (bool nonPublic)
    {
      return null;
    }

    public IPropertyInformation FindInterfaceImplementation (Type implementationType)
    {
      throw new InvalidOperationException ("FindInterfaceImplementation can only be called on inteface properties.");
    }

    public IEnumerable<IPropertyInformation> FindInterfaceDeclarations ()
    {
      return null;
    }

    public ParameterInfo[] GetIndexParameters ()
    {
      return new ParameterInfo[0];
    }

    public IMethodInformation[] GetAccessors (bool nonPublic)
    {
      return new IMethodInformation[0];
    }

    public override bool Equals (object obj)
    {
      return ReferenceEquals (this, obj);
    }

    public override int GetHashCode ()
    {
      return base.GetHashCode();
    }

    public override string ToString ()
    {
      return string.Format ("{0} (invalid property)", _name);
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}