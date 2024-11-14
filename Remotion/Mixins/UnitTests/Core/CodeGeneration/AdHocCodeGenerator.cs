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
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration
{
  /// <summary>
  /// Allows tests to generate code into an <see cref="TypeBuilder"/> without having to care about defining the 
  /// <see cref="System.Reflection.Emit.AssemblyBuilder"/>, etc.
  /// </summary>
  public class AdHocCodeGenerator
  {
    private readonly AssemblyBuilder _assemblyBuilder;
    private readonly ModuleBuilder _moduleBuilder;
    private readonly string _filename;

    private int _typeCounter;

    public AdHocCodeGenerator (string assemblyDirectory, string assemblyName = "AdHocCodeGenerator")
    {
      ArgumentUtility.CheckNotNullOrEmpty("assemblyName", assemblyName);

      _filename = assemblyName + ".dll";
      _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
      _moduleBuilder = _assemblyBuilder.DefineDynamicModule(_filename);
    }

    public AssemblyBuilder AssemblyBuilder
    {
      get { return _assemblyBuilder; }
    }

    public ModuleBuilder ModuleBuilder
    {
      get { return _moduleBuilder; }
    }

    public TypeBuilder CreateType (string typeName = null, Type baseType = null)
    {
      typeName = typeName ?? ("Test_" + _typeCounter++);

      return _moduleBuilder.DefineType(typeName, TypeAttributes.Public, baseType);
    }

    public Tuple<TypeBuilder, MethodBuilder> CreateMethod (
        string typeName = null,
        string methodName = null,
        MethodAttributes methodAttributes = MethodAttributes.Public | MethodAttributes.Static,
        Type returnType = null,
        Type[] parameterTypes = null,
        Action<MethodBuilder> action = null)
    {
      var typeBuilder = CreateType(typeName);

      var methodBuilder = CreateMethod(typeBuilder, methodName, methodAttributes, returnType, parameterTypes, action);

      return Tuple.Create(typeBuilder, methodBuilder);
    }

    public MethodBuilder CreateMethod (
        TypeBuilder typeBuilder,
        string methodName = null,
        MethodAttributes methodAttributes = MethodAttributes.Public | MethodAttributes.Static,
        Type returnType = null,
        Type[] parameterTypes = null,
        Action<MethodBuilder> action = null)
    {
      methodName = methodName ?? ("Test_" + Guid.NewGuid().ToString().Replace("-", ""));
      returnType = returnType ?? typeof(void);
      parameterTypes = parameterTypes ?? Type.EmptyTypes;

      var methodBuilder = typeBuilder.DefineMethod(methodName, methodAttributes, returnType, parameterTypes);
      if (action != null)
        action(methodBuilder);
      return methodBuilder;
    }

    public T CreateMethodAndRun<T> (
        string typeName = null,
        MethodAttributes methodAttributes = MethodAttributes.Public | MethodAttributes.Static,
        string methodName = null,
        Action<MethodBuilder> action = null,
        bool saveOnError = false)
    {
      var returnType = typeof(T);
      var parameterTypes = Type.EmptyTypes;

      var tuple = CreateMethod(typeName, methodName, methodAttributes, returnType, parameterTypes, action);
      var actualType = tuple.Item1.CreateType();

      try
      {
        return (T)actualType.InvokeMember(tuple.Item2.Name, BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, null);
      }
      catch (Exception)
      {
        if (saveOnError)
          Console.WriteLine(Save());
        throw;
      }
    }

    public string Save ()
    {
      if (File.Exists(_filename))
      {
        try
        {
          File.Delete(_filename);
        }
        catch (UnauthorizedAccessException)
        {
          Assert.Fail(
              $"Assembly '{_filename}' already exists, likely because it could not be properly cleaned during a previous test-run. "
              + "Please delete the bin-directory manually and re-run the tests.");
        }
      }
      // TODO RM-9285: save generated assembly for use with IL verification.
      return _moduleBuilder.FullyQualifiedName;
    }

    public void AddCustomAttribute (Type type)
    {
      var customAttributeBuilder = CreateCustomAttributeBuilder(type);
      _assemblyBuilder.SetCustomAttribute(customAttributeBuilder);
    }

    public CustomAttributeBuilder CreateCustomAttributeBuilder (Type type)
    {
      var constructorInfo = type.GetConstructor(Type.EmptyTypes);
      Assertion.IsNotNull(constructorInfo, "Type must have a public default constructor.");
      var customAttributeBuilder = new CustomAttributeBuilder(constructorInfo, new object[0]);
      return customAttributeBuilder;
    }
  }
}
