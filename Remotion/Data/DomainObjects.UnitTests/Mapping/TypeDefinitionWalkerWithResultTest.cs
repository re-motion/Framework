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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  public class TypeDefinitionWalkerWithResultTest : TypeDefinitionWalkerTestBase
  {
    private class TestableTypeDefinitionWalker<T> : TypeDefinitionWalker<T>
        where T : class
    {
      public Func<ClassDefinition, T> ClassDefinitionHandler { get; set; } = _ => null;

      public Func<InterfaceDefinition, T> InterfaceDefinitionHandler { get; set; } = _ => null;

      public List<TypeDefinition> VisitedTypeDefinitions { get; } = new();

      public TestableTypeDefinitionWalker (TypeDefinitionWalkerDirection direction, Predicate<T> match)
          : base(direction, match)
      {
      }

      /// <inheritdoc />
      public override T VisitClassDefinition (ClassDefinition classDefinition)
      {
        VisitedTypeDefinitions.Add(classDefinition);

        var result = ClassDefinitionHandler(classDefinition);
        if (Match(result))
          return result;

        return base.VisitClassDefinition(classDefinition);
      }

      /// <inheritdoc />
      public override T VisitInterfaceDefinition (InterfaceDefinition interfaceDefinition)
      {
        VisitedTypeDefinitions.Add(interfaceDefinition);

        var result = InterfaceDefinitionHandler(interfaceDefinition);
        if (Match(result))
          return result;

        return base.VisitInterfaceDefinition(interfaceDefinition);
      }
    }

    [Test]
    public void VisitClassDefinition_WithChildrenDirection_IsCalledInCorrectOrder ()
    {
      var (domainBase, person, customer, organizationalUnit) = TypeDefinitionWalkerTest.CreateClassTestDomain();

      var walker = new TestableTypeDefinitionWalker<string>(TypeDefinitionWalkerDirection.Descendants, value => value != null);
      Assert.That(walker.VisitClassDefinition(domainBase), Is.Null);

      Assert.That(walker.VisitedTypeDefinitions, Is.EqualTo(new[] { domainBase, person, customer, organizationalUnit }));
    }

    [Test]
    public void VisitClassDefinition_WithChildrenDirection_AbortsTraversalOnNonNullValue ()
    {
      var (domainBase, person, customer, _) = TypeDefinitionWalkerTest.CreateClassTestDomain();

      var walker = new TestableTypeDefinitionWalker<string>(TypeDefinitionWalkerDirection.Descendants, value => value != null);
      walker.ClassDefinitionHandler = classDefinition => classDefinition == customer ? "finish" : null;
      Assert.That(walker.VisitClassDefinition(domainBase), Is.EqualTo("finish"));

      Assert.That(walker.VisitedTypeDefinitions, Is.EqualTo(new[] { domainBase, person, customer }));
    }

    [Test]
    public void VisitClassDefinition_WithParentsDirection_IsCalledInCorrectOrder ()
    {
      var (domainBase, person, customer, _) = TypeDefinitionWalkerTest.CreateClassTestDomain();

      var walker = new TestableTypeDefinitionWalker<string>(TypeDefinitionWalkerDirection.Ascendants, value => value != null);
      Assert.That(walker.VisitClassDefinition(customer), Is.Null);

      Assert.That(walker.VisitedTypeDefinitions, Is.EqualTo(new[] { customer, person, domainBase }));
    }

    [Test]
    public void VisitClassDefinition_WithParentsDirection_AbortsTraversalOnNonNullValue ()
    {
      var (_, person, customer, _) = TypeDefinitionWalkerTest.CreateClassTestDomain();

      var walker = new TestableTypeDefinitionWalker<string>(TypeDefinitionWalkerDirection.Ascendants, value => value != null);
      walker.ClassDefinitionHandler = classDefinition => classDefinition == person ? "finish" : null;
      Assert.That(walker.VisitClassDefinition(customer), Is.EqualTo("finish"));

      Assert.That(walker.VisitedTypeDefinitions, Is.EqualTo(new[] { customer, person }));
    }

    [Test]
    public void VisitInterfaceDefinition_WithChildrenDirection_IsCalledInCorrectOrder ()
    {
      var (_, iPerson, person, iCustomer) = TypeDefinitionWalkerTest.CreateInterfaceTestDomain();

      var walker = new TestableTypeDefinitionWalker<string>(TypeDefinitionWalkerDirection.Descendants, value => value != null);
      Assert.That(walker.VisitInterfaceDefinition(iPerson), Is.Null);

      Assert.That(walker.VisitedTypeDefinitions, Is.EqualTo(new TypeDefinition[] { iPerson, person, iCustomer }));
    }

    [Test]
    public void VisitInterfaceDefinition_WithChildrenDirection_AbortsTraversalOnNonNullValue ()
    {
      var (iDomainBase, iPerson, _, _) = TypeDefinitionWalkerTest.CreateInterfaceTestDomain();

      var walker = new TestableTypeDefinitionWalker<string>(TypeDefinitionWalkerDirection.Descendants, value => value != null);
      walker.InterfaceDefinitionHandler = interfaceDefinition => interfaceDefinition == iPerson ? "finish" : null;
      Assert.That(walker.VisitInterfaceDefinition(iDomainBase), Is.EqualTo("finish"));

      Assert.That(walker.VisitedTypeDefinitions, Is.EqualTo(new[] { iDomainBase, iPerson }));
    }

    [Test]
    public void VisitInterfaceDefinition_WithParentsDirection_IsCalledInCorrectOrder ()
    {
      var (iDomainBase, iPerson, _, iCustomer) = TypeDefinitionWalkerTest.CreateInterfaceTestDomain();

      var walker = new TestableTypeDefinitionWalker<string>(TypeDefinitionWalkerDirection.Ascendants, value => value != null);
      Assert.That(walker.VisitInterfaceDefinition(iCustomer), Is.Null);

      Assert.That(walker.VisitedTypeDefinitions, Is.EqualTo(new[] { iCustomer, iPerson, iDomainBase }));
    }

    [Test]
    public void VisitInterfaceDefinition_WithParentsDirection_AbortsTraversalOnNonNullValue ()
    {
      var (_, iPerson, _, iCustomer) = TypeDefinitionWalkerTest.CreateInterfaceTestDomain();

      var walker = new TestableTypeDefinitionWalker<string>(TypeDefinitionWalkerDirection.Ascendants, value => value != null);
      walker.InterfaceDefinitionHandler = interfaceDefinition => interfaceDefinition == iPerson ? "finish" : null;
      Assert.That(walker.VisitInterfaceDefinition(iCustomer), Is.EqualTo("finish"));

      Assert.That(walker.VisitedTypeDefinitions, Is.EqualTo(new[] { iCustomer, iPerson }));
    }
  }
}
