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
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.Reflection.TypeDiscovery;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.Abstractions;
using Remotion.TypePipe.MutableReflection;
using Rhino.Mocks;

namespace Remotion.Reflection.CodeGeneration.TypePipe.UnitTests
{
  [TestFixture]
  public class RemotionModuleBuilderFactoryDecoratorTest
  {
    private IModuleBuilderFactory _innerFactoryMock;

    private RemotionModuleBuilderFactoryDecorator _factory;

    [SetUp]
    public void SetUp ()
    {
      _innerFactoryMock = MockRepository.GenerateStrictMock<IModuleBuilderFactory>();

      _factory = new RemotionModuleBuilderFactoryDecorator (_innerFactoryMock);
    }

    [Test]
    public void CreateModuleBuilder ()
    {
      var assemblyName = "assembly name";
      var assemblyDirectoryOrNull = "directory";
      var strongNamed = BooleanObjectMother.GetRandomBoolean();
      var keyFilePathOrNull = "key file path";

      var moduleBuilderMock = MockRepository.GenerateStrictMock<IModuleBuilder>();
      var assemblyBuilderMock = MockRepository.GenerateStrictMock<IAssemblyBuilder>();
      _innerFactoryMock
          .Expect (mock => mock.CreateModuleBuilder (assemblyName, assemblyDirectoryOrNull, strongNamed, keyFilePathOrNull))
          .Return (moduleBuilderMock);
      moduleBuilderMock.Expect (mock => mock.AssemblyBuilder).Return (assemblyBuilderMock);
      assemblyBuilderMock
          .Expect (mock => mock.SetCustomAttribute (Arg<CustomAttributeDeclaration>.Is.Anything))
          .WhenCalled (
              mi =>
              {
                var declaration = (CustomAttributeDeclaration) mi.Arguments.Single();
                Assert.That (declaration.Type, Is.SameAs (typeof (NonApplicationAssemblyAttribute)));
                Assert.That (declaration.ConstructorArguments, Is.Empty);
              });

      var result = _factory.CreateModuleBuilder (assemblyName, assemblyDirectoryOrNull, strongNamed, keyFilePathOrNull);

      _innerFactoryMock.VerifyAllExpectations();
      moduleBuilderMock.VerifyAllExpectations();
      assemblyBuilderMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (moduleBuilderMock));
    }
  }
}