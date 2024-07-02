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
using System.IO;
using Moq;
using NUnit.Framework;
using Remotion.Mixins.Context;
using Remotion.Mixins.MixerTools.UnitTests.TestDomain;
using Remotion.Mixins.Validation;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.TypePipe;
using Remotion.TypePipe.Implementation;

namespace Remotion.Mixins.MixerTools.UnitTests
{
  [TestFixture]
  public class MixerTest
  {
    private string _assemblyOutputDirectoy;
    private string _modulePath;

    private Mock<IMixedTypeFinder> _mixedTypeFinderStub;
    private Mock<IMixerPipelineFactory> _mixerPipelineFactoryStub;
    private Mock<IPipeline> _pipelineStub;

    private Mixer _mixer;

    private Mock<IReflectionService> _reflectionServiceDynamicMock;
    private Mock<ICodeManager> _codeManagerDynamicMock;

    private Type _fakeMixedType;
    private MixinConfiguration _configuration;


    [SetUp]
    public void SetUp ()
    {
      _assemblyOutputDirectoy = Path.Combine(AppContext.BaseDirectory, "MixerTest");
      _modulePath = Path.Combine(_assemblyOutputDirectoy, "Signed.dll");

      if (Directory.Exists(_assemblyOutputDirectoy))
        Directory.Delete(_assemblyOutputDirectoy, true);

      _mixedTypeFinderStub = new Mock<IMixedTypeFinder>();
      _mixerPipelineFactoryStub = new Mock<IMixerPipelineFactory>();
      _pipelineStub = new Mock<IPipeline>();

      _mixer = new Mixer(_mixedTypeFinderStub.Object, _mixerPipelineFactoryStub.Object, _assemblyOutputDirectoy);

      _reflectionServiceDynamicMock = new Mock<IReflectionService>();
      _codeManagerDynamicMock = new Mock<ICodeManager>();
      _pipelineStub.Setup(stub => stub.ReflectionService).Returns(_reflectionServiceDynamicMock.Object);
      _pipelineStub.Setup(stub => stub.CodeManager).Returns(_codeManagerDynamicMock.Object);

      _fakeMixedType = typeof(int);
      _configuration = new MixinConfiguration();

      _mixedTypeFinderStub.Setup(stub => stub.FindMixedTypes(_configuration)).Returns(new[] { _fakeMixedType });

      _mixerPipelineFactoryStub.Setup(stub => stub.GetModulePaths(_assemblyOutputDirectoy)).Returns(new[] { _modulePath });
      _mixerPipelineFactoryStub.Setup(stub => stub.CreatePipeline(_assemblyOutputDirectoy)).Returns(_pipelineStub.Object);
    }

    [TearDown]
    public void TearDown ()
    {
      if (Directory.Exists(_assemblyOutputDirectoy))
        Directory.Delete(_assemblyOutputDirectoy, true);
    }

    [Test]
    public void PrepareOutputDirectory_DirectoryDoesNotExist ()
    {
      Assert.That(Directory.Exists(_assemblyOutputDirectoy), Is.False);

      _mixer.PrepareOutputDirectory();

      Assert.That(Directory.Exists(_assemblyOutputDirectoy), Is.True);
    }

    [Test]
    public void PrepareOutputDirectory_DirectoryDoesExist ()
    {
      Directory.CreateDirectory(_assemblyOutputDirectoy);
      Assert.That(Directory.Exists(_assemblyOutputDirectoy), Is.True);

      _mixer.PrepareOutputDirectory();

      Assert.That(Directory.Exists(_assemblyOutputDirectoy), Is.True);
    }

    [Test]
    public void PrepareOutputDirectory_ModuleIsDeleted ()
    {
      Directory.CreateDirectory(_assemblyOutputDirectoy);
      CreateEmptyFile(_modulePath);

      Assert.That(File.Exists(_modulePath), Is.True);

      _mixer.PrepareOutputDirectory();

      Assert.That(File.Exists(_modulePath), Is.False);
    }

    [Test]
    public void Execute_FindsClassContexts ()
    {
      var mixedTypeFinderMock = new Mock<IMixedTypeFinder>(MockBehavior.Strict);
      mixedTypeFinderMock.Setup(mock => mock.FindMixedTypes(_configuration)).Returns(new Type[0]).Verifiable();
      _codeManagerDynamicMock.Setup(stub => stub.FlushCodeToDisk()).Returns(new string[0]);

      var mixer = new Mixer(mixedTypeFinderMock.Object, _mixerPipelineFactoryStub.Object, _assemblyOutputDirectoy);
      mixer.Execute(_configuration);

      mixedTypeFinderMock.Verify();
    }

    [Test]
    public void Execute_RaisesClassContextBeingProcessed ()
    {
      _codeManagerDynamicMock.Setup(stub => stub.FlushCodeToDisk()).Returns(new string[0]);
      object eventSender = null;
      TypeEventArgs eventArgs = null;

      _mixer.TypeBeingProcessed += (sender, args) =>
      {
        eventSender = sender;
        eventArgs = args;
      };
      _mixer.Execute(_configuration);

      Assert.That(eventSender, Is.SameAs(_mixer));
      Assert.That(eventArgs.Type, Is.SameAs(_fakeMixedType));
    }

