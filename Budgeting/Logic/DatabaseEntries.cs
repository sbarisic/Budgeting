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

	[DALTable("maestroplus")]
	class MaestroPlusOption : DbEntry {
		[DALField("id")]
		public int ID;

		[DALField("month_count")]
		public int MonthCount;

		[DALField("interest")]
		public float Interest;
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
		public string UserID;

		[DALField("userid")]
		public string Token;
	}
}