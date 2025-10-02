using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Xml;
using System.Net;
using System.Text;
using DocumentFormat.OpenXml.Presentation;
using System.IdentityModel.Protocols.WSTrust;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text.RegularExpressions;
using AjaxControlToolkit.HtmlEditor.ToolbarButtons;
using System.Security.Policy;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Security.Cryptography;
using Microsoft.Ajax.Utilities;
using Microsoft.SqlServer.Server;
using AjaxControlToolkit;
using System.Activities.Expressions;
using Irony.Parsing;
using static Microsoft.FSharp.Core.ByRefKinds;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Formatting = Newtonsoft.Json.Formatting;
public partial class ProcessApiWithK : System.Web.UI.Page
{
    public string _ReqType;
    public GetMsg23 ErrObj = new GetMsg23();
    SqlConnection Conn;
    SqlCommand Comm;
    SqlDataAdapter Adp;
    DataSet Ds;
    SqlDataReader Dr;
    string _NewID = DateTime.Now.ToString("yyyyMMddHHmmssfff");
    Random Rnd = new Random();
    bool Bool = true;
    string HostIp = HttpContext.Current.Request.UserHostAddress.ToString();
    string _Company = "";
    string _Logo = "";
    string strQry = "";
    string _MailID = "";
    string _MailPass = "";
    string _MailHost = "";
    string _SMSSender = "APPSMS";
    string _SMSUser = "";
    string _SMSPass = "";
    string _RefFormNo = "";
    string _UpLnFormNo = "";
    string _Token = "GW739IESP1956rerir";
    string _Tokenlogin = "JaiGW739IESPrerirDarbar";
    string membername = "";
    clsGeneral objGen = new clsGeneral();
    string KeyE = "6b04d38748f94490a636cf1be3d82841";
    string IV = "f8adbf3c94b7463d";
    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
    string constr1 = ConfigurationManager.ConnectionStrings["constr1"].ConnectionString;
    DAL ObjDal = new DAL();
    string IsoStart;
    string IsoEnd;
    string sResult = string.Empty;
    string ipAddress = "";
    string userAgent = "";
    protected void Page_Load(object sender, EventArgs e)
    {

        try
        {
            Session["CompID"] = "100009";
            Request.InputStream.Position = 0;
            using (var inputStream = new StreamReader(Request.InputStream))
            {
                string json = inputStream.ReadToEnd();
                try
                {
                    string current_datetime = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    int random_number = new Random().Next(0, 999);
                    string formatted_datetime = current_datetime + random_number.ToString().PadLeft(3, '0');
                    sResult = formatted_datetime;
                    ipAddress = Context.Request.UserHostAddress.ToString();
                    if (HttpContext.Current != null && HttpContext.Current.Request != null && HttpContext.Current.Request.UserAgent != null)
                    {
                        userAgent = HttpContext.Current.Request.UserAgent.ToUpper();
                    }
                    string URL = "https://" + HttpContext.Current.Request.Url.Host + "/ProcessApiWithK.aspx";
                    string sql_req = "INSERT INTO Tbl_ApiRequest_ResponseQrCodeutility (ReqID,Request,postdata,HostIP,HostType) ";
                    sql_req += "VALUES ('" + sResult.Trim() + "','" + URL.Trim() + "', '" + json.Replace("//n", "\\n") + "','" + ipAddress + "','" + userAgent + "')";
                    int x_Req = Convert.ToInt32(SqlHelper.ExecuteNonQuery(constr, CommandType.Text, sql_req));
                    if (HttpContext.Current.Request.HttpMethod.ToUpper() != "POST")
                    {
                        BlockRequest("Only POST requests allowed.", sResult);
                        return;
                    }
                }
                catch (Exception) { }

if (!string.IsNullOrEmpty(json))
{
    try
    {
                        //// ---- Strongly typed model ----
                        OrderRequest request = JsonConvert.DeserializeObject<OrderRequest>(json);
                        OrderRequest order = JsonConvert.DeserializeObject<OrderRequest>(json);

                        if (order != null && !string.IsNullOrEmpty(order.reqtype))
                        {
                            if (order.reqtype.ToLower() == "createorder")
                            {
                                if (order.items != null && order.items.Count > 0)
                                {
                                    // Generate unique OrderId
                                    long orderId = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));

                                    using (SqlConnection conn = new SqlConnection(constr))
                                    {
                                        conn.Open();

                                        // Step 2: Insert into Tbl_Orders (Master)
                                        string insertOrderSql = @"INSERT INTO Tbl_Orders (OrderId, Client, Category, GrandTotal,userid) 
                                              VALUES (@OrderId, @Client, @Category, @GrandTotal,@userid)";

                                        using (SqlCommand cmd = new SqlCommand(insertOrderSql, conn))
                                        {
                                            cmd.Parameters.AddWithValue("@OrderId", orderId);
                                            cmd.Parameters.AddWithValue("@Client", order.client);
                                            cmd.Parameters.AddWithValue("@Category", order.category);
                                            cmd.Parameters.AddWithValue("@GrandTotal", Convert.ToDecimal(order.grandTotal));
                                            cmd.Parameters.AddWithValue("@userid", Convert.ToDecimal(order.userid));
                                            cmd.ExecuteNonQuery();
                                        }

                                        // Step 3: Prepare DataTable for Bulk Insert in Tbl_OrderItems
                                        DataTable dt = new DataTable();
                                        dt.Columns.Add("OrderId", typeof(long));
                                        dt.Columns.Add("ProductId", typeof(int));   // ✅ नया column
                                        dt.Columns.Add("ProductName", typeof(string));
                                        dt.Columns.Add("Quantity", typeof(int));
                                        dt.Columns.Add("Rate", typeof(decimal));
                                        dt.Columns.Add("TotalAmount", typeof(decimal));

                                        foreach (var item in order.items)
                                        {
                                            if (item == null) continue; // 🚫 null skip

                                            if (item.ProductId == 0 || string.IsNullOrEmpty(item.ProductName))
                                                continue; // 🚫 invalid rows skip

                                            DataRow dr = dt.NewRow();
                                            dr["OrderId"] = orderId;
                                            dr["ProductId"] = item.ProductId;
                                            dr["ProductName"] = item.ProductName;
                                            dr["Quantity"] = Convert.ToInt32(item.Quantity);
                                            dr["Rate"] = Convert.ToDecimal(item.Rate);
                                            dr["TotalAmount"] = Convert.ToDecimal(item.TotalAmount);
                                            dt.Rows.Add(dr); // ✅ सिर्फ़ valid rows
                                            // साथ ही Product table update
                                    //        string updateProductSql = @"UPDATE Product 
                                    //SET StockQty =@Qty, 
                                    //Price = @Price
                                    //WHERE ProductId = @ProductId";
                                    //        using (SqlCommand cmd = new SqlCommand(updateProductSql, conn))
                                    //        {
                                    //            cmd.Parameters.AddWithValue("@Qty", Convert.ToInt32(item.Quantity));
                                    //            cmd.Parameters.AddWithValue("@Price", Convert.ToDecimal(item.Rate));
                                    //            cmd.Parameters.AddWithValue("@ProductId", item.ProductId); // ⚠️ ProductId JSON से भी भेजना पड़ेगा
                                    //            cmd.ExecuteNonQuery();
                                    //        }
                                        }

                                        // Step 4: Bulk Insert Items
                                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                                        {
                                            bulkCopy.DestinationTableName = "Tbl_OrderItems";
                                            bulkCopy.ColumnMappings.Add("OrderId", "OrderId");
                                            bulkCopy.ColumnMappings.Add("ProductId", "ProductId");
                                            bulkCopy.ColumnMappings.Add("ProductName", "ProductName");
                                            bulkCopy.ColumnMappings.Add("Quantity", "Quantity");
                                            bulkCopy.ColumnMappings.Add("Rate", "Rate");
                                            bulkCopy.ColumnMappings.Add("TotalAmount", "TotalAmount");
                                            bulkCopy.WriteToServer(dt);
                                        }
                                    }

                                    // Step 5: Send Success Response
                                    string Result_Json = "{\"response\":\"SUCCESS\",\"orderId\":\"" + orderId + "\"}";
                                    Response.Clear();
                                    Response.ContentType = "application/json";
                                    Response.Write(Result_Json);
                                    }
                                else
                                {
                                    // Invalid order (no items)
                                    Response.Write("{\"response\":\"FAILED\",\"message\":\"No items found\"}");
                                }
                            }
                            else
                            {
                                //// अगर reqtype createorder नहीं है
                                //Response.Write("{\"response\":\"FAILED\",\"message\":\"Invalid reqtype\"}");
                                //    //var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                                Process(request.reqtype, dict);
                            }
                        }
                    }
    catch (Exception ex)
    {
        string Result_Json = "{\"response\":\"FAILED\",\"error\":\"" + ex.Message + "\"}";
        string sql_res = "UPDATE Tbl_ApiRequest_ResponseQrCodeutility SET Response = '" + Result_Json.Trim() + "' WHERE ReqID = '" + sResult.Trim() + "'";
        int x_res = ObjDal.SaveData(sql_res);

        Response.Clear();
        Response.ContentType = "application/json";
        Response.Write(Result_Json);
    }
}
            }
        }
        catch (Exception)
        {
            string Result_Json = "";
            Result_Json = "{\"response\":\"FAILED\"}";
            string sql_res = "UPDATE Tbl_ApiRequest_ResponseQrCodeutility SET Response = '" + Result_Json.Trim() + "' WHERE ReqID = '" + sResult.Trim() + "'";
            int x_res = ObjDal.SaveData(sql_res);
            Result_Json = Result_Json.Replace(Environment.NewLine, "").Replace("\n", "").Replace("\r", "");
            Response.Clear();
            Response.ContentType = "application/json";
            Response.Write(Result_Json);
        }

        Response.End();
    }
   

    //private void Process(string reqtype, Dictionary<string, object> dict)
    //{
    //    throw new NotImplementedException();
    //}

    public void BlockRequest(string msg, string reqId)
    {
        string resultJson = $"{{\"response\":\"FAILED\",\"msg\":\"{msg}\"}}";
        string sql_res = "UPDATE Tbl_ApiRequest_ResponseQrCodeutility SET Response = '" + resultJson.Trim() + "',ReqType = 'reqlogin' WHERE ReqID = '" + reqId.Trim() + "'";
        int x_res = ObjDal.SaveData(sql_res);
        resultJson = resultJson.Replace(Environment.NewLine, "").Replace("\n", "").Replace("\r", "");
        Response.Clear();
        Response.ContentType = "application/json";
        Response.Write(resultJson);
    }
    public void Process(string _Reqtype, Dictionary<string, string> dict)
    {
        try
        {
            if (_Reqtype == "reqlogin")
            {
                string _ReqUser = ClearInject(dict["username"]);
                string _Reqpass = ClearInject(dict["password"]);
                string Result_Json = CheckLogin(_ReqUser, _Reqpass);
                //string Result_Json = "{\"response\":\"FAILED\",\"msg\":\"Invalid Login Details.\"}";
                string sql_res = "UPDATE Tbl_ApiRequest_ResponseQrCodeutility SET Response = '" + Result_Json.Trim() + "',ReqType = 'reqlogin' WHERE ReqID = '" + sResult.Trim() + "'";
                int x_res = ObjDal.SaveData(sql_res);
                Result_Json = Result_Json.Replace(Environment.NewLine, "").Replace("\n", "").Replace("\r", "");
                Response.Clear();
                Response.ContentType = "application/json";
                Response.Write(Result_Json);
                return;

            }
            else if (_Reqtype == "addgroup")
            {

                string _Reqsusername = ClearInject(dict["groupname"]);
                string Result_Json = addgroup(_Reqsusername);
                string sql_res = "UPDATE Tbl_ApiRequest_ResponseQrCodeutility SET Response = '" + Result_Json.Trim() + "',ReqType = 'addgroup' WHERE ReqID = '" + sResult.Trim() + "'";
                int x_res = ObjDal.SaveData(sql_res);
                Result_Json = Result_Json.Replace(Environment.NewLine, "").Replace("\n", "").Replace("\r", "");
                Response.Clear();
                Response.ContentType = "application/json";
                Response.Write(Result_Json);
            }
            else if (_Reqtype == "grouplist")
            {

                string Result_Json = FUN_groupname();
                string sql_res = "UPDATE Tbl_ApiRequest_ResponseQrCodeutility SET Response = '" + Result_Json.Trim() + "',ReqType = 'grouplist' WHERE ReqID = '" + sResult.Trim() + "'";
                int x_res = ObjDal.SaveData(sql_res);
                Result_Json = Result_Json.Replace(Environment.NewLine, "").Replace("\n", "").Replace("\r", "");
                Response.Clear();
                Response.ContentType = "application/json";
                Response.Write(Result_Json);
            }
            else if (_Reqtype == "salesregistration")
            {
                string _Reqsuserid = ClearInject(dict["userid"]);
                string _Reqsusername = ClearInject(dict["username"]);
                string _Reqtpassw = ClearInject(dict["password"]);
                string _Reqmobile = ClearInject(dict["mobileno"]);
                //string _Reqstatus = ClearInject(dict["status"]);
                string Result_Json = FUN_registration(_Reqsuserid, _Reqsusername, _Reqtpassw, _Reqmobile);
                string sql_res = "UPDATE Tbl_ApiRequest_ResponseQrCodeutility SET Response = '" + Result_Json.Trim() + "',ReqType = 'salesregistration' WHERE ReqID = '" + sResult.Trim() + "'";
                int x_res = ObjDal.SaveData(sql_res);
                Result_Json = Result_Json.Replace(Environment.NewLine, "").Replace("\n", "").Replace("\r", "");
                Response.Clear();
                Response.ContentType = "application/json";
                Response.Write(Result_Json);
            }
            else if (_Reqtype == "clientregistration")
            {

                string _Reqsusername = ClearInject(dict["clientname"]);
                string _Reqtpassw = ClearInject(dict["address"]);
                string _Reqmobile = ClearInject(dict["mobileno"]);
                //string _Reqstatus = ClearInject(dict["status"]);
                string Result_Json = client_registration(_Reqsusername, _Reqtpassw, _Reqmobile);
                string sql_res = "UPDATE Tbl_ApiRequest_ResponseQrCodeutility SET Response = '" + Result_Json.Trim() + "',ReqType = 'clientregistration' WHERE ReqID = '" + sResult.Trim() + "'";
                int x_res = ObjDal.SaveData(sql_res);
                Result_Json = Result_Json.Replace(Environment.NewLine, "").Replace("\n", "").Replace("\r", "");
                Response.Clear();
                Response.ContentType = "application/json";
                Response.Write(Result_Json);
            }
            else if (_Reqtype == "clientlist")
            {

                string Result_Json = FUN_clientname();
                string sql_res = "UPDATE Tbl_ApiRequest_ResponseQrCodeutility SET Response = '" + Result_Json.Trim() + "',ReqType = 'clientlist' WHERE ReqID = '" + sResult.Trim() + "'";
                int x_res = ObjDal.SaveData(sql_res);
                Result_Json = Result_Json.Replace(Environment.NewLine, "").Replace("\n", "").Replace("\r", "");
                Response.Clear();
                Response.ContentType = "application/json";
                Response.Write(Result_Json);
            }
            else if (_Reqtype == "addcategory")
            {

                string _Reqcategory = ClearInject(dict["categoryanme"]);
                string _Reqdescrip = ClearInject(dict["description"]);
                string Result_Json = category_registration(_Reqcategory, _Reqdescrip);
                string sql_res = "UPDATE Tbl_ApiRequest_ResponseQrCodeutility SET Response = '" + Result_Json.Trim() + "',ReqType = 'addcategory' WHERE ReqID = '" + sResult.Trim() + "'";
                int x_res = ObjDal.SaveData(sql_res);
                Result_Json = Result_Json.Replace(Environment.NewLine, "").Replace("\n", "").Replace("\r", "");
                Response.Clear();
                Response.ContentType = "application/json";
                Response.Write(Result_Json);
            }
            else if (_Reqtype == "categorylist")
            {

                string Result_Json = FUN_category();
                string sql_res = "UPDATE Tbl_ApiRequest_ResponseQrCodeutility SET Response = '" + Result_Json.Trim() + "',ReqType = 'categorylist' WHERE ReqID = '" + sResult.Trim() + "'";
                int x_res = ObjDal.SaveData(sql_res);
                Result_Json = Result_Json.Replace(Environment.NewLine, "").Replace("\n", "").Replace("\r", "");
                Response.Clear();
                Response.ContentType = "application/json";
                Response.Write(Result_Json);
            }
            else if (_Reqtype == "addproduct")
            {

                string _Reqcategory = ClearInject(dict["productname"]);
                //string _Reqdescrip = ClearInject(dict["description"]);
                string _Reqcategroyid = ClearInject(dict["categoryname"]);
                string _Reqprice = ClearInject(dict["price"]);
                string _Reqqty = ClearInject(dict["quantity"]);
                string Result_Json = product_registration(_Reqcategory, _Reqcategroyid, _Reqprice, _Reqqty);
                string sql_res = "UPDATE Tbl_ApiRequest_ResponseQrCodeutility SET Response = '" + Result_Json.Trim() + "',ReqType = 'addproduct' WHERE ReqID = '" + sResult.Trim() + "'";
                int x_res = ObjDal.SaveData(sql_res);
                Result_Json = Result_Json.Replace(Environment.NewLine, "").Replace("\n", "").Replace("\r", "");
                Response.Clear();
                Response.ContentType = "application/json";
                Response.Write(Result_Json);
            }
            else if (_Reqtype == "productlist")
            {
                string _Reqcategory = ClearInject(dict["categoryid"]);
                string Result_Json = FUN_product(_Reqcategory);
                string sql_res = "UPDATE Tbl_ApiRequest_ResponseQrCodeutility SET Response = '" + Result_Json.Trim() + "',ReqType = 'productlist' WHERE ReqID = '" + sResult.Trim() + "'";
                int x_res = ObjDal.SaveData(sql_res);
                Result_Json = Result_Json.Replace(Environment.NewLine, "").Replace("\n", "").Replace("\r", "");
                Response.Clear();
                Response.ContentType = "application/json";
                Response.Write(Result_Json);
            }
            else if (_Reqtype == "orderdetail")
            {
                string _Reqorderid = ClearInject(dict["orderid"]);
                string Result_Json = FUN_orderdetail(_Reqorderid);
                string sql_res = "UPDATE Tbl_ApiRequest_ResponseQrCodeutility SET Response = '" + Result_Json.Trim() + "',ReqType = 'orderdetail' WHERE ReqID = '" + sResult.Trim() + "'";
                int x_res = ObjDal.SaveData(sql_res);
                Result_Json = Result_Json.Replace(Environment.NewLine, "").Replace("\n", "").Replace("\r", "");
                Response.Clear();
                Response.ContentType = "application/json";
                Response.Write(Result_Json);
            }
            else if (_Reqtype == "vieworderdetail")
            {
                string _Reqorderid = ClearInject(dict["orderid"]);
                string Result_Json = FUN_orderdetailview(_Reqorderid);
                string sql_res = "UPDATE Tbl_ApiRequest_ResponseQrCodeutility SET Response = '" + Result_Json.Trim() + "',ReqType = 'vieworderdetail' WHERE ReqID = '" + sResult.Trim() + "'";
                int x_res = ObjDal.SaveData(sql_res);
                Result_Json = Result_Json.Replace(Environment.NewLine, "").Replace("\n", "").Replace("\r", "");
                Response.Clear();
                Response.ContentType = "application/json";
                Response.Write(Result_Json);
            }
            else if (_Reqtype == "orderupdate")
            {
                string _Reqorderid = ClearInject(dict["orderid"]);
                string _Reqbillno = ClearInject(dict["billno"]);
                string _Requserid = ClearInject(dict["userid"]);
                string _Reqstatus = ClearInject(dict["status"]);
                string Result_Json = FUN_orderupdate(_Reqorderid,_Reqbillno, _Requserid, _Reqstatus);
                string sql_res = "UPDATE Tbl_ApiRequest_ResponseQrCodeutility SET Response = '" + Result_Json.Trim() + "',ReqType = 'orderupdate' WHERE ReqID = '" + sResult.Trim() + "'";
                int x_res = ObjDal.SaveData(sql_res);
                Result_Json = Result_Json.Replace(Environment.NewLine, "").Replace("\n", "").Replace("\r", "");
                Response.Clear();
                Response.ContentType = "application/json";
                Response.Write(Result_Json);
            }
            else if (_Reqtype == "getuserpermission")
            {
                string _Reqorderid = ClearInject(dict["userid"]);
                string Result_Json = FUN_userpermission(_Reqorderid);
                string sql_res = "UPDATE Tbl_ApiRequest_ResponseQrCodeutility SET Response = '" + Result_Json.Trim() + "',ReqType = 'getuserpermission' WHERE ReqID = '" + sResult.Trim() + "'";
                int x_res = ObjDal.SaveData(sql_res);
                Result_Json = Result_Json.Replace(Environment.NewLine, "").Replace("\n", "").Replace("\r", "");
                Response.Clear();
                Response.ContentType = "application/json";
                Response.Write(Result_Json);
            }
            else if (_Reqtype == "userlist")
            {

                string Result_Json = FUN_username();
                string sql_res = "UPDATE Tbl_ApiRequest_ResponseQrCodeutility SET Response = '" + Result_Json.Trim() + "',ReqType = 'userlist' WHERE ReqID = '" + sResult.Trim() + "'";
                int x_res = ObjDal.SaveData(sql_res);
                Result_Json = Result_Json.Replace(Environment.NewLine, "").Replace("\n", "").Replace("\r", "");
                Response.Clear();
                Response.ContentType = "application/json";
                Response.Write(Result_Json);
            }
            else
            {
                ErrObj.Response = "FAILED";
                WriteJson(ErrObj);
            }
        }
        catch (Exception)
        {
            ErrObj.Response = "FAILED";
            WriteJson(ErrObj);
        }
    }
    public class SalesRegistrationRequest
    {
        public string reqtype { get; set; }
        public string userid { get; set; }     // 👈 इसे string रखो
        public string username { get; set; }
        public string password { get; set; }
        public string mobileno { get; set; }
    }
    public class Item
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class OrderRequest
    {
        public string reqtype { get; set; }
        public string client { get; set; }
        public string category { get; set; }
        public List<Item> items { get; set; }
        public decimal grandTotal { get; set; }
        public decimal userid { get; set; }
    }
    public string CheckLogin(string userName, string pass)
    {
      
        try
        {
            string _output = "";
            string groupId = "";
            string groupname = "";
            bool isValid = false;
            DataTable dt = new DataTable();
            string Str = "";
            //Str = "SELECT SalespersonID FROM Salesperson AS A WHERE A.Username = '" + userName + "' and a.password='"+ pass +"'";
            Str = " SELECT a.Uid,a.username,a.Passw,a.groupid,b.GroupName FROM M_usermaster AS A inner join M_UserGroupMaster as b on a.groupid=b.groupid  WHERE A.Username = '" + userName + "' and a.passw='" + pass + "'";
            dt = SqlHelper.ExecuteDataset(constr, CommandType.Text, Str).Tables[0];
            if (dt.Rows.Count > 0)
            {

                groupId = dt.Rows[0]["groupid"].ToString().Trim();
                groupname = dt.Rows[0]["GroupName"].ToString().Trim();
                //return "{\"response\":\"OK\",\"msg\":\"Success\"}";
                //return "{\"response\":\"OK\",\"msg\":\"Success\",\\\"groupid\\\":\\\"\" + groupId + \"\\\",\\\"groupname\\\":\\\"\" + groupname + \"\\\"}";
                _output = "{\"permissionid\":\"" + dt.Rows[0]["groupid"] + "\",\"permissionname\":\"" + dt.Rows[0]["GroupName"].ToString() + "\",\"response\":\"OK\",\"msg\":\"Success\"";
                _output += "}";
                //_output = "{\"response\":\"OK\",\"msg\":\"Success\",\"groupid\":\"" + dt.Rows[0]["groupid"] + "\",";
                //_output += "\"groupname\":\"" + dt.Rows[0]["GroupName"].ToString() + "\"}";
                return _output;

            }
            else {
                return "{\"response\":\"FAILED\",\"msg\":\"Invalid Login Details.\"}";
            }
        }
        catch (Exception ex)
        {
            return "{\"response\":\"FAILED\",\"msg\":\"" + ex.Message + "\"}";
        }
    }
    private string addgroup(string username)
    {
        bool isValidUser = true;
        string _output = "";


        if (username.ToString() == "")
        {
            _output = "{\"response\":\"FAILED\",\"msg\":\"Please enter group Name.\"}";
            return _output;

        }
        if (checkgroup(username) != "Ok")
        {
            _output = "{\"response\":\"FAILED\",\"msg\":\"Your Group Name already registered on another Group Name.\"}";
            return _output;
        }
        string strQry;
            strQry = "Insert into M_usergroupmaster (groupid,GroupName,Activestatus)";
            strQry += "Values((SELECT ISNULL(MAX(GroupID),0) + 1 FROM M_UserGroupMaster),'" + username.ToString() + "','Y')";
            int i = Convert.ToInt32(SqlHelper.ExecuteNonQuery(constr, CommandType.Text, strQry));
            if (i > 0)
            {
               
                    _output = "{\"response\":\"OK\",\"msg\":\"Add Group Successfully!!\"}";
            }
            else
            {
                _output = "{\"response\":\"FAILED\",\"msg\":\"Not Registered.\"}";
            }
        return _output;
    }
    private string FUN_registration(string userid,string username, string password, string mobileno)
    {
        bool isValidUser = true;
        string _output = "";
        if (userid.ToString() == "")
        {
            _output = "{\"response\":\"FAILED\",\"msg\":\"Please enter User Id.\"}";
            return _output;

        }

        if (username.ToString() == "")
        {
            _output = "{\"response\":\"FAILED\",\"msg\":\"Please enter User Name.\"}";
            return _output;

        }
        if (mobileno.ToString() == "")
        {
            _output = "{\"response\":\"FAILED\",\"msg\":\"Please enter Mobile no.\"}";
            return _output;

        }
        if (checkname(username) != "Ok")
        {
            _output = "{\"response\":\"FAILED\",\"msg\":\"Your username already registered on another user.\"}";
            return _output;
        }
        if (checkmobilesales(mobileno) != "Ok")
        {
            _output = "{\"response\":\"FAILED\",\"msg\":\"Your Mobile no already registered on another sales person.\"}";
            return _output;
        }
        if (isValidUser)
        {
            string SqlStr2 = "select groupid from M_usergroupmaster ";
            string userid1 = "";
            SqlStr2 += " where groupid='" + userid + "'";
            DataTable Dt2 = new DataTable();
            Dt2 = SqlHelper.ExecuteDataset(constr, CommandType.Text, SqlStr2).Tables[0];
            if (Dt2.Rows.Count > 0)
            {
                userid1= Dt2.Rows[0]["groupid"].ToString();
            }
            else
            {
                _output = "{\"response\":\"FAILED\",\"msg\":\"Your group Not registered.\"}";
                return _output;
            }
            string strQry;
                strQry = "Insert into M_usermaster (groupid,UserName,Passw,Mobileno)";
                strQry += "Values('"+ userid1 + "','" + username.ToString() + "','" + password.ToString() + "',";
                strQry += "'" + mobileno.ToString()+ "')";
                int i = Convert.ToInt32(SqlHelper.ExecuteNonQuery(constr, CommandType.Text, strQry));
                if (i > 0)
                {
                    string SqlStr1 = "select FORMAT(rectimestamp, 'dd-MMM-yyyy hh:mm tt') AS FormattedDate,* from M_usermaster";
                    SqlStr1 += " where MobileNo = '" + mobileno.ToString() + "'";
                    DataTable Dt1 = new DataTable();
                    Dt1 = SqlHelper.ExecuteDataset(constr, CommandType.Text, SqlStr1).Tables[0];
                    if (Dt1.Rows.Count > 0)
                    {
                        _output = "{\"response\":\"OK\",\"msg\":\"Sales Registered Successfully!!\",\"username\":\"" + Dt1.Rows[0]["username"] + "\",";
                        _output += "\"password\":\"" + Dt1.Rows[0]["passw"].ToString() + "\",\"joindate\":\"" + Dt1.Rows[0]["FormattedDate"] + "\"}";
                    }
                }
                else
                {
                    _output = "{\"response\":\"FAILED\",\"msg\":\"Not Registered.\"}";
                }
        }
        else
        {
            _output = "{\"response\":\"FAILED\",\"sponsorname\":\"\",\"msg\":\"Invalid Login Details.\"}";
        }

        return _output;
    }
    private string client_registration(string username, string address, string mobileno)
    {
        bool isValidUser = true;
        string _output = "";


        if (username.ToString() == "")
        {
            _output = "{\"response\":\"FAILED\",\"msg\":\"Please enter Client Name.\"}";
            return _output;

        }
        if (address.ToString() == "")
        {
            _output = "{\"response\":\"FAILED\",\"msg\":\"Please enter address.\"}";
            return _output;

        }
        if (mobileno.ToString() == "")
        {
            _output = "{\"response\":\"FAILED\",\"msg\":\"Please enter Mobile no.\"}";
            return _output;

        }
        if (checkmobileclient(mobileno) != "Ok")
        {
            _output = "{\"response\":\"FAILED\",\"msg\":\"Your Mobile no already registered on another Client.\"}";
            return _output;
        }
        if (isValidUser)
        {
            string strQry;
            strQry = "Insert into Client (ClientName,Address,Mobile)";
            strQry += "Values('" + username.ToString() + "','" + address.ToString() + "',";
            strQry += "'" + mobileno.ToString() + "')";
            int i = Convert.ToInt32(SqlHelper.ExecuteNonQuery(constr, CommandType.Text, strQry));
            if (i > 0)
            {
                string SqlStr1 = "select FORMAT(JoinDate, 'dd-MMM-yyyy hh:mm tt') AS FormattedDate,* from Client";
                SqlStr1 += " where Mobile = '" + mobileno.ToString() + "'";
                DataTable Dt1 = new DataTable();
                Dt1 = SqlHelper.ExecuteDataset(constr, CommandType.Text, SqlStr1).Tables[0];
                if (Dt1.Rows.Count > 0)
                {
                    _output = "{\"response\":\"OK\",\"msg\":\"Client Registered Successfully!!\",\"clientname\":\"" + Dt1.Rows[0]["Clientname"] + "\",";
                    _output += "\"address\":\"" + Dt1.Rows[0]["address"].ToString() + "\",\"mobileno\":\"" + Dt1.Rows[0]["mobile"] + "\",\"joindate\":\"" + Dt1.Rows[0]["FormattedDate"] + "\"}";
                }
            }
            else
            {
                _output = "{\"response\":\"FAILED\",\"msg\":\"Not Registered.\"}";
            }
        }
        else
        {
            _output = "{\"response\":\"FAILED\",\"sponsorname\":\"\",\"msg\":\"Invalid Login Details.\"}";
        }

        return _output;
    }
    private string category_registration(string categoryname, string description)
    {
        bool isValidUser = true;
        string _output = "";


        if (categoryname.ToString() == "")
        {
            _output = "{\"response\":\"FAILED\",\"msg\":\"Please enter category Name.\"}";
            return _output;

        }
        //if (description.ToString() == "")
        //{
        //    _output = "{\"response\":\"FAILED\",\"msg\":\"Please enter description.\"}";
        //    return _output;

        //}
        if (checkcategory(categoryname) != "Ok")
        {
            _output = "{\"response\":\"FAILED\",\"msg\":\"Your category already registered.\"}";
            return _output;
        }
        if (isValidUser)
        {
            string strQry;
            strQry = "Insert into Category (CategoryName,Description)";
            strQry += "Values('" + categoryname.ToString() + "','" + description.ToString() + "')";
            int i = Convert.ToInt32(SqlHelper.ExecuteNonQuery(constr, CommandType.Text, strQry));
            if (i > 0)
            {
                    _output = "{\"response\":\"OK\",\"msg\":\"Add Category Successfully!!\"}";
            }
            else
            {
                _output = "{\"response\":\"FAILED\",\"msg\":\"Not Registered.\"}";
            }
        }
        else
        {
            _output = "{\"response\":\"FAILED\",\"sponsorname\":\"\",\"msg\":\"Invalid Login Details.\"}";
        }

        return _output;
    }
    private string product_registration(string productname, string categoryname, string price,string qty)
    {
        bool isValidUser = true;
        string _output = "";
        if (productname.ToString() == "")
        {
            _output = "{\"response\":\"FAILED\",\"msg\":\"Please enter Product Name.\"}";
            return _output;

        }

        if (categoryname.ToString() == "0")
        {
            _output = "{\"response\":\"FAILED\",\"msg\":\"Please enter category Name.\"}";
            return _output;

        }
        if (price.ToString() == "")
        {
            _output = "{\"response\":\"FAILED\",\"msg\":\"Please enter price.\"}";
            return _output;

        }
        if (price.ToString() == "0")
        {
            _output = "{\"response\":\"FAILED\",\"msg\":\"Please enter price greater than zero.\"}";
            return _output;

        }
        
        //    if (qty.ToString() == "0")
        //{
        //    _output = "{\"response\":\"FAILED\",\"msg\":\"Please enter quantity greater than zero.\"}";
        //    return _output;

        //}
        //if (description.ToString() == "")
        //{
        //    _output = "{\"response\":\"FAILED\",\"msg\":\"Please enter description.\"}";
        //    return _output;

        //}
        if (checkproduct(productname) != "Ok")
        {
            _output = "{\"response\":\"FAILED\",\"msg\":\"Your category already registered.\"}";
            return _output;
        }
        if (isValidUser)
        {
            string CategoryID = "";
            string SqlStr2 = "select CategoryID from Category ";
            SqlStr2 += " where CategoryName='" + categoryname.ToString() + "'";
            DataTable Dt2 = new DataTable();
            Dt2 = SqlHelper.ExecuteDataset(constr, CommandType.Text, SqlStr2).Tables[0];
            if (Dt2.Rows.Count > 0)
            {
                CategoryID = Dt2.Rows[0]["CategoryID"].ToString();
            }
            else {
                _output = "{\"response\":\"FAILED\",\"msg\":\"Your category Not registered.\"}";
                return _output;
            }
            string strQry;
            strQry = "Insert into Product (ProductName,CategoryId,price,StockQty)";
            strQry += "Values('"+ productname.ToString() + "','" + CategoryID.ToString() + "','" + price.ToString() + "','"+ qty.ToString ()+"')";
            int i = Convert.ToInt32(SqlHelper.ExecuteNonQuery(constr, CommandType.Text, strQry));
            if (i > 0)
            {
                _output = "{\"response\":\"OK\",\"msg\":\"Add Product Successfully!!\"}";
            }
            else
            {
                _output = "{\"response\":\"FAILED\",\"msg\":\"Not Registered.\"}";
            }
        }
        else
        {
            _output = "{\"response\":\"FAILED\",\"sponsorname\":\"\",\"msg\":\"Invalid Login Details.\"}";
        }

        return _output;
    }
    private string checkmobilesales(string mobileno)
    {
        string sql = "";
        string _Output = "";
        string errType = "";

        try
        {
            if (!string.IsNullOrWhiteSpace(mobileno))
            {
                sql = "SELECT COUNT(*) AS Cnt FROM M_usermaster WHERE MobileNo = '" + mobileno + "'";
                DataTable Dt = new DataTable();
                Dt = SqlHelper.ExecuteDataset(constr, CommandType.Text, sql).Tables[0];
                if (Dt.Rows.Count > 0)
                {
                    if (Convert.ToInt64(Dt.Rows[0]["Cnt"]) >= 1)
                    {
                        errType = "Mobileno";
                        _Output = "Faild";
                    }
                    else
                    {
                        _Output = "Ok";
                    }
                }
            }
        }
        catch (Exception)
        {
            // Handle exception if needed
        }

        return _Output;
    }
    private string checkname(string username)
    {
        string sql = "";
        string _Output = "";
        string errType = "";

        try
        {
            if (!string.IsNullOrWhiteSpace(username))
            {
                sql = "SELECT COUNT(*) AS Cnt FROM M_usermaster WHERE username = '" + username + "'";
                DataTable Dt = new DataTable();
                Dt = SqlHelper.ExecuteDataset(constr, CommandType.Text, sql).Tables[0];
                if (Dt.Rows.Count > 0)
                {
                    if (Convert.ToInt64(Dt.Rows[0]["Cnt"]) >= 1)
                    {
                        errType = "Username";
                        _Output = "Faild";
                    }
                    else
                    {
                        _Output = "Ok";
                    }
                }
            }
        }
        catch (Exception)
        {
            // Handle exception if needed
        }

        return _Output;
    }
    private string checkgroup(string groupname)
    {
        string sql = "";
        string _Output = "";
        string errType = "";

        try
        {
            if (!string.IsNullOrWhiteSpace(groupname))
            {
                sql = "SELECT COUNT(*) AS Cnt FROM M_usergroupmaster WHERE grouopname = '" + groupname + "'";
                DataTable Dt = new DataTable();
                Dt = SqlHelper.ExecuteDataset(constr, CommandType.Text, sql).Tables[0];
                if (Dt.Rows.Count > 0)
                {
                    if (Convert.ToInt64(Dt.Rows[0]["Cnt"]) >= 1)
                    {
                        errType = "groupname";
                        _Output = "Faild";
                    }
                    else
                    {
                        _Output = "Ok";
                    }
                }
            }
        }
        catch (Exception)
        {
            // Handle exception if needed
        }

        return _Output;
    }
    private string checkmobileclient(string mobileno)
    {
        string sql = "";
        string _Output = "";
        string errType = "";

        try
        {
            if (!string.IsNullOrWhiteSpace(mobileno))
            {
                sql = "SELECT COUNT(*) AS Cnt FROM client WHERE Mobile = '" + mobileno + "'";
                DataTable Dt = new DataTable();
                Dt = SqlHelper.ExecuteDataset(constr, CommandType.Text, sql).Tables[0];
                if (Dt.Rows.Count > 0)
                {
                    if (Convert.ToInt64(Dt.Rows[0]["Cnt"]) >= 1)
                    {
                        errType = "Mobileno";
                        _Output = "Faild";
                    }
                    else
                    {
                        _Output = "Ok";
                    }
                }
            }
        }
        catch (Exception)
        {
            // Handle exception if needed
        }

        return _Output;
    }
    private string checkcategory(string categoryname)
    {
        string sql = "";
        string _Output = "";
        string errType = "";

        try
        {
            if (!string.IsNullOrWhiteSpace(categoryname))
            {
                sql = "SELECT COUNT(*) AS Cnt FROM Category WHERE CategoryName= '" + categoryname + "'";
                DataTable Dt = new DataTable();
                Dt = SqlHelper.ExecuteDataset(constr, CommandType.Text, sql).Tables[0];
                if (Dt.Rows.Count > 0)
                {
                    if (Convert.ToInt64(Dt.Rows[0]["Cnt"]) >= 1)
                    {
                        errType = "CategoryName";
                        _Output = "Faild";
                    }
                    else
                    {
                        _Output = "Ok";
                    }
                }
            }
        }
        catch (Exception)
        {
            // Handle exception if needed
        }

        return _Output;
    }
    private string checkproduct(string productanme)
    {
        string sql = "";
        string _Output = "";
        string errType = "";

        try
        {
            if (!string.IsNullOrWhiteSpace(productanme))
            {
                sql = "SELECT COUNT(*) AS Cnt FROM Product WHERE ProductName= '" + productanme + "'";
                DataTable Dt = new DataTable();
                Dt = SqlHelper.ExecuteDataset(constr, CommandType.Text, sql).Tables[0];
                if (Dt.Rows.Count > 0)
                {
                    if (Convert.ToInt64(Dt.Rows[0]["Cnt"]) >= 1)
                    {
                        errType = "ProductName";
                        _Output = "Faild";
                    }
                    else
                    {
                        _Output = "Ok";
                    }
                }
            }
        }
        catch (Exception)
        {
            // Handle exception if needed
        }

        return _Output;
    }
    private string getbillno(string billno)
    {
        string _Output = "";
        string errType;
        string col;
        string sql;
        try
        {

            if (!string.IsNullOrWhiteSpace(billno))
        {
            sql = "SELECT COUNT(billno) AS Cnt FROM Tbl_Orders WHERE Billno= '" + billno + "'";
            DataTable Dt = new DataTable();
            Dt = SqlHelper.ExecuteDataset(constr, CommandType.Text, sql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                if (Convert.ToInt64(Dt.Rows[0]["Cnt"]) >= 1)
                {
                    errType = "Billno already exist.";
                    _Output = "Faild";
                }
                else
                {
                    _Output = "Ok";
                }
            }
        }
      
        }
        catch (Exception)
        {
            // Handle exception if needed
        }
        return _Output;
    }
    private string getorderno(string orderid)
    {
        string _Output = "";
        string errType;
        string col;
        string sql;
        try
        {

            if (!string.IsNullOrWhiteSpace(orderid))
            {
                sql = "SELECT COUNT(OrderId) AS Cnt FROM Tbl_Orders WHERE OrderId= '" + orderid + "'";
                DataTable Dt = new DataTable();
                Dt = SqlHelper.ExecuteDataset(constr, CommandType.Text, sql).Tables[0];
                if (Dt.Rows.Count > 0)
                {
                    //if (Convert.ToInt64(Dt.Rows[0]["Cnt"]) >= 1)
                    //{
                    //    errType = "Order no already exist.";
                    //    _Output = "Faild";
                    //}
                    //else
                    //{
                    _Output = "Ok";
                    //}
                }
                else {
                    _Output = "Faild";
                }
            }

        }
        catch (Exception)
        {
            // Handle exception if needed
        }
        return _Output;
    }
    private string getorderstatus(string orderid,string orderstatus)
    {
        string _Output = "";
        string errType;
        string col;
        string sql;
        DataTable Dt = new DataTable();
        try
        {

            if (!string.IsNullOrWhiteSpace(orderid))
            {
                if (orderstatus == "P")
                {
                    sql = "SELECT COUNT(OrderId) AS Cnt FROM Tbl_Orders WHERE OrderId= '" + orderid + "' and activestatus='" + orderstatus + "'";
                    Dt = SqlHelper.ExecuteDataset(constr, CommandType.Text, sql).Tables[0];
                }
                if (orderstatus == "I")
                {
                    sql = "SELECT COUNT(OrderId) AS Cnt FROM Tbl_Orders WHERE OrderId= '" + orderid + "' and Invoicestatus='" + orderstatus + "'";
                    Dt = SqlHelper.ExecuteDataset(constr, CommandType.Text, sql).Tables[0];
                }
                if (orderstatus == "D")
                {
                    sql = "SELECT COUNT(OrderId) AS Cnt FROM Tbl_Orders WHERE OrderId= '" + orderid + "' and Dispatchstatus='" + orderstatus + "'";
                    Dt = SqlHelper.ExecuteDataset(constr, CommandType.Text, sql).Tables[0];
                }               
                if (Convert.ToInt32(Dt.Rows[0]["Cnt"]) > 0)
                {
                    //if (Convert.ToInt64(Dt.Rows[0]["Cnt"]) >= 1)
                    //{
                    //    errType = "Order no already exist.";
                    //    _Output = "Faild";
                    //}
                    //else
                    //{
                    _Output = "Ok";
                    //}
                }
                else
                {
                    _Output = "Faild";
                }
            }

        }
        catch (Exception)
        {
            // Handle exception if needed
        }
        return _Output;
    }
    private string FUN_groupname()
    {
        string _Output = "";
        int RecordCount = 0;
        try
        {
           
                //int FormNo = GetFormNo(ClearInject(userid));
                DataTable dt = new DataTable();
                string col;
                _Output = "{\"allgroup\":[  ";

                string strQry = "Exec Sp_GetgroupList";
                DataSet ds1 = SqlHelper.ExecuteDataset(constr, CommandType.Text, strQry);
                if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                {
                    dt = ds1.Tables[0];
                }
                foreach (DataRow Dr in dt.Rows)
                {
                    col = "{";

                    foreach (DataColumn column in dt.Columns)
                    {
                        string value = Dr[column] == DBNull.Value ? "0" : Dr[column].ToString();
                        col += "\"" + column.ColumnName + "\":\"" + value + "\",";
                    }

                    col = col.TrimEnd(',');
                    col += "},";
                    _Output += col;
                }

                if (dt.Rows.Count > 0)
                {
                    _Output = _Output.TrimEnd(',');
                    //RecordCount = Convert.ToInt32(ds1.Tables[1].Rows[0]["RecordCount"]);
                }

                _Output += "],\"response\":\"OK\",\"msg\":\"Success\"}";
        }
        catch (Exception ex)
        {
            _Output = "{\"response\":\"FAILED\",\"msg\":\"" + ex.Message + "\"}";
        }
        return _Output;
    }
    private string FUN_username()
    {
        string _Output = "";
        int RecordCount = 0;
        try
        {

            //int FormNo = GetFormNo(ClearInject(userid));
            DataTable dt = new DataTable();
            string col;
            _Output = "{\"alluser\":[  ";

            string strQry = "Exec Sp_GetUserList";
            DataSet ds1 = SqlHelper.ExecuteDataset(constr, CommandType.Text, strQry);
            if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
            {
                dt = ds1.Tables[0];
            }
            foreach (DataRow Dr in dt.Rows)
            {
                col = "{";

                foreach (DataColumn column in dt.Columns)
                {
                    string value = Dr[column] == DBNull.Value ? "0" : Dr[column].ToString();
                    col += "\"" + column.ColumnName + "\":\"" + value + "\",";
                }

                col = col.TrimEnd(',');
                col += "},";
                _Output += col;
            }

            if (dt.Rows.Count > 0)
            {
                _Output = _Output.TrimEnd(',');
                //RecordCount = Convert.ToInt32(ds1.Tables[1].Rows[0]["RecordCount"]);
            }

            _Output += "],\"response\":\"OK\",\"msg\":\"Success\"}";
        }
        catch (Exception ex)
        {
            _Output = "{\"response\":\"FAILED\",\"msg\":\"" + ex.Message + "\"}";
        }
        return _Output;
    }
    private string FUN_clientname()
    {
        string _Output = "";
        int RecordCount = 0;
        try
        {

            //int FormNo = GetFormNo(ClearInject(userid));
            DataTable dt = new DataTable();
            string col;
            _Output = "{\"allclient\":[  ";

            string strQry = "Exec Sp_GetClientList";
            DataSet ds1 = SqlHelper.ExecuteDataset(constr, CommandType.Text, strQry);
            if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
            {
                dt = ds1.Tables[0];
            }
            foreach (DataRow Dr in dt.Rows)
            {
                col = "{";

                foreach (DataColumn column in dt.Columns)
                {
                    string value = Dr[column] == DBNull.Value ? "0" : Dr[column].ToString();
                    col += "\"" + column.ColumnName + "\":\"" + value + "\",";
                }

                col = col.TrimEnd(',');
                col += "},";
                _Output += col;
            }

            if (dt.Rows.Count > 0)
            {
                _Output = _Output.TrimEnd(',');
                //RecordCount = Convert.ToInt32(ds1.Tables[1].Rows[0]["RecordCount"]);
            }

            _Output += "],\"response\":\"OK\",\"msg\":\"Success\"}";
        }
        catch (Exception ex)
        {
            _Output = "{\"response\":\"FAILED\",\"msg\":\"" + ex.Message + "\"}";
        }
        return _Output;
    }
    private string FUN_category()
    {
        string _Output = "";
        int RecordCount = 0;
        try
        {

            //int FormNo = GetFormNo(ClearInject(userid));
            DataTable dt = new DataTable();
            string col;
            _Output = "{\"allcategory\":[  ";

            string strQry = "Exec Sp_GetCategoryList";
            DataSet ds1 = SqlHelper.ExecuteDataset(constr, CommandType.Text, strQry);
            if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
            {
                dt = ds1.Tables[0];
            }
            foreach (DataRow Dr in dt.Rows)
            {
                col = "{";

                foreach (DataColumn column in dt.Columns)
                {
                    string value = Dr[column] == DBNull.Value ? "0" : Dr[column].ToString();
                    col += "\"" + column.ColumnName + "\":\"" + value + "\",";
                }

                col = col.TrimEnd(',');
                col += "},";
                _Output += col;
            }

            if (dt.Rows.Count > 0)
            {
                _Output = _Output.TrimEnd(',');
                //RecordCount = Convert.ToInt32(ds1.Tables[1].Rows[0]["RecordCount"]);
            }

            _Output += "],\"response\":\"OK\",\"msg\":\"Success\"}";
        }
        catch (Exception ex)
        {
            _Output = "{\"response\":\"FAILED\",\"msg\":\"" + ex.Message + "\"}";
        }
        return _Output;
    }

    private string FUN_product(string categoryid)
    {
        string _Output = "";
        int RecordCount = 0;
        try
        {

            //int FormNo = GetFormNo(ClearInject(userid));
            //DataTable dt = new DataTable();
            //string col;
            //_Output = "{\"allproduct\":[  ";

            //string strQry = "Exec Sp_GetProductList";
            //DataSet ds1 = SqlHelper.ExecuteDataset(constr, CommandType.Text, strQry);
            //if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
            //{
            //    dt = ds1.Tables[0];
            //}
            //foreach (DataRow Dr in dt.Rows)
            //{
            //    col = "{";

            //    foreach (DataColumn column in dt.Columns)
            //    {
            //        string value = Dr[column] == DBNull.Value ? "0" : Dr[column].ToString();
            //        col += "\"" + column.ColumnName + "\":\"" + value + "\",";
            //    }

            //    col = col.TrimEnd(',');
            //    col += "},";
            //    _Output += col;
            //}

            //if (dt.Rows.Count > 0)
            //{
            //    _Output = _Output.TrimEnd(',');
            //    //RecordCount = Convert.ToInt32(ds1.Tables[1].Rows[0]["RecordCount"]);
            //}

            //_Output += "],\"response\":\"OK\",\"msg\":\"Success\"}";
            DataTable dt = new DataTable();
            string col;
            _Output = "{\"allproduct\":[  ";

            //string strQry = "Exec Sp_GetProductList";
            string strQry = "Exec Sp_GetProductListNew '"+ categoryid.ToString() + "' ";
            DataSet ds1 = SqlHelper.ExecuteDataset(constr, CommandType.Text, strQry);
            if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
            {
                dt = ds1.Tables[0];
            }
            decimal grandTotal = 0;
            foreach (DataRow Dr in dt.Rows)
            {
                col = "{";

                foreach (DataColumn column in dt.Columns)
                {
                    string value = Dr[column] == DBNull.Value ? "0" : Dr[column].ToString();
                    col += "\"" + column.ColumnName + "\":\"" + value + "\",";
                }

                // grand total calculate
                if (dt.Columns.Contains("TotalAmount"))
                    grandTotal += Convert.ToDecimal(Dr["TotalAmount"]);

                col = col.TrimEnd(',');
                col += "},";
                _Output += col;
            }

            // 👉 yahan extra Total row add karni hai
            if (dt.Rows.Count > 0)
            {
                _Output += "{";
                //_Output += "\"ProductName\":\"Total\",";
                //_Output += "\"Quantity\":\"0\",";
                //_Output += "\"Rate\":\"0.00\",";
                _Output += "\"Total\":\"" + grandTotal.ToString("0.00") + "\"";
                _Output += "}";
                _Output += "],\"response\":\"OK\",\"msg\":\"Success\"}";
            }
            else {
                _Output += "],\"response\":\"FAILED\",\"msg\":\"No Record Found\"}";
            }

        }
        catch (Exception ex)
        {
            _Output = "{\"response\":\"FAILED\",\"msg\":\"" + ex.Message + "\"}";
        }
        return _Output;
    }
    
    private string FUN_orderdetail(string orderid)
    {
        string _Output = "";
        int RecordCount = 0;
        try
        {

            DataTable dt = new DataTable();
            string col;
            _Output = "{\"Orderdetail\":[  ";

            //string strQry = "Exec Sp_GetProductList";
            string strQry = "Exec sp_GetOrderDetailById '" + orderid.ToString() + "' ";
            DataSet ds1 = SqlHelper.ExecuteDataset(constr, CommandType.Text, strQry);
            if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
            {
                dt = ds1.Tables[0];
            }
            foreach (DataRow Dr in dt.Rows)
            {
                col = "{";

                foreach (DataColumn column in dt.Columns)
                {
                    string value = Dr[column] == DBNull.Value ? "0" : Dr[column].ToString();
                    col += "\"" + column.ColumnName + "\":\"" + value + "\",";
                }

                col = col.TrimEnd(',');
                col += "},";
                _Output += col;
            }

            if (dt.Rows.Count > 0)
            {
                _Output = _Output.TrimEnd(',');
                //RecordCount = Convert.ToInt32(ds1.Tables[1].Rows[0]["RecordCount"]);
                _Output += "],\"response\":\"OK\",\"msg\":\"Success\"}";
            }
            else
            {
                _Output += "],\"response\":\"FAILED\",\"msg\":\"No Record Found\"}";
            }

        }
        catch (Exception ex)
        {
            _Output = "{\"response\":\"FAILED\",\"msg\":\"" + ex.Message + "\"}";
        }
        return _Output;
    }
    private string FUN_orderdetailview(string orderid)
    {
        string _Output = "";
        int RecordCount = 0;
        try
        {

            DataTable dt = new DataTable();
            string col;
            _Output = "{\"Orderdetailview\":[  ";
            string strQry = "Exec sp_GetOrderview '" + orderid.ToString() + "' ";
            DataSet ds1 = SqlHelper.ExecuteDataset(constr, CommandType.Text, strQry);
            if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
            {
                dt = ds1.Tables[0];
            }
            foreach (DataRow Dr in dt.Rows)
            {
                col = "{";

                foreach (DataColumn column in dt.Columns)
                {
                    string value = Dr[column] == DBNull.Value ? "0" : Dr[column].ToString();
                    col += "\"" + column.ColumnName + "\":\"" + value + "\",";
                }

                col = col.TrimEnd(',');
                col += "},";
                _Output += col;
            }

            if (dt.Rows.Count > 0)
            {
                _Output = _Output.TrimEnd(',');
                //RecordCount = Convert.ToInt32(ds1.Tables[1].Rows[0]["RecordCount"]);
                _Output += "],\"response\":\"OK\",\"msg\":\"Success\"}";
            }
            else
            {
                _Output += "],\"response\":\"FAILED\",\"msg\":\"No Record Found\"}";
            }

        }
        catch (Exception ex)
        {
            _Output = "{\"response\":\"FAILED\",\"msg\":\"" + ex.Message + "\"}";
        }
        return _Output;
    }
    private string FUN_orderupdate(string orderid,string billno,string userid, string activestatus)
    {
        string _Output = "";
        int RecordCount = 0;
        string errType = "";
        try
        {
            if (getorderno(orderid) != "Ok")
            {
                _Output = "{\"response\":\"FAILED\",\"msg\":\"No record found.\"}";
                return _Output;
            }

            
            // ====== पहले Update Query चलाओ ======
            if (activestatus == "P")
            {
                // Product pack update
                string updateQry = @"UPDATE Tbl_Orders 
                         SET ActiveStatus = @ActiveStatus,
                             UserId = @UserId,
                             BillNo = @BillNo,
                             ProductDate = GETDATE()
                         WHERE OrderId = @OrderId";

                SqlParameter[] updateParams = {
        new SqlParameter("@ActiveStatus", activestatus),
        new SqlParameter("@UserId", userid),
        new SqlParameter("@BillNo", billno),
        new SqlParameter("@OrderId", orderid)
    };
                int upd = SqlHelper.ExecuteNonQuery(constr, CommandType.Text, updateQry, updateParams);
            }
            else if (activestatus == "I")
            {
                if (getbillno(billno) != "Ok")
                {
                    _Output = "{\"response\":\"FAILED\",\"msg\":\"Your Bill No already registered.\"}";
                    return _Output;
                }
                // पहले चेक करो कि Product Pack हुआ या नहीं
                if (getorderstatus(orderid, "P") != "Ok")
                {
                    _Output = "{\"response\":\"FAILED\",\"msg\":\"Your Order Product Pack is Pending.\"}";
                    return _Output;
                }

                // Invoice update
                string updateQry = @"UPDATE Tbl_Orders 
                         SET Invoicestatus = @ActiveStatus,
                             UserId = @UserId,
                             BillNo = @BillNo,
                             Invoicedate = GETDATE()
                         WHERE OrderId = @OrderId";

                SqlParameter[] updateParams = {
        new SqlParameter("@ActiveStatus", activestatus),
        new SqlParameter("@UserId", userid),
        new SqlParameter("@BillNo", billno),
        new SqlParameter("@OrderId", orderid)
    };
                int upd = SqlHelper.ExecuteNonQuery(constr, CommandType.Text, updateQry, updateParams);
            }
            else if (activestatus == "D")
            {
                // पहले चेक करो कि Invoice हुआ या नहीं
                if (getorderstatus(orderid, "I") != "Ok")
                {
                    _Output = "{\"response\":\"FAILED\",\"msg\":\"Your Order Invoice is Pending.\"}";
                    return _Output;
                }

                // Dispatch update
                string updateQry = @"UPDATE Tbl_Orders 
                         SET Dispatchstatus = @ActiveStatus,
                             UserId = @UserId,
                             BillNo = @BillNo,
                             Dispatchdate = GETDATE()
                         WHERE OrderId = @OrderId";

                SqlParameter[] updateParams = {
        new SqlParameter("@ActiveStatus", activestatus),
        new SqlParameter("@UserId", userid),
        new SqlParameter("@BillNo", billno),
        new SqlParameter("@OrderId", orderid)
    };
                int upd = SqlHelper.ExecuteNonQuery(constr, CommandType.Text, updateQry, updateParams);
            }
            else
            {
                _Output = "{\"response\":\"FAILED\",\"msg\":\"Invalid Order Status.\"}";
                return _Output;
            }

            // ====== फिर से Data निकालो ======
            DataTable dt = new DataTable();
            _Output = "{\"orderupdate\":[  ";

            string strQry = "Exec sp_Getupdateorderlist '" + orderid.ToString() + "' ";
            DataSet ds1 = SqlHelper.ExecuteDataset(constr, CommandType.Text, strQry);

            if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
            {
                dt = ds1.Tables[0];
            }

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    _Output += "{";
                    _Output += "\"OrderNo\":\"" + dr["OrderNo"].ToString() + "\",";
                    _Output += "\"CurrentStatus\":\"" + dr["CurrentStatus"].ToString() + "\",";
                    _Output += "\"Invoicestatus\":\"" + dr["Invoicestatus"].ToString() + "\",";
                    _Output += "\"Dispatchstatus\":\"" + dr["Dispatchstatus"].ToString() + "\",";
                    _Output += "\"ProductDate\":\"" + dr["ProductDate"].ToString() + "\",";
                    _Output += "\"Invoicedate\":\"" + dr["Invoicedate"].ToString() + "\",";
                    _Output += "\"Dispatchdate\":\"" + dr["Dispatchdate"].ToString() + "\",";
                    //_Output += "\"UserId\":\"" + dr["UserId"].ToString() + "\",";
                    _Output += "\"BillNo\":\"" + dr["BillNo"].ToString() + "\"";
                    _Output += "},";
                }
                // यहाँ JSON बनाने का loop होगा अगर row by row details चाहिए
                // फिलहाल आपका पुराना code रख रहा हूँ
                _Output = _Output.TrimEnd(',');
                _Output += "],\"response\":\"OK\",\"msg\":\"Success\"}";
            }
            else
            {
                _Output += "],\"response\":\"FAILED\",\"msg\":\"No Record Found\"}";
            }

        }
        catch (Exception ex)
        {
            _Output = "{\"response\":\"FAILED\",\"msg\":\"" + ex.Message + "\"}";
        }
        return _Output;
    }
    private string FUN_userpermission(string userid)
    {
        string _Output = "";
        int RecordCount = 0;
        try
        {

            DataTable dt = new DataTable();
            string col;
            _Output = "{\"userpermission\":[  ";

            //string strQry = "Exec Sp_GetProductList";
            string strQry = "Exec Sp_GetParentMenuDigital " + userid + "  ";
            DataSet ds1 = SqlHelper.ExecuteDataset(constr, CommandType.Text, strQry);
            if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
            {
                dt = ds1.Tables[0];
            }
            foreach (DataRow Dr in dt.Rows)
            {
                col = "{";

                foreach (DataColumn column in dt.Columns)
                {
                    string value = Dr[column] == DBNull.Value ? "0" : Dr[column].ToString();
                    col += "\"" + column.ColumnName + "\":\"" + value + "\",";
                }

                col = col.TrimEnd(',');
                col += "},";
                _Output += col;
            }

            if (dt.Rows.Count > 0)
            {
                _Output = _Output.TrimEnd(',');
                //RecordCount = Convert.ToInt32(ds1.Tables[1].Rows[0]["RecordCount"]);
                _Output += "],\"response\":\"OK\",\"msg\":\"Success\"}";
            }
            else
            {
                _Output += "],\"response\":\"FAILED\",\"msg\":\"No Record Found\"}";
            }

        }
        catch (Exception ex)
        {
            _Output = "{\"response\":\"FAILED\",\"msg\":\"" + ex.Message + "\"}";
        }
        return _Output;
    }

    public string JsonEncode(string str)
    {
        str = str.Replace("\\", "\\\\");
        str = str.Replace("\"", "\\\"");
        str = str.Replace("//n", " \\n ");
        str = str.Replace("\\n", "\n");
        str = str.Replace("\n", " \\n ");
        if (!string.IsNullOrEmpty(str))
        {
            str = str.Replace(Environment.NewLine, " \\n ");
        }
        str = str.Replace("\r\n", " \\n ");
        str = str.Replace("\t", "\\t");
        return str;
    }
    private string ClearInject(string str)
    {
        string strReturn = str.Replace("'", "''").Replace("\t", " ");
        strReturn = strReturn.Replace("\\\\", "\\");
        if (!string.IsNullOrEmpty(strReturn))
        {
            strReturn = strReturn.Replace(Environment.NewLine, " \\n ");
        }
        return strReturn;
    }
    public void WriteJson(object _object)
    {
        try
        {
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string jsonData = javaScriptSerializer.Serialize(_object);
            WriteRaw(jsonData);
        }
        catch (Exception)
        {
            if (Conn != null)
            {
                if (Conn.State == ConnectionState.Open)
                {
                    Conn.Close();
                }
            }
        }
    }
    public void WriteRaw(string text)
    {
        try
        {
            Response.Write(text);
        }
        catch (Exception)
        {
            if (Conn != null)
            {
                if (Conn.State == ConnectionState.Open)
                {
                    Conn.Close();
                }
            }
        }
    }
    public DataSet ConvertJsonStringToDataSet(string jsonString)
    {
        XmlDocument xd = new XmlDocument();
        jsonString = "{ \"rootNode\": {" + jsonString.Trim().TrimStart('{').TrimEnd('}') + "} }";
        xd = JsonConvert.DeserializeXmlNode(jsonString);
        DataSet ds = new DataSet();
        ds.ReadXml(new XmlNodeReader(xd));
        return ds;
    }
    private string Decrypt(string data, byte[] Key, byte[] IV)
    {
        byte[] cipherText = Convert.FromBase64String(data);
        string plaintext = null;

        using (AesManaged aes = new AesManaged())
        {
            ICryptoTransform decryptor = aes.CreateDecryptor(Key, IV);

            using (MemoryStream ms = new MemoryStream(cipherText))
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader reader = new StreamReader(cs))
                    {
                        plaintext = reader.ReadToEnd();
                    }
                }
            }
        }

        return plaintext;
    }
    private string Encrypt(string plainText, byte[] Key, byte[] IV)
    {
        byte[] encrypted;

        using (AesManaged aes = new AesManaged())
        {
            ICryptoTransform encryptor = aes.CreateEncryptor(Key, IV);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                }
                encrypted = ms.ToArray();
            }
        }

        return Convert.ToBase64String(encrypted);
    }
    private string CreateRandomAlphanumericString(int size)
    {
        char[] allowedChars = "0123456789".ToCharArray();
        byte[] bytes = new byte[size];
        using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
        {
            crypto.GetNonZeroBytes(bytes);
        }

        StringBuilder retVal = new StringBuilder(size);
        foreach (byte b in bytes)
        {
            retVal.Append(allowedChars[b % allowedChars.Length]);
        }

        return retVal.ToString();
    }
}
public class GetMsg23
{
    private string _Error;

    public string Response
    {
        get { return _Error; }
        set { _Error = value; }
    }
}
