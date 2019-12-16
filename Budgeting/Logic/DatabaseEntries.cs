using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Web;
using System.Reflection;

namespace Budgeting.Logic {
	public class DbEntry {
		public SQLiteParameter CreateParameter(string Name) {
			FieldInfo[] Fields = DALFieldAttribute.GetDALFields(GetType());

			for (int i = 0; i < Fields.Length; i++) {
				DALFieldAttribute DALField = Fields[i].GetCustomAttribute<DALFieldAttribute>();

				if (DALField.ColumnName == Name)
					return new SQLiteParameter("@" + Name, Fields[i].GetValue(this));
			}

			throw new Exception(string.Format("Field {0} not found in {1}", Name, GetType().Name));
		}

		public SQLiteParameter CreateParameterID() {
			return CreateParameter("id");
		}
	}

	[DALTable("transactions")]
	class Transaction : DbEntry {
		[DALField("id")]
		public int ID = 0;

		[DALField("date")]
		public DateTime Date;

		[DALField("value")]
		public float Value;

		[DALField("desc")]
		public string Description;

		[DALField("user")]
		public int UserID;

		[DALField("maestro_monthly")]
		public int MaestroMonthly;

		public Transaction() {
		}

		public Transaction(User Usr, int Year, int Month, int Day, float Value) {
			Date = new DateTime(Year, Month, Day);
			this.Value = Value;
			Description = "";
			UserID = Usr.ID;
		}

		public Transaction(User Usr, DateTime Date, float Value) : this(Usr, Date.Year, Date.Month, Date.Day, Value) {
		}

		public Transaction(User Usr, BudgetMonth Month, float Value) : this(Usr, Month.FirstDay, Value) {
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

	[DALTable("maestroplus")]
	class MaestroPlusOption : DbEntry {
		[DALField("id")]
		public int ID = 0;

		[DALField("month_count")]
		public int MonthCount = 0;

		[DALField("interest")]
		public float Interest = 0;
	}

	[DALTable("currencies")]
	public class Currency : DbEntry {
		[DALField("id")]
		public int ID = 0;

		[DALField("code")]
		public string Code;

		[DALField("num")]
		public int Num;

		[DALField("name")]
		public string Name;

		[DALField("common")]
		public bool Common;

		[DALField("exchg")]
		public bool Exchange;

		public override string ToString() {
			return string.Format("{0} {1}", Num, Code);
		}
	}

	[DALTable("users")]
	public class User : DbEntry {
		[DALField("id")]
		public int ID;

		[DALField("user")]
		public string Username;

		[DALField("hash")]
		public string Hash;

		[DALField("salt")]
		public string Salt;
	}

	[DALTable("userlogin")]
	public class UserLogin : DbEntry {
		[DALField("id")]
		public int ID;

		[DALField("userid")]
		public int UserID;

		[DALField("datetime")]
		public DateTime DateTime;

		public UserLogin() {
		}

		public UserLogin(User Usr) {
			UserID = Usr.ID;
			DateTime = DateTime.Now;
		}
	}
}