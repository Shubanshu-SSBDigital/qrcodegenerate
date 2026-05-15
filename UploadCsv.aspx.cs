using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;
using OfficeOpenXml;
using System.Collections.Generic;

public partial class UploadCsv : System.Web.UI.Page
{
    protected void btnUpload_Click(object sender, EventArgs e)
    {
        if (!FileUpload1.HasFile)
        {
            lblMessage.Text = "Please select a CSV or Excel file.";
            lblMessage.ForeColor = System.Drawing.Color.Red;
            return;
        }

        string ext = Path.GetExtension(FileUpload1.FileName).ToLower();
        if (ext != ".csv" && ext != ".xlsx")
        {
            lblMessage.Text = "Only CSV or Excel (.xlsx) files are allowed.";
            lblMessage.ForeColor = System.Drawing.Color.Red;
            return;
        }

        string uploadFolder = Server.MapPath("~/Uploads/");
        if (!Directory.Exists(uploadFolder))
            Directory.CreateDirectory(uploadFolder);

        string filePath = Path.Combine(uploadFolder, Path.GetFileName(FileUpload1.FileName));
        FileUpload1.SaveAs(filePath);

        DataTable dt = new DataTable();
        try
        {
            if (ext == ".csv")
                dt = ReadCsvFile(filePath);
            else if (ext == ".xlsx")
                dt = ReadExcelFile(filePath);


            if (CheckForDuplicates(dt))
            {
                lblMessage.Text = "❌ Duplicate QRText or QRValue found in uploaded file or database. No data inserted.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                InsertIntoDatabase(dt);
                lblMessage.Text = "✅ Data uploaded and inserted successfully.";
                lblMessage.ForeColor = System.Drawing.Color.Green;
            }
        }
        catch (Exception ex)
        {
            lblMessage.Text = "❌ Error: " + ex.Message;
            lblMessage.ForeColor = System.Drawing.Color.Red;
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    private DataTable ReadCsvFile(string filePath)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("SerialNo");
        dt.Columns.Add("QRText");
        dt.Columns.Add("QRValue");

        using (StreamReader sr = new StreamReader(filePath))
        {
            string headerLine = sr.ReadLine();
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] values = line.Split(',');
                if (values.Length >= 3)
                {
                    DataRow dr = dt.NewRow();
                    dr["SerialNo"] = values[0].Trim();
                    dr["QRText"] = values[1].Trim();
                    dr["QRValue"] = values[2].Trim();
                    dt.Rows.Add(dr);
                }
            }
        }
        return dt;
    }


    private DataTable ReadExcelFile(string filePath)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("SerialNo");
        dt.Columns.Add("QRText");
        dt.Columns.Add("QRValue");

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using (var package = new ExcelPackage(new FileInfo(filePath)))
        {
            ExcelWorksheet ws = package.Workbook.Worksheets[0];
            int rows = ws.Dimension.Rows;

            for (int i = 2; i <= rows; i++)
            {
                DataRow dr = dt.NewRow();
                dr["SerialNo"] = ws.Cells[i, 1].Text.Trim();
                dr["QRText"] = ws.Cells[i, 2].Text.Trim();
                dr["QRValue"] = ws.Cells[i, 3].Text.Trim();
                dt.Rows.Add(dr);
            }
        }
        return dt;
    }


    private bool CheckForDuplicates(DataTable dt)
    {
        string connStr = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        HashSet<string> qrTextSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        HashSet<string> qrValueSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);


        foreach (DataRow row in dt.Rows)
        {
            string qrText = row["QRText"].ToString();
            string qrValue = row["QRValue"].ToString();

            if (!qrTextSet.Add(qrText) || !qrValueSet.Add(qrValue))
            {

                return true;
            }
        }


        using (SqlConnection con = new SqlConnection(connStr))
        {
            con.Open();
            foreach (DataRow row in dt.Rows)
            {
                string qrText = row["QRText"].ToString();
                string qrValue = row["QRValue"].ToString();

                string query = "SELECT COUNT(*) FROM QRCodeMaster WHERE QRText = @QRText OR QRValue = @QRValue";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@QRText", qrText);
                    cmd.Parameters.AddWithValue("@QRValue", qrValue);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count > 0)
                    {

                        return true;
                    }
                }
            }
        }

        return false;
    }


    private void InsertIntoDatabase(DataTable dt)
    {
        string connStr = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        using (SqlConnection con = new SqlConnection(connStr))
        {     
            con.Open();
            using (SqlTransaction tran = con.BeginTransaction())
            {
                try
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string query = "INSERT INTO QRCodeMaster (SerialNo, QRText, QRValue) VALUES (@SerialNo, @QRText, @QRValue)";
                        using (SqlCommand cmd = new SqlCommand(query, con, tran))
                        {
                            cmd.Parameters.AddWithValue("@SerialNo", row["SerialNo"]);
                            cmd.Parameters.AddWithValue("@QRText", row["QRText"]);
                            cmd.Parameters.AddWithValue("@QRValue", row["QRValue"]);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
    }
}

                                                        
