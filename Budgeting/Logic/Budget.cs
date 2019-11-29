using Budgeting.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Data.SQLite;
using System.Web;
using ListControl = Budgeting.Controls.ListControl;

namespace Budgeting.Logic {
	class BudgetMonth {
		public DateTime FirstDay;
		public DateTime LastDay;

		public BudgetMonth(DateTime Month) {
			FirstDay = new DateTime(Month.Year, Month.Month, 1);
			LastDay = FirstDay.AddMonths(1).AddTicks(-1);
		}

		public bool InMonth(DateTime DT) {
			if (DT >= FirstDay && DT <= LastDay)
				return true;

			return false;
		}

		public string GetName() {
			string MonthName = CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(FirstDay.Month);

			return string.Format("{0} - {1}", FirstDay.Month, MonthName);
		}

		public static string Fmt(DateTime DT, bool Short = true) {
			if (Short)
				return string.Format("{0}.{1}.", DT.Day, DT.Month);

			return string.Format("{0}.{1}.{2}.", DT.Day, DT.Month, DT.Year);
		}
	}

	class MaestroPlusCalculator {
		MaestroPlusOption[] Options;

		public MaestroPlusCalculator(DAL DbDAL) {
			Options = DbDAL.Select<MaestroPlusOption>().ToArray();
		}

		public void Calculate(int Months, float Value, out float OneTimePayment, out float Monthly) {
			for (int i = 0; i < Options.Length; i++) {
				if (Options[i].MonthCount == Months) {
					OneTimePayment = (float)Math.Round(Value * (Options[i].Interest / 100.0f), 2);
					Monthly = (float)Math.Round(Value / Months, 2);
					return;
				}
			}

			throw new Exception(string.Format("Maestro Plus option for {0} months not found", Months));
		}
	}

	class BudgetCalculator {
		List<Transaction> AllTransactions = new List<Transaction>();
		MaestroPlusCalculator MaestroCalc;
		DAL DbDAL;

		public BudgetCalculator(DAL DbDAL, User Usr) {
			this.DbDAL = DbDAL;
			MaestroCalc = new MaestroPlusCalculator(DbDAL);
			AllTransactions = new List<Transaction>(DbDAL.Select<Transaction>(new[] { new SQLiteParameter("@user", Usr.ID) }));
		}

		void AddTransaction(Transaction T, string Description) {
			//float Sign = T.Value < 0 ? -1 : 1;
			//T.Value = (float)Math.Pow(Math.Abs(T.Value) * 0.6f, 1.1f) * Sign;

			T.Description = Description;
			AllTransactions.Add(T);
		}

		void AddMonthly(User Usr, DateTime Start, int Count, float Value, string Description) {
			for (int i = 0; i < Count; i++)
				AddTransaction(new Transaction(Usr, Start.AddMonths(i), Value), Description);
		}

		void AddMaestroPlus(User Usr, DateTime Start, int Months, float Value) {
			MaestroCalc.Calculate(Months, Value, out float OneTime, out float Monthly);

			AddTransaction(new Transaction(Usr, Start, -OneTime), "Maestro OneTime");
			AddTransaction(new Transaction(Usr, Start, Value), "Maestro Money");
			AddMonthly(Usr, Start.AddMonths(1), Months, -Monthly, "Maestro");
		}

		void TransferMoneyToMonth(User Usr, DateTime From, DateTime To, float Value) {
			bool Short = From.Year == To.Year;

			AddTransaction(new Transaction(Usr, From, -Value), "Transfer to " + BudgetMonth.Fmt(To, Short));
			AddTransaction(new Transaction(Usr, To, Value), "Transfer from " + BudgetMonth.Fmt(From, Short));
		}

		IEnumerable<Transaction> GetTransactionsForMonth(BudgetMonth Month) {
			foreach (var T in AllTransactions)
				if (Month.InMonth(T.Date))
					yield return T;
		}

		public void CalculateBudget(ListControl LC) {
			BudgetMonth Month = (BudgetMonth)LC.UserData;

			LC.Clear();
			LC.AddItem(Month.GetName(), ItemColor.Info);

			float Sum = 0;
			Transaction[] Transactions = Transaction.Sort(GetTransactionsForMonth(Month)).ToArray();

			foreach (var T in Transactions) {
				Sum += T.Value;

				if (T.Value > 0)
					LC.AddItem(T.FormatValue(), ItemColor.Green);
				else
					LC.AddItem(T.FormatValue(), ItemColor.Red);
			}

			LC.AddItem(Transaction.FormatValue(Sum));
		}
	}
}