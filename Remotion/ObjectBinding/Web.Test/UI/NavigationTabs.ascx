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
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="NavigationTabs.ascx.cs" Inherits="OBWTest.UI.NavigationTabs" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>


<remotion:TabbedMenu id="TabbedMenu" runat="server">
<tabs>
<remotion:MainMenuTab Text="Tests by Control" ItemID="IndividualControlTests">
<submenutabs>
<remotion:SubMenuTab Text="Boolean" ItemID="BocBooleanValue">
<persistedcommand>
<remotion:NavigationCommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocBooleanValueUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></remotion:NavigationCommand>
</PersistedCommand>
</remotion:SubMenuTab>

<remotion:submenutab Text="CheckBox" ItemID="BocCheckBox">
<persistedcommand>
<remotion:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocCheckBoxUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></remotion:navigationcommand>
</PersistedCommand>
</remotion:submenutab>

<remotion:submenutab Text="DateTime" ItemID="BocDateTimeValue">
<persistedcommand>
<remotion:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocDateTimeValueUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></remotion:navigationcommand>
</PersistedCommand>
</remotion:submenutab>

<remotion:submenutab Text="DropDown" ItemID="BocDropDownMenu">
<persistedcommand>
<remotion:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocDropDownMenuUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></remotion:navigationcommand>
</PersistedCommand>
</remotion:submenutab>

<remotion:submenutab Text="Enum" ItemID="BocEnumValue">
<persistedcommand>
<remotion:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocEnumValueUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></remotion:navigationcommand>
</PersistedCommand>
</remotion:submenutab>

<remotion:submenutab Text="List" ItemID="BocList">
<persistedcommand>
<remotion:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocListUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></remotion:navigationcommand>
</PersistedCommand>
</remotion:submenutab>

<remotion:submenutab Text="List as Grid" ItemID="BocListAsGrid">
<persistedcommand>
<remotion:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocListAsGridUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></remotion:navigationcommand>
</PersistedCommand>
</remotion:submenutab>

<remotion:submenutab Text="Literal" ItemID="BocLiteral">
<persistedcommand>
<remotion:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocLiteralUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></remotion:navigationcommand>
</PersistedCommand>
</remotion:submenutab>

<remotion:submenutab Text="MultilineText" ItemID="BocMultilineTextValue">
<persistedcommand>
<remotion:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocMultilineTextValueUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></remotion:navigationcommand>
</PersistedCommand>
</remotion:submenutab>

<remotion:submenutab Text="Reference" ItemID="BocReferenceValue">
<persistedcommand>
<remotion:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocReferenceValueUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></remotion:navigationcommand>
</PersistedCommand>
</remotion:submenutab>

<remotion:submenutab Text="Auto Complete Reference" ItemID="BocAutoCompleteReferenceValue">
<persistedcommand>
<remotion:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocAutoCompleteReferenceValueUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></remotion:navigationcommand>
</PersistedCommand>
</remotion:submenutab>

<remotion:submenutab Text="Text" ItemID="BocTextValue">
<persistedcommand>
<remotion:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocTextValueUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></remotion:navigationcommand>
</PersistedCommand>
</remotion:submenutab>

</SubMenuTabs>

<persistedcommand>
<remotion:NavigationCommand Type="None"></remotion:NavigationCommand>
</PersistedCommand>
</remotion:MainMenuTab>

<remotion:MainMenuTab Text="Tests for View Layout" ItemID="ViewLayoutTests">
<persistedcommand>
<remotion:navigationcommand Type="WxeFunction" WxeFunctionCommand-MappingID="ViewLayoutTest"></remotion:navigationcommand>
</PersistedCommand>
</remotion:MainMenuTab>

<remotion:MainMenuTab Text="Tests for Control Layout Compatility" ItemID="ControlLayoutTests">
<persistedcommand>
<remotion:navigationcommand Type="WxeFunction" WxeFunctionCommand-MappingID="ControlLayoutTest"></remotion:navigationcommand>
</PersistedCommand>
</remotion:MainMenuTab>

</Tabs>
</remotion:TabbedMenu>
<div style="WIDTH: 100%;TEXT-ALIGN: right">
<asp:CheckBox ID="MainContentScrollableCheckBox" runat="server" AutoPostBack="True" Text="Main content scrollable" OnCheckedChanged="MainContentScrollableCheckBox_OnCheckedChanged"/>
</div>
