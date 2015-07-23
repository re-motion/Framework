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
using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Remotion.Collections;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.TypePipe;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Sample
{
  public class XmlReflectionBusinessObjectStorageProvider : IGetObjectService
  {
    private static readonly DoubleCheckedLockingContainer<XmlReflectionBusinessObjectStorageProvider> s_current
        = new DoubleCheckedLockingContainer<XmlReflectionBusinessObjectStorageProvider> (delegate { return null; });

    public static XmlReflectionBusinessObjectStorageProvider Current
    {
      get { return s_current.Value; }
    }

    public static void SetCurrent (XmlReflectionBusinessObjectStorageProvider provider)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      s_current.Value = provider;
    }

    private readonly string _rootPath;
    private Hashtable _identityMap = new Hashtable();
    private readonly LockingCacheDecorator<Type, XmlSerializer> _attributeOverridesCache = CacheFactory.CreateWithLocking<Type, XmlSerializer>();

    public XmlReflectionBusinessObjectStorageProvider (string rootPath)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("rootPath", rootPath);

      _rootPath = rootPath;
    }

    public void Reset ()
    {
      _identityMap = new Hashtable();
    }

    public string RootPath
    {
      get { return _rootPath; }
    }

    public BindableXmlObject GetObject (Type type, Guid id)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      if (id == Guid.Empty)
        return null;

      BindableXmlObject obj = GetFromIdentityMap (id);
      if (obj != null)
        return obj;


      string typeDir = Path.Combine (_rootPath, type.FullName);
      string fileName = Path.Combine (typeDir, id.ToString());
      if (!File.Exists (fileName))
        return null;

      Type concreteType = GetConcreteType (type);
      XmlSerializer serializer = GetXmlSerializer (concreteType);
      using (FileStream stream = new FileStream (fileName, FileMode.Open, FileAccess.Read))
      {
        obj = (BindableXmlObject) serializer.Deserialize (stream);
      }
      obj._id = id;
      AddToIdentityMap (obj);
      return obj;
    }

    public BindableXmlObject[] GetObjects (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      ArrayList objects = new ArrayList();

      string typeDir = Path.Combine (_rootPath, type.FullName);
      string[] filenames = Directory.GetFiles (typeDir);

      foreach (string filename in filenames)
      {
        Guid id;
        try
        {
          id = new Guid (new FileInfo (filename).Name);
        }
        catch (FormatException)
        {
          continue;
        }

        BindableXmlObject obj = GetObject (type, id);
        if (obj != null)
          objects.Add (obj);
      }

      return (BindableXmlObject[]) objects.ToArray (typeof (BindableXmlObject));
    }

    public void SaveObject (BindableXmlObject obj)
    {
      ArgumentUtility.CheckNotNull ("obj", obj);


      Type targetType = GetTargetType (obj);
      string typeDir = Path.Combine (_rootPath, targetType.FullName);
      if (!Directory.Exists (typeDir))
        Directory.CreateDirectory (typeDir);

      string fileName = Path.Combine (typeDir, obj.ID.ToString());

      Type concreteType = obj.GetType();
      XmlSerializer serializer = GetXmlSerializer (concreteType);
      using (FileStream stream = new FileStream (fileName, FileMode.Create, FileAccess.Write))
      {
        serializer.Serialize (stream, obj);
      }
    }

    public T CreateObject<T> () where T: BindableXmlObject
    {
      return (T) CreateObject (typeof (T), Guid.NewGuid());
    }

    public T CreateObject<T> (Guid id) where T: BindableXmlObject
    {
      return (T) CreateObject (typeof (T), id);
    }

    public BindableXmlObject CreateObject (Type concreteType)
    {
      return CreateObject (concreteType, Guid.NewGuid());
    }

    protected BindableXmlObject CreateObject (Type type, Guid id)
    {
      BindableXmlObject obj = (BindableXmlObject) ObjectFactory.Create (true, type, ParamList.Empty);
      obj._id = id;
      AddToIdentityMap (obj);
      return obj;
    }

    private void AddToIdentityMap (BindableXmlObject obj)
    {
      if (_identityMap.ContainsKey (obj.ID))
        return;

      _identityMap.Add (obj.ID, obj);
    }

    private BindableXmlObject GetFromIdentityMap (Guid id)
    {
      BindableXmlObject bindableXmlObject = (BindableXmlObject) _identityMap[id];
      if (bindableXmlObject == null)
        return null;
      return bindableXmlObject;
    }

    private Type GetConcreteType (Type targetType)
    {
      return TypeFactory.GetConcreteType (targetType);
    }

    private Type GetTargetType (BindableXmlObject obj)
    {
      return ((BindableObjectClass) ((IBusinessObject) obj).BusinessObjectClass).TargetType;
    }

    private XmlSerializer GetXmlSerializer (Type concreteType)
    {
      return _attributeOverridesCache.GetOrCreateValue (
          concreteType,
          delegate (Type type)
          {
            return new XmlSerializer (
                type,
                CreateAttributeOverrides (type),
                new Type[0],
                AttributeUtility.GetCustomAttribute<XmlRootAttribute> (type, true),
                null);
          });
    }

    private XmlAttributeOverrides CreateAttributeOverrides (Type concreteType)
    {
      XmlAttributeOverrides attributeOverrides = new XmlAttributeOverrides();
      foreach (MemberInfo memberInfo in concreteType.FindMembers (
          MemberTypes.Field | MemberTypes.Property,
          BindingFlags.Instance | BindingFlags.Public,
          delegate { return true; },
          null))
      {
        XmlAttributes attributes = new XmlAttributes();
        foreach (XmlElementAttribute attribute in AttributeUtility.GetCustomAttributes<XmlElementAttribute> (memberInfo, true))
          attributes.XmlElements.Add (attribute);
        attributes.XmlAttribute = AttributeUtility.GetCustomAttribute<XmlAttributeAttribute> (memberInfo, true);
        attributes.XmlIgnore = attributes.XmlAttribute == null && attributes.XmlElements.Count == 0;

        attributeOverrides.Add (concreteType, memberInfo.Name, attributes);
      }

      return attributeOverrides;
    }

    IBusinessObjectWithIdentity IGetObjectService.GetObject (BindableObjectClassWithIdentity classWithIdentity, string uniqueIdentifier)
    {
      ArgumentUtility.CheckNotNull ("classWithIdentity", classWithIdentity);
      ArgumentUtility.CheckNotNullOrEmpty ("uniqueIdentifier", uniqueIdentifier);

      return (IBusinessObjectWithIdentity) GetObject (classWithIdentity.TargetType, new Guid (uniqueIdentifier));
    }
  }
}
