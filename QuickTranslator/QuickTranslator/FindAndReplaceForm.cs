using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace QuickTranslator
{
	public class FindAndReplaceForm : Form
	{
		private RichTextBox richTextBox;

		private int lastFoundIndex = -1;

		private IContainer components;

		private RadioButton upRadioButton;

		private RadioButton downRadioButton;

		private GroupBox groupBox1;

		private CheckBox matchWholeWordCheckBox;

		private CheckBox matchCaseCheckBox;

		private Button replaceAllButton;

		private Button replaceButton;

		private Button findNextButton;

		private TextBox replaceWithTextBox;

		private Label label2;

		private TextBox findTextBox;

		private Label label1;

		public FindAndReplaceForm(RichTextBox richTextBox)
		{
			this.InitializeComponent();
			this.richTextBox = richTextBox;
			this.findTextBox.Text = richTextBox.SelectedText;
		}

		private void FindTextBoxTextChanged(object sender, EventArgs e)
		{
			bool enabled = 0 < this.findTextBox.Text.Length;
			this.findNextButton.Enabled = enabled;
			this.replaceButton.Enabled = enabled;
			this.replaceAllButton.Enabled = enabled;
			this.lastFoundIndex = -1;
		}

		private void FindNextButtonClick(object sender, EventArgs e)
		{
			this.Find();
		}

		private void Find()
		{
			RichTextBoxFinds richTextBoxFinds = RichTextBoxFinds.None;
			if (this.matchCaseCheckBox.Checked)
			{
				richTextBoxFinds |= RichTextBoxFinds.MatchCase;
			}
			if (this.matchWholeWordCheckBox.Checked)
			{
				richTextBoxFinds |= RichTextBoxFinds.WholeWord;
			}
			if (this.upRadioButton.Checked)
			{
				richTextBoxFinds |= RichTextBoxFinds.Reverse;
			}
			if (this.upRadioButton.Checked)
			{
				this.lastFoundIndex = this.richTextBox.Find(this.findTextBox.Text, 0, this.richTextBox.SelectionStart, richTextBoxFinds);
			}
			else
			{
				this.lastFoundIndex = this.richTextBox.Find(this.findTextBox.Text, this.richTextBox.SelectionStart + this.richTextBox.SelectionLength, richTextBoxFinds);
			}
			if (0 <= this.lastFoundIndex)
			{
				this.richTextBox.SelectionStart = this.lastFoundIndex;
				this.richTextBox.SelectionLength = this.findTextBox.Text.Length;
			}
			this.replaceButton.Enabled = (0 <= this.lastFoundIndex);
		}

		private void ReplaceButtonClick(object sender, EventArgs e)
		{
			if (this.richTextBox.SelectionStart == this.lastFoundIndex)
			{
				this.richTextBox.SelectedText = this.replaceWithTextBox.Text;
			}
			this.Find();
		}

		private void ReplaceAllButtonClick(object sender, EventArgs e)
		{
			this.richTextBox.Text = this.richTextBox.Text.Replace(this.findTextBox.Text, this.replaceWithTextBox.Text);
			this.lastFoundIndex = -1;
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (e.KeyCode == Keys.Escape)
			{
				base.Close();
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(FindAndReplaceForm));
			this.label1 = new Label();
			this.findTextBox = new TextBox();
			this.label2 = new Label();
			this.replaceWithTextBox = new TextBox();
			this.findNextButton = new Button();
			this.replaceButton = new Button();
			this.replaceAllButton = new Button();
			this.matchCaseCheckBox = new CheckBox();
			this.matchWholeWordCheckBox = new CheckBox();
			this.groupBox1 = new GroupBox();
			this.downRadioButton = new RadioButton();
			this.upRadioButton = new RadioButton();
			this.groupBox1.SuspendLayout();
			base.SuspendLayout();
			this.label1.Location = new Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new Size(75, 22);
			this.label1.TabIndex = 0;
			this.label1.Text = "Find:";
			this.findTextBox.Location = new Point(93, 6);
			this.findTextBox.Name = "findTextBox";
			this.findTextBox.Size = new Size(276, 20);
			this.findTextBox.TabIndex = 0;
			this.findTextBox.TextChanged += new EventHandler(this.FindTextBoxTextChanged);
			this.label2.Location = new Point(12, 40);
			this.label2.Name = "label2";
			this.label2.Size = new Size(75, 22);
			this.label2.TabIndex = 0;
			this.label2.Text = "Replace With:";
			this.replaceWithTextBox.Location = new Point(93, 37);
			this.replaceWithTextBox.Name = "replaceWithTextBox";
			this.replaceWithTextBox.Size = new Size(276, 20);
			this.replaceWithTextBox.TabIndex = 2;
			this.findNextButton.Enabled = false;
			this.findNextButton.Location = new Point(385, 4);
			this.findNextButton.Name = "findNextButton";
			this.findNextButton.Size = new Size(75, 23);
			this.findNextButton.TabIndex = 1;
			this.findNextButton.Text = "Find Next";
			this.findNextButton.UseVisualStyleBackColor = true;
			this.findNextButton.Click += new EventHandler(this.FindNextButtonClick);
			this.replaceButton.Enabled = false;
			this.replaceButton.Location = new Point(385, 35);
			this.replaceButton.Name = "replaceButton";
			this.replaceButton.Size = new Size(75, 23);
			this.replaceButton.TabIndex = 3;
			this.replaceButton.Text = "Replace";
			this.replaceButton.UseVisualStyleBackColor = true;
			this.replaceButton.Click += new EventHandler(this.ReplaceButtonClick);
			this.replaceAllButton.Enabled = false;
			this.replaceAllButton.Location = new Point(385, 64);
			this.replaceAllButton.Name = "replaceAllButton";
			this.replaceAllButton.Size = new Size(75, 23);
			this.replaceAllButton.TabIndex = 4;
			this.replaceAllButton.Text = "Replace All";
			this.replaceAllButton.UseVisualStyleBackColor = true;
			this.replaceAllButton.Click += new EventHandler(this.ReplaceAllButtonClick);
			this.matchCaseCheckBox.Location = new Point(12, 106);
			this.matchCaseCheckBox.Name = "matchCaseCheckBox";
			this.matchCaseCheckBox.Size = new Size(104, 24);
			this.matchCaseCheckBox.TabIndex = 5;
			this.matchCaseCheckBox.Text = "Match Case";
			this.matchCaseCheckBox.UseVisualStyleBackColor = true;
			this.matchWholeWordCheckBox.Location = new Point(12, 136);
			this.matchWholeWordCheckBox.Name = "matchWholeWordCheckBox";
			this.matchWholeWordCheckBox.Size = new Size(150, 24);
			this.matchWholeWordCheckBox.TabIndex = 6;
			this.matchWholeWordCheckBox.Text = "Match Whole Word";
			this.matchWholeWordCheckBox.UseVisualStyleBackColor = true;
			this.groupBox1.Controls.Add(this.downRadioButton);
			this.groupBox1.Controls.Add(this.upRadioButton);
			this.groupBox1.Location = new Point(169, 106);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new Size(200, 54);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Direction";
			this.downRadioButton.Checked = true;
			this.downRadioButton.Location = new Point(110, 19);
			this.downRadioButton.Name = "downRadioButton";
			this.downRadioButton.Size = new Size(84, 27);
			this.downRadioButton.TabIndex = 1;
			this.downRadioButton.TabStop = true;
			this.downRadioButton.Text = "Down";
			this.downRadioButton.UseVisualStyleBackColor = true;
			this.upRadioButton.Location = new Point(13, 19);
			this.upRadioButton.Name = "upRadioButton";
			this.upRadioButton.Size = new Size(73, 27);
			this.upRadioButton.TabIndex = 0;
			this.upRadioButton.Text = "Up";
			this.upRadioButton.UseVisualStyleBackColor = true;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(470, 169);
			base.Controls.Add(this.groupBox1);
			base.Controls.Add(this.matchWholeWordCheckBox);
			base.Controls.Add(this.matchCaseCheckBox);
			base.Controls.Add(this.replaceAllButton);
			base.Controls.Add(this.replaceButton);
			base.Controls.Add(this.findNextButton);
			base.Controls.Add(this.replaceWithTextBox);
			base.Controls.Add(this.findTextBox);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.KeyPreview = true;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "FindAndReplaceForm";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Find and Replace (ô Trung)";
			this.groupBox1.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
