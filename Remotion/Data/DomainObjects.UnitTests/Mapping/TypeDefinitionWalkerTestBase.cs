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
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  public abstract class TypeDefinitionWalkerTestBase
  {
    protected static (ClassDefinition domainBase, ClassDefinition person, ClassDefinition customer, ClassDefinition organizationalUnit) CreateClassTestDomain ()
    {
      var domainBase = ClassDefinitionObjectMother.CreateClassDefinition("DomainBase");
      var person = ClassDefinitionObjectMother.CreateClassDefinition("Person", baseClass: domainBase);
      var customer = ClassDefinitionObjectMother.CreateClassDefinition("Customer", baseClass: person);
      var organizationalUnit = ClassDefinitionObjectMother.CreateClassDefinition("OrganizationalUnit", baseClass: domainBase);
      domainBase.SetDerivedClasses(new[] { person, organizationalUnit });
      person.SetDerivedClasses(new[] { customer });
      customer.SetDerivedClasses(Array.Empty<ClassDefinition>());
      organizationalUnit.SetDerivedClasses(Array.Empty<ClassDefinition>());

      return (domainBase, person, customer, organizationalUnit);
    }

    protected static (InterfaceDefinition iDomainBase, InterfaceDefinition iPerson, ClassDefinition person, InterfaceDefinition iCustomer) CreateInterfaceTestDomain ()
    {
      var iDomainBase = InterfaceDefinitionObjectMother.CreateInterfaceDefinition();
      var iPerson = InterfaceDefinitionObjectMother.CreateInterfaceDefinition(extendedInterfaces: new[] { iDomainBase });
      var person = ClassDefinitionObjectMother.CreateClassDefinition(implementedInterfaces: new[] { iPerson });
      var iCustomer = InterfaceDefinitionObjectMother.CreateInterfaceDefinition(extendedInterfaces: new[] { iPerson });
      iDomainBase.SetImplementingClasses(Array.Empty<ClassDefinition>());
      iDomainBase.SetExtendingInterfaces(new []{iPerson});
      iPerson.SetImplementingClasses(new []{person});
      iPerson.SetExtendingInterfaces(new []{iCustomer});
      person.SetDerivedClasses(Array.Empty<ClassDefinition>());
      iCustomer.SetImplementingClasses(Array.Empty<ClassDefinition>());
      iCustomer.SetExtendingInterfaces(Array.Empty<InterfaceDefinition>());

      return (iDomainBase, iPerson, person, iCustomer);
    }
  }
}
