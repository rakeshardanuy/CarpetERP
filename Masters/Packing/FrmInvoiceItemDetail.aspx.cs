﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
public partial class Masters_Packing_FrmInvoiceItemDetail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["varCompanyId"] == null)
        {
            Response.Redirect("~/Login.aspx");
        }
        if (IsPostBack == false)
        {
            TxtInvoiceId.Text = Request.QueryString["ID"];
            Fill_Grid();
        }
    }
    private void Fill_Grid()
    {
        GDItemDetail.DataSource = Fill_Grid_Data();
        GDItemDetail.DataBind();
    }

    private DataSet Fill_Grid_Data()
    {
        DataSet ds = null;
        SqlConnection con = new SqlConnection(ErpGlobal.DBCONNECTIONSTRING);
        try
        {
            string strsql = @"Select P.PackingId Sr_No,PI.Quality as Qualityname,Sum(Pcs) Pcs,Sum(Area) Area,Price from Packing P,PackingInformation PI,V_FinishedItemDetail VF 
                             Where P.PackingId=PI.PackingId And PI.Finishedid=VF.Item_Finished_id And P.PackingId=" + TxtInvoiceId.Text + " And P.MasterCompanyId=" + Session["varCompanyId"] + @"
                             Group By P.PackingId,PI.Quality,Price";
            con.Open();
            ds = SqlHelper.ExecuteDataset(con, CommandType.Text, strsql);
        }
        catch(Exception ex)
        {
            UtilityModule.MessageAlert(ex.Message, "Master/Packing/FrmInvoiceItemDetail.aspx");
            Logs.WriteErrorLog("Masters_Packing_FrmInvoiceItemDetail|Fill_Grid_Data|" + ex.Message);
        }
        finally
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
                con.Dispose();
            }
        }
        return ds;
    }
    protected void GDItemDetail_RowCreated(object sender, GridViewRowEventArgs e)
    {
        //Add CSS class on header row.
        if (e.Row.RowType == DataControlRowType.Header)
            e.Row.CssClass = "header";

        //Add CSS class on normal row.
        if (e.Row.RowType == DataControlRowType.DataRow &&
                  e.Row.RowState == DataControlRowState.Normal)
            e.Row.CssClass = "normal";

        //Add CSS class on alternate row.
        if (e.Row.RowType == DataControlRowType.DataRow &&
                  e.Row.RowState == DataControlRowState.Alternate)
            e.Row.CssClass = "alternate";
    }
}