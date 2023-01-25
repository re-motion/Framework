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
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Mixins.Utilities;
using Remotion.TypePipe;
using Remotion.TypePipe.Implementation;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration
{
  [TestFixture]
  public class ObjectFactoryImplementationTest
  {
    private Mock<IPipeline> _defaultPipelineMock;

    private ObjectFactoryImplementation _implementation;

    [SetUp]
    public void SetUp ()
    {
      _defaultPipelineMock = new Mock<IPipeline>(MockBehavior.Strict);

      var pipelineRegistryStub = new Mock<IPipelineRegistry>();
      pipelineRegistryStub.Setup(stub => stub.DefaultPipeline).Returns(_defaultPipelineMock.Object);
      _implementation = new ObjectFactoryImplementation(pipelineRegistryStub.Object);
    }

    [Test]
    public void CreateInstance_UsesPipeline ()
    {
      var allowNonPublicConstructors = BooleanObjectMother.GetRandomBoolean();
      var fakeInstance = new object();
      _defaultPipelineMock
          .Setup(mock => mock.Create(typeof(BaseType1), ParamList.Empty, allowNonPublicConstructors))
          .Returns(fakeInstance)
          .Verifiable();

      var instance = _implementation.CreateInstance(allowNonPublicConstructors, typeof(BaseType1), ParamList.Empty);

      _defaultPipelineMock.Verify();
      Assert.That(instance, Is.SameAs(fakeInstance));
    }

    [Test]
    public void CreateInstance_WithNoPreparedMixinInstances_SetsUpEmptyPreparedMixins ()
    {
      object[] actualSuppliedInstances = null;
      _defaultPipelineMock
          .Setup(mock => mock.Create(typeof(BaseType1), ParamList.Empty, false))
          .Returns(new object())
          .Callback(() => actualSuppliedInstances = MixedObjectInstantiationScope.Current.SuppliedMixinInstances)
          .Verifiable();

      using (new MixedObjectInstantiationScope(new object()))
      {
        _implementation.CreateInstance(false, typeof(BaseType1), ParamList.Empty);
      }

      _defaultPipelineMock.Verify();
      Assert.That(actualSuppliedInstances, Is.Empty);
    }

    [Test]
    public void CreateInstance_WithPreparedMixinInstances_SetsUpPreparedMixins ()
    {
      var preparedMixin1 = new object();
      var preparedMixin2 = new object();

      object[] actualSuppliedInstances = null;

      _defaultPipelineMock
          .Setup(mock => mock.Create(typeof(BaseType1), ParamList.Empty, false))
          .Returns(new object())
          .Callback(() => actualSuppliedInstances = MixedObjectInstantiationScope.Current.SuppliedMixinInstances)
          .Verifiable();

      using (new MixedObjectInstantiationScope(new object()))
      {
        _implementation.CreateInstance(false, typeof(BaseType1), ParamList.Empty, preparedMixin1, preparedMixin2);
      }

      _defaultPipelineMock.Verify();
      Assert.That(actualSuppliedInstances, Is.EqualTo(new[] { preparedMixin1, preparedMixin2 }));
    }

    [Test]
    public void CreateInstance_WithUnmixedType_StillUsesTypePipe ()
    {
      var allowNonPublicConstructors = BooleanObjectMother.GetRandomBoolean();
      var fakeInstance = new object();
      _defaultPipelineMock
          .Setup(mock => mock.Create(typeof(object), ParamList.Empty, allowNonPublicConstructors))
          .Returns(fakeInstance)
          .Verifiable();

      var instance = _implementation.CreateInstance(allowNonPublicConstructors, typeof(object), ParamList.Empty);

      _defaultPipelineMock.Verify();
      Assert.That(instance, Is.SameAs(fakeInstance));
    }

    [Test]
    public void CreateInstance_WithUnmixedType_AndPreparedMixinInstances_Throws ()
    {
      Assert.That(
          () => _implementation.CreateInstance(BooleanObjectMother.GetRandomBoolean(), typeof(object), ParamList.Empty, new object()),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "There is no mixin configuration for type System.Object, so no mixin instances must be specified.", "preparedMixins"));
    }

    [Test]
    public void CreateInstance_WithInterface ()
    {
      Assert.That(
          () => _implementation.CreateInstance(false, typeof(IServiceProvider), ParamList.Empty, false),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("Cannot instantiate type 'System.IServiceProvider', it's an interface.", "targetOrConcreteType"));
    }

    [Test]
    public void CreateInstance_WithConcreteType ()
    {
      var allowNonPublicCtor = BooleanObjectMother.GetRandomBoolean();
      var concreteType = TypeFactory.GetConcreteType(typeof(BaseType1));
      var paramList = ParamList.Create("blub");
      var fakeInstance = new object();

      var reflectionServiceMock = new Mock<IReflectionService>(MockBehavior.Strict);
      _defaultPipelineMock.Setup(_ => _.ReflectionService).Returns(reflectionServiceMock.Object);
      reflectionServiceMock.Setup(_ => _.InstantiateAssembledType(concreteType, paramList, allowNonPublicCtor)).Returns(fakeInstance).Verifiable();

      var instance = _implementation.CreateInstance(allowNonPublicCtor, concreteType, paramList);

      reflectionServiceMock.Verify();
      Assert.That(instance, Is.SameAs(fakeInstance));
    }

    [Test]
    public void CreateInstance_WithConcreteType_AndPreparedMixins ()
    {
      var allowNonPublicCtor = BooleanObjectMother.GetRandomBoolean();
      var concreteType = TypeFactory.GetConcreteType(typeof(BaseType1));
      var paramList = ParamList.Create("blub");

      var fakePreparedInstance = new object();

      var reflectionServiceMock = new Mock<IReflectionService>(MockBehavior.Strict);
      _defaultPipelineMock.Setup(_ => _.ReflectionService).Returns(reflectionServiceMock.Object);
      reflectionServiceMock
          .Setup(_ => _.InstantiateAssembledType(concreteType, paramList, allowNonPublicCtor))
          .Returns(new object())
          .Callback(
              (Type assembledType, ParamList constructorArguments, bool allowNonPublicConstructor) =>
              {
                Assert.That(MixedObjectInstantiationScope.HasCurrent, Is.True);
                Assert.That(MixedObjectInstantiationScope.Current.SuppliedMixinInstances, Is.EqualTo(new[] { fakePreparedInstance }));
              })
          .Verifiable();

      _implementation.CreateInstance(allowNonPublicCtor, concreteType, paramList, fakePreparedInstance);

      reflectionServiceMock.Verify();
    }
  }
}
