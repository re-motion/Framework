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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure.TypePipe;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.TypePipe.Development.UnitTesting.ObjectMothers.CodeGeneration;
using Remotion.TypePipe.Dlr.Ast;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.TypeAssembly;
using Remotion.TypePipe.TypeAssembly.Implementation;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.TypePipe
{
  [TestFixture]
  public class DomainObjectParticipantTest : StandardMappingTest
  {
    private ITypeDefinitionProvider _typeDefinitionProviderMock;

    private DomainObjectParticipant _participant;
    private IInterceptedPropertyFinder _interceptedPropertyFinderMock;

    private ProxyTypeAssemblyContext _proxyTypeAssemblyContext;
    private MutableType _proxyType;

    public override void SetUp ()
    {
      base.SetUp();

      _typeDefinitionProviderMock = MockRepository.GenerateStrictMock<ITypeDefinitionProvider>();
      _interceptedPropertyFinderMock = MockRepository.GenerateStrictMock<IInterceptedPropertyFinder>();

      _participant = new DomainObjectParticipant();
      PrivateInvoke.SetNonPublicField (_participant, "_typeDefinitionProvider", _typeDefinitionProviderMock);
      PrivateInvoke.SetNonPublicField (_participant, "_interceptedPropertyFinder", _interceptedPropertyFinderMock);

      _proxyTypeAssemblyContext = ProxyTypeAssemblyContextObjectMother.Create (requestedType: typeof (Order));
      _proxyType = _proxyTypeAssemblyContext.ProxyType;
    }

    [Test]
    public void PartialTypeIdentifierProvider ()
    {
      var typeIdentifierProvider = _participant.PartialTypeIdentifierProvider;

      Assert.That (typeIdentifierProvider, Is.Null);
    }

    [Test]
    public void Participate_UsesDomainObjectTypeToGetInterceptedProperties ()
    {
      var fakeClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      var fakeInterceptors = new IAccessorInterceptor[0];
      _typeDefinitionProviderMock.Expect (mock => mock.GetTypeDefinition (_proxyType.BaseType)).Return (fakeClassDefinition);
      _interceptedPropertyFinderMock.Expect (mock => mock.GetPropertyInterceptors (fakeClassDefinition, _proxyType.BaseType)).Return (fakeInterceptors);

      _participant.Participate (null, _proxyTypeAssemblyContext);

      _typeDefinitionProviderMock.VerifyAllExpectations();
      _interceptedPropertyFinderMock.VerifyAllExpectations();
    }

    [Test]
    public void Participate_AddsMarkerInterface_And_OverridesHooks ()
    {
      StubGetPropertyInterceptors();

      _participant.Participate (null, _proxyTypeAssemblyContext);

      Assert.That (_proxyType.AddedInterfaces, Is.EqualTo (new[] { typeof (IInterceptedDomainObject) }));
      Assert.That (_proxyType.AddedMethods, Has.Count.EqualTo (2));
      
      var performConstructorCheck = _proxyType.AddedMethods.Single (m => m.Name == "PerformConstructorCheck");
      Assert.That (performConstructorCheck.Body, Is.TypeOf<DefaultExpression>().And.Property ("Type").SameAs (typeof (void)));

      var getPublicDomainObjectTypeImplementation = _proxyType.AddedMethods.Single (m => m.Name == "GetPublicDomainObjectTypeImplementation");
      Assert.That (
          getPublicDomainObjectTypeImplementation.Body,
          Is.TypeOf<ConstantExpression>().And.Property ("Value").SameAs (_proxyTypeAssemblyContext.RequestedType));
    }

    [Test]
    public void Participate_ExecutesAccessorInterceptors ()
    {
      var accessorInterceptor = MockRepository.GenerateStrictMock<IAccessorInterceptor>();
      accessorInterceptor.Expect (mock => mock.Intercept (_proxyType));
      StubGetPropertyInterceptors (accessorInterceptors: new[] { accessorInterceptor });

      _participant.Participate (null, _proxyTypeAssemblyContext);

      accessorInterceptor.VerifyAllExpectations();
    }

    [Test]
    public void Participate_NonDomainObject_Nop ()
    {
      var context = ProxyTypeAssemblyContextObjectMother.Create (requestedType: typeof (object));

      _participant.Participate (null, context);

      Assert.That (_proxyType.AddedInterfaces, Has.No.Member (typeof (IInterceptedDomainObject)));
      _typeDefinitionProviderMock.AssertWasNotCalled (mock => mock.GetTypeDefinition (Arg<Type>.Is.Anything));
    }

    [Test]
    public void Participate_UnmappedDomainObject_Nop ()
    {
      _typeDefinitionProviderMock.Stub (stub => stub.GetTypeDefinition (_proxyTypeAssemblyContext.RequestedType)).Return (null);

      _participant.Participate (null, _proxyTypeAssemblyContext);

      Assert.That (_proxyType.AddedInterfaces, Has.No.Member (typeof (IInterceptedDomainObject)));
    }

    [Test]
    public void Participate_AbstractDomainObject ()
    {
      var abstractClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition (isAbstract: true);
      _typeDefinitionProviderMock.Stub (stub => stub.GetTypeDefinition (_proxyTypeAssemblyContext.RequestedType)).Return (abstractClassDefinition);

      _participant.Participate (null, _proxyTypeAssemblyContext);

      Assert.That (_proxyType.AddedInterfaces, Has.No.Member (typeof (IInterceptedDomainObject)));
    }

    [Test]
    public void HandleNonSubclassableType ()
    {
      _typeDefinitionProviderMock.Stub (stub => stub.GetTypeDefinition (typeof (object))).Return (null);
      Assert.That (() => _participant.HandleNonSubclassableType (typeof (object)), Throws.Nothing);
    }

    [Test]
    public void HandleNonSubclassableType_AbstractDomainObject ()
    {
      var abstractClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition (isAbstract: true);
      _typeDefinitionProviderMock.Stub (stub => stub.GetTypeDefinition (_proxyTypeAssemblyContext.RequestedType)).Return (abstractClassDefinition);
      Assert.That (() => _participant.HandleNonSubclassableType (_proxyTypeAssemblyContext.RequestedType), Throws.Nothing);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage =
        "The requested type 'NonSubclassableDomainObject' is derived from DomainObject but cannot be subclassed.")]
    public void HandleNonSubclassableType_UnsubclassableDomainObject ()
    {
      var nonAbstractClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition (isAbstract: false);
      _typeDefinitionProviderMock.Stub (stub => stub.GetTypeDefinition (typeof (NonSubclassableDomainObject))).Return (nonAbstractClassDefinition);
      _participant.HandleNonSubclassableType (typeof (NonSubclassableDomainObject));
    }

    [Test]
    public void HandleNonSubclassableType_NonDomainObject_Nop ()
    {
      _participant.HandleNonSubclassableType (typeof (object));

      Assert.That(_proxyType.AddedInterfaces, Has.No.Member(typeof(IInterceptedDomainObject)));
      _typeDefinitionProviderMock.AssertWasNotCalled(mock => mock.GetTypeDefinition(Arg<Type>.Is.Anything));
    }

    [Test]
    public void GetAdditionalTypeID ()
    {
      Assert.That (_participant.GetAdditionalTypeID (typeof (object)), Is.Null);
    }

    [Test]
    public void GetOrCreateAdditionalType ()
    {
      Assert.That (
          _participant.GetOrCreateAdditionalType (new object(), MockRepository.GenerateStrictMock<IAdditionalTypeAssemblyContext>()),
          Is.Null);
    }

    private void StubGetPropertyInterceptors (params IAccessorInterceptor[] accessorInterceptors)
    {
      var fakeClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      _typeDefinitionProviderMock.Stub (stub => stub.GetTypeDefinition (Arg<Type>.Is.Anything)).Return (fakeClassDefinition);
      _interceptedPropertyFinderMock.Stub (stub => stub.GetPropertyInterceptors (null, null)).IgnoreArguments().Return (accessorInterceptors);
    }

    [DBTable]
    public sealed class NonSubclassableDomainObject : DomainObject { }
  }
}