using Budgeting.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Budgeting {
	public class MainRadioButton {
		public string Text;
		public string InputID;
		public ManageDataState NewState;

		public MainRadioButton(string Text, ManageDataState State) {
			this.Text = Text;
			NewState = State;
			InputID = "cbId" + Utils.RandomString(8);
		}
	}

	public partial class _ManageData : Page {
		protected void Page_Load(object sender, EventArgs e) {
			BudgetSession S = BudgetSession.Get(this);

			if (!S.Authenticated()) {
				Response.Redirect("Login.aspx");
				return;
			}

			ManageDataSession Data = ManageDataSession.Get(this);
			HandleShowState(Data.State);
		}

		void HandleShowState(ManageDataState State) {
			switch (State) {
				case ManageDataState.Main:
					ShowMain();
					break;

				case ManageDataState.AddSingle:
					ShowSingleTransaction();
					break;

				default:
					throw new NotImplementedException();
			}
		}

		void ShowMain() {
			List<MainRadioButton> RadioButtons = new List<MainRadioButton>();
			RadioButtons.Add(new MainRadioButton("Add Single", ManageDataState.AddSingle));
			RadioButtons.Add(new MainRadioButton("Add Multiple", ManageDataState.AddMultiple));
			RadioButtons.Add(new MainRadioButton("Add Repeating", ManageDataState.AddRepeating));
			RadioButtons.Add(new MainRadioButton("Add MaestroPlus", ManageDataState.AddMaestroPlus));
			RadioButtons.Add(new MainRadioButton("Manage MaestroPlus", ManageDataState.ManageMaestroPlus));

			rptOptions.DataSource = RadioButtons;
			rptOptions.DataBind();
		}

		void ShowSingleTransaction() {
			divDateFrom.Visible = true;
			lblDateFrom.InnerText = "Transaction Date";
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