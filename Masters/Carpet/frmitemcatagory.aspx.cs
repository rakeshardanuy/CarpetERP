﻿using System;using CarpetERP.Core.DAL;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

public partial class frmitemcatagory : CustomPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["varCompanyId"] == null)
        {
            Response.Redirect("~/Login.aspx");
        }
        if (IsPostBack == false)
        {
            switch (Session["varCompanyNo"].ToString())
            {
                case "3":
                    tdHSCode.Visible = true;
                    tdtxtHSCode.Visible = true;
                    break;
                default:
                    tdHSCode.Visible = false;
                    tdtxtHSCode.Visible = false;
                    break;
            }
            lablechange();
            fill_grid();
            txtcatagory.Focus();
        }
        Lblerrer.Visible = false;
    }

    public void lablechange()
    {
        String[] ParameterList = new String[8];
        ParameterList = UtilityModule.ParameteLabel(Convert.ToInt32(Session["varCompanyId"]));
        lblqualityname1.Text = ParameterList[0];
        lbldesignname.Text = ParameterList[1];
        lblcolorname.Text = ParameterList[2];
        lblshapename.Text = ParameterList[3];
        lblsizename.Text = ParameterList[4];
        lblcategoryname.Text = ParameterList[5];
        lblshqadename.Text = ParameterList[7];
        LblCONTENT.Text = ParameterList[8];
        LblDESCRIPTION.Text = ParameterList[9];
        LblPATTERN.Text = ParameterList[10];
        LblFITSIZE.Text = ParameterList[11];
    }
    private void fill_grid()
    {
        gditemcatagory.DataSource = Fill_Grid_Data();
        gditemcatagory.DataBind();
    }
    private DataSet Fill_Grid_Data()
    {
        DataSet ds = null;
        try
        {
            string strsql = @"select CATEGORY_ID as Sr_No,Category_Name as " + lblcategoryname.Text + @",Code "; 
            
            if (tdtxtHSCode.Visible == true)
            {
                strsql = strsql + " , HSCODE ";
            }
            
            strsql = strsql + @"From ITEM_CATEGORY_MASTER(Nolock) 
            Where MasterCompanyId = " + Session["varCompanyId"] + " order by Category_Name ";

            ds = SqlHelper.ExecuteDataset(strsql);
        }
        catch (Exception ex)
        {
            Lblerrer.Visible = true;
            Lblerrer.Text = ex.Message;
        }
        return ds;
    }
    private void Store_Data()
    {
        Validated();
        if (txtcatagory.Text != "")
        {
            SqlConnection con = new SqlConnection(ErpGlobal.DBCONNECTIONSTRING);
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            SqlTransaction Tran = con.BeginTransaction();
            try
            {
                if (txtcatagory.Text != "")
                {
                    SqlParameter[] _arrPara1 = new SqlParameter[18];
                    _arrPara1[0] = new SqlParameter("@CATEGORY_NAME", SqlDbType.VarChar, 50);
                    _arrPara1[1] = new SqlParameter("@PARAMETER_ID_1", SqlDbType.Int);
                    _arrPara1[2] = new SqlParameter("@PARAMETER_ID_2", SqlDbType.Int);
                    _arrPara1[3] = new SqlParameter("@PARAMETER_ID_3", SqlDbType.Int);
                    _arrPara1[4] = new SqlParameter("@PARAMETER_ID_4", SqlDbType.Int);
                    _arrPara1[5] = new SqlParameter("@PARAMETER_ID_5", SqlDbType.Int);
                    _arrPara1[6] = new SqlParameter("@PARAMETER_ID_6", SqlDbType.Int);
                    _arrPara1[7] = new SqlParameter("@category_id", SqlDbType.Int);
                    _arrPara1[8] = new SqlParameter("@CODE", SqlDbType.NChar, 10);
                    _arrPara1[9] = new SqlParameter("@varuserid", SqlDbType.Int);
                    _arrPara1[10] = new SqlParameter("@varCompanyId", SqlDbType.Int);
                    _arrPara1[11] = new SqlParameter("@HSCODE", SqlDbType.NVarChar);
                    _arrPara1[12] = new SqlParameter("@categorySeperateDetail", SqlDbType.NVarChar, 100);
                    _arrPara1[13] = new SqlParameter("@PoufTypeCategory", SqlDbType.Int);

                    _arrPara1[14] = new SqlParameter("@PARAMETER_ID_9", SqlDbType.Int);
                    _arrPara1[15] = new SqlParameter("@PARAMETER_ID_10", SqlDbType.Int);
                    _arrPara1[16] = new SqlParameter("@PARAMETER_ID_11", SqlDbType.Int);
                    _arrPara1[17] = new SqlParameter("@PARAMETER_ID_12", SqlDbType.Int);

                    _arrPara1[0].Value = txtcatagory.Text.ToUpper();
                    _arrPara1[1].Value = chk_1.Checked == true ? 1 : 0;
                    _arrPara1[2].Value = chk_2.Checked == true ? 2 : 0;
                    _arrPara1[3].Value = chk_3.Checked == true ? 3 : 0;
                    _arrPara1[4].Value = chk_4.Checked == true ? 4 : 0;
                    _arrPara1[5].Value = chk_5.Checked == true ? 5 : 0;
                    _arrPara1[6].Value = chk_6.Checked == true ? 6 : 0;
                    
                    if (btnSave.Text == "Update")
                    {
                        _arrPara1[7].Value = gditemcatagory.SelectedValue;
                        btnSave.Text = "Save";
                    }
                    else
                    {
                        _arrPara1[7].Value = 0;
                    }
                    _arrPara1[8].Value = txtode.Text.ToUpper();

                    _arrPara1[9].Value = Session["varuserid"].ToString();
                    _arrPara1[10].Value = Session["varCompanyId"].ToString();
                    _arrPara1[11].Value = TxtHSCode.Text;
                    int n = ChkBoxList.Items.Count;
                    string str = null;
                    for (int i = 0; i < n; i++)
                    {
                        if (ChkBoxList.Items[i].Selected)
                        {
                            str = str == null ? ChkBoxList.Items[i].Value : str + "," + ChkBoxList.Items[i].Value;
                        }
                    }
                    _arrPara1[12].Value = str;

                    _arrPara1[13].Value = 0;
                    if (ChkPoufTypeCategory.Checked == true)
                    {
                        _arrPara1[13].Value = 1;
                    }

                    _arrPara1[14].Value = chk_9.Checked == true ? 9 : 0;
                    _arrPara1[15].Value = chk_10.Checked == true ? 10 : 0;
                    _arrPara1[16].Value = chk_11.Checked == true ? 11 : 0;
                    _arrPara1[17].Value = chk_12.Checked == true ? 12 : 0;

                    SqlHelper.ExecuteNonQuery(Tran, CommandType.StoredProcedure, "PRO_ITEM_CATEGORY_PARAMETERS1", _arrPara1);
                    Tran.Commit();
                    Lblerrer.Text = "";
                    visilbe();
                }
                else
                {
                    Lblerrer.Text = "Importent field missing.............";
                }
            }
            catch (Exception ex)
            {
                Lblerrer.Visible = true;
                Lblerrer.Text = ex.Message;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                if (con != null)
                {
                    con.Dispose();
                }
            }
        }
        else
        {
            if (Lblerrer.Text == "Category Name Or Code already exists............")
            {
                Lblerrer.Visible = true;
                Lblerrer.Text = "CATEGORYNAME already exists............";
            }
            else
            {
                Lblerrer.Visible = true;
                Lblerrer.Text = "Please Fill Details............";
            }
        }
        fill_grid();
    }
    private void visilbe()
    {
        for (int i = 0; i < ChkBoxList.Items.Count; i++)
        {
            ChkBoxList.Items[i].Selected = false;
        }
    }
    private void ClearAll()
    {
        txtcatagory.Text = "";
        chk_1.Checked = false;
        chk_2.Checked = false;
        chk_3.Checked = false;
        chk_4.Checked = false;
        chk_5.Checked = false;
        chk_6.Checked = false;
        chk_9.Checked = false;
        chk_10.Checked = false;
        chk_11.Checked = false;
        chk_12.Checked = false;
        ChkBoxList.Items[0].Selected = false;
        ChkBoxList.Items[1].Selected = false;
        txtode.Text = "";
        TxtHSCode.Text = "";
        txtcatagory.Focus();
        chk_1.Enabled = true;
        chk_2.Enabled = true;
        chk_3.Enabled = true;
        chk_4.Enabled = true;
        chk_5.Enabled = true;
        chk_6.Enabled = true;
        chk_9.Enabled = true;
        chk_10.Enabled = true;
        chk_11.Enabled = true;
        chk_12.Enabled = true;

        ChkPoufTypeCategory.Checked = false;
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        Store_Data();
        ClearAll();
        txtcatagory.Focus();
        btnSave.Text = "Save";
        btndelete.Visible = false;
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        Lblerrer.Text = "";
        btnSave.Text = "Save";
        ClearAll();
    }
    protected void gditemcatagory_SelectedIndexChanged(object sender, EventArgs e)
    {
        ClearAll();
        DataSet ds;
        string id = gditemcatagory.SelectedDataKey.Value.ToString();

        ViewState["CategoryId"] = id;
        ds = SqlHelper.ExecuteDataset("select * from ITEM_CATEGORY_PARAMETERS(Nolock) WHERE CATEGORY_ID=" + id + "order by PARAMETER_ID");
        btnSave.Text = "Update";
        btndelete.Visible = true;
        try
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                switch (Convert.ToInt32(dr["PARAMETER_ID"]))
                {
                    case 1:
                        chk_1.Checked = true;
                        break;
                    case 2:
                        chk_2.Checked = true;
                        break;
                    case 3:
                        chk_3.Checked = true;
                        break;
                    case 4:
                        chk_4.Checked = true;
                        break;
                    case 5:
                        chk_5.Checked = true;
                        break;
                    case 6:
                        chk_6.Checked = true;
                        break;
                    case 9:
                        chk_9.Checked = true;
                        break;
                    case 10:
                        chk_10.Checked = true;
                        break;
                    case 11:
                        chk_11.Checked = true;
                        break;
                    case 12:
                        chk_12.Checked = true;
                        break;
                }
            }
            txtcatagory.Text = gditemcatagory.Rows[gditemcatagory.SelectedIndex].Cells[1].Text;
            txtode.Text = gditemcatagory.Rows[gditemcatagory.SelectedIndex].Cells[2].Text;

            DataSet ds2 = SqlHelper.ExecuteDataset("select * from CategorySeparate(Nolock) WHERE CATEGORYID=" + id + "");
            if (ds2.Tables[0].Rows.Count > 0)
            {
                for (int j = 0; j < ChkBoxList.Items.Count; j++)
                {
                    for (int i = 0; i < ds2.Tables[0].Rows.Count; i++)
                    {
                        if (ChkBoxList.Items[j].Value == ds2.Tables[0].Rows[i]["id"].ToString())
                        {
                            ChkBoxList.Items[j].Selected = true;
                        }
                    }
                }
            }
            DataSet ds3 = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text,
            @"Select Item_finished_id From V_finisheditemdetail Where CATEGORY_ID = " + id + @"
            Select PoufTypeCategory From ITEM_CATEGORY_MASTER Where CATEGORY_ID = " + id + "");
            if (ds3.Tables[0].Rows.Count > 0)
            {
                chk_1.Enabled = false;
                chk_2.Enabled = false;
                chk_3.Enabled = false;
                chk_4.Enabled = false;
                chk_5.Enabled = false;
                chk_6.Enabled = false;
                chk_9.Enabled = false;
                chk_10.Enabled = false;
                chk_11.Enabled = false;
                chk_12.Enabled = false;
            }
            ChkPoufTypeCategory.Checked = false;
            if (ds3.Tables[1].Rows.Count > 0)
            {
                if (ds3.Tables[1].Rows[0]["PoufTypeCategory"].ToString() == "1")
                {
                    ChkPoufTypeCategory.Checked = true;
                }
            }
        }
        catch (Exception ex)
        {
            Lblerrer.Text = ex.Message;
        }
    }
    protected void gditemcatagory_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "javascript:setMouseOverColor(this);";
            e.Row.Attributes["onmouseout"] = "javascript:setMouseOutColor(this);";
            e.Row.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.gditemcatagory, "Select$" + e.Row.RowIndex);
        }
    }
    private void Validated()
    {
        try
        {
            string strsql = @"select CATEGORY_NAME,code 
                From ITEM_CATEGORY_MASTER(Nolock)  
                where CATEGORY_NAME='" + txtcatagory.Text + "' And  MasterCompanyId=" + Session["varCompanyId"];

            if (btnSave.Text == "Update")
            {
                strsql = strsql + " CATEGORY_ID<>" + ViewState["CategoryId"];
            }

            DataSet ds = SqlHelper.ExecuteDataset(strsql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                Lblerrer.Visible = true;
                Lblerrer.Text = "Category Name Or Code already exists............";
                txtcatagory.Text = "";
                txtcatagory.Focus();
            }
            else
            {
                Lblerrer.Text = "";
            }
        }
        catch (Exception ex)
        {
            Lblerrer.Text = ex.Message;
        }
    }
    protected void btndelete_Click(object sender, EventArgs e)
    {
        SqlConnection con = new SqlConnection(ErpGlobal.DBCONNECTIONSTRING);
        if (con.State == ConnectionState.Closed)
        {
            con.Open();
        }
        SqlTransaction Tran = con.BeginTransaction();
        try
        {            
            SqlParameter[] _array = new SqlParameter[2];
            _array[0] = new SqlParameter("@CATEGORY_ID", ViewState["CategoryId"].ToString());
            _array[1] = new SqlParameter("@Message", SqlDbType.NVarChar, 100);
            _array[1].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(Tran, CommandType.StoredProcedure, "Pro_DeleteItem_Category", _array);
            Tran.Commit();
            Lblerrer.Visible = true;
            Lblerrer.Text = _array[1].Value.ToString();
            txtcatagory.Text = "";
        }
        catch (Exception ex)
        {
            Tran.Rollback();
            Lblerrer.Visible = true;
            Lblerrer.Text = ex.Message;
        }
        finally
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
                con.Dispose();
            }
        }
        fill_grid();
        btndelete.Visible = false;
        btnSave.Text = "Save";
        ClearAll();
    }
}
