using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Budgeting.Controls;
using ListControl = Budgeting.Controls.ListControl;

using Budgeting.Logic;

namespace Budgeting {
	public partial class _Default : Page {
		ListControl[] MonthLists;
		DAL DbDAL;

		protected void Page_Load(object sender, EventArgs e) {
			DbDAL = new DAL();

			if (BudgetSession.GetUser(DbDAL, "admin") == null) {
				BudgetSession.CreateUser(DbDAL, "admin");
				BudgetSession.ResetPassword(DbDAL, "admin", "root");
			}

			BudgetSession S = BudgetSession.Get(this);
			if (!S.Authenticated()) {
				Response.Redirect("Login.aspx");
				return;
			}

			CreateMonthLists();
			Calculate(S.CurrentUser);
		}

		void Calculate(User Usr) {
			BudgetCalculator Calculator = new BudgetCalculator(DbDAL, Usr);

			for (int i = 0; i < MonthLists.Length; i++)
				Calculator.CalculateBudget(MonthLists[i]);
		}

		void CreateMonthLists() {
			TableRow Row = new TableRow();
			MonthLists = new ListControl[12];

			int CurMonth = DateTime.Now.Month;

			for (int i = 0; i < 12; i++) {
				TableCell Cell = new TableCell();

				BudgetMonth BMonth = new BudgetMonth(DateTime.Now.AddMonths(i));

				Cell.Controls.Add(MonthLists[i] = CreateListControl(BMonth));
				Row.Cells.Add(Cell);

				//MonthLists[i].AddItem(BMonth.GetName(), ItemColor.Info);
			}

			DataTable.Rows.Add(Row);
		}

		ListControl CreateListControl(object UserData) {
			ListControl LC = (ListControl)Page.LoadControl("~/Controls/ListControl.ascx");
			LC.UserData = UserData;

			return LC;
		}
	}
}