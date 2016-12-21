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
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration
{
  public enum PropertyKind
  {
    Static,
    Instance
  }

  public class CustomPropertyEmitter : IAttributableEmitter
  {
    private readonly CustomClassEmitter _declaringType;
    private readonly PropertyBuilder _propertyBuilder;

    private readonly Type _propertyType;
    private readonly string _name;
    private readonly PropertyKind _propertyKind;
    private readonly Type[] _indexParameters;

    private IMethodEmitter _getMethod;
    private IMethodEmitter _setMethod;

    public CustomPropertyEmitter (CustomClassEmitter declaringType, string name, PropertyKind propertyKind, Type propertyType, Type[] indexParameters, PropertyAttributes attributes)
    {
      ArgumentUtility.CheckNotNull ("declaringType", declaringType);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNull ("propertyType", propertyType);
      ArgumentUtility.CheckNotNull ("indexParameters", indexParameters);

      _declaringType = declaringType;
      _name = name;
      _propertyKind = propertyKind;

      _propertyType = propertyType;
      _indexParameters = indexParameters;
    
      CallingConventions callingConvention = propertyKind == PropertyKind.Instance ? CallingConventions.HasThis : CallingConventions.Standard;
      _propertyBuilder = _declaringType.TypeBuilder.DefineProperty (
           name, attributes, callingConvention, propertyType, null, null, indexParameters, null, null);
    }

    public Type PropertyType
    {
      get { return _propertyType; }
    }

    public Type[] IndexParameters
    {
      get { return _indexParameters; }
    }

    public IMethodEmitter GetMethod
    {
      get { return _getMethod; }
      set
      {
        if (value != null)
        {
          _getMethod = value;
          _propertyBuilder.SetGetMethod (_getMethod.MethodBuilder);
        }
        else
          throw new ArgumentNullException ("value", "Due to limitations in Reflection.Emit, property accessors cannot be set to null.");
      }
    }

    public IMethodEmitter SetMethod
    {
      get { return _setMethod; }
      set
      {
        if (value != null)
        {
          _setMethod = value;
          _propertyBuilder.SetSetMethod (_setMethod.MethodBuilder);
        }
        else
          throw new ArgumentNullException ("value", "Due to limitations in Reflection.Emit, property accessors cannot be set to null.");
      }
    }

    public string Name
    {
      get { return _name; }
    }

    public PropertyKind PropertyKind
    {
      get { return _propertyKind; }
    }

    public CustomClassEmitter DeclaringType
    {
      get { return _declaringType; }
    }

    public PropertyBuilder PropertyBuilder
    {
      get { return _propertyBuilder; }
    }

    public CustomPropertyEmitter ImplementWithBackingField ()
    {
      string fieldName = MakeBackingFieldName (Name);
      FieldReference backingField;
      if (PropertyKind == PropertyKind.Static)
        backingField = _declaringType.CreateStaticField (fieldName, PropertyType);
      else
        backingField = _declaringType.CreateField (fieldName, PropertyType);
      return ImplementWithBackingField (backingField);
    }

    private static string MakeBackingFieldName (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      return "_fieldFor" + propertyName;
    }

    public CustomPropertyEmitter ImplementWithBackingField (FieldReference backingField)
    {
      ArgumentUtility.CheckNotNull ("backingField", backingField);
      if (GetMethod != null)
        GetMethod.AddStatement (new ReturnStatement (backingField));
      if (SetMethod != null)
      {
        SetMethod.AddStatement (
            new AssignStatement (backingField, SetMethod.ArgumentReferences[IndexParameters.Length].ToExpression()));
        SetMethod.ImplementByReturningVoid();
      }
      return this;
    }

    // TODO FS: Test for explicit interface implementation
    public IMethodEmitter CreateGetMethod ()
    {
      if (GetMethod != null)
        throw new InvalidOperationException ("This property already has a getter method.");
      else
      {
        MethodAttributes flags = MethodAttributes.Public | MethodAttributes.SpecialName;
        if (PropertyKind == PropertyKind.Static)
          flags |= MethodAttributes.Static;

        IMethodEmitter method = _declaringType.CreateMethod (BuildAccessorMethodName (Name, "get"), flags, PropertyType, IndexParameters);

        GetMethod = method;
        return method;
      }
    }

    // TODO FS: Test
    public static string BuildAccessorMethodName(string propertyName, string accessorName)
    {
      string s = propertyName;
      string sPath = "";
      string sPureName = s;
      int iSplit = s.LastIndexOf ('.');
      if (iSplit >= 0)
      {
        sPath = s.Substring (0, iSplit) + ".";
        sPureName = s.Substring (iSplit + 1, s.Length - iSplit - 1);
      }
      return sPath + accessorName + "_" + sPureName;
    }

    // TODO FS: Test for explicit interface implementation
    public IMethodEmitter CreateSetMethod ()
    {
      if (SetMethod != null)
        throw new InvalidOperationException ("This property already has a setter method.");
      else
      {
        MethodAttributes flags = MethodAttributes.Public | MethodAttributes.SpecialName;
        if (PropertyKind == PropertyKind.Static)
          flags |= MethodAttributes.Static;

        Type[] setterParameterTypes = new Type[IndexParameters.Length + 1];
        IndexParameters.CopyTo (setterParameterTypes, 0);
        setterParameterTypes[IndexParameters.Length] = PropertyType;

        IMethodEmitter method = _declaringType.CreateMethod (BuildAccessorMethodName (Name,"set"), flags, typeof (void), setterParameterTypes);

        SetMethod = method;
        return method;
      }
    }

    public void AddCustomAttribute (CustomAttributeBuilder customAttribute)
    {
      ArgumentUtility.CheckNotNull ("customAttribute", customAttribute);
      _propertyBuilder.SetCustomAttribute (customAttribute);
    }

  }
}
