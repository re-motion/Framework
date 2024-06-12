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

#if NETFRAMEWORK

using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Remoting;
using System.Runtime.Serialization;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Remotion.Utilities;

namespace Remotion.Tools.UnitTests
{
  [TestFixture]
#if NETFRAMEWORK
  // Required for test CreateInAppDomain_FromShadowCopiedScenario
  [Serializable]
#endif  
  public class AppDomainAssemblyResolverTest
  {
    private string _testDllPath;
    private string _testExePath;
    private string _testInvalidDllPath;

    private string _domainBase;
    private AppDomain _appDomain;

    [OneTimeSetUp]
    public void OneTimeSetUp ()
    {
      _testDllPath = CreateAssembly("TestDll", "TestDll" + "." + "dll");
      _testExePath = CreateAssembly("TestExe", "TestExe" + "." + "exe");
      _testInvalidDllPath = CreateAssembly("TestDllWhatever", "TestDllInvalid" + "." + "dll");
    }

    [OneTimeTearDown]
    public void TestFixtureTearDown ()
    {
      FileUtility.DeleteAndWaitForCompletion(_testDllPath);
      FileUtility.DeleteAndWaitForCompletion(_testExePath);
      FileUtility.DeleteAndWaitForCompletion(_testInvalidDllPath);
    }

    [SetUp]
    public void SetUp ()
    {
      _domainBase = Path.Combine(AppContext.BaseDirectory, "AppDomainAsselbyResolverTest");
      _appDomain = AppDomain.CreateDomain("Test", null, _domainBase, null, false);
    }

    [TearDown]
    public void TearDown ()
    {
      AppDomain.Unload(_appDomain);

      if (Directory.Exists(_domainBase))
        Directory.Delete(_domainBase, true);
    }

    [Test]
    public void CreateInAppDomain ()
    {
      var resolver = AppDomainAssemblyResolver.CreateInAppDomain(_appDomain, AppContext.BaseDirectory);
      Assert.That(RemotingServices.IsTransparentProxy(resolver), Is.True);
      Assert.That(resolver.AssemblyDirectory, Is.EqualTo(AppContext.BaseDirectory));
    }

    [Test]
    public void CreateInAppDomain_FromShadowCopiedScenario ()
    {
      var setupInfo = AppDomain.CurrentDomain.SetupInformation;
      setupInfo.ShadowCopyFiles = "true";
      var shadowCopiedAppDomain = AppDomain.CreateDomain("ShadowCopier", AppDomain.CurrentDomain.Evidence, setupInfo);
      try
      {
        shadowCopiedAppDomain.DoCallBack(
            delegate
            {
              var resolver = AppDomainAssemblyResolver.CreateInAppDomain(_appDomain, AppContext.BaseDirectory);
              Assert.That(resolver, Is.Not.Null);
            });
      }
      finally
      {
        AppDomain.Unload(shadowCopiedAppDomain);
      }
    }

    [Test]
    public void Register_AllowsAssembliesToBeResolved ()
    {
      var resolver = CreateResolver();

      ExactTypeConstraint constraint = Throws.Exception.TypeOf<FileNotFoundException>().Or.TypeOf<SerializationException>();
      Assert.That(() => _appDomain.DoCallBack(delegate { }), constraint); // Serialized type should not be available in the other AppDomain

      resolver.Register(_appDomain);

      _appDomain.DoCallBack(delegate { });
    }

    [Test]
    public void Resolve_FindsDlls ()
    {
      var resolver = CreateResolver();

      resolver.Register(_appDomain);

      _appDomain.DoCallBack(() => Assembly.Load("TestDll"));
    }

    [Test]
    public void Resolve_FindsExes ()
    {
      var resolver = CreateResolver();

      resolver.Register(_appDomain);

      _appDomain.DoCallBack(() => Assembly.Load("TestExe"));
    }

    [Test]
    public void Resolve_NonExistingAssembly ()
    {
      var resolver = CreateResolver();

      resolver.Register(_appDomain);
      Assert.That(
          () => _appDomain.DoCallBack(() => Assembly.Load("TestTxt")),
          Throws.InstanceOf<FileNotFoundException>()
              .With.Message.EqualTo(
                  "Could not load file or assembly 'TestTxt' or one of its dependencies. The system cannot find the file specified."));
    }

    [Test]
    public void Resolve_ManifestDoesntMatch ()
    {
      var resolver = CreateResolver();

      resolver.Register(_appDomain);
      Assert.That(
          () => _appDomain.DoCallBack(() => Assembly.Load("TestDllInvalid")),
          Throws.InstanceOf<FileLoadException>()
              .With.Message.EqualTo(
                  "Could not load file or assembly 'TestDllInvalid' or one of its dependencies. Could not find or load a specific file. (Exception from HRESULT: 0x80131621)"));
    }

    private AppDomainAssemblyResolver CreateResolver ()
    {
      return AppDomainAssemblyResolver.CreateInAppDomain(_appDomain, AppContext.BaseDirectory);
    }

    private string CreateAssembly (string assemblyName, string moduleName)
    {
      var targetDirectory = AppContext.BaseDirectory;
      var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Save, targetDirectory);
      assemblyBuilder.DefineDynamicModule(moduleName);
      assemblyBuilder.Save(moduleName);
      return Path.Combine(targetDirectory, moduleName);
    }
  }
}
#endif
