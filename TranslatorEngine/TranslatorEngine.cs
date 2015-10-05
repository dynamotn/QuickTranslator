using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace TranslatorEngine
{
	public class TranslatorEngine
	{
		public const int CHINESE_LOOKUP_MAX_LENGTH = 20;

		private static bool dictionaryDirty = true;

		private static Dictionary<string, string> hanVietDictionary = new Dictionary<string, string>();

		private static Dictionary<string, string> vietPhraseDictionary = new Dictionary<string, string>();

		private static Dictionary<string, string> thieuChuuDictionary = new Dictionary<string, string>();

		private static Dictionary<string, string> lacVietDictionary = new Dictionary<string, string>();

		private static Dictionary<string, string> cedictDictionary = new Dictionary<string, string>();

		private static Dictionary<string, string> chinesePhienAmEnglishDictionary = new Dictionary<string, string>();

		private static Dictionary<string, string> vietPhraseOneMeaningDictionary = new Dictionary<string, string>();

		private static Dictionary<string, string> onlyVietPhraseDictionary = new Dictionary<string, string>();

		private static Dictionary<string, string> onlyNameDictionary = new Dictionary<string, string>();

		private static DataSet onlyVietPhraseDictionaryHistoryDataSet = new DataSet();

		private static DataSet onlyNameDictionaryHistoryDataSet = new DataSet();

		private static DataSet hanVietDictionaryHistoryDataSet = new DataSet();

		private static List<string> ignoredChinesePhraseList = new List<string>();

		private static List<string> ignoredChinesePhraseForBrowserList = new List<string>();

		private static object lockObject = new object();

		public static string LastTranslatedWord_HanViet = "";

		public static string LastTranslatedWord_VietPhrase = "";

		public static string LastTranslatedWord_VietPhraseOneMeaning = "";

		private static char[] trimCharsForAnalyzer = new char[]
		{
			' ',
			'\n',
			'\t'
		};

		public static bool DictionaryDirty
		{
			get
			{
				return TranslatorEngine.dictionaryDirty;
			}
			set
			{
				TranslatorEngine.dictionaryDirty = value;
			}
		}

		public static string GetVietPhraseValueFromKey(string key)
		{
			if (!TranslatorEngine.onlyVietPhraseDictionary.ContainsKey(key))
			{
				return null;
			}
			return TranslatorEngine.onlyVietPhraseDictionary[key];
		}

		public static string GetNameValueFromKey(string key)
		{
			if (!TranslatorEngine.onlyNameDictionary.ContainsKey(key))
			{
				return null;
			}
			return TranslatorEngine.onlyNameDictionary[key];
		}

		public static void DeleteKeyFromVietPhraseDictionary(string key, bool sorting)
		{
			TranslatorEngine.vietPhraseDictionary.Remove(key);
			TranslatorEngine.vietPhraseOneMeaningDictionary.Remove(key);
			TranslatorEngine.onlyVietPhraseDictionary.Remove(key);
			if (sorting)
			{
				TranslatorEngine.SaveDictionaryToFile(ref TranslatorEngine.onlyVietPhraseDictionary, DictionaryConfigurationHelper.GetVietPhraseDictionaryPath());
			}
			else
			{
				TranslatorEngine.SaveDictionaryToFileWithoutSorting(TranslatorEngine.onlyVietPhraseDictionary, DictionaryConfigurationHelper.GetVietPhraseDictionaryPath());
			}
			TranslatorEngine.writeVietPhraseHistoryLog(key, "Deleted");
		}

		public static void DeleteKeyFromNameDictionary(string key, bool sorting)
		{
			TranslatorEngine.vietPhraseDictionary.Remove(key);
			TranslatorEngine.vietPhraseOneMeaningDictionary.Remove(key);
			TranslatorEngine.onlyNameDictionary.Remove(key);
			if (sorting)
			{
				TranslatorEngine.SaveDictionaryToFile(ref TranslatorEngine.onlyNameDictionary, DictionaryConfigurationHelper.GetNamesDictionaryPath());
			}
			else
			{
				TranslatorEngine.SaveDictionaryToFileWithoutSorting(TranslatorEngine.onlyNameDictionary, DictionaryConfigurationHelper.GetNamesDictionaryPath());
			}
			TranslatorEngine.writeNamesHistoryLog(key, "Deleted");
		}

		public static void DeleteKeyFromPhienAmDictionary(string key, bool sorting)
		{
			TranslatorEngine.hanVietDictionary.Remove(key);
			if (sorting)
			{
				TranslatorEngine.SaveDictionaryToFile(ref TranslatorEngine.hanVietDictionary, DictionaryConfigurationHelper.GetChinesePhienAmWordsDictionaryPath());
			}
			else
			{
				TranslatorEngine.SaveDictionaryToFileWithoutSorting(TranslatorEngine.hanVietDictionary, DictionaryConfigurationHelper.GetChinesePhienAmWordsDictionaryPath());
			}
			TranslatorEngine.writePhienAmHistoryLog(key, "Deleted");
		}

		public static void UpdateVietPhraseDictionary(string key, string value, bool sorting)
		{
			if (TranslatorEngine.vietPhraseDictionary.ContainsKey(key))
			{
				TranslatorEngine.vietPhraseDictionary[key] = value;
			}
			else
			{
				TranslatorEngine.vietPhraseDictionary.Add(key, value);
			}
			if (TranslatorEngine.vietPhraseOneMeaningDictionary.ContainsKey(key))
			{
				TranslatorEngine.vietPhraseOneMeaningDictionary[key] = value.Split(new char[]
				{
					'/',
					'|'
				})[0];
			}
			else
			{
				TranslatorEngine.vietPhraseOneMeaningDictionary.Add(key, value.Split(new char[]
				{
					'/',
					'|'
				})[0]);
			}
			if (TranslatorEngine.onlyVietPhraseDictionary.ContainsKey(key))
			{
				TranslatorEngine.onlyVietPhraseDictionary[key] = value;
				TranslatorEngine.writeVietPhraseHistoryLog(key, "Updated");
			}
			else
			{
				if (sorting)
				{
					TranslatorEngine.onlyVietPhraseDictionary.Add(key, value);
				}
				else
				{
					TranslatorEngine.onlyVietPhraseDictionary = TranslatorEngine.AddEntryToDictionaryWithoutSorting(TranslatorEngine.onlyVietPhraseDictionary, key, value);
				}
				TranslatorEngine.writeVietPhraseHistoryLog(key, "Added");
			}
			if (sorting)
			{
				TranslatorEngine.SaveDictionaryToFile(ref TranslatorEngine.onlyVietPhraseDictionary, DictionaryConfigurationHelper.GetVietPhraseDictionaryPath());
				return;
			}
			TranslatorEngine.SaveDictionaryToFileWithoutSorting(TranslatorEngine.onlyVietPhraseDictionary, DictionaryConfigurationHelper.GetVietPhraseDictionaryPath());
		}

		private static Dictionary<string, string> AddEntryToDictionaryWithoutSorting(Dictionary<string, string> dictionary, string key, string value)
		{
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			foreach (KeyValuePair<string, string> current in dictionary)
			{
				dictionary2.Add(current.Key, current.Value);
			}
			dictionary2.Add(key, value);
			return dictionary2;
		}

		public static void UpdateNameDictionary(string key, string value, bool sorting)
		{
			if (TranslatorEngine.vietPhraseDictionary.ContainsKey(key))
			{
				TranslatorEngine.vietPhraseDictionary[key] = value;
			}
			else
			{
				TranslatorEngine.vietPhraseDictionary.Add(key, value);
			}
			if (TranslatorEngine.vietPhraseOneMeaningDictionary.ContainsKey(key))
			{
				TranslatorEngine.vietPhraseOneMeaningDictionary[key] = value.Split(new char[]
				{
					'/',
					'|'
				})[0];
			}
			else
			{
				TranslatorEngine.vietPhraseOneMeaningDictionary.Add(key, value.Split(new char[]
				{
					'/',
					'|'
				})[0]);
			}
			if (TranslatorEngine.onlyNameDictionary.ContainsKey(key))
			{
				TranslatorEngine.onlyNameDictionary[key] = value;
				TranslatorEngine.writeNamesHistoryLog(key, "Updated");
			}
			else
			{
				if (sorting)
				{
					TranslatorEngine.onlyNameDictionary.Add(key, value);
				}
				else
				{
					TranslatorEngine.onlyNameDictionary = TranslatorEngine.AddEntryToDictionaryWithoutSorting(TranslatorEngine.onlyNameDictionary, key, value);
				}
				TranslatorEngine.writeNamesHistoryLog(key, "Added");
			}
			if (sorting)
			{
				TranslatorEngine.SaveDictionaryToFile(ref TranslatorEngine.onlyNameDictionary, DictionaryConfigurationHelper.GetNamesDictionaryPath());
				return;
			}
			TranslatorEngine.SaveDictionaryToFileWithoutSorting(TranslatorEngine.onlyNameDictionary, DictionaryConfigurationHelper.GetNamesDictionaryPath());
		}

		public static void UpdatePhienAmDictionary(string key, string value, bool sorting)
		{
			if (TranslatorEngine.hanVietDictionary.ContainsKey(key))
			{
				TranslatorEngine.hanVietDictionary[key] = value;
				TranslatorEngine.writePhienAmHistoryLog(key, "Updated");
			}
			else
			{
				if (sorting)
				{
					TranslatorEngine.hanVietDictionary.Add(key, value);
				}
				else
				{
					TranslatorEngine.hanVietDictionary = TranslatorEngine.AddEntryToDictionaryWithoutSorting(TranslatorEngine.hanVietDictionary, key, value);
				}
				TranslatorEngine.writePhienAmHistoryLog(key, "Added");
			}
			if (sorting)
			{
				TranslatorEngine.SaveDictionaryToFile(ref TranslatorEngine.hanVietDictionary, DictionaryConfigurationHelper.GetChinesePhienAmWordsDictionaryPath());
				return;
			}
			TranslatorEngine.SaveDictionaryToFileWithoutSorting(TranslatorEngine.hanVietDictionary, DictionaryConfigurationHelper.GetChinesePhienAmWordsDictionaryPath());
		}

		public static void SaveDictionaryToFileWithoutSorting(Dictionary<string, string> dictionary, string filePath)
		{
			string text = filePath + "." + DateTime.Now.Ticks;
			if (File.Exists(filePath))
			{
				File.Copy(filePath, text, true);
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> current in dictionary)
			{
				stringBuilder.Append(current.Key).Append("=").AppendLine(current.Value);
			}
			try
			{
				File.WriteAllText(filePath, stringBuilder.ToString(), Encoding.UTF8);
			}
			catch (Exception ex)
			{
				try
				{
					File.Copy(text, filePath, true);
				}
				catch
				{
				}
				throw ex;
			}
			finally
			{
				if (File.Exists(filePath))
				{
					File.Delete(text);
				}
			}
		}

		public static void SaveDictionaryToFile(ref Dictionary<string, string> dictionary, string filePath)
		{
			IOrderedEnumerable<KeyValuePair<string, string>> orderedEnumerable = from pair in dictionary
			orderby pair.Key.Length descending, pair.Key
			select pair;
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			string text = filePath + "." + DateTime.Now.Ticks;
			if (File.Exists(filePath))
			{
				File.Copy(filePath, text, true);
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> current in orderedEnumerable)
			{
				stringBuilder.Append(current.Key).Append("=").AppendLine(current.Value);
				dictionary2.Add(current.Key, current.Value);
			}
			dictionary = dictionary2;
			try
			{
				File.WriteAllText(filePath, stringBuilder.ToString(), Encoding.UTF8);
			}
			catch (Exception ex)
			{
				try
				{
					File.Copy(text, filePath, true);
				}
				catch
				{
				}
				throw ex;
			}
			finally
			{
				if (File.Exists(filePath))
				{
					File.Delete(text);
				}
			}
		}

		public static string ChineseToHanViet(string chinese, out CharRange[] chineseHanVietMappingArray)
		{
			TranslatorEngine.LastTranslatedWord_HanViet = "";
			List<CharRange> list = new List<CharRange>();
			StringBuilder stringBuilder = new StringBuilder();
			int length = chinese.Length;
			for (int i = 0; i < length - 1; i++)
			{
				int length2 = stringBuilder.ToString().Length;
				char c = chinese[i];
				char character = chinese[i + 1];
				if (TranslatorEngine.isChinese(c))
				{
					if (TranslatorEngine.isChinese(character))
					{
						TranslatorEngine.appendTranslatedWord(stringBuilder, TranslatorEngine.ChineseToHanViet(c), ref TranslatorEngine.LastTranslatedWord_HanViet, ref length2);
						stringBuilder.Append(" ");
						TranslatorEngine.LastTranslatedWord_HanViet += " ";
						list.Add(new CharRange(length2, TranslatorEngine.ChineseToHanViet(c).Length));
					}
					else
					{
						TranslatorEngine.appendTranslatedWord(stringBuilder, TranslatorEngine.ChineseToHanViet(c), ref TranslatorEngine.LastTranslatedWord_HanViet, ref length2);
						list.Add(new CharRange(length2, TranslatorEngine.ChineseToHanViet(c).Length));
					}
				}
				else
				{
					stringBuilder.Append(c);
					TranslatorEngine.LastTranslatedWord_HanViet += c.ToString();
					list.Add(new CharRange(length2, 1));
				}
			}
			if (TranslatorEngine.isChinese(chinese[length - 1]))
			{
				TranslatorEngine.appendTranslatedWord(stringBuilder, TranslatorEngine.ChineseToHanViet(chinese[length - 1]), ref TranslatorEngine.LastTranslatedWord_HanViet);
				list.Add(new CharRange(stringBuilder.ToString().Length, TranslatorEngine.ChineseToHanViet(chinese[length - 1]).Length));
			}
			else
			{
				stringBuilder.Append(chinese[length - 1]);
				TranslatorEngine.LastTranslatedWord_HanViet += chinese[length - 1].ToString();
				list.Add(new CharRange(stringBuilder.ToString().Length, 1));
			}
			chineseHanVietMappingArray = list.ToArray();
			TranslatorEngine.LastTranslatedWord_HanViet = "";
			return stringBuilder.ToString();
		}

		public static string ChineseToHanVietForBrowser(string chinese)
		{
			if (string.IsNullOrEmpty(chinese))
			{
				return "";
			}
			chinese = TranslatorEngine.StandardizeInputForBrowser(chinese);
			StringBuilder stringBuilder = new StringBuilder();
			string[] array = TranslatorEngine.classifyWordsIntoLatinAndChinese(chinese);
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				if (!string.IsNullOrEmpty(text))
				{
					if (TranslatorEngine.isChinese(text[0]))
					{
						CharRange[] array3;
						stringBuilder.Append(TranslatorEngine.ChineseToHanViet(text, out array3));
					}
					else
					{
						stringBuilder.Append(text);
					}
				}
			}
			return stringBuilder.ToString();
		}

		public static string ChineseToHanVietForBatch(string chinese)
		{
			string str = "";
			StringBuilder stringBuilder = new StringBuilder();
			int length = chinese.Length;
			for (int i = 0; i < length - 1; i++)
			{
				char c = chinese[i];
				char character = chinese[i + 1];
				if (TranslatorEngine.isChinese(c))
				{
					if (TranslatorEngine.isChinese(character))
					{
						TranslatorEngine.appendTranslatedWord(stringBuilder, TranslatorEngine.ChineseToHanViet(c), ref str);
						stringBuilder.Append(" ");
						str += " ";
					}
					else
					{
						TranslatorEngine.appendTranslatedWord(stringBuilder, TranslatorEngine.ChineseToHanViet(c), ref str);
					}
				}
				else
				{
					stringBuilder.Append(c);
					str += c.ToString();
				}
			}
			if (TranslatorEngine.isChinese(chinese[length - 1]))
			{
				TranslatorEngine.appendTranslatedWord(stringBuilder, TranslatorEngine.ChineseToHanViet(chinese[length - 1]), ref str);
			}
			else
			{
				stringBuilder.Append(chinese[length - 1]);
				str += chinese[length - 1].ToString();
			}
			return stringBuilder.ToString();
		}

		public static string ChineseToHanViet(char chinese)
		{
			if (chinese == ' ')
			{
				return "";
			}
			if (!TranslatorEngine.hanVietDictionary.ContainsKey(chinese.ToString()))
			{
				return TranslatorEngine.ToNarrow(chinese.ToString());
			}
			return TranslatorEngine.hanVietDictionary[chinese.ToString()];
		}

		public static string ChineseToVietPhrase(string chinese, int wrapType, int translationAlgorithm, bool prioritizedName, out CharRange[] chinesePhraseRanges, out CharRange[] vietPhraseRanges)
		{
			TranslatorEngine.LastTranslatedWord_VietPhrase = "";
			List<CharRange> list = new List<CharRange>();
			List<CharRange> list2 = new List<CharRange>();
			StringBuilder stringBuilder = new StringBuilder();
			int num = chinese.Length - 1;
			int i = 0;
			while (i <= num)
			{
				bool flag = false;
				for (int j = 20; j > 0; j--)
				{
					if (chinese.Length >= i + j && TranslatorEngine.vietPhraseDictionary.ContainsKey(chinese.Substring(i, j)) && (!prioritizedName || !TranslatorEngine.containsName(chinese, i, j)) && ((translationAlgorithm != 0 && translationAlgorithm != 2) || TranslatorEngine.isLongestPhraseInSentence(chinese, i, j, TranslatorEngine.vietPhraseDictionary, translationAlgorithm)))
					{
						list.Add(new CharRange(i, j));
						if (wrapType == 0)
						{
							TranslatorEngine.appendTranslatedWord(stringBuilder, TranslatorEngine.vietPhraseDictionary[chinese.Substring(i, j)], ref TranslatorEngine.LastTranslatedWord_VietPhrase);
							list2.Add(new CharRange(stringBuilder.ToString().Length - TranslatorEngine.vietPhraseDictionary[chinese.Substring(i, j)].Length, TranslatorEngine.vietPhraseDictionary[chinese.Substring(i, j)].Length));
						}
						else if (wrapType == 1 || wrapType == 11)
						{
							TranslatorEngine.appendTranslatedWord(stringBuilder, "[" + TranslatorEngine.vietPhraseDictionary[chinese.Substring(i, j)] + "]", ref TranslatorEngine.LastTranslatedWord_VietPhrase);
							list2.Add(new CharRange(stringBuilder.ToString().Length - TranslatorEngine.vietPhraseDictionary[chinese.Substring(i, j)].Length - 2, TranslatorEngine.vietPhraseDictionary[chinese.Substring(i, j)].Length + 2));
						}
						else if (TranslatorEngine.hasOnlyOneMeaning(TranslatorEngine.vietPhraseDictionary[chinese.Substring(i, j)]))
						{
							TranslatorEngine.appendTranslatedWord(stringBuilder, TranslatorEngine.vietPhraseDictionary[chinese.Substring(i, j)], ref TranslatorEngine.LastTranslatedWord_VietPhrase);
							list2.Add(new CharRange(stringBuilder.ToString().Length - TranslatorEngine.vietPhraseDictionary[chinese.Substring(i, j)].Length, TranslatorEngine.vietPhraseDictionary[chinese.Substring(i, j)].Length));
						}
						else
						{
							TranslatorEngine.appendTranslatedWord(stringBuilder, "[" + TranslatorEngine.vietPhraseDictionary[chinese.Substring(i, j)] + "]", ref TranslatorEngine.LastTranslatedWord_VietPhrase);
							list2.Add(new CharRange(stringBuilder.ToString().Length - TranslatorEngine.vietPhraseDictionary[chinese.Substring(i, j)].Length - 2, TranslatorEngine.vietPhraseDictionary[chinese.Substring(i, j)].Length + 2));
						}
						if (TranslatorEngine.nextCharIsChinese(chinese, i + j - 1))
						{
							stringBuilder.Append(" ");
							TranslatorEngine.LastTranslatedWord_VietPhrase += " ";
						}
						flag = true;
						i += j;
						break;
					}
				}
				if (!flag)
				{
					int length = stringBuilder.ToString().Length;
					int num2 = TranslatorEngine.ChineseToHanViet(chinese[i]).Length;
					list.Add(new CharRange(i, 1));
					if (TranslatorEngine.isChinese(chinese[i]))
					{
						TranslatorEngine.appendTranslatedWord(stringBuilder, ((wrapType != 1) ? "" : "[") + TranslatorEngine.ChineseToHanViet(chinese[i]) + ((wrapType != 1) ? "" : "]"), ref TranslatorEngine.LastTranslatedWord_VietPhrase);
						if (TranslatorEngine.nextCharIsChinese(chinese, i))
						{
							stringBuilder.Append(" ");
							TranslatorEngine.LastTranslatedWord_VietPhrase += " ";
						}
						num2 += ((wrapType != 1) ? 0 : 2);
					}
					else if ((chinese[i] == '"' || chinese[i] == '\'') && !TranslatorEngine.LastTranslatedWord_VietPhrase.EndsWith(" ") && !TranslatorEngine.LastTranslatedWord_VietPhrase.EndsWith(".") && !TranslatorEngine.LastTranslatedWord_VietPhrase.EndsWith("?") && !TranslatorEngine.LastTranslatedWord_VietPhrase.EndsWith("!") && !TranslatorEngine.LastTranslatedWord_VietPhrase.EndsWith("\t") && i < chinese.Length - 1 && chinese[i + 1] != ' ' && chinese[i + 1] != ',')
					{
						stringBuilder.Append(" ").Append(chinese[i]);
						TranslatorEngine.LastTranslatedWord_VietPhrase = TranslatorEngine.LastTranslatedWord_VietPhrase + " " + chinese[i].ToString();
					}
					else
					{
						stringBuilder.Append(chinese[i]);
						TranslatorEngine.LastTranslatedWord_VietPhrase += chinese[i].ToString();
						num2 = 1;
					}
					list2.Add(new CharRange(length, num2));
					i++;
				}
			}
			chinesePhraseRanges = list.ToArray();
			vietPhraseRanges = list2.ToArray();
			TranslatorEngine.LastTranslatedWord_VietPhrase = "";
			return stringBuilder.ToString();
		}

		public static string ChineseToVietPhraseForBrowser(string chinese, int wrapType, int translationAlgorithm, bool prioritizedName)
		{
			chinese = TranslatorEngine.StandardizeInputForBrowser(chinese);
			StringBuilder stringBuilder = new StringBuilder();
			string[] array = TranslatorEngine.classifyWordsIntoLatinAndChinese(chinese);
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				if (!string.IsNullOrEmpty(text))
				{
					if (TranslatorEngine.isChinese(text[0]))
					{
						CharRange[] array3;
						CharRange[] array4;
						stringBuilder.Append(TranslatorEngine.ChineseToVietPhrase(text, wrapType, translationAlgorithm, prioritizedName, out array3, out array4));
					}
					else
					{
						stringBuilder.Append(text);
					}
				}
			}
			return stringBuilder.ToString();
		}

		public static string ChineseToVietPhraseForBatch(string chinese, int wrapType, int translationAlgorithm, bool prioritizedName)
		{
			string text = "";
			StringBuilder stringBuilder = new StringBuilder();
			int num = chinese.Length - 1;
			int i = 0;
			while (i <= num)
			{
				bool flag = false;
				for (int j = 20; j > 0; j--)
				{
					if (chinese.Length >= i + j && TranslatorEngine.vietPhraseDictionary.ContainsKey(chinese.Substring(i, j)) && (!prioritizedName || !TranslatorEngine.containsName(chinese, i, j)) && ((translationAlgorithm != 0 && translationAlgorithm != 2) || TranslatorEngine.isLongestPhraseInSentence(chinese, i, j, TranslatorEngine.vietPhraseDictionary, translationAlgorithm)))
					{
						if (!string.IsNullOrEmpty(TranslatorEngine.vietPhraseDictionary[chinese.Substring(i, j)]))
						{
							if (wrapType == 0)
							{
								TranslatorEngine.appendTranslatedWord(stringBuilder, TranslatorEngine.vietPhraseDictionary[chinese.Substring(i, j)], ref text);
							}
							else if (wrapType == 1 || wrapType == 11)
							{
								TranslatorEngine.appendTranslatedWord(stringBuilder, "[" + TranslatorEngine.vietPhraseDictionary[chinese.Substring(i, j)] + "]", ref text);
							}
							else if (TranslatorEngine.hasOnlyOneMeaning(TranslatorEngine.vietPhraseDictionary[chinese.Substring(i, j)]))
							{
								TranslatorEngine.appendTranslatedWord(stringBuilder, TranslatorEngine.vietPhraseDictionary[chinese.Substring(i, j)], ref text);
							}
							else
							{
								TranslatorEngine.appendTranslatedWord(stringBuilder, "[" + TranslatorEngine.vietPhraseDictionary[chinese.Substring(i, j)] + "]", ref text);
							}
							if (TranslatorEngine.nextCharIsChinese(chinese, i + j - 1))
							{
								stringBuilder.Append(" ");
								text += " ";
							}
						}
						flag = true;
						i += j;
						break;
					}
				}
				if (!flag)
				{
					if (TranslatorEngine.isChinese(chinese[i]))
					{
						TranslatorEngine.appendTranslatedWord(stringBuilder, ((wrapType != 1) ? "" : "[") + TranslatorEngine.ChineseToHanViet(chinese[i]) + ((wrapType != 1) ? "" : "]"), ref text);
						if (TranslatorEngine.nextCharIsChinese(chinese, i))
						{
							stringBuilder.Append(" ");
							text += " ";
						}
					}
					else if ((chinese[i] == '"' || chinese[i] == '\'') && !text.EndsWith(" ") && !text.EndsWith(".") && !text.EndsWith("?") && !text.EndsWith("!") && !text.EndsWith("\t") && i < chinese.Length - 1 && chinese[i + 1] != ' ' && chinese[i + 1] != ',')
					{
						stringBuilder.Append(" ").Append(chinese[i]);
						text = text + " " + chinese[i].ToString();
					}
					else
					{
						stringBuilder.Append(chinese[i]);
						text += chinese[i].ToString();
					}
					i++;
				}
			}
			return stringBuilder.ToString().Replace("  ", " ");
		}

		public static string ChineseToVietPhraseOneMeaning(string chinese, int wrapType, int translationAlgorithm, bool prioritizedName, out CharRange[] chinesePhraseRanges, out CharRange[] vietPhraseRanges)
		{
			TranslatorEngine.LastTranslatedWord_VietPhraseOneMeaning = "";
			List<CharRange> list = new List<CharRange>();
			List<CharRange> list2 = new List<CharRange>();
			StringBuilder stringBuilder = new StringBuilder();
			int num = chinese.Length - 1;
			int i = 0;
			while (i <= num)
			{
				bool flag = false;
				for (int j = 20; j > 0; j--)
				{
					if (chinese.Length >= i + j && TranslatorEngine.vietPhraseOneMeaningDictionary.ContainsKey(chinese.Substring(i, j)) && (!prioritizedName || !TranslatorEngine.containsName(chinese, i, j)) && ((translationAlgorithm != 0 && translationAlgorithm != 2) || TranslatorEngine.isLongestPhraseInSentence(chinese, i, j, TranslatorEngine.vietPhraseOneMeaningDictionary, translationAlgorithm)))
					{
						list.Add(new CharRange(i, j));
						if (wrapType == 0)
						{
							TranslatorEngine.appendTranslatedWord(stringBuilder, TranslatorEngine.vietPhraseOneMeaningDictionary[chinese.Substring(i, j)], ref TranslatorEngine.LastTranslatedWord_VietPhraseOneMeaning);
							list2.Add(new CharRange(stringBuilder.ToString().Length - TranslatorEngine.vietPhraseOneMeaningDictionary[chinese.Substring(i, j)].Length, TranslatorEngine.vietPhraseOneMeaningDictionary[chinese.Substring(i, j)].Length));
						}
						else
						{
							TranslatorEngine.appendTranslatedWord(stringBuilder, "[" + TranslatorEngine.vietPhraseOneMeaningDictionary[chinese.Substring(i, j)] + "]", ref TranslatorEngine.LastTranslatedWord_VietPhraseOneMeaning);
							list2.Add(new CharRange(stringBuilder.ToString().Length - TranslatorEngine.vietPhraseOneMeaningDictionary[chinese.Substring(i, j)].Length - 2, TranslatorEngine.vietPhraseOneMeaningDictionary[chinese.Substring(i, j)].Length + 2));
						}
						if (TranslatorEngine.nextCharIsChinese(chinese, i + j - 1))
						{
							stringBuilder.Append(" ");
							TranslatorEngine.LastTranslatedWord_VietPhraseOneMeaning += " ";
						}
						flag = true;
						i += j;
						break;
					}
				}
				if (!flag)
				{
					int length = stringBuilder.ToString().Length;
					int num2 = TranslatorEngine.ChineseToHanViet(chinese[i]).Length;
					list.Add(new CharRange(i, 1));
					if (TranslatorEngine.isChinese(chinese[i]))
					{
						TranslatorEngine.appendTranslatedWord(stringBuilder, ((wrapType != 1) ? "" : "[") + TranslatorEngine.ChineseToHanViet(chinese[i]) + ((wrapType != 1) ? "" : "]"), ref TranslatorEngine.LastTranslatedWord_VietPhraseOneMeaning);
						if (TranslatorEngine.nextCharIsChinese(chinese, i))
						{
							stringBuilder.Append(" ");
							TranslatorEngine.LastTranslatedWord_VietPhraseOneMeaning += " ";
						}
						num2 += ((wrapType != 1) ? 0 : 2);
					}
					else if ((chinese[i] == '"' || chinese[i] == '\'') && !TranslatorEngine.LastTranslatedWord_VietPhraseOneMeaning.EndsWith(" ") && !TranslatorEngine.LastTranslatedWord_VietPhraseOneMeaning.EndsWith(".") && !TranslatorEngine.LastTranslatedWord_VietPhraseOneMeaning.EndsWith("?") && !TranslatorEngine.LastTranslatedWord_VietPhraseOneMeaning.EndsWith("!") && !TranslatorEngine.LastTranslatedWord_VietPhraseOneMeaning.EndsWith("\t") && i < chinese.Length - 1 && chinese[i + 1] != ' ' && chinese[i + 1] != ',')
					{
						stringBuilder.Append(" ").Append(chinese[i]);
						TranslatorEngine.LastTranslatedWord_VietPhraseOneMeaning = TranslatorEngine.LastTranslatedWord_VietPhraseOneMeaning + " " + chinese[i].ToString();
					}
					else
					{
						stringBuilder.Append(chinese[i]);
						TranslatorEngine.LastTranslatedWord_VietPhraseOneMeaning += chinese[i].ToString();
						num2 = 1;
					}
					list2.Add(new CharRange(length, num2));
					i++;
				}
			}
			chinesePhraseRanges = list.ToArray();
			vietPhraseRanges = list2.ToArray();
			TranslatorEngine.LastTranslatedWord_VietPhraseOneMeaning = "";
			return stringBuilder.ToString();
		}

		public static string ChineseToVietPhraseOneMeaningForBrowser(string chinese, int wrapType, int translationAlgorithm, bool prioritizedName)
		{
			chinese = TranslatorEngine.StandardizeInputForBrowser(chinese);
			StringBuilder stringBuilder = new StringBuilder();
			string[] array = TranslatorEngine.classifyWordsIntoLatinAndChinese(chinese);
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				if (!string.IsNullOrEmpty(text))
				{
					if (TranslatorEngine.isChinese(text[0]))
					{
						CharRange[] array3;
						CharRange[] array4;
						stringBuilder.Append(TranslatorEngine.ChineseToVietPhraseOneMeaning(text, wrapType, translationAlgorithm, prioritizedName, out array3, out array4));
					}
					else
					{
						stringBuilder.Append(text);
					}
				}
			}
			return stringBuilder.ToString();
		}

		public static string ChineseToVietPhraseOneMeaningForBatch(string chinese, int wrapType, int translationAlgorithm, bool prioritizedName)
		{
			string text = "";
			StringBuilder stringBuilder = new StringBuilder();
			int num = chinese.Length - 1;
			int i = 0;
			while (i <= num)
			{
				bool flag = false;
				for (int j = 20; j > 0; j--)
				{
					if (chinese.Length >= i + j && TranslatorEngine.vietPhraseOneMeaningDictionary.ContainsKey(chinese.Substring(i, j)) && (!prioritizedName || !TranslatorEngine.containsName(chinese, i, j)) && ((translationAlgorithm != 0 && translationAlgorithm != 2) || TranslatorEngine.isLongestPhraseInSentence(chinese, i, j, TranslatorEngine.vietPhraseOneMeaningDictionary, translationAlgorithm)))
					{
						if (!string.IsNullOrEmpty(TranslatorEngine.vietPhraseOneMeaningDictionary[chinese.Substring(i, j)]))
						{
							if (wrapType == 0)
							{
								TranslatorEngine.appendTranslatedWord(stringBuilder, TranslatorEngine.vietPhraseOneMeaningDictionary[chinese.Substring(i, j)], ref text);
							}
							else
							{
								TranslatorEngine.appendTranslatedWord(stringBuilder, "[" + TranslatorEngine.vietPhraseOneMeaningDictionary[chinese.Substring(i, j)] + "]", ref text);
							}
							if (TranslatorEngine.nextCharIsChinese(chinese, i + j - 1))
							{
								stringBuilder.Append(" ");
								text += " ";
							}
						}
						flag = true;
						i += j;
						break;
					}
				}
				if (!flag)
				{
					if (TranslatorEngine.isChinese(chinese[i]))
					{
						TranslatorEngine.appendTranslatedWord(stringBuilder, ((wrapType != 1) ? "" : "[") + TranslatorEngine.ChineseToHanViet(chinese[i]) + ((wrapType != 1) ? "" : "]"), ref text);
						if (TranslatorEngine.nextCharIsChinese(chinese, i))
						{
							stringBuilder.Append(" ");
							text += " ";
						}
					}
					else if ((chinese[i] == '"' || chinese[i] == '\'') && !text.EndsWith(" ") && !text.EndsWith(".") && !text.EndsWith("?") && !text.EndsWith("!") && !text.EndsWith("\t") && i < chinese.Length - 1 && chinese[i + 1] != ' ' && chinese[i + 1] != ',')
					{
						stringBuilder.Append(" ").Append(chinese[i]);
						text = text + " " + chinese[i].ToString();
					}
					else
					{
						stringBuilder.Append(chinese[i]);
						text += chinese[i].ToString();
					}
					i++;
				}
			}
			return stringBuilder.ToString();
		}

		public static string ChineseToNameForBatch(string chinese)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = chinese.Length - 1;
			int i = 0;
			while (i <= num)
			{
				bool flag = false;
				if (TranslatorEngine.isChinese(chinese[i]))
				{
					for (int j = 20; j > 0; j--)
					{
						if (chinese.Length >= i + j && TranslatorEngine.onlyNameDictionary.ContainsKey(chinese.Substring(i, j)))
						{
							stringBuilder.Append(TranslatorEngine.onlyNameDictionary[chinese.Substring(i, j)]);
							flag = true;
							i += j;
							break;
						}
					}
				}
				if (!flag)
				{
					stringBuilder.Append(chinese[i]);
					i++;
				}
			}
			return stringBuilder.ToString();
		}

		public static string ChineseToMeanings(string chinese, out int phraseTranslatedLength)
		{
			string text = "";
			if (chinese.Length == 0)
			{
				phraseTranslatedLength = 0;
				return "";
			}
			int num = 0;
			for (int i = 20; i > 0; i--)
			{
				if (chinese.Length >= i)
				{
					string text2 = chinese.Substring(0, i);
					if (TranslatorEngine.vietPhraseDictionary.ContainsKey(text2))
					{
						string text3 = text;
						text = string.Concat(new string[]
						{
							text3,
							text2,
							" <<VietPhrase>> ",
							TranslatorEngine.vietPhraseDictionary[text2].Replace("/", "; "),
							"\n-----------------\n"
						});
						if (num == 0)
						{
							num = text2.Length;
						}
					}
				}
			}
			for (int j = 20; j > 0; j--)
			{
				if (chinese.Length >= j)
				{
					string text2 = chinese.Substring(0, j);
					if (TranslatorEngine.lacVietDictionary.ContainsKey(text2))
					{
						string text4 = text;
						text = string.Concat(new string[]
						{
							text4,
							text2,
							" <<Lạc Việt>>\n",
							TranslatorEngine.lacVietDictionary[text2],
							"\n-----------------\n"
						});
						if (num == 0)
						{
							num = 1;
						}
					}
				}
			}
			for (int k = 20; k > 0; k--)
			{
				if (chinese.Length >= k)
				{
					string text2 = chinese.Substring(0, k);
					if (TranslatorEngine.cedictDictionary.ContainsKey(text2))
					{
						string text5 = text;
						text = string.Concat(new string[]
						{
							text5,
							text2,
							" <<Cedict or Babylon>> ",
							TranslatorEngine.cedictDictionary[text2].Replace("] /", "] ").Replace("/", "; "),
							"\n-----------------\n"
						});
						if (num == 0)
						{
							num = 1;
						}
					}
				}
			}
			if (TranslatorEngine.thieuChuuDictionary.ContainsKey(chinese[0].ToString()))
			{
				num = ((num == 0) ? 1 : num);
				object obj = text;
				text = string.Concat(new object[]
				{
					obj,
					chinese[0],
					" <<Thiều Chửu>> ",
					TranslatorEngine.thieuChuuDictionary[chinese[0].ToString()],
					"\n-----------------\n"
				});
			}
			int num2 = (chinese.Length < 10) ? chinese.Length : 10;
			text = text + chinese.Substring(0, num2).Trim("\n\t ".ToCharArray()) + " <<Phiên Âm English>> ";
			for (int l = 0; l < num2; l++)
			{
				if (TranslatorEngine.chinesePhienAmEnglishDictionary.ContainsKey(chinese[l].ToString()))
				{
					text = text + "[" + TranslatorEngine.chinesePhienAmEnglishDictionary[chinese[l].ToString()] + "] ";
				}
				else
				{
					text = text + TranslatorEngine.ChineseToHanViet(chinese[l]) + " ";
				}
			}
			if (num == 0)
			{
				num = 1;
				text = chinese[0] + "\n-----------------\nNot Found";
			}
			phraseTranslatedLength = num;
			return text;
		}

		public static void LoadDictionaries()
		{
			lock (TranslatorEngine.lockObject)
			{
				if (TranslatorEngine.dictionaryDirty)
				{
					ThreadPool.QueueUserWorkItem(new WaitCallback(TranslatorEngine.loadHanVietDictionaryWithNewThread));
					ThreadPool.QueueUserWorkItem(new WaitCallback(TranslatorEngine.loadThieuChuuDictionaryWithNewThread));
					ThreadPool.QueueUserWorkItem(new WaitCallback(TranslatorEngine.loadLacVietDictionaryWithNewThread));
					ThreadPool.QueueUserWorkItem(new WaitCallback(TranslatorEngine.loadCedictDictionaryWithNewThread));
					ThreadPool.QueueUserWorkItem(new WaitCallback(TranslatorEngine.loadChinesePhienAmEnglishDictionaryWithNewThread));
					ThreadPool.QueueUserWorkItem(new WaitCallback(TranslatorEngine.loadIgnoredChinesePhraseListsWithNewThread));
					ThreadPool.QueueUserWorkItem(new WaitCallback(TranslatorEngine.loadOnlyNameDictionaryHistoryWithNewThread));
					ThreadPool.QueueUserWorkItem(new WaitCallback(TranslatorEngine.loadOnlyVietPhraseDictionaryHistoryWithNewThread));
					ThreadPool.QueueUserWorkItem(new WaitCallback(TranslatorEngine.loadHanVietDictionaryHistoryWithNewThread));
					ManualResetEvent[] array = new ManualResetEvent[]
					{
						new ManualResetEvent(false)
					};
					ThreadPool.QueueUserWorkItem(new WaitCallback(TranslatorEngine.loadOnlyVietPhraseDictionaryWithNewThread), array[0]);
					ManualResetEvent[] array2 = new ManualResetEvent[]
					{
						new ManualResetEvent(false)
					};
					ThreadPool.QueueUserWorkItem(new WaitCallback(TranslatorEngine.loadOnlyNameDictionaryWithNewThread), array2[0]);
					WaitHandle.WaitAll(array);
					WaitHandle.WaitAll(array2);
					TranslatorEngine.loadVietPhraseDictionary();
					TranslatorEngine.vietPhraseDictionaryToVietPhraseOneMeaningDictionary();
					TranslatorEngine.dictionaryDirty = false;
				}
			}
		}

		private static void loadHanVietDictionaryWithNewThread(object stateInfo)
		{
			TranslatorEngine.loadHanVietDictionary();
		}

		private static void loadHanVietDictionary()
		{
			TranslatorEngine.hanVietDictionary.Clear();
			using (TextReader textReader = new StreamReader(DictionaryConfigurationHelper.GetChinesePhienAmWordsDictionaryPath(), true))
			{
				string text;
				while ((text = textReader.ReadLine()) != null)
				{
					string[] array = text.Split(new char[]
					{
						'='
					});
					if (array.Length == 2 && !TranslatorEngine.hanVietDictionary.ContainsKey(array[0]))
					{
						TranslatorEngine.hanVietDictionary.Add(array[0], array[1]);
					}
				}
			}
		}

		private static void loadVietPhraseDictionary()
		{
			TranslatorEngine.vietPhraseDictionary.Clear();
			foreach (KeyValuePair<string, string> current in TranslatorEngine.onlyNameDictionary)
			{
				if (!TranslatorEngine.vietPhraseDictionary.ContainsKey(current.Key))
				{
					TranslatorEngine.vietPhraseDictionary.Add(current.Key, current.Value);
				}
			}
			foreach (KeyValuePair<string, string> current2 in TranslatorEngine.onlyVietPhraseDictionary)
			{
				if (!TranslatorEngine.vietPhraseDictionary.ContainsKey(current2.Key))
				{
					TranslatorEngine.vietPhraseDictionary.Add(current2.Key, current2.Value);
				}
			}
		}

		private static void loadOnlyVietPhraseDictionaryWithNewThread(object stateInfo)
		{
			TranslatorEngine.loadOnlyVietPhraseDictionary();
			((ManualResetEvent)stateInfo).Set();
		}

		private static void loadOnlyVietPhraseDictionary()
		{
			TranslatorEngine.onlyVietPhraseDictionary.Clear();
			using (TextReader textReader = new StreamReader(DictionaryConfigurationHelper.GetVietPhraseDictionaryPath(), true))
			{
				string text;
				while ((text = textReader.ReadLine()) != null)
				{
					string[] array = text.Split(new char[]
					{
						'='
					});
					if (array.Length == 2 && !TranslatorEngine.onlyVietPhraseDictionary.ContainsKey(array[0]))
					{
						TranslatorEngine.onlyVietPhraseDictionary.Add(array[0], array[1]);
					}
				}
			}
		}

		private static void loadOnlyNameDictionaryWithNewThread(object stateInfo)
		{
			TranslatorEngine.loadOnlyNameDictionary();
			((ManualResetEvent)stateInfo).Set();
		}

		private static void loadOnlyNameDictionary()
		{
			TranslatorEngine.onlyNameDictionary.Clear();
			using (TextReader textReader = new StreamReader(DictionaryConfigurationHelper.GetNamesDictionaryPath(), true))
			{
				string text;
				while ((text = textReader.ReadLine()) != null)
				{
					string[] array = text.Split(new char[]
					{
						'='
					});
					if (array.Length == 2 && !TranslatorEngine.onlyNameDictionary.ContainsKey(array[0]))
					{
						TranslatorEngine.onlyNameDictionary.Add(array[0], array[1]);
					}
				}
			}
		}

		private static void vietPhraseDictionaryToVietPhraseOneMeaningDictionary()
		{
			TranslatorEngine.vietPhraseOneMeaningDictionary.Clear();
			foreach (KeyValuePair<string, string> current in TranslatorEngine.vietPhraseDictionary)
			{
				TranslatorEngine.vietPhraseOneMeaningDictionary.Add(current.Key, (current.Value.Contains("/") || current.Value.Contains("|")) ? current.Value.Split(new char[]
				{
					'/',
					'|'
				})[0] : current.Value);
			}
		}

		private static void loadThieuChuuDictionaryWithNewThread(object stateInfo)
		{
			TranslatorEngine.loadThieuChuuDictionary();
		}

		private static void loadThieuChuuDictionary()
		{
			TranslatorEngine.thieuChuuDictionary.Clear();
			using (TextReader textReader = new StreamReader(DictionaryConfigurationHelper.GetThieuChuuDictionaryPath(), true))
			{
				string text;
				while ((text = textReader.ReadLine()) != null)
				{
					string[] array = text.Split(new char[]
					{
						'='
					});
					if (array.Length == 2 && !TranslatorEngine.thieuChuuDictionary.ContainsKey(array[0]))
					{
						TranslatorEngine.thieuChuuDictionary.Add(array[0], array[1]);
					}
				}
			}
		}

		private static void loadLacVietDictionaryWithNewThread(object stateInfo)
		{
			TranslatorEngine.loadLacVietDictionary();
		}

		private static void loadLacVietDictionary()
		{
			TranslatorEngine.lacVietDictionary.Clear();
			using (TextReader textReader = new StreamReader(DictionaryConfigurationHelper.GetLacVietDictionaryPath(), true))
			{
				string text;
				while ((text = textReader.ReadLine()) != null)
				{
					string[] array = text.Split(new char[]
					{
						'='
					});
					if (array.Length == 2 && !TranslatorEngine.lacVietDictionary.ContainsKey(array[0]))
					{
						TranslatorEngine.lacVietDictionary.Add(array[0], array[1]);
					}
				}
			}
		}

		private static void loadCedictDictionaryWithNewThread(object stateInfo)
		{
			TranslatorEngine.loadCedictDictionary();
		}

		private static void loadChinesePhienAmEnglishDictionaryWithNewThread(object stateInfo)
		{
			TranslatorEngine.loadChinesePhienAmEnglishDictionary();
		}

		private static void loadIgnoredChinesePhraseListsWithNewThread(object stateInfo)
		{
			TranslatorEngine.loadIgnoredChinesePhraseLists();
		}

		private static void loadOnlyVietPhraseDictionaryHistoryWithNewThread(object stateInfo)
		{
			TranslatorEngine.loadOnlyVietPhraseDictionaryHistory();
		}

		private static void loadOnlyNameDictionaryHistoryWithNewThread(object stateInfo)
		{
			TranslatorEngine.loadOnlyNameDictionaryHistory();
		}

		private static void loadHanVietDictionaryHistoryWithNewThread(object stateInfo)
		{
			TranslatorEngine.loadHanVietDictionaryHistory();
		}

		private static void loadOnlyVietPhraseDictionaryHistory()
		{
			TranslatorEngine.LoadDictionaryHistory(DictionaryConfigurationHelper.GetVietPhraseDictionaryHistoryPath(), ref TranslatorEngine.onlyVietPhraseDictionaryHistoryDataSet);
		}

		private static void loadOnlyNameDictionaryHistory()
		{
			TranslatorEngine.LoadDictionaryHistory(DictionaryConfigurationHelper.GetNamesDictionaryHistoryPath(), ref TranslatorEngine.onlyNameDictionaryHistoryDataSet);
		}

		private static void loadHanVietDictionaryHistory()
		{
			TranslatorEngine.LoadDictionaryHistory(DictionaryConfigurationHelper.GetChinesePhienAmWordsDictionaryHistoryPath(), ref TranslatorEngine.hanVietDictionaryHistoryDataSet);
		}

		public static void LoadDictionaryHistory(string dictionaryHistoryPath, ref DataSet dictionaryHistoryDataSet)
		{
			dictionaryHistoryDataSet.Clear();
			string name = "DictionaryHistory";
			if (!dictionaryHistoryDataSet.Tables.Contains(name))
			{
				dictionaryHistoryDataSet.Tables.Add(name);
				dictionaryHistoryDataSet.Tables[name].Columns.Add("Entry", Type.GetType("System.String"));
				dictionaryHistoryDataSet.Tables[name].Columns.Add("Action", Type.GetType("System.String"));
				dictionaryHistoryDataSet.Tables[name].Columns.Add("User Name", Type.GetType("System.String"));
				dictionaryHistoryDataSet.Tables[name].Columns.Add("Updated Date", Type.GetType("System.DateTime"));
				dictionaryHistoryDataSet.Tables[name].PrimaryKey = new DataColumn[]
				{
					dictionaryHistoryDataSet.Tables[name].Columns["Entry"]
				};
			}
			if (!File.Exists(dictionaryHistoryPath))
			{
				return;
			}
			string name2 = CharsetDetector.DetectChineseCharset(dictionaryHistoryPath);
			using (TextReader textReader = new StreamReader(dictionaryHistoryPath, Encoding.GetEncoding(name2)))
			{
				textReader.ReadLine();
				string text;
				while ((text = textReader.ReadLine()) != null)
				{
					string[] array = text.Split(new char[]
					{
						'\t'
					});
					if (array.Length == 4)
					{
						DataRow dataRow = dictionaryHistoryDataSet.Tables[name].Rows.Find(array[0]);
						if (dataRow == null)
						{
							dictionaryHistoryDataSet.Tables[name].Rows.Add(new object[]
							{
								array[0],
								array[1],
								array[2],
								DateTime.ParseExact(array[3], "yyyy-MM-dd HH:mm:ss.fffzzz", null)
							});
						}
						else
						{
							dataRow[1] = array[1];
							dataRow[2] = array[2];
							dataRow[3] = DateTime.ParseExact(array[3], "yyyy-MM-dd HH:mm:ss.fffzzz", null);
						}
					}
				}
			}
		}

		private static void loadCedictDictionary()
		{
			TranslatorEngine.cedictDictionary.Clear();
			using (TextReader textReader = new StreamReader(DictionaryConfigurationHelper.GetCEDictDictionaryPath(), Encoding.UTF8))
			{
				string text;
				while ((text = textReader.ReadLine()) != null)
				{
					if (!text.StartsWith("#"))
					{
						string text2 = text.Substring(0, text.IndexOf(" ["));
						string[] array = text2.Split(new char[]
						{
							' '
						});
						for (int i = 0; i < array.Length; i++)
						{
							string key = array[i];
							if (!TranslatorEngine.cedictDictionary.ContainsKey(key))
							{
								TranslatorEngine.cedictDictionary.Add(key, text.Substring(text.IndexOf(" [")));
							}
						}
					}
				}
			}
			using (TextReader textReader2 = new StreamReader(DictionaryConfigurationHelper.GetBabylonDictionaryPath(), Encoding.UTF8))
			{
				string text3;
				while ((text3 = textReader2.ReadLine()) != null)
				{
					string[] array2 = text3.Split(new char[]
					{
						'='
					});
					if (!TranslatorEngine.cedictDictionary.ContainsKey(array2[0]))
					{
						TranslatorEngine.cedictDictionary.Add(array2[0], array2[1]);
					}
				}
			}
		}

		private static void loadChinesePhienAmEnglishDictionary()
		{
			TranslatorEngine.chinesePhienAmEnglishDictionary.Clear();
			using (TextReader textReader = new StreamReader(DictionaryConfigurationHelper.GetChinesePhienAmEnglishWordsDictionaryPath(), true))
			{
				string text;
				while ((text = textReader.ReadLine()) != null)
				{
					string[] array = text.Split(new char[]
					{
						'='
					});
					if (array.Length == 2 && !TranslatorEngine.chinesePhienAmEnglishDictionary.ContainsKey(array[0]))
					{
						TranslatorEngine.chinesePhienAmEnglishDictionary.Add(array[0], array[1]);
					}
				}
			}
		}

		private static void loadIgnoredChinesePhraseLists()
		{
			TranslatorEngine.ignoredChinesePhraseList.Clear();
			TranslatorEngine.ignoredChinesePhraseForBrowserList.Clear();
			char[] trimChars = "\t\n".ToCharArray();
			using (TextReader textReader = new StreamReader(DictionaryConfigurationHelper.GetIgnoredChinesePhraseListPath(), true))
			{
				string text;
				while ((text = textReader.ReadLine()) != null)
				{
					if (!string.IsNullOrEmpty(text))
					{
						string item = TranslatorEngine.standardizeInputWithoutRemovingIgnoredChinesePhrases(text).Trim(trimChars);
						if (!TranslatorEngine.ignoredChinesePhraseList.Contains(item))
						{
							TranslatorEngine.ignoredChinesePhraseList.Add(item);
						}
						string item2 = TranslatorEngine.standardizeInputForBrowserWithoutRemovingIgnoredChinesePhrases(text).Trim(trimChars);
						if (!TranslatorEngine.ignoredChinesePhraseForBrowserList.Contains(item2))
						{
							TranslatorEngine.ignoredChinesePhraseForBrowserList.Add(item2);
						}
					}
				}
			}
			TranslatorEngine.ignoredChinesePhraseList.Sort(new Comparison<string>(TranslatorEngine.compareStringByDescending));
			TranslatorEngine.ignoredChinesePhraseForBrowserList.Sort(new Comparison<string>(TranslatorEngine.compareStringByDescending));
		}

		private static int compareStringByDescending(string x, string y)
		{
			if (x == null)
			{
				if (y == null)
				{
					return 0;
				}
				return 1;
			}
			else
			{
				if (y == null)
				{
					return -1;
				}
				int num = x.Length.CompareTo(y.Length);
				if (num != 0)
				{
					return num * -1;
				}
				return x.CompareTo(y) * -1;
			}
		}

		public static string StandardizeInput(string original)
		{
			string standardizedChinese = TranslatorEngine.standardizeInputWithoutRemovingIgnoredChinesePhrases(original);
			return TranslatorEngine.removeIgnoredChinesePhrases(standardizedChinese);
		}

		private static string standardizeInputWithoutRemovingIgnoredChinesePhrases(string original)
		{
			if (string.IsNullOrEmpty(original) || string.IsNullOrEmpty(original))
			{
				return "";
			}
			string[] array = new string[]
			{
				"，",
				"。",
				"：",
				"“",
				"”",
				"‘",
				"’",
				"？",
				"！",
				"「",
				"」",
				"．",
				"、",
				"\u3000",
				"…"
			};
			string[] array2 = new string[]
			{
				", ",
				".",
				": ",
				"\"",
				"\" ",
				"'",
				"' ",
				"?",
				"!",
				"\"",
				"\" ",
				".",
				", ",
				" ",
				"..."
			};
			string text = original;
			for (int i = 0; i < array.Length; i++)
			{
				text = text.Replace(array[i], array2[i]);
			}
			text = text.Replace("  ", " ").Replace(" \r\n", "\n").Replace(" \n", "\n").Replace(" ,", ",");
			text = TranslatorEngine.ToNarrow(text);
			int length = text.Length;
			StringBuilder stringBuilder = new StringBuilder();
			for (int j = 0; j < length - 1; j++)
			{
				char c = text[j];
				char c2 = text[j + 1];
				if (TranslatorEngine.isChinese(c))
				{
					if (!TranslatorEngine.isChinese(c2) && c2 != ',' && c2 != '.' && c2 != ':' && c2 != ';' && c2 != '"' && c2 != '\'' && c2 != '?' && c2 != ' ' && c2 != '!')
					{
						stringBuilder.Append(c).Append(" ");
					}
					else
					{
						stringBuilder.Append(c);
					}
				}
				else if (c == '\t' || c == ' ' || c == '"' || c == '\'' || c == '\n')
				{
					stringBuilder.Append(c);
				}
				else if (c == '!' || c == '.' || c == '?')
				{
					if (c2 == '"' || c2 == ' ' || c2 == '\'')
					{
						stringBuilder.Append(c);
					}
					else
					{
						stringBuilder.Append(c).Append(" ");
					}
				}
				else if (TranslatorEngine.isChinese(c2))
				{
					stringBuilder.Append(c).Append(" ");
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			stringBuilder.Append(text[length - 1]);
			text = TranslatorEngine.indentAllLines(stringBuilder.ToString(), true);
			return text.Replace(". . . . . .", "...");
		}

		public static string StandardizeInputForBrowser(string original)
		{
			string standardizedChinese = TranslatorEngine.standardizeInputForBrowserWithoutRemovingIgnoredChinesePhrases(original);
			return TranslatorEngine.removeIgnoredChinesePhrasesForBrowser(standardizedChinese);
		}

		private static string standardizeInputForBrowserWithoutRemovingIgnoredChinesePhrases(string original)
		{
			if (string.IsNullOrEmpty(original) || string.IsNullOrEmpty(original))
			{
				return "";
			}
			string[] array = new string[]
			{
				"，",
				"。",
				"：",
				"“",
				"”",
				"‘",
				"’",
				"？",
				"！",
				"「",
				"」",
				"．",
				"、",
				"\u3000",
				"…"
			};
			string[] array2 = new string[]
			{
				", ",
				".",
				": ",
				"\"",
				"\" ",
				"'",
				"' ",
				"?",
				"!",
				"\"",
				"\" ",
				".",
				", ",
				" ",
				"..."
			};
			string text = original;
			for (int i = 0; i < array.Length; i++)
			{
				text = text.Replace(array[i], array2[i]);
			}
			text = text.Replace("  ", " ").Replace(" \r\n", "\n").Replace(" \n", "\n");
			text = TranslatorEngine.ToNarrow(text);
			int length = text.Length;
			StringBuilder stringBuilder = new StringBuilder();
			for (int j = 0; j < length - 1; j++)
			{
				char c = text[j];
				char c2 = text[j + 1];
				if (TranslatorEngine.isChinese(c))
				{
					if (!TranslatorEngine.isChinese(c2) && c2 != ',' && c2 != '.' && c2 != ':' && c2 != ';' && c2 != '"' && c2 != '\'' && c2 != '?' && c2 != ' ' && c2 != '!')
					{
						stringBuilder.Append(c).Append(" ");
					}
					else
					{
						stringBuilder.Append(c);
					}
				}
				else if (c == '\t' || c == ' ' || c == '"' || c == '\'' || c == '\n')
				{
					stringBuilder.Append(c);
				}
				else if (TranslatorEngine.isChinese(c2))
				{
					stringBuilder.Append(c).Append(" ");
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			stringBuilder.Append(text[length - 1]);
			return TranslatorEngine.indentAllLines(stringBuilder.ToString());
		}

		private static string indentAllLines(string text, bool insertBlankLine)
		{
			string[] array = text.Split(new char[]
			{
				'\n'
			}, StringSplitOptions.RemoveEmptyEntries);
			StringBuilder stringBuilder = new StringBuilder();
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text2 = array2[i];
				if (!string.IsNullOrEmpty(text2.Trim()))
				{
					stringBuilder.Append("\t" + text2.Trim()).Append("\n").Append(insertBlankLine ? "\n" : "");
				}
			}
			return stringBuilder.ToString();
		}

		private static string indentAllLines(string text)
		{
			return TranslatorEngine.indentAllLines(text, false);
		}

		private static bool isChinese(char character)
		{
			return TranslatorEngine.hanVietDictionary.ContainsKey(character.ToString());
		}

		private static bool hasOnlyOneMeaning(string meaning)
		{
			return meaning.Split(new char[]
			{
				'/',
				'|'
			}).Length == 1;
		}

		internal static string ToWide(string str)
		{
			int length = str.Length;
			int i;
			for (i = 0; i < length; i++)
			{
				char c = str[i];
				if (c >= '!' && c <= '~')
				{
					break;
				}
			}
			if (i >= length)
			{
				return str;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (i = 0; i < length; i++)
			{
				char c = str[i];
				if (c >= '!' && c <= '~')
				{
					stringBuilder.Append(c - '!' + '！');
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		internal static string ToNarrow(string str)
		{
			int length = str.Length;
			int i;
			for (i = 0; i < length; i++)
			{
				char c = str[i];
				if (c >= '！' && c <= '～')
				{
					break;
				}
			}
			if (i >= length)
			{
				return str;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (i = 0; i < length; i++)
			{
				char c = str[i];
				if (c >= '！' && c <= '～')
				{
					stringBuilder.Append(c - '！' + '!');
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		private static void appendTranslatedWord(StringBuilder result, string translatedText, ref string lastTranslatedWord)
		{
			int num = 0;
			TranslatorEngine.appendTranslatedWord(result, translatedText, ref lastTranslatedWord, ref num);
		}

		private static void appendTranslatedWord(StringBuilder result, string translatedText, ref string lastTranslatedWord, ref int startIndexOfNextTranslatedText)
		{
			if (lastTranslatedWord.EndsWith("\n") || lastTranslatedWord.EndsWith("\t") || lastTranslatedWord.EndsWith(". ") || lastTranslatedWord.EndsWith("\"") || lastTranslatedWord.EndsWith("'") || lastTranslatedWord.EndsWith("? ") || lastTranslatedWord.EndsWith("! ") || lastTranslatedWord.EndsWith(".\" ") || lastTranslatedWord.EndsWith("?\" ") || lastTranslatedWord.EndsWith("!\" "))
			{
				lastTranslatedWord = TranslatorEngine.toUpperCase(translatedText);
			}
			else if (lastTranslatedWord.EndsWith(" "))
			{
				lastTranslatedWord = translatedText;
			}
			else
			{
				lastTranslatedWord = " " + translatedText;
			}
			if ((string.IsNullOrEmpty(translatedText) || translatedText[0] == ',' || translatedText[0] == '.' || translatedText[0] == '?' || translatedText[0] == '!') && 0 < result.Length && result[result.Length - 1] == ' ')
			{
				result = result.Remove(result.Length - 1, 1);
				startIndexOfNextTranslatedText--;
			}
			result.Append(lastTranslatedWord);
		}

		private static string toUpperCase(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}
			if (text.StartsWith("["))
			{
				return "[" + char.ToUpper(text[1]) + ((text.Length <= 2) ? "" : text.Substring(2));
			}
			return char.ToUpper(text[0]) + ((text.Length <= 1) ? "" : text.Substring(1));
		}

		private static bool nextCharIsChinese(string chinese, int currentPhraseEndIndex)
		{
			return chinese.Length - 1 > currentPhraseEndIndex && TranslatorEngine.isChinese(chinese[currentPhraseEndIndex + 1]);
		}

		private static string[] classifyWordsIntoLatinAndChinese(string inputText)
		{
			List<string> list = new List<string>();
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			for (int i = 0; i < inputText.Length; i++)
			{
				char c = inputText[i];
				if (TranslatorEngine.isChinese(c))
				{
					if (flag)
					{
						stringBuilder.Append(c);
					}
					else
					{
						list.Add(stringBuilder.ToString());
						stringBuilder.Length = 0;
						stringBuilder.Append(c);
					}
					flag = true;
				}
				else
				{
					if (!flag)
					{
						stringBuilder.Append(c);
					}
					else
					{
						list.Add(stringBuilder.ToString());
						stringBuilder.Length = 0;
						stringBuilder.Append(c);
					}
					flag = false;
				}
			}
			list.Add(stringBuilder.ToString());
			return list.ToArray();
		}

		public static bool IsInVietPhrase(string chinese)
		{
			return TranslatorEngine.vietPhraseDictionary.ContainsKey(chinese);
		}

		public static string ChineseToHanVietForAnalyzer(string chinese)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < chinese.Length; i++)
			{
				char c = chinese[i];
				if (TranslatorEngine.hanVietDictionary.ContainsKey(c.ToString()))
				{
					stringBuilder.Append(TranslatorEngine.hanVietDictionary[c.ToString()] + " ");
				}
				else
				{
					stringBuilder.Append(c + " ");
				}
			}
			return stringBuilder.ToString().Trim();
		}

		public static string ChineseToVietPhraseForAnalyzer(string chinese, int translationAlgorithm, bool prioritizedName)
		{
			return TranslatorEngine.ChineseToVietPhraseForBrowser(chinese, 11, translationAlgorithm, prioritizedName).Trim(TranslatorEngine.trimCharsForAnalyzer);
		}

		private static bool containsName(string chinese, int startIndex, int phraseLength)
		{
			if (phraseLength < 2)
			{
				return false;
			}
			if (TranslatorEngine.onlyNameDictionary.ContainsKey(chinese.Substring(startIndex, phraseLength)))
			{
				return false;
			}
			int num = startIndex + phraseLength - 1;
			int num2 = 2;
			for (int i = startIndex + 1; i <= num; i++)
			{
				for (int j = 20; j >= num2; j--)
				{
					if (chinese.Length >= i + j && TranslatorEngine.onlyNameDictionary.ContainsKey(chinese.Substring(i, j)))
					{
						return true;
					}
				}
			}
			return false;
		}

		private static bool isLongestPhraseInSentence(string chinese, int startIndex, int phraseLength, Dictionary<string, string> dictionary, int translationAlgorithm)
		{
			if (phraseLength < 2)
			{
				return true;
			}
			int num = (translationAlgorithm == 0) ? phraseLength : ((phraseLength < 3) ? 3 : phraseLength);
			int num2 = startIndex + phraseLength - 1;
			for (int i = startIndex + 1; i <= num2; i++)
			{
				for (int j = 20; j > num; j--)
				{
					if (chinese.Length >= i + j && dictionary.ContainsKey(chinese.Substring(i, j)))
					{
						return false;
					}
				}
			}
			return true;
		}

		public static int GetVietPhraseDictionaryCount()
		{
			return TranslatorEngine.onlyVietPhraseDictionary.Count;
		}

		public static int GetNameDictionaryCount()
		{
			return TranslatorEngine.onlyNameDictionary.Count;
		}

		public static int GetPhienAmDictionaryCount()
		{
			return TranslatorEngine.hanVietDictionary.Count;
		}

		public static bool ExistInPhienAmDictionary(string chinese)
		{
			return chinese.Length == 1 && TranslatorEngine.hanVietDictionary.ContainsKey(chinese);
		}

		private static void updateHistoryLogInCache(string key, string action, ref DataSet dictionaryHistoryDataSet)
		{
			string name = "DictionaryHistory";
			DataRow dataRow = dictionaryHistoryDataSet.Tables[name].Rows.Find(key);
			if (dataRow == null)
			{
				dictionaryHistoryDataSet.Tables[name].Rows.Add(new object[]
				{
					key,
					action,
					Environment.GetEnvironmentVariable("USERNAME"),
					DateTime.Now
				});
				return;
			}
			dataRow[1] = action;
			dataRow[2] = Environment.GetEnvironmentVariable("USERNAME");
			dataRow[3] = DateTime.Now;
		}

		private static void writeVietPhraseHistoryLog(string key, string action)
		{
			TranslatorEngine.updateHistoryLogInCache(key, action, ref TranslatorEngine.onlyVietPhraseDictionaryHistoryDataSet);
			TranslatorEngine.WriteHistoryLog(key, action, DictionaryConfigurationHelper.GetVietPhraseDictionaryHistoryPath());
		}

		private static void writeNamesHistoryLog(string key, string action)
		{
			TranslatorEngine.updateHistoryLogInCache(key, action, ref TranslatorEngine.onlyNameDictionaryHistoryDataSet);
			TranslatorEngine.WriteHistoryLog(key, action, DictionaryConfigurationHelper.GetNamesDictionaryHistoryPath());
		}

		private static void writePhienAmHistoryLog(string key, string action)
		{
			TranslatorEngine.updateHistoryLogInCache(key, action, ref TranslatorEngine.hanVietDictionaryHistoryDataSet);
			TranslatorEngine.WriteHistoryLog(key, action, DictionaryConfigurationHelper.GetChinesePhienAmWordsDictionaryHistoryPath());
		}

		public static string GetVietPhraseHistoryLogRecord(string key)
		{
			return TranslatorEngine.getDictionaryHistoryLogRecordInCache(key, TranslatorEngine.onlyVietPhraseDictionaryHistoryDataSet);
		}

		public static string GetNameHistoryLogRecord(string key)
		{
			return TranslatorEngine.getDictionaryHistoryLogRecordInCache(key, TranslatorEngine.onlyNameDictionaryHistoryDataSet);
		}

		public static string GetPhienAmHistoryLogRecord(string key)
		{
			return TranslatorEngine.getDictionaryHistoryLogRecordInCache(key, TranslatorEngine.hanVietDictionaryHistoryDataSet);
		}

		private static string getDictionaryHistoryLogRecordInCache(string key, DataSet dictionaryHistoryDataSet)
		{
			string name = "DictionaryHistory";
			DataRow dataRow = dictionaryHistoryDataSet.Tables[name].Rows.Find(key);
			if (dataRow == null)
			{
				return "";
			}
			return string.Format("Entry này đã được <{0}> bởi <{1}> vào <{2}>.", dataRow[1], dataRow[2], ((DateTime)dataRow[3]).ToString("yyyy-MM-dd HH:mm:ss.fffzzz"));
		}

		public static void CompressPhienAmDictionaryHistory()
		{
			TranslatorEngine.CompressDictionaryHistory(TranslatorEngine.hanVietDictionaryHistoryDataSet, DictionaryConfigurationHelper.GetChinesePhienAmWordsDictionaryHistoryPath());
		}

		public static void CompressOnlyVietPhraseDictionaryHistory()
		{
			TranslatorEngine.CompressDictionaryHistory(TranslatorEngine.onlyVietPhraseDictionaryHistoryDataSet, DictionaryConfigurationHelper.GetVietPhraseDictionaryHistoryPath());
		}

		public static void CompressOnlyNameDictionaryHistory()
		{
			TranslatorEngine.CompressDictionaryHistory(TranslatorEngine.onlyNameDictionaryHistoryDataSet, DictionaryConfigurationHelper.GetNamesDictionaryHistoryPath());
		}

		private static void CompressDictionaryHistory(DataSet dictionaryHistoryDataSet, string dictionaryHistoryFilePath)
		{
			string name = "DictionaryHistory";
			string text = dictionaryHistoryFilePath + "." + DateTime.Now.Ticks;
			if (File.Exists(dictionaryHistoryFilePath))
			{
				File.Copy(dictionaryHistoryFilePath, text, true);
			}
			using (TextWriter textWriter = new StreamWriter(dictionaryHistoryFilePath, false, Encoding.UTF8))
			{
				try
				{
					textWriter.WriteLine("Entry\tAction\tUser Name\tUpdated Date");
					DataTable dataTable = dictionaryHistoryDataSet.Tables[name];
					foreach (DataRow dataRow in dataTable.Rows)
					{
						textWriter.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}", new object[]
						{
							dataRow[0],
							dataRow[1],
							dataRow[2],
							((DateTime)dataRow[3]).ToString("yyyy-MM-dd HH:mm:ss.fffzzz")
						}));
					}
				}
				catch (Exception ex)
				{
					try
					{
						textWriter.Close();
					}
					catch
					{
					}
					if (File.Exists(dictionaryHistoryFilePath))
					{
						try
						{
							File.Copy(text, dictionaryHistoryFilePath, true);
						}
						catch
						{
						}
					}
					throw ex;
				}
				finally
				{
					File.Delete(text);
				}
			}
		}

		public static void WriteHistoryLog(string key, string action, string logPath)
		{
			if (!File.Exists(logPath))
			{
				File.AppendAllText(logPath, "Entry\tAction\tUser Name\tUpdated Date\r\n", Encoding.UTF8);
			}
			File.AppendAllText(logPath, string.Concat(new string[]
			{
				key,
				"\t",
				action,
				"\t",
				Environment.GetEnvironmentVariable("USERNAME"),
				"\t",
				DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffzzz"),
				"\r\n"
			}), Encoding.UTF8);
		}

		public static void CreateHistoryLog(string key, string action, ref StringBuilder historyLogs)
		{
			historyLogs.AppendLine(string.Concat(new string[]
			{
				key,
				"\t",
				action,
				"\t",
				Environment.GetEnvironmentVariable("USERNAME"),
				"\t",
				DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffzzz")
			}));
		}

		public static void WriteHistoryLog(string historyLogs, string logPath)
		{
			if (!File.Exists(logPath))
			{
				File.AppendAllText(logPath, "Entry\tAction\tUser Name\tUpdated Date\r\n", Encoding.UTF8);
			}
			File.AppendAllText(logPath, historyLogs, Encoding.UTF8);
		}

		private static string removeIgnoredChinesePhrases(string standardizedChinese)
		{
			if (string.IsNullOrEmpty(standardizedChinese))
			{
				return string.Empty;
			}
			string text = standardizedChinese;
			foreach (string current in TranslatorEngine.ignoredChinesePhraseList)
			{
				text = text.Replace(current, string.Empty);
			}
			return text.Replace("\t\n\n", string.Empty);
		}

		private static string removeIgnoredChinesePhrasesForBrowser(string standardizedChinese)
		{
			if (string.IsNullOrEmpty(standardizedChinese))
			{
				return string.Empty;
			}
			string text = standardizedChinese;
			foreach (string current in TranslatorEngine.ignoredChinesePhraseForBrowserList)
			{
				text = text.Replace(current, string.Empty);
			}
			return text.Replace("\t\n\n", string.Empty);
		}
	}
}
