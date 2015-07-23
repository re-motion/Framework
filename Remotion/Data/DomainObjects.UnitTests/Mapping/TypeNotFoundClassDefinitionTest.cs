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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class TypeNotFoundClassDefinitionTest
  {
    private ClassDefinitionForUnresolvedRelationPropertyType _classDefinition;
    private Type _classType;
    private IPropertyInformation _relationProperty;

    [SetUp]
    public void SetUp ()
    {
      _classType = typeof (ClassNotInMapping);
      _relationProperty = MockRepository.GenerateStub<IPropertyInformation>();
      _classDefinition = new ClassDefinitionForUnresolvedRelationPropertyType ("Test", _classType, _relationProperty);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_classDefinition.ClassType, Is.SameAs (_classType));
      Assert.That (_classDefinition.BaseClass, Is.Null);
      Assert.That (_classDefinition.IsClassTypeResolved, Is.False);
      Assert.That (_classDefinition.IsAbstract, Is.False);
      Assert.That (_classDefinition.RelationProperty, Is.SameAs (_relationProperty));
    }
  }
}