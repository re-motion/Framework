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
using Remotion.Configuration.TypeDiscovery;
using Remotion.Development.UnitTesting.Configuration;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Rhino.Mocks;

namespace Remotion.UnitTests.Configuration.TypeDiscovery
{
  [TestFixture]
  public class RootAssembliesElementTest
  {
    private IAssemblyLoader _loaderStub;
    private const string _emptyXmlFragment = @"<rootAssemblies />";
    private const string _xmlFragmentWithFilesAndNames = @"<rootAssemblies>
            <byName>
              <include name=""mscorlib"" />
            </byName>
            <byFile>
              <include filePattern=""ActaNova.*.dll"" />
            </byFile>
          </rootAssemblies>";

    [SetUp]
    public void SetUp ()
    {
      _loaderStub = MockRepository.GenerateStub<IAssemblyLoader> ();
    }

    [Test]
    public void Deserialization_WithEmptyFragment ()
    {
      var element = DeserializeFromXmlFragment (_emptyXmlFragment);
      Assert.That (element.ByName.Count, Is.EqualTo (0));
      Assert.That (element.ByFile.Count, Is.EqualTo (0));
    }

    [Test]
    public void Deserialization_WithNonEmptyFragment ()
    {
      var element = DeserializeFromXmlFragment(_xmlFragmentWithFilesAndNames);
      Assert.That (element.ByName.Count, Is.EqualTo (1));
      Assert.That (element.ByName.First().Name, Is.EqualTo ("mscorlib"));
      Assert.That (element.ByFile.Count, Is.EqualTo (1));
      Assert.That (element.ByFile.First().FilePattern, Is.EqualTo ("ActaNova.*.dll"));
    }

    [Test]
    public void CreateRootAssemblyFinder_Empty ()
    {
      var element = DeserializeFromXmlFragment (_emptyXmlFragment);

      var finder = element.CreateRootAssemblyFinder (_loaderStub);
      Assert.That (finder.InnerFinders.Count, Is.EqualTo (2));

      Assert.That (finder.InnerFinders[0], Is.InstanceOf (typeof (NamedRootAssemblyFinder)));
      var namedFinder = (NamedRootAssemblyFinder) finder.InnerFinders[0];
      Assert.That (namedFinder.Specifications, Is.Empty);
      Assert.That (namedFinder.AssemblyLoader, Is.SameAs (_loaderStub));

      var filePatternFinder = (FilePatternRootAssemblyFinder) finder.InnerFinders[1];
      Assert.That (filePatternFinder.Specifications, Is.Empty);
      Assert.That (filePatternFinder.AssemblyLoader, Is.SameAs (_loaderStub));

      Assert.That (finder.FindRootAssemblies (), Is.Empty);
    }

    [Test]
    public void CreateRootAssemblyFinder_NonEmpty ()
    {
      var element = DeserializeFromXmlFragment (_xmlFragmentWithFilesAndNames);

      var finder = element.CreateRootAssemblyFinder (_loaderStub);
      Assert.That (finder.InnerFinders.Count, Is.EqualTo (2));

      Assert.That (finder.InnerFinders[0], Is.InstanceOf (typeof (NamedRootAssemblyFinder)));
      var namedFinder = (NamedRootAssemblyFinder) finder.InnerFinders[0];
      Assert.That (namedFinder.Specifications.Single ().AssemblyName.ToString (), Is.EqualTo ("mscorlib"));
      Assert.That (namedFinder.AssemblyLoader, Is.SameAs (_loaderStub));

      var filePatternFinder = (FilePatternRootAssemblyFinder) finder.InnerFinders[1];
      Assert.That (filePatternFinder.Specifications.Single ().FilePattern, Is.EqualTo ("ActaNova.*.dll"));
      Assert.That (filePatternFinder.AssemblyLoader, Is.SameAs (_loaderStub));
    }

    private RootAssembliesElement DeserializeFromXmlFragment (string xmlFragment)
    {
      var element = new RootAssembliesElement ();
      ConfigurationHelper.DeserializeElement (element, xmlFragment);
      return element;
    }
  }
}
