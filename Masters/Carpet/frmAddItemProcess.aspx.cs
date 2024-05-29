﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

public partial class Masters_Process_frmAddItemProcess : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["varcompanyId"] == null)
        {
            Response.Redirect("~/Login.aspx");
        }
        if (!IsPostBack)
        {
            UtilityModule.ConditonalListFill(ref lstProcess, "select Process_name_Id,Process_Name from Process_name_Master order by Process_Name");
            lblItemName.Text = SqlHelper.ExecuteScalar(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, "select Item_Name from Item_master Where Item_id=" + Request.QueryString["a"] + " And MasterCompanyId=" + Session["varcompanyId"] + "").ToString();
            switch (Session["varcompanyid"].ToString())
            {
                case "8":
                    TDquality.Visible = false;
                    Fillselectprocess();
                    break;
                default:
                    TDquality.Visible = true;
                    TDDesign.Visible = true;
                    break;
            }
            //
            if (TDquality.Visible == true)
            {
                UtilityModule.ConditionalComboFill(ref DDQuality, "select QualityId,QualityName From Quality Where Item_Id=" + Request.QueryString["a"] + " order by QualityName", true, "--Plz Select--");
            }
        }
    }
    protected void Fillselectprocess()
    {
        string str = @"select PNM.process_Name_id,PNM.Process_Name from Process_name_Master PNM,Item_Process IP
                      Where PNM.Process_name_id=IP.ProcessId And ItemId=" + Request.QueryString["a"] + " And PNM.MasterCompanyid=" + Session["varcompanyid"] + "";
        if (DDQuality.SelectedIndex > 0)
        {
            str = str + " and IP.QualityId=" + DDQuality.SelectedValue;
        }
        if (DDDesign.SelectedIndex > 0)
        {
            str = str + " and IP.DesignId=" + DDDesign.SelectedValue;
        }
        else
        {
            str = str + " and IP.DesignId=0";
        }
        str = str + "  order by IP.SeqNo";
        UtilityModule.ConditonalListFill(ref lstSelectProcess, str);
    }
    protected void btngo_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < lstProcess.Items.Count; i++)
        {
            if (lstProcess.Items[i].Selected)
            {
                //Check if process Already Exists
                if (!lstSelectProcess.Items.Contains(lstProcess.Items[i]))
                {
                    lstSelectProcess.Items.Add(new ListItem(lstProcess.Items[i].Text, lstProcess.Items[i].Value));
                }
            }
        }
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        List<ListItem> lstselected = new List<ListItem>();

        foreach (ListItem liItems in lstSelectProcess.Items)
        {
            if (liItems.Selected == true)
            {
                lstselected.Add(liItems);
            }
        }

        //3. Loop through the List "lstSelected" and
        // remove ListItems from ListBox "lstSelectProcess" that are in 
        // lstSelected List
        foreach (ListItem liSelected in lstselected)
        {
            lstSelectProcess.Items.Remove(liSelected);
        }

    }
    protected void btnsave_Click(object sender, EventArgs e)
    {
        SqlConnection con = new SqlConnection(ErpGlobal.DBCONNECTIONSTRING);
        if (con.State == ConnectionState.Closed)
        {
            con.Open();
        }
        SqlTransaction Tran = con.BeginTransaction();
        try
        {
            SqlParameter[] array = new SqlParameter[7];
            array[0] = new SqlParameter("@Itemid", SqlDbType.Int);
            array[1] = new SqlParameter("@UserId", SqlDbType.Int);
            array[2] = new SqlParameter("@MasterCompanyId", SqlDbType.Int);
            array[3] = new SqlParameter("@ProcessId_SeqNO", SqlDbType.VarChar, 100);
            array[4] = new SqlParameter("@Msg", SqlDbType.VarChar, 50);
            array[5] = new SqlParameter("@QualityId", SqlDbType.Int);
            array[6] = new SqlParameter("@DesignId", SqlDbType.Int);

            array[0].Value = Request.QueryString["a"];
            array[1].Value = Session["varuserid"];
            array[2].Value = Session["varcompanyId"];

            string str = "";
            string strnew = "";
            // find ProcessId And SeqNo
            int seqNo = 0;
            for (int i = 0; i < lstSelectProcess.Items.Count; i++)
            {

                seqNo += 1;
                str = lstSelectProcess.Items[i].Value + "," + seqNo;
                if (strnew == "")
                {
                    strnew = str;
                }
                else
                {
                    strnew = strnew + "|" + str;

                }

            }
            array[3].Value = strnew;
            array[4].Direction = ParameterDirection.Output;
            array[5].Value = TDquality.Visible == false ? "0" : DDQuality.SelectedValue;
            array[6].Value = TDDesign.Visible == false ? "0" : (DDDesign.SelectedIndex > 0 ? DDDesign.SelectedValue : "0");
            //Save Data
            SqlHelper.ExecuteNonQuery(Tran, CommandType.StoredProcedure, "Pro_SaveItem_Process", array);
            Tran.Commit();
            lblMessage.Text = array[4].Value.ToString();
        }
        catch (Exception ex)
        {
            Tran.Rollback();
            lblMessage.Text = ex.Message;
        }
        finally
        {
            con.Dispose();
            con.Close();
        }
    }
    protected void DDQuality_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (TDDesign.Visible == true)
        {
            FillDesign();
        }

        Fillselectprocess();
    }

    private void FillDesign()
    {
        string str = "select Distinct designId,designName From V_FinisheditemDetail  Where QualityId=" + DDQuality.SelectedValue + " order by designName";
        UtilityModule.ConditionalComboFill(ref DDDesign, str, true, "--Plz Select--");
    }

    protected void DDDesign_SelectedIndexChanged(object sender, EventArgs e)
    {
        Fillselectprocess();
    }
}