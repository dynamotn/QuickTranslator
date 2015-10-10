using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace QuickTranslator
{
	public class PostTTVForm : Form
	{
		private string viet;

		private string hanViet;

		private string vietPhrase;

		private string trung;

		private string postToTTVTemplate;

		private DateTime editingStartDateTime;

		private IContainer components;

		private Label label3;

		private Label label4;

		private Label label5;

		private Label label6;

		private Label label2;

		private RichTextBox postContentRichTextBox;

		private TextBox thaoLuanTextBox;

		private CheckBox vietCheckBox;

		private Button copyToClipboardButton;

		private GroupBox groupBox5;

		private GroupBox groupBox4;

		private CheckBox vietPhraseOneMeaningCheckBox;

		private CheckBox hanVietCheckBox;

		private CheckBox trungCheckBox;

		private GroupBox groupBox2;

		private Label label11;

		private TextBox lineFourTextBox;

		private Label label10;

		private TextBox lineThreeTextBox;

		private Label label9;

		private TextBox lineTwoTextBox;

		private TextBox lineOneTextBox;

		private Label label8;

		private Label label1;

		public PostTTVForm(string viet, string hanViet, string vietPhrase, string trung, DateTime editingStartDateTime)
		{
			this.InitializeComponent();
			this.postToTTVTemplate = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "PostToTTV.config")).Trim();
			this.loadSettings();
			this.viet = viet;
			this.hanViet = hanViet;
			this.vietPhrase = vietPhrase;
			this.trung = trung;
			this.editingStartDateTime = editingStartDateTime;
			string text = trung.Substring(0, (trung.IndexOf('\n') < 0) ? 0 : trung.IndexOf('\n')).Trim();
			if (text.StartsWith("第") && (text.Contains("章") || text.Contains("节")))
			{
				string text2 = vietPhrase.Substring(0, (vietPhrase.IndexOf('\n') < 0) ? 0 : vietPhrase.IndexOf('\n'));
				int num = text2.IndexOfAny(":-".ToCharArray());
				if (0 < num)
				{
					int num2 = num;
					while (num2 < text2.Length && (text2[num2] == ' ' || text2[num2] == ':' || text2[num2] == '-'))
					{
						num2++;
					}
					if (num < num2)
					{
						this.vietPhrase = vietPhrase.Substring(0, num2) + vietPhrase[num2].ToString().ToUpper() + vietPhrase.Substring(num2 + 1, vietPhrase.Length - num2 - 1);
						this.lineTwoTextBox.Text = this.vietPhrase.Substring(0, vietPhrase.IndexOf('\n')).Trim();
					}
				}
			}
			this.LineOneTextBoxTextChanged(null, null);
		}

		private void HuyButtonClick(object sender, EventArgs e)
		{
			base.Close();
		}

		private void LineOneTextBoxTextChanged(object sender, EventArgs e)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("[FONT=\"Palatino Linotype\"]");
			string text = this.postToTTVTemplate;
			text = text.Replace("$dong1$", this.lineOneTextBox.Text);
			text = text.Replace("$dong2$", this.lineTwoTextBox.Text);
			text = text.Replace("$dong3$", string.IsNullOrEmpty(this.lineThreeTextBox.Text) ? "" : ("Tác giả: " + this.lineThreeTextBox.Text));
			text = text.Replace("$dong4$", this.lineFourTextBox.Text);
			string text2 = DateTime.Now.Subtract(this.editingStartDateTime).ToString().Replace(":", " : ");
			if (8 < text2.Length)
			{
				text2 = text2.Substring(0, text2.Length - 8);
			}
			text = text.Replace("$thoiGian$", text2);
			stringBuilder.AppendLine(text).AppendLine("");
			if (this.vietCheckBox.Checked)
			{
				stringBuilder.Append("[SPOILER=\"              Việt             \"]").AppendLine(this.viet).AppendLine("[/SPOILER]");
			}
			if (this.vietPhraseOneMeaningCheckBox.Checked)
			{
				stringBuilder.Append("[SPOILER=\"       VietPhrase       \"]").AppendLine(this.vietPhrase).AppendLine("[/SPOILER]");
			}
			if (this.hanVietCheckBox.Checked)
			{
				stringBuilder.Append("[SPOILER=\"          Hán Việt        \"]").AppendLine(this.hanViet).AppendLine("[/SPOILER]");
			}
			if (this.trungCheckBox.Checked)
			{
				stringBuilder.Append("[SPOILER=\"            Trung           \"][CODE]").AppendLine(this.trung).AppendLine("[/CODE][/SPOILER]");
			}
			if (!string.IsNullOrEmpty(this.thaoLuanTextBox.Text))
			{
				stringBuilder.Append("[COLOR=\"#F5F5FF\"]...[/COLOR]Thảo luận: [URL=\"").Append(this.thaoLuanTextBox.Text).AppendLine("\"]tại đây![/URL]");
			}
			stringBuilder.Append("[/FONT]");
			this.postContentRichTextBox.Text = stringBuilder.ToString();
		}

		private void CopyToClipboardButtonClick(object sender, EventArgs e)
		{
			try
			{
				Clipboard.SetDataObject(this.postContentRichTextBox.Text, true, 50, 100);
			}
			catch (ExternalException)
			{
			}
			this.saveSettings();
			base.Close();
		}

		private void loadSettings()
		{
			this.lineOneTextBox.Text = PostTTVFormSettings.Default.line1;
			this.lineTwoTextBox.Text = PostTTVFormSettings.Default.line2;
			this.lineThreeTextBox.Text = PostTTVFormSettings.Default.line3;
			this.lineFourTextBox.Text = PostTTVFormSettings.Default.line4;
			this.vietCheckBox.Checked = PostTTVFormSettings.Default.spoilerViet;
			this.vietPhraseOneMeaningCheckBox.Checked = PostTTVFormSettings.Default.spoilerVietPhraseOneMeaning;
			this.hanVietCheckBox.Checked = PostTTVFormSettings.Default.spoilerHanViet;
			this.trungCheckBox.Checked = PostTTVFormSettings.Default.spoilerTrung;
			this.thaoLuanTextBox.Text = PostTTVFormSettings.Default.discussionUrl;
		}

		private void saveSettings()
		{
			PostTTVFormSettings.Default.line1 = this.lineOneTextBox.Text;
			PostTTVFormSettings.Default.line2 = this.lineTwoTextBox.Text;
			PostTTVFormSettings.Default.line3 = this.lineThreeTextBox.Text;
			PostTTVFormSettings.Default.line4 = this.lineFourTextBox.Text;
			PostTTVFormSettings.Default.spoilerViet = this.vietCheckBox.Checked;
			PostTTVFormSettings.Default.spoilerVietPhraseOneMeaning = this.vietPhraseOneMeaningCheckBox.Checked;
			PostTTVFormSettings.Default.spoilerHanViet = this.hanVietCheckBox.Checked;
			PostTTVFormSettings.Default.spoilerTrung = this.trungCheckBox.Checked;
			PostTTVFormSettings.Default.discussionUrl = this.thaoLuanTextBox.Text;
			PostTTVFormSettings.Default.Save();
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(PostTTVForm));
			this.label8 = new Label();
			this.lineOneTextBox = new TextBox();
			this.lineTwoTextBox = new TextBox();
			this.label9 = new Label();
			this.lineThreeTextBox = new TextBox();
			this.label10 = new Label();
			this.lineFourTextBox = new TextBox();
			this.label11 = new Label();
			this.groupBox2 = new GroupBox();
			this.trungCheckBox = new CheckBox();
			this.hanVietCheckBox = new CheckBox();
			this.vietCheckBox = new CheckBox();
			this.vietPhraseOneMeaningCheckBox = new CheckBox();
			this.groupBox4 = new GroupBox();
			this.label6 = new Label();
			this.label5 = new Label();
			this.label4 = new Label();
			this.label3 = new Label();
			this.groupBox5 = new GroupBox();
			this.thaoLuanTextBox = new TextBox();
			this.label1 = new Label();
			this.copyToClipboardButton = new Button();
			this.postContentRichTextBox = new RichTextBox();
			this.label2 = new Label();
			this.groupBox2.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox5.SuspendLayout();
			base.SuspendLayout();
			this.label8.BackColor = SystemColors.GradientInactiveCaption;
			this.label8.Font = new Font("Arial", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.label8.Location = new Point(6, 16);
			this.label8.Name = "label8";
			this.label8.Size = new Size(74, 22);
			this.label8.TabIndex = 21;
			this.label8.Text = "Dòng 1:";
			this.label8.TextAlign = ContentAlignment.MiddleRight;
			this.lineOneTextBox.Location = new Point(86, 18);
			this.lineOneTextBox.Name = "lineOneTextBox";
			this.lineOneTextBox.Size = new Size(434, 20);
			this.lineOneTextBox.TabIndex = 0;
			this.lineOneTextBox.TextChanged += new EventHandler(this.LineOneTextBoxTextChanged);
			this.lineTwoTextBox.Location = new Point(86, 44);
			this.lineTwoTextBox.Name = "lineTwoTextBox";
			this.lineTwoTextBox.Size = new Size(434, 20);
			this.lineTwoTextBox.TabIndex = 1;
			this.lineTwoTextBox.TextChanged += new EventHandler(this.LineOneTextBoxTextChanged);
			this.label9.BackColor = SystemColors.GradientInactiveCaption;
			this.label9.Font = new Font("Arial", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.label9.Location = new Point(6, 42);
			this.label9.Name = "label9";
			this.label9.Size = new Size(74, 22);
			this.label9.TabIndex = 23;
			this.label9.Text = "Dòng 2:";
			this.label9.TextAlign = ContentAlignment.MiddleRight;
			this.lineThreeTextBox.Location = new Point(86, 70);
			this.lineThreeTextBox.Name = "lineThreeTextBox";
			this.lineThreeTextBox.Size = new Size(434, 20);
			this.lineThreeTextBox.TabIndex = 2;
			this.lineThreeTextBox.TextChanged += new EventHandler(this.LineOneTextBoxTextChanged);
			this.label10.BackColor = SystemColors.GradientInactiveCaption;
			this.label10.Font = new Font("Arial", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.label10.Location = new Point(6, 68);
			this.label10.Name = "label10";
			this.label10.Size = new Size(74, 22);
			this.label10.TabIndex = 25;
			this.label10.Text = "Dòng 3:";
			this.label10.TextAlign = ContentAlignment.MiddleRight;
			this.lineFourTextBox.Location = new Point(86, 96);
			this.lineFourTextBox.Name = "lineFourTextBox";
			this.lineFourTextBox.Size = new Size(434, 20);
			this.lineFourTextBox.TabIndex = 3;
			this.lineFourTextBox.TextChanged += new EventHandler(this.LineOneTextBoxTextChanged);
			this.label11.BackColor = SystemColors.GradientInactiveCaption;
			this.label11.Font = new Font("Arial", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.label11.Location = new Point(6, 94);
			this.label11.Name = "label11";
			this.label11.Size = new Size(74, 22);
			this.label11.TabIndex = 27;
			this.label11.Text = "Dòng 4:";
			this.label11.TextAlign = ContentAlignment.MiddleRight;
			this.groupBox2.Controls.Add(this.trungCheckBox);
			this.groupBox2.Controls.Add(this.hanVietCheckBox);
			this.groupBox2.Controls.Add(this.vietCheckBox);
			this.groupBox2.Controls.Add(this.vietPhraseOneMeaningCheckBox);
			this.groupBox2.Location = new Point(10, 156);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new Size(594, 59);
			this.groupBox2.TabIndex = 3;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Nội Dung (Spoiler)";
			this.trungCheckBox.Checked = true;
			this.trungCheckBox.CheckState = CheckState.Checked;
			this.trungCheckBox.Location = new Point(456, 19);
			this.trungCheckBox.Name = "trungCheckBox";
			this.trungCheckBox.Size = new Size(123, 24);
			this.trungCheckBox.TabIndex = 2;
			this.trungCheckBox.Text = "Tiếng Trung";
			this.trungCheckBox.UseVisualStyleBackColor = true;
			this.trungCheckBox.CheckedChanged += new EventHandler(this.LineOneTextBoxTextChanged);
			this.hanVietCheckBox.Checked = true;
			this.hanVietCheckBox.CheckState = CheckState.Checked;
			this.hanVietCheckBox.Location = new Point(315, 19);
			this.hanVietCheckBox.Name = "hanVietCheckBox";
			this.hanVietCheckBox.Size = new Size(123, 24);
			this.hanVietCheckBox.TabIndex = 1;
			this.hanVietCheckBox.Text = "Hán Việt";
			this.hanVietCheckBox.UseVisualStyleBackColor = true;
			this.hanVietCheckBox.CheckedChanged += new EventHandler(this.LineOneTextBoxTextChanged);
			this.vietCheckBox.Location = new Point(6, 19);
			this.vietCheckBox.Name = "vietCheckBox";
			this.vietCheckBox.Size = new Size(123, 24);
			this.vietCheckBox.TabIndex = 0;
			this.vietCheckBox.Text = "Việt";
			this.vietCheckBox.UseVisualStyleBackColor = true;
			this.vietCheckBox.CheckedChanged += new EventHandler(this.LineOneTextBoxTextChanged);
			this.vietPhraseOneMeaningCheckBox.Checked = true;
			this.vietPhraseOneMeaningCheckBox.CheckState = CheckState.Checked;
			this.vietPhraseOneMeaningCheckBox.Location = new Point(146, 19);
			this.vietPhraseOneMeaningCheckBox.Name = "vietPhraseOneMeaningCheckBox";
			this.vietPhraseOneMeaningCheckBox.Size = new Size(149, 24);
			this.vietPhraseOneMeaningCheckBox.TabIndex = 0;
			this.vietPhraseOneMeaningCheckBox.Text = "VietPhrase một nghĩa";
			this.vietPhraseOneMeaningCheckBox.UseVisualStyleBackColor = true;
			this.vietPhraseOneMeaningCheckBox.CheckedChanged += new EventHandler(this.LineOneTextBoxTextChanged);
			this.groupBox4.Controls.Add(this.label6);
			this.groupBox4.Controls.Add(this.label5);
			this.groupBox4.Controls.Add(this.label4);
			this.groupBox4.Controls.Add(this.label3);
			this.groupBox4.Controls.Add(this.label8);
			this.groupBox4.Controls.Add(this.lineOneTextBox);
			this.groupBox4.Controls.Add(this.label9);
			this.groupBox4.Controls.Add(this.lineTwoTextBox);
			this.groupBox4.Controls.Add(this.lineFourTextBox);
			this.groupBox4.Controls.Add(this.label10);
			this.groupBox4.Controls.Add(this.label11);
			this.groupBox4.Controls.Add(this.lineThreeTextBox);
			this.groupBox4.Location = new Point(10, 12);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new Size(594, 127);
			this.groupBox4.TabIndex = 2;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Tiêu Đề";
			this.label6.Location = new Point(526, 96);
			this.label6.Name = "label6";
			this.label6.Size = new Size(62, 20);
			this.label6.TabIndex = 28;
			this.label6.Text = "(Converter)";
			this.label6.TextAlign = ContentAlignment.MiddleLeft;
			this.label5.Location = new Point(526, 70);
			this.label5.Name = "label5";
			this.label5.Size = new Size(62, 20);
			this.label5.TabIndex = 28;
			this.label5.Text = "(Tác giả)";
			this.label5.TextAlign = ContentAlignment.MiddleLeft;
			this.label4.Location = new Point(526, 44);
			this.label4.Name = "label4";
			this.label4.Size = new Size(62, 20);
			this.label4.TabIndex = 28;
			this.label4.Text = "(Chương)";
			this.label4.TextAlign = ContentAlignment.MiddleLeft;
			this.label3.Location = new Point(526, 17);
			this.label3.Name = "label3";
			this.label3.Size = new Size(62, 20);
			this.label3.TabIndex = 28;
			this.label3.Text = "(Quyển)";
			this.label3.TextAlign = ContentAlignment.MiddleLeft;
			this.groupBox5.Controls.Add(this.thaoLuanTextBox);
			this.groupBox5.Controls.Add(this.label1);
			this.groupBox5.Location = new Point(10, 234);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new Size(594, 70);
			this.groupBox5.TabIndex = 4;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Link đến Thảo Luận";
			this.thaoLuanTextBox.Location = new Point(6, 19);
			this.thaoLuanTextBox.Name = "thaoLuanTextBox";
			this.thaoLuanTextBox.Size = new Size(573, 20);
			this.thaoLuanTextBox.TabIndex = 4;
			this.thaoLuanTextBox.TextChanged += new EventHandler(this.LineOneTextBoxTextChanged);
			this.label1.ForeColor = SystemColors.ControlDarkDark;
			this.label1.Location = new Point(6, 45);
			this.label1.Name = "label1";
			this.label1.Size = new Size(573, 22);
			this.label1.TabIndex = 3;
			this.label1.Text = "Ví dụ: Mãng Hoang Kỷ là http://www.tangthuvien.com/forum/showthread.php?t=95472";
			this.copyToClipboardButton.Location = new Point(220, 563);
			this.copyToClipboardButton.Name = "copyToClipboardButton";
			this.copyToClipboardButton.Size = new Size(179, 34);
			this.copyToClipboardButton.TabIndex = 5;
			this.copyToClipboardButton.Text = "&Chép vào Clipboard và Đóng";
			this.copyToClipboardButton.UseVisualStyleBackColor = true;
			this.copyToClipboardButton.Click += new EventHandler(this.CopyToClipboardButtonClick);
			this.postContentRichTextBox.Location = new Point(10, 348);
			this.postContentRichTextBox.Name = "postContentRichTextBox";
			this.postContentRichTextBox.ReadOnly = true;
			this.postContentRichTextBox.ScrollBars = RichTextBoxScrollBars.Vertical;
			this.postContentRichTextBox.Size = new Size(594, 200);
			this.postContentRichTextBox.TabIndex = 7;
			this.postContentRichTextBox.Text = "";
			this.label2.Location = new Point(10, 322);
			this.label2.Name = "label2";
			this.label2.Size = new Size(594, 23);
			this.label2.TabIndex = 8;
			this.label2.Text = "/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////";
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(616, 609);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.postContentRichTextBox);
			base.Controls.Add(this.copyToClipboardButton);
			base.Controls.Add(this.groupBox4);
			base.Controls.Add(this.groupBox5);
			base.Controls.Add(this.groupBox2);
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "PostTTVForm";
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Post to TangThuVien.com";
			this.groupBox2.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.groupBox5.ResumeLayout(false);
			this.groupBox5.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
