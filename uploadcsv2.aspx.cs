//using OfficeOpenXml;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Data.SqlClient;
//using System.Data;
//using System.IO;
//using System.Linq;
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;

//public partial class uploadcsv2 : System.Web.UI.Page
//{
//    protected void btnUpload_Click(object sender, EventArgs e)
//    {

//        if (!FileUpload1.HasFile)
//        {
//            lblMessage.Text = "Please select a CSV or Excel file.";
//            lblMessage.ForeColor = System.Drawing.Color.Red;
//            return;
//        }        


//        string ext = Path.GetExtension(FileUpload1.FileName).ToLower();
//        if (ext != ".csv" && ext != ".xlsx")
//        {
//            lblMessage.Text = "Only CSV or Excel (.xlsx) files are allowed.";
//            lblMessage.ForeColor = System.Drawing.Color.Red;
//            return;
//        }


//        string uploadFolder = Server.MapPath("~/Uploads/");
//        if (!Directory.Exists(uploadFolder))
//            Directory.CreateDirectory(uploadFolder);

//        string filePath = Path.Combine(uploadFolder, Path.GetFileName(FileUpload1.FileName));
//        FileUpload1.SaveAs(filePath);

//        DataTable dt = new DataTable();

//        try
//        {

//            if (ext == ".csv")
//                dt = ReadCsvFile(filePath);
//            else
//                dt = ReadExcelFile(filePath);


//            if (CheckForDuplicates(dt))
//            {
//                lblMessage.Text = "❌ Duplicate qrtext or qrvalue found. Upload stopped.";
//                lblMessage.ForeColor = System.Drawing.Color.Red;
//            }
//            else
//            {

//                InsertIntoDatabase(dt);
//                lblMessage.Text = "✅ File uploaded and data inserted successfully.";
//                lblMessage.ForeColor = System.Drawing.Color.Green;
//            }
//        }     
//        catch (Exception ex)
//        {
//            lblMessage.Text = "❌ Error: " + ex.Message;
//            lblMessage.ForeColor = System.Drawing.Color.Red;
//        }
//        finally
//        {

//            if (File.Exists(filePath))
//                File.Delete(filePath);
//        }
//    }


//    private DataTable ReadCsvFile(string filePath)
//    {
//        DataTable dt = new DataTable();
//        dt.Columns.Add("SerialNo"); 
//        dt.Columns.Add("qrtext");
//        dt.Columns.Add("qrvalue");

//        using (StreamReader sr = new StreamReader(filePath))
//        {
//            sr.ReadLine(); 

//            string line;
//            while ((line = sr.ReadLine()) != null)
//            {
//                string[] values = line.Split(',');

//                if (values.Length >= 3)
//                {
//                    DataRow dr = dt.NewRow();
//                    dr["SerialNo"] = values[0].Trim();

//                    dr["qrtext"] = values[1].Trim().Replace("\"", "");
//                    dr["qrvalue"] = values[2].Trim().Replace("\"", "");

//                    dt.Rows.Add(dr);
//                }
//            }
//        }

//        return dt;
//    }

//    private DataTable ReadExcelFile(string filePath)
//    {
//        DataTable dt = new DataTable();
//        dt.Columns.Add("SerialNo"); 
//        dt.Columns.Add("qrtext");
//        dt.Columns.Add("qrvalue");

//        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

//        using (var pkg = new ExcelPackage(new FileInfo(filePath)))
//        {
//            ExcelWorksheet ws = pkg.Workbook.Worksheets[0];
//            int rows = ws.Dimension.Rows;

//            for (int i = 2; i <= rows; i++) 
//            {
//                DataRow dr = dt.NewRow();

//                dr["SerialNo"] = ws.Cells[i, 1].Text.Trim();

//                dr["qrtext"] = ws.Cells[i, 2].Text.Trim().Replace("\"", "");
//                dr["qrvalue"] = ws.Cells[i, 3].Text.Trim().Replace("\"", "");

//                dt.Rows.Add(dr);
//            }
//        }

//        return dt;
//    }

//    private bool CheckForDuplicates(DataTable dt)
//    {
//        string conn = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

//        HashSet<string> qrTextSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
//        HashSet<string> qrValueSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);


//        foreach (DataRow row in dt.Rows)
//        {
//            string qrtext = row["qrtext"].ToString();
//            string qrvalue = row["qrvalue"].ToString();

//            if (!qrTextSet.Add(qrtext) || !qrValueSet.Add(qrvalue))
//                return true;  
//        }


//        using (SqlConnection con = new SqlConnection(conn))
//        {
//            con.Open();

//            foreach (DataRow row in dt.Rows)
//            {
//                string qrtext = row["qrtext"].ToString();
//                string qrvalue = row["qrvalue"].ToString();

//                string q = "SELECT COUNT(*) FROM qrcodemaster_1 WHERE qrtext=@qrtext OR qrvalue=@qrvalue";

//                using (SqlCommand cmd = new SqlCommand(q, con))
//                {
//                    cmd.Parameters.AddWithValue("@qrtext", qrtext);
//                    cmd.Parameters.AddWithValue("@qrvalue", qrvalue);

