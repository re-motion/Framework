<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OnLoadEventTest.aspx.cs"
  Inherits="Remotion.Web.Test.OnLoadEventTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title></title>
  <script type="text/javascript">
    function Page_OnLoad()
    {
      window.alert('On Load');
    }
  </script>
</head>
<body>
  <form id="form1" runat="server">
  <asp:ScriptManager runat="server" EnablePartialRendering="true" />
  <asp:UpdatePanel runat="server">
    <ContentTemplate>
      <asp:Button ID="AsyncButton" Text="Async PostBack" runat="server" />
    </ContentTemplate>
  </asp:UpdatePanel>
  <asp:Button ID="SyncButton" Text="Sync PostBack" runat="server" />
  </form>
</body>
</html>
