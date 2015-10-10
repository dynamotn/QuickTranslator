using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using TranslatorEngine;

namespace QuickTranslator
{
	public class UpdateVietPhraseForm : Form
	{
		private delegate void GenericDelegate();

		private const string BAIKE_CHECKING = "Đang kiểm tra...";

		private const string BAIKE_EXIST = "Tồn tại.";

		private const string BAIKE_NOT_EXIST = "Không tồn tại.";

		private int type;

		private int baikeCheckingLoopLimit;

		private bool needSurfBaike;

		private string chinesePhraseToSurfBaike = "";

		private IContainer components;

		private LinkLabel surfBaikeLinkLabel;

		private Label existInBaikeLabel;

		private GroupBox groupBox2;

		private BackgroundWorker backgroundWorker;

		private LinkLabel capitalizeAllLinkLabel;

		private LinkLabel capitalize1WordLinkLabel;

		private LinkLabel capitalize2WordsLinkLabel;

		private LinkLabel capitalize3WordsLinkLabel;

		private GroupBox groupBox1;

		private CheckBox compressDictionaryHistoryCheckBox;

		private Label updatedByLabel;

		private Label entryCountLabel;

		private Label label3;

		private CheckBox sortingCheckBox;

		private Label vietPhraseLabel;

		private Button updateButton;

		private Button deleteButton;

		private Button cancelButton;

		private TextBox vietPhraseRichTextBox;

		private TextBox hanVietRichTextBox;

		private Label label2;

		private TextBox chineseTextBox;

		private Label label1;

		public bool NeedSurfBaike
		{
			get
			{
				return this.needSurfBaike;
			}
		}

		public string ChinesePhraseToSurfBaike
		{
			get
			{
				return this.chinesePhraseToSurfBaike;
			}
		}

		public UpdateVietPhraseForm()
		{
			this.InitializeComponent();
		}

		public UpdateVietPhraseForm(string chineseToLookup, int type)
		{
			this.InitializeComponent();
			this.chineseTextBox.Text = chineseToLookup.Trim(new char[]
			{
				' ',
				'.',
				':',
				';',
				'?',
				'!',
				'"',
				'\'',
				',',
				'\n',
				'\t'
			});
			this.type = type;
			this.vietPhraseLabel.Text = ((type == 0) ? "VietPhrase:" : "Name:");
			this.Text = ((type == 0) ? "Update VietPhrase" : ((type == 1) ? "Update Name (chính)" : "Update Name (phụ)"));
			this.entryCountLabel.Text = (((type == 0) ? TranslatorEngine.TranslatorEngine.GetVietPhraseDictionaryCount() : TranslatorEngine.TranslatorEngine.GetNameDictionaryCount(type == 1)).ToString("0,0") ?? "");
		}

		private void ChineseTextBoxTextChanged(object sender, EventArgs e)
		{
			this.updateButton.Enabled = (this.chineseTextBox.Text.Trim() != "");
			this.deleteButton.Enabled = (this.chineseTextBox.Text.Trim() != "");
			this.existInBaikeLabel.Text = "Đang kiểm tra...";
			this.existInBaikeLabel.ForeColor = SystemColors.ControlText;
			this.surfBaikeLinkLabel.Visible = false;
			if (this.chineseTextBox.Text.Trim() == "")
			{
				this.hanVietRichTextBox.Text = "";
				this.vietPhraseRichTextBox.Text = "";
				this.checkBaikeInNewThread(this.chineseTextBox.Text.Trim());
				return;
			}
			CharRange[] array;
			this.hanVietRichTextBox.Text = TranslatorEngine.TranslatorEngine.ChineseToHanViet(this.chineseTextBox.Text, out array).Trim();
			string text = (this.type == 0) ? TranslatorEngine.TranslatorEngine.GetVietPhraseValueFromKey(this.chineseTextBox.Text) : TranslatorEngine.TranslatorEngine.GetNameValueFromKey(this.chineseTextBox.Text, this.type == 1);
			this.vietPhraseRichTextBox.Text = ((text == null) ? ((this.type == 0) ? this.hanVietRichTextBox.Text : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(this.hanVietRichTextBox.Text)) : text);
			this.deleteButton.Enabled = (text != null);
			this.updateButton.Text = ((text != null) ? "Update" : "Add");
			this.updatedByLabel.Text = ((this.type == 0) ? TranslatorEngine.TranslatorEngine.GetVietPhraseHistoryLogRecord(this.chineseTextBox.Text) : TranslatorEngine.TranslatorEngine.GetNameHistoryLogRecord(this.chineseTextBox.Text, this.type == 1));
			this.checkBaikeInNewThread(this.chineseTextBox.Text.Trim());
		}

