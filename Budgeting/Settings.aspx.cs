using Budgeting.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Budgeting {
	public partial class _Settings : Page {
		protected void Page_Load(object sender, EventArgs e) {
			BudgetSession S = BudgetSession.Get(this);
			if (!S.Authenticated()) {
				Response.Redirect("Login.aspx");
				return;
			}
		}
	}
}