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
using Remotion.Utilities;

namespace Remotion.Mixins.CrossReferencer.Utilities
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