		private void DeleteButtonClick(object sender, EventArgs e)
		{
			if (this.type == 0)
			{
				TranslatorEngine.TranslatorEngine.DeleteKeyFromVietPhraseDictionary(this.chineseTextBox.Text, this.sortingCheckBox.Checked);
			}
			else
			{
				TranslatorEngine.TranslatorEngine.DeleteKeyFromNameDictionary(this.chineseTextBox.Text, this.sortingCheckBox.Checked, this.type == 1);
			}
			this.CompressDictionaryHistory();
			base.Close();
		}

		private void UpdateButtonClick(object sender, EventArgs e)
		{
			if (this.chineseTextBox.Text.Trim() == "")
			{
				return;
			}
			if (this.type == 0)
			{
				TranslatorEngine.TranslatorEngine.UpdateVietPhraseDictionary(this.chineseTextBox.Text, this.vietPhraseRichTextBox.Text, this.sortingCheckBox.Checked);
			}
			else
			{
				TranslatorEngine.TranslatorEngine.UpdateNameDictionary(this.chineseTextBox.Text, this.vietPhraseRichTextBox.Text, this.sortingCheckBox.Checked, this.type == 1);
			}
			this.CompressDictionaryHistory();
			base.Close();
		}

		private void CompressDictionaryHistory()
		{
			if (this.compressDictionaryHistoryCheckBox.Checked)
			{
				if (this.type == 0)
				{
					TranslatorEngine.TranslatorEngine.CompressOnlyVietPhraseDictionaryHistory();
					return;
				}
				TranslatorEngine.TranslatorEngine.CompressOnlyNameDictionaryHistory(this.type == 1);
			}
		}

		private void CancelButtonClick(object sender, EventArgs e)
		{
			base.Close();
		}

		private void VietPhraseRichTextBoxKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
			{
				e.Handled = true;
			}
		}

		private void UpdateVietPhraseFormLoad(object sender, EventArgs e)
		{
			this.ChineseTextBoxTextChanged(null, null);
			this.vietPhraseRichTextBox.SelectAll();
			base.ActiveControl = this.vietPhraseRichTextBox;
		}

