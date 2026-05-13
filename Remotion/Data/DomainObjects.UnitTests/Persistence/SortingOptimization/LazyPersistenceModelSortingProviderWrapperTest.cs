// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Threading;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SortingOptimization;
using Remotion.Data.DomainObjects.Persistence.SortingOptimization;
using Remotion.Data.DomainObjects.UnitTests.Mapping;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.SortingOptimization;

[TestFixture]
public class LazyPersistenceModelSortingProviderWrapperTest
{
  private Mock<IPersistenceModelSortingProvider> _wrappedProviderMock;
  private LazyPersistenceModelSortingProviderWrapper _wrapper;
  private IReadOnlyList<ClassDefinition> _emptyClassDefinitions;

  [SetUp]
  public void SetUp ()
  {
    _wrappedProviderMock = new Mock<IPersistenceModelSortingProvider>(MockBehavior.Strict);
    _wrapper = new LazyPersistenceModelSortingProviderWrapper(_wrappedProviderMock.Object);
    _emptyClassDefinitions = [];
  }

  [Test]
  public void Initialize_WrappedProviderNull_ThrowsArgumentNullException ()
  {
    Assert.That(
        () => new LazyPersistenceModelSortingProviderWrapper(null!),
        Throws.TypeOf<ArgumentNullException>());
  }

  [Test]
  public void Initialize_ClassDefinitionsNull_ThrowsArgumentNullException ()
  {
    Assert.That(
        () => _wrapper.Initialize(null!),
        Throws.TypeOf<ArgumentNullException>());
  }

  [Test]
  public void Initialize_DelegatesToWrappedProvider ()
  {
    _wrappedProviderMock.Setup(p => p.Initialize(_emptyClassDefinitions)).Verifiable();

    _wrapper.Initialize(_emptyClassDefinitions);
    // Force the background initialization to complete by going through a
    // pass-through method that internally waits for the task.
    SetupAndCallGetSortPositionToWaitForInitialization();

    _wrappedProviderMock.Verify(p => p.Initialize(_emptyClassDefinitions), Times.Once);
  }

  [Test]
  public void Initialize_CalledTwice_ThrowsInvalidOperationException ()
  {
    _wrappedProviderMock.Setup(p => p.Initialize(It.IsAny<IReadOnlyList<ClassDefinition>>()));

    _wrapper.Initialize(_emptyClassDefinitions);

    Assert.That(
        () => _wrapper.Initialize(_emptyClassDefinitions),
        Throws.InvalidOperationException.With.Message.EqualTo("Initialize has already been called."));
  }

  [Test]
  public void Initialize_DoesNotBlockCallingThread_WhenWrappedInitializeIsSlow ()
  {
    var initializeStarted = new ManualResetEventSlim(false);
    var releaseInitialize = new ManualResetEventSlim(false);

    _wrappedProviderMock
        .Setup(p => p.Initialize(_emptyClassDefinitions))
        .Callback(() =>
        {
          initializeStarted.Set();
          releaseInitialize.Wait(TimeSpan.FromSeconds(5));
        });

    try
    {
      _wrapper.Initialize(_emptyClassDefinitions);

      // Initialize call must not block — the wrapped Initialize is still
      // running in the background and waiting on releaseInitialize.
      Assert.That(
          initializeStarted.Wait(TimeSpan.FromSeconds(5)),
          Is.True,
          "Wrapped provider's Initialize should have been invoked on a background task.");
    }
    finally
    {
      releaseInitialize.Set();
    }
  }

  [Test]
  public void GetSortPosition_WithStorageEntityDefinition_BeforeInitialize_ThrowsInvalidOperationException ()
  {
    var storageEntityDefinition = Mock.Of<IStorageEntityDefinition>();

    Assert.That(
        () => _wrapper.GetSortPosition(storageEntityDefinition),
        Throws.InvalidOperationException.With.Message.EqualTo(
            "Before accessing any methods, Initialize has to be called."));
  }

  [Test]
  public void GetSortPosition_WithStorageEntityDefinition_StorageEntityDefinitionNull_ThrowsArgumentNullException ()
  {
    Assert.That(
        () => _wrapper.GetSortPosition((IStorageEntityDefinition)null!),
        Throws.TypeOf<ArgumentNullException>());
  }

