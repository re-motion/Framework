using System;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Represents an object that was freshly loaded from the data source.
  /// </summary>
  public class FreshlyLoadedObjectData : ILoadedObjectData
  {
    private readonly DataContainer _freshlyLoadedDataContainer;

    public FreshlyLoadedObjectData (DataContainer freshlyLoadedDataContainer)
    {
      ArgumentUtility.CheckNotNull ("freshlyLoadedDataContainer", freshlyLoadedDataContainer);

      if (freshlyLoadedDataContainer.IsRegistered)
        throw new ArgumentException ("The DataContainer must not have been registered with a ClientTransaction.", "freshlyLoadedDataContainer");

      if (freshlyLoadedDataContainer.HasDomainObject)
        throw new ArgumentException ("The DataContainer must not have been registered with a DomainObject.", "freshlyLoadedDataContainer");

      _freshlyLoadedDataContainer = freshlyLoadedDataContainer;
    }

    public ObjectID ObjectID
    {
      get { return _freshlyLoadedDataContainer.ID; }
    }

    public DataContainer FreshlyLoadedDataContainer
    {
      get { return _freshlyLoadedDataContainer; }
    }

    public DomainObject GetDomainObjectReference ()
    {
      if (!_freshlyLoadedDataContainer.HasDomainObject)
        throw new InvalidOperationException ("Cannot obtain a DomainObject reference for a freshly loaded object that has not yet been registered.");
      
      return _freshlyLoadedDataContainer.DomainObject;
    }

    public void Accept (ILoadedObjectVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.VisitFreshlyLoadedObject (this);
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}