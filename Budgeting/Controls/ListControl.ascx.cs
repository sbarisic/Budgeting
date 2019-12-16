using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Budgeting.Controls {
	public delegate void OnListControlEntryClick(HttpServerUtility Server, HttpRequest Request, ListControlEntry Entry);

	public class ListControlEntry {
		public string Value {
			get; set;
		}

		public string Argument {
			get; set;
		}

		public ItemColor Color {
			get; set;
		}

		public bool Clickable {
			get; set;
		}

		public OnListControlEntryClick OnClick {
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

					case ItemColor.Orange:
						TextColor = "text-warning";
						break;
				}

				return string.Format("list-group-item list-group-item-dark {0}", TextColor);
			}
		}

		public ListControlEntry(string Value, ItemColor Color, string Argument = null, OnListControlEntryClick OnClick = null) {
			this.Value = Value;
			this.Color = Color;
			this.OnClick = OnClick;
			this.Argument = Argument;
			Clickable = OnClick != null;
		}
	}

	public enum ItemColor {
		Default,
		Green,
		Red,
		Primary,
		Info,
		Orange
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

		public void AddItem(string Entry, ItemColor Clr = ItemColor.Default, string UniqueID = null, OnListControlEntryClick OnClick = null) {
			Entries.Add(new ListControlEntry(Entry, Clr, UniqueID, OnClick));
		}

		public void Clear() {
			Entries.Clear();
		}

		protected void OnItem_Click(object sender, EventArgs e) {
			if (sender is LinkButton LBtn && LBtn.CommandName == "OnClick")
				foreach (var E in Entries)
					if (E.Argument == LBtn.CommandArgument)
						E.OnClick?.Invoke(Server, Request, E);
		}
	}
}