<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TestUserControl.ascx.cs" Inherits="Remotion.ObjectBinding.Web.Test.Shared.Validation.TestUserControl" %>
<%@ Register tagPrefix="remotion" Namespace="Remotion.Web.UI.Controls" Assembly="Remotion.Web" %>
<%@ Register tagPrefix="remotion" Namespace="Remotion.ObjectBinding.Web.UI.Controls" Assembly="Remotion.ObjectBinding.Web" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.ObjectBinding.Web.UI.Controls.Validation" Assembly="Remotion.ObjectBinding.Web" %>

<%@ Import Namespace="System" %>
<table id="FormGrid" runat="server" style="margin-top: 0%">
  <tr>
    <td></td>
    <td>
      <remotion:BocTextValue ID="LastNameField_UserControlBinding" runat="server" PropertyIdentifier="LastName" DataSourceControl="CurrentObject">
        <TextBoxStyle TextMode="SingleLine" AutoPostBack="False"></TextBoxStyle>
      </remotion:BocTextValue>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocTextValue ID="FirstNameField_UserControlBinding" runat="server" PropertyIdentifier="FirstName" DataSourceControl="CurrentObject"></remotion:BocTextValue>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocTextValue ID="ParterLastNameField" runat="server" DataSourceControl="PartnerDataSource" PropertyIdentifier="LastName" Width="100%">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </remotion:BocTextValue>
    </td>
  </tr>
  <tr>
    <td>
      
    </td>
    <td>
      <remotion:bocreferencevalue id="ReferenceField" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Partner" NullItemErrorMessage="Fehler"></remotion:bocreferencevalue>
    </td>
  </tr>
</table>

<remotion:BindableObjectDataSourceControlValidationResultDispatchingValidator ID="DataSourceValidationResultDispatchingValidator" ControlToValidate="CurrentObject" runat="server"></remotion:BindableObjectDataSourceControlValidationResultDispatchingValidator>
<remotion:FormGridManager ID="FormGridManager" runat="server" Visible="true"></remotion:FormGridManager>
<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" />
<remotion:BusinessObjectReferenceDataSourceControl id="PartnerDataSource" runat="server" PropertyIdentifier="Partner" DataSourceControl="CurrentObject"></remotion:BusinessObjectReferenceDataSourceControl>
