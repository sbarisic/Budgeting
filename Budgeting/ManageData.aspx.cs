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
		public bool Checked;
		public bool Enabled;

		public string CustomAttributes {
			get {
				string Ret = "";

				if (Checked)
					Ret += " checked=\"checked\"";

				if (!Enabled)
					Ret += " disabled=\"disabled\"";

				return Ret;
			}
		}

		public string InputIDLiteral {
			get {
				return string.Format("'{0}'", InputID);
			}
		}

		public MainRadioButton(string Text, ManageDataState State) {
			this.Text = Text;
			NewState = State;
			InputID = "radioBox_" + NewState.ToString() + "_" + Utils.RandomString(8);
			Checked = false;
			Enabled = true;
		}

		public override string ToString() {
			return InputID;
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
			HandleShowState(Data, Data.State);
		}

		void HandleShowState(ManageDataSession Data, ManageDataState State, bool ChangeState = false) {
			divRadioOptions.Visible = false;
			divDateFrom.Visible = false;
			divDateTo.Visible = false;
			divCurAmt.Visible = false;


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

			if (ChangeState)
				Data.State = State;
		}

		void ShowMain() {
			ManageDataSession Data = ManageDataSession.Get(this);

			if (IsPostBack) {
				MainRadioButton SelBtn = GetMainRadioButton(Data);

				foreach (var Btn in Data.MainRadioButtons)
					Btn.Checked = false;

				SelBtn.Checked = true;
			} else {
				Data.MainRadioButtons.Clear();
				Data.MainRadioButtons.Add(new MainRadioButton("Add Single", ManageDataState.AddSingle) { Checked = true });
				Data.MainRadioButtons.Add(new MainRadioButton("Add Multiple", ManageDataState.AddMultiple) { Enabled = false });
				Data.MainRadioButtons.Add(new MainRadioButton("Add Repeating", ManageDataState.AddRepeating) { Enabled = false });
				Data.MainRadioButtons.Add(new MainRadioButton("Add MaestroPlus", ManageDataState.AddMaestroPlus) { Enabled = false });
				Data.MainRadioButtons.Add(new MainRadioButton("Manage MaestroPlus", ManageDataState.ManageMaestroPlus) { Enabled = false });

				foreach (var Btn in Data.MainRadioButtons)
					if (Btn.Checked)
						mainRadioResult.Value = Btn.InputID;
			}

			divRadioOptions.Visible = true;
			rptOptions.DataSource = Data.MainRadioButtons;
			rptOptions.DataBind();
		}

		MainRadioButton GetMainRadioButton(ManageDataSession Data) {
			string ID = mainRadioResult.Value;

			foreach (var Btn in Data.MainRadioButtons)
				if (Btn.InputID == ID)
					return Btn;

			return null;
		}

		void ShowSingleTransaction() {
			titleManageData.InnerText = "Insert Single Transaction";

			divDateFrom.Visible = true;
			lblDateFrom.InnerText = "Transaction Date";

			divCurAmt.Visible = true;
		}

		void DisplayError(string Msg) {
			labelError.InnerText = Msg;
			labelError.Visible = true;
		}

		protected void Confirm_Click(object sender, EventArgs e) {
			ManageDataSession Data = ManageDataSession.Get(this);

			switch (Data.State) {
				case ManageDataState.Main: {
						MainRadioButton Next = GetMainRadioButton(Data);
						HandleShowState(Data, Next.NewState, true);
						return;
					}

				case ManageDataState.AddSingle: {

						break;
					}

				case ManageDataState.AddMultiple:
					break;

				case ManageDataState.AddRepeating:
					break;

				case ManageDataState.AddMaestroPlus:
					break;

				case ManageDataState.ManageMaestroPlus:
					break;

				default:
					throw new Exception("Invalid state " + Data.State);
			}



			/*if (DateTime.TryParse(dateBegin.Value, out DateTime DateBegin)) {
				Out += DateBegin.ToString() + " to ";
			}

			if (DateTime.TryParse(dateEnd.Value, out DateTime DateEnd)) {
				Out += DateEnd.ToString();
			}*/

			Server.TransferRequest(Request.Url.AbsolutePath, false);
		}
	}
}