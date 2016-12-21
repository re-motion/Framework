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
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Rhino.Mocks;

namespace Remotion.UnitTests.Configuration.TypeDiscovery
{
  [TestFixture]
  public class ByNameRootAssemblyElementCollectionTest
  {
    private const string _xmlFragment = @"<byName>
              <include name=""mscorlib"" />
              <include name=""System.Xml, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"" includeReferencedAssemblies=""true"" />
            </byName>";

    [Test]
    public void Deserialization ()
    {
      var collection = DeserializeFromXmlFragment (_xmlFragment);

      ByNameRootAssemblyElement[] result = collection.ToArray();
      Assert.That (result.Length, Is.EqualTo (2));
      Assert.That (result[0].Name, Is.EqualTo ("mscorlib"));
      Assert.That (result[1].Name, Is.EqualTo ("System.Xml, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"));
    }

    [Test]
    public void IncludeReferencedAssemblies_FalseByDefault ()
    {
      var collection = DeserializeFromXmlFragment (_xmlFragment);

      ByNameRootAssemblyElement[] result = collection.ToArray ();
      Assert.That (result[0].IncludeReferencedAssemblies, Is.False);
    }

    [Test]
    public void IncludeReferencedAssemblies_TrueIfSpecified ()
    {
      var collection = DeserializeFromXmlFragment (_xmlFragment);

      ByNameRootAssemblyElement[] result = collection.ToArray ();
      Assert.That (result[1].IncludeReferencedAssemblies, Is.True);
    }

    [Test]
    public void Add ()
    {
      var element1 = new ByNameRootAssemblyElement { Name = "x", IncludeReferencedAssemblies = true };
      var element2 = new ByNameRootAssemblyElement { Name = "y" };
      var element3 = new ByNameRootAssemblyElement { Name = "z" };

      var collection = new ByNameRootAssemblyElementCollection ();
      collection.Add (element1);
      collection.Add (element2);
      collection.Add (element3);

      ByNameRootAssemblyElement[] result = collection.ToArray ();
      Assert.That (result, Is.EqualTo (new[] { element1, element2, element3 }));
    }

    [Test]
    public void RemoveAt ()
    {
      var element1 = new ByNameRootAssemblyElement { Name = "x", IncludeReferencedAssemblies = true };
      var element2 = new ByNameRootAssemblyElement { Name = "y" };
      var element3 = new ByNameRootAssemblyElement { Name = "z" };

      var collection = new ByNameRootAssemblyElementCollection ();
      collection.Add (element1);
      collection.Add (element2);
      collection.Add (element3);
      collection.RemoveAt (1);

      ByNameRootAssemblyElement[] result = collection.ToArray ();
      Assert.That (result, Is.EquivalentTo (new[] { element1, element3 }));
    }

    [Test]
    public void Clear ()
    {
      var element1 = new ByNameRootAssemblyElement { Name = "x", IncludeReferencedAssemblies = true };
      var element2 = new ByNameRootAssemblyElement { Name = "y" };
      var element3 = new ByNameRootAssemblyElement { Name = "z" };

      var collection = new ByNameRootAssemblyElementCollection ();
      collection.Add (element1);
      collection.Add (element2);
      collection.Add (element3);
      collection.Clear ();

      ByNameRootAssemblyElement[] result = collection.ToArray ();
      Assert.That (result, Is.Empty);
    }

    [Test]
    public void CreateRootAssemblyFinder ()
    {
      var collection = DeserializeFromXmlFragment (_xmlFragment);
      var loaderStub = MockRepository.GenerateStub<IAssemblyLoader>();

      var finder = collection.CreateRootAssemblyFinder (loaderStub);

      var specs = finder.Specifications.ToArray();
      Assert.That (specs[0].AssemblyName.ToString (), Is.EqualTo ("mscorlib"));
      Assert.That (specs[0].FollowReferences, Is.False);

      Assert.That (specs[1].AssemblyName.ToString (), Is.EqualTo ("System.Xml, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"));
      Assert.That (specs[1].FollowReferences, Is.True);

      Assert.That (finder.AssemblyLoader, Is.SameAs (loaderStub));
    }

    private ByNameRootAssemblyElementCollection DeserializeFromXmlFragment (string xmlFragment)
    {
      var collection = new ByNameRootAssemblyElementCollection ();
      ConfigurationHelper.DeserializeElement (collection, xmlFragment);

      return collection;
    }
  }
}
