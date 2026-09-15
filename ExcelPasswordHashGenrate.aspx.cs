using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using OfficeOpenXml;

public partial class ExcelPasswordHashGenrate : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
       
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    protected void btnGenerateHash_Click(object sender, EventArgs e)
    {
        try
        {

            lblMessage.Text = "";
            lblMessage.CssClass = "message";

         
            if (!fuExcel.HasFile)
            {
                lblMessage.Text = "Please select an Excel file.";
                lblMessage.CssClass = "message error";
                return;
            }

          
            string extension =
                Path.GetExtension(fuExcel.FileName).ToLower();

            if (extension != ".xlsx")
            {
                lblMessage.Text =
                    "Please upload an .xlsx Excel file.";

                lblMessage.CssClass = "message error";
                return;
            }

            using (MemoryStream stream = new MemoryStream())
            {
             
                fuExcel.PostedFile.InputStream.CopyTo(stream);

                stream.Position = 0;

                using (ExcelPackage package =
                       new ExcelPackage(stream))
                {
                    
                    if (package.Workbook.Worksheets.Count == 0)
                    {
                        lblMessage.Text =
                            "Excel file does not contain any worksheet.";

                        lblMessage.CssClass = "message error";
                        return;
                    }


                    ExcelWorksheet worksheet =
                        package.Workbook.Worksheets[0];

                    if (worksheet.Dimension == null)
                    {
                        lblMessage.Text =
                            "Excel worksheet is empty.";

                        lblMessage.CssClass = "message error";
                        return;
                    }


                    int startRow =
                        worksheet.Dimension.Start.Row;

                    int endRow =
                        worksheet.Dimension.End.Row;

                    int startColumn =
                        worksheet.Dimension.Start.Column;

                    int endColumn =
                        worksheet.Dimension.End.Column;

                   
                    int passwordColumn = -1;

                    for (int col = startColumn;
                         col <= endColumn;
                         col++)
                    {
                        string header =
                            worksheet.Cells[
                                startRow,
                                col
                            ].Text.Trim();

                        if (string.Equals(
                            header,
                            "PASSWORD",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            passwordColumn = col;
                            break;
                        }
                    }

                    if (passwordColumn == -1)
                    {
                        lblMessage.Text =
                            "PASSWORD column was not found.";

                        lblMessage.CssClass =
                            "message error";

                        return;
                    }

                    int passwordHashColumn = -1;

                    for (int col = startColumn;
                         col <= endColumn;
                         col++)
                    {
                        string header =
                            worksheet.Cells[
                                startRow,
                                col
                            ].Text.Trim();

                        if (string.Equals(
                            header,
                            "PasswordHash",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            passwordHashColumn = col;
                            break;
                        }
                    }


                    if (passwordHashColumn == -1)
                    {
                        passwordHashColumn = endColumn + 1;

                        worksheet.Cells[
                            startRow,
                            passwordHashColumn
                        ].Value = "PasswordHash";
                    }

                    int hashedCount = 0;


                    for (int row = startRow + 1;
                         row <= endRow;
                         row++)
                    {

                        string password =
                            worksheet.Cells[
                                row,
                                passwordColumn
                            ].Text.Trim();


                        if (string.IsNullOrWhiteSpace(password))
                        {
                            continue;
                        }


                        string hashedPassword =
                            ComputeSha256Hash(password);

                       
                        worksheet.Cells[
                            row,
                            passwordHashColumn
                        ].Value = hashedPassword;

                        hashedCount++;
                    }

                   
                    if (worksheet.Dimension != null)
                    {
                        worksheet.Cells[
                            worksheet.Dimension.Address
                        ].AutoFitColumns();
                    }

                  
                    string outputFileName =
                        Path.GetFileNameWithoutExtension(
                            fuExcel.FileName)
                        + "_PasswordHash.xlsx";

                    
                    byte[] outputBytes =
                        package.GetAsByteArray();


                  
                    Response.Clear();
                    Response.ClearHeaders();
                    Response.ClearContent();

                    Response.ContentType =
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    Response.AddHeader(
                        "Content-Disposition",
                        "attachment; filename=\"" +
                        outputFileName +
                        "\"");

                    Response.AddHeader(
                        "Content-Length",
                        outputBytes.Length.ToString());

                    Response.BinaryWrite(outputBytes);

                    Response.Flush();

                    HttpContext.Current
                        .ApplicationInstance
                        .CompleteRequest();
                }
            }
        }
        catch (Exception ex)
        {
            lblMessage.Text =
                "Error: " + ex.Message;

            lblMessage.CssClass =
                "message error";
        }
    }


    public string ComputeSha256Hash(string rawData)
    {
        using (SHA256 sha256Hash =
               SHA256.Create())
        {
            byte[] bytes =
                sha256Hash.ComputeHash(
                    Encoding.UTF8.GetBytes(rawData));

            StringBuilder builder =
                new StringBuilder();

            foreach (byte b in bytes)
            {
                builder.Append(
                    b.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}
