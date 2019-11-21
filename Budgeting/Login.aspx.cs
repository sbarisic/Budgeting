using Budgeting.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Budgeting {
	public partial class _Login : Page {
		protected void Page_Load(object sender, EventArgs e) {
			BudgetSession S = BudgetSession.Get(this);
			if (S.Authenticated()) {
				Response.Redirect("Default.aspx");
				return;
			}
		}

		protected void SignIn_Click(object sender, EventArgs e) {
			DAL DbDAL = new DAL();
			BudgetSession S = BudgetSession.Get(this);

			if (!S.LogIn(DbDAL, inputUsername.Value, inputPassword.Value)) {
				labelError.InnerText = "Wrong username or password";
				labelError.Visible = true;
				return;
			}

			if (S.Authenticated()) {
				Response.Redirect("Default.aspx");
				return;
			}
		}
	}
}