		private void CapitalizeAllLinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			this.vietPhraseRichTextBox.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(this.vietPhraseRichTextBox.Text);
			this.vietPhraseRichTextBox.SelectAll();
			this.vietPhraseRichTextBox.Focus();
		}

		private void Capitalize1WordLinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			this.vietPhraseRichTextBox.Text = this.capitalizeWords(this.vietPhraseRichTextBox.Text, 1);
			this.vietPhraseRichTextBox.SelectAll();
			this.vietPhraseRichTextBox.Focus();
		}

		private string capitalizeWords(string originalText, int numberOfWordsToBeCapitalized)
		{
			if (string.IsNullOrEmpty(originalText))
			{
				return string.Empty;
			}
			string[] array = originalText.Split(" ".ToCharArray(), numberOfWordsToBeCapitalized + 1);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				if (i < numberOfWordsToBeCapitalized)
				{
					stringBuilder.Append(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(array[i]));
				}
				else
				{
					stringBuilder.Append(CultureInfo.CurrentCulture.TextInfo.ToLower(array[i]));
				}
				stringBuilder.Append(" ");
			}
			return stringBuilder.ToString().Trim();
		}

		private void Capitalize2WordsLinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			this.vietPhraseRichTextBox.Text = this.capitalizeWords(this.vietPhraseRichTextBox.Text, 2);
			this.vietPhraseRichTextBox.SelectAll();
			this.vietPhraseRichTextBox.Focus();
		}

		private void Capitalize3WordsLinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			this.vietPhraseRichTextBox.Text = this.capitalizeWords(this.vietPhraseRichTextBox.Text, 3);
			this.vietPhraseRichTextBox.SelectAll();
			this.vietPhraseRichTextBox.Focus();
		}

		private void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
		{
			int millisecondsTimeout = (int)e.Argument;
			string b = "";
			bool phraseExistsInBaike = false;
			string text = null;
			while (0 <= this.baikeCheckingLoopLimit)
			{
				text = this.chineseTextBox.Text.Trim();
				if (!string.IsNullOrEmpty(text) && text != b)
				{
					try
					{
						phraseExistsInBaike = this.existsInBaike(text);
					}
					catch
					{
						phraseExistsInBaike = false;
					}
					finally
					{
						b = text;
					}
					this.updateBaikeCheckingResultToGui(phraseExistsInBaike);
				}
				Thread.Sleep(millisecondsTimeout);
				this.baikeCheckingLoopLimit--;
			}
		}

		private void checkBaikeInNewThread(string chinese)
		{
			bool phraseExistsInBaike = false;
			ThreadPool.QueueUserWorkItem(delegate(object param0)
			{
				try
				{
					phraseExistsInBaike = this.existsInBaike(chinese);
				}
				catch
				{
					phraseExistsInBaike = false;
				}
				this.updateBaikeCheckingResultToGui(phraseExistsInBaike);
			});
		}

		private void updateBaikeCheckingResultToGui(bool phraseExistsInBaike)
		{
			string result = phraseExistsInBaike ? "Tồn tại." : "Không tồn tại.";
			Color foreColor = phraseExistsInBaike ? Color.Blue : Color.Red;
			UpdateVietPhraseForm.GenericDelegate method = delegate
			{
				this.existInBaikeLabel.Text = result;
				this.existInBaikeLabel.ForeColor = foreColor;
				this.surfBaikeLinkLabel.Visible = phraseExistsInBaike;
			};
			try
			{
				base.BeginInvoke(method);
			}
			catch
			{
			}
		}

		private bool existsInBaike(string chinesePhrase)
		{
			if (string.IsNullOrEmpty(chinesePhrase))
			{
				return false;
			}
//			string requestUriString = "http://baike.baidu.com/list-php/dispose/searchword.php?word=" + HttpUtility.UrlEncode(chinesePhrase, Encoding.GetEncoding("gb2312")) + "&pic=1";
//			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUriString);
			bool result=true;
//			using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
//			{
//				using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
//				{
//					streamReader.ReadLine();
//					string text = streamReader.ReadLine();
//					if (string.IsNullOrEmpty(text))
//					{
//						result = false;
//					}
//					else if (text.Contains("error"))
//					{
//						result = false;
//					}
//					else if (text.Contains("notexists"))
//					{
//						result = false;
//					}
//					else if (text.Contains("URL=/view/"))
//					{
//						result = true;
//					}
//					else
//					{
//						result = false;
//					}
//				}
//			}
			return result;
		}

		private void BackgroundWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
		}

		private void SurfBaikeLinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			this.chinesePhraseToSurfBaike = this.chineseTextBox.Text.Trim();
			this.needSurfBaike = true;
			base.Close();
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UpdateVietPhraseForm));
			this.label1 = new Label();
			this.chineseTextBox = new TextBox();
			this.label2 = new Label();
			this.hanVietRichTextBox = new TextBox();
			this.vietPhraseLabel = new Label();
			this.vietPhraseRichTextBox = new TextBox();
			this.updateButton = new Button();
			this.cancelButton = new Button();
			this.deleteButton = new Button();
			this.sortingCheckBox = new CheckBox();
			this.label3 = new Label();
			this.entryCountLabel = new Label();
			this.updatedByLabel = new Label();
			this.compressDictionaryHistoryCheckBox = new CheckBox();
			this.groupBox1 = new GroupBox();
			this.capitalize3WordsLinkLabel = new LinkLabel();
			this.capitalize2WordsLinkLabel = new LinkLabel();
			this.capitalize1WordLinkLabel = new LinkLabel();
			this.capitalizeAllLinkLabel = new LinkLabel();
			this.backgroundWorker = new BackgroundWorker();
			this.groupBox2 = new GroupBox();
			this.surfBaikeLinkLabel = new LinkLabel();
			this.existInBaikeLabel = new Label();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			base.SuspendLayout();
			this.label1.Location = new Point(9, 33);
			this.label1.Name = "label1";
			this.label1.Size = new Size(73, 18);
			this.label1.TabIndex = 0;
			this.label1.Text = "Chinese:";
			this.label1.TextAlign = ContentAlignment.MiddleRight;
			this.chineseTextBox.Font = new Font("Arial Unicode MS", 14.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.chineseTextBox.Location = new Point(88, 26);
			this.chineseTextBox.Name = "chineseTextBox";
			this.chineseTextBox.Size = new Size(451, 33);
			this.chineseTextBox.TabIndex = 4;
			this.chineseTextBox.TextChanged += new EventHandler(this.ChineseTextBoxTextChanged);
			this.label2.Location = new Point(9, 85);
			this.label2.Name = "label2";
			this.label2.Size = new Size(73, 18);
			this.label2.TabIndex = 0;
			this.label2.Text = "Hán Việt:";
			this.label2.TextAlign = ContentAlignment.MiddleRight;
			this.hanVietRichTextBox.Font = new Font("Arial Unicode MS", 12f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.hanVietRichTextBox.Location = new Point(88, 65);
			this.hanVietRichTextBox.Multiline = true;
			this.hanVietRichTextBox.Name = "hanVietRichTextBox";
			this.hanVietRichTextBox.ReadOnly = true;
			this.hanVietRichTextBox.Size = new Size(451, 58);
			this.hanVietRichTextBox.TabIndex = 5;
			this.vietPhraseLabel.Location = new Point(9, 187);
			this.vietPhraseLabel.Name = "vietPhraseLabel";
			this.vietPhraseLabel.Size = new Size(73, 18);
			this.vietPhraseLabel.TabIndex = 0;
			this.vietPhraseLabel.Text = "VietPhrase:";
			this.vietPhraseLabel.TextAlign = ContentAlignment.MiddleRight;
			this.vietPhraseRichTextBox.Font = new Font("Arial Unicode MS", 12f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.vietPhraseRichTextBox.Location = new Point(88, 127);
			this.vietPhraseRichTextBox.Multiline = true;
			this.vietPhraseRichTextBox.Name = "vietPhraseRichTextBox";
			this.vietPhraseRichTextBox.Size = new Size(451, 132);
			this.vietPhraseRichTextBox.TabIndex = 6;
			this.vietPhraseRichTextBox.KeyDown += new KeyEventHandler(this.VietPhraseRichTextBoxKeyDown);
			this.updateButton.Location = new Point(158, 352);
			this.updateButton.Name = "updateButton";
			this.updateButton.Size = new Size(84, 23);
			this.updateButton.TabIndex = 12;
			this.updateButton.Text = "Update or Add";
			this.updateButton.UseVisualStyleBackColor = true;
			this.updateButton.Click += new EventHandler(this.UpdateButtonClick);
			this.cancelButton.DialogResult = DialogResult.Cancel;
			this.cancelButton.Location = new Point(450, 352);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new Size(84, 23);
			this.cancelButton.TabIndex = 14;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new EventHandler(this.CancelButtonClick);
			this.deleteButton.Location = new Point(304, 352);
			this.deleteButton.Name = "deleteButton";
			this.deleteButton.Size = new Size(84, 23);
			this.deleteButton.TabIndex = 13;
			this.deleteButton.Text = "Delete";
			this.deleteButton.UseVisualStyleBackColor = true;
			this.deleteButton.Click += new EventHandler(this.DeleteButtonClick);
			this.sortingCheckBox.Location = new Point(88, 288);
			this.sortingCheckBox.Name = "sortingCheckBox";
			this.sortingCheckBox.Size = new Size(456, 24);
			this.sortingCheckBox.TabIndex = 7;
			this.sortingCheckBox.Text = "Sắp xếp lại dữ liệu từ điển";
			this.sortingCheckBox.UseVisualStyleBackColor = true;
			this.label3.Location = new Point(9, 2);
			this.label3.Name = "label3";
			this.label3.Size = new Size(73, 18);
			this.label3.TabIndex = 0;
			this.label3.Text = "Entries:";
			this.label3.TextAlign = ContentAlignment.MiddleRight;
			this.entryCountLabel.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.entryCountLabel.Location = new Point(88, 0);
			this.entryCountLabel.Name = "entryCountLabel";
			this.entryCountLabel.Size = new Size(160, 23);
			this.entryCountLabel.TabIndex = 7;
			this.entryCountLabel.Text = "label4";
			this.entryCountLabel.TextAlign = ContentAlignment.MiddleLeft;
			this.updatedByLabel.Font = new Font("Arial Unicode MS", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.updatedByLabel.ForeColor = Color.FromArgb(0, 0, 192);
			this.updatedByLabel.Location = new Point(88, 264);
			this.updatedByLabel.Name = "updatedByLabel";
			this.updatedByLabel.Size = new Size(456, 23);
			this.updatedByLabel.TabIndex = 8;
			this.updatedByLabel.TextAlign = ContentAlignment.MiddleLeft;
			this.compressDictionaryHistoryCheckBox.Location = new Point(88, 312);
			this.compressDictionaryHistoryCheckBox.Name = "compressDictionaryHistoryCheckBox";
			this.compressDictionaryHistoryCheckBox.Size = new Size(456, 24);
			this.compressDictionaryHistoryCheckBox.TabIndex = 11;
			this.compressDictionaryHistoryCheckBox.Text = "Nén Dictionary History (với mỗi entry, chỉ giữ lại history của hành động gần nhất)";
			this.compressDictionaryHistoryCheckBox.UseVisualStyleBackColor = true;
			this.groupBox1.Controls.Add(this.capitalize3WordsLinkLabel);
			this.groupBox1.Controls.Add(this.capitalize2WordsLinkLabel);
			this.groupBox1.Controls.Add(this.capitalize1WordLinkLabel);
			this.groupBox1.Controls.Add(this.capitalizeAllLinkLabel);
			this.groupBox1.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.groupBox1.Location = new Point(544, 128);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new Size(144, 128);
			this.groupBox1.TabIndex = 15;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Edit nhanh";
			this.capitalize3WordsLinkLabel.Location = new Point(8, 74);
			this.capitalize3WordsLinkLabel.Name = "capitalize3WordsLinkLabel";
			this.capitalize3WordsLinkLabel.Size = new Size(128, 16);
			this.capitalize3WordsLinkLabel.TabIndex = 0;
			this.capitalize3WordsLinkLabel.TabStop = true;
			this.capitalize3WordsLinkLabel.Text = "Viết hoa 3 từ đơn đầu";
			this.capitalize3WordsLinkLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(this.Capitalize3WordsLinkLabelLinkClicked);
			this.capitalize2WordsLinkLabel.Location = new Point(8, 45);
			this.capitalize2WordsLinkLabel.Name = "capitalize2WordsLinkLabel";
			this.capitalize2WordsLinkLabel.Size = new Size(128, 16);
			this.capitalize2WordsLinkLabel.TabIndex = 0;
			this.capitalize2WordsLinkLabel.TabStop = true;
			this.capitalize2WordsLinkLabel.Text = "Viết hoa 2 từ đơn đầu";
			this.capitalize2WordsLinkLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(this.Capitalize2WordsLinkLabelLinkClicked);
			this.capitalize1WordLinkLabel.Location = new Point(8, 16);
			this.capitalize1WordLinkLabel.Name = "capitalize1WordLinkLabel";
			this.capitalize1WordLinkLabel.Size = new Size(128, 16);
			this.capitalize1WordLinkLabel.TabIndex = 0;
			this.capitalize1WordLinkLabel.TabStop = true;
			this.capitalize1WordLinkLabel.Text = "Viết hoa 1 từ đơn đầu";
			this.capitalize1WordLinkLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(this.Capitalize1WordLinkLabelLinkClicked);
			this.capitalizeAllLinkLabel.Location = new Point(8, 103);
			this.capitalizeAllLinkLabel.Name = "capitalizeAllLinkLabel";
			this.capitalizeAllLinkLabel.Size = new Size(128, 16);
			this.capitalizeAllLinkLabel.TabIndex = 0;
			this.capitalizeAllLinkLabel.TabStop = true;
			this.capitalizeAllLinkLabel.Text = "Viết hoa tất cả";
			this.capitalizeAllLinkLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(this.CapitalizeAllLinkLabelLinkClicked);
			this.backgroundWorker.WorkerSupportsCancellation = true;
			this.backgroundWorker.DoWork += new DoWorkEventHandler(this.BackgroundWorkerDoWork);
			this.backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.BackgroundWorkerRunWorkerCompleted);
			this.groupBox2.Controls.Add(this.surfBaikeLinkLabel);
			this.groupBox2.Controls.Add(this.existInBaikeLabel);
			this.groupBox2.Location = new Point(544, 59);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new Size(144, 64);
			this.groupBox2.TabIndex = 16;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Tồn tại trên Baike?";
			this.surfBaikeLinkLabel.Location = new Point(105, 28);
			this.surfBaikeLinkLabel.Name = "surfBaikeLinkLabel";
			this.surfBaikeLinkLabel.Size = new Size(34, 20);
			this.surfBaikeLinkLabel.TabIndex = 17;
			this.surfBaikeLinkLabel.TabStop = true;
			this.surfBaikeLinkLabel.Text = "Xem";
			this.surfBaikeLinkLabel.Visible = false;
			this.surfBaikeLinkLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(this.SurfBaikeLinkLabelLinkClicked);
			this.existInBaikeLabel.Location = new Point(8, 24);
			this.existInBaikeLabel.Name = "existInBaikeLabel";
			this.existInBaikeLabel.Size = new Size(88, 23);
			this.existInBaikeLabel.TabIndex = 0;
			this.existInBaikeLabel.Text = "Đang kiểm tra...";
			this.existInBaikeLabel.TextAlign = ContentAlignment.MiddleLeft;
			base.AcceptButton = this.updateButton;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.CancelButton = this.cancelButton;
			base.ClientSize = new Size(695, 383);
			base.Controls.Add(this.groupBox2);
			base.Controls.Add(this.groupBox1);
			base.Controls.Add(this.compressDictionaryHistoryCheckBox);
			base.Controls.Add(this.updatedByLabel);
			base.Controls.Add(this.entryCountLabel);
			base.Controls.Add(this.sortingCheckBox);
			base.Controls.Add(this.deleteButton);
			base.Controls.Add(this.cancelButton);
			base.Controls.Add(this.updateButton);
			base.Controls.Add(this.vietPhraseRichTextBox);
			base.Controls.Add(this.hanVietRichTextBox);
			base.Controls.Add(this.vietPhraseLabel);
			base.Controls.Add(this.chineseTextBox);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.label1);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "UpdateVietPhraseForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Update VietPhrase/Name";
			base.Load += new EventHandler(this.UpdateVietPhraseFormLoad);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
