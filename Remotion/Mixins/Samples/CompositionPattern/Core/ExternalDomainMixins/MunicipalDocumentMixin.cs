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
using Remotion.Mixins.Samples.CompositionPattern.Core.Domain;
using Remotion.Mixins.Samples.CompositionPattern.Core.Domain.Mixins;

namespace Remotion.Mixins.Samples.CompositionPattern.Core.ExternalDomainMixins
{
  /// <summary>
  /// Changes the implementation of <see cref="IDocument"/> in order to follow the municpal standards.
  /// Actually, we would like to extend <see cref="ISettlement"/>'s implementation of <see cref="IDocument"/> (and - since we need access to the 
  /// <see cref="IMunicipalSettlement.MunicipalityID"/>) have a dependency on <see cref="IMunicipalSettlement"/>). 
  /// It is not possible to use a mixin in order to override the members added by another mixin.
  /// Therefore, we have two options: mix the <see cref="DocumentMixin"/> or extend <see cref="DocumentMixin"/>.
  /// Since we need to access the <see cref="DocumentMixin"/>'s <see cref="Mixin{TTarget}.Target"/> property, the latter option
  /// seems cleaner. Unfortunately, in both situations we cannot access the <see cref="IMunicipalSettlement.MunicipalityID"/> without casting. We
  /// declare an additional dependency on <see cref="IMunicipalSettlement"/>, but the only way to get the 
  /// <see cref="Mixin{TTarget}.Target"/> property to include the <see cref="IMunicipalSettlement.MunicipalityID"/> property would be to
  /// refactor <see cref="DocumentMixin"/> to become a generic mixin. We don't do that to illustrate how to work without a generic mixin.
  /// </summary>
  // [Extends (typeof (Settlement), 
  //     SuppressedMixins = new[] { typeof (DocumentMixin) }, 
  //     AdditionalDependencies = new[] { typeof (IMunicipalSettlement) })] // => omitted in order to simulate that the municipal configuration is only present in certain configurations
  public class MunicipalDocumentMixin : DocumentMixin
  {
    public override string Title
    {
      get { return base.Title; }
      set { base.Title = value + " (for municipality " + ((IMunicipalSettlement) Target).MunicipalityID + ")"; }
    }
  }
}