using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class uploadcsv2 : System.Web.UI.Page
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
            else
                dt = ReadExcelFile(filePath);

           
            if (CheckForDuplicates(dt))
            {
                lblMessage.Text = "❌ Duplicate qrtext or qrvalue found. Upload stopped.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
           
                InsertIntoDatabase(dt);
                lblMessage.Text = "✅ File uploaded and data inserted successfully.";
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
        dt.Columns.Add("qrtext");
        dt.Columns.Add("qrvalue");

        using (StreamReader sr = new StreamReader(filePath))
        {
            sr.ReadLine(); 

            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] values = line.Split(',');

                if (values.Length >= 3)
                {
                    DataRow dr = dt.NewRow();
                    dr["SerialNo"] = values[0].Trim();
                 
                    dr["qrtext"] = values[1].Trim().Replace("\"", "");
                    dr["qrvalue"] = values[2].Trim().Replace("\"", "");

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
        dt.Columns.Add("qrtext");
        dt.Columns.Add("qrvalue");

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using (var pkg = new ExcelPackage(new FileInfo(filePath)))
        {
            ExcelWorksheet ws = pkg.Workbook.Worksheets[0];
            int rows = ws.Dimension.Rows;

            for (int i = 2; i <= rows; i++) 
            {
                DataRow dr = dt.NewRow();

                dr["SerialNo"] = ws.Cells[i, 1].Text.Trim();
               
                dr["qrtext"] = ws.Cells[i, 2].Text.Trim().Replace("\"", "");
                dr["qrvalue"] = ws.Cells[i, 3].Text.Trim().Replace("\"", "");

                dt.Rows.Add(dr);
            }
        }

        return dt;
    }

    private bool CheckForDuplicates(DataTable dt)
    {
        string conn = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        HashSet<string> qrTextSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        HashSet<string> qrValueSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

      
        foreach (DataRow row in dt.Rows)
        {
            string qrtext = row["qrtext"].ToString();
            string qrvalue = row["qrvalue"].ToString();

            if (!qrTextSet.Add(qrtext) || !qrValueSet.Add(qrvalue))
                return true;  
        }

      
        using (SqlConnection con = new SqlConnection(conn))
        {
            con.Open();

            foreach (DataRow row in dt.Rows)
            {
                string qrtext = row["qrtext"].ToString();
                string qrvalue = row["qrvalue"].ToString();

                string q = "SELECT COUNT(*) FROM qrcodemaster_1 WHERE qrtext=@qrtext OR qrvalue=@qrvalue";

                using (SqlCommand cmd = new SqlCommand(q, con))
                {
                    cmd.Parameters.AddWithValue("@qrtext", qrtext);
                    cmd.Parameters.AddWithValue("@qrvalue", qrvalue);

                    if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                        return true; 
                }
            }
        }

        return false;
    }

   
    private void InsertIntoDatabase(DataTable dt)
    {
        string conn = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        using (SqlConnection con = new SqlConnection(conn))
        {
            con.Open();
            SqlTransaction tran = con.BeginTransaction();

            try
            {
                foreach (DataRow row in dt.Rows)
                {
                    string insert = @"INSERT INTO qrcodemaster_1
                                      (qrvalue, qrtext, qruse, createddate, updatedate)
                                      VALUES
                                      (@qrvalue, @qrtext, 'N', GETDATE(), GETDATE())";

                    using (SqlCommand cmd = new SqlCommand(insert, con, tran))
                    {
                        cmd.Parameters.AddWithValue("@qrvalue", row["qrvalue"]);
                        cmd.Parameters.AddWithValue("@qrtext", row["qrtext"]);
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
