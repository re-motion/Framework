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

namespace Remotion.Validation.UnitTests.TestHelpers
{
  public static class TypeUtility
  {
    public static Type CreateDynamicTypeWithCustomAttribute (
        Type classTypeToInstantiate, string className, Type attributeType, Type[] attrParams, object[] attrValues)
    {
      var dynamicAssembly = new AssemblyName();
      dynamicAssembly.Name = "DynamicAssembly";
      var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(dynamicAssembly, AssemblyBuilderAccess.Run);
      var moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");

      var typeBuilder = moduleBuilder.DefineType(className, TypeAttributes.Public | TypeAttributes.Class, classTypeToInstantiate);

      var attributeCtorInfo = attributeType.GetConstructor(attrParams);
      var attributeBuilder = new CustomAttributeBuilder(attributeCtorInfo, attrValues);
      typeBuilder.SetCustomAttribute(attributeBuilder);

      var collectorType = typeBuilder.CreateType();
      return collectorType;
    }
  }
}
