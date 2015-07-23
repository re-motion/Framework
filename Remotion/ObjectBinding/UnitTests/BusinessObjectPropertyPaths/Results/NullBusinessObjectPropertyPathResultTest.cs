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
using NUnit.Framework;
using Remotion.ObjectBinding.BusinessObjectPropertyPaths.Results;

namespace Remotion.ObjectBinding.UnitTests.BusinessObjectPropertyPaths.Results
{
  [TestFixture]
  public class NullBusinessObjectPropertyPathResultTest
  {
    private IBusinessObjectPropertyPathResult _result;

    [SetUp]
    public void SetUp ()
    {
      _result = new NullBusinessObjectPropertyPathResult();
    }

    [Test]
    public void GetValue ()
    {
      Assert.That (_result.GetValue(), Is.Null);
    }

    [Test]
    public void GetString ()
    {
      Assert.That (_result.GetString (string.Empty), Is.Empty);
    }

    [Test]
    public void GetResultObject ()
    {
      Assert.That (_result.ResultObject, Is.Null);
    }
 
    [Test]
    public void GetResulProperty ()
    {
      Assert.That (_result.ResultProperty, Is.Null);
    }

    [Test]
    public void GetIsNull ()
    {
      Assert.That (_result.IsNull, Is.True);
    }
  }
}