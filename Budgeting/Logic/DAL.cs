using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SQLite;
using System.Data;
using System.IO;
using System.Reflection;

namespace Budgeting.Logic {
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	sealed class DALFieldAttribute : Attribute {
		public string ColumnName;

		public DALFieldAttribute(string ColumnName) {
			this.ColumnName = ColumnName;
		}
	}

	public class DAL {
		SQLiteConnection Con;
		DataSet DataSet = new DataSet();
		DataTable DataTable;

		public DAL() {
		}

		void Open() {
			if (Con == null) {
				Con = new SQLiteConnection("Data Source=C:\\Projekti\\Budgeting\\database\\Main.db;Version=3;New=False;Compress=True;");
			}


			Con.Open();
		}

		void Close() {
			Con.Close();
		}

		public void NonQuery(string TxtQuery) {
			Open();

			using (SQLiteCommand Cmd = Con.CreateCommand()) {
				Cmd.CommandText = TxtQuery;
				Cmd.ExecuteNonQuery();
			}

			Close();
		}

		public DataTable Query(string TxtQuery) {
			Open();

			using (SQLiteCommand Cmd = Con.CreateCommand())
			using (SQLiteDataAdapter Adapter = new SQLiteDataAdapter(TxtQuery, Con)) {
				DataSet.Reset();
				Adapter.Fill(DataSet);
				DataTable = DataSet.Tables[0];
			}

			Close();
			return DataTable;
		}

		public IEnumerable<T> Select<T>(string TableName, string Condition = "") where T : class, new() {
			if (!string.IsNullOrEmpty(Condition))
				Condition = " where " + Condition;
			else
				Condition = "";

			DataTable Table = Query("select * from " + TableName + Condition);
			int RowCount = Table.Rows.Count;

			FieldInfo[] Fields = typeof(T).GetFields().Where(F => F.GetCustomAttribute<DALFieldAttribute>() != null).ToArray();

			for (int i = 0; i < RowCount; i++) {
				T Obj = new T();

				for (int j = 0; j < Fields.Length; j++) {
					string Name = Fields[j].GetCustomAttribute<DALFieldAttribute>().ColumnName;
					object ColVal = Table.Rows[i][Name];

					if (ColVal.GetType() != Fields[j].FieldType)
						ColVal = Convert.ChangeType(ColVal, Fields[j].FieldType);

					Fields[j].SetValue(Obj, ColVal);
				}

				yield return Obj;
			}

			yield break;
		}

		public void Insert<T>(string TableName, T Val) where T : class {
			SQLiteCommand SQL = Con.CreateCommand();
			SQL.CommandText = "INSERT INTO @table VALUES";



			SQL.ExecuteNonQuery();
		}
	}
}