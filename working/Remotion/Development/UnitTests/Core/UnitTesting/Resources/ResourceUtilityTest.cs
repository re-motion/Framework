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
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Resources;

namespace Remotion.Development.UnitTests.Core.UnitTesting.Resources
{
  [TestFixture]
  public class ResourceUtilityTest
  {
    
    [Test]
    public void GetResourceStream ()
    {
      var resourceStream = ResourceUtility.GetResourceStream (Assembly.GetExecutingAssembly (), 
          "Remotion.Development.UnitTests.Core.UnitTesting.Resources.TestEmbeddedResource.txt");
      
      Assert.That (resourceStream.Length, Is.EqualTo(11));
    }

    [Test]
    public void GetResourceStream_WithType ()
    {
      var resourceStream = ResourceUtility.GetResourceStream (GetType (), "TestEmbeddedResource.txt");

      Assert.That (resourceStream.Length, Is.EqualTo (11));
    }

    [Test]
    public void GetResource ()
    {
      var resourceBytes = ResourceUtility.GetResource (Assembly.GetExecutingAssembly (),
          "Remotion.Development.UnitTests.Core.UnitTesting.Resources.TestEmbeddedResource.txt");

      Assert.That (resourceBytes.Length, Is.EqualTo (11));
    }

    [Test]
    public void GetResource_WithType ()
    {
      var resourceBytes = ResourceUtility.GetResource (GetType(), "TestEmbeddedResource.txt");

      Assert.That (resourceBytes.Length, Is.EqualTo (11));
    }

    [Test]
    public void GetResourceString ()
    {
      var resourceContent = ResourceUtility.GetResourceString(Assembly.GetExecutingAssembly (),
          "Remotion.Development.UnitTests.Core.UnitTesting.Resources.TestEmbeddedResource.txt");

      Assert.That (resourceContent, Is.EqualTo("Testcontent"));
    }

    [Test]
    public void GetResourceString_WithType ()
    {
      var resourceContent = ResourceUtility.GetResourceString (GetType (), "TestEmbeddedResource.txt");

      Assert.That (resourceContent, Is.EqualTo ("Testcontent"));
    }
  }
}