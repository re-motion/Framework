using System;
using System.Collections.Generic;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  [ImplementationFor (typeof (IBusinessObjectReferenceDataSourceControlValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Compound)]
  public class CompoundBusinessObjectReferenceDataSourceControlValidatorFactory : CompoundValidatorFactory<BusinessObjectReferenceDataSourceControl>, IBusinessObjectReferenceDataSourceControlValidatorFactory
  {
    public CompoundBusinessObjectReferenceDataSourceControlValidatorFactory (IEnumerable<IBusinessObjectReferenceDataSourceControlValidatorFactory> innerFactories)
        : base(innerFactories)
    {
    }
  }
}