using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SQLite;
using System.Data;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace Budgeting.Logic {
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	sealed class DALFieldAttribute : Attribute {
		public string ColumnName;

		public DALFieldAttribute(string ColumnName) {
			this.ColumnName = ColumnName;
		}

		public static FieldInfo[] GetDALFields(Type T) {
			return T.GetFields().Where(F => F.GetCustomAttribute<DALFieldAttribute>() != null).ToArray();
		}

		public static FieldInfo[] GetDALFields<T>() {
			return GetDALFields(typeof(T));
		}
	}

	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
	sealed class DALTableAttribute : Attribute {
		public string TableName;

		public DALTableAttribute(string TableName) {
			this.TableName = TableName;
		}

		public static string GetTableName<T>() {
			return typeof(T).GetCustomAttribute<DALTableAttribute>().TableName;
		}
	}

	public class DAL {
		public enum QueryMode {
			AND,
			OR
		}

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

		public DataTable Query(string TableName, SQLiteParameter[] Params, QueryMode Mode, int LimitCount = 0) {
			Open();

			string SelectQuery = "select * from " + TableName;

			if (Params != null && Params.Length > 0) {
				SelectQuery = SelectQuery + " where ";
				List<string> Conditions = new List<string>();

				foreach (var P in Params)
					Conditions.Add(string.Format("{0} = {1}", P.ParameterName.Substring(1), P.ParameterName));

				SelectQuery = SelectQuery + string.Join(string.Format(" {0} ", Mode), Conditions);
			}

			if (LimitCount > 0)
				SelectQuery = string.Format("{0} order by rowid desc limit {1}", SelectQuery, LimitCount);

			using (SQLiteCommand Cmd = Con.CreateCommand())
			using (SQLiteDataAdapter Adapter = new SQLiteDataAdapter(SelectQuery, Con)) {
				if (Params != null)
					Adapter.SelectCommand.Parameters.AddRange(Params);

				DataSet.Reset();
				Adapter.Fill(DataSet);
				DataTable = DataSet.Tables[0];
			}

			Close();
			return DataTable;
		}

		IEnumerable<T> Select<T>(string TableName, SQLiteParameter[] Conditions, QueryMode Mode = QueryMode.AND) where T : DbEntry, new() {
			DataTable Table = Query(TableName, Conditions, Mode);
			int RowCount = Table.Rows.Count;
			FieldInfo[] Fields = DALFieldAttribute.GetDALFields<T>();

			for (int i = 0; i < RowCount; i++) {
				T Obj = new T();

				for (int j = 0; j < Fields.Length; j++) {
					string Name = Fields[j].GetCustomAttribute<DALFieldAttribute>().ColumnName;
					object ColVal = Table.Rows[i][Name];

					if (ColVal is DBNull)
						ColVal = null;

					Type FT = Fields[j].FieldType;

					if (ColVal != null && ColVal.GetType() != FT) {
						Type Underlying;

						if ((Underlying = Nullable.GetUnderlyingType(FT)) != null) {
							if (ColVal != null) {
								ColVal = Convert.ChangeType(ColVal, Underlying);
								ColVal = Activator.CreateInstance(FT, new[] { ColVal });
							}
						} else
							ColVal = Convert.ChangeType(ColVal, FT);
					}

					Fields[j].SetValue(Obj, ColVal);
				}

				yield return Obj;
			}

			yield break;
		}

		public IEnumerable<T> Select<T>(SQLiteParameter[] Conditions = null, QueryMode Mode = QueryMode.AND) where T : DbEntry, new() {
			return Select<T>(DALTableAttribute.GetTableName<T>(), Conditions ?? new SQLiteParameter[] { }, Mode);
		}

		void Insert<T>(string TableName, T Val) where T : DbEntry {
			Open();

			List<SQLiteParameter> ValueColumns = new List<SQLiteParameter>();
			FieldInfo[] Fields = DALFieldAttribute.GetDALFields<T>();

			foreach (var F in Fields) {
				DALFieldAttribute DALField = F.GetCustomAttribute<DALFieldAttribute>();

				if (DALField.ColumnName == "id")
					continue;

				object ObjVal = F.GetValue(Val);

				if (ObjVal != null) {
					SQLiteParameter Param = new SQLiteParameter("@" + DALField.ColumnName);
					Param.Value = ObjVal;

					ValueColumns.Add(Param);
				}
			}

			using (SQLiteCommand SQL = Con.CreateCommand()) {
				string ParamNames = string.Join(", ", ValueColumns.Select(C => C.ParameterName.Substring(1)));
				string ParamValues = string.Join(", ", ValueColumns.Select(C => C.ParameterName));

				SQL.CommandText = string.Format("INSERT INTO {0}({1}) VALUES({2})", TableName, ParamNames, ParamValues);
				SQL.Parameters.AddRange(ValueColumns.ToArray());
				SQL.ExecuteNonQuery();
			}

			Close();

			using (DataTable DT = Query(TableName, null, QueryMode.AND, 1)) {
				int ID = (int)DT.Rows[0].Field<long>("id");
				typeof(T).GetField("ID").SetValue(Val, ID);
			}
		}

		public void Insert<T>(T Val) where T : DbEntry {
			Insert(DALTableAttribute.GetTableName<T>(), Val);
		}

		public void Insert<T>(IEnumerable<T> Vals) where T : DbEntry {
			foreach (var V in Vals)
				Insert(V);
		}

		public void Update<T>(T Val) where T : class {
			Open();
			DbEntry ValEntry = Val as DbEntry;

			List<SQLiteParameter> Params = new List<SQLiteParameter>();
			FieldInfo[] Fields = DALFieldAttribute.GetDALFields<T>();

			foreach (var F in Fields) {
				DALFieldAttribute DALField = F.GetCustomAttribute<DALFieldAttribute>();
				Params.Add(ValEntry.CreateParameter(DALField.ColumnName));
			}

			using (SQLiteCommand SQL = Con.CreateCommand()) {
				string NewValues = string.Join(", ", Params.Select(C => C.ParameterName.Substring(1) + " = " + C.ParameterName));
				string Conditions = string.Join(", ", Params.Select(C => C.ParameterName));

				SQL.CommandText = string.Format("UPDATE {0} SET {1} WHERE id = @id", DALTableAttribute.GetTableName<T>(), NewValues);
				SQL.Parameters.Add(ValEntry.CreateParameterID());
				SQL.Parameters.AddRange(Params.ToArray());

				SQL.ExecuteNonQuery();
			}

			Close();
		}

		public void Delete<T>(T Val) where T : class {
			Open();
			DbEntry ValEntry = Val as DbEntry;

			using (SQLiteCommand SQL = Con.CreateCommand()) {
				SQL.CommandText = string.Format("DELETE FROM {0} WHERE id = @id", DALTableAttribute.GetTableName<T>());
				SQL.Parameters.Add(ValEntry.CreateParameterID());
				SQL.ExecuteNonQuery();
			}

			Close();
		}
	}
}