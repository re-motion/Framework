using System;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration
{
  /// <summary>
  /// Defines a base class for attributes that reference a method by declaring type, name, and signature. Cannot be used to reference a closed
  /// generic method.
  /// </summary>
  public abstract class MethodReferencingAttribute : Attribute
  {
    private readonly Type _declaringType;
    private readonly string _methodName;
    private readonly string _methodSignature;

    protected MethodReferencingAttribute (Type declaringType, string methodName, string methodSignature)
    {
      ArgumentUtility.CheckNotNull ("declaringType", declaringType);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      ArgumentUtility.CheckNotNullOrEmpty ("methodSignature", methodSignature);

      _declaringType = declaringType;
      _methodName = methodName;
      _methodSignature = methodSignature;
    }

    public Type DeclaringType
    {
      get { return _declaringType; }
    }

    public string MethodName
    {
      get { return _methodName; }
    }

    public string MethodSignature
    {
      get { return _methodSignature; }
    }

    public MethodInfo ResolveReferencedMethod ()
    {
      return MethodResolver.ResolveMethod (DeclaringType, MethodName, MethodSignature);
    }
  }
}