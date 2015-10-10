//using QuickConverter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace QuickTranslator
{
	public class ExportToWordForm : Form
	{
		private string chinese;

		private string hanViet;

		private string vietPhrase;

		private string vietPhraseOneMeaning;

		private string viet;

		private IContainer components;

		private CheckBox insertBlankLineCheckBox;

		private Button closeButton;

		private Label label1;

		private ComboBox comboBox2;

		private ComboBox comboBox3;

		private ComboBox comboBox4;

		private ComboBox comboBox5;

		private Button exportButton;

		private ComboBox comboBox1;

		private Label label2;

		private Label label3;

		private Label label4;

		private Label label5;

		private GroupBox exportFormatGroupBox;

		public ExportToWordForm()
		{
			this.InitializeComponent();
		}

		public ExportToWordForm(string chinese, string hanViet, string vietPhrase, string vietPhraseOneMeaning, string viet)
		{
			this.InitializeComponent();
			this.chinese = chinese;
			this.hanViet = hanViet;
			this.vietPhrase = vietPhrase;
			this.vietPhraseOneMeaning = vietPhraseOneMeaning;
			this.viet = viet.Replace("\n\n", "\n").Replace("\n", "\n\n");
		}

		private void ExportToWordFormLoad(object sender, EventArgs e)
		{
			this.PopulateControls();
			this.exportButton.Focus();
		}

		public void PopulateControls()
		{
			this.populateComboBox(this.comboBox1);
			this.comboBox1.SelectedItem = "Trung";
			this.populateComboBox(this.comboBox2);
			this.comboBox2.SelectedItem = "Hán Việt";
			this.populateComboBox(this.comboBox3);
			this.comboBox3.SelectedItem = "VietPhrase";
			this.populateComboBox(this.comboBox4);
			this.comboBox4.SelectedItem = "VietPhrase một nghĩa";
			this.populateComboBox(this.comboBox5);
			this.comboBox5.SelectedItem = "Việt";
		}

		private void populateComboBox(ComboBox comboBox)
		{
			comboBox.Items.AddRange(new string[]
			{
				"<None>",
				"Trung",
				"Hán Việt",
				"VietPhrase",
				"VietPhrase một nghĩa",
				"Việt"
			});
		}

		private void ExportButtonClick(object sender, EventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.CheckFileExists = false;
			saveFileDialog.Filter = "Microsoft Word (*.doc)|*.doc";
			DialogResult dialogResult = saveFileDialog.ShowDialog();
			if (dialogResult != DialogResult.OK)
			{
				return;
			}
			string fileName = saveFileDialog.FileName;
			this.ExportToWord(fileName);
			base.Close();
		}

		public void ExportToWord(string exportFilePath)
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			this.AnalyzeColumn(this.comboBox1, list, list2);
			this.AnalyzeColumn(this.comboBox2, list, list2);
			this.AnalyzeColumn(this.comboBox3, list, list2);
			this.AnalyzeColumn(this.comboBox4, list, list2);
			this.AnalyzeColumn(this.comboBox5, list, list2);
//			ColumnExporter.Export(list.ToArray(), list2.ToArray(), !this.insertBlankLineCheckBox.Checked, exportFilePath);
		}

		private void AnalyzeColumn(ComboBox comboBox, List<string> columnNameList, List<string> columnContentList)
		{
			if (comboBox.SelectedIndex == 1)
			{
				columnNameList.Add("Trung");
				columnContentList.Add(this.chinese);
				return;
			}
			if (comboBox.SelectedIndex == 2)
			{
				columnNameList.Add("Hán Việt");
				columnContentList.Add(this.hanViet);
				return;
			}
			if (comboBox.SelectedIndex == 3)
			{
				columnNameList.Add("VietPhrase");
				columnContentList.Add(this.vietPhrase);
				return;
			}
			if (comboBox.SelectedIndex == 4)
			{
				columnNameList.Add("VietPhrase một nghĩa");
				columnContentList.Add(this.vietPhraseOneMeaning);
				return;
			}
			if (comboBox.SelectedIndex == 5)
			{
				columnNameList.Add("Việt");
				columnContentList.Add(this.viet);
			}
		}

		private void CloseButtonClick(object sender, EventArgs e)
		{
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ExportToWordForm));
			this.exportFormatGroupBox = new GroupBox();
			this.insertBlankLineCheckBox = new CheckBox();
			this.label5 = new Label();
			this.label4 = new Label();
			this.label3 = new Label();
			this.label2 = new Label();
			this.comboBox5 = new ComboBox();
			this.comboBox4 = new ComboBox();
			this.comboBox3 = new ComboBox();
			this.comboBox2 = new ComboBox();
			this.comboBox1 = new ComboBox();
			this.label1 = new Label();
			this.exportButton = new Button();
			this.closeButton = new Button();
			this.exportFormatGroupBox.SuspendLayout();
			base.SuspendLayout();
			this.exportFormatGroupBox.Controls.Add(this.insertBlankLineCheckBox);
			this.exportFormatGroupBox.Controls.Add(this.label5);
			this.exportFormatGroupBox.Controls.Add(this.label4);
			this.exportFormatGroupBox.Controls.Add(this.label3);
			this.exportFormatGroupBox.Controls.Add(this.label2);
			this.exportFormatGroupBox.Controls.Add(this.comboBox5);
			this.exportFormatGroupBox.Controls.Add(this.comboBox4);
			this.exportFormatGroupBox.Controls.Add(this.comboBox3);
			this.exportFormatGroupBox.Controls.Add(this.comboBox2);
			this.exportFormatGroupBox.Controls.Add(this.comboBox1);
			this.exportFormatGroupBox.Controls.Add(this.label1);
			this.exportFormatGroupBox.Location = new Point(12, 12);
			this.exportFormatGroupBox.Name = "exportFormatGroupBox";
			this.exportFormatGroupBox.Size = new Size(444, 188);
			this.exportFormatGroupBox.TabIndex = 0;
			this.exportFormatGroupBox.TabStop = false;
			this.exportFormatGroupBox.Text = "Export Format";
			this.insertBlankLineCheckBox.Checked = true;
			this.insertBlankLineCheckBox.CheckState = CheckState.Checked;
			this.insertBlankLineCheckBox.Location = new Point(184, 152);
			this.insertBlankLineCheckBox.Name = "insertBlankLineCheckBox";
			this.insertBlankLineCheckBox.Size = new Size(217, 24);
			this.insertBlankLineCheckBox.TabIndex = 6;
			this.insertBlankLineCheckBox.Text = "Chèn thêm dòng trắng giữa các dòng";
			this.insertBlankLineCheckBox.UseVisualStyleBackColor = true;
			this.label5.Location = new Point(99, 129);
			this.label5.Name = "label5";
			this.label5.Size = new Size(67, 17);
			this.label5.TabIndex = 0;
			this.label5.Text = "Column 5:";
			this.label5.TextAlign = ContentAlignment.MiddleRight;
			this.label4.Location = new Point(99, 103);
			this.label4.Name = "label4";
			this.label4.Size = new Size(67, 17);
			this.label4.TabIndex = 0;
			this.label4.Text = "Column 4:";
			this.label4.TextAlign = ContentAlignment.MiddleRight;
			this.label3.Location = new Point(98, 76);
			this.label3.Name = "label3";
			this.label3.Size = new Size(67, 17);
			this.label3.TabIndex = 0;
			this.label3.Text = "Column 3:";
			this.label3.TextAlign = ContentAlignment.MiddleRight;
			this.label2.Location = new Point(99, 49);
			this.label2.Name = "label2";
			this.label2.Size = new Size(67, 17);
			this.label2.TabIndex = 0;
			this.label2.Text = "Column 2:";
			this.label2.TextAlign = ContentAlignment.MiddleRight;
			this.comboBox5.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBox5.FormattingEnabled = true;
			this.comboBox5.Location = new Point(184, 127);
			this.comboBox5.Name = "comboBox5";
			this.comboBox5.Size = new Size(192, 21);
			this.comboBox5.TabIndex = 5;
			this.comboBox4.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBox4.FormattingEnabled = true;
			this.comboBox4.Location = new Point(184, 101);
			this.comboBox4.Name = "comboBox4";
			this.comboBox4.Size = new Size(192, 21);
			this.comboBox4.TabIndex = 4;
			this.comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBox3.FormattingEnabled = true;
			this.comboBox3.Location = new Point(184, 74);
			this.comboBox3.Name = "comboBox3";
			this.comboBox3.Size = new Size(192, 21);
			this.comboBox3.TabIndex = 3;
			this.comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBox2.FormattingEnabled = true;
			this.comboBox2.Location = new Point(184, 47);
			this.comboBox2.Name = "comboBox2";
			this.comboBox2.Size = new Size(192, 21);
			this.comboBox2.TabIndex = 2;
			this.comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Location = new Point(184, 20);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new Size(192, 21);
			this.comboBox1.TabIndex = 1;
			this.label1.Location = new Point(99, 22);
			this.label1.Name = "label1";
			this.label1.Size = new Size(67, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "Column 1:";
			this.label1.TextAlign = ContentAlignment.MiddleRight;
			this.exportButton.Location = new Point(144, 216);
			this.exportButton.Name = "exportButton";
			this.exportButton.Size = new Size(75, 24);
			this.exportButton.TabIndex = 6;
			this.exportButton.Text = "Export";
			this.exportButton.UseVisualStyleBackColor = true;
			this.exportButton.Click += new EventHandler(this.ExportButtonClick);
			this.closeButton.DialogResult = DialogResult.Cancel;
			this.closeButton.Location = new Point(248, 216);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new Size(79, 23);
			this.closeButton.TabIndex = 7;
			this.closeButton.Text = "Close";
			this.closeButton.UseVisualStyleBackColor = true;
			this.closeButton.Click += new EventHandler(this.CloseButtonClick);
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.CancelButton = this.closeButton;
			base.ClientSize = new Size(470, 247);
			base.Controls.Add(this.closeButton);
			base.Controls.Add(this.exportButton);
			base.Controls.Add(this.exportFormatGroupBox);
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ExportToWordForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Export To Word";
			base.Load += new EventHandler(this.ExportToWordFormLoad);
			this.exportFormatGroupBox.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
