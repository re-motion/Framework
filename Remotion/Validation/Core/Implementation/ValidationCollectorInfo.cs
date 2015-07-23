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
using Remotion.Utilities;
using Remotion.Validation.Providers;

namespace Remotion.Validation.Implementation
{
  /// <summary>
  /// Associates an <see cref="IComponentValidationCollector"/> with the type of the <see cref="IValidationCollectorProvider"/> 
  /// that was used to resolve the collector.
  /// </summary>
  public sealed class ValidationCollectorInfo
  {
    private readonly IComponentValidationCollector _collector;
    private readonly Type _providerType;

    public ValidationCollectorInfo (IComponentValidationCollector collector, Type providerType)
    {
      ArgumentUtility.CheckNotNull ("collector", collector);
      ArgumentUtility.CheckNotNull ("providerType", providerType);

      _providerType = providerType;
      _collector = collector;
    }

    public IComponentValidationCollector Collector
    {
      get { return _collector; }
    }

    public Type ProviderType
    {
      get { return _providerType; }
    }
  }
}