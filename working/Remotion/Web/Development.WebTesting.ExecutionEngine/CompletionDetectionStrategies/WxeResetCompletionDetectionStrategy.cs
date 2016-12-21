﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using log4net;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.ExecutionEngine.CompletionDetectionStrategies
{
  /// <summary>
  /// Blocks until the WXE function token is different and the WXE post back sequence number (for the current <see cref="PageObjectContext"/>) has
  /// been reset.
  /// </summary>
  public class WxeResetCompletionDetectionStrategy : ICompletionDetectionStrategy
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (WxeResetCompletionDetectionStrategy));

    /// <inheritdoc/>
    public object PrepareWaitForCompletion (PageObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      return WxeCompletionDetectionHelpers.GetWxeFunctionToken (context);
    }

    /// <inheritdoc/>
    public void WaitForCompletion (PageObjectContext context, object state)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("state", state);

      var oldWxeFunctionToken = (string) state;
      WxeCompletionDetectionHelpers.WaitForNewWxeFunctionToken (s_log, context, oldWxeFunctionToken);

      const int expectedWxePostBackSequenceNumber = 2;
      WxeCompletionDetectionHelpers.WaitForExpectedWxePostBackSequenceNumber (s_log, context, expectedWxePostBackSequenceNumber);
    }
  }
}