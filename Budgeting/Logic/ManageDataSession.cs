using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data.SQLite;
using System.Web.SessionState;

namespace Budgeting.Logic {
	public enum ManageDataState {
		None = 0,
		Main,
		AddSingle,
		AddMultiple,
		AddRepeating,
		AddMaestroPlus,
		ManageMaestroPlus
	}

	public class ManageDataSession {
		public ManageDataState State;
		public List<MainRadioButton> MainRadioButtons = new List<MainRadioButton>();

		public ManageDataSession() {
			State = ManageDataState.Main;
		}

		public static ManageDataSession Get(Page P) {
			return Get(P.Session);
		}

		public static ManageDataSession Get(MasterPage P) {
			return Get(P.Session);
		}

		static ManageDataSession Get(HttpSessionState Session) {
			object BS = Session[nameof(ManageDataSession)];

			if (BS is ManageDataSession S)
				return S;

			return (ManageDataSession)(Session[nameof(ManageDataSession)] = new ManageDataSession());
		}
	}
}