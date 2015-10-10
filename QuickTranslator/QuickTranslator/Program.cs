using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using TranslatorEngine;

namespace QuickTranslator
{
	internal sealed class Program
	{
		[STAThread]
		private static void Main(string[] args)
		{
			Application.ThreadException += new ThreadExceptionEventHandler(Program.Application_ThreadException);
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Program.AppDomain_CurrentDomain_UnhandledException);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}

		private static void AppDomain_CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Program.UnhandledExceptionHandler((Exception)e.ExceptionObject);
		}

		private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			Program.UnhandledExceptionHandler(e.Exception);
		}

		private static void UnhandledExceptionHandler(Exception exception)
		{
			string text = "QuickTranslator";
			ApplicationLog.Log(Path.GetDirectoryName(Application.ExecutablePath), text, exception);
			MessageBox.Show("Lỗi chương trình! Hãy gửi " + text + ".log cho tác giả. Xin cám ơn!", "Lỗi chương trình", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			Application.Exit();
		}
	}
}
