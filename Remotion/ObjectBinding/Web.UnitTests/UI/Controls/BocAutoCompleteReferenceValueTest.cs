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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.Web.UnitTesting.Configuration;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Validation;
using Remotion.ObjectBinding.Web.UnitTests.Domain;
using Remotion.Web.Services;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls
{
  [TestFixture]
  public class BocAutoCompleteReferenceValueTest : BocTest
  {
    private class GetObjectService : IGetObjectService
    {
      private readonly IBusinessObjectWithIdentity _objectToReturn;

      public GetObjectService (IBusinessObjectWithIdentity objectToReturn)
      {
        _objectToReturn = objectToReturn;
      }

      public IBusinessObjectWithIdentity GetObject (BindableObjectClassWithIdentity classWithIdentity, string uniqueIdentifier)
      {
        return _objectToReturn;
      }
    }

    private Page _page;
    private BocAutoCompleteReferenceValueMock _control;
    private TypeWithReference _businessObject;
    private IBusinessObjectDataSource _dataSource;
    private IBusinessObjectReferenceProperty _propertyReferenceValue;
    private IWebServiceFactory _webServiceFactoryStub;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _webServiceFactoryStub = MockRepository.GenerateStub<IWebServiceFactory>();

      _control = new BocAutoCompleteReferenceValueMock (_webServiceFactoryStub);
      _control.ID = "BocAutoCompleteReferenceValue";
      _control.Value = (IBusinessObjectWithIdentity) _businessObject;

      MockRepository mockRepository = new MockRepository();
      _page = mockRepository.PartialMultiMock<Page> (typeof (ISmartPage));
      ((ISmartPage) _page).Stub (stub => stub.Context).Return (new HttpContextWrapper (HttpContext.Current));
      ((ISmartPage) _page).Stub (stub => stub.Site).Return (null);
      _page.Replay();
      _page.Controls.Add (_control);

      _businessObject = TypeWithReference.Create();

      _propertyReferenceValue =
          (IBusinessObjectReferenceProperty) ((IBusinessObject) _businessObject).BusinessObjectClass.GetPropertyDefinition ("ReferenceValue");

      _dataSource = new StubDataSource (((IBusinessObject) _businessObject).BusinessObjectClass);
      _dataSource.BusinessObject = (IBusinessObject) _businessObject;

      ((IBusinessObject) _businessObject).BusinessObjectClass.BusinessObjectProvider.AddService<IGetObjectService>
          (new GetObjectService ((IBusinessObjectWithIdentity) TypeWithReference.Create()));
      ((IBusinessObject) _businessObject).BusinessObjectClass.BusinessObjectProvider.AddService<IBusinessObjectWebUIService>
          (new ReflectionBusinessObjectWebUIService());
    }

    [Test]
    public void EvaluateWaiConformityDebugLevelUndefined ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
      _control.EvaluateWaiConformity();

      Assert.That (WcagHelperMock.HasWarning, Is.False);
      Assert.That (WcagHelperMock.HasError, Is.False);
    }

    [Test]
    public void EvaluateWaiConformityDebugLevelA ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
      _control.EvaluateWaiConformity();

      Assert.That (WcagHelperMock.HasWarning, Is.False);
      Assert.That (WcagHelperMock.HasError, Is.True);
      Assert.That (WcagHelperMock.Control, Is.EqualTo (_control));
    }

    [Test]
    [Obsolete ("This feature has been deprecated and will be removed in version 1.22.0. (Version 1.21.3)", false)]
    public void IsEventCommandDisabledWithWcagOverride ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
      _control.Command.Type = CommandType.Event;
      Assert.That (((IBocReferenceValueBase) _control).IsCommandEnabled(), Is.False);
    }

    [Test]
    [Obsolete ("This feature has been deprecated and will be removed in version 1.22.0. (Version 1.21.3)", false)]
    public void IsEventCommandEnabledWithoutWcagOverride ()
    {
      _businessObject.ReferenceValue = TypeWithReference.Create();
      _control.DataSource = _dataSource;
      _control.Property = _propertyReferenceValue;
      WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
      _control.Command.Type = CommandType.Event;
      Assert.That (((IBocReferenceValueBase) _control).IsCommandEnabled(), Is.True);
    }

    [Test]
    [Obsolete ("This feature has been deprecated and will be removed in version 1.22.0. (Version 1.21.3)", false)]
    public void IsWxeFunctionCommandDisabledWithWcagOverride ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
      _control.Command.Type = CommandType.WxeFunction;
      Assert.That (((IBocReferenceValueBase) _control).IsCommandEnabled(), Is.False);
    }

    [Test]
    [Obsolete ("This feature has been deprecated and will be removed in version 1.22.0. (Version 1.21.3)", false)]
    public void IsWxeFunctionCommandEnabledWithoutWcagOverride ()
    {
      _businessObject.ReferenceValue = TypeWithReference.Create();
      _control.DataSource = _dataSource;
      _control.Property = _propertyReferenceValue;
      WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
      _control.Command.Type = CommandType.WxeFunction;
      Assert.That (((IBocReferenceValueBase) _control).IsCommandEnabled(), Is.True);
    }

    [Test]
    public void GetTrackedClientIDsInReadOnlyMode ()
    {
      _control.ReadOnly = true;
      string[] actual = _control.GetTrackedClientIDs();
      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.Length, Is.EqualTo (0));
    }

    [Test]
    public void GetTrackedClientIDsInEditMode ()
    {
      _control.ReadOnly = false;
      string[] actual = _control.GetTrackedClientIDs();
      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.Length, Is.EqualTo (1));
      Assert.That (actual[0], Is.EqualTo (((IBocAutoCompleteReferenceValue) _control).GetTextValueName()));
    }

    [Test]
    public void SetValueToObject ()
    {
      IBusinessObjectWithIdentity referencedObject = (IBusinessObjectWithIdentity) TypeWithReference.Create();
      _control.IsDirty = false;
      _control.Value = referencedObject;
      Assert.That (_control.Value, Is.EqualTo (referencedObject));
      Assert.That (_control.IsDirty, Is.True);
    }

    [Test]
    public void SetValueToNull ()
    {
      _control.IsDirty = false;
      _control.Value = null;
      Assert.That (_control.Value, Is.EqualTo (null));
      Assert.That (_control.IsDirty, Is.True);
    }

    [Test]
    public void HasValue_ValueIsSet_ReturnsTrue ()
    {
      IBusinessObjectWithIdentity referencedObject = (IBusinessObjectWithIdentity) TypeWithReference.Create();
      _control.Value = referencedObject;
      Assert.That (_control.HasValue, Is.True);
    }

    [Test]
    public void HasValue_ValueIsNull_ReturnsFalse ()
    {
      _control.Value = null;
      Assert.That (_control.HasValue, Is.False);
    }

    [Test]
    public void LoadValueAndInterimTrue ()
    {
      _businessObject.ReferenceValue = TypeWithReference.Create();
      _control.DataSource = _dataSource;
      _control.Property = _propertyReferenceValue;
      _control.Value = null;
      _control.IsDirty = true;

      _control.LoadValue (true);
      Assert.That (_control.Value, Is.EqualTo (null));
      Assert.That (_control.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimFalseWithObject ()
    {
      _businessObject.ReferenceValue = TypeWithReference.Create();
      _control.DataSource = _dataSource;
      _control.Property = _propertyReferenceValue;
      _control.Value = null;
      _control.IsDirty = true;

      _control.LoadValue (false);
      Assert.That (_control.Value, Is.EqualTo (_businessObject.ReferenceValue));
      Assert.That (_control.IsDirty, Is.False);
    }

    [Test]
    public void LoadValueAndInterimFalseWithNull ()
    {
      _businessObject.ReferenceValue = null;
      _control.DataSource = _dataSource;
      _control.Property = _propertyReferenceValue;
      _control.Value = (IBusinessObjectWithIdentity) TypeWithReference.Create();
      _control.IsDirty = true;

      _control.LoadValue (false);
      Assert.That (_control.Value, Is.EqualTo (_businessObject.ReferenceValue));
      Assert.That (_control.IsDirty, Is.False);
    }

    [Test]
    public void LoadUnboundValueAndInterimTrue ()
    {
      IBusinessObjectWithIdentity value = (IBusinessObjectWithIdentity) TypeWithReference.Create();
      _control.Property = _propertyReferenceValue;
      _control.Value = null;
      _control.IsDirty = true;

      _control.LoadUnboundValue (value, true);
      Assert.That (_control.Value, Is.EqualTo (null));
      Assert.That (_control.IsDirty, Is.True);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithObject ()
    {
      IBusinessObjectWithIdentity value = (IBusinessObjectWithIdentity) TypeWithReference.Create();
      _control.Property = _propertyReferenceValue;
      _control.Value = null;
      _control.IsDirty = true;

      _control.LoadUnboundValue (value, false);
      Assert.That (_control.Value, Is.EqualTo (value));
      Assert.That (_control.IsDirty, Is.False);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithNull ()
    {
      const IBusinessObjectWithIdentity value = null;
      _control.Property = _propertyReferenceValue;
      _control.Value = (IBusinessObjectWithIdentity) TypeWithReference.Create();
      _control.IsDirty = true;

      _control.LoadUnboundValue (value, false);
      Assert.That (_control.Value, Is.EqualTo (value));
      Assert.That (_control.IsDirty, Is.False);
    }

    [Test]
    public void SaveControlState ()
    {
      _control.Value = (IBusinessObjectWithIdentity) _businessObject;

      object state = _control.SaveControlState();
      Assert.That (state is object[]);

      object[] stateArray = (object[]) state;
      Assert.That (stateArray.Length, Is.EqualTo (4));

      Assert.That (stateArray[1], Is.EqualTo (_control.Value.UniqueIdentifier));
      Assert.That (stateArray[2], Is.EqualTo (_control.Value.DisplayName));
      Assert.That (stateArray[3], Is.InstanceOf<SearchAvailableObjectWebServiceContext>());
    }

    [Test]
    public void LoadControlState ()
    {
      object parentState = ((object[]) _control.SaveControlState())[0];
      object[] state = new object[4];

      Guid uniqueIdentifier = Guid.NewGuid();
      state[0] = parentState;
      state[1] = uniqueIdentifier.ToString();
      state[2] = "DisplayName";
      state[3] = SearchAvailableObjectWebServiceContext.Create (null, null, null);

      _control.LoadControlState (state);
      Assert.That (((IBocReferenceValueBase) _control).GetLabelText(), Is.EqualTo ("DisplayName"));
      Assert.That (_control.BusinessObjectUniqueIdentifier, Is.EqualTo (uniqueIdentifier.ToString()));
    }

    [Test]
    public void LoadPostDataNotRequired ()
    {
      PrivateInvoke.SetNonPublicField (_control, "_hasBeenRenderedInPreviousLifecycle", false);

      bool result = ((IPostBackDataHandler) _control).LoadPostData (null, null);
      Assert.That (result, Is.False);
    }

    [Test]
    public void LoadPostData_ContainsNoData ()
    {
      PrivateInvoke.InvokeNonPublicMethod (_control, "CreateChildControls");

      var postbackCollection = new NameValueCollection();

      _control.IsDirty = false;
      PrivateInvoke.SetNonPublicField (_control, "_hasBeenRenderedInPreviousLifecycle", true);
      ((ISmartPage) _control.Page).Stub (stub => stub.GetPostBackCollection()).Return (postbackCollection);

      _webServiceFactoryStub.Stub (stub => stub.CreateJsonService<ISearchAvailableObjectWebService> ("~/SearchService.asmx"))
          .Return (MockRepository.GenerateStub<ISearchAvailableObjectWebService>());
      _control.AppRelativeTemplateSourceDirectory = "~/";
      _control.SearchServicePath = "~/SearchService.asmx";

      bool result = ((IPostBackDataHandler) _control).LoadPostData (_control.UniqueID, postbackCollection);
      Assert.That (_control.IsDirty, Is.False);
      Assert.That (result, Is.False);
    }

    [Test]
    public void LoadPostData_Empty_NotChanged ()
    {
      PrivateInvoke.InvokeNonPublicMethod (_control, "CreateChildControls");

      var postbackCollection = new NameValueCollection();

      postbackCollection.Add (
          ((IBocAutoCompleteReferenceValue) _control).GetKeyValueName(),
          ((IBocAutoCompleteReferenceValue) _control).NullValueString);
      postbackCollection.Add (((IBocAutoCompleteReferenceValue) _control).GetTextValueName(), string.Empty);

      _control.Value = null;
      _control.IsDirty = false;
      PrivateInvoke.SetNonPublicField (_control, "_hasBeenRenderedInPreviousLifecycle", true);
      ((ISmartPage) _control.Page).Stub (stub => stub.GetPostBackCollection()).Return (postbackCollection);

      _webServiceFactoryStub.Stub (stub => stub.CreateJsonService<ISearchAvailableObjectWebService> ("~/SearchService.asmx"))
          .Return (MockRepository.GenerateStub<ISearchAvailableObjectWebService>());
      _control.AppRelativeTemplateSourceDirectory = "~/";
      _control.SearchServicePath = "~/SearchService.asmx";

      bool result = ((IPostBackDataHandler) _control).LoadPostData (_control.UniqueID, postbackCollection);
      Assert.That (_control.IsDirty, Is.False);
      Assert.That (result, Is.False);
    }

    [Test]
    public void LoadPostData_NotEmpty_SetEmpty ()
    {
      PrivateInvoke.InvokeNonPublicMethod (_control, "CreateChildControls");
      var controlInvoker = new Remotion.Development.Web.UnitTesting.UI.Controls.ControlInvoker (_control);
      var controlState = controlInvoker.SaveControlState();
      controlInvoker.LoadControlState (controlState);

      var postbackCollection = new NameValueCollection();

      postbackCollection.Add (((IBocAutoCompleteReferenceValue) _control).GetKeyValueName(), string.Empty);
      postbackCollection.Add (((IBocAutoCompleteReferenceValue) _control).GetTextValueName(), string.Empty);

      _control.IsDirty = false;
      PrivateInvoke.SetNonPublicField (_control, "_hasBeenRenderedInPreviousLifecycle", true);
      ((ISmartPage) _control.Page).Stub (stub => stub.GetPostBackCollection()).Return (postbackCollection);

      _webServiceFactoryStub.Stub (stub => stub.CreateJsonService<ISearchAvailableObjectWebService> ("~/SearchService.asmx"))
          .Return (MockRepository.GenerateStub<ISearchAvailableObjectWebService>());
      _control.AppRelativeTemplateSourceDirectory = "~/";
      _control.SearchServicePath = "~/SearchService.asmx";

      bool result = ((IPostBackDataHandler) _control).LoadPostData (_control.UniqueID, postbackCollection);
      Assert.That (_control.IsDirty, Is.True);
      Assert.That (result, Is.True);
      Assert.That (_control.Value, Is.Null);
      Assert.That (_control.BusinessObjectUniqueIdentifier, Is.Null);
      Assert.That (((IBocAutoCompleteReferenceValue) _control).GetLabelText(), Is.Null);
    }

    [Test]
    public void LoadPostData_NotEmpty_ChangedToValidValue ()
    {
      PrivateInvoke.InvokeNonPublicMethod (_control, "CreateChildControls");
      var controlInvoker = new Remotion.Development.Web.UnitTesting.UI.Controls.ControlInvoker (_control);
      var controlState = controlInvoker.SaveControlState();
      controlInvoker.LoadControlState (controlState);

      var postbackCollection = new NameValueCollection();

      Guid value = Guid.NewGuid();
      postbackCollection.Add (((IBocAutoCompleteReferenceValue) _control).GetKeyValueName(), value.ToString());
      postbackCollection.Add (((IBocAutoCompleteReferenceValue) _control).GetTextValueName(), "NewValue");

      _control.IsDirty = false;

      PrivateInvoke.SetNonPublicField (_control, "_hasBeenRenderedInPreviousLifecycle", true);
      ((ISmartPage) _control.Page).Stub (stub => stub.GetPostBackCollection()).Return (postbackCollection);

      _webServiceFactoryStub
          .Stub (stub => stub.CreateJsonService<ISearchAvailableObjectWebService> ("~/SearchService.asmx"))
          .Return (MockRepository.GenerateStub<ISearchAvailableObjectWebService>());
      _control.AppRelativeTemplateSourceDirectory = "~/";
      _control.SearchServicePath = "~/SearchService.asmx";

      bool result = ((IPostBackDataHandler) _control).LoadPostData (_control.UniqueID, postbackCollection);
      Assert.That (_control.IsDirty, Is.True);
      Assert.That (result, Is.True);
      Assert.That (_control.BusinessObjectUniqueIdentifier, Is.EqualTo (value.ToString()));
      Assert.That (((IBocAutoCompleteReferenceValue) _control).GetLabelText(), Is.EqualTo ("NewValue"));
    }

    [Test]
    public void LoadPostData_NotEmpty_NotChanged ()
    {
      PrivateInvoke.InvokeNonPublicMethod (_control, "CreateChildControls");

      var postbackCollection = new NameValueCollection();

      string value = _control.Value.UniqueIdentifier;
      string displayName = _control.Value.DisplayName;
      Assert.That (value, Is.Not.Null.Or.Empty);
      Assert.That (displayName, Is.Not.Null.Or.Empty);
      postbackCollection.Add (((IBocAutoCompleteReferenceValue) _control).GetKeyValueName(), value);
      postbackCollection.Add (((IBocAutoCompleteReferenceValue) _control).GetTextValueName(), displayName);

      _control.IsDirty = false;

      PrivateInvoke.SetNonPublicField (_control, "_hasBeenRenderedInPreviousLifecycle", true);
      ((ISmartPage) _control.Page).Stub (stub => stub.GetPostBackCollection()).Return (postbackCollection);

      _webServiceFactoryStub
          .Stub (stub => stub.CreateJsonService<ISearchAvailableObjectWebService> ("~/SearchService.asmx"))
          .Return (MockRepository.GenerateStub<ISearchAvailableObjectWebService>());
      _control.AppRelativeTemplateSourceDirectory = "~/";
      _control.SearchServicePath = "~/SearchService.asmx";

      bool result = ((IPostBackDataHandler) _control).LoadPostData (_control.UniqueID, postbackCollection);
      Assert.That (_control.IsDirty, Is.False);
      Assert.That (result, Is.False);
      Assert.That (((IBocAutoCompleteReferenceValue) _control).BusinessObjectUniqueIdentifier, Is.EqualTo (displayName));
      Assert.That (((IBocAutoCompleteReferenceValue) _control).GetLabelText(), Is.EqualTo (displayName));
    }

    [Test]
    public void LoadPostData_Empty_ChangedToInvalidValue ()
    {
      PrivateInvoke.InvokeNonPublicMethod (_control, "CreateChildControls");
      var controlInvoker = new Remotion.Development.Web.UnitTesting.UI.Controls.ControlInvoker (_control);
      var controlState = controlInvoker.SaveControlState();
      controlInvoker.LoadControlState (controlState);

      var postbackCollection = new NameValueCollection();

      postbackCollection.Add (
          ((IBocAutoCompleteReferenceValue) _control).GetKeyValueName(),
          ((IBocAutoCompleteReferenceValue) _control).NullValueString);
      postbackCollection.Add (((IBocAutoCompleteReferenceValue) _control).GetTextValueName(), "InvalidValue");

      _control.Value = null;
      _control.IsDirty = false;

      PrivateInvoke.SetNonPublicField (_control, "_hasBeenRenderedInPreviousLifecycle", true);
      ((ISmartPage) _control.Page).Stub (stub => stub.GetPostBackCollection()).Return (postbackCollection);

      _webServiceFactoryStub
          .Stub (stub => stub.CreateJsonService<ISearchAvailableObjectWebService> ("~/SearchService.asmx"))
          .Return (MockRepository.GenerateStub<ISearchAvailableObjectWebService>());
      _control.AppRelativeTemplateSourceDirectory = "~/";
      _control.SearchServicePath = "~/SearchService.asmx";

      bool result = ((IPostBackDataHandler) _control).LoadPostData (_control.UniqueID, postbackCollection);
      Assert.That (_control.IsDirty, Is.True);
      Assert.That (result, Is.True);
      Assert.That (_control.BusinessObjectUniqueIdentifier, Is.Null);
      Assert.That (((IBocAutoCompleteReferenceValue) _control).GetLabelText(), Is.EqualTo ("InvalidValue"));
    }

    [Test]
    public void LoadPostData_NotEmpty_ChangedToInvalidValue ()
    {
      PrivateInvoke.InvokeNonPublicMethod (_control, "CreateChildControls");
      var controlInvoker = new Remotion.Development.Web.UnitTesting.UI.Controls.ControlInvoker (_control);
      var controlState = controlInvoker.SaveControlState();
      controlInvoker.LoadControlState (controlState);

      var postbackCollection = new NameValueCollection();

      postbackCollection.Add (
          ((IBocAutoCompleteReferenceValue) _control).GetKeyValueName(),
          ((IBocAutoCompleteReferenceValue) _control).NullValueString);
      postbackCollection.Add (((IBocAutoCompleteReferenceValue) _control).GetTextValueName(), "InvalidValue");

      _control.IsDirty = false;

      PrivateInvoke.SetNonPublicField (_control, "_hasBeenRenderedInPreviousLifecycle", true);
      ((ISmartPage) _control.Page).Stub (stub => stub.GetPostBackCollection()).Return (postbackCollection);

      _webServiceFactoryStub
          .Stub (stub => stub.CreateJsonService<ISearchAvailableObjectWebService> ("~/SearchService.asmx"))
          .Return (MockRepository.GenerateStub<ISearchAvailableObjectWebService>());
      _control.AppRelativeTemplateSourceDirectory = "~/";
      _control.SearchServicePath = "~/SearchService.asmx";

      bool result = ((IPostBackDataHandler) _control).LoadPostData (_control.UniqueID, postbackCollection);
      Assert.That (_control.IsDirty, Is.True);
      Assert.That (result, Is.True);
      Assert.That (_control.BusinessObjectUniqueIdentifier, Is.Null);
      Assert.That (((IBocAutoCompleteReferenceValue) _control).GetLabelText(), Is.EqualTo ("InvalidValue"));
    }

    [Test]
    public void LoadPostData_ResolvesDisplayName ()
    {
      PrivateInvoke.InvokeNonPublicMethod (_control, "CreateChildControls");
      var controlInvoker = new Remotion.Development.Web.UnitTesting.UI.Controls.ControlInvoker (_control);
      var controlState = controlInvoker.SaveControlState();
      controlInvoker.LoadControlState (controlState);

      var postbackCollection = new NameValueCollection();

      postbackCollection.Add (
          ((IBocAutoCompleteReferenceValue) _control).GetKeyValueName(),
          ((IBocAutoCompleteReferenceValue) _control).NullValueString);
      postbackCollection.Add (((IBocAutoCompleteReferenceValue) _control).GetTextValueName(), "SomeValue");

      _control.IsDirty = false;
      PrivateInvoke.SetNonPublicField (_control, "_hasBeenRenderedInPreviousLifecycle", true);
      var searchAvailableObjectWebServiceContext = SearchAvailableObjectWebServiceContext.Create (_dataSource, _propertyReferenceValue, "Args");
      PrivateInvoke.SetNonPublicField (_control, "_searchServiceContextFromPreviousLifeCycle", searchAvailableObjectWebServiceContext);
      ((ISmartPage) _control.Page).Stub (stub => stub.GetPostBackCollection()).Return (postbackCollection);

      var searchServiceStub = MockRepository.GenerateStub<ISearchAvailableObjectWebService>();
      searchServiceStub
          .Stub (
              stub => stub.SearchExact (
                  "SomeValue",
                  searchAvailableObjectWebServiceContext.BusinessObjectClass,
                  searchAvailableObjectWebServiceContext.BusinessObjectProperty,
                  searchAvailableObjectWebServiceContext.BusinessObjectIdentifier,
                  searchAvailableObjectWebServiceContext.Args))
          .Return (new BusinessObjectWithIdentityProxy { DisplayName = "ValidName", UniqueIdentifier = "ValidIdentifier" });
      _webServiceFactoryStub
          .Stub (stub => stub.CreateJsonService<ISearchAvailableObjectWebService> ("~/SearchService.asmx"))
          .Return (searchServiceStub);
      _control.AppRelativeTemplateSourceDirectory = "~/";
      _control.SearchServicePath = "~/SearchService.asmx";

      bool result = ((IPostBackDataHandler) _control).LoadPostData (_control.UniqueID, postbackCollection);
      Assert.That (_control.IsDirty, Is.True);
      Assert.That (result, Is.True);
      Assert.That (_control.BusinessObjectUniqueIdentifier, Is.EqualTo ("ValidIdentifier"));
      Assert.That (((IBocAutoCompleteReferenceValue) _control).GetLabelText(), Is.EqualTo ("ValidName"));
    }

    [Test]
    public void LoadValueInterim ()
    {
      _control.IsDirty = true;

      _control.Property = _propertyReferenceValue;
      _control.DataSource = _dataSource;

      var newValue = (IBusinessObjectWithIdentity) TypeWithReference.Create();
      _control.Value = newValue;
      Assert.That (_control.Value, Is.EqualTo (newValue));

      _control.LoadValue (true);

      Assert.That (_control.Value, Is.EqualTo (newValue));
      Assert.That (_control.IsDirty);
    }

    [Test]
    public void LoadValueInitial ()
    {
      _control.IsDirty = true;

      _control.Property = _propertyReferenceValue;
      _control.DataSource = _dataSource;

      var propertyValue = _dataSource.BusinessObject.GetProperty (_propertyReferenceValue);
      var newValue = (IBusinessObjectWithIdentity) TypeWithReference.Create();
      _control.Value = newValue;
      Assert.That (_control.Value, Is.EqualTo (newValue));

      _control.LoadValue (false);

      Assert.That (_control.Value, Is.EqualTo (propertyValue));
      Assert.That (!_control.IsDirty);
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceNull ()
    {
      var value = (IBusinessObjectWithIdentity) TypeWithReference.Create();
      _control.DataSource = null;
      _control.Property = _propertyReferenceValue;
      _control.Value = value;
      _control.IsDirty = true;

      _control.LoadValue (false);
      Assert.That (_control.Value, Is.EqualTo (value));
      Assert.That (_control.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimFalseWithPropertyNull ()
    {
      var value = (IBusinessObjectWithIdentity) TypeWithReference.Create();
      _control.DataSource = _dataSource;
      _control.Property = null;
      _control.Value = value;
      _control.IsDirty = true;

      _control.LoadValue (false);
      Assert.That (_control.Value, Is.EqualTo (value));
      Assert.That (_control.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceBusinessObjectNull ()
    {
      _dataSource.BusinessObject = null;
      _control.DataSource = _dataSource;
      _control.Property = _propertyReferenceValue;
      _control.Value = (IBusinessObjectWithIdentity) TypeWithReference.Create();
      _control.IsDirty = true;

      _control.LoadValue (false);
      Assert.That (_control.Value, Is.EqualTo (null));
      Assert.That (_control.IsDirty, Is.False);
    }


    [Test]
    public void LoadUnboundValueInterim ()
    {
      var oldValue = _control.Value;
      var newValue = (IBusinessObjectWithIdentity) TypeWithReference.Create();

      _control.LoadUnboundValue (newValue, true);

      Assert.That (_control.Value, Is.EqualTo (oldValue));
      Assert.That (_control.IsDirty);
    }

    [Test]
    public void LoadUnboundValueInitial ()
    {
      var newValue = (IBusinessObjectWithIdentity) TypeWithReference.Create();

      _control.LoadUnboundValue (newValue, false);

      Assert.That (_control.Value, Is.EqualTo (newValue));
      Assert.That (!_control.IsDirty);
    }

    [Test]
    public void SaveValueAndInterimTrue ()
    {
      var value = TypeWithReference.Create();
      _businessObject.ReferenceValue = value;
      _control.DataSource = _dataSource;
      _control.Property = _propertyReferenceValue;
      _control.Value = null;
      _control.IsDirty = true;

      var result = _control.SaveValue (true);
      Assert.That (result, Is.False);
      Assert.That (_businessObject.ReferenceValue, Is.EqualTo (value));
      Assert.That (_control.IsDirty, Is.True);
    }

    [Test]
    public void SaveValueAndInterimFalse ()
    {
      var value = TypeWithReference.Create();
      _businessObject.ReferenceValue = value;
      _control.DataSource = _dataSource;
      _control.Property = _propertyReferenceValue;
      _control.Value = null;
      _control.IsDirty = true;

      var result = _control.SaveValue (false);
      Assert.That (result, Is.True);
      Assert.That (_businessObject.ReferenceValue, Is.EqualTo (null));
      Assert.That (_control.IsDirty, Is.False);
    }

    [Test]
    public void SaveValueAndNotValid ()
    {
      var value = TypeWithReference.Create();
      _businessObject.ReferenceValue = value;
      _control.DataSource = _dataSource;
      _control.Property = _propertyReferenceValue;
      _control.Value = null;
      _control.IsDirty = true;
      _control.RegisterValidator (new AlwaysInvalidValidator());

      var result = _control.SaveValue (false);
      Assert.That (result, Is.False);
      Assert.That (_businessObject.ReferenceValue, Is.EqualTo (value));
      Assert.That (_control.IsDirty, Is.True);
    }

    [Test]
    public void SaveValueAndIsDirtyFalse ()
    {
      var value = TypeWithReference.Create();
      _businessObject.ReferenceValue = value;
      _control.DataSource = _dataSource;
      _control.Property = _propertyReferenceValue;
      _control.Value = null;
      _control.IsDirty = false;

      var result = _control.SaveValue (false);
      Assert.That (result, Is.True);
      Assert.That (_businessObject.ReferenceValue, Is.EqualTo (value));
      Assert.That (_control.IsDirty, Is.False);
    }

    [Test]
    public void CreateValidatorsReadOnly ()
    {
      _control.NullItemErrorMessage = "RequiredFieldErrorMessage";
      _control.ReadOnly = true;
      _control.Required = true;
      BaseValidator[] validators = _control.CreateValidators().ToArray();
      Assert.That (validators.Length, Is.EqualTo (0));
    }

    [Test]
    public void CreateValidatorsNotRequired ()
    {
      _control.ReadOnly = false;
      _control.Required = false;
      BaseValidator[] validators = _control.CreateValidators().ToArray();
      Assert.That (validators.Length, Is.EqualTo (1));
      Assert.That (validators[0] is BocAutoCompleteReferenceValueInvalidDisplayNameValidator);
    }

    [Test]
    public void CreateValidatorsEditableRequired ()
    {
      _control.ReadOnly = false;
      _control.Required = true;
      BaseValidator[] validators = _control.CreateValidators().ToArray();
      Assert.That (validators.Length, Is.EqualTo (2));
      Assert.That (validators[0] is RequiredFieldValidator);
      Assert.That (validators[1] is BocAutoCompleteReferenceValueInvalidDisplayNameValidator);
    }

    [Test]
    public void RaisePostDataChangedEvent ()
    {
      bool eventHandlerCalled = false;
      _control.SelectionChanged += (sender, e) => { eventHandlerCalled = true; };
      ((IPostBackDataHandler) _control).RaisePostDataChangedEvent();

      Assert.That (eventHandlerCalled);
    }

    [Test]
    public void RaisePostDataChangedEventReadOnly ()
    {
      _control.ReadOnly = true;
      bool eventHandlerCalled = false;
      _control.SelectionChanged += (sender, e) => { eventHandlerCalled = true; };
      ((IPostBackDataHandler) _control).RaisePostDataChangedEvent();

      Assert.That (!eventHandlerCalled);
    }

    [Test]
    public void RaisePostDataChangedEventDisabled ()
    {
      _control.Enabled = false;
      bool eventHandlerCalled = false;
      _control.SelectionChanged += (sender, e) => { eventHandlerCalled = true; };
      ((IPostBackDataHandler) _control).RaisePostDataChangedEvent();

      Assert.That (!eventHandlerCalled);
    }

    [Test]
    public void GetValidationValue_ValueSet ()
    {
      var value = TypeWithReference.Create ("Name");
      _control.Value = (IBusinessObjectWithIdentity) value;

      Assert.That (_control.ValidationValue, Is.EqualTo (value.UniqueIdentifier + "\n" + value.DisplayName));
    }

    [Test]
    public void GetValidationValue_ValueNull ()
    {
      _control.Value = null;

      Assert.That (_control.ValidationValue, Is.Null);
    }

    [Test]
    public void GetValidationValue_UniqueIdentifierNull ()
    {
      var classStub = MockRepository.GenerateStub<IBusinessObjectClass>();
      var propertyStub = MockRepository.GenerateStub<IBusinessObjectProperty>();
      var businessObjectWithIdentityStub = MockRepository.GenerateStub<IBusinessObjectWithIdentity>();

      businessObjectWithIdentityStub.Stub (stub => stub.UniqueIdentifier).Return (null);
      businessObjectWithIdentityStub.Stub (_ => _.BusinessObjectClass).Return (classStub);
      businessObjectWithIdentityStub.Stub (_ => _.GetProperty (propertyStub)).Return ("Name");

      classStub.Stub (_ => _.GetPropertyDefinition ("DisplayName")).Return (propertyStub);
      propertyStub.Stub (_ => _.IsAccessible (businessObjectWithIdentityStub)).Return (true);

      _control.Value = businessObjectWithIdentityStub;

      Assert.That (_control.ValidationValue, Is.EqualTo ("\nName"));
    }

    [Test]
    public void GetTextValueName ()
    {
      Assert.That (((IBocAutoCompleteReferenceValue) _control).GetTextValueName(), Is.EqualTo ("BocAutoCompleteReferenceValue_TextValue"));
    }

    [Test]
    public void GetKeyValueName ()
    {
      Assert.That (((IBocAutoCompleteReferenceValue) _control).GetKeyValueName(), Is.EqualTo ("BocAutoCompleteReferenceValue_KeyValue"));
    }

    [Test]
    public void CreateValidators_UsesValidatorFactory ()
    {
      var control = new BocAutoCompleteReferenceValue();
      var serviceLocatorMock = MockRepository.GenerateMock<IServiceLocator>();
      var factoryMock = MockRepository.GenerateMock<IBocAutoCompleteReferenceValueValidatorFactory>();
      serviceLocatorMock.Expect (m => m.GetInstance<IBocAutoCompleteReferenceValueValidatorFactory>()).Return (factoryMock);
      factoryMock.Expect (f => f.CreateValidators (control, false)).Return (new List<BaseValidator>());

      using (new ServiceLocatorScope (serviceLocatorMock))
      {
        control.CreateValidators();
      }

      factoryMock.VerifyAllExpectations();
      serviceLocatorMock.VerifyAllExpectations();
    }
  }
}