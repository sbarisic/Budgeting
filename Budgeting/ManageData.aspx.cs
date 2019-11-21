using Budgeting.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Budgeting {
	public partial class _ManageData : Page {
		protected void Page_Load(object sender, EventArgs e) {
			BudgetSession S = BudgetSession.Get(this);

			if (!S.Authenticated()) {
				Response.Redirect("Login.aspx");
				return;
			}

			ManageDataSession Data = ManageDataSession.Get(this);

			/*if (Data.State == ManageDataState.Main)
				divDateFromTo.Visible = true;*/
		}

		protected void Confirm_Click(object sender, EventArgs e) {
			ManageDataSession Data = ManageDataSession.Get(this);
			string Out = "";

			if (DateTime.TryParse(dateBegin.Value, out DateTime DateBegin)) {
				Out = DateBegin.ToString() + "<br/>";
			}

			if (DateTime.TryParse(dateEnd.Value, out DateTime DateEnd)) {
				Out += DateEnd.ToString();
			}


			labelError.InnerText = Out;
			labelError.Visible = true;
			return;
		}
	}
}