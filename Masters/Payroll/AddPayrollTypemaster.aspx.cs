﻿using System;using CarpetERP.Core.DAL;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Masters_Payroll_AddPayrollTypemaster : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["varCompanyId"] == null)
        {
            Response.Redirect("~/Login.aspx");
        }     
    }
}