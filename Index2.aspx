<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Index2.aspx.cs" Inherits="Index2" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>QR Generator</title>
</head>

<body>
    <form runat="server">

        <h2>QR Code Generator</h2>

       
        <asp:Button ID="btnStart" runat="server"
            Text="Generate QR Data"
            OnClick="btnStart_Click" />

        <br /><br />

        
        <asp:Panel ID="pnlDownload" runat="server" Visible="false">

            <h3>Files Generated Successfully ✅</h3>

            <asp:Button ID="btnDownloadCSV" runat="server"
                Text="Download CSV"
                OnClick="btnDownloadCSV_Click" />

            <br /><br />

            <asp:Button ID="btnDownloadExcel" runat="server"
                Text="Download Excel"
                OnClick="btnDownloadExcel_Click" />

        </asp:Panel>

    </form>
</body>
</html>