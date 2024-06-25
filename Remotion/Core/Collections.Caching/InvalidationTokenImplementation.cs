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

namespace Remotion.Collections.Caching
{
  /// <summary>
  /// The <see cref="InvalidationTokenImplementation"/> is the non-threadsafe implementation of the <see cref="InvalidationToken"/> class.
  /// </summary>
  /// <remarks>Instantiate via <see cref="InvalidationToken"/>.<see cref="InvalidationToken.Create"/>.</remarks>
  /// <threadsafety static="true" instance="false" />
  internal sealed class InvalidationTokenImplementation : InvalidationToken
  {
    private long _currentRevisionValue;

    internal InvalidationTokenImplementation ()
    {
      // Use the instance's hash-code as revision seed value to allow for a reasonably different number space. 
      // The hash-code is often different between reference types and therefor adds a bit of randomness to the revisions.
      _currentRevisionValue = Math.Abs(GetHashCode()) * -1;
    }

    public override void Invalidate ()
    {
      _currentRevisionValue++;
    }

    protected override long GetCurrentRevisionValue ()
    {
      return _currentRevisionValue;
    }
  }
}
