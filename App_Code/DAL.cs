using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;


/// <summary>
/// Summary description for DAL
/// </summary>
public class DAL
{
    public SqlConnection objSQlConnection;

    public SqlConnection objSQlConnection1;

    public SqlDataAdapter sda = new SqlDataAdapter();
    public DataTable dt = new DataTable();
    public HttpRequest request;
    public string AppUrl = "";
    public SqlConnectionStringBuilder Connection = new SqlConnectionStringBuilder();
    public SqlConnectionStringBuilder Connection1 = new SqlConnectionStringBuilder();
    public SqlCommand sqlCmd = new SqlCommand();
    public string activeCondition = "RowStatus='Y'";
    public string tblUserGrpMaster = "M_UserGroupMaster";
    public string tblUserMaster = "M_UserMaster";
    public string tblsalesMaster = "Salesperson";
    public string tblclientMaster = "Client";
    public string Isostart = " Alter Database brandfactory Set Allow_SnapShot_isolation on; Set Transaction Isolation level read uncommitted;Set nocount on; Begin Tran  ";
    public string IsoEnd = " Commit Tran ";
    public string dBName = "brandfactory";
    
    public DAL()
    {
        // Setting Connection String
        // Connection.ConnectionString = "Server=164.132.18.172;UID=ubgshp;PWD=S#09B!81ee$;Database=BigShopee;Pooling=False;Connect Timeout=1500000000"
        Connection.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        objSQlConnection = new SqlConnection(Connection.ConnectionString);

        Connection1.ConnectionString = ConfigurationManager.ConnectionStrings["constr1"].ConnectionString;
        objSQlConnection1 = new SqlConnection(Connection1.ConnectionString);
    }

    public int SaveData(string qry)
    {
        int a = 0;
        try
        {


            // objSQlConnection.Open()
            if (objSQlConnection.State == ConnectionState.Closed)
                objSQlConnection.Open();
            sqlCmd = new SqlCommand(qry, objSQlConnection);
            a = sqlCmd.ExecuteNonQuery();
            // objSQlConnection.Close()
            if (objSQlConnection.State == ConnectionState.Open)
                objSQlConnection.Close();

        }
        catch (Exception ex)
        {
            if (!(objSQlConnection == null))
            {
                if (objSQlConnection.State == ConnectionState.Open)
                    objSQlConnection.Close();
            }
        }
        return a;
    }
    public int SaveData1(string qry)
    {
        int a = 0;
        try
        {
            // objSQlConnection.Open()
            if (objSQlConnection.State == ConnectionState.Closed)
                objSQlConnection.Open();
            sqlCmd = new SqlCommand(qry, objSQlConnection);
            sqlCmd.CommandTimeout = 0;
            a = sqlCmd.ExecuteNonQuery();
            // objSQlConnection.Close()
            if (objSQlConnection.State == ConnectionState.Open)
                objSQlConnection.Close();
            return a;
        }
        catch (Exception ex)
        {
            if (!(objSQlConnection == null))
            {
                if (objSQlConnection.State == ConnectionState.Open)
                    objSQlConnection.Close();
            }
        }
        return a;
    }

    public DataTable GetData(string qry)
    {
        DataTable tempDt = new DataTable();
        try
        {
            if (objSQlConnection.State == ConnectionState.Closed)
                objSQlConnection.Open();
            sda = new SqlDataAdapter(qry, objSQlConnection);
            dt = new DataTable();
            sda.Fill(dt);
            tempDt = dt;
        }
        catch (Exception ex)
        {
            if (!(objSQlConnection == null))
            {
                if (objSQlConnection.State == ConnectionState.Open)
                    objSQlConnection.Close();
            }
        }
        return tempDt;
    }

    public DataTable GetDataWithIso(string qry)
    {
        DataTable tempDt = new DataTable();
        try
        {
            if (objSQlConnection1.State == ConnectionState.Closed)
                objSQlConnection1.Open();

            qry = Isostart + qry + IsoEnd;

            sda = new SqlDataAdapter(qry, objSQlConnection1);
            dt = new DataTable();
            sda.Fill(dt);
            sda.Fill(dt);
            tempDt = dt;
        }
        catch (Exception ex)
        {
            if (!(objSQlConnection1 == null))
            {
                if (objSQlConnection1.State == ConnectionState.Open)
                    objSQlConnection1.Close();
            }
        }
        return tempDt;
    }


