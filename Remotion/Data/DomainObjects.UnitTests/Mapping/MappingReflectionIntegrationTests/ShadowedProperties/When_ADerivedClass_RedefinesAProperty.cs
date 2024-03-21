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
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.MappingReflectionIntegrationTests.ShadowedProperties
{
  [TestFixture]
  public class When_ADerivedClass_RedefinesAProperty : MappingReflectionIntegrationTestBase
  {
    private ClassDefinition _derivedClassDefinition;
    private ClassDefinition _baseClassDefinition;
    private PropertyInfoAdapter _basePropertyInfo;
    private PropertyInfoAdapter _newPropertyInfo;

    public override void SetUp ()
    {
      base.SetUp();

      _derivedClassDefinition = GetClassDefinition(typeof(Shadower));
      _baseClassDefinition = _derivedClassDefinition.BaseClass;

      _basePropertyInfo = GetPropertyInformation((Base b) => b.Name);
      _newPropertyInfo = GetPropertyInformation((Shadower s) => s.Name);
    }

    [Test]
    public void BothProperties_ShouldHaveDifferentPropertyDefinitions ()
    {
      var basePropertyDefinition = _baseClassDefinition.ResolveProperty(_basePropertyInfo);
      Assert.That(basePropertyDefinition, Is.Not.Null);

      var newPropertyDefinition = _derivedClassDefinition.ResolveProperty(_newPropertyInfo);
      Assert.That(newPropertyDefinition, Is.Not.Null);

      Assert.That(newPropertyDefinition, Is.Not.SameAs(basePropertyDefinition));
    }

    [Test]
    public void TheDerivedClass_ShouldAlsoHaveTheBasePropertyDefinition_AsPartOfItsInheritedProperties ()
    {
      var basePropertyDefinitionOnBaseClass = _baseClassDefinition.ResolveProperty(_basePropertyInfo);

      Assert.That(_derivedClassDefinition.MyPropertyDefinitions, Has.No.Member(basePropertyDefinitionOnBaseClass));
      Assert.That(_derivedClassDefinition.GetPropertyDefinitions(), Has.Member(basePropertyDefinitionOnBaseClass));
    }
  }
}
