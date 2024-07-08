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
using Moq;
using Remotion.Mixins.CrossReferencer.Utilities;

namespace Remotion.Mixins.CrossReferencer.UnitTests.Helpers
{
  internal static class StubFactory
  {
    public static IIdentifierGenerator<T> CreateIdentifierGeneratorStub<T> (IDictionary<string, T> values)
    {
      var identifierGeneratorStub = new Mock<IIdentifierGenerator<T>>();

      foreach (var value in values)
        identifierGeneratorStub.Setup(ig => ig.GetIdentifier(value.Value)).Returns(value.Key);

      identifierGeneratorStub.Setup(ig => ig.Elements).Returns(values.Values);

      return identifierGeneratorStub.Object;
    }

    public static IIdentifierGenerator<T> CreateIdentifierGeneratorStub<T> (IEnumerable<T> values)
    {
      var identifierGeneratorStub = new Mock<IIdentifierGenerator<T>>();

      var i = 0;
      foreach (var value in values)
        identifierGeneratorStub.Setup(ig => ig.GetIdentifier(value)).Returns((i++).ToString());

      identifierGeneratorStub.Setup(ig => ig.Elements).Returns(values);

      return identifierGeneratorStub.Object;
    }
  }
}
