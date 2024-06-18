// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
using System.Collections.Generic;
using MixinXRef.Utility;
using Rhino.Mocks;

namespace MixinXRef.UnitTests.Helpers
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
