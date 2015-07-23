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
using Remotion.Security;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.Security.UI
{
  public enum PermissionSource
  {
    SecurableObject,
    WxeFunction
  }

  [AttributeUsage (AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public abstract class DemandTargetPermissionAttribute : Attribute
  {
    // types

    // static members

    // member fields

    private PermissionSource _permissionSource;
    private Type _functionType;
    private string _methodName;
    private Type _securableClass;

    // construction and disposing

    protected DemandTargetPermissionAttribute (Type functionType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("functionType", functionType, typeof (WxeFunction));

      _permissionSource = PermissionSource.WxeFunction;
      _functionType = functionType;
    }

    protected DemandTargetPermissionAttribute (object methodEnum)
    {
      Enum enumValue = ArgumentUtility.CheckNotNullAndType<Enum> ("methodEnum", methodEnum);
      CheckDeclaringTypeOfMethodNameEnum (enumValue);

      _permissionSource = PermissionSource.SecurableObject;
      _securableClass = enumValue.GetType ().DeclaringType;
      _methodName = enumValue.ToString ();
    }

    protected DemandTargetPermissionAttribute (object methodEnum, Type securableClass)
    {
      Enum enumValue = ArgumentUtility.CheckNotNullAndType<Enum> ("methodEnum", methodEnum);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("securableClass", securableClass, typeof (ISecurableObject));

      CheckDeclaringTypeOfMethodNameEnum (enumValue, securableClass);

      _permissionSource = PermissionSource.SecurableObject;
      _securableClass = securableClass;
      _methodName = enumValue.ToString ();
    }

    protected DemandTargetPermissionAttribute (string methodName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);

      _permissionSource = PermissionSource.SecurableObject;
      _methodName = methodName;
    }

    protected DemandTargetPermissionAttribute (string methodName, Type securableClass)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("securableClass", securableClass, typeof (ISecurableObject));

      _permissionSource = PermissionSource.SecurableObject;
      _methodName = methodName;
      _securableClass = securableClass;
    }

    // methods and properties

    public PermissionSource PermissionSource
    {
      get { return _permissionSource; }
    }
	
    public Type FunctionType
    {
      get { return _functionType; }
    }

    public string MethodName
    {
      get { return _methodName; }
    }
  
    public Type SecurableClass
    {
      get { return _securableClass; }
    }

    protected void CheckDeclaringTypeOfMethodNameEnum (Enum methodNameEnum)
    {
      ArgumentUtility.CheckNotNull ("methodNameEnum", methodNameEnum);

      Type enumType = methodNameEnum.GetType ();

      if (enumType.DeclaringType == null)
        throw new ArgumentException (string.Format ("Enumerated type '{0}' is not declared as a nested type.", enumType.FullName), "methodNameEnum");

      if (!typeof (ISecurableObject).IsAssignableFrom (enumType.DeclaringType))
      {
        throw new ArgumentException (string.Format (
                "The declaring type of enumerated type '{0}' does not implement interface '{1}'.",
                enumType.FullName,
                typeof (ISecurableObject).FullName),
            "methodNameEnum");
      }
    }

    protected void CheckDeclaringTypeOfMethodNameEnum (Enum enumValue, Type securableClass)
    {
      CheckDeclaringTypeOfMethodNameEnum (enumValue);

      Type enumType = enumValue.GetType ();
      if (!enumType.DeclaringType.IsAssignableFrom (securableClass))
      {
        throw new ArgumentException (
            string.Format ("Type '{0}' cannot be assigned to the declaring type of enumerated type '{1}'.", securableClass, enumType),
            "securableClass");
      }
    }
  }
}
