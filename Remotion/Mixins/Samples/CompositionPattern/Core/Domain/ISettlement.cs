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
using Remotion.Mixins.Samples.CompositionPattern.Core.Framework;

namespace Remotion.Mixins.Samples.CompositionPattern.Core.Domain
{
  /// <summary>
  /// <see cref="ISettlement"/> is the composed interface for the <see cref="Settlement"/> class and the mixins it uses. It contains all members 
  /// defined by the core domain object and the used mixins. It is planned that the <see cref="ComposedInterfaceAttribute"/> will become unnecessary
  /// at a later point of time; the mixin engine will recognize <see cref="ISettlement"/> to be the composed interface for <see cref="Settlement"/>
  /// because <see cref="Settlement"/> derives from <see cref="ComposedDomainObject{TComposedInterface}"/>, passing <see cref="ISettlement"/> as
  /// the type parameter.
  /// </summary>
  public interface ISettlement : ISettlementImplementation, IDocument, ITenantBoundObject
  {
  }
}