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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.NUnit;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Samples.DynamicMixinBuilding.Core;
using Remotion.ServiceLocation;
using Remotion.TypePipe;
using Remotion.TypePipe.Implementation;

namespace Remotion.Mixins.Samples.DynamicMixinBuilding.UnitTests
{
  [TestFixture]
  public class DynamicMixinBuilderTest
  {
    public class SampleTarget
    {
      public bool VoidMethodCalled = false;

      public virtual string StringMethod (int intArg)
      {
        return "SampleTarget.StringMethod (" + intArg + ")";
      }

      public virtual void VoidMethod ()
      {
        VoidMethodCalled = true;
      }
    }

    private static class ObjectFactory
    {
      public static T Create<T> (ParamList constructorParameters)
      {
        var objectFactoryImplementation = SafeServiceLocator.Current.GetInstance<IObjectFactoryImplementation>();
        return (T)objectFactoryImplementation.CreateInstance(false, typeof(T), constructorParameters);
      }
    }

    private readonly List<Tuple<object, MethodInfo, object[], object>> _calls = new List<Tuple<object, MethodInfo, object[], object>>();
    private MethodInvocationHandler _invocationHandler;
    private DynamicMixinBuilder _builder;
    private ServiceLocatorScope _serviceLocatorScope;
    private string _tempDirectory;

    [SetUp]
    public void SetUp ()
    {
      _tempDirectory = PrepareDirectory();

      // Force-initialize ObjectFactory to ensure it does not default onto the test pipeline.
      Dev.Null = Remotion.Mixins.ObjectFactory.Create<object>();

      // Set new default pipeline to avoid cached types to influence each other.
      var pipelineRegistry = CreatePipelineRegistry();
      var serviceLocator = DefaultServiceLocator.Create();
      // ReSharper disable once RedundantTypeArgumentsOfMethod
      serviceLocator.RegisterSingle<IPipelineRegistry>(() => pipelineRegistry);
      _serviceLocatorScope = new ServiceLocatorScope(serviceLocator);

      DynamicMixinBuilder.Scope = new ModuleScope(true, false, "DynamicMixinBuilder.Signed", Path.Combine(_tempDirectory, "DynamicMixinBuilder.Signed.dll"),
        "DynamicMixinBuilder.Unsigned", Path.Combine(_tempDirectory, "DynamicMixinBuilder.Unsigned.dll"));

      _invocationHandler = delegate (object instance, MethodInfo method, object[] args, BaseMethodInvoker baseMethod)
      {
        object result = baseMethod(args);
        _calls.Add(Tuple.Create(instance, method, args, result));
        return "Intercepted: " + result;
      };

      _calls.Clear();

      _builder = new DynamicMixinBuilder(typeof(SampleTarget));
    }

    private IPipelineRegistry CreatePipelineRegistry ()
    {
      var pipelineRegistry = SafeServiceLocator.Current.GetInstance<IPipelineRegistry>();
      var pipelineFactory = SafeServiceLocator.Current.GetInstance<IPipelineFactory>();
      var previousPipeline = pipelineRegistry.DefaultPipeline;

      var pipeline = pipelineFactory.Create(
          previousPipeline.ParticipantConfigurationID,
          previousPipeline.Settings,
          previousPipeline.Participants.ToArray());

      return new DefaultPipelineRegistry(pipeline);
    }

    private string PrepareDirectory ()
    {
      string directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
      Directory.CreateDirectory(directory);

      CopyFile(typeof(Mixin<,>).Assembly.ManifestModule.FullyQualifiedName, directory); // Core/Mixins assembly
      CopyFile(typeof(MethodInvocationHandler).Assembly.ManifestModule.FullyQualifiedName, directory); // Samples assembly
      return directory;
    }

    private void CopyFile (string sourcePath, string targetDirectory)
    {
      Assert.That(Directory.Exists(targetDirectory), Is.True);
      File.Copy(sourcePath, Path.Combine(targetDirectory, Path.GetFileName(sourcePath)));
    }

    [TearDown]
    public void TearDown ()
    {
#if FEATURE_ASSEMBLYBUILDER_SAVE
      if (DynamicMixinBuilder.Scope.StrongNamedModule != null)
      {
        DynamicMixinBuilder.Scope.SaveAssembly(true);
        PEVerifier.CreateDefault().VerifyPEFile(DynamicMixinBuilder.Scope.StrongNamedModule.FullyQualifiedName);
      }
      if (DynamicMixinBuilder.Scope.WeakNamedModule != null)
      {
        DynamicMixinBuilder.Scope.SaveAssembly(false);
        PEVerifier.CreateDefault().VerifyPEFile(DynamicMixinBuilder.Scope.WeakNamedModule.FullyQualifiedName);
      }
#endif
      _serviceLocatorScope.Dispose();
      Directory.Delete(_tempDirectory, true);
    }

    [Test]
    public void BuildMixinType_CreatesType ()
    {
      Type t = new DynamicMixinBuilder(typeof(object)).BuildMixinType(_invocationHandler);
      Assert.That(t, Is.Not.Null);
    }

