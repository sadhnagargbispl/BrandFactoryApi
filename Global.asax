<%@ Application Language="C#" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Data" %>

<script RunAt="server">
    protected void Application_Start(object sender, EventArgs e)
    {
        // Code that runs on application startup

        //Application["InvDB"] = "KordyInv";
        //Application["Connect"] = "Server=164.132.18.172;UID=ubgshp;PWD=S#09B!81ee$;Database=BigShopee;Pooling=False;Connect Timeout=100000000;";

        Application["Connect"] = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        Application["Connect1"] = ConfigurationManager.ConnectionStrings["constr1"].ConnectionString;
    }

    protected void Application_End(object sender, EventArgs e)
    {
        // Code that runs on application shutdown
    }

    protected void Application_Error(object sender, EventArgs e)
    {
        // Code that runs when an unhandled error occurs
    }

    protected void Session_Start(object sender, EventArgs e)
    {
        // Code that runs when a new session is started

        //getData();
    }

    protected void Session_End(object sender, EventArgs e)
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.
    }
    //protected void Application_BeginRequest(Object sender, EventArgs e)
    //{
    //    if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
    //    {
    //        HttpContext.Current.Response.StatusCode = 200;
    //        HttpContext.Current.Response.End();
    //    }

    //    HttpContext.Current.Response.Headers.Set("Access-Control-Allow-Origin", "*");
    //    HttpContext.Current.Response.Headers.Set("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
    //    HttpContext.Current.Response.Headers.Set("Access-Control-Allow-Headers", "Content-Type, Authorization");
    //}


    //protected void getData()
    //{
    //    cls_DataAccess dbConnect = new cls_DataAccess(Application["Connect1"].ToString());
    //    DAL objdal = new DAL();
    //    try
    //    {
    //        SqlDataReader dRead;
    //        SqlCommand cmd;
    //        DataTable dtCompany = new DataTable();
    //        if (Application["dtCompany"] == null)
    //        {
    //            if (dbConnect.cnnObject == null)
    //            {
    //                dbConnect.OpenConnection();
    //            }
    //            DataSet ds = new DataSet();
    //            SqlDataAdapter adp = new SqlDataAdapter();
    //            string strQ = objdal.Isostart + " select * from " + objdal.dBName + " ..M_CompanyMaster" + objdal.IsoEnd;
    //            adp = new SqlDataAdapter(strQ, dbConnect.cnnObject);
    //            adp.Fill(ds);
    //            dtCompany = ds.Tables[0];
    //            Application["dtCompany"] = dtCompany;
    //        }
    //        else
    //        {
    //            if (dbConnect.cnnObject == null)
    //            {
    //                dbConnect.OpenConnection();
    //            }
    //            DataSet ds = new DataSet();
    //            SqlDataAdapter adp = new SqlDataAdapter();
    //            string strQ = objdal.Isostart + " select * from " + objdal.dBName + " ..M_CompanyMaster" + objdal.IsoEnd;
    //            adp = new SqlDataAdapter(strQ, dbConnect.cnnObject);
    //            adp.Fill(ds);
    //            dtCompany = ds.Tables[0];
    //            Application["dtCompany"] = dtCompany;
    //        }

    //        if (dtCompany.Rows.Count > 0)
    //        {
    //            Session["CompName"] = dtCompany.Rows[0]["CompName"];
    //            Session["CompAdd"] = dtCompany.Rows[0]["CompAdd"];
    //            Session["CompWeb"] = string.IsNullOrEmpty(dtCompany.Rows[0]["WebSite"].ToString()) ? "index.asp" : dtCompany.Rows[0]["WebSite"];
    //            Session["Title"] = dtCompany.Rows[0]["CompTitle"];
    //            Session["CompMail"] = dtCompany.Rows[0]["CompMail"];
    //            Session["CompMobile"] = dtCompany.Rows[0]["MobileNo"];
    //            Session["ClientId"] = dtCompany.Rows[0]["smsSenderId"];
    //            Session["SmsId"] = dtCompany.Rows[0]["smsUserNm"];
    //            Session["SmsPass"] = dtCompany.Rows[0]["smPass"];
    //            Session["MailPass"] = dtCompany.Rows[0]["mailPass"];
    //            Session["MailHost"] = dtCompany.Rows[0]["mailHost"];
    //            Session["AdminWeb"] = dtCompany.Rows[0]["AdminWeb"];
    //            Session["CompCST"] = dtCompany.Rows[0]["CompCSTNo"];
    //            Session["CompState"] = dtCompany.Rows[0]["CompState"];
    //            Session["CompDate"] = Convert.ToDateTime(dtCompany.Rows[0]["RecTimeStamp"]).ToString("dd-MMM-yyyy");
    //            Session["Spons"] = "KL223344";
    //            Session["CompWeb1"] = dtCompany.Rows[0]["WebSite"];
    //            Session["CompMovieWeb"] = "";
    //            Session["SmsAPI"] = "";
    //            Session["CompShortUrl"] = dtCompany.Rows[0]["UrlShort"];
    //            Session["LogoUrl"] = dtCompany.Rows[0]["LogoUrl"];
    //        }
    //        else
    //        {
    //            Session["CompName"] = "";
    //            Session["CompAdd"] = "";
    //            Session["CompWeb"] = "";
    //            Session["Title"] = "Welcome";
    //        }

    //        DataTable dtConfig = new DataTable();
    //        if (Application["dtConfig"] == null)
    //        {
    //            if (dbConnect.cnnObject == null)
    //            {
    //                dbConnect.OpenConnection();
    //            }
    //            string strQ = objdal.Isostart + " select * from " + objdal.dBName + "..M_ConfigMaster " + objdal.IsoEnd;
    //            DataSet ds = new DataSet();
    //            SqlDataAdapter adp = new SqlDataAdapter(strQ, dbConnect.cnnObject);
    //            adp.Fill(ds);
    //            dtConfig = ds.Tables[0];
    //            Application["dtConfig"] = dtConfig;
    //        }
    //        else
    //        {
    //            dtConfig = (DataTable)Application["dtConfig"];
    //        }

    //        if (dtConfig.Rows.Count > 0)
    //        {
    //            Session["IsGetExtreme"] = dtConfig.Rows[0]["IsGetExtreme"];
    //            Session["IsTopUp"] = dtConfig.Rows[0]["IsTopUp"];
    //            Session["IsSendSMS"] = dtConfig.Rows[0]["IsSendSMS"];
    //            Session["IdNoPrefix"] = dtConfig.Rows[0]["IdNoPrefix"];
    //            Session["IsFreeJoin"] = dtConfig.Rows[0]["IsFreeJoin"];
    //            Session["IsStartJoin"] = dtConfig.Rows[0]["IsStartJoin"];
    //            Session["JoinStartFrm"] = dtConfig.Rows[0]["JoinStartFrm"];
    //            Session["IsSubPlan"] = dtConfig.Rows[0]["IsSubPlan"];
    //            Session["Logout"] = dtConfig.Rows[0]["LogoutPg"];
    //        }
    //        else
    //        {
    //            Session["IsGetExtreme"] = "N";
    //            Session["IsTopUp"] = "N";
    //            Session["IsSendSMS"] = "N";
    //            Session["IdNoPrefix"] = "";
    //            Session["IsFreeJoin"] = "N";
    //            Session["IsStartJoin"] = "N";
    //            Session["JoinStartFrm"] = "01-Sep-2011";
    //            Session["IsSubPlan"] = "N";
    //            Session["Logout"] = "https://djiomart.com/";
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        // handle exception
    //    }
    //    DataTable dtMsession = new DataTable();
    //    if (Application["dtMsession"] == null)
    //    {
    //        if (dbConnect.cnnObject == null)
    //        {
    //            dbConnect.OpenConnection();
    //        }
    //        DataSet ds = new DataSet();
    //        SqlDataAdapter adp = new SqlDataAdapter();
    //        string strQ = objdal.Isostart + " select Max(SEssid) as SessID from " + objdal.dBName + "..D_Monthlypaydetail  " + objdal.IsoEnd;
    //        adp = new SqlDataAdapter(strQ, dbConnect.cnnObject);
    //        adp.Fill(ds);
    //        dtMsession = ds.Tables[0];
    //        Application["dtMsession"] = dtMsession;
    //    }
    //    else
    //    {
    //        dtMsession = (DataTable)Application["dtMsession"];
    //    }

    //    if (dtMsession.Rows.Count > 0)
    //    {
    //        Session["MaxSessn"] = dtMsession.Rows[0]["SessID"];
    //    }
    //    else
    //    {
    //        Session["MaxSessn"] = "";
    //    }

    //    DataTable dtsession = new DataTable();
    //    if (Application["dtsession"] == null)
    //    {
    //        if (dbConnect.cnnObject == null)
    //        {
    //            dbConnect.OpenConnection();
    //        }
    //        DataSet ds = new DataSet();
    //        SqlDataAdapter adp = new SqlDataAdapter();
    //        string strQ = objdal.Isostart + " select Max(SEssid) as SessID from " + objdal.dBName + "..m_SessnMaster  " + objdal.IsoEnd;
    //        adp = new SqlDataAdapter(strQ, dbConnect.cnnObject);
    //        adp.Fill(ds);

    //        dtsession = ds.Tables[0];
    //        Application["dtsession"] = dtsession;
    //    }
    //    else
    //    {
    //        dtsession = (DataTable)Application["dtsession"];
    //    }

    //    if (dtsession.Rows.Count > 0)
    //    {
    //        Session["CurrentSessn"] = dtsession.Rows[0]["SessID"];
    //    }
    //    else
    //    {
    //        Session["CurrentSessn"] = "";
    //    }
    //    if (dbConnect.cnnObject != null)
    //    {
    //        if (dbConnect.cnnObject.State == ConnectionState.Open)
    //        {
    //            dbConnect.cnnObject.Close();
    //        }
    //    }

    //}


</script>
