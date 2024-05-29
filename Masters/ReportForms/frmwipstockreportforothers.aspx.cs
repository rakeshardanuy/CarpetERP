﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.IO;

public partial class Masters_ReportForms_frmwipstockreportforothers : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["varcompanyId"] == null)
        {
            Response.Redirect("~/Login.aspx");
        }
        if (!IsPostBack)
        {
            string str = @"select Distinct CI.CompanyId,CI.CompanyName from Companyinfo CI,Company_Authentication CA Where CI.CompanyId=CA.CompanyId And CA.UserId=" + Session["varuserId"] + "  And CI.MasterCompanyId=" + Session["varCompanyId"] + @" Order By CompanyName                                                      
                           select Distinct ICM.CATEGORY_ID,ICM.CATEGORY_NAME from ITEM_CATEGORY_MASTER ICM inner join CategorySeparate cs on ICM.CATEGORY_ID=cs.Categoryid and cs.id=0 and ICM.MasterCompanyid=" + Session["varcompanyid"] + @"
                           select Val,Type from Sizetype
                           Select CustomerId,CustomerCode From CustomerInfo Where MasterCompanyId=" + Session["varCompanyId"] + " Order By CustomerCode";

            DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, str);

            UtilityModule.ConditionalComboFillWithDS(ref DDcompany, ds, 0, false, "");
            UtilityModule.ConditionalComboFillWithDS(ref DDCategory, ds, 1, true, "--Plz Select--");
            UtilityModule.ConditionalComboFillWithDS(ref DDsizetype, ds, 2, false, "--Plz Select--");
            UtilityModule.ConditionalComboFillWithDS(ref DDCustCode, ds, 3, true, "--Select--");
            if (DDcompany.Items.Count > 0)
            {
                DDcompany.SelectedValue = Session["CurrentWorkingCompanyID"].ToString();
                DDcompany.Enabled = false;
            }
        }
    }
    protected void DDCategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlcategorycange();
    }
    private void ddlcategorycange()
    {
        DDQuality.Items.Clear();
        DDDesign.Items.Clear();
        DDColor.Items.Clear();
        DDShape.Items.Clear();
        DDSize.Items.Clear();
        TRDDQuality.Visible = false;
        TRDDDesign.Visible = false;
        TRDDColor.Visible = false;
        TRDDShape.Visible = false;
        TRDDSize.Visible = false;
        string strsql = @"SELECT [CATEGORY_PARAMETERS_ID],[CATEGORY_ID],IPM.[PARAMETER_ID],PARAMETER_NAME 
                      FROM [ITEM_CATEGORY_PARAMETERS] IPM inner join PARAMETER_MASTER PM on 
                      IPM.[PARAMETER_ID]=PM.[PARAMETER_ID] where [CATEGORY_ID]=" + DDCategory.SelectedValue + " And PM.MasterCompanyId=" + Session["varCompanyId"];
        DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, strsql);
        if (ds.Tables[0].Rows.Count > 0)
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {

                switch (dr["PARAMETER_ID"].ToString())
                {
                    case "1":
                        TRDDQuality.Visible = true;
                        break;
                    case "2":
                        TRDDDesign.Visible = true;
                        break;
                    case "3":
                        TRDDColor.Visible = true;
                        break;
                    case "6":
                        //TDIColorShade.Visible = true;
                        break;
                    case "4":
                        TRDDShape.Visible = true;
                        break;
                    case "5":
                        TRDDSize.Visible = true;
                        break;
                    case "10":
                        //TDIcolor.Visible = true;
                        break;
                }
            }
        }

        string stritem = "select distinct IM.Item_Id,IM.Item_Name from  Item_Parameter_Master IPM  inner Join Item_Master IM on IM.Item_Id=IPM.Item_Id inner join Item_Category_Master ICM on ICM.Category_Id=IM.Category_Id where  IM.Category_Id=" + DDCategory.SelectedValue + " And IM.MasterCompanyId=" + Session["varCompanyId"] + " order by IM.item_name";
        UtilityModule.ConditionalComboFill(ref ddItemName, stritem, true, "---Select Item----");
    }
    private void QDCSDDFill(DropDownList Quality, DropDownList Design, DropDownList Color, DropDownList Shape, int Itemid)
    {
        string Str = @"SELECT QUALITYID,QUALITYNAME FROM QUALITY WHERE ITEM_ID=" + Itemid + " And MasterCompanyId=" + Session["varCompanyId"] + @" Order By QUALITYNAME
                     SELECT DESIGNID,DESIGNNAME from DESIGN Where  MasterCompanyId=" + Session["varCompanyId"] + @" Order By DESIGNNAME
                     SELECT COLORID,COLORNAME FROM COLOR Where  MasterCompanyId=" + Session["varCompanyId"] + @" Order By COLORNAME
                     SELECT SHAPEID,SHAPENAME FROM SHAPE Where  MasterCompanyId=" + Session["varCompanyId"] + @" Order By SHAPENAME
                     SELECT SHADECOLORID,SHADECOLORNAME FROM SHADECOLOR Where  MasterCompanyId=" + Session["varCompanyId"] + " Order By SHADECOLORNAME";

        DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, Str);

        UtilityModule.ConditionalComboFillWithDS(ref Quality, ds, 0, true, "--SELECT--");
        UtilityModule.ConditionalComboFillWithDS(ref Design, ds, 1, true, "--SELECT--");
        UtilityModule.ConditionalComboFillWithDS(ref Color, ds, 2, true, "--SELECT--");
        UtilityModule.ConditionalComboFillWithDS(ref Shape, ds, 3, true, "--SELECT--");
    }
    protected void ddItemName_SelectedIndexChanged(object sender, EventArgs e)
    {
        QDCSDDFill(DDQuality, DDDesign, DDColor, DDShape, Convert.ToInt16(ddItemName.SelectedValue));
    }
    protected void DDShape_SelectedIndexChanged(object sender, EventArgs e)
    {
        FillSize();
    }
    protected void FillSize()
    {
        string size = "";
        string str = "";

        switch (DDsizetype.SelectedValue)
        {
            case "1":
                size = "Sizemtr";
                break;
            case "0":
                size = "Sizeft";
                break;
            case "2":
                size = "Sizeinch";
                break;
            default:
                size = "Sizeft";
                break;
        }
        //size Query

        str = "Select Distinct S.Sizeid,S." + size + " As  " + size + @" From Size S 
                 Where shapeid=" + DDShape.SelectedValue + " And S.MasterCompanyId=" + Session["varCompanyId"] + " order by " + size + "";

        UtilityModule.ConditionalComboFill(ref DDSize, str, true, "--SELECT--");
        //

    }
    protected void DDsizetype_SelectedIndexChanged(object sender, EventArgs e)
    {
        FillSize();
    }
    protected void btnPreview_Click(object sender, EventArgs e)
    {
        if (chkWIPdetail.Checked == true)
        {
            if (Session["VarCompanyNo"].ToString() == "38")
            {
                WIPDetailVikramKM();
            }
            else
            {
                WIPDetail();
            }
        }
        else
        {
            WIPsummary();
        }
    }
    protected void WIPsummary()
    {
        string where = "";
        lblmsg.Text = "";
        if (DDCategory.SelectedIndex > 0)
        {
            where = where + " and vf.CATEGORY_ID=" + DDCategory.SelectedValue;
        }
        if (ddItemName.SelectedIndex > 0)
        {
            where = where + " and vf.Item_id=" + ddItemName.SelectedValue;
        }
        if (DDQuality.SelectedIndex > 0)
        {
            where = where + " and vf.qualityid=" + DDQuality.SelectedValue;
        }
        if (DDDesign.SelectedIndex > 0)
        {
            where = where + " and vf.Designid=" + DDDesign.SelectedValue;
        }
        if (DDColor.SelectedIndex > 0)
        {
            where = where + " and vf.colorid=" + DDColor.SelectedValue;
        }
        if (DDShape.SelectedIndex > 0)
        {
            where = where + " and vf.shapeid=" + DDShape.SelectedValue;
        }
        if (DDSize.SelectedIndex > 0)
        {
            where = where + " and vf.sizeid=" + DDSize.SelectedValue;
        }
        SqlConnection con = new SqlConnection(ErpGlobal.DBCONNECTIONSTRING);
        if (con.State == ConnectionState.Closed)
        {
            con.Open();
        }
        SqlCommand cmd = new SqlCommand();
        if (Session["varcompanyId"].ToString() == "44")
        {
            cmd.CommandText = "Pro_getWipreportforothers_agni";
            cmd.Connection = con;
        }
        else
        {
            cmd.CommandText = "Pro_getWipreportforothers";
            cmd.Connection = con;
        
        }
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = 300;
        //********
        cmd.Parameters.AddWithValue("@companyId", DDcompany.SelectedValue);
        cmd.Parameters.AddWithValue("@where", where);
        cmd.Parameters.AddWithValue("@customerid", DDCustCode.SelectedIndex > 0 ? DDCustCode.SelectedValue : "0");
        cmd.Parameters.AddWithValue("@orderid", DDOrderNo.SelectedIndex > 0 ? DDOrderNo.SelectedValue : "0");
        DataTable dt = new DataTable();
        dt.Load(cmd.ExecuteReader());
        //*************
        DataSet ds = new DataSet();
        ds.Tables.Add(dt);

        ds.Tables[0].Columns.Remove("Finishedid");
        ds.Tables[0].Columns.Remove("Orderid");
        ds.Tables[0].Columns.Remove("CustomerorderNo");
        ds.Tables[0].Columns.Remove("Flagsize");
        if (ds.Tables[0].Rows.Count > 0)
        {
            //Export to excel
            GridView GridView1 = new GridView();
            GridView1.AllowPaging = false;

            GridView1.DataSource = ds;
            GridView1.DataBind();
            lblmsg.Text = "Wait....";
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition",
             "attachment;filename=WIPStock" + DateTime.Now + ".xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                //Apply text style to each Row
                GridView1.Rows[i].Attributes.Add("class", "textmode");
            }
            GridView1.RenderControl(hw);

            //style to format numbers to string
            string style = @"<style> .textmode { mso-number-format:\@; } </style>";
            Response.Write(style);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            lblmsg.Text = "Done.....";
            //*************

        }
        else
        {
            lblmsg.Text = "No Records found...";
        }
    }
    protected void WIPDetail()
    {
        string where = "";
        lblmsg.Text = "";
        if (DDCategory.SelectedIndex > 0)
        {
            where = where + " and vf.CATEGORY_ID=" + DDCategory.SelectedValue;
        }
        if (ddItemName.SelectedIndex > 0)
        {
            where = where + " and vf.Item_id=" + ddItemName.SelectedValue;
        }
        if (DDQuality.SelectedIndex > 0)
        {
            where = where + " and vf.qualityid=" + DDQuality.SelectedValue;
        }
        if (DDDesign.SelectedIndex > 0)
        {
            where = where + " and vf.Designid=" + DDDesign.SelectedValue;
        }
        if (DDColor.SelectedIndex > 0)
        {
            where = where + " and vf.colorid=" + DDColor.SelectedValue;
        }
        if (DDShape.SelectedIndex > 0)
        {
            where = where + " and vf.shapeid=" + DDShape.SelectedValue;
        }
        if (DDSize.SelectedIndex > 0)
        {
            where = where + " and vf.sizeid=" + DDSize.SelectedValue;
        }
        SqlConnection con = new SqlConnection(ErpGlobal.DBCONNECTIONSTRING);
        if (con.State == ConnectionState.Closed)
        {
            con.Open();
        }
        SqlCommand cmd = new SqlCommand("PRO_WIPDETAILSFOROTHERS", con);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = 3000;
        //********
        cmd.Parameters.AddWithValue("@companyId", DDcompany.SelectedValue);
        cmd.Parameters.AddWithValue("@where", where);
        cmd.Parameters.AddWithValue("@customerid", DDCustCode.SelectedIndex > 0 ? DDCustCode.SelectedValue : "0");
        cmd.Parameters.AddWithValue("@orderid", DDOrderNo.SelectedIndex > 0 ? DDOrderNo.SelectedValue : "0");
        DataTable dt = new DataTable();
        dt.Load(cmd.ExecuteReader());
        //*************
        DataSet ds = new DataSet();
        ds.Tables.Add(dt);

        //SqlParameter[] param = new SqlParameter[2];
        //param[0] = new SqlParameter("@companyId", DDcompany.SelectedValue);
        //param[1] = new SqlParameter("@where", where);
        //*************
        //DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.StoredProcedure, "Pro_Wipdetails", param);
        ds.Tables[0].Columns.Remove("Stockno");
        ds.Tables[0].Columns.Remove("SeqNo");
        //Export to excel
        if (ds.Tables[0].Rows.Count > 0)
        {
            GridView GridView1 = new GridView();
            GridView1.AllowPaging = false;

            GridView1.DataSource = ds;
            GridView1.DataBind();
            lblmsg.Text = "Wait....";
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition",
             "attachment;filename=WIPStockDetail" + DateTime.Now + ".xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                //Apply text style to each Row
                GridView1.Rows[i].Attributes.Add("class", "textmode");
            }
            GridView1.RenderControl(hw);

            //style to format numbers to string
            string style = @"<style> .textmode { mso-number-format:\@; } </style>";
            Response.Write(style);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            lblmsg.Text = "Done.....";
            //*************
        }
        else
        {
            lblmsg.Text = "No records found..";
        }
    }
    protected void WIPDetailVikramKM()
    {
        string where = "";
        lblmsg.Text = "";
        if (DDCategory.SelectedIndex > 0)
        {
            where = where + " and vf.CATEGORY_ID=" + DDCategory.SelectedValue;
        }
        if (ddItemName.SelectedIndex > 0)
        {
            where = where + " and vf.Item_id=" + ddItemName.SelectedValue;
        }
        if (DDQuality.SelectedIndex > 0)
        {
            where = where + " and vf.qualityid=" + DDQuality.SelectedValue;
        }
        if (DDDesign.SelectedIndex > 0)
        {
            where = where + " and vf.Designid=" + DDDesign.SelectedValue;
        }
        if (DDColor.SelectedIndex > 0)
        {
            where = where + " and vf.colorid=" + DDColor.SelectedValue;
        }
        if (DDShape.SelectedIndex > 0)
        {
            where = where + " and vf.shapeid=" + DDShape.SelectedValue;
        }
        if (DDSize.SelectedIndex > 0)
        {
            where = where + " and vf.sizeid=" + DDSize.SelectedValue;
        }
        SqlConnection con = new SqlConnection(ErpGlobal.DBCONNECTIONSTRING);
        if (con.State == ConnectionState.Closed)
        {
            con.Open();
        }
        SqlCommand cmd = new SqlCommand("PRO_WIPDETAILSFOROTHERS_VikramKhamaria", con);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = 3000;
        //********
        cmd.Parameters.AddWithValue("@companyId", DDcompany.SelectedValue);
        cmd.Parameters.AddWithValue("@where", where);
        cmd.Parameters.AddWithValue("@customerid", DDCustCode.SelectedIndex > 0 ? DDCustCode.SelectedValue : "0");
        cmd.Parameters.AddWithValue("@orderid", DDOrderNo.SelectedIndex > 0 ? DDOrderNo.SelectedValue : "0");
        DataTable dt = new DataTable();
        dt.Load(cmd.ExecuteReader());
        //*************
        DataSet ds = new DataSet();
        ds.Tables.Add(dt);

        //SqlParameter[] param = new SqlParameter[2];
        //param[0] = new SqlParameter("@companyId", DDcompany.SelectedValue);
        //param[1] = new SqlParameter("@where", where);
        //*************
        //DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.StoredProcedure, "Pro_Wipdetails", param);
        ds.Tables[0].Columns.Remove("Stockno");
        ds.Tables[0].Columns.Remove("SeqNo");
        //Export to excel
        if (ds.Tables[0].Rows.Count > 0)
        {
            GridView GridView1 = new GridView();
            GridView1.AllowPaging = false;

            GridView1.DataSource = ds;
            GridView1.DataBind();
            lblmsg.Text = "Wait....";
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition",
             "attachment;filename=WIPStockDetail" + DateTime.Now + ".xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                //Apply text style to each Row
                GridView1.Rows[i].Attributes.Add("class", "textmode");
            }
            GridView1.RenderControl(hw);

            //style to format numbers to string
            string style = @"<style> .textmode { mso-number-format:\@; } </style>";
            Response.Write(style);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            lblmsg.Text = "Done.....";
            //*************
        }
        else
        {
            lblmsg.Text = "No records found..";
        }
    }
    protected void DDCustCode_SelectedIndexChanged(object sender, EventArgs e)
    {
        string orderNo = "CustomerOrderNo";
        if (Session["varcompanyId"].ToString() == "9")
        {
            orderNo = "Localorder + '#' + CustomerorderNo ";
        }
        string Str = @"Select OrderId," + orderNo + " as CustomerOrderNo From OrderMaster where CustomerId=" + DDCustCode.SelectedValue + " And CompanyId=" + DDcompany.SelectedValue + " Order By CustomerOrderNo";
        if (Session["varcompanyId"].ToString() == "16" || Session["varcompanyId"].ToString() == "28")
        {
            Str = @"Select OrderId," + orderNo + " as CustomerOrderNo From OrderMaster where Status = 0 And CustomerId=" + DDCustCode.SelectedValue + " And CompanyId=" + DDcompany.SelectedValue + " Order By CustomerOrderNo";
        }
        UtilityModule.ConditionalComboFill(ref DDOrderNo, Str, true, "--Select--");
    }
}