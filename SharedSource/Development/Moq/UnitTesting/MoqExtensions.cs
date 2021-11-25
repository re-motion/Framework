// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under
// the Apache License, Version 2.0 (the "License"); you may not use this
// file except in compliance with the License.  You may obtain a copy of the
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the
// License for the specific language governing permissions and limitations
// under the License.
//

using System;
using Moq.Language;
using Moq.Language.Flow;
using Remotion.Utilities;

namespace Remotion.Development.Moq.UnitTesting
{
  public static partial class MoqExtensions
  {
    public static ICallbackResult InSequence<T> (this T mock, ref int sequenceCounter, int expectedPosition)
        where T : ICallback
    {
      var counter = sequenceCounter;
      var callbackResult = mock.Callback(
          () =>
          {
            if (counter != expectedPosition)
            {
              throw new InvalidOperationException(
                  "Method call was not in the right position of the sequence.\r\n"
                  + $"Expected position {expectedPosition}.\r\nActual position {counter}");
            }
          });

      sequenceCounter++;
      return callbackResult;
    }
  }
}