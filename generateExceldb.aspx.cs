using MySql.Data.MySqlClient;
using OfficeOpenXml;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

public partial class generateExceldb : System.Web.UI.Page
{

    private DataTable GetQRCodeData()
    {
        string connStr =
            ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        DataTable dt = new DataTable();

        using (SqlConnection con = new SqlConnection(connStr))
        {
            string query = @"
            SELECT srno, qrvalue, qrtext, qruse,
                   createddate, updatedate
            FROM qrcodemaster_1
            ORDER BY srno";

            using (SqlDataAdapter da = new SqlDataAdapter(query, con))
            {
                da.Fill(dt);
            }
        }

        return dt;
    }


    protected void btnCSV_Click(object sender, EventArgs e)
    {
        try
        {
            DataTable dt = GetQRCodeData();

            StringBuilder csv = new StringBuilder();

          
            csv.AppendLine(
                "srno,qrvalue,qrtext,qruse,createddate,updatedate");

            foreach (DataRow row in dt.Rows)
            {
                string[] values =
                {
                    FormatValue(row["srno"]),
                    FormatValue(row["qrvalue"]),
                    FormatValue(row["qrtext"]),
                    FormatValue(row["qruse"]),
                    FormatDate(row["createddate"]),
                    FormatDate(row["updatedate"])
                };

                csv.AppendLine(string.Join(",",
                    values.Select(EscapeCsv)));
            }

            byte[] bom = Encoding.UTF8.GetPreamble();
            byte[] data = Encoding.UTF8.GetBytes(csv.ToString());
            byte[] output = bom.Concat(data).ToArray();

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "text/csv";
            Response.ContentEncoding = Encoding.UTF8;
            Response.AddHeader("Content-Disposition",
                "attachment; filename=QRCodeMaster.csv");

            Response.BinaryWrite(output);
            Response.Flush();
            Context.ApplicationInstance.CompleteRequest();
        }
        catch (Exception)
        {
            ShowError("Unable to generate CSV. Please check the database connection.");
        }
    }

    protected void btnExcel_Click(object sender, EventArgs e)
    {
        try
        {
            DataTable dt = GetQRCodeData();

            ExcelPackage.LicenseContext =
                LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet ws =
                    package.Workbook.Worksheets.Add("QRCodeMaster");

              
                for (int col = 0; col < dt.Columns.Count; col++)
                {
                    ws.Cells[1, col + 1].Value =
                        dt.Columns[col].ColumnName;
                }

               
                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    for (int col = 0; col < dt.Columns.Count; col++)
                    {
                        object value = dt.Rows[row][col];

                        if (value == DBNull.Value)
                            continue;

                       
                        if (col == 0 || col == 1 || col == 2 || col == 3)
                        {
                            ws.Cells[row + 2, col + 1].Value =
                                Convert.ToString(value, CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            ws.Cells[row + 2, col + 1].Value = value;
                        }
                    }
                }

               
                ws.Column(5).Style.Numberformat.Format =
                    "yyyy-mm-dd hh:mm:ss";

                ws.Column(6).Style.Numberformat.Format =
                    "yyyy-mm-dd hh:mm:ss";

              
                using (var header = ws.Cells[1, 1, 1, dt.Columns.Count])
                {
                    header.Style.Font.Bold = true;
                    header.Style.Font.Color.SetColor(
                        System.Drawing.Color.White);

                    header.Style.Fill.PatternType =
                        OfficeOpenXml.Style.ExcelFillStyle.Solid;

                    header.Style.Fill.BackgroundColor.SetColor(
                        System.Drawing.Color.DarkGreen);
                }

                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                byte[] fileBytes = package.GetAsByteArray();

                Response.Clear();
                Response.Buffer = true;
                Response.ContentType =
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                Response.AddHeader("Content-Disposition",
                    "attachment; filename=QRCodeMaster.xlsx");

                Response.BinaryWrite(fileBytes);
                Response.Flush();

                Context.ApplicationInstance.CompleteRequest();
            }
        }
        catch (Exception)
        {
            ShowError("Unable to generate Excel. Please check the database connection.");
        }
    }

    private string FormatValue(object value)
    {
        if (value == null || value == DBNull.Value)
            return "";

        return Convert.ToString(value, CultureInfo.InvariantCulture);
    }

    private string FormatDate(object value)
    {
        if (value == null || value == DBNull.Value)
            return "";

        return Convert.ToDateTime(value).ToString(
            "yyyy-MM-dd HH:mm:ss",
            CultureInfo.InvariantCulture);
    }

    private string EscapeCsv(string value)
    {
        if (value == null)
            return "";

      
        return "\"" + value.Replace("\"", "\"\"") + "\"";
    }

    private void ShowError(string message)
    {
        lblMessage.Text = message;
        lblMessage.ForeColor = System.Drawing.Color.Red;
    }
}
