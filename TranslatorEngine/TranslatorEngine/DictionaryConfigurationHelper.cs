using System;
using System.IO;
using System.Reflection;

namespace TranslatorEngine
{
	public class DictionaryConfigurationHelper
	{
		private static string directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

		private static string thuatToanNhan = string.Empty;

		public static bool IsNhanByPronouns
		{
			get
			{
				if (string.IsNullOrEmpty(DictionaryConfigurationHelper.thuatToanNhan))
				{
					DictionaryConfigurationHelper.readThuatToanNhan();
				}
				return DictionaryConfigurationHelper.thuatToanNhan == "1";
			}
		}

		public static bool IsNhanByPronounsAndNames
		{
			get
			{
				if (string.IsNullOrEmpty(DictionaryConfigurationHelper.thuatToanNhan))
				{
					DictionaryConfigurationHelper.readThuatToanNhan();
				}
				return DictionaryConfigurationHelper.thuatToanNhan == "2";
			}
		}

		public static bool IsNhanByPronounsAndNamesAndVietPhrase
		{
			get
			{
				if (string.IsNullOrEmpty(DictionaryConfigurationHelper.thuatToanNhan))
				{
					DictionaryConfigurationHelper.readThuatToanNhan();
				}
				return DictionaryConfigurationHelper.thuatToanNhan == "3";
			}
		}

		public static string GetNamesPhuDictionaryPath()
		{
			return DictionaryConfigurationHelper.GetDictionaryPathByKey("NamesPhu");
		}

		public static string GetNamesDictionaryPath()
		{
			return DictionaryConfigurationHelper.GetDictionaryPathByKey("Names");
		}

		public static string GetNamesDictionaryHistoryPath()
		{
			return Path.Combine(Path.GetDirectoryName(DictionaryConfigurationHelper.GetNamesDictionaryPath()), Path.GetFileNameWithoutExtension(DictionaryConfigurationHelper.GetNamesDictionaryPath()) + "History" + Path.GetExtension(DictionaryConfigurationHelper.GetNamesDictionaryPath()));
		}

		public static string GetNamesPhuDictionaryHistoryPath()
		{
			return Path.Combine(Path.GetDirectoryName(DictionaryConfigurationHelper.GetNamesPhuDictionaryPath()), Path.GetFileNameWithoutExtension(DictionaryConfigurationHelper.GetNamesPhuDictionaryPath()) + "History" + Path.GetExtension(DictionaryConfigurationHelper.GetNamesPhuDictionaryPath()));
		}

		public static string GetVietPhraseDictionaryPath()
		{
			return DictionaryConfigurationHelper.GetDictionaryPathByKey("VietPhrase");
		}

		public static string GetVietPhraseDictionaryHistoryPath()
		{
			return Path.Combine(Path.GetDirectoryName(DictionaryConfigurationHelper.GetVietPhraseDictionaryPath()), Path.GetFileNameWithoutExtension(DictionaryConfigurationHelper.GetVietPhraseDictionaryPath()) + "History" + Path.GetExtension(DictionaryConfigurationHelper.GetVietPhraseDictionaryPath()));
		}

		public static string GetChinesePhienAmWordsDictionaryPath()
		{
			return DictionaryConfigurationHelper.GetDictionaryPathByKey("ChinesePhienAmWords");
		}

		public static string GetChinesePhienAmWordsDictionaryHistoryPath()
		{
			return Path.Combine(Path.GetDirectoryName(DictionaryConfigurationHelper.GetChinesePhienAmWordsDictionaryPath()), Path.GetFileNameWithoutExtension(DictionaryConfigurationHelper.GetChinesePhienAmWordsDictionaryPath()) + "History" + Path.GetExtension(DictionaryConfigurationHelper.GetChinesePhienAmWordsDictionaryPath()));
		}

		public static string GetChinesePhienAmEnglishWordsDictionaryPath()
		{
			return DictionaryConfigurationHelper.GetDictionaryPathByKey("ChinesePhienAmEnglishWords");
		}

		public static string GetCEDictDictionaryPath()
		{
			return DictionaryConfigurationHelper.GetDictionaryPathByKey("CEDict");
		}

		public static string GetBabylonDictionaryPath()
		{
			return DictionaryConfigurationHelper.GetDictionaryPathByKey("Babylon");
		}

		public static string GetLacVietDictionaryPath()
		{
			return DictionaryConfigurationHelper.GetDictionaryPathByKey("LacViet");
		}

		public static string GetThieuChuuDictionaryPath()
		{
			return DictionaryConfigurationHelper.GetDictionaryPathByKey("ThieuChuu");
		}

		public static string GetIgnoredChinesePhraseListPath()
		{
			return DictionaryConfigurationHelper.GetDictionaryPathByKey("IgnoredChinesePhrases");
		}

		public static string GetLuatNhanDictionaryPath()
		{
			return DictionaryConfigurationHelper.GetDictionaryPathByKey("LuatNhan");
		}

		public static string GetPronounsDictionaryPath()
		{
			return DictionaryConfigurationHelper.GetDictionaryPathByKey("Pronouns");
		}

		private static string GetDictionaryPathByKey(string dictionaryKey)
		{
			string[] array = File.ReadAllLines(Path.Combine(DictionaryConfigurationHelper.directoryPath, "Dictionaries.config"));
			string text = string.Empty;
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text2 = array2[i];
				if (!string.IsNullOrEmpty(text2) && !text2.StartsWith("#") && text2.StartsWith(dictionaryKey + "="))
				{
					text = text2.Split(new char[]
					{
						'='
					})[1];
					break;
				}
			}
			if (!Path.IsPathRooted(text))
			{
				text = Path.Combine(DictionaryConfigurationHelper.directoryPath, text);
			}
			if (!File.Exists(text))
			{
				throw new FileNotFoundException("Dictionary Not Found: " + text);
			}
			return text;
		}

		private static void readThuatToanNhan()
		{
			string[] array = File.ReadAllLines(Path.Combine(DictionaryConfigurationHelper.directoryPath, "Dictionaries.config"));
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				if (!string.IsNullOrEmpty(text) && !text.StartsWith("#") && text.StartsWith("ThuatToanNhan="))
				{
					DictionaryConfigurationHelper.thuatToanNhan = text.Split(new char[]
					{
						'='
					})[1];
					return;
				}
			}
		}
	}
}
