using System;
using Moq;
using Moq.Language;
using Moq.Language.Flow;

namespace Remotion.Data.DomainObjects.UnitTests.EventReceiver
{
  public static class DomainObjectCollectionMockEventReceiverExtensions
  {
    public static ISetup<IDomainObjectCollectionMockEventReceiver> SetupAdding (
        this Mock<IDomainObjectCollectionMockEventReceiver> fluent,
        DomainObjectCollection sender,
        DomainObject domainObject)
    {
      return fluent
          .Setup(mock => mock.Adding(sender, It.Is<DomainObjectCollectionChangeEventArgs>(args => args.DomainObject == domainObject)));
    }

    public static ISetup<IDomainObjectCollectionMockEventReceiver> SetupAdding (
        this ISetupConditionResult<IDomainObjectCollectionMockEventReceiver> fluent,
        DomainObjectCollection sender,
        DomainObject domainObject)
    {
      return fluent
          .Setup(mock => mock.Adding(sender, It.Is<DomainObjectCollectionChangeEventArgs>(args => args.DomainObject == domainObject)));
    }

    public static ISetup<IDomainObjectCollectionMockEventReceiver> SetupAdded (
        this Mock<IDomainObjectCollectionMockEventReceiver> fluent,
        DomainObjectCollection sender,
        DomainObject domainObject)
    {
      return fluent
          .Setup(mock => mock.Added(sender, It.Is<DomainObjectCollectionChangeEventArgs>(args => args.DomainObject == domainObject)));
    }

    public static ISetup<IDomainObjectCollectionMockEventReceiver> SetupAdded (
        this ISetupConditionResult<IDomainObjectCollectionMockEventReceiver> fluent,
        DomainObjectCollection sender,
        DomainObject domainObject)
    {
      return fluent
          .Setup(mock => mock.Added(sender, It.Is<DomainObjectCollectionChangeEventArgs>(args => args.DomainObject == domainObject)));
    }

    public static ISetup<IDomainObjectCollectionMockEventReceiver> SetupRemoving (
        this Mock<IDomainObjectCollectionMockEventReceiver> fluent,
        DomainObjectCollection sender,
        DomainObject domainObject)
    {
      return fluent
          .Setup(mock => mock.Removing(sender, It.Is<DomainObjectCollectionChangeEventArgs>(args => args.DomainObject == domainObject)));
    }

    public static ISetup<IDomainObjectCollectionMockEventReceiver> SetupRemoving (
        this ISetupConditionResult<IDomainObjectCollectionMockEventReceiver> fluent,
        DomainObjectCollection sender,
        DomainObject domainObject)
    {
      return fluent
          .Setup(mock => mock.Removing(sender, It.Is<DomainObjectCollectionChangeEventArgs>(args => args.DomainObject == domainObject)));
    }

    public static ISetup<IDomainObjectCollectionMockEventReceiver> SetupRemoved (
        this Mock<IDomainObjectCollectionMockEventReceiver> fluent,
        DomainObjectCollection sender,
        DomainObject domainObject)
    {
      return fluent
          .Setup(mock => mock.Removed(sender, It.Is<DomainObjectCollectionChangeEventArgs>(args => args.DomainObject == domainObject)));
    }

    public static ISetup<IDomainObjectCollectionMockEventReceiver> SetupRemoved (
        this ISetupConditionResult<IDomainObjectCollectionMockEventReceiver> fluent,
        DomainObjectCollection sender,
        DomainObject domainObject)
    {
      return fluent
          .Setup(mock => mock.Removed(sender, It.Is<DomainObjectCollectionChangeEventArgs>(args => args.DomainObject == domainObject)));
    }
  }
}
