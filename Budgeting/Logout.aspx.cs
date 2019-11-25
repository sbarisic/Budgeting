using Budgeting.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Budgeting {
	public partial class _Logout : Page {
		protected void Page_Load(object sender, EventArgs e) {
			ManageDataSession Data = ManageDataSession.Get(this);
			Data.State = ManageDataState.Main;

			BudgetSession S = BudgetSession.Get(this);
			if (S.Authenticated())
				S.LogOut();

			Response.Redirect("Login.aspx");
		}
	}
}