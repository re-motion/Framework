<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrameContent.aspx.cs" Inherits="Remotion.Web.Test.IFrameSupport.FrameContent" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
  <script type="text/javascript">
    function UpdateMain()
    {
      window.parent.Refresh();
    }
  </script>
</head>
<body>
    <form id="form1" runat="server">
      
      <input type="hidden" name="DummyAutoCompleteReferenceValue$Boc_HiddenField" value="==null=="/>
      <input type="hidden" name="DummyAutoCompleteReferenceValue$Boc_TextBox" value=""/>
      <input type="hidden" name="DummyBooleanField$Boc_HiddenField" value="null"/>
      <input type="hidden" name="DummyReferenceValue$Boc_DropDownList" value="==null=="/>
      <input type="hidden" name="ScriptManager_HiddenField1" value=";;AjaxControlToolkit, Version=3.5.40412.0, Culture=neutral, PublicKeyToken=28f01b0e84b6d53e:de-AT:1547e793-5b7e-48fe-8490-03a375b13a33:de1feab2:f9cec9bc:a67c2700:f2c8e708:8613aea7:3202a5a2:ab09e3fe:87104b7c:be6fb298"/>
      <input type="hidden" name="SubFilesFormPage_view_LazyContainer$ctl01$ObjectFormPageDataSource_SubFiles_CurrentViewSelector" value="SCCH_d6a096bb-afa6-494b-9ee8-04f2108f0446_Standard for SubFilesFormPage_view_LazyContainer$ctl01$ObjectFormPageDataSource_SubFiles"/>
      <input type="hidden" name="SubFilesFormPage_view_LazyContainer$ctl01$ObjectFormPageDataSource_SubFiles_TextFilterField" value=""/>

      <asp:ScriptManager runat="server" ID="ScriptManager" EnablePartialRendering="True" />
    <div>
    right
      <asp:UpdatePanel runat="server">
        <ContentTemplate>
          <asp:Button runat="server" OnClick="Button_Click"/>          
          <br/>
          <%= DateTime.Now.ToLongTimeString() %>
        </ContentTemplate>
      </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
