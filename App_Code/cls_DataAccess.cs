using System;
using System.Data;
using System.Data.SqlClient;

public class cls_DataAccess
{
    public SqlConnection cnnObject;
    public string _SerucityCode = "";
    private SqlCommand cmd;
    private SqlTransaction tran;
    private string _ConnectionString;
    public event EventHandler ConnectionOpen;
    public cls_DataAccess(string strConnectionString)
    {
        _ConnectionString = strConnectionString;
    }

    public string ClearInject(string StrObj)
    {
        StrObj = StrObj.Replace(";", "").Replace("'", "").Replace("=", "");
        return StrObj;
    }

    public SqlConnection OpenConnection()
    {
        try
        {
            if (cnnObject == null)
            {
                cnnObject = new SqlConnection(_ConnectionString);
            }
            if (cnnObject.State == ConnectionState.Closed || cnnObject.State == ConnectionState.Broken)
            {
                cnnObject.Open();

                //ConnectionOpen?.Invoke();
            }
            return cnnObject;
        }
        catch (Exception)
        {
            //ConnectionOpen?.Invoke();
            return null;
        }
    }

    public void CloseConnection()
    {
        try
        {
            if (cnnObject.State == ConnectionState.Open)
            {
                cnnObject.Close();
            }
        }
        catch (Exception)
        {
        }
    }

    public string ExecuteScaller_old(string strQuery)
    {
        if (cnnObject == null || cnnObject.State == ConnectionState.Closed)
        {
            OpenConnection();
        }

        cmd = new SqlCommand();
        try
        {
            cmd.Connection = cnnObject;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = strQuery;

            object result = cmd.ExecuteScalar();
            return result != null ? result.ToString() : "";

        }
        catch (Exception)
        {
            return "";
        }
    }
    public string ExistOrNot(string strQuery)
    {
        string _returnValue = "";

        if (cnnObject == null || cnnObject.State == ConnectionState.Closed)
        {
            OpenConnection();
        }

        using (SqlDataAdapter da = new SqlDataAdapter(strQuery, cnnObject))
        {
            DataTable DTable = new DataTable();

            try
            {
                da.Fill(DTable);

                if (DTable.Rows.Count > 0 && DTable.Rows[0][0] != DBNull.Value)
                {
                    _returnValue = DTable.Rows[0][0].ToString();
                }
            }
            catch (Exception)
            {
            }
        }
        return _returnValue;
    }

    // Implement other methods similarly
    public void Fill_DataSET_Tables(string strQuery, ref DataSet DS, string strTableName)
    {
        if (cnnObject == null)
        {
            OpenConnection();
        }

        using (SqlDataAdapter da = new SqlDataAdapter(strQuery, cnnObject))
        {
            try
            {
                da.Fill(DS, strTableName);
            }
            catch (Exception)
            {
                // Handle the exception or log it
            }
        }
    }

    public DateTime Get_ServerDate()
    {
        string SqlD = "Select Convert(Varchar,Getdate(),106) as Dts";
        DataSet SqlDs = new DataSet();

        Fill_DataSET_Tables(SqlD, ref SqlDs, "MyDate");

        if (cnnObject == null || cnnObject.State == ConnectionState.Closed)
        {
            OpenConnection();
        }

        if (SqlDs.Tables["MyDate"].Rows.Count > 0)
        {
            return Convert.ToDateTime(SqlDs.Tables["MyDate"].Rows[0]["Dts"]);
        }
        else
        {
            return DateTime.Now;
        }
    }

    public void closeConnection()
    {
        throw new NotImplementedException();
    }

    public void Fill_Data_Tables(string strQuery, DataTable tmpTable)
    {
        throw new NotImplementedException();
    }

    // Implement other methods similarly
}