using System;
using Remotion.Mixins.Samples.CompositionPattern.Core.Domain;
using Remotion.Mixins.Samples.CompositionPattern.Core.ExternalDomainMixins;

// ReSharper disable CheckNamespace
public static class Remotion_Mixins_Samples_CompositionPattern_Core_ExternalDomainMixins_MunicipalSettlementExtensions 
{
  public static IMunicipalSettlement AsMunicipalSettlement (this ISettlement settlement)
  {
    return (IMunicipalSettlement) settlement;
  }
}
// ReSharper restore CheckNamespace
