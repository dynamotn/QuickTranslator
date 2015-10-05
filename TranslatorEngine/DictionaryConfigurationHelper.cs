using System;
using System.IO;
using System.Reflection;

namespace TranslatorEngine
{
	public class DictionaryConfigurationHelper
	{
		private static string directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

		public static string GetNamesDictionaryPath()
		{
			return DictionaryConfigurationHelper.GetDictionaryPathByKey("Names");
		}

		public static string GetNamesDictionaryHistoryPath()
		{
			return Path.Combine(Path.GetDirectoryName(DictionaryConfigurationHelper.GetNamesDictionaryPath()), Path.GetFileNameWithoutExtension(DictionaryConfigurationHelper.GetNamesDictionaryPath()) + "History" + Path.GetExtension(DictionaryConfigurationHelper.GetNamesDictionaryPath()));
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
	}
}