//                    if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
//                        return true; 
//                }
//            }
//        }

//        return false;
//    }


//    private void InsertIntoDatabase(DataTable dt)
//    {
//        string conn = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

//        using (SqlConnection con = new SqlConnection(conn))
//        {
//            con.Open();
//            SqlTransaction tran = con.BeginTransaction();

//            try
//            {
//                foreach (DataRow row in dt.Rows)
//                {
//                    string insert = @"INSERT INTO qrcodemaster_1
//                                      (qrvalue, qrtext, qruse, createddate, updatedate)
//                                      VALUES
//                                      (@qrvalue, @qrtext, 'N', GETDATE(), GETDATE())";

//                    using (SqlCommand cmd = new SqlCommand(insert, con, tran))
//                    {
//                        cmd.Parameters.AddWithValue("@qrvalue", row["qrvalue"]);
//                        cmd.Parameters.AddWithValue("@qrtext", row["qrtext"]);
//                        cmd.ExecuteNonQuery();
//                    }
//                }

//                tran.Commit();
//            }
//            catch
//            {
//                tran.Rollback();
//                throw;
//            }
//        }
//    }
//}



using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class uploadcsv2 : System.Web.UI.Page
{
    public class DuplicateInfo
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public string FoundIn { get; set; }
        public string Rows { get; set; }
    }

    protected void btnUpload_Click(object sender, EventArgs e)
    {
        lblMessage.Text = "";
        lblSummary.Text = "";
        gvDuplicates.DataSource = null;
        gvDuplicates.DataBind();

        if (!FileUpload1.HasFile)
        {
            ShowError("Please select a CSV or Excel file.");
            return;
        }

        string ext = Path.GetExtension(FileUpload1.FileName).ToLowerInvariant();

        if (ext != ".csv" && ext != ".xlsx")
        {
            ShowError("Only CSV or Excel (.xlsx) files are allowed.");
            return;
        }

        string uploadFolder = Server.MapPath("~/Uploads/");
        string filePath = Path.Combine(
            uploadFolder, Guid.NewGuid().ToString("N") + ext);

        try
        {
            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            FileUpload1.SaveAs(filePath);

            DataTable dt = ext == ".csv"
                ? ReadCsvFile(filePath)
                : ReadExcelFile(filePath);

            if (dt.Rows.Count == 0)
            {
                ShowError("The uploaded file contains no data rows.");
                return;
            }

            // Find ALL duplicate qrtext and qrvalue values.
            List<DuplicateInfo> duplicates = CheckForDuplicates(dt);

            int qrtextCount = duplicates
                .Where(x => x.Type == "qrtext")
                .Select(x => x.Value)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count();

            int qrvalueCount = duplicates
                .Where(x => x.Type == "qrvalue")
                .Select(x => x.Value)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count();

            if (duplicates.Count > 0)
            {
                lblMessage.Text =
                    "Upload stopped. Please correct the duplicate values and upload again.";

                lblMessage.ForeColor =
                    System.Drawing.Color.Red;

                lblSummary.Text =
                    "Total duplicate qrtext values: " + qrtextCount +
                    "<br />Total duplicate qrvalue values: " + qrvalueCount;

                gvDuplicates.DataSource = duplicates;
                gvDuplicates.DataBind();

                return;
            }

            InsertIntoDatabase(dt);

            lblMessage.Text =
                "File uploaded and data inserted successfully.";

            lblMessage.ForeColor =
                System.Drawing.Color.Green;

            lblSummary.Text =
                "Total records inserted: " + dt.Rows.Count;
        }
        catch (Exception ex)
        {
            ShowError("Error: " + ex.Message);
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    private void ShowError(string message)
    {
        lblMessage.Text = message;
        lblMessage.ForeColor = System.Drawing.Color.Red;
    }

    private DataTable CreateDataTable()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("SerialNo");
        dt.Columns.Add("qrtext");
        dt.Columns.Add("qrvalue");
        return dt;
    }

    private DataTable ReadCsvFile(string filePath)
    {
        DataTable dt = CreateDataTable();

        using (StreamReader sr = new StreamReader(filePath))
        {
            // Skip header.
            sr.ReadLine();

            string line;
            int rowNumber = 1;

            while ((line = sr.ReadLine()) != null)
            {
                rowNumber++;

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // Assumes CSV values do not contain embedded commas.
                string[] values = line.Split(',');

                if (values.Length < 3)
                    continue;

                DataRow dr = dt.NewRow();

                dr["SerialNo"] = values[0].Trim();
                dr["qrtext"] = values[1].Trim().Trim('"');
                dr["qrvalue"] = values[2].Trim().Trim('"');

                dt.Rows.Add(dr);
            }
        }

        return dt;
    }

    private DataTable ReadExcelFile(string filePath)
    {
        DataTable dt = CreateDataTable();

        ExcelPackage.LicenseContext =
            LicenseContext.NonCommercial;

        using (var pkg = new ExcelPackage(new FileInfo(filePath)))
        {
            ExcelWorksheet ws = pkg.Workbook.Worksheets[0];

            if (ws.Dimension == null)
                return dt;

            int rows = ws.Dimension.Rows;

            for (int i = 2; i <= rows; i++)
            {
                DataRow dr = dt.NewRow();

                dr["SerialNo"] = ws.Cells[i, 1].Text.Trim();
                dr["qrtext"] = ws.Cells[i, 2].Text.Trim().Trim('"');
                dr["qrvalue"] = ws.Cells[i, 3].Text.Trim().Trim('"');

                dt.Rows.Add(dr);
            }
        }

        return dt;
    }

    private List<DuplicateInfo> CheckForDuplicates(DataTable dt)
    {
        string connString =
            ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        var duplicates = new List<DuplicateInfo>();

        // Track values within the uploaded file.
        var textRows = new Dictionary<string, List<string>>(
            StringComparer.OrdinalIgnoreCase);

        var valueRows = new Dictionary<string, List<string>>(
            StringComparer.OrdinalIgnoreCase);

        foreach (DataRow row in dt.Rows)
        {
            string serial = row["SerialNo"].ToString().Trim();

            string rowLabel = string.IsNullOrWhiteSpace(serial)
                ? "Data row " + (dt.Rows.IndexOf(row) + 2)
                : serial;

            string text = row["qrtext"].ToString().Trim();
            string value = row["qrvalue"].ToString().Trim();

            AddRow(textRows, text, rowLabel);
            AddRow(valueRows, value, rowLabel);
        }

        // Detect duplicates within the uploaded file.
        foreach (var item in textRows.Where(x => x.Value.Count > 1))
        {
            duplicates.Add(new DuplicateInfo
            {
                Type = "qrtext",
                Value = item.Key,
                FoundIn = "Uploaded file",
                Rows = string.Join(", ", item.Value)
            });
        }

        foreach (var item in valueRows.Where(x => x.Value.Count > 1))
        {
            duplicates.Add(new DuplicateInfo
            {
                Type = "qrvalue",
                Value = item.Key,
                FoundIn = "Uploaded file",
                Rows = string.Join(", ", item.Value)
            });
        }

        // Check all unique values against the database.
        using (SqlConnection con = new SqlConnection(connString))
        {
            con.Open();

            foreach (string text in textRows.Keys)
            {
                if (DatabaseContains(con, "qrtext", text))
                {
                    duplicates.Add(new DuplicateInfo
                    {
                        Type = "qrtext",
                        Value = text,
                        FoundIn = "Database (qrcodemaster_1)",
                        Rows = string.Join(", ", textRows[text])
                    });
                }
            }

            foreach (string value in valueRows.Keys)
            {
                if (DatabaseContains(con, "qrvalue", value))
                {
                    duplicates.Add(new DuplicateInfo
                    {
                        Type = "qrvalue",
                        Value = value,
                        FoundIn = "Database (qrcodemaster_1)",
                        Rows = string.Join(", ", valueRows[value])
                    });
                }
            }
        }

        return duplicates
            .OrderBy(x => x.Type)
            .ThenBy(x => x.Value)
            .ThenBy(x => x.FoundIn)
            .ToList();
    }

    private void AddRow(
        Dictionary<string, List<string>> dictionary,
        string value,
        string rowLabel)
    {
        // Empty values are not counted as duplicates here.
        if (string.IsNullOrWhiteSpace(value))
            return;

        if (!dictionary.ContainsKey(value))
            dictionary[value] = new List<string>();

        dictionary[value].Add(rowLabel);
    }

    private bool DatabaseContains(
        SqlConnection con,
        string columnName,
        string value)
    {
        // Column names come only from the fixed calls above.
        string query = "SELECT CASE WHEN EXISTS " +
            "(SELECT 1 FROM qrcodemaster_1 WHERE " +
            columnName + " = @value) THEN 1 ELSE 0 END";

        using (SqlCommand cmd = new SqlCommand(query, con))
        {
            cmd.Parameters.Add("@value", SqlDbType.VarChar, 100)
                .Value = value;

            return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
        }
    }

    private void InsertIntoDatabase(DataTable dt)
    {
        string connString =
            ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        using (SqlConnection con = new SqlConnection(connString))
        {
            con.Open();

            using (SqlTransaction tran = con.BeginTransaction())
            {
                try
                {
                    string insert = @"
                        INSERT INTO qrcodemaster_1
                            (qrvalue, qrtext, qruse, createddate, updatedate)
                        VALUES
                            (@qrvalue, @qrtext, 'N', GETDATE(), GETDATE())";

                    foreach (DataRow row in dt.Rows)
                    {
                        using (SqlCommand cmd =
                            new SqlCommand(insert, con, tran))
                        {
                            cmd.Parameters.Add("@qrvalue",
                                SqlDbType.VarChar, 100)
                                .Value = row["qrvalue"].ToString().Trim();

                            cmd.Parameters.Add("@qrtext",
                                SqlDbType.VarChar, 100)
                                .Value = row["qrtext"].ToString().Trim();

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
