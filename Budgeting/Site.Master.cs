﻿using Budgeting.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Budgeting {
	public partial class SiteMaster : MasterPage {
		protected void Page_Load(object sender, EventArgs e) {
			BudgetSession S = BudgetSession.Get(this);

			if (S.Authenticated()) {
				navHome.Visible = true;
				navManageData.Visible = true;
				navExchange.Visible = true;
				navLogout.Visible = true;
			} else {
				navLogin.Visible = true;
			}
		}
	}
}