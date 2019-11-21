using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
	}
}