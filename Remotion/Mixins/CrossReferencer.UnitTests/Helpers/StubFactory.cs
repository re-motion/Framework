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
      var identifierGeneratorStub = MockRepository.GenerateStub<IIdentifierGenerator<T>> ();

      foreach (var value in values)
        identifierGeneratorStub.Stub (ig => ig.GetIdentifier (value.Value)).Return (value.Key);

      identifierGeneratorStub.Stub (ig => ig.Elements).Return (values.Values);

      return identifierGeneratorStub;
    }

    public static IIdentifierGenerator<T> CreateIdentifierGeneratorStub<T> (IEnumerable<T> values)
    {
      var identifierGeneratorStub = MockRepository.GenerateStub<IIdentifierGenerator<T>> ();

      var i = 0;
      foreach (var value in values)
        identifierGeneratorStub.Stub (ig => ig.GetIdentifier (value)).Return ((i++).ToString ());

      identifierGeneratorStub.Stub (ig => ig.Elements).Return (values);

      return identifierGeneratorStub;
    }
  }
}
