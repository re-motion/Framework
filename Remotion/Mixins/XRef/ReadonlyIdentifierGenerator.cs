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
using System;
using System.Collections.Generic;

namespace Remotion.Mixins.XRef
{
  public class ReadonlyIdentifierGenerator<T> : IIdentifierGenerator<T>
  {
    private readonly IIdentifierGenerator<T> _identifierGenerator;

    private readonly string _defaultValue;

    public ReadonlyIdentifierGenerator (IIdentifierGenerator<T> identifierGenerator, string defaultValue)
    {
      ArgumentUtility.CheckNotNull("identifierGenerator", identifierGenerator);
      ArgumentUtility.CheckNotNull("defaultValue", defaultValue);

      _identifierGenerator = identifierGenerator;
      _defaultValue = defaultValue;
    }

    public string GetIdentifier (T item)
    {
      return _identifierGenerator.GetIdentifier(item, _defaultValue);
    }

    public string GetIdentifier (T item, string defaultIfNotPresent)
    {
      return _identifierGenerator.GetIdentifier(item, defaultIfNotPresent);
    }

    public IEnumerable<T> Elements
    {
      get { return _identifierGenerator.Elements; }
    }
  }
}
