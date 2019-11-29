using Budgeting.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Budgeting.Controls;
using System.Data.SQLite;

using ListControl = Budgeting.Controls.ListControl;

namespace Budgeting {
	public partial class _ExchangeRates : Page {
		protected void Page_Load(object sender, EventArgs e) {
			BudgetSession S = BudgetSession.Get(this);

			if (!S.Authenticated()) {
				Response.Redirect("Login.aspx");
				return;
			}

			DAL DbDAL = new DAL();
			Currency[] Currencies = DbDAL.Select<Currency>(new[] { new SQLiteParameter("@exchg", 1) }).ToArray();

			if (S.ExchangeCurrency == null)
				S.ExchangeCurrency = Currencies.Where(C => C.Code == "EUR").First();

			RefreshSelected(S);
			btnDropdown.Clear();

			foreach (var Cur in Currencies) {
				btnDropdown.AddItem(new DropdownEntry(Cur.Code));
			}

			btnDropdown.Bind();
			GenerateExchangeTable(DbDAL, Currencies, S);

			btnDropdown.OnSelect += (Entry) => {
				S.ExchangeCurrency = Currencies.Where(C => C.Code == Entry.Name).First();
				RefreshSelected(S);
				Server.TransferRequest(Request.Url.AbsolutePath, false);
			};
		}

		void RefreshSelected(BudgetSession S) {
			inSelVal.Value = S.ExchangeCurrency.Code;
		}

		void GenerateExchangeTable(DAL DbDAL, Currency[] Currencies, BudgetSession S) {
			ListControl Lst = CreateListControl();
			Tuple<Currency, float>[] Exchanges = Utils.GetExchange(S.ExchangeCurrency, Currencies);

			foreach (var Ex in Exchanges) {
				Lst.AddItem(string.Format("{0} - {1}", Ex.Item1.Code, Ex.Item2));
			}

			TableRow Row = new TableRow();
			TableCell Cell = new TableCell();
			Cell.Controls.Add(Lst);
			Row.Cells.Add(Cell);
			DataTable.Rows.Add(Row);
		}

		ListControl CreateListControl() {
			ListControl LC = (ListControl)Page.LoadControl("~/Controls/ListControl.ascx");
			return LC;
		}
	}
}