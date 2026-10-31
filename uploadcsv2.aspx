<%--<%@ Page Language="C#" AutoEventWireup="true" CodeFile="uploadcsv2.aspx.cs" Inherits="uploadcsv2" %>

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
</html>--%>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="uploadcsv2.aspx.cs" Inherits="uploadcsv2" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Upload CSV or Excel File</title>
    <style>
        body {
            font-family: Arial, sans-serif;
        }

        .upload-container {
            max-width: 1000px;
            margin: 40px auto;
            padding: 20px;
        }

        .message {
            display: block;
            margin: 15px 0;
            font-weight: bold;
        }

        .duplicate-grid {
            width: 100%;
            border-collapse: collapse;
            margin-top: 15px;
        }

        .duplicate-grid th {
            background-color: #b02a37;
            color: white;
            padding: 10px;
        }

        .duplicate-grid td {
            padding: 8px;
            border: 1px solid #ddd;
            overflow-wrap: anywhere;
        }

        .summary {
            margin-top: 15px;
            font-weight: bold;
            line-height: 1.8;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="upload-container">

            <h2>Upload QR Code CSV / Excel File</h2>

            <asp:FileUpload ID="FileUpload1" runat="server" />

            <br /><br />

            <asp:Button ID="btnUpload" runat="server"
                Text="Upload and Insert Data"
                OnClick="btnUpload_Click"
                style="padding:8px 20px; font-size:16px;" />

            <asp:Label ID="lblMessage" runat="server"
                CssClass="message" />

            <asp:Label ID="lblSummary" runat="server"
                CssClass="summary" />

            <asp:GridView ID="gvDuplicates" runat="server"
                AutoGenerateColumns="False"
                CssClass="duplicate-grid"
                GridLines="Both"
                EmptyDataText="No duplicate records found.">

                <Columns>
                    <asp:BoundField DataField="Type"
                        HeaderText="Duplicate Type" />

                    <asp:BoundField DataField="Value"
                        HeaderText="Duplicate Value" />

                    <asp:BoundField DataField="FoundIn"
                        HeaderText="Found In" />

                    <asp:BoundField DataField="Rows"
                        HeaderText="CSV / Excel Row(s)" />
                </Columns>
            </asp:GridView>

        </div>
    </form>
</body>
</html>
