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

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Form.aspx.cs" Inherits="OBWTest.ControlLayoutTests.Form" MasterPageFile="~/StandardMode.Master" %>

<%@ Register TagPrefix="obwt" TagName="NavigationTabs" Src="../UI/NavigationTabs.ascx" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <asp:ScriptManager ID="ScriptManager" runat="server" EnablePartialRendering="true" AsyncPostBackTimeout="3600" />
  <remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" />
  <remotion:FormGridManager ID="FormGridManager" runat="server" />
  <remotion:SingleView ID="OuterSingleView" runat="server">
    <TopControls>
      <obwt:NavigationTabs ID="NavigationTabs" runat="server" />
    </TopControls>
    <View>
      <asp:PlaceHolder runat="server">
        <table id="FormGrid" runat="server">
          <tr>
            <td>
              Line&nbsp;1
            </td>
            <td>
              <remotion:BocCheckBox ID="Line01CheckBox01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Deceased" Width="3em" />
              <remotion:BocBooleanValue ID="Line01BooleanValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Deceased" Width="3em" />
              <remotion:BocTextValue ID="Line01TextValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="LastName" Width="8em" />
              <remotion:BocEnumValue ID="Line01EnumValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="MarriageStatus" Width="8em" />
              <remotion:BocDateTimeValue ID="Line01DateTimeValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateOfBirth" Width="15em" />
              <remotion:BocDateTimeValue ID="Line01DateTimeValue02" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateOfDeath" Width="10em" />
              <remotion:BocReferenceValue ID="Line01ReferenceValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Partner" Width="15em" ReserveOptionsMenuWidth="true">
                <OptionsMenuItems>
                  <remotion:WebMenuItem ItemID="Item1" Text="Item 1" />
                </OptionsMenuItems>
              </remotion:BocReferenceValue>
              <remotion:BocAutoCompleteReferenceValue ID="Line01AutoCompleteReferenceValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Partner"
                Width="15em" ReserveOptionsMenuWidth="true" ControlServicePath="~/IndividualControlTests/BocAutoCompleteReferenceValueWebService.asmx">
                <OptionsMenuItems>
                  <remotion:WebMenuItem ItemID="Item1" Text="Item 1" />
                </OptionsMenuItems>
              </remotion:BocAutoCompleteReferenceValue>
              <remotion:WebButton ID="Line01Button" runat="server" Text="Button" Icon-Url="Images/NullIcon.gif" />
            </td>
            <td>
              Line&nbsp;1
            </td>
          </tr>
          <tr>
            <td>
              Line&nbsp;2
            </td>
            <td>
              <remotion:BocCheckBox ID="Line02CheckBox01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Deceased" Width="3em" ReadOnly="true" />
              <remotion:BocBooleanValue ID="Line02BooleanValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Deceased" Width="3em" ReadOnly="true" />
              <remotion:BocTextValue ID="Line02TextValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="LastName" Width="8em" ReadOnly="true" />
              <remotion:BocEnumValue ID="Line02EnumValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="MarriageStatus" Width="8em" ReadOnly="true" />
              <remotion:BocDateTimeValue ID="Line02DateTimeValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateOfBirth" Width="15em" ReadOnly="true" />
              <remotion:BocDateTimeValue ID="Line02DateTimeValue02" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateOfDeath" Width="10em" ReadOnly="true" />
              <remotion:BocReferenceValue ID="Line02ReferenceValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Partner" Width="15em" ReserveOptionsMenuWidth="true" ReadOnly="true">
                <OptionsMenuItems>
                  <remotion:WebMenuItem ItemID="Item1" Text="Item 1" />
                </OptionsMenuItems>
              </remotion:BocReferenceValue>
              <remotion:BocAutoCompleteReferenceValue ID="Line02AutoCompleteReferenceValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Partner"
                Width="15em" ReserveOptionsMenuWidth="true" ReadOnly="true" ControlServicePath="~/IndividualControlTests/BocAutoCompleteReferenceValueWebService.asmx">
                <OptionsMenuItems>
                  <remotion:WebMenuItem ItemID="Item1" Text="Item 1" />
                </OptionsMenuItems>
              </remotion:BocAutoCompleteReferenceValue>
              <remotion:WebButton ID="Line02Button" runat="server" Text="Button" Icon-Url="Images/NullIcon.gif" />
            </td>
            <td>
              Line&nbsp;2
            </td>
          </tr>
          <tr>
            <td>
              Line&nbsp;3
            </td>
            <td>
              M
              <remotion:BocCheckBox ID="Line03CheckBox01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Deceased" Width="3em" />
              M
              <remotion:BocBooleanValue ID="Line03BooleanValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Deceased" Width="3em" />
              M
              <remotion:BocTextValue ID="Line03TextValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="LastName" Width="8em" />
              M
              <remotion:BocEnumValue ID="Line03EnumValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="MarriageStatus" Width="8em" />
              M
              <remotion:BocDateTimeValue ID="Line03DateTimeValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateOfBirth" Width="15em" />
              M
              <remotion:BocDateTimeValue ID="Line03DateTimeValue02" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateOfDeath" Width="10em" />
              M
              <remotion:BocReferenceValue ID="Line03ReferenceValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Partner" Width="15em" ReserveOptionsMenuWidth="true">
                <OptionsMenuItems>
                  <remotion:WebMenuItem ItemID="Item1" Text="Item 1" />
                </OptionsMenuItems>
              </remotion:BocReferenceValue>
              M
              <remotion:BocAutoCompleteReferenceValue ID="Line03AutoCompleteReferenceValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Partner"
                Width="15em" ReserveOptionsMenuWidth="true" ControlServicePath="~/IndividualControlTests/BocAutoCompleteReferenceValueWebService.asmx">
                <OptionsMenuItems>
                  <remotion:WebMenuItem ItemID="Item1" Text="Item 1" />
                </OptionsMenuItems>
              </remotion:BocAutoCompleteReferenceValue>
              M
              <remotion:WebButton ID="Line03Button" runat="server" Text="Button" Icon-Url="Images/NullIcon.gif" />
              M
            </td>
            <td>
              Line&nbsp;3
            </td>
          </tr>
          <tr>
            <td>
              Line&nbsp;3
            </td>
            <td>
              M
              <remotion:BocCheckBox ID="Line04CheckBox01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Deceased" Width="3em" ReadOnly="true" />
              M
              <remotion:BocBooleanValue ID="Line04BooleanValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Deceased" Width="3em" ReadOnly="true" />
              M
              <remotion:BocTextValue ID="Line04TextValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="LastName" Width="8em" ReadOnly="true" />
              M
              <remotion:BocEnumValue ID="Line04EnumValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="MarriageStatus" Width="8em" ReadOnly="true" />
              M
              <remotion:BocDateTimeValue ID="Line04DateTimeValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateOfBirth" Width="15em" ReadOnly="true" />
              M
              <remotion:BocDateTimeValue ID="Line04DateTimeValue02" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateOfDeath" Width="10em" ReadOnly="true" />
              M
              <remotion:BocReferenceValue ID="Line04ReferenceValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Partner" Width="15em" ReserveOptionsMenuWidth="true" ReadOnly="true">
                <OptionsMenuItems>
                  <remotion:WebMenuItem ItemID="Item1" Text="Item 1" />
                </OptionsMenuItems>
              </remotion:BocReferenceValue>
              M
              <remotion:BocAutoCompleteReferenceValue ID="Line04AutoCompleteReferenceValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Partner"
                Width="15em" ReserveOptionsMenuWidth="true" ReadOnly="true" ControlServicePath="~/IndividualControlTests/BocAutoCompleteReferenceValueWebService.asmx">
                <OptionsMenuItems>
                  <remotion:WebMenuItem ItemID="Item1" Text="Item 1" />
                </OptionsMenuItems>
              </remotion:BocAutoCompleteReferenceValue>
              M
              <remotion:WebButton ID="Line04Button" runat="server" Text="Button" Icon-Url="Images/NullIcon.gif" />
              M
            </td>
            <td>
              Line&nbsp;3
            </td>
          </tr>
          <tr>
            <td>
              Check Box
            </td>
            <td>
              <remotion:BocCheckBox ID="CheckBox" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Deceased" ShowDescription="True" />
              <remotion:BocCheckBox ID="CheckBoxReadOnly" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Deceased" ReadOnly="true" ShowDescription="True" />
              M
            </td>
            <td>
            </td>
          </tr>
          <tr>
            <td>
              Boolean Value
            </td>
            <td>
              <remotion:BocBooleanValue ID="BooleanValue" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Deceased" />
              <remotion:BocBooleanValue ID="BooleanValueReadOnly" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Deceased" ReadOnly="true" />
              M
            </td>
            <td>
            </td>
          </tr>
          <tr>
            <td>
              Text Value
            </td>
            <td>
              <remotion:BocTextValue ID="TextValue" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="LastName" />
              <remotion:BocTextValue ID="TextValueReadOnly" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="LastName" ReadOnly="true" />
              M
            </td>
            <td>
            </td>
          </tr>
          <tr>
            <td>
              Enum Value
            </td>
            <td>
              <remotion:BocEnumValue ID="EnumValue" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="MarriageStatus" />
              <remotion:BocEnumValue ID="EnumValueReadOnly" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="MarriageStatus" ReadOnly="true" />
              M
            </td>
            <td>
            </td>
          </tr>
          <tr>
            <td>
              Date Time Value
            </td>
            <td>
              <remotion:BocDateTimeValue ID="DateTmeValue" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateOfBirth" />
              <remotion:BocDateTimeValue ID="DateTimeValueReadOnly" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateOfBirth" ReadOnly="true" />
              M
            </td>
            <td>
            </td>
          </tr>
          <tr>
            <td>
              Date Value
            </td>
            <td>
              <remotion:BocDateTimeValue ID="DateValue" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateOfDeath" />
              <remotion:BocDateTimeValue ID="DateValueReadOnly" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateOfDeath" ReadOnly="true" />
              M
            </td>
            <td>
            </td>
          </tr>
          <tr>
            <td>
              Reference Value
            </td>
            <td>
              <remotion:BocReferenceValue ID="ReferenceValue" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Partner" >
                <OptionsMenuItems>
                  <remotion:WebMenuItem ItemID="Item1" Text="Item 1" />
                </OptionsMenuItems>
              </remotion:BocReferenceValue>
              <remotion:BocReferenceValue ID="ReferenceValueReadOnly" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Partner" ReadOnly="true">
                <OptionsMenuItems>
                  <remotion:WebMenuItem ItemID="Item1" Text="Item 1" />
                </OptionsMenuItems>
              </remotion:BocReferenceValue>
              M
            </td>
            <td>
            </td>
          </tr>
          <tr>
            <td>
              Auto Complete Refence Value
            </td>
            <td>
              <remotion:BocAutoCompleteReferenceValue ID="AutoCompleteReferenceValue" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Partner" 
                ControlServicePath="~/IndividualControlTests/BocAutoCompleteReferenceValueWebService.asmx">
                <OptionsMenuItems>
                  <remotion:WebMenuItem ItemID="Item1" Text="Item 1" />
                </OptionsMenuItems>
              </remotion:BocAutoCompleteReferenceValue>
              <remotion:BocAutoCompleteReferenceValue ID="AutoCompleteReferenceValueReadOnly" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Partner"
                ReadOnly="true" ControlServicePath="~/IndividualControlTests/BocAutoCompleteReferenceValueWebService.asmx">
                <OptionsMenuItems>
                  <remotion:WebMenuItem ItemID="Item1" Text="Item 1" />
                </OptionsMenuItems>
              </remotion:BocAutoCompleteReferenceValue>
              M
            </td>
            <td>
            </td>
          </tr>
          <tr>
            <td>
            HTML Input
            </td>
            <td>
            M
            <input type="text" name="TextInput" value="Some Text" />
            M
            <input type="checkbox" nane="CheckBoxInput" checked="checked" />
            M
            <select name="Select" >
            <option selected="selected">Option 1</option>
            </select>
            </td>
            <td>
            </td>
          </tr>
          <tr>
            <td>
              Button
            </td>
            <td>
              <remotion:WebButton ID="WebButton" runat="server" Text="Button" />
              <remotion:WebButton ID="WebButtonWithImage" runat="server" Text="Button" Icon-Url="Images/NullIcon.gif" />
              <remotion:WebButton ID="WebButtonDisabled" runat="server" Text="Button" Icon-Url="Images/NullIcon.gif" />
              M
              <remotion:WebButton runat="server" Text="Standard" ButtonType="Standard" />
              M
              <remotion:WebButton runat="server" Text="Primary" ButtonType="Primary" />
              M
              <remotion:WebButton runat="server" Text="Supplemental" ButtonType="Supplemental" />
              M
            </td>
            <td>
            </td>
          </tr>
          <tr>
            <td>
              DropDownMenu
            </td>
            <td>
              M
              <remotion:DropDownMenu runat="server" TitleText="Standard" ButtonType="Standard" />
              M
              <remotion:DropDownMenu runat="server" ButtonType="Standard" />
              M
              <remotion:DropDownMenu runat="server" TitleText="Primary" ButtonType="Primary" />
              M
              <remotion:DropDownMenu runat="server" ButtonType="Primary" />
              M
              <remotion:DropDownMenu runat="server" TitleText="Supplemental" ButtonType="Supplemental" />
              M
              <remotion:DropDownMenu runat="server" ButtonType="Supplemental" />
              M
            </td>
            <td>
            </td>
          </tr>

        </table>
        <p></p>
        <div>
          Line 11 M
          <remotion:BocCheckBox ID="Line11CheckBox01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Deceased" Width="3em" />
          M
          <remotion:BocBooleanValue ID="Line11BooleanValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Deceased" Width="3em" />
          M
          <remotion:BocTextValue ID="Line11TextValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="LastName" Width="8em" />
          M
          <remotion:BocEnumValue ID="Line11EnumValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="MarriageStatus" Width="8em" />
          M
          <remotion:BocDateTimeValue ID="Line11DateTimeValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateOfBirth" Width="15em" />
          M
          <remotion:BocDateTimeValue ID="Line11DateTimeValue02" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateOfDeath" Width="10em" />
          M
          <remotion:BocReferenceValue ID="Line11ReferenceValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Partner" Width="15em" ReserveOptionsMenuWidth="true">
            <OptionsMenuItems>
              <remotion:WebMenuItem ItemID="Item1" Text="Item 1" />
            </OptionsMenuItems>
          </remotion:BocReferenceValue>
          M
          <remotion:BocAutoCompleteReferenceValue ID="Line11AutoCompleteReferenceValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Partner"
            Width="15em" ReserveOptionsMenuWidth="true" ControlServicePath="~/IndividualControlTests/BocAutoCompleteReferenceValueWebService.asmx">
            <OptionsMenuItems>
              <remotion:WebMenuItem ItemID="Item1" Text="Item 1" />
            </OptionsMenuItems>
          </remotion:BocAutoCompleteReferenceValue>
          M
          <remotion:WebButton ID="Line11Button" runat="server" Text="Button" Icon-Url="Images/NullIcon.gif" />
          M
        </div>
      </asp:PlaceHolder>
    </View>
    <BottomControls>
    </BottomControls>
  </remotion:SingleView>
</asp:Content>
