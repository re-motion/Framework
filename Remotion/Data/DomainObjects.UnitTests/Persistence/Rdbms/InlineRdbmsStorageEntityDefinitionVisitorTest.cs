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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
  [TestFixture]
  public class InlineRdbmsStorageEntityDefinitionVisitorTest : StandardMappingTest
  {
    public interface IVisitorCallReceiver
    {
      void HandleTableDefinition (TableDefinition table, Action<IRdbmsStorageEntityDefinition> continuation);
      void HandleFilterViewDefinition (FilterViewDefinition filterView, Action<IRdbmsStorageEntityDefinition> continuation);
      void HandleUnionViewDefinition (UnionViewDefinition unionView, Action<IRdbmsStorageEntityDefinition> continuation);
      void HandleEmptyViewDefinition (EmptyViewDefinition emptyView, Action<IRdbmsStorageEntityDefinition> continuation);
    }

    public interface IVisitorCallReceiver<T>
    {
      T HandleTableDefinition (TableDefinition table, Func<IRdbmsStorageEntityDefinition, T> continuation);
      T HandleFilterViewDefinition (FilterViewDefinition filterView, Func<IRdbmsStorageEntityDefinition, T> continuation);
      T HandleUnionViewDefinition (UnionViewDefinition unionView, Func<IRdbmsStorageEntityDefinition, T> continuation);
      T HandleEmptyViewDefinition (EmptyViewDefinition emptyView, Func<IRdbmsStorageEntityDefinition, T> continuation);
    }

    private TableDefinition _tableDefinition;
    private FilterViewDefinition _filterViewDefinition;
    private UnionViewDefinition _unionViewDefinition;
    private EmptyViewDefinition _emptyViewDefinition;
    private Mock<IVisitorCallReceiver> _voidReceiverMock;
    private Mock<IVisitorCallReceiver<string>> _nonVoidReceiverMock;

    public override void SetUp ()
    {
      base.SetUp();

      _tableDefinition = TableDefinitionObjectMother.Create(TestDomainStorageProviderDefinition);
      _filterViewDefinition = FilterViewDefinitionObjectMother.Create(
          TestDomainStorageProviderDefinition, new EntityNameDefinition(null, "FilterView"), _tableDefinition);
      _unionViewDefinition = UnionViewDefinitionObjectMother.Create(
          TestDomainStorageProviderDefinition, new EntityNameDefinition(null, "UnionView"), new[] { _tableDefinition });
      _emptyViewDefinition = EmptyViewDefinitionObjectMother.Create(TestDomainStorageProviderDefinition);

      _voidReceiverMock = new Mock<IVisitorCallReceiver>(MockBehavior.Strict);
      _nonVoidReceiverMock = new Mock<IVisitorCallReceiver<string>>(MockBehavior.Strict);
    }

    [Test]
    public void Visit_WithoutResult_TableDefinition ()
    {
      _voidReceiverMock.Setup(mock => mock.HandleTableDefinition(_tableDefinition, It.IsAny<Action<IRdbmsStorageEntityDefinition>>())).Verifiable();

      InlineRdbmsStorageEntityDefinitionVisitor.Visit(
          _tableDefinition,
          _voidReceiverMock.Object.HandleTableDefinition,
          _voidReceiverMock.Object.HandleFilterViewDefinition,
          _voidReceiverMock.Object.HandleUnionViewDefinition,
          _voidReceiverMock.Object.HandleEmptyViewDefinition);

      _voidReceiverMock.Verify();
    }

    [Test]
    public void Visit_WithoutResult_FilterViewDefinition ()
    {
      _voidReceiverMock.Setup(mock => mock.HandleFilterViewDefinition(_filterViewDefinition, It.IsAny<Action<IRdbmsStorageEntityDefinition>>())).Verifiable();

      InlineRdbmsStorageEntityDefinitionVisitor.Visit(
          _filterViewDefinition,
          _voidReceiverMock.Object.HandleTableDefinition,
          _voidReceiverMock.Object.HandleFilterViewDefinition,
          _voidReceiverMock.Object.HandleUnionViewDefinition,
          _voidReceiverMock.Object.HandleEmptyViewDefinition);

      _voidReceiverMock.Verify();
    }

    [Test]
    public void Visit_WithoutResult_UnionViewDefinition ()
    {
      _voidReceiverMock.Setup(mock => mock.HandleUnionViewDefinition(_unionViewDefinition, It.IsAny<Action<IRdbmsStorageEntityDefinition>>())).Verifiable();

      InlineRdbmsStorageEntityDefinitionVisitor.Visit(
          _unionViewDefinition,
          _voidReceiverMock.Object.HandleTableDefinition,
          _voidReceiverMock.Object.HandleFilterViewDefinition,
          _voidReceiverMock.Object.HandleUnionViewDefinition,
          _voidReceiverMock.Object.HandleEmptyViewDefinition);

      _voidReceiverMock.Verify();
    }

    [Test]
    public void Visit_WithoutResult_EmptyViewDefinition ()
    {
      _voidReceiverMock.Setup(mock => mock.HandleEmptyViewDefinition(_emptyViewDefinition, It.IsAny<Action<IRdbmsStorageEntityDefinition>>())).Verifiable();

      InlineRdbmsStorageEntityDefinitionVisitor.Visit(
          _emptyViewDefinition,
          _voidReceiverMock.Object.HandleTableDefinition,
          _voidReceiverMock.Object.HandleFilterViewDefinition,
          _voidReceiverMock.Object.HandleUnionViewDefinition,
          _voidReceiverMock.Object.HandleEmptyViewDefinition);

      _voidReceiverMock.Verify();
    }

    [Test]
    public void Visit_WithoutResult_Continuation_TableDefinition ()
    {
      _voidReceiverMock
          .Setup(mock => mock.HandleTableDefinition(_tableDefinition, It.IsAny<Action<IRdbmsStorageEntityDefinition>>()))
          .Callback((TableDefinition _, Action<IRdbmsStorageEntityDefinition> continuation) => continuation(_filterViewDefinition))
          .Verifiable();
      _voidReceiverMock
          .Setup(mock => mock.HandleFilterViewDefinition(_filterViewDefinition, It.IsAny<Action<IRdbmsStorageEntityDefinition>>()))
          .Verifiable();

      InlineRdbmsStorageEntityDefinitionVisitor.Visit(
          _tableDefinition,
          _voidReceiverMock.Object.HandleTableDefinition,
          _voidReceiverMock.Object.HandleFilterViewDefinition,
          _voidReceiverMock.Object.HandleUnionViewDefinition,
          _voidReceiverMock.Object.HandleEmptyViewDefinition);

      _voidReceiverMock.Verify();
    }

    [Test]
    public void Visit_WithoutResult_Continuation_FilterViewDefinition ()
    {
      _voidReceiverMock
          .Setup(mock => mock.HandleFilterViewDefinition(_filterViewDefinition, It.IsAny<Action<IRdbmsStorageEntityDefinition>>()))
          .Callback((FilterViewDefinition _, Action<IRdbmsStorageEntityDefinition> continuation) => continuation(_unionViewDefinition))
          .Verifiable();
      _voidReceiverMock
          .Setup(mock => mock.HandleUnionViewDefinition(_unionViewDefinition, It.IsAny<Action<IRdbmsStorageEntityDefinition>>()))
          .Verifiable();

      InlineRdbmsStorageEntityDefinitionVisitor.Visit(
          _filterViewDefinition,
          _voidReceiverMock.Object.HandleTableDefinition,
          _voidReceiverMock.Object.HandleFilterViewDefinition,
          _voidReceiverMock.Object.HandleUnionViewDefinition,
          _voidReceiverMock.Object.HandleEmptyViewDefinition);

      _voidReceiverMock.Verify();
    }

    [Test]
    public void Visit_WithoutResult_Continuation_UnionViewDefinition ()
    {
      _voidReceiverMock
          .Setup(mock => mock.HandleUnionViewDefinition(_unionViewDefinition, It.IsAny<Action<IRdbmsStorageEntityDefinition>>()))
          .Callback((UnionViewDefinition _, Action<IRdbmsStorageEntityDefinition> continuation) => continuation(_filterViewDefinition))
          .Verifiable();
      _voidReceiverMock
          .Setup(mock => mock.HandleFilterViewDefinition(_filterViewDefinition, It.IsAny<Action<IRdbmsStorageEntityDefinition>>()))
          .Verifiable();

      InlineRdbmsStorageEntityDefinitionVisitor.Visit(
          _unionViewDefinition,
          _voidReceiverMock.Object.HandleTableDefinition,
          _voidReceiverMock.Object.HandleFilterViewDefinition,
          _voidReceiverMock.Object.HandleUnionViewDefinition,
          _voidReceiverMock.Object.HandleEmptyViewDefinition);

      _voidReceiverMock.Verify();
    }

    [Test]
    public void Visit_WithoutResult_Continuation_EmptyViewDefinition ()
    {
      _voidReceiverMock
          .Setup(mock => mock.HandleEmptyViewDefinition(_emptyViewDefinition, It.IsAny<Action<IRdbmsStorageEntityDefinition>>()))
          .Callback((EmptyViewDefinition _, Action<IRdbmsStorageEntityDefinition> continuation) => continuation(_filterViewDefinition))
          .Verifiable();
      _voidReceiverMock
          .Setup(mock => mock.HandleFilterViewDefinition(_filterViewDefinition, It.IsAny<Action<IRdbmsStorageEntityDefinition>>()))
          .Verifiable();

      InlineRdbmsStorageEntityDefinitionVisitor.Visit(
          _emptyViewDefinition,
          _voidReceiverMock.Object.HandleTableDefinition,
          _voidReceiverMock.Object.HandleFilterViewDefinition,
          _voidReceiverMock.Object.HandleUnionViewDefinition,
          _voidReceiverMock.Object.HandleEmptyViewDefinition);

      _voidReceiverMock.Verify();
    }

    [Test]
    public void Visit_WithResult_TableDefinition ()
    {
      _nonVoidReceiverMock
          .Setup(mock => mock.HandleTableDefinition(_tableDefinition, It.IsAny<Func<IRdbmsStorageEntityDefinition, string>>()))
          .Returns("test")
          .Verifiable();

      var result = InlineRdbmsStorageEntityDefinitionVisitor.Visit<string>(
          _tableDefinition,
          _nonVoidReceiverMock.Object.HandleTableDefinition,
          _nonVoidReceiverMock.Object.HandleFilterViewDefinition,
          _nonVoidReceiverMock.Object.HandleUnionViewDefinition,
          _nonVoidReceiverMock.Object.HandleEmptyViewDefinition);

      _nonVoidReceiverMock.Verify();
      Assert.That(result, Is.EqualTo("test"));
    }

    [Test]
    public void Visit_WithResult_FilterViewDefinition ()
    {
      _nonVoidReceiverMock
          .Setup(mock => mock.HandleFilterViewDefinition(_filterViewDefinition, It.IsAny<Func<IRdbmsStorageEntityDefinition, string>>()))
          .Returns("test")
          .Verifiable();

      var result = InlineRdbmsStorageEntityDefinitionVisitor.Visit<string>(
          _filterViewDefinition,
          _nonVoidReceiverMock.Object.HandleTableDefinition,
          _nonVoidReceiverMock.Object.HandleFilterViewDefinition,
          _nonVoidReceiverMock.Object.HandleUnionViewDefinition,
          _nonVoidReceiverMock.Object.HandleEmptyViewDefinition);

      _nonVoidReceiverMock.Verify();
      Assert.That(result, Is.EqualTo("test"));
    }

    [Test]
    public void Visit_WithResult_UnionViewDefinition ()
    {
      _nonVoidReceiverMock
          .Setup(mock => mock.HandleUnionViewDefinition(_unionViewDefinition, It.IsAny<Func<IRdbmsStorageEntityDefinition, string>>()))
          .Returns("test")
          .Verifiable();

      var result = InlineRdbmsStorageEntityDefinitionVisitor.Visit<string>(
          _unionViewDefinition,
          _nonVoidReceiverMock.Object.HandleTableDefinition,
          _nonVoidReceiverMock.Object.HandleFilterViewDefinition,
          _nonVoidReceiverMock.Object.HandleUnionViewDefinition,
          _nonVoidReceiverMock.Object.HandleEmptyViewDefinition);

      _nonVoidReceiverMock.Verify();
      Assert.That(result, Is.EqualTo("test"));
    }

    [Test]
    public void Visit_WithResult_EmptyViewDefinition ()
    {
      _nonVoidReceiverMock
          .Setup(mock => mock.HandleEmptyViewDefinition(_emptyViewDefinition, It.IsAny<Func<IRdbmsStorageEntityDefinition, string>>()))
          .Returns("test")
          .Verifiable();

      var result = InlineRdbmsStorageEntityDefinitionVisitor.Visit<string>(
          _emptyViewDefinition,
          _nonVoidReceiverMock.Object.HandleTableDefinition,
          _nonVoidReceiverMock.Object.HandleFilterViewDefinition,
          _nonVoidReceiverMock.Object.HandleUnionViewDefinition,
          _nonVoidReceiverMock.Object.HandleEmptyViewDefinition);

      _nonVoidReceiverMock.Verify();
      Assert.That(result, Is.EqualTo("test"));
    }

    [Test]
    public void Visit_WithResult_Continuation_TableDefinition ()
    {
      _nonVoidReceiverMock
            .Setup(mock => mock.HandleTableDefinition(_tableDefinition, It.IsAny<Func<IRdbmsStorageEntityDefinition, string>>()))
            .Callback(
                (TableDefinition _, Func<IRdbmsStorageEntityDefinition, string> continuation) =>
                {
                  var value = continuation(_filterViewDefinition);
                  Assert.That(value, Is.EqualTo("3"));
                })
            .Returns("4")
            .Verifiable();
      _nonVoidReceiverMock
            .Setup(mock => mock.HandleFilterViewDefinition(_filterViewDefinition, It.IsAny<Func<IRdbmsStorageEntityDefinition, string>>()))
            .Returns("3")
            .Verifiable();

      var result = InlineRdbmsStorageEntityDefinitionVisitor.Visit<string>(
          _tableDefinition,
          _nonVoidReceiverMock.Object.HandleTableDefinition,
          _nonVoidReceiverMock.Object.HandleFilterViewDefinition,
          _nonVoidReceiverMock.Object.HandleUnionViewDefinition,
          _nonVoidReceiverMock.Object.HandleEmptyViewDefinition);

      _nonVoidReceiverMock.Verify();
      Assert.That(result, Is.EqualTo("4"));
    }

    [Test]
    public void Visit_WithResult_Continuation_FilterViewDefinition ()
    {
      _nonVoidReceiverMock
            .Setup(mock => mock.HandleFilterViewDefinition(_filterViewDefinition, It.IsAny<Func<IRdbmsStorageEntityDefinition, string>>()))
            .Callback(
                (FilterViewDefinition _, Func<IRdbmsStorageEntityDefinition, string> continuation) =>
                {
                  var value = continuation(_unionViewDefinition);
                  Assert.That(value, Is.EqualTo("2"));
                })
            .Returns("3")
            .Verifiable();
      _nonVoidReceiverMock
            .Setup(mock => mock.HandleUnionViewDefinition(_unionViewDefinition, It.IsAny<Func<IRdbmsStorageEntityDefinition, string>>()))
            .Returns("2")
            .Verifiable();

      var result = InlineRdbmsStorageEntityDefinitionVisitor.Visit<string>(
          _filterViewDefinition,
          _nonVoidReceiverMock.Object.HandleTableDefinition,
          _nonVoidReceiverMock.Object.HandleFilterViewDefinition,
          _nonVoidReceiverMock.Object.HandleUnionViewDefinition,
          _nonVoidReceiverMock.Object.HandleEmptyViewDefinition);

      _nonVoidReceiverMock.Verify();
      Assert.That(result, Is.EqualTo("3"));
    }

    [Test]
    public void Visit_WithResult_Continuation_UnionViewDefinition ()
    {
      _nonVoidReceiverMock
            .Setup(mock => mock.HandleUnionViewDefinition(_unionViewDefinition, It.IsAny<Func<IRdbmsStorageEntityDefinition, string>>()))
            .Callback(
                (UnionViewDefinition _, Func<IRdbmsStorageEntityDefinition, string> continuation) =>
                {
                  var value = continuation(_filterViewDefinition);
                  Assert.That(value, Is.EqualTo("1"));
                })
            .Returns("2")
            .Verifiable();
      _nonVoidReceiverMock
          .Setup(mock => mock.HandleFilterViewDefinition(_filterViewDefinition, It.IsAny<Func<IRdbmsStorageEntityDefinition, string>>()))
          .Returns("1")
          .Verifiable();

      var result = InlineRdbmsStorageEntityDefinitionVisitor.Visit<string>(
          _unionViewDefinition,
          _nonVoidReceiverMock.Object.HandleTableDefinition,
          _nonVoidReceiverMock.Object.HandleFilterViewDefinition,
          _nonVoidReceiverMock.Object.HandleUnionViewDefinition,
          _nonVoidReceiverMock.Object.HandleEmptyViewDefinition);

      _nonVoidReceiverMock.Verify();
      Assert.That(result, Is.EqualTo("2"));
    }

    [Test]
    public void Visit_WithResult_Continuation_EmptyViewDefinition ()
    {
      _nonVoidReceiverMock
            .Setup(mock => mock.HandleEmptyViewDefinition(_emptyViewDefinition, It.IsAny<Func<IRdbmsStorageEntityDefinition, string>>()))
            .Callback(
                (EmptyViewDefinition _, Func<IRdbmsStorageEntityDefinition, string> continuation) =>
                {
                  var value = continuation(_filterViewDefinition);
                  Assert.That(value, Is.EqualTo("3"));
                })
            .Returns("4")
            .Verifiable();
      _nonVoidReceiverMock
          .Setup(mock => mock.HandleFilterViewDefinition(_filterViewDefinition, It.IsAny<Func<IRdbmsStorageEntityDefinition, string>>()))
          .Returns("3")
          .Verifiable();

      var result = InlineRdbmsStorageEntityDefinitionVisitor.Visit<string>(
          _emptyViewDefinition,
          _nonVoidReceiverMock.Object.HandleTableDefinition,
          _nonVoidReceiverMock.Object.HandleFilterViewDefinition,
          _nonVoidReceiverMock.Object.HandleUnionViewDefinition,
          _nonVoidReceiverMock.Object.HandleEmptyViewDefinition);

      _nonVoidReceiverMock.Verify();
      Assert.That(result, Is.EqualTo("4"));
    }
  }
}
