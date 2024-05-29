﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

public partial class Masters_Carpet_DefineItemCodeOther : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["varCompanyId"] == null)
        {
            Response.Redirect("~/Login.aspx");
        }   
        if (!IsPostBack)
        {
            SqlConnection con = new SqlConnection(ErpGlobal.DBCONNECTIONSTRING);
            con.Open();
            try
            {
                string str = @"Select HS_CODE,MIN_ODRDERQTY,PRICE,INNER_PACKING,MASTER_PACKING,MASTER_PKG_SIZE,MASTER_PKG_SIZE_1,MASTER_PKG_SIZE_2,DESCRIPTION,VOLUME,DRAWBACKRATE,
            LOAD_20,LOAD_40,LOAD_40HQ,MATERIALREMARKS,FINISHREMARKS,PACKINGREMARKS,ISNULL(Iron,0) IRONWT,ISNULL(Glass,0) WOODWT,ISNULL(NetWeight,0) NETWT FROM ITEM_PARAMETER_OTHER,MAIN_ITEM_IMAGE
            WHERE finishedid=Item_Finished_ID AND Item_Finished_ID =" + Request.QueryString["SrNo"] + " And MAIN_ITEM_IMAGE.MasterCompanyid=" + Session["varCompanyId"];
                DataSet ds = SqlHelper.ExecuteDataset(con, CommandType.Text, str);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    TxtHsCode.Text = ds.Tables[0].Rows[0]["HS_CODE"].ToString();
                    TxtMinOrderQty.Text = ds.Tables[0].Rows[0]["MIN_ODRDERQTY"].ToString();
                    TxtPrice.Text = ds.Tables[0].Rows[0]["PRICE"].ToString();
                    TxtInnerPkg.Text = ds.Tables[0].Rows[0]["INNER_PACKING"].ToString();
                    TxtMasterPkg.Text = ds.Tables[0].Rows[0]["MASTER_PACKING"].ToString();
                    TxtMasterPkgSize.Text = ds.Tables[0].Rows[0]["MASTER_PKG_SIZE"].ToString();
                    TxtMasterPkgSize1.Text = ds.Tables[0].Rows[0]["MASTER_PKG_SIZE_1"].ToString();
                    TxtMasterPkgSize2.Text = ds.Tables[0].Rows[0]["MASTER_PKG_SIZE_2"].ToString();
                    TxtDesc.Text = ds.Tables[0].Rows[0]["DESCRIPTION"].ToString();
                    TxtVolume.Text = ds.Tables[0].Rows[0]["VOLUME"].ToString();
                    TxtDrawBackRate.Text = ds.Tables[0].Rows[0]["DRAWBACKRATE"].ToString();
                    TxtLoad20.Text = ds.Tables[0].Rows[0]["LOAD_20"].ToString();
                    TxtLoad40.Text = ds.Tables[0].Rows[0]["LOAD_40"].ToString();
                    Txtload40HQ.Text = ds.Tables[0].Rows[0]["LOAD_40HQ"].ToString();
                    txtMaterialremarks.Text = ds.Tables[0].Rows[0]["MATERIALREMARKS"].ToString();
                    TxtFinishRemarks.Text = ds.Tables[0].Rows[0]["FINISHREMARKS"].ToString();
                    TxtPkgRemarks.Text = ds.Tables[0].Rows[0]["PACKINGREMARKS"].ToString();
                    TxtIronWt.Text = ds.Tables[0].Rows[0]["IRONWT"].ToString();
                    TxtWoodWt.Text = ds.Tables[0].Rows[0]["WOODWT"].ToString();
                    TxtNetWt.Text = ds.Tables[0].Rows[0]["NETWT"].ToString();

                }
            }

            catch (Exception ex)
            {
                LblMsg.Text = ex.Message.ToString();
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }
    }
    protected void BTNSave_Click(object sender, EventArgs e)
    {
        SqlConnection con = new SqlConnection(ErpGlobal.DBCONNECTIONSTRING);
        con.Open();
        try
        {
            SqlParameter[] _param = new SqlParameter[22];
            _param[0] = new SqlParameter("@ItemFinishedID", Request.QueryString["SrNo"]);
            _param[1] = new SqlParameter("@HSCODE", TxtHsCode.Text);
            _param[2] = new SqlParameter("@Description", TxtDesc.Text);
            _param[3] = new SqlParameter("@MinOrderQty", TxtMinOrderQty.Text);
            _param[4] = new SqlParameter("@Price", TxtPrice.Text);
            _param[5] = new SqlParameter("@InnerPacking", TxtInnerPkg.Text);
            _param[6] = new SqlParameter("@MasterPacking", TxtMasterPkg.Text);
            _param[7] = new SqlParameter("@MasterPackingSize", TxtMasterPkgSize.Text);
            _param[8] = new SqlParameter("@MasterPackingSize1", TxtMasterPkgSize1.Text);
            _param[9] = new SqlParameter("@MasterPackingSize2", TxtMasterPkgSize2.Text);
            _param[10] = new SqlParameter("@Volume", TxtVolume.Text);
            _param[11] = new SqlParameter("@DrawBackRate", TxtDrawBackRate.Text);
            _param[12] = new SqlParameter("@Load20", TxtLoad20.Text);
            _param[13] = new SqlParameter("@Load40", TxtLoad40.Text);
            _param[14] = new SqlParameter("@Load40HQ", Txtload40HQ.Text);
            _param[15] = new SqlParameter("@MaterialRemarks", txtMaterialremarks.Text);
            _param[16] = new SqlParameter("@FinishRemarks", TxtFinishRemarks.Text);
            _param[17] = new SqlParameter("@PackingRemarks", TxtPkgRemarks.Text);
            _param[18] = new SqlParameter("@IronWt", TxtIronWt.Text);
            _param[19] = new SqlParameter("@WoodWt", TxtWoodWt.Text);
            _param[20] = new SqlParameter("@NetWt", TxtNetWt.Text);
            _param[21] = new SqlParameter("@MasterCompanyId", Session["VarCompanyId"]);

            SqlHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, "Save_Parameter_Other", _param);
            LblMsg.Text = "Record(s) has been saved successfully!";
        }
        catch (Exception ex)
        {
            UtilityModule.MessageAlert(ex.Message, "Masters/Carpet/DefineItemcodeOther");
            LblMsg.Text = ex.ToString();
        }
        finally
        {
            con.Close();
            con.Dispose();
        }

    }
}