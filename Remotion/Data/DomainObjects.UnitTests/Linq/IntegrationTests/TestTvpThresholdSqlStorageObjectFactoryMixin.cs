using JetBrains.Annotations;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2016;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Linq.IntegrationTests;

public class TestSqlStorageObjectFactory : SqlStorageObjectFactory
{
  public TestSqlStorageObjectFactory ([NotNull] IStorageSettings storageSettings, [NotNull] ITypeConversionProvider typeConversionProvider, [NotNull] IDataContainerValidator dataContainerValidator)
      : base(storageSettings, typeConversionProvider, dataContainerValidator)
  {
  }

  public static int? Threshold { get; set; }

  protected override int GetTableValuedParameterThreshold ()
  {
    if (Threshold.HasValue)
      return Threshold.Value;

    return base.GetTableValuedParameterThreshold();
  }
}
