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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model
{
  [TestFixture]
  public class EmptyViewDefinitionTest
  {
    private SimpleStoragePropertyDefinition _timestampProperty;
    private ObjectIDStoragePropertyDefinition _objectIDProperty;
    private SimpleStoragePropertyDefinition _property1;
    private SimpleStoragePropertyDefinition _property2;
    private SimpleStoragePropertyDefinition _property3;
    
    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;
    private EntityNameDefinition[] _synonyms;

    private EmptyViewDefinition _emptyViewDefinition;

    [SetUp]
    public void SetUp ()
    {
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition ("SPID");
      
      _timestampProperty = SimpleStoragePropertyDefinitionObjectMother.TimestampProperty;
      _objectIDProperty = ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty;
      _property1 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Column1");
      _property2 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Column2");
      _property3 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Column3");

      _synonyms = new[] { new EntityNameDefinition ("Schema", "Test") };
      _emptyViewDefinition = new EmptyViewDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition ("Schema", "Test"),
          _objectIDProperty,
          _timestampProperty,
          new[] { _property1, _property2, _property3 },
          _synonyms);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_emptyViewDefinition.StorageProviderDefinition, Is.SameAs (_storageProviderDefinition));
      Assert.That (_emptyViewDefinition.ViewName, Is.EqualTo (new EntityNameDefinition ("Schema", "Test")));

      Assert.That (_emptyViewDefinition.ObjectIDProperty, Is.SameAs (_objectIDProperty));
      Assert.That (_emptyViewDefinition.TimestampProperty, Is.SameAs (_timestampProperty));
      Assert.That (_emptyViewDefinition.DataProperties, Is.EqualTo (new[] { _property1, _property2, _property3 }));

      Assert.That (_emptyViewDefinition.Indexes, Is.Empty);
      Assert.That (_emptyViewDefinition.Synonyms, Is.EqualTo (_synonyms));
    }

    [Test]
    public void Initialization_ViewNameNull ()
    {
      var emptyViewDefinition = new EmptyViewDefinition (
          _storageProviderDefinition,
          null,
          _objectIDProperty,
          _timestampProperty,
          new SimpleStoragePropertyDefinition[0],
          new EntityNameDefinition[0]);
      Assert.That (emptyViewDefinition.ViewName, Is.Null);
    }

    [Test]
    public void Accept ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<IRdbmsStorageEntityDefinitionVisitor> ();

      visitorMock.Expect (mock => mock.VisitEmptyViewDefinition(_emptyViewDefinition));
      visitorMock.Replay ();

      _emptyViewDefinition.Accept (visitorMock);

      visitorMock.VerifyAllExpectations ();
    }
  }
}