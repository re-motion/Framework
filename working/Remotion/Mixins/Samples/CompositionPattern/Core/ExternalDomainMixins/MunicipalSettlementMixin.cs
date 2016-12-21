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
using Remotion.Mixins.Samples.CompositionPattern.Core.Framework;

namespace Remotion.Mixins.Samples.CompositionPattern.Core.ExternalDomainMixins
{
  /// <summary>
  /// Extends all <see cref="ISettlement"/> implementations with municipal-specific properties. There is no composed interface to access all members
  /// of <see cref="ISettlement"/> and <see cref="IMunicipalSettlement"/>, Instead, the 
  /// <see cref="Remotion_Mixins_Samples_CompositionPattern_Core_ExternalDomainMixins_MunicipalSettlementExtensions"/> provides an extension method
  /// for <see cref="ISettlement"/> to enable that access.
  /// </summary>
  // [Extends (typeof (Settlement))] // => omitted in order to simulate that the municipal configuration is only present in certain configurations
  public class MunicipalSettlementMixin : DomainObjectMixin<ISettlement>, IMunicipalSettlement
  {
    protected override void OnTargetReferenceInitializing ()
    {
      base.OnTargetReferenceInitializing ();
      MunicipalityID = 12;
    }
    
    public virtual int MunicipalityID { get; set; }

    public virtual string GetDescriptionForMayors ()
    {
      return string.Format ("MunicipalSettlement: {0} ({1}), {2}", Target.Title, Target.SettlementKind, MunicipalityID);
    }
  }
}