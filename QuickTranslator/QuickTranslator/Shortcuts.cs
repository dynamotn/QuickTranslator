using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuickTranslator
{
	public class Shortcuts
	{
		private static Dictionary<string, string> shortcutDictionary = new Dictionary<string, string>();

		public static void LoadFromFile(string dictPath)
		{
			Shortcuts.shortcutDictionary.Clear();
			using (TextReader textReader = new StreamReader(dictPath, true))
			{
				string text;
				while ((text = textReader.ReadLine()) != null)
				{
					if (text.Split(new char[]
					{
						'='
					}).Length == 2)
					{
						string text2 = text.Split(new char[]
						{
							'='
						})[0];
						string value = text.Split(new char[]
						{
							'='
						})[1];
						if (!Shortcuts.shortcutDictionary.ContainsKey(text2.ToLower()))
						{
							Shortcuts.shortcutDictionary.Add(text2.ToLower(), value);
						}
					}
				}
			}
		}

		public static void SaveDictionaryToFile(string dictPath)
		{
			IOrderedEnumerable<KeyValuePair<string, string>> orderedEnumerable = from pair in Shortcuts.shortcutDictionary
			orderby pair.Key.Length descending, pair.Key
			select pair;
			using (TextWriter textWriter = new StreamWriter(dictPath, false, Encoding.UTF8))
			{
				foreach (KeyValuePair<string, string> current in orderedEnumerable)
				{
					textWriter.WriteLine(current.Key + "=" + current.Value);
				}
			}
		}

		public static string GetValueFromKey(string key)
		{
			if (Shortcuts.shortcutDictionary.ContainsKey(key.ToLower()))
			{
				return Shortcuts.shortcutDictionary[key.ToLower()];
			}
			return null;
		}
	}
}
