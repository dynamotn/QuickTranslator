using org.mozilla.intl.chardet;
using System;
using System.IO;

namespace TranslatorEngine
{
	public class CharsetDetector
	{
		public static string DetectedCharset;

		public static string DetectChineseCharset(string filePath)
		{
			CharsetDetector.DetectedCharset = "GB2312";
			nsDetector nsDetector = new nsDetector(3);
			Notifier aObserver = new Notifier();
			nsDetector.Init(aObserver);
			byte[] array = new byte[1024];
			int aLen = File.OpenRead(filePath).Read(array, 0, array.Length);
			bool flag = nsDetector.isAscii(array, aLen);
			if (!flag)
			{
				nsDetector.DoIt(array, aLen, false);
			}
			nsDetector.DataEnd();
			if (flag)
			{
				CharsetDetector.DetectedCharset = "ASCII";
			}
			if (File.ReadAllText(filePath).Contains("CONTENT=\"text/html; charset=gb2312\""))
			{
				CharsetDetector.DetectedCharset = "GB2312";
			}
			return CharsetDetector.DetectedCharset;
		}
	}
}
