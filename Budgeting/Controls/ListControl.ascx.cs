using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Budgeting.Controls {
	public class ListControlEntry {
		public string Value {
			get; set;
		}

		public ItemColor Color {
			get; set;
		}

		public string EntryCSS {
			get {
				string TextColor = "text-light";

				switch (Color) {
					case ItemColor.Green:
						TextColor = "text-success";
						break;

					case ItemColor.Red:
						TextColor = "text-danger";
						break;

					case ItemColor.Primary:
						TextColor = "text-primary";
						break;

					case ItemColor.Info:
						TextColor = "text-info";
						break;
				}

				return string.Format("list-group-item list-group-item-dark {0}", TextColor);
			}
		}

		public ListControlEntry(string Value, ItemColor Color) {
			this.Value = Value;
			this.Color = Color;
		}
	}

	public enum ItemColor {
		Default,
		Green,
		Red,
		Primary,
		Info
	}

	public partial class ListControl : System.Web.UI.UserControl {
		public object UserData;

		List<ListControlEntry> Entries = new List<ListControlEntry>();

		protected void Page_Load(object sender, EventArgs e) {
			ListControlList.DataSource = Entries;
			RefreshList();
		}

		void RefreshList() {
			ListControlList.DataBind();
		}

		public void AddItem(string Entry, ItemColor Clr = ItemColor.Default) {
			Entries.Add(new ListControlEntry(Entry, Clr));
		}

		public void Clear() {
			Entries.Clear();
		}
	}
}