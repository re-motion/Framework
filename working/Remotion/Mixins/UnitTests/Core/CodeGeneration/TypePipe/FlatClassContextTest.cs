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
using Remotion.Development.UnitTesting;
using Remotion.Mixins.CodeGeneration.TypePipe;
using Remotion.Mixins.Context;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.TypePipe
{
  [TestFixture]
  public class FlatClassContextTest
  {
    private ClassContext _originalClassContext;
    private FlatClassContext _flatClassContext;

    [SetUp]
    public void SetUp ()
    {
      _originalClassContext = ClassContextObjectMother.Create (typeof (string), typeof (DateTime));
      _flatClassContext = FlatClassContext.Create (_originalClassContext);
    }

    [Test]
    public void GetRealObject_ReconstructsClassContext ()
    {
      var result = _flatClassContext.GetRealValue();

      Assert.That (result, Is.EqualTo (_originalClassContext));
    }

    [Test]
    public void Serializable ()
    {
      var result = Serializer.SerializeAndDeserialize (_flatClassContext);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.GetRealValue(), Is.EqualTo (_originalClassContext));
    }
  }
}