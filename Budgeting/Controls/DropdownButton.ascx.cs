using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Budgeting.Controls {
	public class DropdownEntry {
		public string ID;
		public string Name;
		public Action OnClick;

		public DropdownEntry(string Name, Action OnClick = null) {
			this.Name = Name;
			this.OnClick = OnClick;
		}
	}

	public partial class DropdownButton : System.Web.UI.UserControl {
		List<DropdownEntry> Entries = new List<DropdownEntry>();

		public event Action<DropdownEntry> OnSelect;

		public DropdownButton() : base() {
			Attributes.Add("Value", "Dropdown");
		}

		protected void Page_Load(object sender, EventArgs e) {
			btnDropdown.InnerText = Attributes["Value"];
		}

		public void Clear() {
			Entries.Clear();
		}

		public void AddItem(DropdownEntry Entry) {
			Entry.ID = Entries.Count.ToString();
			Entries.Add(Entry);
		}

		public void Bind() {
			repDropdownItems.DataSource = Entries;
			repDropdownItems.DataBind();
		}

		protected void Dropdown_Click(object sender, EventArgs e) {
			int Idx = int.Parse(((Button)sender).CommandName);

			DropdownEntry Entry = Entries[Idx];
			OnSelect?.Invoke(Entry);
			Entry.OnClick?.Invoke();
		}

		protected override void Render(HtmlTextWriter writer) {
			foreach (var E in Entries) {
				Page.ClientScript.RegisterForEventValidation(E.ID);
			}

			base.Render(writer);
		}
	}
}