    [Test]
    public void BuildMixinType_CreatesTypeDerivedFromMixin ()
    {
      Type t = new DynamicMixinBuilder(typeof(object)).BuildMixinType(_invocationHandler);
      Assert.That(Reflection.TypeExtensions.CanAscribeTo(t, typeof(Mixin<,>)), Is.True);
    }

    [Test]
    public void BuildMixinType_AddsMethodsWithOverrideAttribute ()
    {
      _builder.OverrideMethod(typeof(SampleTarget).GetMethod("StringMethod"));
      Type t = _builder.BuildMixinType(_invocationHandler);

      MethodInfo overriderMethod = t.GetMethod("StringMethod");
      Assert.That(overriderMethod, Is.Not.Null);
      Assert.That(overriderMethod.IsDefined(typeof(OverrideTargetAttribute), false), Is.True);
    }

    [Test]
    public void BuildMixinType_OverrideMethod_FromWrongType ()
    {
      Assert.That(
          () => _builder.OverrideMethod(typeof(object).GetMethod("ToString")),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The declaring type of the method must be the "
                  + "target type.", "method"));
    }

    [Test]
    public void GeneratedTypeHoldsHandler ()
    {
      Type t = _builder.BuildMixinType(_invocationHandler);
      FieldInfo handlerField = t.GetField("InvocationHandler");
      Assert.That(handlerField, Is.Not.Null);
      Assert.That(handlerField.GetValue(null), Is.SameAs(_invocationHandler));
    }

    [Test]
    public void GeneratedMethodIsIntercepted ()
    {
      _builder.OverrideMethod(typeof(SampleTarget).GetMethod("StringMethod"));
      Type t = _builder.BuildMixinType(_invocationHandler);

      using (MixinConfiguration.BuildFromActive().ForClass(typeof(SampleTarget)).Clear().AddMixins(t).EnterScope())
      {
        SampleTarget target = ObjectFactory.Create<SampleTarget>(ParamList.Empty);
        target.StringMethod(4);
        Assert.That(_calls.Count == 1, Is.True);
      }
    }

    [Test]
    public void GeneratedMethodIsIntercepted_WithRightParameters ()
    {
      _builder.OverrideMethod(typeof(SampleTarget).GetMethod("StringMethod"));
      Type t = _builder.BuildMixinType(_invocationHandler);

      using (MixinConfiguration.BuildFromActive().ForClass(typeof(SampleTarget)).Clear().AddMixins(t).EnterScope())
      {
        SampleTarget target = ObjectFactory.Create<SampleTarget>(ParamList.Empty);
        target.StringMethod(4);

        Tuple<object, MethodInfo, object[], object> callInfo = _calls[0];
        Assert.That(callInfo.Item1, Is.SameAs(target));
        Assert.That(callInfo.Item2, Is.EqualTo(typeof(SampleTarget).GetMethod("StringMethod")));
        Assert.That(callInfo.Item3, Is.EquivalentTo(new object[] { 4 } ));
      }
    }

    [Test]
    public void GeneratedMethodIsIntercepted_WithRightReturnValue ()
    {
      _builder.OverrideMethod(typeof(SampleTarget).GetMethod("StringMethod"));
      Type t = _builder.BuildMixinType(_invocationHandler);

      using (MixinConfiguration.BuildFromActive().ForClass(typeof(SampleTarget)).Clear().AddMixins(t).EnterScope())
      {
        SampleTarget target = ObjectFactory.Create<SampleTarget>(ParamList.Empty);
        target.StringMethod(4);

        Tuple<object, MethodInfo, object[], object> callInfo = _calls[0];
        Assert.That(callInfo.Item4, Is.EqualTo("SampleTarget.StringMethod (4)"));
      }
    }

    [Test]
    public void GeneratedMethodIsIntercepted_WithCorrectBase ()
    {
      _builder.OverrideMethod(typeof(SampleTarget).GetMethod("StringMethod"));
      Type t = _builder.BuildMixinType(_invocationHandler);

      using (MixinConfiguration.BuildFromActive().ForClass(typeof(SampleTarget)).Clear().AddMixins(t).EnterScope())
      {
        SampleTarget target = ObjectFactory.Create<SampleTarget>(ParamList.Empty);
        string result = target.StringMethod(4);
        Assert.That(result, Is.EqualTo("Intercepted: SampleTarget.StringMethod (4)"));
      }
    }

    [Test]
    public void InterceptVoidMethod ()
    {
      _builder.OverrideMethod(typeof(SampleTarget).GetMethod("VoidMethod"));
      Type t = _builder.BuildMixinType(_invocationHandler);

      using (MixinConfiguration.BuildFromActive().ForClass(typeof(SampleTarget)).Clear().AddMixins(t).EnterScope())
      {
        SampleTarget target = ObjectFactory.Create<SampleTarget>(ParamList.Empty);
        target.VoidMethod();
        Assert.That(target.VoidMethodCalled, Is.True);

        Tuple<object, MethodInfo, object[], object> callInfo = _calls[0];
        Assert.That(callInfo.Item4, Is.EqualTo(null));
      }
    }
  }
}
