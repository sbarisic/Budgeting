using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data.SQLite;
using System.Web.SessionState;

namespace Budgeting.Logic {
	public class BudgetSession {
		public Currency ExchangeCurrency;

		public static BudgetSession Get(Page P) {
			return Get(P.Session);
		}

		public static BudgetSession Get(MasterPage P) {
			return Get(P.Session);
		}

		static BudgetSession Get(HttpSessionState Session) {
			object BS = Session[nameof(BudgetSession)];

			if (BS is BudgetSession S)
				return S;

			return (BudgetSession)(Session[nameof(BudgetSession)] = new BudgetSession());
		}

		public User CurrentUser;

		public bool LogIn(DAL DbDAL, string Username, string Password) {
			User Usr = GetUser(DbDAL, Username);

			if (Usr == null)
				return false;

			if (PasswordManager.IsValidPassword(Password, Usr.Salt, Usr.Hash)) {
				DbDAL.Insert(new UserLogin(Usr));
				CurrentUser = Usr;
				return true;
			}

			return false;
		}

		public void LogOut() {
			CurrentUser = null;
		}

		public bool Authenticated() {
			return CurrentUser != null;
		}

		public static User GetUser(DAL DbDAL, string Username) {
			return DbDAL.Select<User>(new[] { new SQLiteParameter("@user", Username) }).FirstOrDefault();
		}

		public static void CreateUser(DAL DbDAL, string Username) {
			User NewUser = new User();
			NewUser.Username = Username;
			DbDAL.Insert(NewUser);
		}

		public static void ResetPassword(DAL DbDAL, string Username, string Password) {
			User Usr = GetUser(DbDAL, Username);
			PasswordManager.GenerateSaltHashPair(Password, out Usr.Hash, out Usr.Salt);
			DbDAL.Update(Usr);
		}
	}
}