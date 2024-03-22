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
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.ServiceLocation;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.UrlMapping;
using Remotion.Web.UnitTests.Core.Utilities;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.UrlMapping
{

  [TestFixture]
  public class UrlMappingConfigurationTest
  {
    [SetUp]
    public virtual void SetUp ()
    {
      UrlMappingConfiguration.SetCurrent(null);
    }

    [TearDown]
    public virtual void TearDown ()
    {
      UrlMappingConfiguration.SetCurrent(null);
    }

    [Test]
    public void LoadMappingFromFile ()
    {
      UrlMappingConfiguration mapping = UrlMappingConfigurationUtility.CreateUrlMappingConfiguration(@"Res\UrlMapping.xml");

      Assert.IsNotNull(mapping, "Mapping is null.");

      Assert.IsNotNull(mapping.Mappings, "Rules are null.");
      Assert.That(mapping.Mappings.Count, Is.EqualTo(3));

      Assert.IsNotNull(mapping.Mappings[0], "First rule is null.");
      Assert.IsNotNull(mapping.Mappings[1], "Second rule is null.");
      Assert.IsNotNull(mapping.Mappings[2], "Thrid rule is null.");

      Assert.That(mapping.Mappings[0].ID, Is.EqualTo("First"));
      Assert.That(mapping.Mappings[0].FunctionType, Is.EqualTo(typeof(FirstMappedFunction)));
      Assert.That(mapping.Mappings[0].Resource, Is.EqualTo("~/First.wxe"));

      Assert.That(mapping.Mappings[1].ID, Is.EqualTo("Second"));
      Assert.That(mapping.Mappings[1].FunctionType, Is.EqualTo(typeof(SecondMappedFunction)));
      Assert.That(mapping.Mappings[1].Resource, Is.EqualTo("~/Second.wxe"));

      Assert.That(mapping.Mappings[2].ID, Is.Null);
      Assert.That(mapping.Mappings[2].FunctionType, Is.EqualTo(typeof(FirstMappedFunction)));
      Assert.That(mapping.Mappings[2].Resource, Is.EqualTo("~/Primary.wxe"));
    }

    [Test]
    public void LoadMappingFromFileWithInvalidFilename ()
    {
      Assert.That(
          () => UrlMappingConfigurationUtility.CreateUrlMappingConfiguration(@"Res\InvalidFilename.xml"),
          Throws.InstanceOf<FileNotFoundException>());
    }

    [Test]
    public void GetCurrentMapping ()
    {
      UrlMappingConfiguration.SetCurrent(null!);

      var fileFinder = BaseDirectoryBasedUrlMappingFileFinder.Create(@"Res\UrlMapping.xml");
      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle<IUrlMappingFileFinder>(() => fileFinder);
      using var _ = new ServiceLocatorScope(serviceLocator);

      UrlMappingConfiguration mapping = UrlMappingConfiguration.Current;
      Assert.That(mapping, Is.Not.Null);
    }

    [Test]
    public void GetCurrentMappingFromConfiguration ()
    {
      UrlMappingConfiguration mapping = UrlMappingConfigurationUtility.CreateUrlMappingConfiguration(@"Res\UrlMapping.xml");

      Assert.IsNotNull(mapping, "Mapping is null.");

      Assert.IsNotNull(mapping.Mappings, "Rules are null.");
      Assert.That(mapping.Mappings.Count, Is.EqualTo(3));

      Assert.IsNotNull(mapping.Mappings[0], "First rule is null.");
      Assert.IsNotNull(mapping.Mappings[1], "Second rule is null.");
      Assert.IsNotNull(mapping.Mappings[2], "Thrid rule is null.");

      Assert.That(mapping.Mappings[0].ID, Is.EqualTo("First"));
      Assert.That(mapping.Mappings[0].FunctionType, Is.EqualTo(typeof(FirstMappedFunction)));
      Assert.That(mapping.Mappings[0].Resource, Is.EqualTo("~/First.wxe"));

      Assert.That(mapping.Mappings[1].ID, Is.EqualTo("Second"));
      Assert.That(mapping.Mappings[1].FunctionType, Is.EqualTo(typeof(SecondMappedFunction)));
      Assert.That(mapping.Mappings[1].Resource, Is.EqualTo("~/Second.wxe"));

      Assert.That(mapping.Mappings[2].ID, Is.Null);
      Assert.That(mapping.Mappings[2].FunctionType, Is.EqualTo(typeof(FirstMappedFunction)));
      Assert.That(mapping.Mappings[2].Resource, Is.EqualTo("~/Primary.wxe"));
    }

    [Test]
    public void FindByFunctionType ()
    {
      UrlMappingCollection mappings = UrlMappingConfigurationUtility.CreateUrlMappingConfiguration(@"Res\UrlMapping.xml").Mappings;

      UrlMappingEntry entry = mappings[0];
      Assert.AreSame(mappings[0], mappings.Find(entry.FunctionType), "Could not find {0}.", entry.FunctionType.FullName);

      entry = mappings[1];
      Assert.AreSame(mappings[1], mappings.Find(entry.FunctionType), "Could not find {0}.", entry.FunctionType.FullName);

      entry = mappings[2];
      Assert.AreSame(mappings[0], mappings.Find(entry.FunctionType), "Could not find {0}.", entry.FunctionType.FullName);

      Assert.IsNull(mappings.Find(typeof(UnmappedFunction)), "Found mapping for unmapped function.");
    }

    [Test]
    public void FindByResource ()
    {
      UrlMappingCollection mappings = UrlMappingConfigurationUtility.CreateUrlMappingConfiguration(@"Res\UrlMapping.xml").Mappings;

      UrlMappingEntry entry = mappings[0];
      Assert.AreSame(mappings[0], mappings.Find(entry.Resource), "Could not find {0}.", entry.Resource);

      entry = mappings[1];
      Assert.AreSame(mappings[1], mappings.Find(entry.Resource), "Could not find {0}.", entry.Resource);

      entry = mappings[2];
      Assert.AreSame(mappings[2], mappings.Find(entry.Resource), "Could not find {0}.", entry.Resource);

      Assert.IsNull(mappings.Find("~/unmapped.wxe"), "Found mapping for unmapped resource.");
    }

    [Test]
    public void FindByID ()
    {
      UrlMappingCollection mappings = UrlMappingConfigurationUtility.CreateUrlMappingConfiguration(@"Res\UrlMapping.xml").Mappings;

      UrlMappingEntry entry = mappings[0];
      Assert.AreSame(mappings[0], mappings.FindByID(entry.ID), "Could not find {0}.", entry.ID);

      entry = mappings[1];
      Assert.AreSame(mappings[1], mappings.FindByID(entry.ID), "Could not find {0}.", entry.Resource);

      Assert.IsNull(mappings.FindByID("unknown"), "Found mapping for unknown id.");
    }
  }
}
