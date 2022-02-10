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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// Visits the given <see cref="TypeDefinition"/> and executes a handler based on the entity's type.
  /// </summary>
  public static class InlineTypeDefinitionVisitor
  {
    private class TypeDefinitionVisitor<T> : ITypeDefinitionVisitor<T>
    {
      private readonly Func<ClassDefinition, T?> _classDefinitionHandler;
      private readonly Func<InterfaceDefinition, T?> _interfaceDefinitionHandler;

      public TypeDefinitionVisitor (
          Func<ClassDefinition, T?> classDefinitionHandler,
          Func<InterfaceDefinition, T?> interfaceDefinitionHandler)
      {
        _classDefinitionHandler = classDefinitionHandler;
        _interfaceDefinitionHandler = interfaceDefinitionHandler;
      }

      /// <inheritdoc />
      public T? VisitClassDefinition (ClassDefinition classDefinition)
      {
        ArgumentUtility.CheckNotNull("classDefinition", classDefinition);

        return _classDefinitionHandler(classDefinition);
      }

      public T? VisitInterfaceDefinition (InterfaceDefinition interfaceDefinition)
      {
        ArgumentUtility.CheckNotNull("interfaceDefinition", interfaceDefinition);

        return _interfaceDefinitionHandler(interfaceDefinition);
      }
    }

    private class TypeDefinitionVisitor : ITypeDefinitionVisitor
    {
      private readonly Action<ClassDefinition> _classDefinitionHandler;
      private readonly Action<InterfaceDefinition> _interfaceDefinitionHandler;

      public TypeDefinitionVisitor (
          Action<ClassDefinition> classDefinitionHandler,
          Action<InterfaceDefinition> interfaceDefinitionHandler)
      {
        _classDefinitionHandler = classDefinitionHandler;
        _interfaceDefinitionHandler = interfaceDefinitionHandler;
      }

      /// <inheritdoc />
      public void VisitClassDefinition (ClassDefinition classDefinition)
      {
        ArgumentUtility.CheckNotNull("classDefinition", classDefinition);

        _classDefinitionHandler(classDefinition);
      }

      /// <inheritdoc />
      public void VisitInterfaceDefinition (InterfaceDefinition interfaceDefinition)
      {
        ArgumentUtility.CheckNotNull("interfaceDefinition", interfaceDefinition);

        _interfaceDefinitionHandler(interfaceDefinition);
      }
    }

    public static T? Visit<T> (
        TypeDefinition typeDefinition,
        Func<ClassDefinition, T> classDefinitionHandler,
        Func<InterfaceDefinition, T> interfaceDefinitionHandler)
    {
      ArgumentUtility.CheckNotNull(nameof(typeDefinition), typeDefinition);
      ArgumentUtility.CheckNotNull(nameof(classDefinitionHandler), classDefinitionHandler);
      ArgumentUtility.CheckNotNull(nameof(interfaceDefinitionHandler), interfaceDefinitionHandler);

      var visitor = new TypeDefinitionVisitor<T>(classDefinitionHandler, interfaceDefinitionHandler);
      return typeDefinition.Accept(visitor);
    }

    public static void Visit (
        TypeDefinition typeDefinition,
        Action<ClassDefinition> classDefinitionHandler,
        Action<InterfaceDefinition> interfaceDefinitionHandler)
    {
      ArgumentUtility.CheckNotNull(nameof(typeDefinition), typeDefinition);
      ArgumentUtility.CheckNotNull(nameof(classDefinitionHandler), classDefinitionHandler);
      ArgumentUtility.CheckNotNull(nameof(interfaceDefinitionHandler), interfaceDefinitionHandler);

      var visitor = new TypeDefinitionVisitor(classDefinitionHandler, interfaceDefinitionHandler);
      typeDefinition.Accept(visitor);
    }
  }
}
