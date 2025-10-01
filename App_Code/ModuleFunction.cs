using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.UI.WebControls;
using System.Net;

public class ModuleFunction
{
    private DataTable dt;
    private DAL objDal;
    public string EncodeBase64(string data)
    {
        string s = data.Trim().Replace(" ", "+");
        if ((s.Length % 4) > 0)
        {
            s = s.PadRight(s.Length + (4 - (s.Length % 4)), '=');
        }
        return s;
    }

    public void FillCombo(string qry, ref DropDownList ddlToBeFill, string dataText = "", string dataValue = "")
    {
        dt = new DataTable();
        dt = objDal.GetData(qry);
        ddlToBeFill.DataSource = dt;
        ddlToBeFill.DataTextField = dataText;
        ddlToBeFill.DataValueField = dataValue;
        ddlToBeFill.DataBind();
    }

    public ModuleFunction()
    {
        objDal = new DAL();
    }

    /// <summary>
    /// Method to get Client ip address
    /// </summary>
    /// <param name="GetLan"> Set to true if want to get local(LAN) Connected ip address</param>
    /// <returns></returns>
    public string GetVisitorIPAddress(bool GetLan = false)
    {
        string visitorIPAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

        if (string.IsNullOrEmpty(visitorIPAddress))
        {
            visitorIPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        }

        if (string.IsNullOrEmpty(visitorIPAddress))
        {
            visitorIPAddress = HttpContext.Current.Request.UserHostAddress;
        }

        if (string.IsNullOrEmpty(visitorIPAddress) || visitorIPAddress.Trim() == "::1")
        {
            GetLan = true;
            visitorIPAddress = string.Empty;
        }

        if (GetLan)
        {
            if (string.IsNullOrEmpty(visitorIPAddress))
            {
                // This is for Local(LAN) Connected ID Address
                string stringHostName = Dns.GetHostName();
                // Get Ip Host Entry
                IPHostEntry ipHostEntries = Dns.GetHostEntry(stringHostName);
                // Get Ip Address From The Ip Host Entry Address List
                IPAddress[] arrIpAddress = ipHostEntries.AddressList;

                try
                {
                    visitorIPAddress = arrIpAddress[arrIpAddress.Length - 2].ToString();
                }
                catch
                {
                    try
                    {
                        visitorIPAddress = arrIpAddress[0].ToString();
                    }
                    catch
                    {
                        try
                        {
                            arrIpAddress = Dns.GetHostAddresses(stringHostName);
                            visitorIPAddress = arrIpAddress[0].ToString();
                        }
                        catch
                        {
                            visitorIPAddress = "127.0.0.1";
                        }
                    }
                }
            }
        }
        return visitorIPAddress;
    }
}
