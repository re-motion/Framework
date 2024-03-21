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
  /// Walks through the given <see cref="TypeDefinition"/>'s hierarchy and executes a handler based on the entity's type.
  /// </summary>
  public static class InlineTypeDefinitionWalker
  {
    private class InlineTypeDefinitionWalkerImplementation : TypeDefinitionWalker
    {
      private readonly Action<ClassDefinition> _classDefinitionHandler;
      private readonly Action<InterfaceDefinition> _interfaceDefinitionHandler;

      public InlineTypeDefinitionWalkerImplementation (
          TypeDefinitionWalkerDirection direction,
          Action<ClassDefinition> classDefinitionHandler,
          Action<InterfaceDefinition> interfaceDefinitionHandler)
          : base(direction)
      {
        _classDefinitionHandler = classDefinitionHandler;
        _interfaceDefinitionHandler = interfaceDefinitionHandler;
      }

      /// <inheritdoc />
      public override void VisitClassDefinition (ClassDefinition classDefinition)
      {
        ArgumentUtility.CheckNotNull("classDefinition", classDefinition);

        _classDefinitionHandler(classDefinition);
        base.VisitClassDefinition(classDefinition);
      }

      /// <inheritdoc />
      public override void VisitInterfaceDefinition (InterfaceDefinition interfaceDefinition)
      {
        ArgumentUtility.CheckNotNull("interfaceDefinition", interfaceDefinition);

        _interfaceDefinitionHandler(interfaceDefinition);
        base.VisitInterfaceDefinition(interfaceDefinition);
      }
    }

    private class InlineTypeDefinitionWalkerImplementation<T> : TypeDefinitionWalker<T>
    {
      private readonly Func<ClassDefinition, T?> _classDefinitionHandler;
      private readonly Func<InterfaceDefinition, T?> _interfaceDefinitionHandler;

      public InlineTypeDefinitionWalkerImplementation (
          TypeDefinitionWalkerDirection direction,
          Func<ClassDefinition, T?> classDefinitionHandler,
          Func<InterfaceDefinition, T?> interfaceDefinitionHandler,
          Predicate<T?> match)
          : base(direction, match)
      {
        _classDefinitionHandler = classDefinitionHandler;
        _interfaceDefinitionHandler = interfaceDefinitionHandler;
      }

      /// <inheritdoc />
      public override T? VisitClassDefinition (ClassDefinition classDefinition)
      {
        ArgumentUtility.CheckNotNull("classDefinition", classDefinition);

        var result = _classDefinitionHandler(classDefinition);
        if (Match(result))
          return result;

        return base.VisitClassDefinition(classDefinition);
      }

      /// <inheritdoc />
      public override T? VisitInterfaceDefinition (InterfaceDefinition interfaceDefinition)
      {
        ArgumentUtility.CheckNotNull("interfaceDefinition", interfaceDefinition);

        var result = _interfaceDefinitionHandler(interfaceDefinition);
        if (Match(result))
          return result;

        return base.VisitInterfaceDefinition(interfaceDefinition);
      }
    }

    /// <summary>
    /// Walks over the specified <paramref name="typeDefinition"/> and its ancestors, executing the specified handlers for each element.
    /// </summary>
    public static void WalkAncestors (
        TypeDefinition typeDefinition,
        Action<ClassDefinition> classDefinitionHandler,
        Action<InterfaceDefinition> interfaceDefinitionHandler)
    {
      ArgumentUtility.CheckNotNull(nameof(typeDefinition), typeDefinition);
      ArgumentUtility.CheckNotNull(nameof(classDefinitionHandler), classDefinitionHandler);
      ArgumentUtility.CheckNotNull(nameof(interfaceDefinitionHandler), interfaceDefinitionHandler);

      var inlineTypeDefinitionWalker = new InlineTypeDefinitionWalkerImplementation(TypeDefinitionWalkerDirection.Ascendants, classDefinitionHandler, interfaceDefinitionHandler);
      typeDefinition.Accept(inlineTypeDefinitionWalker);
    }

    /// <summary>
    /// Walks over the specified <paramref name="typeDefinition"/> and its ancestors, executing the specified handlers for each element
    /// until <paramref name="match"/> returns <see langword="true"/> for one of the handler's results.
    /// </summary>
    public static T? WalkAncestors<T> (
        TypeDefinition typeDefinition,
        Func<ClassDefinition, T?> classDefinitionHandler,
        Func<InterfaceDefinition, T?> interfaceDefinitionHandler,
        Predicate<T?> match)
    {
      ArgumentUtility.CheckNotNull(nameof(typeDefinition), typeDefinition);
      ArgumentUtility.CheckNotNull(nameof(classDefinitionHandler), classDefinitionHandler);
      ArgumentUtility.CheckNotNull(nameof(interfaceDefinitionHandler), interfaceDefinitionHandler);
      ArgumentUtility.CheckNotNull(nameof(match), match);

      var inlineTypeDefinitionWalker = new InlineTypeDefinitionWalkerImplementation<T>(
          TypeDefinitionWalkerDirection.Ascendants,
          classDefinitionHandler,
          interfaceDefinitionHandler,
          match);
      return typeDefinition.Accept(inlineTypeDefinitionWalker);
    }

    /// <summary>
    /// Walks over the specified <paramref name="typeDefinition"/> and its descendants, executing the specified handlers for each element.
    /// </summary>
    public static void WalkDescendants (
        TypeDefinition typeDefinition,
        Action<ClassDefinition> classDefinitionHandler,
        Action<InterfaceDefinition> interfaceDefinitionHandler)
    {
      ArgumentUtility.CheckNotNull(nameof(typeDefinition), typeDefinition);
      ArgumentUtility.CheckNotNull(nameof(classDefinitionHandler), classDefinitionHandler);
      ArgumentUtility.CheckNotNull(nameof(interfaceDefinitionHandler), interfaceDefinitionHandler);

      var inlineTypeDefinitionWalker = new InlineTypeDefinitionWalkerImplementation(TypeDefinitionWalkerDirection.Descendants, classDefinitionHandler, interfaceDefinitionHandler);
      typeDefinition.Accept(inlineTypeDefinitionWalker);
    }

    /// <summary>
    /// Walks over the specified <paramref name="typeDefinition"/> and its descendants, executing the specified handlers for each element
    /// until <paramref name="match"/> returns <see langword="true"/> for one of the handler's results.
    /// </summary>
    public static T? WalkDescendants<T> (
        TypeDefinition typeDefinition,
        Func<ClassDefinition, T?> classDefinitionHandler,
        Func<InterfaceDefinition, T?> interfaceDefinitionHandler,
        Predicate<T?> match)
    {
      ArgumentUtility.CheckNotNull(nameof(typeDefinition), typeDefinition);
      ArgumentUtility.CheckNotNull(nameof(classDefinitionHandler), classDefinitionHandler);
      ArgumentUtility.CheckNotNull(nameof(interfaceDefinitionHandler), interfaceDefinitionHandler);
      ArgumentUtility.CheckNotNull(nameof(match), match);

      var inlineTypeDefinitionWalker = new InlineTypeDefinitionWalkerImplementation<T>(
          TypeDefinitionWalkerDirection.Descendants,
          classDefinitionHandler,
          interfaceDefinitionHandler,
          match);
      return typeDefinition.Accept(inlineTypeDefinitionWalker);
    }
  }
}
