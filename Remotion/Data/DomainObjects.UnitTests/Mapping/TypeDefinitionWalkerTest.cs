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
  [TestFixture]
  public class TypeDefinitionWalkerTest : TypeDefinitionWalkerTestBase
  {
    private class TestableTypeDefinitionWalker : TypeDefinitionWalker
    {
      public List<TypeDefinition> VisitedTypeDefinitions { get; } = new();

      public TestableTypeDefinitionWalker (TypeDefinitionWalkerDirection direction)
          : base(direction)
      {
      }

      /// <inheritdoc />
      public override void VisitClassDefinition (ClassDefinition classDefinition)
      {
        VisitedTypeDefinitions.Add(classDefinition);
        base.VisitClassDefinition(classDefinition);
      }

      /// <inheritdoc />
      public override void VisitInterfaceDefinition (InterfaceDefinition interfaceDefinition)
      {
        VisitedTypeDefinitions.Add(interfaceDefinition);
        base.VisitInterfaceDefinition(interfaceDefinition);
      }
    }

    [Test]
    public void VisitClassDefinition_WithChildrenDirection_IsCalledInCorrectOrder ()
    {
      var (domainBase, person, customer, organizationalUnit) = CreateClassTestDomain();

      var walker = new TestableTypeDefinitionWalker(TypeDefinitionWalkerDirection.Descendants);
      walker.VisitClassDefinition(domainBase);

      Assert.That(walker.VisitedTypeDefinitions, Is.EqualTo(new[] { domainBase, person, customer, organizationalUnit }));
    }

    [Test]
    public void VisitClassDefinition_WithParentsDirection_IsCalledInCorrectOrder ()
    {
      var (domainBase, person, customer, _) = CreateClassTestDomain();

      var walker = new TestableTypeDefinitionWalker(TypeDefinitionWalkerDirection.Ascendants);
      walker.VisitClassDefinition(customer);

      Assert.That(walker.VisitedTypeDefinitions, Is.EqualTo(new[] { customer, person, domainBase }));
    }

    [Test]
    public void VisitInterfaceDefinition_WithChildrenDirection_IsCalledInCorrectOrder ()
    {
      var (_, iPerson, person, iCustomer) = CreateInterfaceTestDomain();

      var walker = new TestableTypeDefinitionWalker(TypeDefinitionWalkerDirection.Descendants);
      walker.VisitInterfaceDefinition(iPerson);

      Assert.That(walker.VisitedTypeDefinitions, Is.EqualTo(new TypeDefinition[] { iPerson, person, iCustomer }));
    }

    [Test]
    public void VisitInterfaceDefinition_WithParentsDirection_IsCalledInCorrectOrder ()
    {
      var (iDomainBase, iPerson, _, _) = CreateInterfaceTestDomain();

      var walker = new TestableTypeDefinitionWalker(TypeDefinitionWalkerDirection.Ascendants);
      walker.VisitInterfaceDefinition(iPerson);

      Assert.That(walker.VisitedTypeDefinitions, Is.EqualTo(new[] { iPerson, iDomainBase }));
    }
  }
}