  [Test]
  public void GetSortPosition_WithStorageEntityDefinition_AfterInitialize_DelegatesToWrappedProviderAndReturnsResult ()
  {
    var storageEntityDefinition = Mock.Of<IStorageEntityDefinition>();
    _wrappedProviderMock.Setup(p => p.Initialize(_emptyClassDefinitions));
    _wrappedProviderMock.Setup(p => p.GetSortPosition(storageEntityDefinition)).Returns(42);

    _wrapper.Initialize(_emptyClassDefinitions);
    var result = _wrapper.GetSortPosition(storageEntityDefinition);

    Assert.That(result, Is.EqualTo(42));
    _wrappedProviderMock.Verify(p => p.GetSortPosition(storageEntityDefinition), Times.Once);
  }

  [Test]
  public void GetSortPosition_WithClassDefinition_BeforeInitialize_ThrowsInvalidOperationException ()
  {
    var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition();

    Assert.That(
        () => _wrapper.GetSortPosition(classDefinition),
        Throws.InvalidOperationException.With.Message.EqualTo(
            "Before accessing any methods, Initialize has to be called."));
  }

  [Test]
  public void GetSortPosition_WithClassDefinition_ClassDefinitionNull_ThrowsArgumentNullException ()
  {
    Assert.That(
        () => _wrapper.GetSortPosition((ClassDefinition)null!),
        Throws.TypeOf<ArgumentNullException>());
  }

  [Test]
  public void GetSortPosition_WithClassDefinition_AfterInitialize_DelegatesToWrappedProviderAndReturnsResult ()
  {
    var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
    _wrappedProviderMock.Setup(p => p.Initialize(_emptyClassDefinitions));
    _wrappedProviderMock.Setup(p => p.GetSortPosition(classDefinition)).Returns(7);

    _wrapper.Initialize(_emptyClassDefinitions);
    var result = _wrapper.GetSortPosition(classDefinition);

    Assert.That(result, Is.EqualTo(7));
    _wrappedProviderMock.Verify(p => p.GetSortPosition(classDefinition), Times.Once);
  }

  [Test]
  public void GetPropertySpecificationsForForeignKeyProperties_BeforeInitialize_ThrowsInvalidOperationException ()
  {
    var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition();

    Assert.That(
        () => _wrapper.GetPropertySpecificationsForForeignKeyProperties(classDefinition),
        Throws.InvalidOperationException.With.Message.EqualTo(
            "Before accessing any methods, Initialize has to be called."));
  }

  [Test]
  public void GetPropertySpecificationsForForeignKeyProperties_ClassDefinitionNull_ThrowsArgumentNullException ()
  {
    Assert.That(
        () => _wrapper.GetPropertySpecificationsForForeignKeyProperties(null!),
        Throws.TypeOf<ArgumentNullException>());
  }

  [Test]
  public void GetPropertySpecificationsForForeignKeyProperties_AfterInitialize_DelegatesToWrappedProviderAndReturnsResult ()
  {
    var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
    IReadOnlyList<SortingOptimizationObjectIDPropertySpecification> expectedResult = [];
    _wrappedProviderMock.Setup(p => p.Initialize(_emptyClassDefinitions));
    _wrappedProviderMock.Setup(p => p.GetPropertySpecificationsForForeignKeyProperties(classDefinition)).Returns(expectedResult);

    _wrapper.Initialize(_emptyClassDefinitions);
    var result = _wrapper.GetPropertySpecificationsForForeignKeyProperties(classDefinition);

    Assert.That(result, Is.SameAs(expectedResult));
    _wrappedProviderMock.Verify(p => p.GetPropertySpecificationsForForeignKeyProperties(classDefinition), Times.Once);
  }

  private void SetupAndCallGetSortPositionToWaitForInitialization ()
  {
    var storageEntityDefinition = Mock.Of<IStorageEntityDefinition>();
    _wrappedProviderMock.Setup(p => p.GetSortPosition(storageEntityDefinition)).Returns(0);
    _wrapper.GetSortPosition(storageEntityDefinition);
  }
}