    [Test]
    public void Execute_GetsConcreteType_WithActivatedConfiguration ()
    {
      MixinConfiguration activeConfiguration = null;

      _reflectionServiceDynamicMock
          .Setup(mock => mock.GetAssembledType(_fakeMixedType))
          .Returns(typeof(FakeConcreteMixedType))
          .Callback(() => activeConfiguration = MixinConfiguration.ActiveConfiguration)
          .Verifiable();
      _codeManagerDynamicMock.Setup(stub => stub.FlushCodeToDisk()).Returns(new string[0]);

      _mixer.Execute(_configuration);

      _reflectionServiceDynamicMock.Verify();
      Assert.That(activeConfiguration, Is.SameAs(_configuration));
      Assert.That(_mixer.FinishedTypes.Count, Is.EqualTo(1));
      Assert.That(_mixer.FinishedTypes[_fakeMixedType], Is.SameAs(typeof(FakeConcreteMixedType)));
    }

    [Test]
    public void Execute_ValidationError ()
    {
      var validationException = new ValidationException(new ValidationLogData());

      _reflectionServiceDynamicMock
          .Setup(mock => mock.GetAssembledType(_fakeMixedType))
          .Throws(validationException)
          .Verifiable();
      _codeManagerDynamicMock.Setup(stub => stub.FlushCodeToDisk()).Returns(new string[0]);

      object eventSender = null;
      ValidationErrorEventArgs eventArgs = null;

      _mixer.ValidationErrorOccurred += (sender, args) =>
      {
        eventSender = sender;
        eventArgs = args;
      };
      _mixer.Execute(_configuration);

      _reflectionServiceDynamicMock.Verify();

      Assert.That(eventSender, Is.SameAs(_mixer));
      Assert.That(eventArgs.ValidationException, Is.SameAs(validationException));
    }

    [Test]
    public void Execute_OtherError ()
    {
      var exception = new Exception("x");

      _reflectionServiceDynamicMock
          .Setup(mock => mock.GetAssembledType(_fakeMixedType))
          .Throws(exception)
          .Verifiable();
      _codeManagerDynamicMock.Setup(stub => stub.FlushCodeToDisk()).Returns(new string[0]);

      object eventSender = null;
      ErrorEventArgs eventArgs = null;

      _mixer.ErrorOccurred += (sender, args) =>
      {
        eventSender = sender;
        eventArgs = args;
      };
      _mixer.Execute(_configuration);

      _reflectionServiceDynamicMock.Verify();

      Assert.That(eventSender, Is.SameAs(_mixer));
      Assert.That(eventArgs.Exception, Is.SameAs(exception));
    }

    [Test]
    public void Execute_Saves ()
    {
      _codeManagerDynamicMock.Setup(mock => mock.FlushCodeToDisk()).Returns(new[] { "a" }).Verifiable();

      _mixer.Execute(_configuration);

      _codeManagerDynamicMock.Verify();
      Assert.That(_mixer.GeneratedFiles, Is.EqualTo(new[] { "a" }));
    }

    [Test]
    public void Create ()
    {
      var mixer = Mixer.Create("A", "D", 1);
      Assert.That(mixer.MixerPipelineFactory, Is.TypeOf(typeof(MixerPipelineFactory)));
      Assert.That(((MixerPipelineFactory)mixer.MixerPipelineFactory).AssemblyName, Is.EqualTo("A"));

      Assert.That(mixer.AssemblyOutputDirectory, Is.EqualTo("D"));

      Assert.That(mixer.MixedTypeFinder, Is.TypeOf(typeof(MixedTypeFinder)));
      Assert.That(((MixedTypeFinder)mixer.MixedTypeFinder).TypeDiscoveryService, Is.TypeOf(typeof(AssemblyFinderTypeDiscoveryService)));

      var service = (AssemblyFinderTypeDiscoveryService)((MixedTypeFinder)mixer.MixedTypeFinder).TypeDiscoveryService;
      Assert.That(service.AssemblyFinder, Is.TypeOf(typeof(CachingAssemblyFinderDecorator)));

      var assemblyFinder = (AssemblyFinder)((CachingAssemblyFinderDecorator)service.AssemblyFinder).InnerFinder;
      Assert.That(assemblyFinder.RootAssemblyFinder, Is.TypeOf(typeof(SearchPathRootAssemblyFinder)));
      var rootAssemblyFinder = ((SearchPathRootAssemblyFinder)assemblyFinder.RootAssemblyFinder);
      Assert.That(rootAssemblyFinder.BaseDirectory, Is.EqualTo(AppContext.BaseDirectory));
      Assert.That(rootAssemblyFinder.ConsiderDynamicDirectory, Is.False);
      Assert.That(rootAssemblyFinder.AssemblyLoader, Is.TypeOf(typeof(FilteringAssemblyLoader)));
      Assert.That(((FilteringAssemblyLoader)rootAssemblyFinder.AssemblyLoader).Filter, Is.TypeOf(typeof(LoadAllAssemblyLoaderFilter)));

      Assert.That(assemblyFinder.AssemblyLoader, Is.TypeOf(typeof(FilteringAssemblyLoader)));
      Assert.That(((FilteringAssemblyLoader)assemblyFinder.AssemblyLoader).Filter, Is.TypeOf(typeof(LoadAllAssemblyLoaderFilter)));
    }

    private void CreateEmptyFile (string path)
    {
      using (File.Create(path))
      {
      }
    }

    [IgnoreForMixinConfiguration]
    public class FakeConcreteMixedType : BaseType1
    {
      public FakeConcreteMixedType ()
      {
      }

      public ClassContext ClassContext
      {
        get { throw new NotImplementedException(); }
      }

      public object[] Mixins
      {
        get { throw new NotImplementedException(); }
      }

      public object FirstNextCallProxy
      {
        get { throw new NotImplementedException(); }
      }
    }
  }
}
