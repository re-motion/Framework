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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class TypeDefinitionHierarchyTest
  {
    [Test]
    public void ArePartOfSameHierarchy ()
    {
      var iOther = InterfaceDefinitionObjectMother.CreateInterfaceDefinition();
      var iExtendedOther = InterfaceDefinitionObjectMother.CreateInterfaceDefinition(extendedInterfaces: new[] { iOther });
      var iDomainBase = InterfaceDefinitionObjectMother.CreateInterfaceDefinition();
      var domainBase = ClassDefinitionObjectMother.CreateClassDefinition("Base", implementedInterfaces: new[] { iDomainBase });
      var person = ClassDefinitionObjectMother.CreateClassDefinition("Person", baseClass: domainBase);
      var customer = ClassDefinitionObjectMother.CreateClassDefinition("Customer", baseClass: person, implementedInterfaces: new[] { iExtendedOther });
      var employee = ClassDefinitionObjectMother.CreateClassDefinition("Employee", baseClass: person);
      var iCompany = InterfaceDefinitionObjectMother.CreateInterfaceDefinition(extendedInterfaces: new[] { iDomainBase });

      var separate = ClassDefinitionObjectMother.CreateClassDefinition("Separate");

      Assert.That(TypeDefinitionHierarchy.ArePartOfSameHierarchy(separate, iOther), Is.False);
      Assert.That(TypeDefinitionHierarchy.ArePartOfSameHierarchy(iOther, separate), Is.False);
      Assert.That(TypeDefinitionHierarchy.ArePartOfSameHierarchy(iOther, person), Is.False);
      Assert.That(TypeDefinitionHierarchy.ArePartOfSameHierarchy(person, iOther), Is.False);

      Assert.That(TypeDefinitionHierarchy.ArePartOfSameHierarchy(customer, employee), Is.True);
      Assert.That(TypeDefinitionHierarchy.ArePartOfSameHierarchy(employee, customer), Is.True);
      Assert.That(TypeDefinitionHierarchy.ArePartOfSameHierarchy(employee, iCompany), Is.True);
      Assert.That(TypeDefinitionHierarchy.ArePartOfSameHierarchy(iCompany, employee), Is.True);
    }

    [Test]
    public void ArePartOfSameHierarchy_WithSameTypeDefinition_ReturnsTrue ()
    {
      var typeDefinition = TypeDefinitionObjectMother.CreateInterfaceDefinition();

      Assert.That(TypeDefinitionHierarchy.ArePartOfSameHierarchy(typeDefinition, typeDefinition), Is.True);
    }

    [Test]
    public void GetHierarchyRoots ()
    {
      var iOther = InterfaceDefinitionObjectMother.CreateInterfaceDefinition();
      var iExtendedOther = InterfaceDefinitionObjectMother.CreateInterfaceDefinition(extendedInterfaces: new[] { iOther });
      var iDomainBase = InterfaceDefinitionObjectMother.CreateInterfaceDefinition();
      var domainBase = ClassDefinitionObjectMother.CreateClassDefinition("Base", implementedInterfaces: new[] { iDomainBase });
      var person = ClassDefinitionObjectMother.CreateClassDefinition("Person", baseClass: domainBase);
      var customer = ClassDefinitionObjectMother.CreateClassDefinition("Customer", baseClass: person, implementedInterfaces: new[] { iExtendedOther });
      var employee = ClassDefinitionObjectMother.CreateClassDefinition("Employee", baseClass: person);

      var separate = ClassDefinitionObjectMother.CreateClassDefinition("Separate");

      var hierarchyRoots = TypeDefinitionHierarchy.GetHierarchyRoots(new[] { customer, employee, separate });

      Assert.That(hierarchyRoots, Is.EqualTo(new TypeDefinition[] { domainBase, iDomainBase, iOther, separate }));
    }

    [Test]
    public void GetDescendantsAndSelf ()
    {
      var iDomainBase = InterfaceDefinitionObjectMother.CreateInterfaceDefinition();
      var iExternal = InterfaceDefinitionObjectMother.CreateInterfaceDefinitionWithDefaultProperties(extendedInterfaces: new[] { iDomainBase });
      var domainBase = ClassDefinitionObjectMother.CreateClassDefinition("DomainBase", implementedInterfaces: new[] { iDomainBase });
      var person = ClassDefinitionObjectMother.CreateClassDefinition("Person", baseClass: domainBase);
      var customer = ClassDefinitionObjectMother.CreateClassDefinitionWithDefaultProperties("Customer", baseClass: person);
      var organizationalUnit = ClassDefinitionObjectMother.CreateClassDefinitionWithDefaultProperties("OrganizationalUnit", baseClass: domainBase);
      iDomainBase.SetExtendingInterfaces(new[] { iExternal });
      iDomainBase.SetImplementingClasses(new[] { domainBase });
      domainBase.SetDerivedClasses(new[] { person, organizationalUnit });
      person.SetDerivedClasses(new[] { customer });

      var actual = TypeDefinitionHierarchy.GetDescendantsAndSelf(iDomainBase);
      var expected = new TypeDefinition[]
                     {
                         iDomainBase,
                         domainBase,
                         person,
                         customer,
                         organizationalUnit,
                         iExternal
                     };

      Assert.That(actual, Is.EqualTo(expected));
    }
  }
}
