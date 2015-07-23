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
using Remotion.Mixins.Samples.CompositionPattern.Core.Domain.Mixins;
using Remotion.Mixins.Samples.CompositionPattern.Core.Framework;
using Remotion.TypePipe;

namespace Remotion.Mixins.Samples.CompositionPattern.Core.Domain
{
  /// <summary>
  /// Implements the core concerns of a "Settlement" domain object. Technical concerns can be added by using mixins. The composed interface for this 
  /// class is <see cref="ISettlement"/>, it contains all members defined by the core domain object and the used mixins. 
  /// <see cref="ISettlementImplementation"/> is a technical detail that enables the compiler to verify that all members of <see cref="Settlement"/>
  /// are also present on <see cref="ISettlement"/>. External components can also extend <see cref="Settlement"/> with external concerns. Members
  /// added by such concerns are not present in the composed interface; they are provided via extension methods.
  /// </summary>
  [Uses (typeof (DocumentMixin))] // introduces IDocument
  [Uses (typeof (TenantBoundMixin))] // introduces ITenantBoundObject
  [Uses (typeof (SerialNumberMixin))] // overrides ToString
  public class Settlement : ComposedDomainObject<ISettlement>, ISettlementImplementation
  {
    /// <summary>
    /// Uses the <see cref="ComposedDomainObject{TComposedInterface}.NewObject{TComposite}"/> method to instantiate the composed 
    /// <see cref="Settlement"/> class and return it as an instance of the composed interface <see cref="ISettlement"/>.
    /// </summary>
    /// <returns></returns>
    public static ISettlement NewObject ()
    {
      return NewObject<Settlement> (ParamList.Empty);
    }

    protected override void OnReferenceInitializing ()
    {
      base.OnReferenceInitializing ();
      Events.Committing += Committing;
    }

    public virtual string SettlementKind { get; set; }

    public override string ToString ()
    {
      return string.Format ("Settlement: {0} ({1}); Tenant: {2}", This.Title, This.SettlementKind, This.Tenant);
    }

    private void Committing (object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty (SettlementKind))
        SettlementKind = "Ordinary";
    }
  }
}