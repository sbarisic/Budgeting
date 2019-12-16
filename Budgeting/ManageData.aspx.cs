using Budgeting.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.SQLite;

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

			if (Data.SelectedCurrency == null) {
				DAL DbDAL = new DAL();
				Data.SelectedCurrency = DbDAL.Select<Currency>().Where(C => C.Code == "HRK").First();
			}

			HandleShowState(Data, Data.State);
		}

		void HandleShowState(ManageDataSession Data, ManageDataState State, bool ChangeState = false) {
			divRadioOptions.Visible = false;
			divDateFrom.Visible = false;
			divDateTo.Visible = false;
			divCurAmt.Visible = false;
			divMonthCount.Visible = false;

			btnBack.Visible = true;

			btnCurSel.InnerText = Data.SelectedCurrency.Code;

			switch (State) {
				case ManageDataState.Main:
					ShowMain();
					break;

				case ManageDataState.AddSingle:
					ShowSingleTransaction();
					break;

				case ManageDataState.AddMultiple:
					ShowMultipleTransaction();
					break;

				case ManageDataState.AddMaestroPlus:
					ShowAddMaestro();
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

				if (SelBtn != null)
					SelBtn.Checked = true;
			} else {
				Data.MainRadioButtons.Clear();
				Data.MainRadioButtons.Add(new MainRadioButton("Add Single", ManageDataState.AddSingle) { Checked = true });
				Data.MainRadioButtons.Add(new MainRadioButton("Add Multiple", ManageDataState.AddMultiple) { Enabled = true });
				Data.MainRadioButtons.Add(new MainRadioButton("Add Repeating", ManageDataState.AddRepeating) { Enabled = false });
				Data.MainRadioButtons.Add(new MainRadioButton("Add MaestroPlus", ManageDataState.AddMaestroPlus) { Enabled = true });
				Data.MainRadioButtons.Add(new MainRadioButton("Manage MaestroPlus", ManageDataState.ManageMaestroPlus) { Enabled = false });

				foreach (var Btn in Data.MainRadioButtons)
					if (Btn.Checked)
						mainRadioResult.Value = Btn.InputID;
			}

			btnBack.Visible = false;
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
			divComment.Visible = true;
		}

		void ShowMultipleTransaction() {
			titleManageData.InnerText = "Insert Multiple Transactions";

			divDateFrom.Visible = true;
			lblDateFrom.InnerText = "Transaction From";

			divDateTo.Visible = true;
			lblDateTo.InnerText = "Transaction To";

			divCurAmt.Visible = true;
			divComment.Visible = true;
		}

		void ShowAddMaestro() {
			titleManageData.InnerText = "Add Maestro Transaction";

			divDateFrom.Visible = true;
			lblDateFrom.InnerText = "Transaction From";

			divMonthCount.Visible = true;
			divCurAmt.Visible = true;
			divComment.Visible = true;
		}

		void DisplayError(string Msg) {
			labelError.InnerText = Msg;
			labelError.Visible = true;
		}

		protected void Back_Click(object sender, EventArgs e) {
			ManageDataSession Data = ManageDataSession.Get(this);
			HandleShowState(Data, ManageDataState.Main, true);
			Server.TransferRequest(Request.Url.AbsolutePath, false);
		}

		protected void Confirm_Click(object sender, EventArgs e) {
			ManageDataSession Data = ManageDataSession.Get(this);
			BudgetSession S = BudgetSession.Get(this);

			switch (Data.State) {
				case ManageDataState.Main: {
						MainRadioButton Next = GetMainRadioButton(Data);
						HandleShowState(Data, Next.NewState, true);
						return;
					}

				case ManageDataState.AddSingle: {
						float Amt = float.Parse(inCurAmt.Value);
						DateTime Date = DateTime.Parse(dateBegin.Value);

						DAL DbDAL = new DAL();
						Transaction T = new Transaction(S.CurrentUser, Date, Amt);
						T.Description = inComment.Value.Trim();

						DbDAL.Insert(T);
						break;
					}

				case ManageDataState.AddMultiple: {
						float Amt = float.Parse(inCurAmt.Value);
						DateTime From = DateTime.Parse(dateBegin.Value);
						DateTime To = DateTime.Parse(dateEnd.Value).AddDays(1);

						DAL DbDAL = new DAL();

						while (From < To) {
							Transaction T = new Transaction(S.CurrentUser, From, Amt);
							T.Description = inComment.Value.Trim();

							DbDAL.Insert(T);
							From = From.AddMonths(1);
						}

						break;
					}

				case ManageDataState.AddRepeating:
					break;

				case ManageDataState.AddMaestroPlus: {
						DAL DbDAL = new DAL();
						MaestroPlusCalculator MaestroCalc = new MaestroPlusCalculator(DbDAL);

						int MonthCount = int.Parse(inMonthCount.Value);
						float Amt = float.Parse(inCurAmt.Value);
						DateTime From = DateTime.Parse(dateBegin.Value);
						string Comment = inComment.Value.Trim();

						MaestroCalc.Calculate(MonthCount, Amt, out float OneTime, out float Monthly);

						Transaction TOneTime = new Transaction(S.CurrentUser, From, -OneTime);
						TOneTime.Description = "Maestro OneTime " + Comment;
						DbDAL.Insert(TOneTime);

						Transaction MaestroPayment = new Transaction(S.CurrentUser, From, Amt);
						MaestroPayment.Description = Comment;
						DbDAL.Insert(MaestroPayment);

						From = From.AddMonths(1);

						for (int i = 0; i < MonthCount; i++) {
							Transaction MaestroMonthly = new Transaction(S.CurrentUser, From, -Monthly);
							MaestroMonthly.Description = Comment;
							MaestroMonthly.MaestroMonthly = 1;
							DbDAL.Insert(MaestroMonthly);

							From = From.AddMonths(1);
						}

						break;
					}

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