    public int UpdateData(string qry, string ParaName = "", string ParaValue = "")
    {
        // objSQlConnection.Open()
        int j = 0;
        try
        {
            if (objSQlConnection.State == ConnectionState.Closed)
                objSQlConnection.Open();
            string[] strParaName, strParaValue;
            int i = 0;

            if (string.IsNullOrEmpty(ParaName) && string.IsNullOrEmpty(ParaValue))
            {
                sqlCmd = new SqlCommand(qry, objSQlConnection);
            }
            else
            {
                strParaName = ParaName.Split(';');
                strParaValue = ParaValue.Split(';');
                sqlCmd = new SqlCommand(qry, objSQlConnection);
                for (i = 0; i < strParaName.Length; i++)
                {
                    sqlCmd.Parameters.AddWithValue(strParaName[i], strParaValue[i]);
                }
            }

            int a = sqlCmd.ExecuteNonQuery();
            // objSQlConnection.Close()
            if (objSQlConnection.State == ConnectionState.Open)
                objSQlConnection.Close();
            j = a;
        }
        catch (Exception ex)
        {
            if (!(objSQlConnection == null))
            {
                if (objSQlConnection.State == ConnectionState.Open)
                    objSQlConnection.Close();
            }
        }
        return j;
    }
    public int ExecuteProcedure(string procname, string ParaName = "", string ParaValue = "")
    {
        int j = 0;
        try
        {
            SqlCommand cmd = objSQlConnection.CreateCommand();
            string[] strParaName, strParaValue;
            if (objSQlConnection.State == ConnectionState.Closed)
                objSQlConnection.Open();
            // objSQlConnection.Open()
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(ParaName) && !string.IsNullOrEmpty(ParaValue))
            {
                strParaName = ParaName.Split(';');
                strParaValue = ParaValue.Split(';');
                for (int i = 0; i < strParaName.Length; i++)
                {
                    cmd.Parameters.AddWithValue(strParaName[i], strParaValue[i]);
                }
            }
            cmd.CommandText = procname;
            int a = cmd.ExecuteNonQuery();
            // objSQlConnection.Close()
            if (objSQlConnection.State == ConnectionState.Open)
                objSQlConnection.Close();
            j = a;
        }
        catch (Exception ex)
        {
            if (!(objSQlConnection == null))
            {
                if (objSQlConnection.State == ConnectionState.Open)
                    objSQlConnection.Close();
            }
        }
        return j;
    }

    public DataTable GenerateTreeProc(string strqry)
    {

        DataTable tempDt = new DataTable();
        try
        {
            if (objSQlConnection.State == ConnectionState.Closed)
                objSQlConnection.Open();
            SqlCommand comm = new SqlCommand(strqry, objSQlConnection);
            comm.CommandTimeout = 100000000;
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            dt = new DataTable();
            sda.Fill(dt);
            if (objSQlConnection.State == ConnectionState.Open)
                objSQlConnection.Close();
            tempDt = dt;
        }
        catch (Exception ex)
        {
            if (!(objSQlConnection == null))
            {
                if (objSQlConnection.State == ConnectionState.Open)
                    objSQlConnection.Close();
            }
        }
        return tempDt;
    }

    public DataSet ExecProcDataSet(string strqry)
    {
        DataSet dsGetData = new DataSet();
        try
        {
            if (objSQlConnection .State == ConnectionState.Closed)
                objSQlConnection.Open();

            var comm = new SqlCommand(strqry, objSQlConnection);
            comm.CommandTimeout = 100000000;
            var sda = new SqlDataAdapter(comm);
            sda.Fill(dsGetData);

            if (objSQlConnection.State == ConnectionState.Open)
                objSQlConnection.Close();
        }
        catch (Exception ex)
        {
            if (objSQlConnection != null && objSQlConnection.State == ConnectionState.Open)
                objSQlConnection.Close();

            throw;
        }
        return dsGetData;
    }

    public int UpdateMultipleFields(string tblName, string ParaName = "", string ParaValue = "", string whereCond = "")
    {

        int j = 0;
        try
        {
            string SubPart = "";
            if (objSQlConnection.State == ConnectionState.Closed)
                objSQlConnection.Open();
            string[] strParaName, strParaValue;
            if (!string.IsNullOrEmpty(ParaName) && !string.IsNullOrEmpty(ParaValue))
            {
                strParaName = ParaName.Split(';');
                strParaValue = ParaValue.Split(';');
                for (int i = 0; i < strParaName.Length; i++)
                {
                    SubPart = SubPart + strParaName[i] + "='" + strParaValue[i] + "',";
                }
            }
            if (!string.IsNullOrEmpty(SubPart))
                SubPart = SubPart.Remove(SubPart.Length - 1, 1);

            string qry = " update " + tblName + " set " + SubPart + whereCond;
            sqlCmd = new SqlCommand(qry, objSQlConnection);
            int a = sqlCmd.ExecuteNonQuery();
            // Dim a As Integer = sqlCmd.ExecuteNonQuery()
            // objSQlConnection.Close()
            if (objSQlConnection.State == ConnectionState.Open)
                objSQlConnection.Close();
            j = a;
        }
        catch (Exception ex)
        {
            if (!(objSQlConnection == null))
            {
                if (objSQlConnection.State == ConnectionState.Open)
                    objSQlConnection.Close();
            }
        }
        return j;
    }
    public string ClearInject(string StrObj)
    {
        try
        {
            //StrObj = Strings.Replace(Strings.Replace(Strings.Replace(StrObj, ";", ""), "'", ""), "=", "");
            StrObj = StrObj.Trim().Replace(";", "").Replace("'", "").Replace("=", "");
        }
        catch (Exception ex)
        {
        }
        return StrObj;
    }

    public void FillCombo(string qry, ref DropDownList CmbNm, string DisFld, string ValFld)
    {
        try
        {
            dt = new DataTable();
            dt = GetData(qry);
            {
                var withBlock = CmbNm;
                withBlock.DataSource = dt;
                withBlock.DataTextField = DisFld;
                withBlock.DataValueField = ValFld;
                withBlock.DataBind();
            }
        }
        catch (Exception ex)
        {
        }
    }
    public void WriteToFile(string text)
    {
        try
        {
            string path = HttpContext.Current.Server.MapPath("~/images/ErrorLog.txt");
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(text);
                writer.WriteLine("--------------------------------------------------------");
                writer.Close();
            }
        }
        catch (Exception ex)
        {
        }
    }
}
