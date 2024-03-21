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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure.TypePipe;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.TypePipe.Development.UnitTesting.ObjectMothers.CodeGeneration;
using Remotion.TypePipe.Dlr.Ast;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.TypeAssembly;
using Remotion.TypePipe.TypeAssembly.Implementation;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.TypePipe
{
  [TestFixture]
  public class DomainObjectParticipantTest : StandardMappingTest
  {
    private Mock<IClassDefinitionProvider> _classDefinitionProviderMock;

    private DomainObjectParticipant _participant;
    private Mock<IInterceptedPropertyFinder> _interceptedPropertyFinderMock;

    private ProxyTypeAssemblyContext _proxyTypeAssemblyContext;
    private MutableType _proxyType;

    public override void SetUp ()
    {
      base.SetUp();

      _classDefinitionProviderMock = new Mock<IClassDefinitionProvider>(MockBehavior.Strict);
      _interceptedPropertyFinderMock = new Mock<IInterceptedPropertyFinder>(MockBehavior.Strict);

      _participant = new DomainObjectParticipant(_classDefinitionProviderMock.Object, _interceptedPropertyFinderMock.Object);

      _proxyTypeAssemblyContext = ProxyTypeAssemblyContextObjectMother.Create(requestedType: typeof(Order));
      _proxyType = _proxyTypeAssemblyContext.ProxyType;
    }

    [Test]
    public void PartialTypeIdentifierProvider ()
    {
      var typeIdentifierProvider = _participant.PartialTypeIdentifierProvider;

      Assert.That(typeIdentifierProvider, Is.Null);
    }

    [Test]
    public void Participate_UsesDomainObjectTypeToGetInterceptedProperties ()
    {
      var fakeClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      var fakeInterceptors = new IAccessorInterceptor[0];
      _classDefinitionProviderMock.Setup(mock => mock.GetClassDefinition(_proxyType.BaseType)).Returns(fakeClassDefinition).Verifiable();
      _interceptedPropertyFinderMock.Setup(mock => mock.GetPropertyInterceptors(fakeClassDefinition, _proxyType.BaseType)).Returns(fakeInterceptors).Verifiable();

      _participant.Participate(null, _proxyTypeAssemblyContext);

      _classDefinitionProviderMock.Verify();
      _interceptedPropertyFinderMock.Verify();
    }

    [Test]
    public void Participate_AddsMarkerInterface_And_OverridesHooks ()
    {
      StubGetPropertyInterceptors();

      _participant.Participate(null, _proxyTypeAssemblyContext);

      Assert.That(_proxyType.AddedInterfaces, Is.EqualTo(new[] { typeof(IInterceptedDomainObject) }));
      Assert.That(_proxyType.AddedMethods, Has.Count.EqualTo(2));

      var performConstructorCheck = _proxyType.AddedMethods.Single(m => m.Name == "PerformConstructorCheck");
      Assert.That(performConstructorCheck.Body, Is.TypeOf<DefaultExpression>().And.Property("Type").SameAs(typeof(void)));

      var getPublicDomainObjectTypeImplementation = _proxyType.AddedMethods.Single(m => m.Name == "GetPublicDomainObjectTypeImplementation");
      Assert.That(
          getPublicDomainObjectTypeImplementation.Body,
          Is.TypeOf<ConstantExpression>().And.Property("Value").SameAs(_proxyTypeAssemblyContext.RequestedType));
    }

    [Test]
    public void Participate_ExecutesAccessorInterceptors ()
    {
      var accessorInterceptor = new Mock<IAccessorInterceptor>(MockBehavior.Strict);
      accessorInterceptor.Setup(mock => mock.Intercept(_proxyType)).Verifiable();
      StubGetPropertyInterceptors(accessorInterceptors: new[] { accessorInterceptor.Object });

      _participant.Participate(null, _proxyTypeAssemblyContext);

      accessorInterceptor.Verify();
    }

    [Test]
    public void Participate_NonDomainObject_Nop ()
    {
      var context = ProxyTypeAssemblyContextObjectMother.Create(requestedType: typeof(object));

      _participant.Participate(null, context);

      Assert.That(_proxyType.AddedInterfaces, Has.No.Member(typeof(IInterceptedDomainObject)));
      _classDefinitionProviderMock.Verify(mock => mock.GetClassDefinition(It.IsAny<Type>()), Times.Never());
    }

    [Test]
    public void Participate_UnmappedDomainObject_Nop ()
    {
      _classDefinitionProviderMock.Setup(stub => stub.GetClassDefinition(_proxyTypeAssemblyContext.RequestedType)).Returns((ClassDefinition)null);

      _participant.Participate(null, _proxyTypeAssemblyContext);

      Assert.That(_proxyType.AddedInterfaces, Has.No.Member(typeof(IInterceptedDomainObject)));
    }

    [Test]
    public void Participate_AbstractDomainObject ()
    {
      var abstractClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition(isAbstract: true);
      _classDefinitionProviderMock.Setup(stub => stub.GetClassDefinition(_proxyTypeAssemblyContext.RequestedType)).Returns(abstractClassDefinition);

      _participant.Participate(null, _proxyTypeAssemblyContext);

      Assert.That(_proxyType.AddedInterfaces, Has.No.Member(typeof(IInterceptedDomainObject)));
    }

    [Test]
    public void HandleNonSubclassableType ()
    {
      _classDefinitionProviderMock.Setup(stub => stub.GetClassDefinition(typeof(object))).Returns((ClassDefinition)null);
      Assert.That(() => _participant.HandleNonSubclassableType(typeof(object)), Throws.Nothing);
    }

    [Test]
    public void HandleNonSubclassableType_AbstractDomainObject ()
    {
      var abstractClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition(isAbstract: true);
      _classDefinitionProviderMock.Setup(stub => stub.GetClassDefinition(_proxyTypeAssemblyContext.RequestedType)).Returns(abstractClassDefinition);
      Assert.That(() => _participant.HandleNonSubclassableType(_proxyTypeAssemblyContext.RequestedType), Throws.Nothing);
    }

    [Test]
    public void HandleNonSubclassableType_UnsubclassableDomainObject ()
    {
      var nonAbstractClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition(isAbstract: false);
      _classDefinitionProviderMock.Setup(stub => stub.GetClassDefinition(typeof(NonSubclassableDomainObject))).Returns(nonAbstractClassDefinition);
      Assert.That(
          () => _participant.HandleNonSubclassableType(typeof(NonSubclassableDomainObject)),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo("The requested type 'NonSubclassableDomainObject' is derived from DomainObject but cannot be subclassed."));
    }

    [Test]
    public void HandleNonSubclassableType_NonDomainObject_Nop ()
    {
      _participant.HandleNonSubclassableType(typeof(object));

      Assert.That(_proxyType.AddedInterfaces, Has.No.Member(typeof(IInterceptedDomainObject)));
      _classDefinitionProviderMock.Verify(mock => mock.GetClassDefinition(It.IsAny<Type>()), Times.Never());
    }

    [Test]
    public void GetAdditionalTypeID ()
    {
      Assert.That(_participant.GetAdditionalTypeID(typeof(object)), Is.Null);
    }

    [Test]
    public void GetOrCreateAdditionalType ()
    {
      Assert.That(
          _participant.GetOrCreateAdditionalType(new object(), new Mock<IAdditionalTypeAssemblyContext>(MockBehavior.Strict).Object),
          Is.Null);
    }

    private void StubGetPropertyInterceptors (params IAccessorInterceptor[] accessorInterceptors)
    {
      var fakeClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      _classDefinitionProviderMock.Setup(stub => stub.GetClassDefinition(It.IsAny<Type>())).Returns(fakeClassDefinition);
      _interceptedPropertyFinderMock.Setup(stub => stub.GetPropertyInterceptors(It.IsAny<ClassDefinition>(), It.IsAny<Type>())).Returns(accessorInterceptors);
    }

    [DBTable]
    public sealed class NonSubclassableDomainObject : DomainObject { }
  }
}
