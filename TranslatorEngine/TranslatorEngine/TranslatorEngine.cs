using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

		private static Dictionary<string, string> onlyNameOneMeaningDictionary = new Dictionary<string, string>();

		private static Dictionary<string, string> onlyNameChinhDictionary = new Dictionary<string, string>();

		private static Dictionary<string, string> onlyNamePhuDictionary = new Dictionary<string, string>();

		private static Dictionary<string, string> luatNhanDictionary = new Dictionary<string, string>();

		private static Dictionary<string, string> pronounDictionary = new Dictionary<string, string>();

		private static Dictionary<string, string> pronounOneMeaningDictionary = new Dictionary<string, string>();

		private static Dictionary<string, string> nhanByDictionary = null;

		private static Dictionary<string, string> nhanByOneMeaningDictionary = null;

		private static DataSet onlyVietPhraseDictionaryHistoryDataSet = new DataSet();

		private static DataSet onlyNameDictionaryHistoryDataSet = new DataSet();

		private static DataSet onlyNamePhuDictionaryHistoryDataSet = new DataSet();

		private static DataSet hanVietDictionaryHistoryDataSet = new DataSet();

		private static List<string> ignoredChinesePhraseList = new List<string>();

		private static List<string> ignoredChinesePhraseForBrowserList = new List<string>();

		private static object lockObject = new object();

		private static string NULL_STRING = Convert.ToChar(0).ToString();

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

		public static string GetVietPhraseOrNameValueFromKey(string key)
		{
			if (!TranslatorEngine.vietPhraseDictionary.ContainsKey(key))
			{
				return null;
			}
			return TranslatorEngine.vietPhraseDictionary[key];
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

		public static string GetNameValueFromKey(string key, bool isNameChinh)
		{
			Dictionary<string, string> dictionary = isNameChinh ? TranslatorEngine.onlyNameChinhDictionary : TranslatorEngine.onlyNamePhuDictionary;
			if (!dictionary.ContainsKey(key))
			{
				return null;
			}
			return dictionary[key];
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

		public static void DeleteKeyFromNameDictionary(string key, bool sorting, bool isNameChinh)
		{
			TranslatorEngine.vietPhraseDictionary.Remove(key);
			TranslatorEngine.vietPhraseOneMeaningDictionary.Remove(key);
			TranslatorEngine.onlyNameDictionary.Remove(key);
			TranslatorEngine.onlyNameOneMeaningDictionary.Remove(key);
			Dictionary<string, string> dictionary = isNameChinh ? TranslatorEngine.onlyNameChinhDictionary : TranslatorEngine.onlyNamePhuDictionary;
			if (!dictionary.ContainsKey(key))
			{
				return;
			}
			dictionary.Remove(key);
			if (sorting)
			{
				TranslatorEngine.SaveDictionaryToFile(ref dictionary, isNameChinh ? DictionaryConfigurationHelper.GetNamesDictionaryPath() : DictionaryConfigurationHelper.GetNamesPhuDictionaryPath());
			}
			else
			{
				TranslatorEngine.SaveDictionaryToFileWithoutSorting(dictionary, isNameChinh ? DictionaryConfigurationHelper.GetNamesDictionaryPath() : DictionaryConfigurationHelper.GetNamesPhuDictionaryPath());
			}
			TranslatorEngine.writeNamesHistoryLog(key, "Deleted", isNameChinh);
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

		public static void UpdateNameDictionary(string key, string value, bool sorting, bool isNameChinh)
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
			Dictionary<string, string> dictionary = isNameChinh ? TranslatorEngine.onlyNameChinhDictionary : TranslatorEngine.onlyNamePhuDictionary;
			if (dictionary.ContainsKey(key))
			{
				dictionary[key] = value;
				TranslatorEngine.writeNamesHistoryLog(key, "Updated", isNameChinh);
			}
			else
			{
				if (sorting)
				{
					dictionary.Add(key, value);
				}
				else if (isNameChinh)
				{
					TranslatorEngine.onlyNameChinhDictionary = TranslatorEngine.AddEntryToDictionaryWithoutSorting(TranslatorEngine.onlyNameChinhDictionary, key, value);
					dictionary = TranslatorEngine.onlyNameChinhDictionary;
				}
				else
				{
					TranslatorEngine.onlyNamePhuDictionary = TranslatorEngine.AddEntryToDictionaryWithoutSorting(TranslatorEngine.onlyNamePhuDictionary, key, value);
					dictionary = TranslatorEngine.onlyNamePhuDictionary;
				}
				TranslatorEngine.writeNamesHistoryLog(key, "Added", isNameChinh);
			}
			if (TranslatorEngine.onlyNameDictionary.ContainsKey(key))
			{
				TranslatorEngine.onlyNameDictionary[key] = value;
				TranslatorEngine.onlyNameOneMeaningDictionary[key] = value.Split(new char[]
				{
					'/',
					'|'
				})[0];
			}
			else if (sorting)
			{
				TranslatorEngine.onlyNameDictionary.Add(key, value);
				TranslatorEngine.onlyNameOneMeaningDictionary.Add(key, value.Split(new char[]
				{
					'/',
					'|'
				})[0]);
			}
			else
			{
				TranslatorEngine.onlyNameDictionary = TranslatorEngine.AddEntryToDictionaryWithoutSorting(TranslatorEngine.onlyNameDictionary, key, value);
				TranslatorEngine.onlyNameOneMeaningDictionary = TranslatorEngine.AddEntryToDictionaryWithoutSorting(TranslatorEngine.onlyNameOneMeaningDictionary, key, value.Split(new char[]
				{
					'/',
					'|'
				})[0]);
			}
			if (sorting)
			{
				TranslatorEngine.SaveDictionaryToFile(ref dictionary, isNameChinh ? DictionaryConfigurationHelper.GetNamesDictionaryPath() : DictionaryConfigurationHelper.GetNamesPhuDictionaryPath());
				return;
			}
			TranslatorEngine.SaveDictionaryToFileWithoutSorting(dictionary, isNameChinh ? DictionaryConfigurationHelper.GetNamesDictionaryPath() : DictionaryConfigurationHelper.GetNamesPhuDictionaryPath());
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
			if (File.Exists(filePath))
			{
				File.Delete(text);
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
			if (File.Exists(filePath))
			{
				File.Delete(text);
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
			int num = array.Length;
			for (int i = 0; i < num; i++)
			{
				string text = array[i];
				if (!string.IsNullOrEmpty(text))
				{
					string text2;
					if (TranslatorEngine.isChinese(text[0]))
					{
						CharRange[] array2;
						text2 = TranslatorEngine.ChineseToHanViet(text, out array2).TrimStart(new char[0]);
						if (i == 0 || !array[i - 1].EndsWith(", "))
						{
							text2 = TranslatorEngine.toUpperCase(text2);
						}
					}
					else
					{
						text2 = text;
					}
					stringBuilder.Append(text2);
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
			int num2 = -1;
			int num3 = -1;
			int num4 = -1;
			TranslatorEngine.loadNhanByDictionary();
			while (i <= num)
			{
				bool flag = false;
				bool flag2 = true;
				for (int j = 20; j > 0; j--)
				{
					if (chinese.Length >= i + j)
					{
						if (TranslatorEngine.vietPhraseDictionary.ContainsKey(chinese.Substring(i, j)))
						{
							if ((!prioritizedName || !TranslatorEngine.containsName(chinese, i, j)) && ((translationAlgorithm != 0 && translationAlgorithm != 2) || TranslatorEngine.isLongestPhraseInSentence(chinese, i, j, TranslatorEngine.vietPhraseDictionary, translationAlgorithm) || (prioritizedName && TranslatorEngine.onlyNameDictionary.ContainsKey(chinese.Substring(i, j)))))
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
						else if (!chinese.Substring(i, j).Contains("\n") && !chinese.Substring(i, j).Contains("\t") && TranslatorEngine.nhanByDictionary != null && flag2 && 2 < j && num2 < i + j - 1 && TranslatorEngine.IsAllChinese(chinese.Substring(i, j)))
						{
							if (i < num3)
							{
								if (num3 < i + j && j <= num4 - num3)
								{
									j = num3 - i + 1;
								}
							}
							else
							{
								string empty = string.Empty;
								int num5 = -1;
								int num6 = TranslatorEngine.containsLuatNhan(chinese.Substring(i, j), TranslatorEngine.nhanByDictionary, out empty, out num5);
								num3 = i + num6;
								num4 = num3 + num5;
								if (num6 == 0)
								{
									if (TranslatorEngine.isLongestPhraseInSentence(chinese, i - 1, num5 - 1, TranslatorEngine.vietPhraseOneMeaningDictionary, translationAlgorithm))
									{
										j = num5;
										list.Add(new CharRange(i, j));
										string text = TranslatorEngine.ChineseToLuatNhan(chinese.Substring(i, j), TranslatorEngine.nhanByDictionary);
										if (wrapType == 0)
										{
											TranslatorEngine.appendTranslatedWord(stringBuilder, text, ref TranslatorEngine.LastTranslatedWord_VietPhrase);
											list2.Add(new CharRange(stringBuilder.ToString().Length - text.Length, text.Length));
										}
										else if (wrapType == 1 || wrapType == 11)
										{
											TranslatorEngine.appendTranslatedWord(stringBuilder, "[" + text + "]", ref TranslatorEngine.LastTranslatedWord_VietPhrase);
											list2.Add(new CharRange(stringBuilder.ToString().Length - text.Length - 2, text.Length + 2));
										}
										else if (TranslatorEngine.hasOnlyOneMeaning(text))
										{
											TranslatorEngine.appendTranslatedWord(stringBuilder, text, ref TranslatorEngine.LastTranslatedWord_VietPhrase);
											list2.Add(new CharRange(stringBuilder.ToString().Length - text.Length, text.Length));
										}
										else
										{
											TranslatorEngine.appendTranslatedWord(stringBuilder, "[" + text + "]", ref TranslatorEngine.LastTranslatedWord_VietPhrase);
											list2.Add(new CharRange(stringBuilder.ToString().Length - text.Length - 2, text.Length + 2));
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
								else if (0 >= num6)
								{
									num2 = i + j - 1;
									flag2 = false;
									int num7 = 100;
									while (i + num7 < chinese.Length && TranslatorEngine.isChinese(chinese[i + num7 - 1]))
									{
										num7++;
									}
									if (i + num7 <= chinese.Length)
									{
										num6 = TranslatorEngine.containsLuatNhan(chinese.Substring(i, num7), TranslatorEngine.nhanByDictionary, out empty, out num5);
										if (num6 < 0)
										{
											num2 = i + num7 - 1;
										}
									}
								}
							}
						}
					}
				}
				if (!flag)
				{
					int length = stringBuilder.ToString().Length;
					int num8 = TranslatorEngine.ChineseToHanViet(chinese[i]).Length;
					list.Add(new CharRange(i, 1));
					if (TranslatorEngine.isChinese(chinese[i]))
					{
						TranslatorEngine.appendTranslatedWord(stringBuilder, ((wrapType != 1) ? "" : "[") + TranslatorEngine.ChineseToHanViet(chinese[i]) + ((wrapType != 1) ? "" : "]"), ref TranslatorEngine.LastTranslatedWord_VietPhrase);
						if (TranslatorEngine.nextCharIsChinese(chinese, i))
						{
							stringBuilder.Append(" ");
							TranslatorEngine.LastTranslatedWord_VietPhrase += " ";
						}
						num8 += ((wrapType != 1) ? 0 : 2);
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
						num8 = 1;
					}
					list2.Add(new CharRange(length, num8));
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
			int num2 = -1;
			int num3 = -1;
			int num4 = -1;
			while (i <= num)
			{
				bool flag = false;
				bool flag2 = true;
				for (int j = 20; j > 0; j--)
				{
					if (chinese.Length >= i + j)
					{
						if (TranslatorEngine.vietPhraseDictionary.ContainsKey(chinese.Substring(i, j)))
						{
							if ((!prioritizedName || !TranslatorEngine.containsName(chinese, i, j)) && ((translationAlgorithm != 0 && translationAlgorithm != 2) || TranslatorEngine.isLongestPhraseInSentence(chinese, i, j, TranslatorEngine.vietPhraseDictionary, translationAlgorithm) || (prioritizedName && TranslatorEngine.onlyNameDictionary.ContainsKey(chinese.Substring(i, j)))))
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
						else if (!chinese.Substring(i, j).Contains("\n") && !chinese.Substring(i, j).Contains("\t") && TranslatorEngine.nhanByDictionary != null && flag2 && 2 < j && num2 < i + j - 1 && TranslatorEngine.IsAllChinese(chinese.Substring(i, j)))
						{
							if (i < num3)
							{
								if (num3 < i + j && j <= num4 - num3)
								{
									j = num3 - i + 1;
								}
							}
							else
							{
								string empty = string.Empty;
								int num5 = -1;
								int num6 = TranslatorEngine.containsLuatNhan(chinese.Substring(i, j), TranslatorEngine.nhanByDictionary, out empty, out num5);
								num3 = i + num6;
								num4 = num3 + num5;
								if (num6 == 0)
								{
									if (TranslatorEngine.isLongestPhraseInSentence(chinese, i - 1, num5 - 1, TranslatorEngine.vietPhraseOneMeaningDictionary, translationAlgorithm))
									{
										j = num5;
										string text2 = TranslatorEngine.ChineseToLuatNhan(chinese.Substring(i, j), TranslatorEngine.nhanByDictionary);
										if (wrapType == 0)
										{
											TranslatorEngine.appendTranslatedWord(stringBuilder, text2, ref text);
										}
										else if (wrapType == 1 || wrapType == 11)
										{
											TranslatorEngine.appendTranslatedWord(stringBuilder, "[" + text2 + "]", ref text);
										}
										else if (TranslatorEngine.hasOnlyOneMeaning(text2))
										{
											TranslatorEngine.appendTranslatedWord(stringBuilder, text2, ref text);
										}
										else
										{
											TranslatorEngine.appendTranslatedWord(stringBuilder, "[" + text2 + "]", ref text);
										}
										if (TranslatorEngine.nextCharIsChinese(chinese, i + j - 1))
										{
											stringBuilder.Append(" ");
											text += " ";
										}
										flag = true;
										i += j;
										break;
									}
								}
								else if (0 >= num6)
								{
									num2 = i + j - 1;
									flag2 = false;
									int num7 = 100;
									while (i + num7 < chinese.Length && TranslatorEngine.isChinese(chinese[i + num7 - 1]))
									{
										num7++;
									}
									if (i + num7 <= chinese.Length)
									{
										num6 = TranslatorEngine.containsLuatNhan(chinese.Substring(i, num7), TranslatorEngine.nhanByDictionary, out empty, out num5);
										if (num6 < 0)
										{
											num2 = i + num7 - 1;
										}
									}
								}
							}
						}
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
			int num2 = -1;
			int num3 = -1;
			int num4 = -1;
			TranslatorEngine.loadNhanByOneMeaningDictionary();
			while (i <= num)
			{
				bool flag = false;
				bool flag2 = true;
				for (int j = 20; j > 0; j--)
				{
					if (chinese.Length >= i + j)
					{
						if (TranslatorEngine.vietPhraseOneMeaningDictionary.ContainsKey(chinese.Substring(i, j)))
						{
							if ((!prioritizedName || !TranslatorEngine.containsName(chinese, i, j)) && ((translationAlgorithm != 0 && translationAlgorithm != 2) || TranslatorEngine.isLongestPhraseInSentence(chinese, i, j, TranslatorEngine.vietPhraseOneMeaningDictionary, translationAlgorithm) || (prioritizedName && TranslatorEngine.onlyNameDictionary.ContainsKey(chinese.Substring(i, j)))))
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
						else if (!chinese.Substring(i, j).Contains("\n") && !chinese.Substring(i, j).Contains("\t") && TranslatorEngine.nhanByOneMeaningDictionary != null && flag2 && 2 < j && num2 < i + j - 1 && TranslatorEngine.IsAllChinese(chinese.Substring(i, j)))
						{
							if (i < num3)
							{
								if (num3 < i + j && j <= num4 - num3)
								{
									j = num3 - i + 1;
								}
							}
							else
							{
								string empty = string.Empty;
								int num5 = -1;
								int num6 = TranslatorEngine.containsLuatNhan(chinese.Substring(i, j), TranslatorEngine.nhanByOneMeaningDictionary, out empty, out num5);
								num3 = i + num6;
								num4 = num3 + num5;
								if (num6 == 0)
								{
									if (TranslatorEngine.isLongestPhraseInSentence(chinese, i - 1, num5 - 1, TranslatorEngine.vietPhraseOneMeaningDictionary, translationAlgorithm))
									{
										j = num5;
										list.Add(new CharRange(i, j));
										string text = TranslatorEngine.ChineseToLuatNhan(chinese.Substring(i, j), TranslatorEngine.nhanByOneMeaningDictionary);
										if (wrapType == 0)
										{
											TranslatorEngine.appendTranslatedWord(stringBuilder, text, ref TranslatorEngine.LastTranslatedWord_VietPhraseOneMeaning);
											list2.Add(new CharRange(stringBuilder.ToString().Length - text.Length, text.Length));
										}
										else
										{
											TranslatorEngine.appendTranslatedWord(stringBuilder, "[" + text + "]", ref TranslatorEngine.LastTranslatedWord_VietPhraseOneMeaning);
											list2.Add(new CharRange(stringBuilder.ToString().Length - text.Length - 2, text.Length + 2));
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
								else if (0 >= num6)
								{
									num2 = i + j - 1;
									flag2 = false;
									int num7 = 100;
									while (i + num7 < chinese.Length && TranslatorEngine.isChinese(chinese[i + num7 - 1]))
									{
										num7++;
									}
									if (i + num7 <= chinese.Length)
									{
										num6 = TranslatorEngine.containsLuatNhan(chinese.Substring(i, num7), TranslatorEngine.nhanByOneMeaningDictionary, out empty, out num5);
										if (num6 < 0)
										{
											num2 = i + num7 - 1;
										}
									}
								}
							}
						}
					}
				}
				if (!flag)
				{
					int length = stringBuilder.ToString().Length;
					int num8 = TranslatorEngine.ChineseToHanViet(chinese[i]).Length;
					list.Add(new CharRange(i, 1));
					if (TranslatorEngine.isChinese(chinese[i]))
					{
						TranslatorEngine.appendTranslatedWord(stringBuilder, ((wrapType != 1) ? "" : "[") + TranslatorEngine.ChineseToHanViet(chinese[i]) + ((wrapType != 1) ? "" : "]"), ref TranslatorEngine.LastTranslatedWord_VietPhraseOneMeaning);
						if (TranslatorEngine.nextCharIsChinese(chinese, i))
						{
							stringBuilder.Append(" ");
							TranslatorEngine.LastTranslatedWord_VietPhraseOneMeaning += " ";
						}
						num8 += ((wrapType != 1) ? 0 : 2);
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
						num8 = 1;
					}
					list2.Add(new CharRange(length, num8));
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
			int num = array.Length;
			for (int i = 0; i < num; i++)
			{
				string text = array[i];
				if (!string.IsNullOrEmpty(text))
				{
					string text2;
					if (TranslatorEngine.isChinese(text[0]))
					{
						CharRange[] array2;
						CharRange[] array3;
						text2 = TranslatorEngine.ChineseToVietPhraseOneMeaning(text, wrapType, translationAlgorithm, prioritizedName, out array2, out array3).TrimStart(new char[0]);
						if (i == 0 || !array[i - 1].EndsWith(", "))
						{
							text2 = TranslatorEngine.toUpperCase(text2);
						}
					}
					else
					{
						text2 = text;
					}
					stringBuilder.Append(text2);
				}
			}
			return stringBuilder.ToString();
		}

		public static string ChineseToVietPhraseOneMeaningForProxy(string chinese, int wrapType, int translationAlgorithm, bool prioritizedName)
		{
			chinese = TranslatorEngine.StandardizeInputForProxy(chinese);
			StringBuilder stringBuilder = new StringBuilder();
			string[] array = TranslatorEngine.classifyWordsIntoLatinAndChineseForProxy(chinese);
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
			DateTime arg_05_0 = DateTime.Now;
			string text = "";
			StringBuilder stringBuilder = new StringBuilder();
			int num = chinese.Length - 1;
			int i = 0;
			int num2 = -1;
			int num3 = -1;
			int num4 = -1;
			while (i <= num)
			{
				bool flag = false;
				bool flag2 = true;
				if (chinese[i] != '\n' && chinese[i] != '\t')
				{
					for (int j = 20; j > 0; j--)
					{
						if (chinese.Length >= i + j)
						{
							if (TranslatorEngine.vietPhraseOneMeaningDictionary.ContainsKey(chinese.Substring(i, j)))
							{
								if ((!prioritizedName || !TranslatorEngine.containsName(chinese, i, j)) && ((translationAlgorithm != 0 && translationAlgorithm != 2) || TranslatorEngine.isLongestPhraseInSentence(chinese, i, j, TranslatorEngine.vietPhraseOneMeaningDictionary, translationAlgorithm) || (prioritizedName && TranslatorEngine.onlyNameDictionary.ContainsKey(chinese.Substring(i, j)))))
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
							else if (!chinese.Substring(i, j).Contains("\n") && !chinese.Substring(i, j).Contains("\t") && TranslatorEngine.nhanByOneMeaningDictionary != null && flag2 && 2 < j && num2 < i + j - 1 && TranslatorEngine.IsAllChinese(chinese.Substring(i, j)))
							{
								if (i < num3)
								{
									if (num3 < i + j && j <= num4 - num3)
									{
										j = num3 - i + 1;
									}
								}
								else
								{
									string empty = string.Empty;
									int num5 = -1;
									int num6 = TranslatorEngine.containsLuatNhan(chinese.Substring(i, j), TranslatorEngine.nhanByOneMeaningDictionary, out empty, out num5);
									num3 = i + num6;
									num4 = num3 + num5;
									if (num6 == 0)
									{
										if (TranslatorEngine.isLongestPhraseInSentence(chinese, i - 1, num5 - 1, TranslatorEngine.vietPhraseOneMeaningDictionary, translationAlgorithm))
										{
											j = num5;
											string text2 = TranslatorEngine.ChineseToLuatNhan(chinese.Substring(i, j), TranslatorEngine.nhanByOneMeaningDictionary);
											if (wrapType == 0)
											{
												TranslatorEngine.appendTranslatedWord(stringBuilder, text2, ref text);
											}
											else
											{
												TranslatorEngine.appendTranslatedWord(stringBuilder, "[" + text2 + "]", ref text);
											}
											if (TranslatorEngine.nextCharIsChinese(chinese, i + j - 1))
											{
												stringBuilder.Append(" ");
												text += " ";
											}
											flag = true;
											i += j;
											break;
										}
									}
									else if (0 >= num6)
									{
										num2 = i + j - 1;
										flag2 = false;
										int num7 = 100;
										while (i + num7 < chinese.Length && TranslatorEngine.isChinese(chinese[i + num7 - 1]))
										{
											num7++;
										}
										if (i + num7 <= chinese.Length)
										{
											num6 = TranslatorEngine.containsLuatNhan(chinese.Substring(i, num7), TranslatorEngine.nhanByOneMeaningDictionary, out empty, out num5);
											if (num6 < 0)
											{
												num2 = i + num7 - 1;
											}
										}
									}
								}
							}
						}
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
				if (chinese.Length >= i && !chinese.Substring(0, i).Contains("\n") && !chinese.Substring(0, i).Contains("\t"))
				{
					if (TranslatorEngine.containsLuatNhan(chinese.Substring(0, i), TranslatorEngine.vietPhraseDictionary) != 0)
					{
						break;
					}
					if (TranslatorEngine.matchesLuatNhan(chinese.Substring(0, i), TranslatorEngine.vietPhraseDictionary))
					{
						string empty = string.Empty;
						TranslatorEngine.ChineseToLuatNhan(chinese.Substring(0, i), TranslatorEngine.vietPhraseDictionary, out empty);
						string text2 = text;
						text = string.Concat(new string[]
						{
							text2,
							empty,
							" <<Luật Nhân>> ",
							TranslatorEngine.luatNhanDictionary[empty].Replace("/", "; "),
							"\n-----------------\n"
						});
						if (num == 0)
						{
							num = i;
						}
					}
				}
			}
			for (int j = 20; j > 0; j--)
			{
				if (chinese.Length >= j)
				{
					string text3 = chinese.Substring(0, j);
					if (TranslatorEngine.vietPhraseDictionary.ContainsKey(text3))
					{
						string text4 = text;
						text = string.Concat(new string[]
						{
							text4,
							text3,
							" <<VietPhrase>> ",
							TranslatorEngine.vietPhraseDictionary[text3].Replace("/", "; "),
							"\n-----------------\n"
						});
						if (num == 0)
						{
							num = text3.Length;
						}
					}
				}
			}
			for (int k = 20; k > 0; k--)
			{
				if (chinese.Length >= k)
				{
					string text3 = chinese.Substring(0, k);
					if (TranslatorEngine.lacVietDictionary.ContainsKey(text3))
					{
						string text5 = text;
						text = string.Concat(new string[]
						{
							text5,
							text3,
							" <<Lạc Việt>>\n",
							TranslatorEngine.lacVietDictionary[text3],
							"\n-----------------\n"
						});
						if (num == 0)
						{
							num = 1;
						}
					}
				}
			}
			for (int l = 20; l > 0; l--)
			{
				if (chinese.Length >= l)
				{
					string text3 = chinese.Substring(0, l);
					if (TranslatorEngine.cedictDictionary.ContainsKey(text3))
					{
						string text6 = text;
						text = string.Concat(new string[]
						{
							text6,
							text3,
							" <<Cedict or Babylon>> ",
							TranslatorEngine.cedictDictionary[text3].Replace("] /", "] ").Replace("/", "; "),
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
			for (int m = 0; m < num2; m++)
			{
				if (TranslatorEngine.chinesePhienAmEnglishDictionary.ContainsKey(chinese[m].ToString()))
				{
					text = text + "[" + TranslatorEngine.chinesePhienAmEnglishDictionary[chinese[m].ToString()] + "] ";
				}
				else
				{
					text = text + TranslatorEngine.ChineseToHanViet(chinese[m]) + " ";
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
					ThreadPool.QueueUserWorkItem(new WaitCallback(TranslatorEngine.loadOnlyNamePhuDictionaryHistoryWithNewThread));
					ThreadPool.QueueUserWorkItem(new WaitCallback(TranslatorEngine.loadOnlyVietPhraseDictionaryHistoryWithNewThread));
					ThreadPool.QueueUserWorkItem(new WaitCallback(TranslatorEngine.loadHanVietDictionaryHistoryWithNewThread));
					ManualResetEvent[] array = new ManualResetEvent[]
					{
						new ManualResetEvent(false)
					};
					ThreadPool.QueueUserWorkItem(new WaitCallback(TranslatorEngine.loadLuatNhanDictionaryWithNewThread), array[0]);
					ManualResetEvent[] array2 = new ManualResetEvent[]
					{
						new ManualResetEvent(false)
					};
					ThreadPool.QueueUserWorkItem(new WaitCallback(TranslatorEngine.loadPronounDictionaryWithNewThread), array2[0]);
					ManualResetEvent[] array3 = new ManualResetEvent[]
					{
						new ManualResetEvent(false)
					};
					ThreadPool.QueueUserWorkItem(new WaitCallback(TranslatorEngine.loadOnlyVietPhraseDictionaryWithNewThread), array3[0]);
					ManualResetEvent[] array4 = new ManualResetEvent[]
					{
						new ManualResetEvent(false)
					};
					ThreadPool.QueueUserWorkItem(new WaitCallback(TranslatorEngine.loadOnlyNameDictionaryWithNewThread), array4[0]);
					WaitHandle.WaitAll(array2);
					WaitHandle.WaitAll(array3);
					WaitHandle.WaitAll(array4);
					TranslatorEngine.loadVietPhraseDictionary();
					TranslatorEngine.vietPhraseDictionaryToVietPhraseOneMeaningDictionary();
					TranslatorEngine.pronounDictionaryToPronounOneMeaningDictionary();
					TranslatorEngine.loadNhanByDictionary();
					TranslatorEngine.loadNhanByOneMeaningDictionary();
					WaitHandle.WaitAll(array);
					TranslatorEngine.dictionaryDirty = false;
				}
			}
		}

		private static void loadLuatNhanDictionaryWithNewThread(object stateInfo)
		{
			TranslatorEngine.loadLuatNhanDictionary();
			((ManualResetEvent)stateInfo).Set();
		}

		private static void loadLuatNhanDictionary()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			using (TextReader textReader = new StreamReader(DictionaryConfigurationHelper.GetLuatNhanDictionaryPath(), true))
			{
				string text;
				while ((text = textReader.ReadLine()) != null)
				{
					if (!text.StartsWith("#"))
					{
						string[] array = text.Split(new char[]
						{
							'='
						});
						if (array.Length == 2 && !dictionary.ContainsKey(array[0]))
						{
							dictionary.Add(array[0], array[1]);
						}
					}
				}
			}
			IOrderedEnumerable<KeyValuePair<string, string>> orderedEnumerable = from pair in dictionary
			orderby pair.Key.Length descending, pair.Key
			select pair;
			TranslatorEngine.luatNhanDictionary.Clear();
			foreach (KeyValuePair<string, string> current in orderedEnumerable)
			{
				TranslatorEngine.luatNhanDictionary.Add(current.Key, current.Value);
			}
		}

		private static int compareLuatNhan(KeyValuePair<string, string> x, KeyValuePair<string, string> y)
		{
			if (x.Key.StartsWith("{0}") || x.Key.EndsWith("{0}"))
			{
				if (!y.Key.StartsWith("{0}") && !y.Key.EndsWith("{0}"))
				{
					return 1;
				}
			}
			else if (y.Key.StartsWith("{0}") || y.Key.EndsWith("{0}"))
			{
				return -1;
			}
			return y.Key.Length - x.Key.Length;
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
			TranslatorEngine.onlyNameOneMeaningDictionary.Clear();
			TranslatorEngine.onlyNameChinhDictionary.Clear();
			TranslatorEngine.onlyNamePhuDictionary.Clear();
			char[] separator = "/|".ToCharArray();
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
						TranslatorEngine.onlyNameOneMeaningDictionary.Add(array[0], array[1].Split(separator)[0]);
						TranslatorEngine.onlyNameChinhDictionary.Add(array[0], array[1]);
					}
				}
			}
			using (TextReader textReader2 = new StreamReader(DictionaryConfigurationHelper.GetNamesPhuDictionaryPath(), true))
			{
				string text2;
				while ((text2 = textReader2.ReadLine()) != null)
				{
					string[] array2 = text2.Split(new char[]
					{
						'='
					});
					if (array2.Length == 2 && !TranslatorEngine.onlyNamePhuDictionary.ContainsKey(array2[0]))
					{
						if (TranslatorEngine.onlyNameDictionary.ContainsKey(array2[0]))
						{
							TranslatorEngine.onlyNameDictionary[array2[0]] = array2[1];
							TranslatorEngine.onlyNameOneMeaningDictionary[array2[0]] = array2[1].Split(separator)[0];
						}
						else
						{
							TranslatorEngine.onlyNameDictionary.Add(array2[0], array2[1]);
							TranslatorEngine.onlyNameOneMeaningDictionary.Add(array2[0], array2[1].Split(separator)[0]);
						}
						TranslatorEngine.onlyNamePhuDictionary.Add(array2[0], array2[1]);
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

		private static void pronounDictionaryToPronounOneMeaningDictionary()
		{
			TranslatorEngine.pronounOneMeaningDictionary.Clear();
			foreach (KeyValuePair<string, string> current in TranslatorEngine.pronounDictionary)
			{
				TranslatorEngine.pronounOneMeaningDictionary.Add(current.Key, (current.Value.Contains("/") || current.Value.Contains("|")) ? current.Value.Split(new char[]
				{
					'/',
					'|'
				})[0] : current.Value);
			}
		}

		private static void loadNhanByDictionary()
		{
			if (DictionaryConfigurationHelper.IsNhanByPronouns)
			{
				TranslatorEngine.nhanByDictionary = TranslatorEngine.pronounDictionary;
				return;
			}
			if (DictionaryConfigurationHelper.IsNhanByPronounsAndNames)
			{
				TranslatorEngine.nhanByDictionary = new Dictionary<string, string>(TranslatorEngine.pronounDictionary);
				using (Dictionary<string, string>.Enumerator enumerator = TranslatorEngine.onlyNameDictionary.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<string, string> current = enumerator.Current;
						if (!TranslatorEngine.nhanByDictionary.ContainsKey(current.Key))
						{
							TranslatorEngine.nhanByDictionary.Add(current.Key, current.Value);
						}
					}
					return;
				}
			}
			if (DictionaryConfigurationHelper.IsNhanByPronounsAndNamesAndVietPhrase)
			{
				TranslatorEngine.nhanByDictionary = new Dictionary<string, string>(TranslatorEngine.pronounDictionary);
				using (Dictionary<string, string>.Enumerator enumerator2 = TranslatorEngine.vietPhraseDictionary.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						KeyValuePair<string, string> current2 = enumerator2.Current;
						if (!TranslatorEngine.nhanByDictionary.ContainsKey(current2.Key))
						{
							TranslatorEngine.nhanByDictionary.Add(current2.Key, current2.Value);
						}
					}
					return;
				}
			}
			TranslatorEngine.nhanByDictionary = null;
		}

		private static void loadNhanByOneMeaningDictionary()
		{
			if (DictionaryConfigurationHelper.IsNhanByPronouns)
			{
				TranslatorEngine.nhanByOneMeaningDictionary = TranslatorEngine.pronounOneMeaningDictionary;
				return;
			}
			if (DictionaryConfigurationHelper.IsNhanByPronounsAndNames)
			{
				TranslatorEngine.nhanByOneMeaningDictionary = new Dictionary<string, string>(TranslatorEngine.pronounOneMeaningDictionary);
				using (Dictionary<string, string>.Enumerator enumerator = TranslatorEngine.onlyNameOneMeaningDictionary.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<string, string> current = enumerator.Current;
						if (!TranslatorEngine.nhanByOneMeaningDictionary.ContainsKey(current.Key))
						{
							TranslatorEngine.nhanByOneMeaningDictionary.Add(current.Key, current.Value);
						}
					}
					return;
				}
			}
			if (DictionaryConfigurationHelper.IsNhanByPronounsAndNamesAndVietPhrase)
			{
				TranslatorEngine.nhanByOneMeaningDictionary = new Dictionary<string, string>(TranslatorEngine.pronounOneMeaningDictionary);
				using (Dictionary<string, string>.Enumerator enumerator2 = TranslatorEngine.vietPhraseOneMeaningDictionary.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						KeyValuePair<string, string> current2 = enumerator2.Current;
						if (!TranslatorEngine.nhanByOneMeaningDictionary.ContainsKey(current2.Key))
						{
							TranslatorEngine.nhanByOneMeaningDictionary.Add(current2.Key, current2.Value);
						}
					}
					return;
				}
			}
			TranslatorEngine.nhanByOneMeaningDictionary = null;
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

		private static void loadPronounDictionaryWithNewThread(object stateInfo)
		{
			TranslatorEngine.loadPronounDictionary();
			((ManualResetEvent)stateInfo).Set();
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

		private static void loadOnlyNamePhuDictionaryHistoryWithNewThread(object stateInfo)
		{
			TranslatorEngine.loadOnlyNamePhuDictionaryHistory();
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

		private static void loadOnlyNamePhuDictionaryHistory()
		{
			TranslatorEngine.LoadDictionaryHistory(DictionaryConfigurationHelper.GetNamesPhuDictionaryHistoryPath(), ref TranslatorEngine.onlyNamePhuDictionaryHistoryDataSet);
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

		private static void loadPronounDictionary()
		{
			TranslatorEngine.pronounDictionary.Clear();
			using (TextReader textReader = new StreamReader(DictionaryConfigurationHelper.GetPronounsDictionaryPath(), true))
			{
				string text;
				while ((text = textReader.ReadLine()) != null)
				{
					string[] array = text.Split(new char[]
					{
						'='
					});
					if (array.Length == 2 && !TranslatorEngine.pronounDictionary.ContainsKey(array[0]))
					{
						TranslatorEngine.pronounDictionary.Add(array[0], array[1]);
					}
				}
			}
		}

		public static void AddIgnoredChinesePhrase(string ignoredChinesePhrase)
		{
			if (TranslatorEngine.ignoredChinesePhraseList.Contains(ignoredChinesePhrase))
			{
				return;
			}
			TranslatorEngine.ignoredChinesePhraseList.Add(ignoredChinesePhrase);
			try
			{
				File.WriteAllLines(DictionaryConfigurationHelper.GetIgnoredChinesePhraseListPath(), TranslatorEngine.ignoredChinesePhraseList.ToArray(), Encoding.UTF8);
			}
			catch
			{
			}
			TranslatorEngine.loadIgnoredChinesePhraseLists();
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
						string text2 = TranslatorEngine.standardizeInputWithoutRemovingIgnoredChinesePhrases(text).Trim(trimChars);
						if (!string.IsNullOrEmpty(text2) && !TranslatorEngine.ignoredChinesePhraseList.Contains(text2))
						{
							TranslatorEngine.ignoredChinesePhraseList.Add(text2);
						}
						string text3 = TranslatorEngine.standardizeInputForBrowserWithoutRemovingIgnoredChinesePhrases(text).Trim(trimChars);
						if (!string.IsNullOrEmpty(text3) && !TranslatorEngine.ignoredChinesePhraseForBrowserList.Contains(text3))
						{
							TranslatorEngine.ignoredChinesePhraseForBrowserList.Add(text3);
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
			string text = TranslatorEngine.ToSimplified(original);
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
				"…",
				TranslatorEngine.NULL_STRING
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
				"...",
				""
			};
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
				if (!char.IsControl(c) || c == '\t' || c == '\n' || c == '\r')
				{
					if (TranslatorEngine.isChinese(c))
					{
						if (!TranslatorEngine.isChinese(c2) && c2 != ',' && c2 != '.' && c2 != ':' && c2 != ';' && c2 != '"' && c2 != '\'' && c2 != '?' && c2 != ' ' && c2 != '!' && c2 != ')')
						{
							stringBuilder.Append(c).Append(" ");
						}
						else
						{
							stringBuilder.Append(c);
						}
					}
					else if (c == '\t' || c == ' ' || c == '"' || c == '\'' || c == '\n' || c == '(')
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
			string text = TranslatorEngine.ToSimplified(original);
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
				"…",
				TranslatorEngine.NULL_STRING
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
				"...",
				""
			};
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

		public static string StandardizeInputForProxy(string original)
		{
			string standardizedChinese = TranslatorEngine.standardizeInputForProxyWithoutRemovingIgnoredChinesePhrases(original);
			return TranslatorEngine.removeIgnoredChinesePhrasesForBrowser(standardizedChinese);
		}

		private static string standardizeInputForProxyWithoutRemovingIgnoredChinesePhrases(string original)
		{
			if (string.IsNullOrEmpty(original) || string.IsNullOrEmpty(original))
			{
				return "";
			}
			string text = TranslatorEngine.ToSimplified(original);
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
				"…",
				TranslatorEngine.NULL_STRING
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
				"...",
				""
			};
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
			return text;
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

		public static bool IsChinese(char character)
		{
			return TranslatorEngine.isChinese(character);
		}

		public static bool IsAllChinese(string text)
		{
			for (int i = 0; i < text.Length; i++)
			{
				char character = text[i];
				if (!TranslatorEngine.isChinese(character))
				{
					return false;
				}
			}
			return true;
		}

		private static bool hasOnlyOneMeaning(string meaning)
		{
			return meaning.Split(new char[]
			{
				'/',
				'|'
			}).Length == 1;
		}

		internal static string ToSimplified(string str)
		{
			return str;
//			return Strings.StrConv(str, VbStrConv.SimplifiedChinese, 0);
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
			if (lastTranslatedWord.EndsWith("\n") || lastTranslatedWord.EndsWith("\t") || lastTranslatedWord.EndsWith(". ") || lastTranslatedWord.EndsWith("\"") || lastTranslatedWord.EndsWith("'") || lastTranslatedWord.EndsWith("? ") || lastTranslatedWord.EndsWith("! ") || lastTranslatedWord.EndsWith(".\" ") || lastTranslatedWord.EndsWith("?\" ") || lastTranslatedWord.EndsWith("!\" ") || lastTranslatedWord.EndsWith(": "))
			{
				lastTranslatedWord = TranslatorEngine.toUpperCase(translatedText);
			}
			else if (lastTranslatedWord.EndsWith(" ") || lastTranslatedWord.EndsWith("("))
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
			if (text.StartsWith("[") && 2 <= text.Length)
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

		private static string[] classifyWordsIntoLatinAndChineseForProxy(string inputText)
		{
			List<string> list = new List<string>();
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < inputText.Length; i++)
			{
				char c = inputText[i];
				if (flag2)
				{
					stringBuilder.Append(c);
					flag = false;
					if (c == '>')
					{
						list.Add(stringBuilder.ToString());
						stringBuilder.Length = 0;
						flag2 = false;
					}
				}
				else if (c == '<')
				{
					list.Add(stringBuilder.ToString());
					stringBuilder.Length = 0;
					stringBuilder.Append(c);
					flag2 = true;
					flag = false;
				}
				else if (TranslatorEngine.isChinese(c))
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

		public static int GetNameDictionaryCount(bool isNameChinh)
		{
			if (!isNameChinh)
			{
				return TranslatorEngine.onlyNamePhuDictionary.Count;
			}
			return TranslatorEngine.onlyNameChinhDictionary.Count;
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

		private static void writeNamesHistoryLog(string key, string action, bool isNameChinh)
		{
			DataSet dataSet = isNameChinh ? TranslatorEngine.onlyNameDictionaryHistoryDataSet : TranslatorEngine.onlyNamePhuDictionaryHistoryDataSet;
			TranslatorEngine.updateHistoryLogInCache(key, action, ref dataSet);
			TranslatorEngine.WriteHistoryLog(key, action, isNameChinh ? DictionaryConfigurationHelper.GetNamesDictionaryHistoryPath() : DictionaryConfigurationHelper.GetNamesPhuDictionaryHistoryPath());
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

		public static string GetNameHistoryLogRecord(string key, bool isNameChinh)
		{
			return TranslatorEngine.getDictionaryHistoryLogRecordInCache(key, isNameChinh ? TranslatorEngine.onlyNameDictionaryHistoryDataSet : TranslatorEngine.onlyNamePhuDictionaryHistoryDataSet);
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

		public static void CompressOnlyNameDictionaryHistory(bool isNameChinh)
		{
			TranslatorEngine.CompressDictionaryHistory(isNameChinh ? TranslatorEngine.onlyNameDictionaryHistoryDataSet : TranslatorEngine.onlyNamePhuDictionaryHistoryDataSet, isNameChinh ? DictionaryConfigurationHelper.GetNamesDictionaryHistoryPath() : DictionaryConfigurationHelper.GetNamesPhuDictionaryHistoryPath());
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

		private static int containsLuatNhan(string chinese, Dictionary<string, string> dictionary)
		{
			string text;
			int num;
			return TranslatorEngine.containsLuatNhan(chinese, dictionary, out text, out num);
		}

		private static int containsLuatNhan(string chinese, Dictionary<string, string> dictionary, out string luatNhan, out int matchedLength)
		{
			int length = chinese.Length;
			foreach (KeyValuePair<string, string> current in TranslatorEngine.luatNhanDictionary)
			{
				if (length >= current.Key.Length - 2)
				{
					string text = current.Key.Replace("{0}", "([^,\\. ?]{1,10})");
					Match match = Regex.Match(chinese, text);
					int num = 0;
					while (match.Success)
					{
						string value = match.Groups[1].Value;
						if (current.Key.StartsWith("{0}"))
						{
							for (int i = 0; i < value.Length; i++)
							{
								if (dictionary.ContainsKey(value.Substring(i)))
								{
									luatNhan = text;
									matchedLength = match.Length - i;
									int result = match.Index + i;
									return result;
								}
							}
						}
						else if (current.Key.EndsWith("{0}"))
						{
							int num2 = value.Length;
							while (0 < num2)
							{
								if (dictionary.ContainsKey(value.Substring(0, num2)))
								{
									luatNhan = text;
									matchedLength = match.Length - (value.Length - num2);
									int result = match.Index;
									return result;
								}
								num2--;
							}
						}
						else if (dictionary.ContainsKey(value))
						{
							luatNhan = text;
							matchedLength = match.Length;
							int result = match.Index;
							return result;
						}
						match = match.NextMatch();
						num++;
						if (num > 1)
						{
							break;
						}
					}
				}
			}
			luatNhan = string.Empty;
			matchedLength = -1;
			return -1;
		}

		private static bool matchesLuatNhan(string chinese, Dictionary<string, string> dictionary)
		{
			foreach (KeyValuePair<string, string> current in TranslatorEngine.luatNhanDictionary)
			{
				string str = current.Key.Replace("{0}", "(.+)");
				Match match = Regex.Match(chinese, "^" + str + "$");
				if (match.Success && dictionary.ContainsKey(match.Groups[1].Value))
				{
					return true;
				}
			}
			return false;
		}

		private static bool matchesLuatNhan(string chinese, Dictionary<string, string> dictionary, string luatNhan)
		{
			Match match = Regex.Match(chinese, "^" + luatNhan + "$");
			return match.Success && dictionary.ContainsKey(match.Groups[1].Value);
		}

		public static string ChineseToLuatNhan(string chinese, Dictionary<string, string> dictionary)
		{
			string empty = string.Empty;
			return TranslatorEngine.ChineseToLuatNhan(chinese, dictionary, out empty);
		}

		public static string ChineseToLuatNhan(string chinese, Dictionary<string, string> dictionary, out string luatNhan)
		{
			int arg_06_0 = chinese.Length;
			foreach (KeyValuePair<string, string> current in TranslatorEngine.luatNhanDictionary)
			{
				string str = current.Key.Replace("{0}", "(.+)");
				Match match = Regex.Match(chinese, "^" + str + "$");
				if (match.Success && dictionary.ContainsKey(match.Groups[1].Value))
				{
					string[] array = dictionary[match.Groups[1].Value].Split(new char[]
					{
						'/',
						'|'
					});
					StringBuilder stringBuilder = new StringBuilder();
					string[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						string newValue = array2[i];
						stringBuilder.Append(current.Value.Replace("{0}", newValue));
						stringBuilder.Append("/");
					}
					luatNhan = current.Key;
					return stringBuilder.ToString().Trim(new char[]
					{
						'/'
					});
				}
			}
			throw new NotImplementedException("Lỗi xử lý luật nhân cho cụm từ: " + chinese);
		}
	}
}
