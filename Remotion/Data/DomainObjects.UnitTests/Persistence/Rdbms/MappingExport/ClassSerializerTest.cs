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
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.MappingExport;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGenerationTestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.MappingExport
{
  [TestFixture]
  public class ClassSerializerTest : SchemaGenerationTestBase
  {

    public override void SetUp ()
    {
      base.SetUp();
    }

    [Test]
    public void Serialize_AddsIdAttribute ()
    {
      var classSerializer = new ClassSerializer (MockRepository.GenerateStub<ITableSerializer>());
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (ClassWithAllDataTypes));
      var actual = classSerializer.Serialize (classDefinition);

      Assert.That (actual.Name.LocalName, Is.EqualTo ("class"));
      Assert.That (actual.Attributes().Select (a => a.Name.LocalName), Contains.Item ("id"));
      Assert.That (actual.Attribute ("id").Value, Is.EqualTo ("ClassWithAllDataTypes"));
    }

    [Test]
    public void Serialize_AddsBaseClassAttribute ()
    {
      var classSerializer = new ClassSerializer (MockRepository.GenerateStub<ITableSerializer>());
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (DerivedAbstractClass));
      var actual = classSerializer.Serialize (classDefinition);

      Assert.That (actual.Attributes().Select (a => a.Name.LocalName), Contains.Item ("baseClass"));
      Assert.That (actual.Attribute ("baseClass").Value, Is.EqualTo ("AbstractClass"));
    }

    [Test]
    public void Serialize_ClassHasNoBaseClass_DoesNotAddAttribute ()
    {
      var classSerializer = new ClassSerializer (MockRepository.GenerateStub<ITableSerializer>());
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (Ceo));
      var actual = classSerializer.Serialize (classDefinition);

      Assert.That (actual.Attributes().Select (a => a.Name.LocalName).Contains("baseClass"), Is.False);
    }

    [Test]
    public void Serialize_AddsAbstractAttribute ()
    {
      var classSerializer = new ClassSerializer (MockRepository.GenerateStub<ITableSerializer>());
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (Ceo));
      var actual = classSerializer.Serialize (classDefinition);

      Assert.That (actual.Attributes().Select (a => a.Name.LocalName), Contains.Item ("isAbstract"));
      Assert.That (actual.Attribute ("isAbstract").Value, Is.EqualTo ("false"));
    }
    
    [Test]
    public void Serialize_AddsAbstractAttribute_AbstractClass ()
    {
      var classSerializer = new ClassSerializer (MockRepository.GenerateStub<ITableSerializer>());
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (DerivedAbstractClass));
      var actual = classSerializer.Serialize (classDefinition);

      Assert.That (actual.Attributes().Select (a => a.Name.LocalName), Contains.Item ("isAbstract"));
      Assert.That (actual.Attribute ("isAbstract").Value, Is.EqualTo ("true"));
    }

    [Test]
    public void Serialize_DoesNotExportTablesForAbstractNonInstantiableClasses ()
    {
      var tableSerializerStub = MockRepository.GenerateStub<ITableSerializer>();
      var classSerializer = new ClassSerializer (tableSerializerStub);
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (AbstractWithoutConcreteClass));
      classSerializer.Serialize (classDefinition);

      tableSerializerStub.AssertWasNotCalled (_ => _.Serialize (classDefinition));
    }

    [Test]
    public void Serialize_AddsTableElements ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (Ceo));
      var tableSerializerMock = MockRepository.GenerateMock<ITableSerializer>();
      var expected1 = new XElement ("property1");

      tableSerializerMock.Expect (_ => _.Serialize (classDefinition)).Return (new [] { expected1 });
      var classSerializer = new ClassSerializer (tableSerializerMock);

      tableSerializerMock.Replay();
      var actual = classSerializer.Serialize (classDefinition);
      tableSerializerMock.VerifyAllExpectations();

      Assert.That (actual.Elements().Count(), Is.EqualTo(1));
      Assert.That (actual.Elements(), Contains.Item(expected1));
    }
  }
}