using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TranslatorEngine;

namespace QuickTranslator
{
	public class UpdatePhienAmForm : Form
	{
		private IContainer components;

		private Label phienAmLabel;

		private Button updateButton;

		private Button cancelButton;

		private Button deleteButton;

		private CheckBox sortingCheckBox;

		private TextBox phienAmTextBox;

		private Label label1;

		private Label label3;

		private TextBox chineseTextBox;

		private Label entryCountLabel;

		public UpdatePhienAmForm()
		{
			this.InitializeComponent();
		}

		public UpdatePhienAmForm(string chineseToLookup)
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
			this.entryCountLabel.Text = TranslatorEngine.TranslatorEngine.GetPhienAmDictionaryCount().ToString();
			base.ActiveControl = this.phienAmTextBox;
		}

		private void ChineseTextBoxTextChanged(object sender, EventArgs e)
		{
			this.updateButton.Enabled = !string.IsNullOrEmpty(this.chineseTextBox.Text.Trim());
			this.deleteButton.Enabled = !string.IsNullOrEmpty(this.chineseTextBox.Text.Trim());
			if (string.IsNullOrEmpty(this.chineseTextBox.Text.Trim()))
			{
				this.phienAmTextBox.Text = "";
				return;
			}
			CharRange[] array;
			this.phienAmTextBox.Text = TranslatorEngine.TranslatorEngine.ChineseToHanViet(this.chineseTextBox.Text, out array).Trim();
			if (1 != this.chineseTextBox.Text.Length)
			{
				this.deleteButton.Enabled = false;
				this.updateButton.Enabled = false;
				return;
			}
			bool flag = TranslatorEngine.TranslatorEngine.ExistInPhienAmDictionary(this.chineseTextBox.Text);
			this.deleteButton.Enabled = flag;
			this.updateButton.Enabled = true;
			this.updateButton.Text = (flag ? "Update" : "Add");
		}

		private void UpdateButtonClick(object sender, EventArgs e)
		{
			if (this.chineseTextBox.Text.Length != 1)
			{
				return;
			}
			if (string.IsNullOrEmpty(this.phienAmTextBox.Text))
			{
				return;
			}
			TranslatorEngine.TranslatorEngine.UpdatePhienAmDictionary(this.chineseTextBox.Text, this.phienAmTextBox.Text, this.sortingCheckBox.Checked);
			base.Close();
		}

		private void DeleteButtonClick(object sender, EventArgs e)
		{
			TranslatorEngine.TranslatorEngine.DeleteKeyFromPhienAmDictionary(this.chineseTextBox.Text, this.sortingCheckBox.Checked);
			base.Close();
		}

		private void CancelButtonClick(object sender, EventArgs e)
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UpdatePhienAmForm));
			this.entryCountLabel = new Label();
			this.phienAmTextBox = new TextBox();
			this.chineseTextBox = new TextBox();
			this.phienAmLabel = new Label();
			this.label3 = new Label();
			this.label1 = new Label();
			this.sortingCheckBox = new CheckBox();
			this.deleteButton = new Button();
			this.cancelButton = new Button();
			this.updateButton = new Button();
			base.SuspendLayout();
			this.entryCountLabel.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.entryCountLabel.Location = new Point(80, -1);
			this.entryCountLabel.Name = "entryCountLabel";
			this.entryCountLabel.Size = new Size(160, 23);
			this.entryCountLabel.TabIndex = 14;
			this.entryCountLabel.Text = "label4";
			this.entryCountLabel.TextAlign = ContentAlignment.MiddleLeft;
			this.phienAmTextBox.Font = new Font("Arial Unicode MS", 12f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.phienAmTextBox.Location = new Point(80, 64);
			this.phienAmTextBox.Name = "phienAmTextBox";
			this.phienAmTextBox.Size = new Size(176, 29);
			this.phienAmTextBox.TabIndex = 2;
			this.chineseTextBox.Font = new Font("Arial Unicode MS", 14.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.chineseTextBox.Location = new Point(80, 25);
			this.chineseTextBox.Name = "chineseTextBox";
			this.chineseTextBox.Size = new Size(176, 33);
			this.chineseTextBox.TabIndex = 1;
			this.chineseTextBox.TextChanged += new EventHandler(this.ChineseTextBoxTextChanged);
			this.phienAmLabel.Location = new Point(0, 72);
			this.phienAmLabel.Name = "phienAmLabel";
			this.phienAmLabel.Size = new Size(73, 18);
			this.phienAmLabel.TabIndex = 9;
			this.phienAmLabel.Text = "Phiên Âm:";
			this.phienAmLabel.TextAlign = ContentAlignment.MiddleRight;
			this.label3.Location = new Point(1, 1);
			this.label3.Name = "label3";
			this.label3.Size = new Size(73, 18);
			this.label3.TabIndex = 8;
			this.label3.Text = "Entries:";
			this.label3.TextAlign = ContentAlignment.MiddleRight;
			this.label1.Location = new Point(1, 32);
			this.label1.Name = "label1";
			this.label1.Size = new Size(73, 18);
			this.label1.TabIndex = 10;
			this.label1.Text = "Chinese:";
			this.label1.TextAlign = ContentAlignment.MiddleRight;
			this.sortingCheckBox.Location = new Point(80, 104);
			this.sortingCheckBox.Name = "sortingCheckBox";
			this.sortingCheckBox.Size = new Size(176, 24);
			this.sortingCheckBox.TabIndex = 3;
			this.sortingCheckBox.Text = "Sắp xếp lại dữ liệu từ điển";
			this.sortingCheckBox.UseVisualStyleBackColor = true;
			this.deleteButton.Location = new Point(96, 136);
			this.deleteButton.Name = "deleteButton";
			this.deleteButton.Size = new Size(84, 23);
			this.deleteButton.TabIndex = 5;
			this.deleteButton.Text = "Delete";
			this.deleteButton.UseVisualStyleBackColor = true;
			this.deleteButton.Click += new EventHandler(this.DeleteButtonClick);
			this.cancelButton.DialogResult = DialogResult.Cancel;
			this.cancelButton.Location = new Point(184, 136);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new Size(84, 23);
			this.cancelButton.TabIndex = 6;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new EventHandler(this.CancelButtonClick);
			this.updateButton.Location = new Point(8, 136);
			this.updateButton.Name = "updateButton";
			this.updateButton.Size = new Size(84, 23);
			this.updateButton.TabIndex = 4;
			this.updateButton.Text = "Update or Add";
			this.updateButton.UseVisualStyleBackColor = true;
			this.updateButton.Click += new EventHandler(this.UpdateButtonClick);
			base.AcceptButton = this.updateButton;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.CancelButton = this.cancelButton;
			base.ClientSize = new Size(277, 171);
			base.Controls.Add(this.sortingCheckBox);
			base.Controls.Add(this.deleteButton);
			base.Controls.Add(this.cancelButton);
			base.Controls.Add(this.updateButton);
			base.Controls.Add(this.entryCountLabel);
			base.Controls.Add(this.phienAmTextBox);
			base.Controls.Add(this.chineseTextBox);
			base.Controls.Add(this.phienAmLabel);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.label1);
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "UpdatePhienAmForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Update Phiên Âm";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
