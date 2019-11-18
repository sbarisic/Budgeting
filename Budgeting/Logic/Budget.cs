using Budgeting.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

	class Transaction {
		[DALField("id")]
		public int ID;

		[DALField("date")]
		public DateTime Date;

		[DALField("value")]
		public float Value;

		[DALField("desc")]
		public string Description;

		[DALField("user")]
		public int UserID;

		public Transaction() {
		}

		public Transaction(int Year, int Month, int Day, float Value) {
			Date = new DateTime(Year, Month, Day);
			this.Value = Value;
			Description = "";
		}

		public Transaction(DateTime Date, float Value) : this(Date.Year, Date.Month, Date.Day, Value) {
		}

		public Transaction(BudgetMonth Month, float Value) : this(Month.FirstDay, Value) {
		}

		public string FormatValue() {
			if (!string.IsNullOrEmpty(Description))
				return Description + "<br/>" + Transaction.FormatValue(Value);

			return Transaction.FormatValue(Value);
		}

		public static string FormatValue(float Value) {
			return string.Format("{0:0.00}", Value);
		}

		public static Transaction[] Sort(IEnumerable<Transaction> Transactions) {
			List<Transaction> Trans = new List<Transaction>(Transactions);
			Trans.Sort((X, Y) => DateTime.Compare(X.Date, Y.Date));
			return Trans.ToArray();
		}
	}

	class MaestroPlusOption {
		[DALField("id")]
		public int ID;

		[DALField("month_count")]
		public int MonthCount;

		[DALField("interest")]
		public float Interest;
	}

	class MaestroPlusCalculator {
		MaestroPlusOption[] Options;

		public MaestroPlusCalculator(DAL DbDAL) {
			Options = DbDAL.Select<MaestroPlusOption>("maestroplus").ToArray();
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

		public BudgetCalculator(DAL DbDAL, int UserID) {
			this.DbDAL = DbDAL;
			MaestroCalc = new MaestroPlusCalculator(DbDAL);

			AllTransactions = new List<Transaction>(DbDAL.Select<Transaction>("transactions", "user = " + UserID));

			/*AddMonthly(new DateTime(2019, 11, 15), 12, 6444.80f, "");
			AddMonthly(new DateTime(2019, 11, 15), 12, 371.80f, "");
			AddMonthly(new DateTime(2019, 11, 15), 12, -1328.01f, "Car");
			AddMonthly(new DateTime(2019, 11, 15), 12, -100.00f, "Phone");
			AddMonthly(new DateTime(2019, 11, 15), 12, -70.00f, "Health Ins");

			AddMonthly(new DateTime(2019, 07, 27), 12, -666.67f, "Maestro1");
			AddMonthly(new DateTime(2019, 07, 27), 12, -83.33f, "Maestro2");

			//AddMaestroPlus(new DateTime(2019, 11, 18), 6, 3000);
			//TransferMoneyToMonth(new DateTime(2019, 11, 18), new DateTime(2020, 1, 30), 1500);

			AddTransaction(new Transaction(2019, 11, 15, -3500), "Uni");
			AddTransaction(new Transaction(2020, 1, 15, -3500), "Uni");

			AddTransaction(new Transaction(2020, 3, 15, -3000), "Car Reg");*/
		}

		void AddTransaction(Transaction T, string Description) {
			//float Sign = T.Value < 0 ? -1 : 1;
			//T.Value = (float)Math.Pow(Math.Abs(T.Value) * 0.6f, 1.1f) * Sign;

			T.Description = Description;
			AllTransactions.Add(T);
		}

		void AddMonthly(DateTime Start, int Count, float Value, string Description) {
			for (int i = 0; i < Count; i++)
				AddTransaction(new Transaction(Start.AddMonths(i), Value), Description);
		}

		void AddMaestroPlus(DateTime Start, int Months, float Value) {
			MaestroCalc.Calculate(Months, Value, out float OneTime, out float Monthly);

			AddTransaction(new Transaction(Start, -OneTime), "Maestro OneTime");
			AddTransaction(new Transaction(Start, Value), "Maestro Money");
			AddMonthly(Start.AddMonths(1), Months, -Monthly, "Maestro");
		}

		void TransferMoneyToMonth(DateTime From, DateTime To, float Value) {
			bool Short = From.Year == To.Year;

			AddTransaction(new Transaction(From, -Value), "Transfer to " + BudgetMonth.Fmt(To, Short));
			AddTransaction(new Transaction(To, Value), "Transfer from " + BudgetMonth.Fmt(From, Short));
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