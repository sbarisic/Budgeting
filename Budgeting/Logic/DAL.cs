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

		public DataTable Query(string TableName, SQLiteParameter[] Params) {
			Open();

			string SelectQuery = "select * from " + TableName;

			if (Params.Length > 0) {
				SelectQuery = SelectQuery + " where ";
				List<string> Conditions = new List<string>();

				foreach (var P in Params)
					Conditions.Add(string.Format("{0} = {1}", P.ParameterName.Substring(1), P.ParameterName));

				SelectQuery = SelectQuery + string.Join(" and ", Conditions);
			}

			using (SQLiteCommand Cmd = Con.CreateCommand())
			using (SQLiteDataAdapter Adapter = new SQLiteDataAdapter(SelectQuery, Con)) {
				Adapter.SelectCommand.Parameters.AddRange(Params);

				DataSet.Reset();
				Adapter.Fill(DataSet);
				DataTable = DataSet.Tables[0];
			}

			Close();
			return DataTable;
		}

		IEnumerable<T> Select<T>(string TableName, SQLiteParameter[] Conditions) where T : class, new() {
			DataTable Table = Query(TableName, Conditions);
			int RowCount = Table.Rows.Count;
			FieldInfo[] Fields = DALFieldAttribute.GetDALFields<T>();

			for (int i = 0; i < RowCount; i++) {
				T Obj = new T();

				for (int j = 0; j < Fields.Length; j++) {
					string Name = Fields[j].GetCustomAttribute<DALFieldAttribute>().ColumnName;
					object ColVal = Table.Rows[i][Name];

					if (ColVal is DBNull)
						ColVal = null;

					if (ColVal != null && ColVal.GetType() != Fields[j].FieldType)
						ColVal = Convert.ChangeType(ColVal, Fields[j].FieldType);

					Fields[j].SetValue(Obj, ColVal);
				}

				yield return Obj;
			}

			yield break;
		}

		public IEnumerable<T> Select<T>(params SQLiteParameter[] Conditions) where T : class, new() {
			return Select<T>(DALTableAttribute.GetTableName<T>(), Conditions);
		}

		void Insert<T>(string TableName, T Val) where T : class {
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
		}

		public void Insert<T>(T Val) where T : class {
			Insert(DALTableAttribute.GetTableName<T>(), Val);
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