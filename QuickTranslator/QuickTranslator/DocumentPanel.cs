using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace QuickTranslator
{
	public class DocumentPanel : DockContent
	{
		public delegate void ClickDelegate(int currentCharIndex);

		public delegate void RightClickDelegate(int currentCharIndex);

		public delegate void AddToVietPhraseDelegate(int type);

		public delegate void SelectTextDelegate();

		public delegate void ShiftAndMouseMoveDelegate(int charIndexUnderMouse);

		public delegate void ShiftUpDelegate();

		public delegate void BaikeingDelegate();

		public delegate void NcikuingDelegate();

		public delegate void CopyToVietDelegate(string textToCopy);

		public delegate void DeleteSelectedTextDelegate(bool remembered);

		public delegate void AddToPhienAmDelegate();

		private const int WM_KEYDOWN = 256;

		private const int WM_KEYUP = 257;

		private const int WM_SYSKEYDOWN = 260;

		private const int WM_SYSKEYUP = 261;

		public DocumentPanel.ClickDelegate ClickHandler;

		public DocumentPanel.RightClickDelegate RightClickHandler;

		public DocumentPanel.AddToVietPhraseDelegate AddToVietPhraseHandler;

		public DocumentPanel.SelectTextDelegate SelectTextHandler;

		public DocumentPanel.ShiftAndMouseMoveDelegate ShiftAndMouseMoveHandler;

		public DocumentPanel.ShiftUpDelegate ShiftUpHandler;

		public DocumentPanel.BaikeingDelegate BaikeingHandler;

		public DocumentPanel.NcikuingDelegate NcikuingHandler;

		public DocumentPanel.CopyToVietDelegate CopyToVietHandler;

		public DocumentPanel.DeleteSelectedTextDelegate DeleteSelectedTextHandler;

		public DocumentPanel.AddToPhienAmDelegate AddToPhienAmHandler;

		private int currentHighlightedTextStartIndex;

		private int currentHighlightedTextLength;

		private bool tabIsDown;

		private int lastCharIndexUnderMouseMove = -1;

		private IContainer components;

		private ToolStripMenuItem deleteRememberToolStripMenuItem;

		private ToolStripMenuItem updateNamePhuToolStripMenuItem;

		public ContextMenuStrip chooseMeaningContextMenuStrip;

		private ToolStripMenuItem ncikuToolStripMenuItem;

		private ToolStripMenuItem updatePhienAmToolStripMenuItem;

		private ToolStripSeparator toolStripSeparator3;

		private ToolStripSeparator toolStripSeparator2;

		private ToolStripMenuItem copyToClipboardToolStripMenuItem;

		private ToolStripMenuItem deleteSelectedTextToolStripMenuItem;

		private ToolStripMenuItem insertBlankLinesToolStripMenuItem;

		private ToolStripSeparator toolStripSeparator1;

		private ToolStripMenuItem copyToVietToolStripMenuItem;

		private ToolStripMenuItem baikeingToolStripMenuItem;

		private ToolStripMenuItem addToNameToolStripMenuItem;

		private ToolStripMenuItem addToVietPhraseToolStripMenuItem;

		private ContextMenuStrip addToVietPhraseContextMenuStrip;

		private ContextMenuStrip commentContextMenuStrip;

		private ToolStripMenuItem clearNoteToolStripMenuItem;

		private ToolStripMenuItem markForReviewToolStripMenuItem;

		public ScrollingRichTextBox contentRichTextBox;

		public int CurrentHighlightedTextStartIndex
		{
			get
			{
				return this.currentHighlightedTextStartIndex;
			}
			set
			{
				this.currentHighlightedTextStartIndex = value;
			}
		}

		public int CurrentHighlightedTextLength
		{
			get
			{
				return this.currentHighlightedTextLength;
			}
			set
			{
				this.currentHighlightedTextLength = value;
			}
		}

		public bool TextReadOnly
		{
			get
			{
				return this.contentRichTextBox.ReadOnly;
			}
			set
			{
				this.contentRichTextBox.ReadOnly = value;
			}
		}

		public DocumentPanel()
		{
			this.InitializeComponent();
			base.DockHandler.GetPersistStringCallback = new GetPersistStringCallback(this.GetPersistStringFromText);
		}

		public DocumentPanel(bool textReadOnly)
		{
			this.InitializeComponent();
			this.contentRichTextBox.ReadOnly = textReadOnly;
			base.DockHandler.GetPersistStringCallback = new GetPersistStringCallback(this.GetPersistStringFromText);
		}

		public string GetPersistStringFromText()
		{
			return this.Text;
		}

		public string GetSelectedText()
		{
			return this.contentRichTextBox.SelectedText;
		}

		public string GetTextContent()
		{
			return this.contentRichTextBox.Text;
		}

		public void SetTextContent(string text)
		{
			this.contentRichTextBox.Text = text;
		}

		public string GetRtfContent()
		{
			return this.contentRichTextBox.Rtf;
		}

		public void SetRftContent(string rft)
		{
			this.contentRichTextBox.Rtf = rft;
		}

		public void AppendText(string text)
		{
			this.contentRichTextBox.AppendText(text);
		}

		public void AppendTextToCurrentCursor(string text)
		{
			this.contentRichTextBox.SelectedText = text;
		}

		public string GetTwoCharsBeforeSelectedText()
		{
			if (this.contentRichTextBox.SelectionStart == 0)
			{
				return "";
			}
			if (this.contentRichTextBox.SelectionStart == 1)
			{
				return this.contentRichTextBox.Text[0].ToString();
			}
			return this.contentRichTextBox.Text.Substring(this.contentRichTextBox.SelectionStart - 2, 2);
		}

		public string GetTwoCharsAfterSelectedText()
		{
			int num = this.contentRichTextBox.SelectionStart;
			if (1 < this.contentRichTextBox.SelectionLength)
			{
				num += this.contentRichTextBox.SelectionLength - 1;
			}
			if (this.contentRichTextBox.TextLength - 1 <= num)
			{
				return "";
			}
			if (this.contentRichTextBox.TextLength - 2 == num)
			{
				return this.contentRichTextBox.Text[num + 1].ToString();
			}
			return this.contentRichTextBox.Text.Substring(num + 1, 2);
		}

		public int getPreviousWordIndex()
		{
			int num = this.contentRichTextBox.SelectionStart;
			bool flag = false;
			while (1 <= num)
			{
				num--;
				if (this.contentRichTextBox.Text[num] == ' ' || this.contentRichTextBox.Text[num] == '.' || this.contentRichTextBox.Text[num] == ',' || this.contentRichTextBox.Text[num] == ':' || this.contentRichTextBox.Text[num] == ';' || this.contentRichTextBox.Text[num] == '?' || this.contentRichTextBox.Text[num] == '!' || this.contentRichTextBox.Text[num] == '\'' || this.contentRichTextBox.Text[num] == '\n' || this.contentRichTextBox.Text[num] == '"')
				{
					flag = true;
				}
				else if (flag)
				{
					break;
				}
			}
			return num;
		}

		public int getNextWordIndex()
		{
			int i = this.contentRichTextBox.SelectionStart;
			bool flag = false;
			while (i <= this.contentRichTextBox.TextLength - 2)
			{
				i++;
				if (this.contentRichTextBox.Text[i] == ' ' || this.contentRichTextBox.Text[i] == '.' || this.contentRichTextBox.Text[i] == ',' || this.contentRichTextBox.Text[i] == ':' || this.contentRichTextBox.Text[i] == ';' || this.contentRichTextBox.Text[i] == '?' || this.contentRichTextBox.Text[i] == '!' || this.contentRichTextBox.Text[i] == '\'' || this.contentRichTextBox.Text[i] == '\t' || this.contentRichTextBox.Text[i] == '\n' || this.contentRichTextBox.Text[i] == '"')
				{
					flag = true;
				}
				else if (flag)
				{
					break;
				}
			}
			return i;
		}

		public int getNextLineIndex()
		{
			int lineFromCharIndex = this.contentRichTextBox.GetLineFromCharIndex(this.contentRichTextBox.SelectionStart);
			int lineFromCharIndex2 = this.contentRichTextBox.GetLineFromCharIndex(this.contentRichTextBox.TextLength - 1);
			int num = this.contentRichTextBox.GetFirstCharIndexFromLine((lineFromCharIndex <= lineFromCharIndex2 - 1) ? (lineFromCharIndex + 1) : lineFromCharIndex);
			while (this.contentRichTextBox.Text[num] == ' ' || this.contentRichTextBox.Text[num] == '.' || this.contentRichTextBox.Text[num] == ',' || this.contentRichTextBox.Text[num] == ':' || this.contentRichTextBox.Text[num] == ';' || this.contentRichTextBox.Text[num] == '?' || this.contentRichTextBox.Text[num] == '!' || this.contentRichTextBox.Text[num] == '\'' || this.contentRichTextBox.Text[num] == '\t' || this.contentRichTextBox.Text[num] == '\n' || this.contentRichTextBox.Text[num] == '"')
			{
				num++;
				if (num == this.contentRichTextBox.TextLength)
				{
					break;
				}
			}
			return num;
		}

		public int getPreviousLineIndex()
		{
			int num = this.contentRichTextBox.GetLineFromCharIndex(this.contentRichTextBox.SelectionStart);
			int num2 = this.contentRichTextBox.GetFirstCharIndexFromLine((num <= 0) ? 0 : (num - 1));
			while (this.contentRichTextBox.Text[num2] == '\n')
			{
				num--;
				num2 = this.contentRichTextBox.GetFirstCharIndexFromLine((num <= 0) ? 0 : (num - 1));
				if (num2 <= 0)
				{
					IL_79:
					while (this.contentRichTextBox.Text[num2] == ' ' || this.contentRichTextBox.Text[num2] == '.' || this.contentRichTextBox.Text[num2] == ',' || this.contentRichTextBox.Text[num2] == ':' || this.contentRichTextBox.Text[num2] == ';' || this.contentRichTextBox.Text[num2] == '?' || this.contentRichTextBox.Text[num2] == '!' || this.contentRichTextBox.Text[num2] == '\'' || this.contentRichTextBox.Text[num2] == '\t' || this.contentRichTextBox.Text[num2] == '\n' || this.contentRichTextBox.Text[num2] == '"')
					{
						num2++;
						if (this.contentRichTextBox.TextLength <= num2)
						{
							break;
						}
					}
					return num2;
				}
			}
//			goto IL_79;
			while (this.contentRichTextBox.Text[num2] == ' ' || this.contentRichTextBox.Text[num2] == '.' || this.contentRichTextBox.Text[num2] == ',' || this.contentRichTextBox.Text[num2] == ':' || this.contentRichTextBox.Text[num2] == ';' || this.contentRichTextBox.Text[num2] == '?' || this.contentRichTextBox.Text[num2] == '!' || this.contentRichTextBox.Text[num2] == '\'' || this.contentRichTextBox.Text[num2] == '\t' || this.contentRichTextBox.Text[num2] == '\n' || this.contentRichTextBox.Text[num2] == '"')
			{
				num2++;
				if (this.contentRichTextBox.TextLength <= num2)
				{
					break;
				}
			}
			return num2;
		}

		public int getCurrentLineIndex()
		{
			return this.contentRichTextBox.GetLineFromCharIndex(this.contentRichTextBox.SelectionStart);
		}

		public int getNextPhysicalLineIndex()
		{
			int num = this.contentRichTextBox.SelectionStart;
			while (this.contentRichTextBox.Text[num] != '\n')
			{
				num++;
				if (this.contentRichTextBox.TextLength - 1 <= num)
				{
					IL_4E:
					while (this.contentRichTextBox.Text[num] == ' ' || this.contentRichTextBox.Text[num] == '.' || this.contentRichTextBox.Text[num] == ',' || this.contentRichTextBox.Text[num] == ':' || this.contentRichTextBox.Text[num] == ';' || this.contentRichTextBox.Text[num] == '?' || this.contentRichTextBox.Text[num] == '!' || this.contentRichTextBox.Text[num] == '\'' || this.contentRichTextBox.Text[num] == '\t' || this.contentRichTextBox.Text[num] == '\n' || this.contentRichTextBox.Text[num] == '"')
					{
						num++;
						if (num == this.contentRichTextBox.TextLength)
						{
							break;
						}
					}
					return num;
				}
			}
//			goto IL_4E;
			while (this.contentRichTextBox.Text[num] == ' ' || this.contentRichTextBox.Text[num] == '.' || this.contentRichTextBox.Text[num] == ',' || this.contentRichTextBox.Text[num] == ':' || this.contentRichTextBox.Text[num] == ';' || this.contentRichTextBox.Text[num] == '?' || this.contentRichTextBox.Text[num] == '!' || this.contentRichTextBox.Text[num] == '\'' || this.contentRichTextBox.Text[num] == '\t' || this.contentRichTextBox.Text[num] == '\n' || this.contentRichTextBox.Text[num] == '"')
			{
				num++;
				if (num == this.contentRichTextBox.TextLength)
				{
					break;
				}
			}
			return num;
		}

		public int getPreviousPhysicalLineIndex()
		{
			int num = this.contentRichTextBox.SelectionStart;
			bool flag = false;
			while (true)
			{
				num--;
				if (num == 0)
				{
					break;
				}
				if (this.contentRichTextBox.Text[num] == '\n' && this.contentRichTextBox.Text[num - 1] == '\n')
				{
					num--;
					if (num <= 0 || flag)
					{
						break;
					}
					flag = true;
				}
			}
			while (this.contentRichTextBox.Text[num] == ' ' || this.contentRichTextBox.Text[num] == '.' || this.contentRichTextBox.Text[num] == ',' || this.contentRichTextBox.Text[num] == ':' || this.contentRichTextBox.Text[num] == ';' || this.contentRichTextBox.Text[num] == '?' || this.contentRichTextBox.Text[num] == '!' || this.contentRichTextBox.Text[num] == '\'' || this.contentRichTextBox.Text[num] == '\t' || this.contentRichTextBox.Text[num] == '\n' || this.contentRichTextBox.Text[num] == '"')
			{
				num++;
				if (num == this.contentRichTextBox.TextLength)
				{
					break;
				}
			}
			return num;
		}

		public string GetHighlightText()
		{
			return this.contentRichTextBox.Text.Substring(this.currentHighlightedTextStartIndex, this.currentHighlightedTextLength);
		}

		public void HighlightText(int startIndex, int length)
		{
			this.HighlightText(startIndex, length, false, true);
		}

		public void HighlightText(int startIndex, int length, bool clearTextSelection, bool scrollToCaret)
		{
			if (base.IsHidden)
			{
				return;
			}
			base.SuspendLayout();
			this.contentRichTextBox.Select(this.currentHighlightedTextStartIndex, this.currentHighlightedTextLength);
			this.contentRichTextBox.SelectionColor = this.contentRichTextBox.ForeColor;
			this.contentRichTextBox.Select(startIndex, length);
			this.contentRichTextBox.SelectionColor = Color.Red;
			this.currentHighlightedTextStartIndex = startIndex;
			this.currentHighlightedTextLength = length;
			if (clearTextSelection)
			{
				this.contentRichTextBox.Select(startIndex, 0);
			}
			if (this.contentRichTextBox.Height <= this.contentRichTextBox.GetPositionFromCharIndex(this.currentHighlightedTextStartIndex).Y + 50 || this.contentRichTextBox.GetPositionFromCharIndex(this.currentHighlightedTextStartIndex).Y <= 30)
			{
				this.contentRichTextBox.ScrollToCaret();
				this.contentRichTextBox.ScrollLineUp();
				this.contentRichTextBox.ScrollLineUp();
			}
			base.ResumeLayout();
		}

		public void SelectText(int startIndex, int length)
		{
			base.SuspendLayout();
			this.contentRichTextBox.Select(startIndex, length);
			if (this.contentRichTextBox.Height <= this.contentRichTextBox.GetPositionFromCharIndex(this.currentHighlightedTextStartIndex).Y + 50 || this.contentRichTextBox.GetPositionFromCharIndex(this.currentHighlightedTextStartIndex).Y <= 50)
			{
				this.contentRichTextBox.ScrollToCaret();
				this.contentRichTextBox.ScrollLineUp();
				this.contentRichTextBox.ScrollLineUp();
			}
			base.ResumeLayout();
		}

		public int getSelectionStart()
		{
			return this.contentRichTextBox.SelectionStart;
		}

		public int getSelectionLength()
		{
			return this.contentRichTextBox.SelectionLength;
		}

		public void SetFontSize(float size)
		{
			this.contentRichTextBox.SuspendLayout();
			this.contentRichTextBox.Font = new Font(this.contentRichTextBox.Font.FontFamily, size);
			this.contentRichTextBox.ResumeLayout();
		}

		public void ScrollToTop()
		{
			this.contentRichTextBox.SelectionStart = 0;
			this.contentRichTextBox.ScrollToCaret();
		}

		public void ScrollToBottom()
		{
			this.contentRichTextBox.SelectionStart = this.contentRichTextBox.TextLength;
			this.contentRichTextBox.ScrollToCaret();
		}

		public void FocusInRichTextBox()
		{
			this.contentRichTextBox.Focus();
		}

		private void ContentRichTextBoxMouseUp(object sender, MouseEventArgs e)
		{
			int num = this.contentRichTextBox.GetCharIndexFromPosition(e.Location);
			char c = this.contentRichTextBox.GetCharFromPosition(e.Location);
			int num2 = 1000;
			int num3 = 0;
			while (0 <= num)
			{
				if (num2 <= num3)
				{
					return;
				}
				if (0 < num && c == ' ')
				{
					num--;
					c = this.contentRichTextBox.Text[num];
				}
				else
				{
					if (num > this.contentRichTextBox.TextLength - 1 || c != '\t')
					{
						break;
					}
					num++;
					c = this.contentRichTextBox.Text[num];
				}
				num3++;
			}
			if (0 < this.contentRichTextBox.SelectionLength)
			{
				if (this.SelectTextHandler != null)
				{
					this.SelectTextHandler();
				}
				return;
			}
			if (e.Button == MouseButtons.Left)
			{
				if (this.ClickHandler != null)
				{
					this.ClickHandler(num);
					return;
				}
			}
			else if (this.RightClickHandler != null)
			{
				this.RightClickHandler(num);
			}
		}

		public void ScrollToASpecificLogicalLine(int logicalLine)
		{
			this.contentRichTextBox.SelectionStart = Math.Max(this.contentRichTextBox.GetFirstCharIndexFromLine(logicalLine), 0);
			this.contentRichTextBox.SelectionLength = 0;
			this.contentRichTextBox.ScrollToCaret();
		}

		public void ScrollToASpecificPhysicalLine(int physicalLine)
		{
			int num = -1;
			string text = this.contentRichTextBox.Text;
			int num2 = 0;
			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] == '\n')
				{
					num2++;
					num = i;
					if (physicalLine - 1 <= num2)
					{
						num = i + 1;
						break;
					}
				}
			}
			if (this.contentRichTextBox.TextLength - 1 < num)
			{
				num = ((this.contentRichTextBox.TextLength == 0) ? 0 : (this.contentRichTextBox.TextLength - 1));
			}
			this.contentRichTextBox.SelectionStart = num;
			this.contentRichTextBox.SelectionLength = 0;
			this.contentRichTextBox.ScrollToCaret();
		}

		private void MarkForReviewToolStripMenuItemClick(object sender, EventArgs e)
		{
			this.contentRichTextBox.SelectionColor = Color.Red;
		}

		public void EnableCommentContextMenuStrip()
		{
			this.contentRichTextBox.ContextMenuStrip = this.commentContextMenuStrip;
		}

		public void EnableAddToVietPhraseContextMenuStrip()
		{
			this.contentRichTextBox.ContextMenuStrip = this.addToVietPhraseContextMenuStrip;
		}

		private void ClearNoteToolStripMenuItemClick(object sender, EventArgs e)
		{
			this.contentRichTextBox.SelectionColor = Color.Black;
		}

		private void AddToVietPhraseToolStripMenuItemClick(object sender, EventArgs e)
		{
			if (this.AddToVietPhraseHandler != null)
			{
				this.AddToVietPhraseHandler(0);
			}
		}

		private void AddToNameToolStripMenuItemClick(object sender, EventArgs e)
		{
			if (this.AddToVietPhraseHandler != null)
			{
				this.AddToVietPhraseHandler(1);
			}
		}

		private void AddToVietPhraseContextMenuStripOpening(object sender, CancelEventArgs e)
		{
			if (this.contentRichTextBox.SelectionLength == 0)
			{
				e.Cancel = true;
			}
		}

		private void BaikeingToolStripMenuItemClick(object sender, EventArgs e)
		{
			if (this.BaikeingHandler != null)
			{
				this.BaikeingHandler();
			}
		}

		private void CopyToVietToolStripMenuItemClick(object sender, EventArgs e)
		{
			if (this.CopyToVietHandler != null)
			{
				this.CopyToVietHandler(this.contentRichTextBox.SelectedText);
			}
		}

		private void contentRichTextBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (this.contentRichTextBox.ReadOnly)
			{
				return;
			}
			if (('A' <= e.KeyChar && e.KeyChar <= 'Z') || ('a' <= e.KeyChar && e.KeyChar <= 'z'))
			{
				return;
			}
			int selectionStart = this.contentRichTextBox.SelectionStart;
			if (selectionStart == 0)
			{
				return;
			}
			int num = selectionStart - 1;
			while (0 <= num && this.contentRichTextBox.Text[num] != '\n' && this.contentRichTextBox.Text[num] != ' ' && this.contentRichTextBox.Text[num] != '\'' && this.contentRichTextBox.Text[num] != '[' && this.contentRichTextBox.Text[num] != ']' && this.contentRichTextBox.Text[num] != '(' && this.contentRichTextBox.Text[num] != ')' && this.contentRichTextBox.Text[num] != '*' && this.contentRichTextBox.Text[num] != '"')
			{
				num--;
			}
			num++;
			int num2 = selectionStart - 1;
			if (num2 <= num)
			{
				return;
			}
			string text = this.contentRichTextBox.Text.Substring(num, num2 - num + 1);
			string textToReplace = this.getTextToReplace(text);
			if (string.IsNullOrEmpty(textToReplace))
			{
				return;
			}
			this.contentRichTextBox.Select(num, num2 - num + 1);
			this.contentRichTextBox.SelectedText = textToReplace;
			this.contentRichTextBox.Select(selectionStart + textToReplace.Length - text.Length, 0);
		}

		private string getTextToReplace(string textToLookup)
		{
			return Shortcuts.GetValueFromKey(textToLookup);
		}

		private void InsertBlankLinesToolStripMenuItemClick(object sender, EventArgs e)
		{
			string text = this.contentRichTextBox.Rtf;
			string text2 = "";
			for (int i = 0; i < 100; i++)
			{
				text2 = text.Replace("\\par\r\n\\par\r\n", "\\par\r\n");
				if (text.Equals(text2))
				{
					break;
				}
				text = text2;
			}
			this.contentRichTextBox.Rtf = text2.Replace("\\par\r\n", "\\par\r\n\\par\r\n");
			this.ScrollToBottom();
		}

		public void SetBackColor(Color color)
		{
			this.contentRichTextBox.BackColor = color;
		}

		public void SetForeColor(Color color)
		{
			this.contentRichTextBox.ForeColor = color;
		}

		public void SetFont(Font font)
		{
			this.contentRichTextBox.Font = font;
		}

		public void AllowDeletingSelectedText()
		{
			this.deleteSelectedTextToolStripMenuItem.Enabled = true;
			this.deleteSelectedTextToolStripMenuItem.Visible = true;
			this.deleteRememberToolStripMenuItem.Enabled = true;
			this.deleteRememberToolStripMenuItem.Visible = true;
		}

		private void DeleteSelectedTextToolStripMenuItemClick(object sender, EventArgs e)
		{
			if (this.DeleteSelectedTextHandler != null)
			{
				this.DeleteSelectedTextHandler(false);
			}
		}

		public void DeleteSelectedText()
		{
			bool readOnly = this.contentRichTextBox.ReadOnly;
			this.contentRichTextBox.ReadOnly = false;
			this.contentRichTextBox.SelectedText = "";
			this.contentRichTextBox.ReadOnly = readOnly;
		}

		private void CopyToClipboardToolStripMenuItemClick(object sender, EventArgs e)
		{
			try
			{
				Clipboard.SetText(string.IsNullOrEmpty(this.contentRichTextBox.SelectedText) ? string.Empty : this.contentRichTextBox.SelectedText);
			}
			catch
			{
			}
		}

		private void UpdatePhienAmToolStripMenuItemClick(object sender, EventArgs e)
		{
			if (this.AddToPhienAmHandler != null)
			{
				this.AddToPhienAmHandler();
			}
		}

		private void NcikuToolStripMenuItemClick(object sender, EventArgs e)
		{
			if (this.NcikuingHandler != null)
			{
				this.NcikuingHandler();
			}
		}

		private void DocumentPanelKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control)
			{
				switch (e.KeyData)
				{
				case Keys.F:
					e.Handled = true;
					return;
				case Keys.G:
					e.Handled = true;
					break;
				default:
					return;
				}
			}
		}

		public void ReplaceHighlightedText(string text)
		{
			this.contentRichTextBox.SelectionStart = this.currentHighlightedTextStartIndex;
			this.contentRichTextBox.SelectionLength = this.currentHighlightedTextLength;
			this.contentRichTextBox.SelectedText = text;
			this.currentHighlightedTextLength = text.Length;
		}

		private void UpdateNamePhuToolStripMenuItemClick(object sender, EventArgs e)
		{
			if (this.AddToVietPhraseHandler != null)
			{
				this.AddToVietPhraseHandler(2);
			}
		}

		protected override bool ProcessKeyPreview(ref Message message)
		{
			if (!"VietPhrase một nghĩa".Equals(this.Text))
			{
				return base.ProcessKeyPreview(ref message);
			}
			Keys keys = (Keys)((int)message.WParam);
			if (message.Msg == 256 && keys == Keys.Tab)
			{
				if (this.tabIsDown)
				{
					return base.ProcessKeyPreview(ref message);
				}
				if (this.currentHighlightedTextStartIndex < 0 || this.currentHighlightedTextLength <= 0 || this.contentRichTextBox.Text[this.currentHighlightedTextStartIndex] == '\n')
				{
					return base.ProcessKeyPreview(ref message);
				}
				this.contentRichTextBox.Cursor = Cursors.SizeAll;
				this.tabIsDown = true;
			}
			else if (message.Msg == 257 && keys == Keys.Tab)
			{
				if (this.tabIsDown)
				{
					this.tabIsDown = false;
					if (this.ShiftUpHandler != null && this.contentRichTextBox.SelectionLength == 0)
					{
						this.ShiftUpHandler();
					}
				}
				this.contentRichTextBox.Cursor = Cursors.IBeam;
				this.lastCharIndexUnderMouseMove = -1;
			}
			return base.ProcessKeyPreview(ref message);
		}

		private void ContentRichTextBoxMouseMove(object sender, MouseEventArgs e)
		{
			if (this.tabIsDown)
			{
				int charIndexFromPosition = this.contentRichTextBox.GetCharIndexFromPosition(e.Location);
				if (this.lastCharIndexUnderMouseMove == charIndexFromPosition)
				{
					return;
				}
				this.lastCharIndexUnderMouseMove = charIndexFromPosition;
				if (this.currentHighlightedTextStartIndex <= charIndexFromPosition && charIndexFromPosition <= this.currentHighlightedTextStartIndex + this.currentHighlightedTextLength - 1)
				{
					this.contentRichTextBox.SelectionStart = this.currentHighlightedTextStartIndex;
					this.contentRichTextBox.SelectionLength = 0;
					return;
				}
				if (charIndexFromPosition < this.GetCurrentBlockStartIndex() || this.GetCurrentBlockEndIndex() < charIndexFromPosition)
				{
					this.contentRichTextBox.SelectionStart = this.currentHighlightedTextStartIndex;
					this.contentRichTextBox.SelectionLength = 0;
					return;
				}
				if (this.ShiftAndMouseMoveHandler != null)
				{
					this.ShiftAndMouseMoveHandler(charIndexFromPosition);
				}
			}
		}

		public int GetCurrentBlockStartIndex()
		{
			int num = this.currentHighlightedTextStartIndex;
			while (num != 0 && this.contentRichTextBox.Text[num - 1] != '\t' && this.contentRichTextBox.Text[num - 1] != '\n' && (this.contentRichTextBox.Text[num - 1] != ' ' || !".,:;!?\"".Contains(this.contentRichTextBox.Text[num - 2].ToString())))
			{
				num--;
			}
			return num;
		}

		public int GetCurrentBlockEndIndex()
		{
			int num = this.currentHighlightedTextStartIndex + this.currentHighlightedTextLength - 1;
			int num2 = this.contentRichTextBox.Text.Length - 1;
			while (num2 != num && !"\n\t.,:;!?".Contains(this.contentRichTextBox.Text[num].ToString()))
			{
				num++;
			}
			return num;
		}

		private void DeleteRememberToolStripMenuItemClick(object sender, EventArgs e)
		{
			if (this.DeleteSelectedTextHandler != null)
			{
				this.DeleteSelectedTextHandler(true);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(DocumentPanel));
			this.commentContextMenuStrip = new ContextMenuStrip(this.components);
			this.markForReviewToolStripMenuItem = new ToolStripMenuItem();
			this.clearNoteToolStripMenuItem = new ToolStripMenuItem();
			this.toolStripSeparator1 = new ToolStripSeparator();
			this.insertBlankLinesToolStripMenuItem = new ToolStripMenuItem();
			this.addToVietPhraseContextMenuStrip = new ContextMenuStrip(this.components);
			this.addToVietPhraseToolStripMenuItem = new ToolStripMenuItem();
			this.addToNameToolStripMenuItem = new ToolStripMenuItem();
			this.updateNamePhuToolStripMenuItem = new ToolStripMenuItem();
			this.updatePhienAmToolStripMenuItem = new ToolStripMenuItem();
			this.toolStripSeparator2 = new ToolStripSeparator();
			this.baikeingToolStripMenuItem = new ToolStripMenuItem();
			this.ncikuToolStripMenuItem = new ToolStripMenuItem();
			this.toolStripSeparator3 = new ToolStripSeparator();
			this.copyToVietToolStripMenuItem = new ToolStripMenuItem();
			this.copyToClipboardToolStripMenuItem = new ToolStripMenuItem();
			this.deleteSelectedTextToolStripMenuItem = new ToolStripMenuItem();
			this.deleteRememberToolStripMenuItem = new ToolStripMenuItem();
			this.contentRichTextBox = new ScrollingRichTextBox();
			this.chooseMeaningContextMenuStrip = new ContextMenuStrip(this.components);
			this.commentContextMenuStrip.SuspendLayout();
			this.addToVietPhraseContextMenuStrip.SuspendLayout();
			base.SuspendLayout();
			this.commentContextMenuStrip.Items.AddRange(new ToolStripItem[]
			{
				this.markForReviewToolStripMenuItem,
				this.clearNoteToolStripMenuItem,
				this.toolStripSeparator1,
				this.insertBlankLinesToolStripMenuItem
			});
			this.commentContextMenuStrip.Name = "contextMenuStrip";
			this.commentContextMenuStrip.Size = new Size(186, 76);
			this.markForReviewToolStripMenuItem.Name = "markForReviewToolStripMenuItem";
			this.markForReviewToolStripMenuItem.Overflow = ToolStripItemOverflow.AsNeeded;
			this.markForReviewToolStripMenuItem.Size = new Size(185, 22);
			this.markForReviewToolStripMenuItem.Text = "Chú ý cho biên dịch";
			this.markForReviewToolStripMenuItem.Click += new EventHandler(this.MarkForReviewToolStripMenuItemClick);
			this.clearNoteToolStripMenuItem.Name = "clearNoteToolStripMenuItem";
			this.clearNoteToolStripMenuItem.Size = new Size(185, 22);
			this.clearNoteToolStripMenuItem.Text = "Bỏ chú ý";
			this.clearNoteToolStripMenuItem.Click += new EventHandler(this.ClearNoteToolStripMenuItemClick);
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new Size(182, 6);
			this.insertBlankLinesToolStripMenuItem.Name = "insertBlankLinesToolStripMenuItem";
			this.insertBlankLinesToolStripMenuItem.Size = new Size(185, 22);
			this.insertBlankLinesToolStripMenuItem.Text = "Chèn các dòng trắng";
			this.insertBlankLinesToolStripMenuItem.Click += new EventHandler(this.InsertBlankLinesToolStripMenuItemClick);
			this.addToVietPhraseContextMenuStrip.Items.AddRange(new ToolStripItem[]
			{
				this.addToVietPhraseToolStripMenuItem,
				this.addToNameToolStripMenuItem,
				this.updateNamePhuToolStripMenuItem,
				this.updatePhienAmToolStripMenuItem,
				this.toolStripSeparator2,
				this.baikeingToolStripMenuItem,
				this.ncikuToolStripMenuItem,
				this.toolStripSeparator3,
				this.copyToVietToolStripMenuItem,
				this.copyToClipboardToolStripMenuItem,
				this.deleteSelectedTextToolStripMenuItem,
				this.deleteRememberToolStripMenuItem
			});
			this.addToVietPhraseContextMenuStrip.Name = "addToVietPhraseContextMenuStrip";
			this.addToVietPhraseContextMenuStrip.Size = new Size(241, 258);
			this.addToVietPhraseContextMenuStrip.Opening += new CancelEventHandler(this.AddToVietPhraseContextMenuStripOpening);
			this.addToVietPhraseToolStripMenuItem.Name = "addToVietPhraseToolStripMenuItem";
			this.addToVietPhraseToolStripMenuItem.ShortcutKeys = (Keys)262230;
			this.addToVietPhraseToolStripMenuItem.Size = new Size(240, 22);
			this.addToVietPhraseToolStripMenuItem.Text = "Update &VietPhrase";
			this.addToVietPhraseToolStripMenuItem.Click += new EventHandler(this.AddToVietPhraseToolStripMenuItemClick);
			this.addToNameToolStripMenuItem.Name = "addToNameToolStripMenuItem";
			this.addToNameToolStripMenuItem.ShortcutKeys = (Keys)262222;
			this.addToNameToolStripMenuItem.Size = new Size(240, 22);
			this.addToNameToolStripMenuItem.Text = "Update &Name (chính)";
			this.addToNameToolStripMenuItem.Click += new EventHandler(this.AddToNameToolStripMenuItemClick);
			this.updateNamePhuToolStripMenuItem.Name = "updateNamePhuToolStripMenuItem";
			this.updateNamePhuToolStripMenuItem.ShortcutKeys = (Keys)262221;
			this.updateNamePhuToolStripMenuItem.Size = new Size(240, 22);
			this.updateNamePhuToolStripMenuItem.Text = "Update Name (phụ)";
			this.updateNamePhuToolStripMenuItem.Click += new EventHandler(this.UpdateNamePhuToolStripMenuItemClick);
			this.updatePhienAmToolStripMenuItem.Name = "updatePhienAmToolStripMenuItem";
			this.updatePhienAmToolStripMenuItem.ShortcutKeys = (Keys)262224;
			this.updatePhienAmToolStripMenuItem.Size = new Size(240, 22);
			this.updatePhienAmToolStripMenuItem.Text = "Update &Phiên Âm";
			this.updatePhienAmToolStripMenuItem.Click += new EventHandler(this.UpdatePhienAmToolStripMenuItemClick);
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new Size(237, 6);
			this.baikeingToolStripMenuItem.Name = "baikeingToolStripMenuItem";
			this.baikeingToolStripMenuItem.ShortcutKeys = (Keys)262210;
			this.baikeingToolStripMenuItem.Size = new Size(240, 22);
			this.baikeingToolStripMenuItem.Text = "&Baike-ing";
			this.baikeingToolStripMenuItem.Click += new EventHandler(this.BaikeingToolStripMenuItemClick);
			this.ncikuToolStripMenuItem.Name = "ncikuToolStripMenuItem";
			this.ncikuToolStripMenuItem.ShortcutKeys = (Keys)262229;
			this.ncikuToolStripMenuItem.Size = new Size(240, 22);
			this.ncikuToolStripMenuItem.Text = "Ncik&u-ing";
			this.ncikuToolStripMenuItem.Click += new EventHandler(this.NcikuToolStripMenuItemClick);
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new Size(237, 6);
			this.copyToVietToolStripMenuItem.Name = "copyToVietToolStripMenuItem";
			this.copyToVietToolStripMenuItem.ShortcutKeys = (Keys)262211;
			this.copyToVietToolStripMenuItem.Size = new Size(240, 22);
			this.copyToVietToolStripMenuItem.Text = "&Copy To Việt";
			this.copyToVietToolStripMenuItem.Click += new EventHandler(this.CopyToVietToolStripMenuItemClick);
			this.copyToClipboardToolStripMenuItem.Name = "copyToClipboardToolStripMenuItem";
			this.copyToClipboardToolStripMenuItem.ShortcutKeys = (Keys)131139;
			this.copyToClipboardToolStripMenuItem.Size = new Size(240, 22);
			this.copyToClipboardToolStripMenuItem.Text = "Copy To Clipboard";
			this.copyToClipboardToolStripMenuItem.Click += new EventHandler(this.CopyToClipboardToolStripMenuItemClick);
			this.deleteSelectedTextToolStripMenuItem.Enabled = false;
			this.deleteSelectedTextToolStripMenuItem.Name = "deleteSelectedTextToolStripMenuItem";
			this.deleteSelectedTextToolStripMenuItem.ShortcutKeys = (Keys)262232;
			this.deleteSelectedTextToolStripMenuItem.Size = new Size(240, 22);
			this.deleteSelectedTextToolStripMenuItem.Text = "&Delete Selected Text";
			this.deleteSelectedTextToolStripMenuItem.Visible = false;
			this.deleteSelectedTextToolStripMenuItem.Click += new EventHandler(this.DeleteSelectedTextToolStripMenuItemClick);
			this.deleteRememberToolStripMenuItem.Enabled = false;
			this.deleteRememberToolStripMenuItem.Name = "deleteRememberToolStripMenuItem";
			this.deleteRememberToolStripMenuItem.ShortcutKeys = (Keys)393304;
			this.deleteRememberToolStripMenuItem.Size = new Size(240, 22);
			this.deleteRememberToolStripMenuItem.Text = "Delete (Remember)";
			this.deleteRememberToolStripMenuItem.Visible = false;
			this.deleteRememberToolStripMenuItem.Click += new EventHandler(this.DeleteRememberToolStripMenuItemClick);
			this.contentRichTextBox.BorderStyle = BorderStyle.FixedSingle;
			this.contentRichTextBox.Dock = DockStyle.Fill;
			this.contentRichTextBox.Font = new Font("Arial Unicode MS", 14.25f, FontStyle.Regular, GraphicsUnit.Point, 163);
			this.contentRichTextBox.HideSelection = false;
			this.contentRichTextBox.Location = new Point(0, 0);
			this.contentRichTextBox.Name = "contentRichTextBox";
			this.contentRichTextBox.ScrollBars = RichTextBoxScrollBars.ForcedVertical;
			this.contentRichTextBox.ShowSelectionMargin = true;
			this.contentRichTextBox.Size = new Size(434, 365);
			this.contentRichTextBox.TabIndex = 0;
			this.contentRichTextBox.Text = "";
			this.contentRichTextBox.KeyPress += new KeyPressEventHandler(this.contentRichTextBox_KeyPress);
			this.contentRichTextBox.MouseMove += new MouseEventHandler(this.ContentRichTextBoxMouseMove);
			this.contentRichTextBox.MouseUp += new MouseEventHandler(this.ContentRichTextBoxMouseUp);
			this.chooseMeaningContextMenuStrip.Name = "chooseMeaningContextMenuStrip";
			this.chooseMeaningContextMenuStrip.Size = new Size(61, 4);
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(434, 365);
			base.Controls.Add(this.contentRichTextBox);
			this.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 128);
//			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.Name = "DocumentPanel";
			this.Text = "DocumentPanel";
			base.KeyDown += new KeyEventHandler(this.DocumentPanelKeyDown);
			this.commentContextMenuStrip.ResumeLayout(false);
			this.addToVietPhraseContextMenuStrip.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
