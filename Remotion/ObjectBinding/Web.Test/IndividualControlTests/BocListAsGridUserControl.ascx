<%-- This file is part of the re-motion Core Framework (www.re-motion.org)
 % Copyright (c) rubicon IT GmbH, www.rubicon.eu
 %
 % The re-motion Core Framework is free software; you can redistribute it 
 % and/or modify it under the terms of the GNU Lesser General Public License 
 % as published by the Free Software Foundation; either version 2.1 of the 
 % License, or (at your option) any later version.
 %
 % re-motion is distributed in the hope that it will be useful, 
 % but WITHOUT ANY WARRANTY; without even the implied warranty of 
 % MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
 % GNU Lesser General Public License for more details.
 %
 % You should have received a copy of the GNU Lesser General Public License
 % along with re-motion; if not, see http://www.gnu.org/licenses.
--%>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="BocListAsGridUserControl.ascx.cs" Inherits="OBWTest.IndividualControlTests.BocListAsGridUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>




<table id=FormGrid width="80%" runat="server">
  <tr>
    <td colSpan=2><remotion:boctextvalue id=FirstNameField runat="server" PropertyIdentifier="FirstName" ReadOnly="True" datasourcecontrol="CurrentObject"></remotion:boctextvalue>&nbsp;<remotion:boctextvalue id=LastNameField runat="server" PropertyIdentifier="LastName" ReadOnly="True" datasourcecontrol="CurrentObject"></remotion:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td></td></tr>
  <tr>
    <td colSpan=2><ros:TestBocList id=ChildrenList runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Children" EnableOptionalValidators="true" alwaysshowpageinfo="True" listmenulinebreaks="BetweenGroups" pagesize="0" indexoffset="100" RowMenuDisplay="Manual" ShowEmptyListMessage="True" Index="InitialOrder" Selection="Multiple" errormessage="test" showeditmodevalidationmarkers="True">
<fixedcolumns>
<remotion:BocValidationErrorIndicatorColumnDefinition ColumnTitleStyle="Icon" />
<remotion:BocRowEditModeColumnDefinition ItemID="EditRow" SaveText="Save" CancelText="Cancel" Width="2em" EditText="Edit"></remotion:BocRowEditModeColumnDefinition>
<remotion:BocCommandColumnDefinition ItemID="E1" Text="E 1" ColumnTitle="Cmd">
<persistedcommand>
<remotion:BocListItemCommand Type="Event" CommandStateType="Remotion.ObjectBinding.Sample::PersonListItemCommandState" ToolTip="An Event Command"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocCommandColumnDefinition>
<remotion:BocCommandColumnDefinition ItemID="Href" Text="Href">
<persistedcommand>
<remotion:BocListItemCommand Type="Href" HrefCommand-Href="edit.aspx?ID={1}&amp;Index={0}"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocCommandColumnDefinition>
<remotion:BocSimpleColumnDefinition ItemID="LastName" PropertyPathIdentifier="LastName" EnforceWidth="True" Width="5em">
<persistedcommand>
<remotion:BocListItemCommand WxeFunctionCommand-Parameters="id" WxeFunctionCommand-TypeName="OBWTest.ViewPersonDetailsWxeFunction,OBWTest" Type="WxeFunction"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocCompoundColumnDefinition ItemID="Name" FormatString="{0}, {1}" EnforceWidth="True" Width="3em" ColumnTitle="Name" IsRowHeader="True">
<propertypathbindings>
<remotion:PropertyPathBinding PropertyPathIdentifier="LastName"></remotion:PropertyPathBinding>
<remotion:PropertyPathBinding PropertyPathIdentifier="FirstName"></remotion:PropertyPathBinding>
</PropertyPathBindings>

<persistedcommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocCompoundColumnDefinition>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="Partner" Width="20em" ColumnTitle="Partner">
<persistedcommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocSimpleColumnDefinition ItemID="PartnerFirstName" PropertyPathIdentifier="Partner.FirstName">
<persistedcommand>
<remotion:BocListItemCommand Type="Event"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="LastName" IsReadOnly="True">
<persistedcommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocCustomColumnDefinition ItemID="CustomCell" PropertyPathIdentifier="LastName" CustomCellType="Remotion.ObjectBinding.Sample::PersonCustomCell" Mode="ControlInEditedRow" ColumnTitle="Custom Cell"></remotion:BocCustomColumnDefinition>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="Deceased">
<persistedcommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocDropDownMenuColumnDefinition ItemID="RowMenu" MenuTitleText="Context" Width="0%" ColumnTitle="Menu"></remotion:BocDropDownMenuColumnDefinition>
</FixedColumns>
</ros:TestBocList></td></tr>

</table>
<p><remotion:WebButton id="SwitchToEditModeButton" runat="server" Text="Switch to Edit Mode"></remotion:WebButton><remotion:WebButton id="EndEditModeButton" runat="server" Text="End Edit Mode"></remotion:WebButton><remotion:WebButton id="CancelEditModeButton" runat="server" Text="Cancel Edit Mode"></remotion:WebButton></p>
<p><remotion:WebButton id="AddItemButton" runat="server" Text="Add Item"></remotion:WebButton>
  <remotion:WebButton id="AddRowButton" runat="server" Text="Add Row"></remotion:WebButton>
  <remotion:BocTextValue id="NumberOfNewRowsField" runat="server" ValueType="Int32" Width="2em" Required="True">
  <textboxstyle textmode="SingleLine">
  </TextBoxStyle>
  </remotion:BocTextValue><remotion:WebButton id="AddRowsButton" runat="server" Text="Add Rows"></remotion:WebButton>
  <remotion:WebButton id="RemoveRowsButton" runat="server" Text="Remove Rows"></remotion:WebButton>
  <remotion:WebButton id="RemoveItemsButton" runat="server" Text="Remove Items"></remotion:WebButton>
</p>
<p>
  <asp:CheckBox id="EnableValidationErrorsCheckBox" runat="server" AutoPostBack="True" Text="Enable validation errors" />
  <asp:DropDownList id="ValidationErrorsScenarioListbox" AutoPostBack="True" runat="server">
    <Items>
      <asp:ListItem Text="All validation errors" Selected="True" Value="all" />
      <asp:ListItem Text="Cell validation errors" Value="cell" />
      <asp:ListItem Text="Row validation errors" Value="row" />
      <asp:ListItem Text="List validation errors" Value="list" />
    </Items>
  </asp:DropDownList>
</p>
<p><asp:checkbox id=ChildrenListEventCheckBox runat="server" Text="ChildrenList Event raised" enableviewstate="False" Enabled="False"></asp:checkbox></p>
<p>
  <asp:label id=ChildrenListEventArgsLabel runat="server" enableviewstate="False"></asp:label>
  <br>
  <asp:Label id="UnhandledValidationErrorsLabel" runat="server" ForeColor="Red" style="white-space: pre"></asp:Label>
</p>
<div style="BORDER-RIGHT: black thin solid; BORDER-TOP: black thin solid; BORDER-LEFT: black thin solid; BORDER-BOTTOM: black thin solid; BACKGROUND-COLOR: #ffff99" runat="server" visible="false" ID="NonVisualControls">
    <remotion:formgridmanager id=FormGridManager runat="server"/>
    <remotion:BindableObjectDataSourceControl id=CurrentObject runat="server" Type="Remotion.ObjectBinding.Sample::Person"/>
    <remotion:BindableObjectDataSourceControl id=EmptyDataSourceControl runat="server" Type="Remotion.ObjectBinding.Sample::Person"/>
    <remotion:BindableObjectDataSourceControlValidationResultDispatchingValidator ID="CurrentObjectValidationResultDispatchingValidator" ControlToValidate="CurrentObject" runat="server" />
</div>
