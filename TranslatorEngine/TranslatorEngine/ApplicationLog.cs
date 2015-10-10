using System;
using System.IO;
using System.Text;

namespace TranslatorEngine
{
	public class ApplicationLog
	{
		public static void Log(string applicationPath, string application, Exception exception)
		{
			try
			{
				string text = Path.Combine(applicationPath, application + ".log");
				FileInfo fileInfo = new FileInfo(text);
				if (fileInfo.Exists && 1000000L < fileInfo.Length)
				{
					fileInfo.Delete();
				}
				string contents = string.Format("{0:G}: {1}\r\n", DateTime.Now, string.Concat(new object[]
				{
					exception.Message,
					"\r\n",
					exception.GetType(),
					"\r\n",
					exception.StackTrace
				}));
				File.AppendAllText(text, contents, Encoding.UTF8);
			}
			catch
			{
			}
		}
	}
}
