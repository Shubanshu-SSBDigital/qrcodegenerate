<%@ Page Language="C#" AutoEventWireup="true" CodeFile="uploadcsv2.aspx.cs" Inherits="uploadcsv2" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Upload CSV or Excel File</title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="text-align:center; margin-top:50px;">
            <asp:FileUpload ID="FileUpload1" runat="server" />
            <br /><br />
            <asp:Button ID="btnUpload" runat="server" Text="Upload and Insert Data"
                OnClick="btnUpload_Click" 
                style="padding:8px 20px; font-size:16px;" />
            <br /><br />
            <asp:Label ID="lblMessage" runat="server" ForeColor="Green"></asp:Label>
        </div>
    </form>
</body>
</html>
