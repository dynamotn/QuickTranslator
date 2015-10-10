using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace QuickTranslator
{
	public class ScrollingRichTextBox : RichTextBox
	{
		private const int WM_VSCROLL = 277;

		private const int SB_LINEUP = 0;

		private const int SB_LINEDOWN = 1;

		private const int SB_TOP = 6;

		private const int SB_BOTTOM = 7;

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		public void ScrollToBottom()
		{
			ScrollingRichTextBox.SendMessage(base.Handle, 277u, new IntPtr(7), new IntPtr(0));
		}

		public void ScrollToTop()
		{
			ScrollingRichTextBox.SendMessage(base.Handle, 277u, new IntPtr(6), new IntPtr(0));
		}

		public void ScrollLineDown()
		{
			ScrollingRichTextBox.SendMessage(base.Handle, 277u, new IntPtr(1), new IntPtr(0));
		}

		public void ScrollLineUp()
		{
			ScrollingRichTextBox.SendMessage(base.Handle, 277u, new IntPtr(0), new IntPtr(0));
		}
	}
}
