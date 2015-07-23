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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Declares a relation as bidirectional. Use <see cref="ContainsForeignKey"/> to indicate the the foreign key side in a one-to-one relation
  /// and the <see cref="BidirectionalRelationAttribute.SortExpression"/> to specify the <b>Order By</b>-clause.
  /// </summary>
  public class DBBidirectionalRelationAttribute: BidirectionalRelationAttribute
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DBBidirectionalRelationAttribute"/> class with the name of the oppsite property
    /// and the <see cref="ContainsForeignKey"/> value.
    /// </summary>
    /// <param name="oppositeProperty">The name of the opposite property. Must not be <see langword="null" /> or empty.</param>
    public DBBidirectionalRelationAttribute (string oppositeProperty)
        : base (oppositeProperty)
    {
      ContainsForeignKey = false;
    }

    /// <summary>Gets or sets a flag that indicates the foreign key side in a one-to-one relation.</summary>
    /// <remarks>The <see cref="ContainsForeignKey"/> property may only be specified on one side of a one-to-one-relaiton.</remarks>
    public bool ContainsForeignKey { get; set; }
  }
}
