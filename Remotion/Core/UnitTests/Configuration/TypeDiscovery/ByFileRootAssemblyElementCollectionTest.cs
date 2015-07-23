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
using System.Configuration;
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
  public class ByFileRootAssemblyElementCollectionTest
  {
    private const string _xmlFragment = @"<byFile>
              <include filePattern=""ActaNova.*.dll"" />
              <include filePattern=""Remotion.*.dll"" includeReferencedAssemblies=""true"" />
              <exclude filePattern=""Remotion.*.Utilities.dll"" />
            </byFile>";

    [Test]
    public void Deserialization ()
    {
      var collection = DeserializeFromXmlFragment (_xmlFragment);
      ByFileRootAssemblyElementBase[] result = collection.ToArray();

      Assert.That (result.Length, Is.EqualTo (3));
      Assert.That (result[0].FilePattern, Is.EqualTo ("ActaNova.*.dll"));
      Assert.That (result[1].FilePattern, Is.EqualTo ("Remotion.*.dll"));
      Assert.That (result[2].FilePattern, Is.EqualTo ("Remotion.*.Utilities.dll"));
    }

    [Test]
    public void Deserialization_Types ()
    {
      var collection = DeserializeFromXmlFragment (_xmlFragment);
      ByFileRootAssemblyElementBase[] result = collection.ToArray ();

      Assert.That (result.Length, Is.EqualTo (3));
      Assert.That (result[0], Is.InstanceOf (typeof (ByFileIncludeRootAssemblyElement)));
      Assert.That (result[1], Is.InstanceOf (typeof (ByFileIncludeRootAssemblyElement)));
      Assert.That (result[2], Is.InstanceOf (typeof (ByFileExcludeRootAssemblyElement)));
    }

    [Test]
    public void IncludeReferencedAssemblies_FalseByDefault ()
    {
      var collection = DeserializeFromXmlFragment (_xmlFragment);

      ByFileRootAssemblyElementBase[] result = collection.ToArray ();
      Assert.That (((ByFileIncludeRootAssemblyElement) result[0]).IncludeReferencedAssemblies, Is.False);
    }

    [Test]
    public void IncludeReferencedAssemblies_TrueIfSpecified ()
    {
      var collection = DeserializeFromXmlFragment (_xmlFragment);

      ByFileRootAssemblyElementBase[] result = collection.ToArray ();
      Assert.That (((ByFileIncludeRootAssemblyElement) result[1]).IncludeReferencedAssemblies, Is.True);
    }

    [Test]
    [ExpectedException (
        typeof (ConfigurationErrorsException), 
        ExpectedMessage = "Unrecognized attribute 'includeReferencedAssemblies'.", 
        MatchType = MessageMatch.Contains)]
    public void IncludeReferencedAssemblies_NotValidWithExclude ()
    {
      const string xmlFragment = @"<byFile>
              <exclude filePattern=""Remotion.*.Utilities.dll"" includeReferencedAssemblies=""true""/>
            </byFile>";

      DeserializeFromXmlFragment (xmlFragment);
    }

    [Test]
    public void Add ()
    {
      var element1 = new ByFileIncludeRootAssemblyElement { FilePattern = "*.dll", IncludeReferencedAssemblies = true };
      var element2 = new ByFileIncludeRootAssemblyElement { FilePattern = "*.exe" };
      var element3 = new ByFileExcludeRootAssemblyElement { FilePattern = "Utilities.exe" };
      
      var collection = new ByFileRootAssemblyElementCollection ();
      collection.Add (element1);
      collection.Add (element2);
      collection.Add (element3);

      ByFileRootAssemblyElementBase[] result = collection.ToArray ();
      Assert.That (result, Is.EqualTo (new ByFileRootAssemblyElementBase[] { element1, element2, element3 }));
    }

    [Test]
    public void RemoveAt ()
    {
      var element1 = new ByFileIncludeRootAssemblyElement { FilePattern = "*.dll", IncludeReferencedAssemblies = true };
      var element2 = new ByFileIncludeRootAssemblyElement { FilePattern = "*.exe" };
      var element3 = new ByFileExcludeRootAssemblyElement { FilePattern = "Utilities.exe" };

      var collection = new ByFileRootAssemblyElementCollection ();
      collection.Add (element1);
      collection.Add (element2);
      collection.Add (element3);
      collection.RemoveAt (1);

      ByFileRootAssemblyElementBase[] result = collection.ToArray ();
      Assert.That (result, Is.EquivalentTo (new ByFileRootAssemblyElementBase[] { element1, element3 }));
    }

    [Test]
    public void Clear ()
    {
      var element1 = new ByFileIncludeRootAssemblyElement { FilePattern = "*.dll", IncludeReferencedAssemblies = true };
      var element2 = new ByFileIncludeRootAssemblyElement { FilePattern = "*.exe" };
      var element3 = new ByFileExcludeRootAssemblyElement { FilePattern = "Utilities.exe" };

      var collection = new ByFileRootAssemblyElementCollection ();
      collection.Add (element1);
      collection.Add (element2);
      collection.Add (element3);
      collection.Clear ();

      ByFileRootAssemblyElementBase[] result = collection.ToArray ();
      Assert.That (result, Is.Empty);
    }

    [Test]
    public void CreateRootAssemblyFinder ()
    {
      var collection = DeserializeFromXmlFragment (_xmlFragment);
      var loaderStub = MockRepository.GenerateStub<IAssemblyLoader>();
      var finder = collection.CreateRootAssemblyFinder (loaderStub);

      Assert.That (finder.SearchPath, Is.EqualTo (AppDomain.CurrentDomain.BaseDirectory));
      Assert.That (finder.FileSearchService, Is.InstanceOf (typeof (FileSystemSearchService)));
      Assert.That (finder.AssemblyLoader, Is.SameAs (loaderStub));

      var specs = finder.Specifications.ToArray();

      Assert.That (specs[0].FilePattern, Is.EqualTo ("ActaNova.*.dll"));
      Assert.That (specs[0].Kind, Is.EqualTo (FilePatternSpecificationKind.IncludeNoFollow));

      Assert.That (specs[1].FilePattern, Is.EqualTo ("Remotion.*.dll"));
      Assert.That (specs[1].Kind, Is.EqualTo (FilePatternSpecificationKind.IncludeFollowReferences));

      Assert.That (specs[2].FilePattern, Is.EqualTo ("Remotion.*.Utilities.dll"));
      Assert.That (specs[2].Kind, Is.EqualTo (FilePatternSpecificationKind.Exclude));
    }


    private ByFileRootAssemblyElementCollection DeserializeFromXmlFragment (string xmlFragment)
    {
      var collection = new ByFileRootAssemblyElementCollection ();
      ConfigurationHelper.DeserializeElement (collection, xmlFragment);

      return collection;
    }
  }
}
