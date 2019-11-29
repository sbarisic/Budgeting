using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Budgeting.Logic {
	public static class Utils {
		const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		static Random Rnd = new Random();

		public static string RandomString(int Len) {
			char[] RndString = new char[Len];

			for (int i = 0; i < RndString.Length; i++) {
				RndString[i] = Chars[Rnd.Next(Chars.Length)];
			}

			return new string(RndString);
		}

		public static string DownloadString(string URL) {
			using (WebClient WC = new WebClient()) {
				return WC.DownloadString(URL);
			}
		}

		public static Currency[] GetCurrenciesOnline() {
			List<Currency> Curs = new List<Currency>();

			string JSONString = DownloadString("https://pkgstore.datahub.io/core/currency-codes/codes-all_json/data/3878667291c4a12eff7f4294013da4e5/codes-all_json.json");
			JArray Result = (JArray)JsonConvert.DeserializeObject(JSONString);

			/*foreach (var E in Result) 
				Curs.Add(new Currency() { Code = E.Key, Name = (string)E.Value });*/

			for (int i = 0; i < Result.Count; i++) {
				JToken Tok = Result[i];

				if (Tok["NumericCode"].Type == JTokenType.Null)
					continue;

				if (Tok["WithdrawalDate"].Type != JTokenType.Null)
					continue;

				Currency Cur = new Currency();
				Cur.Name = Tok.Value<string>("Currency");
				Cur.Num = (int)Tok.Value<float>("NumericCode");
				Cur.Code = Tok.Value<string>("AlphabeticCode");

				if (Cur.Num == 999)
					continue;

				if (Curs.Where(C => C.Num == Cur.Num).Count() > 0)
					continue;

				Curs.Add(Cur);
			}

			return Curs.ToArray();
		}

		public static Tuple<Currency, float>[] GetExchange(Currency Base, IEnumerable<Currency> Symbols) {
			Symbols = Symbols.Where(S => S.Code != Base.Code);
			string Link = string.Format("https://api.exchangeratesapi.io/latest?base={0}&symbols={1}", Base.Code, string.Join(",", Symbols.Select(S => S.Code)));

			string JSONString = DownloadString(Link);
			JObject JSONObj = (JObject)JsonConvert.DeserializeObject(JSONString);

			// JSONObj["rates"]["HRK"].Value<float>()

			List<Tuple<Currency, float>> Results = new List<Tuple<Currency, float>>();

			foreach (var S in Symbols)
				Results.Add(new Tuple<Currency, float>(S, JSONObj["rates"][S.Code].Value<float>()));

			return Results.ToArray();
		}
	}
}