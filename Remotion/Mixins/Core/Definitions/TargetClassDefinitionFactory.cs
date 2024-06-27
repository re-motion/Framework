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
using Microsoft.Extensions.Logging;
using Remotion.Logging;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions.Building;
using Remotion.Mixins.Validation;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  /// <summary>
  /// Creates <see cref="TargetClassDefinition"/> objects from <see cref="ClassContext"/> instances, validating them before returning.
  /// </summary>
  /// <remarks>
  /// This class acts as a facade for <see cref="TargetClassDefinitionBuilder"/> and <see cref="Validator"/>.
  /// </remarks>
  public static class TargetClassDefinitionFactory
  {
    private static readonly ILogger s_logger = LazyLoggerFactory.CreateLogger(typeof(TargetClassDefinitionFactory));

    public static TargetClassDefinition CreateAndValidate (ClassContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      s_logger.LogDebug("Creating a validated class definition for: {0}.", context);

      using (StopwatchScope.CreateScope(s_logger, LogLevel.Debug, "Time needed to create and validate class definition: {elapsed}."))
      {
        var definition = CreateInternal(context);
        Validate(definition);
        return definition;
      }
    }

    public static TargetClassDefinition CreateWithoutValidation (ClassContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      s_logger.LogDebug("Creating an unvalidated class definition for: {0}.", context);

      using (StopwatchScope.CreateScope(s_logger, LogLevel.Debug, "Time needed to create class definition: {elapsed}."))
      {
        return CreateInternal(context);
      }
    }

    private static TargetClassDefinition CreateInternal (ClassContext context)
    {
      var builder = SafeServiceLocator.Current.GetInstance<ITargetClassDefinitionBuilder>();
      return builder.Build(context);
    }

    private static void Validate (TargetClassDefinition definition)
    {
      var logData = Validator.Validate(definition);
      if (logData.GetNumberOfFailures() > 0 || logData.GetNumberOfUnexpectedExceptions() > 0)
        throw new ValidationException(logData);
    }
  }
}
