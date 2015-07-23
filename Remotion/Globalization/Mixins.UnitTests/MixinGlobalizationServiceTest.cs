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
using System.Threading.Tasks;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting;
using Remotion.Globalization.Implementation;
using Remotion.Globalization.Mixins.UnitTests.TestDomain;
using Remotion.Mixins;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Globalization.Mixins.UnitTests
{
  [TestFixture]
  public class MixinGlobalizationServiceTest
  {
    private MixinGlobalizationService _globalizationService;

    [SetUp]
    public void SetUp ()
    {
      _globalizationService = new MixinGlobalizationService (new ResourceManagerResolver(new ResourceAttributeBasedResourceManagerFactory()));
    }

    [Test]
    public void GetResourceManager_WithTypeNotSupportingConversionFromITypeInformationToType ()
    {
      var typeInformation = MockRepository.GenerateStub<ITypeInformation>();

      var result = _globalizationService.GetResourceManager (typeInformation);

      Assert.That (result, Is.TypeOf (typeof (NullResourceManager)));
    }

    [Test]
    public void GetResourceManager_TypeWithoutMixin_ReturnsNullResourceManager ()
    {
      var typeInformation = TypeAdapter.Create (typeof (ClassWithResources));

      var result = _globalizationService.GetResourceManager (typeInformation);

      Assert.That (result, Is.TypeOf (typeof (NullResourceManager)));
    }

    [Test]
    public void GetResourceManager_TypeWithMixin_NoResourceAttribute_ReturnsResourceManagerSetWithEmptyResourceManagerCollection ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes>()
          .AddMixin<MixinWithoutResourceAttribute>()
          .EnterScope())
      {
        var typeInformation = TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes));

        var result = (ResourceManagerSet) _globalizationService.GetResourceManager (typeInformation);

        Assert.That (result.ResourceManagers.Any(), Is.False);
      }
    }

    [Test]
    public void GetResourceManager_TypeWithMixin_WithResourceAttribute_ReturnsResourceManagerSet ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1>()
          .EnterScope())
      {
        var typeInformation = TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes));

        var result = (ResourceManagerSet) _globalizationService.GetResourceManager (typeInformation);

        var innerResourceManager = result.ResourceManagers.Single();
        Assert.That (innerResourceManager, Is.InstanceOf<ResourceManagerWrapper>());
        Assert.That (innerResourceManager.Name, Is.EqualTo (NamedResources.OnMixin1));
      }
    }

    [Test]
    public void GetResourceManager_TypeWithInheritedMixin_WithResourceAttribute_ReturnsResourceManagerSet ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<ClassWithMultiLingualResourcesAttributes>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1>()
          .EnterScope())
      {
        var typeInformation = TypeAdapter.Create (typeof (InheritedClassWithoutMultiLingualResourcesAttributes));

        var result = (ResourceManagerSet) _globalizationService.GetResourceManager (typeInformation);

        var innerResourceManager = result.ResourceManagers.Single();
        Assert.That (innerResourceManager, Is.InstanceOf<ResourceManagerWrapper>());
        Assert.That (innerResourceManager.Name, Is.EqualTo (NamedResources.OnMixin1));
      }
    }

    [Test]
    public void GetResourceManagerTwice_TypeWithDynamicMixinScope_NotSameButEqual ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1>()
          .EnterScope())
      {
        var typeInformation = TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes));

        var result1 = (ResourceManagerSet) _globalizationService.GetResourceManager (typeInformation);
        var result2 = (ResourceManagerSet) _globalizationService.GetResourceManager (typeInformation);

        Assert.That (result1, Is.Not.SameAs (result2));
        Assert.That (result1.Name, Is.EqualTo (result2.Name));
        Assert.That (result1.ResourceManagers.Count(), Is.EqualTo (1));
        Assert.That (result2.ResourceManagers.Count(), Is.EqualTo (1));
        Assert.That (result1.ResourceManagers.Single().GetType(), Is.EqualTo (result2.ResourceManagers.Single().GetType()));
      }
    }

    [Test]
    public void GetResourceManagerTwice_TypeWithStaticMixinAndSameMixinMasterConfiguration_Same ()
    {
      var typeInformation = TypeAdapter.Create (typeof (ClassWithMixinResource));

      var result1 = _globalizationService.GetResourceManager (typeInformation);
      var result2 = _globalizationService.GetResourceManager (typeInformation);
      Assert.That (result1, Is.SameAs (result2));
    }

    [Test]
    public void GetResourceManagerTwice_TypeWithStaticMixinAndNotSameMixinMasterConfiguration_NotSame ()
    {
      var typeInformation = TypeAdapter.Create (typeof (ClassWithMixinResource));

      var result1 = _globalizationService.GetResourceManager (typeInformation);
      MixinConfiguration.ResetMasterConfiguration();
      MixinConfiguration.SetActiveConfiguration (null);
      var result2 = _globalizationService.GetResourceManager (typeInformation);

      Assert.That (result1, Is.Not.SameAs (result2));
    }

    [Test]
    public void GetResourceManagerTwice_TypeWithMixin_DifferentMixinConfiguration_NotSame ()
    {
      var typeInformation = TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes));
      ResourceManagerSet result1;

      var mixinConfiguration = MixinConfiguration.BuildFromActive()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1>();

      using (mixinConfiguration.EnterScope())
      {
        result1 = (ResourceManagerSet) _globalizationService.GetResourceManager (typeInformation);
      }

      ResourceManagerSet result2;
      using (mixinConfiguration.EnterScope())
      {
        result2 = (ResourceManagerSet) _globalizationService.GetResourceManager (typeInformation);
      }

      Assert.That (result1, Is.Not.SameAs (result2));
    }

    [Test]
    public void GetResourceManagerTwice_WithAndWithoutMixinConfiguration_NotSame ()
    {
      var typeInformation = TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes));
      IResourceManager result1;
      using (MixinConfiguration.BuildFromActive()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1>().EnterScope())
      {
        result1 = _globalizationService.GetResourceManager (typeInformation);
      }

      var result2 = _globalizationService.GetResourceManager (typeInformation);

      Assert.That (result1, Is.Not.SameAs (result2));
    }

    [Test]
    public void GetResourceManagerTwice_NewMixinConfigurationWithinConfiguration_NotSame ()
    {
      var typeInformation = TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes));
      IResourceManager result1;
      IResourceManager result2;
      using (MixinConfiguration.BuildFromActive()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1>().EnterScope())
      {
        result1 = _globalizationService.GetResourceManager (typeInformation);

        using (MixinConfiguration.BuildFromActive()
            .ForClass<ClassWithoutMultiLingualResourcesAttributes>()
            .AddMixin<MixinAddingMultiLingualResourcesAttributes1>().EnterScope())
        {
          result2 = _globalizationService.GetResourceManager (typeInformation);
        }
      }

      Assert.That (result1, Is.Not.SameAs (result2));
    }

    [Test]
    public void GetResourceManager_TypeWithMixinOfMixin_WithResourceAttribute_ReturnsResourceManagerSet ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes>()
          .AddMixin<MixinWithoutResourceAttribute>()
          .ForClass<MixinWithoutResourceAttribute>()
          .AddMixin<MixinOfMixinWithResources>()
          .EnterScope())
      {
        var typeInformation = TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes));

        var result = (ResourceManagerSet) _globalizationService.GetResourceManager (typeInformation);

        var innerResourceManager = result.ResourceManagers.Single();
        Assert.That (innerResourceManager, Is.InstanceOf<ResourceManagerWrapper>());
        Assert.That (innerResourceManager.Name, Is.EqualTo (NamedResources.MixinOfMixinWithResources));
      }
    }

    [Test]
    public void GetResourceManager_TypeWithMultipleMixins_ReturnsResourceManagerSet ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1>()
          .AddMixin<MixinWithoutResourceAttribute>()
          .ForClass<MixinWithoutResourceAttribute>()
          .AddMixin<MixinOfMixinWithResources>()
          .EnterScope())
      {
        var typeInformation = TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes));

        var result = (ResourceManagerSet) _globalizationService.GetResourceManager (typeInformation);

        Assert.That (result.ResourceManagers.Count(), Is.EqualTo (2));
      }
    }

    [Test]
    public void GetResourceManager_TypeWithMixinWithMultipleAttributes_ReturnsResourceManagerSet ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes2>()
          .EnterScope())
      {
        var typeInformation = TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes));

        var result = (ResourceManagerSet) _globalizationService.GetResourceManager (typeInformation);

        Assert.That (result.ResourceManagers.Count(), Is.EqualTo (2));
      }
    }

    [Test]
    public void GetResourceManager_TypeWithMultipleMixins_ReturnsMostSpecificResourceManagerFirst ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1>()
          .AddMixin<MixinWithoutResourceAttribute>()
          .ForClass<MixinWithoutResourceAttribute>()
          .AddMixin<MixinOfMixinWithResources>()
          .EnterScope())
      {
        var typeInformation = TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes));

        var result = (ResourceManagerSet) _globalizationService.GetResourceManager (typeInformation);

        Assert.That (result.ResourceManagers.First().Name, Is.EqualTo (NamedResources.MixinOfMixinWithResources));
      }
    }

    
    [Test]
    public void GetResourceManager_ChangingTheMasterConfigurationOnADifferentThreadIsRecognizedOnOtherThreads_CacheIsNotResetForCurrentFieldValue()
    {
      var backup = MixinConfiguration.GetMasterConfiguration();
      try
      {
        var typeInformation1 = TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes));
        var typeInformation2 = TypeAdapter.Create (typeof (InheritedClassWithoutMultiLingualResourcesAttributes));
        var resourceManagerCache =
            (ICache<ITypeInformation, IResourceManager>) PrivateInvoke.GetNonPublicField (_globalizationService, "_resourceManagerCache");
        IResourceManager outValue;

        var newMasterConfiguration = MixinConfiguration.BuildNew().BuildConfiguration();

        Dev.Null = _globalizationService.GetResourceManager (typeInformation1);

        Assert.That (resourceManagerCache.TryGetValue (typeInformation1, out outValue), Is.True);
        Assert.That (outValue, Is.Not.Null);
        Assert.That (resourceManagerCache.TryGetValue (typeInformation2, out outValue), Is.False);
        Assert.That (outValue, Is.Null);

        var task = Task.Run (
            () =>
            {
              MixinConfiguration.SetMasterConfiguration (newMasterConfiguration);
              MixinConfiguration.SetActiveConfiguration (newMasterConfiguration);

              // Populate the cache.
              Dev.Null = _globalizationService.GetResourceManager (typeInformation1);
              Dev.Null = _globalizationService.GetResourceManager (typeInformation2);

              Assert.That (resourceManagerCache.TryGetValue (typeInformation1, out outValue), Is.True);
              Assert.That (outValue, Is.Not.Null);
              Assert.That (resourceManagerCache.TryGetValue (typeInformation2, out outValue), Is.True);
              Assert.That (outValue, Is.Not.Null);
            });

        task.Wait();

        MixinConfiguration.SetActiveConfiguration (newMasterConfiguration);

        // Getting a ResourceManager does not reset the cache because this already happened on other thread.
        Dev.Null = _globalizationService.GetResourceManager (typeInformation1);

        Assert.That (resourceManagerCache.TryGetValue (typeInformation1, out outValue), Is.True);
        Assert.That (outValue, Is.Not.Null);
        Assert.That (resourceManagerCache.TryGetValue (typeInformation2, out outValue), Is.True);
        Assert.That (outValue, Is.Not.Null);
      }
      finally
      {
        MixinConfiguration.SetMasterConfiguration (backup);
        MixinConfiguration.SetActiveConfiguration (backup);
      }
    }

    [Test]
    public void GetResourceManager_ChangingTheMasterConfigurationOnADifferentThreadIsRecognizedOnOtherThreads_CacheIsResetForPossiblyStaleFieldValue()
    {
      var backup = MixinConfiguration.GetMasterConfiguration();
      try
      {
        var typeInformation1 = TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes));
        var typeInformation2 = TypeAdapter.Create (typeof (InheritedClassWithoutMultiLingualResourcesAttributes));
        var resourceManagerCache =
            (ICache<ITypeInformation, IResourceManager>) PrivateInvoke.GetNonPublicField (_globalizationService, "_resourceManagerCache");
        IResourceManager outValue;

        var newMasterConfiguration = MixinConfiguration.BuildNew().BuildConfiguration();
        MixinConfiguration.SetMasterConfiguration (newMasterConfiguration);
        MixinConfiguration.SetActiveConfiguration (newMasterConfiguration);

        Dev.Null = _globalizationService.GetResourceManager (typeInformation1);

        Assert.That (resourceManagerCache.TryGetValue (typeInformation1, out outValue), Is.True);
        Assert.That (resourceManagerCache.TryGetValue (typeInformation2, out outValue), Is.False);

        var task = Task.Run (
            () =>
            {
              var otherMasterConfiguration = MixinConfiguration.BuildNew().BuildConfiguration();
              MixinConfiguration.SetMasterConfiguration (otherMasterConfiguration);
              MixinConfiguration.SetActiveConfiguration (otherMasterConfiguration);

              // Populate the cache.
              Dev.Null = _globalizationService.GetResourceManager (typeInformation1);
              Dev.Null = _globalizationService.GetResourceManager (typeInformation2);

              Assert.That (resourceManagerCache.TryGetValue (typeInformation1, out outValue), Is.True);
              Assert.That (resourceManagerCache.TryGetValue (typeInformation2, out outValue), Is.True);
            });
        task.Wait();

        MixinConfiguration.SetMasterConfiguration (newMasterConfiguration);
        MixinConfiguration.SetActiveConfiguration (newMasterConfiguration);

        // Getting a ResourceManager does not reset the cache because this already happened on other thread.
        Dev.Null = _globalizationService.GetResourceManager (typeInformation1);

        Assert.That (resourceManagerCache.TryGetValue (typeInformation1, out outValue), Is.True);
        Assert.That (resourceManagerCache.TryGetValue (typeInformation2, out outValue), Is.False);
      }
      finally
      {
        MixinConfiguration.SetMasterConfiguration (backup);
        MixinConfiguration.SetActiveConfiguration (backup);
      }
    }
  }
}