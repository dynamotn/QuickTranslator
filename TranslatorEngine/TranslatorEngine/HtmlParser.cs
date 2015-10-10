using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace TranslatorEngine
{
	public class HtmlParser
	{
		private static string[] titleTags;

		private static string[] contentTags;

		private static string[] removedTags;

		private static bool dirty = true;

		private static string directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

		public static string GetChineseContent(string htmlContent, bool needMarkChapterHeaders)
		{
			HtmlParser.LoadConfiguration();
			StringBuilder stringBuilder = new StringBuilder();
			string[] array = HtmlParser.titleTags;
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				if (!string.IsNullOrEmpty(text) && !text.StartsWith("#") && htmlContent.ToLower().Contains(text.ToLower()))
				{
					string text2 = htmlContent.Substring(htmlContent.ToLower().IndexOf(text.ToLower()) + text.Length);
					string text3 = text.Substring(text.LastIndexOf('<') + 1);
					text3 = text3.Substring(0, text3.IndexOfAny(new char[]
					{
						' ',
						'>'
					}));
					if (text2.ToLower().Contains("</" + text3.ToLower() + ">"))
					{
						stringBuilder.AppendLine((needMarkChapterHeaders ? "$CHAPTER_HEADER$. " : "") + text2.Substring(0, text2.ToLower().IndexOf("</" + text3.ToLower() + ">")).TrimStart(" \u3000\t".ToCharArray()));
						break;
					}
				}
			}
			string[] array2 = HtmlParser.contentTags;
			for (int j = 0; j < array2.Length; j++)
			{
				string text4 = array2[j];
				if (!string.IsNullOrEmpty(text4) && !text4.StartsWith("#") && htmlContent.ToLower().Contains(text4.ToLower()))
				{
					string text5 = htmlContent.Substring(htmlContent.ToLower().IndexOf(text4.ToLower()) + text4.Length);
					if (text4 != "<!--bodybegin-->")
					{
						string text6 = text4.Substring(text4.LastIndexOf('<') + 1);
						text6 = text6.Substring(0, text6.IndexOfAny(new char[]
						{
							' ',
							'>'
						}));
						if (text5.ToLower().Contains("</" + text6.ToLower().TrimStart(new char[]
						{
							'/'
						}) + ">"))
						{
							stringBuilder.AppendLine(text5.Substring(0, text5.ToLower().IndexOf("</" + text6.ToLower().TrimStart(new char[]
							{
								'/'
							}) + ">")));
							break;
						}
					}
					else
					{
						string text6 = "<!--bodyend-->";
						if (text5.Contains(text6))
						{
							stringBuilder.AppendLine(text5.Substring(0, text5.ToLower().IndexOf(text6.ToLower())));
						}
					}
				}
			}
			string text7 = stringBuilder.ToString();
			string[] array3 = HtmlParser.removedTags;
			for (int k = 0; k < array3.Length; k++)
			{
				string text8 = array3[k];
				if (!string.IsNullOrEmpty(text8) && !text8.StartsWith("#"))
				{
					text7 = text7.Replace(text8, string.Empty);
				}
			}
			text7 = text7.Replace("<p>", "\n").Replace("</p>", "\n").Replace("<br>", "\n").Replace("<br/>", "\n").Replace("<br />", "\n").Replace("<BR>", "\n").Replace("<BR/>", "\n").Replace("<BR />", "\n").Replace("&nbsp;", "").Replace("&lt;", "").Replace("&gt;", "");
			return Regex.Replace(text7, "<(.|\\n)*?>", string.Empty);
		}

		private static void LoadConfiguration()
		{
			if (!HtmlParser.dirty)
			{
				return;
			}
			HtmlParser.titleTags = File.ReadAllLines(Path.Combine(HtmlParser.directoryPath, "HtmlChapterTitleTags.config"));
			HtmlParser.contentTags = File.ReadAllLines(Path.Combine(HtmlParser.directoryPath, "HtmlChapterContentTags.config"));
			HtmlParser.removedTags = File.ReadAllLines(Path.Combine(HtmlParser.directoryPath, "HtmlRemovedTags.config"));
			HtmlParser.dirty = false;
		}
	}
}
