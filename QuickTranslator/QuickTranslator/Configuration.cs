using System;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace QuickTranslator
{
	[Serializable]
	public class Configuration
	{
		private const int VERSION = 1;

		public bool BrowserPanel_DisableImages;

		public bool BrowserPanel_DisablePopups;

		public int VietPhrase_Wrap = 1;

		public int VietPhraseOneMeaning_Wrap = 1;

		public char HotKeys_CopyHighlightedHanViet = '0';

		public char HotKeys_CopyMeaning1 = '1';

		public char HotKeys_CopyMeaning2 = '2';

		public char HotKeys_CopyMeaning3 = '3';

		public char HotKeys_CopyMeaning4 = '4';

		public char HotKeys_CopyMeaning5 = '5';

		public char HotKeys_CopyMeaning6 = '6';

		public char HotKeys_MoveDownOneLine = 'M';

		public char HotKeys_MoveDownOneParagraph = 'N';

		public char HotKeys_MoveLeftOneWord = 'J';

		public char HotKeys_MoveRightOneWord = 'K';

		public char HotKeys_MoveUpOneLine = 'I';

		public char HotKeys_MoveUpOneParagraph = 'U';

		public string HotKeys_F1 = "";

		public string HotKeys_F2 = "";

		public string HotKeys_F3 = "";

		public string HotKeys_F4 = "";

		public string HotKeys_F5 = "";

		public string HotKeys_F6 = "";

		public string HotKeys_F7 = "";

		public string HotKeys_F8 = "";

		public string HotKeys_F9 = "";

		public bool Layout_VietPhrase = true;

		public bool Layout_VietPhraseOneMeaning = true;

		public Font Style_TrungFont = new Font("Arial Unicode MS", 14f);

		public Font Style_HanVietFont = new Font("Arial Unicode MS", 12f);

		public Font Style_VietPhraseFont = new Font("Arial Unicode MS", 12f);

		public Font Style_VietPhraseOneMeaningFont = new Font("Arial Unicode MS", 12f);

		public Font Style_VietFont = new Font("Arial Unicode MS", 12f);

		public Font Style_NghiaFont = new Font("Arial Unicode MS", 12f);

		public Color Style_TrungForeColor = default(Color);

		public Color Style_HanVietForeColor = default(Color);

		public Color Style_VietPhraseForeColor = default(Color);

		public Color Style_VietPhraseOneMeaningForeColor = default(Color);

		public Color Style_VietForeColor = default(Color);

		public Color Style_NghiaForeColor = default(Color);

		public Color Style_TrungBackColor = default(Color);

		public Color Style_HanVietBackColor = default(Color);

		public Color Style_VietPhraseBackColor = default(Color);

		public Color Style_VietPhraseOneMeaningBackColor = default(Color);

		public Color Style_VietBackColor = default(Color);

		public Color Style_NghiaBackColor = default(Color);

		public int TranslationAlgorithm;

		public bool PrioritizedName = true;

		public bool AlwaysFocusInViet = true;

		public static Configuration LoadFromFile(string configFilePath)
		{
			if (!File.Exists(configFilePath))
			{
				return new Configuration();
			}
			Stream stream = null;
			Configuration result = null;
			try
			{
				IFormatter formatter = new BinaryFormatter();
				stream = new FileStream(configFilePath, FileMode.Open, FileAccess.Read, FileShare.None);
				int arg_2E_0 = (int)formatter.Deserialize(stream);
				result = (Configuration)formatter.Deserialize(stream);
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
				}
			}
			return result;
		}

		public void SaveToFile(string configFilePath)
		{
			Stream stream = null;
			try
			{
				IFormatter formatter = new BinaryFormatter();
				stream = new FileStream(configFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
				formatter.Serialize(stream, 1);
				formatter.Serialize(stream, this);
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
				}
			}
		}
	}
}
