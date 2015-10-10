using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace QuickTranslator
{
	public class ConfigurationPanel : DockContent
	{
		private Configuration configuration;

		private string configFilePath;

		private bool needToRetranslate;

		private bool needToResetPanelStyle;

		private IContainer components;

		private CheckBox prioritizedNameCheckBox;

		private GroupBox groupBox6;

		private CheckBox alwaysFocusInVietCheckBox;

		private GroupBox groupBox7;

		private RadioButton algorithm_LongestVietPhraseFirstWithConditionRadioButton;

		private RadioButton algorithm_LongestVietPhraseFirstRadioButton;

		private RadioButton algorithm_LeftToRightRadioButton;

		private GroupBox groupBox5;

		private Button nghiaBackColorButton;

		private Button nghiaFontButton;

		private Label nghiaLabel;

		private Label vietLabel;

		private Label vietPhraseOneMeaningLabel;

		private Label vietPhraseLabel;

		private Label hanVietLabel;

		private Button vietBackColorButton;

		private Button vietPhraseOneMeaningBackColorButton;

		private Button vietPhraseBackColorButton;

		private Button hanVietBackColorButton;

		private Button trungBackColorButton;

		private Button vietFontButton;

		private Button vietPhraseOneMeaningFontButton;

		private Button vietPhraseFontButton;

		private Button hanVietFontButton;

		private FontDialog fontDialog;

		private ColorDialog colorDialog;

		private Label trungLabel;

		private Button trungFontButton;

		private TabPage tabPage2;

		private TabPage tabPage1;

		private TabControl translateTabPage;

		private TextBox hotKeys_CopyMeaning4TextBox;

		private TextBox hotKeys_MoveRightOneWordTextBox;

		private TextBox hotKeys_CopyMeaning6TextBox;

		private Label label14;

		private Label label13;

		private TextBox hotKeys_CopyMeaning1TextBox;

		private Label label16;

		private Label label15;

		private TextBox hotKeys_CopyMeaning2TextBox;

		private Label label18;

		private Label label17;

		private TextBox hotKeys_CopyMeaning3TextBox;

		private Label label20;

		private Label label19;

		private Label label22;

		private Label label21;

		private TextBox hotKeys_CopyMeaning5TextBox;

		private Label label24;

		private Label label23;

		private Label label8;

		private Label label7;

		private TextBox hotKeys_MoveUpOneLineTextBox;

		private Label label10;

		private Label label9;

		private TextBox hotKeys_MoveDownOneParagraphTextBox;

		private Label label12;

		private Label label11;

		private TextBox hotKeys_MoveUpOneParagraphTextBox;

		private TextBox hotKeys_MoveDownOneLineTextBox;

		private Label label4;

		private Label label3;

		private TextBox hotKeys_MoveLeftOneWordTextBox;

		private Label label6;

		private Label label5;

		private Label label1;

		private Label label2;

		private GroupBox groupBox4;

		private RadioButton vietPhraseOneMeaning_AlwaysWrapRadioButton;

		private RadioButton vietPhraseOneMeaning_NoWrapRadioButton;

		private GroupBox groupBox3;

		private RadioButton vietPhrase_NoWrapRadioButton;

		private RadioButton vietPhrase_OnlyWrapTwoMeaningRadioButton;

		private RadioButton vietPhrase_AlwaysWrapRadioButton;

		private GroupBox groupBox2;

		private CheckBox browserPanel_DisablePopupsCheckBox;

		private CheckBox browserPanel_DisableImagesCheckBox;

		private Button cancelButton;

		private Button saveButton;

		private GroupBox groupBox1;

		private Label label26;

		private Label label25;

		private TextBox f9TextBox;

		private TextBox f7TextBox;

		private TextBox f5TextBox;

		private TextBox f3TextBox;

		private TextBox f8TextBox;

		private TextBox f6TextBox;

		private TextBox f4TextBox;

		private TextBox f2TextBox;

		private TextBox f1TextBox;

		private Label label33;

		private Label label32;

		private Label label31;

		private Label label30;

		private Label label29;

		private Label label28;

		private Label label27;

		private RadioButton vietPhrase_AlwaysWrapExceptHanVietRadioButton;

		private RadioButton vietPhraseOneMeaning_AlwaysWrapExceptHanVietRadioButton;

		private TextBox hotKeys_CopyHighlightedHanVietTextBox;

		private Label label35;

		private Label label34;

		public ConfigurationPanel(string configFilePath)
		{
			this.InitializeComponent();
			base.DockHandler.GetPersistStringCallback = new GetPersistStringCallback(this.GetPersistStringFromText);
			this.configFilePath = configFilePath;
			this.configuration = Configuration.LoadFromFile(configFilePath);
		}

		public void ReloadConfiguration()
		{
			this.configuration = Configuration.LoadFromFile(this.configFilePath);
			this.configurationToGui();
		}

		public string GetPersistStringFromText()
		{
			return this.Text;
		}

		private void SaveButtonClick(object sender, EventArgs e)
		{
			this.guiToConfiguration();
			this.configuration.SaveToFile(this.configFilePath);
			MainForm.ActiveConfiguration = Configuration.LoadFromFile(this.configFilePath);
			if (this.needToRetranslate && base.ParentForm is MainForm)
			{
				((MainForm)base.ParentForm).Retranslate();
				this.needToRetranslate = false;
			}
			if (this.needToResetPanelStyle && base.ParentForm is MainForm)
			{
				((MainForm)base.ParentForm).SetPanelStyle();
				this.needToResetPanelStyle = false;
			}
			this.UndoDockState();
		}

		private void CancelButtonClick(object sender, EventArgs e)
		{
			this.configurationToGui();
			this.UndoDockState();
		}

		private void configurationToGui()
		{
			this.browserPanel_DisableImagesCheckBox.Checked = this.configuration.BrowserPanel_DisableImages;
			this.browserPanel_DisablePopupsCheckBox.Checked = this.configuration.BrowserPanel_DisablePopups;
			this.vietPhrase_NoWrapRadioButton.Checked = (this.configuration.VietPhrase_Wrap == 0);
			this.vietPhrase_AlwaysWrapRadioButton.Checked = (this.configuration.VietPhrase_Wrap == 1);
			this.vietPhrase_AlwaysWrapExceptHanVietRadioButton.Checked = (this.configuration.VietPhrase_Wrap == 11);
			this.vietPhrase_OnlyWrapTwoMeaningRadioButton.Checked = (this.configuration.VietPhrase_Wrap == 2);
			this.vietPhraseOneMeaning_NoWrapRadioButton.Checked = (this.configuration.VietPhraseOneMeaning_Wrap == 0);
			this.vietPhraseOneMeaning_AlwaysWrapRadioButton.Checked = (this.configuration.VietPhraseOneMeaning_Wrap == 1);
			this.vietPhraseOneMeaning_AlwaysWrapExceptHanVietRadioButton.Checked = (this.configuration.VietPhraseOneMeaning_Wrap == 11);
			this.hotKeys_CopyHighlightedHanVietTextBox.Text = this.configuration.HotKeys_CopyHighlightedHanViet.ToString();
			this.hotKeys_CopyMeaning1TextBox.Text = this.configuration.HotKeys_CopyMeaning1.ToString();
			this.hotKeys_CopyMeaning2TextBox.Text = this.configuration.HotKeys_CopyMeaning2.ToString();
			this.hotKeys_CopyMeaning3TextBox.Text = this.configuration.HotKeys_CopyMeaning3.ToString();
			this.hotKeys_CopyMeaning4TextBox.Text = this.configuration.HotKeys_CopyMeaning4.ToString();
			this.hotKeys_CopyMeaning5TextBox.Text = this.configuration.HotKeys_CopyMeaning5.ToString();
			this.hotKeys_CopyMeaning6TextBox.Text = this.configuration.HotKeys_CopyMeaning6.ToString();
			this.hotKeys_MoveDownOneLineTextBox.Text = this.configuration.HotKeys_MoveDownOneLine.ToString();
			this.hotKeys_MoveDownOneParagraphTextBox.Text = this.configuration.HotKeys_MoveDownOneParagraph.ToString();
			this.hotKeys_MoveLeftOneWordTextBox.Text = this.configuration.HotKeys_MoveLeftOneWord.ToString();
			this.hotKeys_MoveRightOneWordTextBox.Text = this.configuration.HotKeys_MoveRightOneWord.ToString();
			this.hotKeys_MoveUpOneLineTextBox.Text = this.configuration.HotKeys_MoveUpOneLine.ToString();
			this.hotKeys_MoveUpOneParagraphTextBox.Text = this.configuration.HotKeys_MoveUpOneParagraph.ToString();
			this.f1TextBox.Text = this.configuration.HotKeys_F1;
			this.f2TextBox.Text = this.configuration.HotKeys_F2;
			this.f3TextBox.Text = this.configuration.HotKeys_F3;
			this.f4TextBox.Text = this.configuration.HotKeys_F4;
			this.f5TextBox.Text = this.configuration.HotKeys_F5;
			this.f6TextBox.Text = this.configuration.HotKeys_F6;
			this.f7TextBox.Text = this.configuration.HotKeys_F7;
			this.f8TextBox.Text = this.configuration.HotKeys_F8;
			this.f9TextBox.Text = this.configuration.HotKeys_F9;
			this.trungLabel.Font = this.configuration.Style_TrungFont;
			this.trungLabel.ForeColor = this.configuration.Style_TrungForeColor;
			this.trungLabel.BackColor = this.configuration.Style_TrungBackColor;
			this.hanVietLabel.Font = this.configuration.Style_HanVietFont;
			this.hanVietLabel.ForeColor = this.configuration.Style_HanVietForeColor;
			this.hanVietLabel.BackColor = this.configuration.Style_HanVietBackColor;
			this.vietPhraseLabel.Font = this.configuration.Style_VietPhraseFont;
			this.vietPhraseLabel.ForeColor = this.configuration.Style_VietPhraseForeColor;
			this.vietPhraseLabel.BackColor = this.configuration.Style_VietPhraseBackColor;
			this.vietPhraseOneMeaningLabel.Font = this.configuration.Style_VietPhraseOneMeaningFont;
			this.vietPhraseOneMeaningLabel.ForeColor = this.configuration.Style_VietPhraseOneMeaningForeColor;
			this.vietPhraseOneMeaningLabel.BackColor = this.configuration.Style_VietPhraseOneMeaningBackColor;
			this.vietLabel.Font = this.configuration.Style_VietFont;
			this.vietLabel.ForeColor = this.configuration.Style_VietForeColor;
			this.vietLabel.BackColor = this.configuration.Style_VietBackColor;
			this.nghiaLabel.Font = this.configuration.Style_NghiaFont;
			this.nghiaLabel.ForeColor = this.configuration.Style_NghiaForeColor;
			this.nghiaLabel.BackColor = this.configuration.Style_NghiaBackColor;
			this.algorithm_LongestVietPhraseFirstRadioButton.Checked = (this.configuration.TranslationAlgorithm == 0);
			this.algorithm_LeftToRightRadioButton.Checked = (this.configuration.TranslationAlgorithm == 1);
			this.algorithm_LongestVietPhraseFirstWithConditionRadioButton.Checked = (this.configuration.TranslationAlgorithm == 2);
			this.prioritizedNameCheckBox.Checked = this.configuration.PrioritizedName;
			this.alwaysFocusInVietCheckBox.Checked = this.configuration.AlwaysFocusInViet;
			this.needToRetranslate = false;
		}

		private void guiToConfiguration()
		{
			this.configuration.BrowserPanel_DisableImages = this.browserPanel_DisableImagesCheckBox.Checked;
			this.configuration.BrowserPanel_DisablePopups = this.browserPanel_DisablePopupsCheckBox.Checked;
			this.configuration.VietPhrase_Wrap = (this.vietPhrase_NoWrapRadioButton.Checked ? 0 : (this.vietPhrase_AlwaysWrapRadioButton.Checked ? 1 : (this.vietPhrase_AlwaysWrapExceptHanVietRadioButton.Checked ? 11 : 2)));
			this.configuration.VietPhraseOneMeaning_Wrap = (this.vietPhraseOneMeaning_NoWrapRadioButton.Checked ? 0 : (this.vietPhraseOneMeaning_AlwaysWrapRadioButton.Checked ? 1 : 11));
			this.configuration.HotKeys_CopyHighlightedHanViet = ((this.hotKeys_CopyHighlightedHanVietTextBox.Text.Length == 0) ? '0' : this.hotKeys_CopyHighlightedHanVietTextBox.Text[0]);
			this.configuration.HotKeys_CopyMeaning1 = ((this.hotKeys_CopyMeaning1TextBox.Text.Length == 0) ? '1' : this.hotKeys_CopyMeaning1TextBox.Text[0]);
			this.configuration.HotKeys_CopyMeaning2 = ((this.hotKeys_CopyMeaning2TextBox.Text.Length == 0) ? '2' : this.hotKeys_CopyMeaning2TextBox.Text[0]);
			this.configuration.HotKeys_CopyMeaning3 = ((this.hotKeys_CopyMeaning3TextBox.Text.Length == 0) ? '3' : this.hotKeys_CopyMeaning3TextBox.Text[0]);
			this.configuration.HotKeys_CopyMeaning4 = ((this.hotKeys_CopyMeaning4TextBox.Text.Length == 0) ? '4' : this.hotKeys_CopyMeaning4TextBox.Text[0]);
			this.configuration.HotKeys_CopyMeaning5 = ((this.hotKeys_CopyMeaning5TextBox.Text.Length == 0) ? '5' : this.hotKeys_CopyMeaning5TextBox.Text[0]);
			this.configuration.HotKeys_CopyMeaning6 = ((this.hotKeys_CopyMeaning6TextBox.Text.Length == 0) ? '6' : this.hotKeys_CopyMeaning6TextBox.Text[0]);
			this.configuration.HotKeys_MoveDownOneLine = ((this.hotKeys_MoveDownOneLineTextBox.Text.Length == 0) ? 'M' : this.hotKeys_MoveDownOneLineTextBox.Text[0]);
			this.configuration.HotKeys_MoveDownOneParagraph = ((this.hotKeys_MoveDownOneParagraphTextBox.Text.Length == 0) ? 'N' : this.hotKeys_MoveDownOneParagraphTextBox.Text[0]);
			this.configuration.HotKeys_MoveLeftOneWord = ((this.hotKeys_MoveLeftOneWordTextBox.Text.Length == 0) ? 'J' : this.hotKeys_MoveLeftOneWordTextBox.Text[0]);
			this.configuration.HotKeys_MoveRightOneWord = ((this.hotKeys_MoveRightOneWordTextBox.Text.Length == 0) ? 'K' : this.hotKeys_MoveRightOneWordTextBox.Text[0]);
			this.configuration.HotKeys_MoveUpOneLine = ((this.hotKeys_MoveUpOneLineTextBox.Text.Length == 0) ? 'I' : this.hotKeys_MoveUpOneLineTextBox.Text[0]);
			this.configuration.HotKeys_MoveUpOneParagraph = ((this.hotKeys_MoveUpOneParagraphTextBox.Text.Length == 0) ? 'U' : this.hotKeys_MoveUpOneParagraphTextBox.Text[0]);
			this.configuration.HotKeys_F1 = this.f1TextBox.Text;
			this.configuration.HotKeys_F2 = this.f2TextBox.Text;
			this.configuration.HotKeys_F3 = this.f3TextBox.Text;
			this.configuration.HotKeys_F4 = this.f4TextBox.Text;
			this.configuration.HotKeys_F5 = this.f5TextBox.Text;
			this.configuration.HotKeys_F6 = this.f6TextBox.Text;
			this.configuration.HotKeys_F7 = this.f7TextBox.Text;
			this.configuration.HotKeys_F8 = this.f8TextBox.Text;
			this.configuration.HotKeys_F9 = this.f9TextBox.Text;
			this.configuration.Style_TrungFont = this.trungLabel.Font;
			this.configuration.Style_TrungForeColor = this.trungLabel.ForeColor;
			this.configuration.Style_TrungBackColor = this.trungLabel.BackColor;
			this.configuration.Style_HanVietFont = this.hanVietLabel.Font;
			this.configuration.Style_HanVietForeColor = this.hanVietLabel.ForeColor;
			this.configuration.Style_HanVietBackColor = this.hanVietLabel.BackColor;
			this.configuration.Style_VietPhraseFont = this.vietPhraseLabel.Font;
			this.configuration.Style_VietPhraseForeColor = this.vietPhraseLabel.ForeColor;
			this.configuration.Style_VietPhraseBackColor = this.vietPhraseLabel.BackColor;
			this.configuration.Style_VietPhraseOneMeaningFont = this.vietPhraseOneMeaningLabel.Font;
			this.configuration.Style_VietPhraseOneMeaningForeColor = this.vietPhraseOneMeaningLabel.ForeColor;
			this.configuration.Style_VietPhraseOneMeaningBackColor = this.vietPhraseOneMeaningLabel.BackColor;
			this.configuration.Style_VietFont = this.vietLabel.Font;
			this.configuration.Style_VietForeColor = this.vietLabel.ForeColor;
			this.configuration.Style_VietBackColor = this.vietLabel.BackColor;
			this.configuration.Style_NghiaFont = this.nghiaLabel.Font;
			this.configuration.Style_NghiaForeColor = this.nghiaLabel.ForeColor;
			this.configuration.Style_NghiaBackColor = this.nghiaLabel.BackColor;
			this.configuration.TranslationAlgorithm = (this.algorithm_LongestVietPhraseFirstRadioButton.Checked ? 0 : (this.algorithm_LeftToRightRadioButton.Checked ? 1 : 2));
			this.configuration.PrioritizedName = this.prioritizedNameCheckBox.Checked;
			this.configuration.AlwaysFocusInViet = this.alwaysFocusInVietCheckBox.Checked;
		}

		private void ConfigurationPanelLoad(object sender, EventArgs e)
		{
			this.configurationToGui();
		}

		private void VietPhrase_NoWrapRadioButtonCheckedChanged(object sender, EventArgs e)
		{
			this.needToRetranslate = true;
		}

		private void VietPhrase_AlwaysWrapRadioButtonCheckedChanged(object sender, EventArgs e)
		{
			this.needToRetranslate = true;
		}

		private void VietPhrase_OnlyWrapTwoMeaningRadioButtonCheckedChanged(object sender, EventArgs e)
		{
			this.needToRetranslate = true;
		}

		private void VietPhraseOneMeaning_NoWrapRadioButtonCheckedChanged(object sender, EventArgs e)
		{
			this.needToRetranslate = true;
		}

		private void VietPhraseOneMeaning_AlwaysWrapRadioButtonCheckedChanged(object sender, EventArgs e)
		{
			this.needToRetranslate = true;
		}

		private void ToDockFix()
		{
			if (base.DockState == DockState.DockBottomAutoHide)
			{
				base.DockState = DockState.DockBottom;
				return;
			}
			if (base.DockState == DockState.DockLeftAutoHide)
			{
				base.DockState = DockState.DockLeft;
				return;
			}
			if (base.DockState == DockState.DockRightAutoHide)
			{
				base.DockState = DockState.DockRight;
				return;
			}
			if (base.DockState == DockState.DockTopAutoHide)
			{
				base.DockState = DockState.DockTop;
			}
		}

		private void UndoDockState()
		{
			if (base.DockState == DockState.DockBottom)
			{
				base.DockState = DockState.DockBottomAutoHide;
				return;
			}
			if (base.DockState == DockState.DockLeft)
			{
				base.DockState = DockState.DockLeftAutoHide;
				return;
			}
			if (base.DockState == DockState.DockRight)
			{
				base.DockState = DockState.DockRightAutoHide;
				return;
			}
			if (base.DockState == DockState.DockTop)
			{
				base.DockState = DockState.DockTopAutoHide;
			}
		}

		private void ChangeFont(Label testLabel)
		{
			this.ToDockFix();
			this.fontDialog.Font = testLabel.Font;
			this.fontDialog.Color = testLabel.ForeColor;
			DialogResult dialogResult = this.fontDialog.ShowDialog();
			if (dialogResult == DialogResult.OK)
			{
				testLabel.Font = this.fontDialog.Font;
				testLabel.ForeColor = this.fontDialog.Color;
				this.needToResetPanelStyle = true;
			}
		}

		private void ChangeBackColor(Label testLabel)
		{
			this.ToDockFix();
			this.colorDialog.Color = testLabel.BackColor;
			DialogResult dialogResult = this.colorDialog.ShowDialog();
			if (dialogResult == DialogResult.OK)
			{
				testLabel.BackColor = this.colorDialog.Color;
				this.needToResetPanelStyle = true;
			}
		}

		private void TrungFontButtonClick(object sender, EventArgs e)
		{
			this.ChangeFont(this.trungLabel);
		}

		private void TrungBackColorButtonClick(object sender, EventArgs e)
		{
			this.ChangeBackColor(this.trungLabel);
		}

		private void HanVietFontButtonClick(object sender, EventArgs e)
		{
			this.ChangeFont(this.hanVietLabel);
		}

		private void HanVietBackColorButtonClick(object sender, EventArgs e)
		{
			this.ChangeBackColor(this.hanVietLabel);
		}

		private void VietPhraseFontButtonClick(object sender, EventArgs e)
		{
			this.ChangeFont(this.vietPhraseLabel);
		}

		private void VietPhraseBackColorButtonClick(object sender, EventArgs e)
		{
			this.ChangeBackColor(this.vietPhraseLabel);
		}

		private void VietPhraseOneMeaningFontButtonClick(object sender, EventArgs e)
		{
			this.ChangeFont(this.vietPhraseOneMeaningLabel);
		}

		private void VietPhraseOneMeaningBackColorButtonClick(object sender, EventArgs e)
		{
			this.ChangeBackColor(this.vietPhraseOneMeaningLabel);
		}

		private void VietFontButtonClick(object sender, EventArgs e)
		{
			this.ChangeFont(this.vietLabel);
		}

		private void VietBackColorButtonClick(object sender, EventArgs e)
		{
			this.ChangeBackColor(this.vietLabel);
		}

		private void NghiaFontButtonClick(object sender, EventArgs e)
		{
			this.ChangeFont(this.nghiaLabel);
		}

		private void NghiaBackColorButtonClick(object sender, EventArgs e)
		{
			this.ChangeBackColor(this.nghiaLabel);
		}

		private void Algorithm_LongestVietPhraseFirstRadioButtonCheckedChanged(object sender, EventArgs e)
		{
			this.needToRetranslate = true;
		}

		private void Algorithm_LeftToRightRadioButtonCheckedChanged(object sender, EventArgs e)
		{
			this.needToRetranslate = true;
		}

		private void Algorithm_LongestVietPhraseFirstWithConditionRadioButtonCheckedChanged(object sender, EventArgs e)
		{
			this.needToRetranslate = true;
		}

		private void PrioritizedNameCheckBoxCheckedChanged(object sender, EventArgs e)
		{
			this.needToRetranslate = true;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ConfigurationPanel));
			this.groupBox1 = new GroupBox();
			this.browserPanel_DisablePopupsCheckBox = new CheckBox();
			this.browserPanel_DisableImagesCheckBox = new CheckBox();
			this.saveButton = new Button();
			this.cancelButton = new Button();
			this.groupBox2 = new GroupBox();
			this.vietPhrase_AlwaysWrapExceptHanVietRadioButton = new RadioButton();
			this.vietPhrase_OnlyWrapTwoMeaningRadioButton = new RadioButton();
			this.vietPhrase_AlwaysWrapRadioButton = new RadioButton();
			this.vietPhrase_NoWrapRadioButton = new RadioButton();
			this.groupBox3 = new GroupBox();
			this.vietPhraseOneMeaning_AlwaysWrapExceptHanVietRadioButton = new RadioButton();
			this.vietPhraseOneMeaning_NoWrapRadioButton = new RadioButton();
			this.vietPhraseOneMeaning_AlwaysWrapRadioButton = new RadioButton();
			this.groupBox4 = new GroupBox();
			this.hotKeys_CopyMeaning6TextBox = new TextBox();
			this.label23 = new Label();
			this.label24 = new Label();
			this.f9TextBox = new TextBox();
			this.f7TextBox = new TextBox();
			this.f5TextBox = new TextBox();
			this.f3TextBox = new TextBox();
			this.f8TextBox = new TextBox();
			this.f6TextBox = new TextBox();
			this.f4TextBox = new TextBox();
			this.f2TextBox = new TextBox();
			this.f1TextBox = new TextBox();
			this.hotKeys_CopyMeaning5TextBox = new TextBox();
			this.label21 = new Label();
			this.label33 = new Label();
			this.label32 = new Label();
			this.label31 = new Label();
			this.label30 = new Label();
			this.label29 = new Label();
			this.label28 = new Label();
			this.label27 = new Label();
			this.label26 = new Label();
			this.label25 = new Label();
			this.label22 = new Label();
			this.hotKeys_CopyMeaning4TextBox = new TextBox();
			this.label19 = new Label();
			this.label20 = new Label();
			this.hotKeys_CopyMeaning3TextBox = new TextBox();
			this.label17 = new Label();
			this.label18 = new Label();
			this.hotKeys_CopyMeaning2TextBox = new TextBox();
			this.label15 = new Label();
			this.label16 = new Label();
			this.hotKeys_CopyHighlightedHanVietTextBox = new TextBox();
			this.hotKeys_CopyMeaning1TextBox = new TextBox();
			this.label35 = new Label();
			this.label13 = new Label();
			this.label34 = new Label();
			this.label14 = new Label();
			this.hotKeys_MoveUpOneParagraphTextBox = new TextBox();
			this.label11 = new Label();
			this.label12 = new Label();
			this.hotKeys_MoveDownOneParagraphTextBox = new TextBox();
			this.label9 = new Label();
			this.label10 = new Label();
			this.hotKeys_MoveUpOneLineTextBox = new TextBox();
			this.label7 = new Label();
			this.label8 = new Label();
			this.hotKeys_MoveDownOneLineTextBox = new TextBox();
			this.label5 = new Label();
			this.label6 = new Label();
			this.hotKeys_MoveLeftOneWordTextBox = new TextBox();
			this.label3 = new Label();
			this.label4 = new Label();
			this.hotKeys_MoveRightOneWordTextBox = new TextBox();
			this.label2 = new Label();
			this.label1 = new Label();
			this.translateTabPage = new TabControl();
			this.tabPage1 = new TabPage();
			this.groupBox5 = new GroupBox();
			this.prioritizedNameCheckBox = new CheckBox();
			this.algorithm_LeftToRightRadioButton = new RadioButton();
			this.algorithm_LongestVietPhraseFirstWithConditionRadioButton = new RadioButton();
			this.algorithm_LongestVietPhraseFirstRadioButton = new RadioButton();
			this.tabPage2 = new TabPage();
			this.groupBox7 = new GroupBox();
			this.alwaysFocusInVietCheckBox = new CheckBox();
			this.groupBox6 = new GroupBox();
			this.nghiaBackColorButton = new Button();
			this.trungLabel = new Label();
			this.vietBackColorButton = new Button();
			this.hanVietLabel = new Label();
			this.vietPhraseOneMeaningBackColorButton = new Button();
			this.vietPhraseLabel = new Label();
			this.vietPhraseBackColorButton = new Button();
			this.vietPhraseOneMeaningLabel = new Label();
			this.hanVietBackColorButton = new Button();
			this.vietLabel = new Label();
			this.trungBackColorButton = new Button();
			this.nghiaLabel = new Label();
			this.nghiaFontButton = new Button();
			this.trungFontButton = new Button();
			this.vietFontButton = new Button();
			this.hanVietFontButton = new Button();
			this.vietPhraseOneMeaningFontButton = new Button();
			this.vietPhraseFontButton = new Button();
			this.colorDialog = new ColorDialog();
			this.fontDialog = new FontDialog();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.translateTabPage.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.groupBox7.SuspendLayout();
			this.groupBox6.SuspendLayout();
			base.SuspendLayout();
			this.groupBox1.Controls.Add(this.browserPanel_DisablePopupsCheckBox);
			this.groupBox1.Controls.Add(this.browserPanel_DisableImagesCheckBox);
			this.groupBox1.Enabled = false;
			this.groupBox1.Location = new Point(0, 592);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new Size(129, 38);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Browser Panel";
			this.groupBox1.Visible = false;
			this.browserPanel_DisablePopupsCheckBox.Checked = true;
			this.browserPanel_DisablePopupsCheckBox.CheckState = CheckState.Checked;
			this.browserPanel_DisablePopupsCheckBox.Location = new Point(6, 37);
			this.browserPanel_DisablePopupsCheckBox.Name = "browserPanel_DisablePopupsCheckBox";
			this.browserPanel_DisablePopupsCheckBox.Size = new Size(118, 24);
			this.browserPanel_DisablePopupsCheckBox.TabIndex = 2;
			this.browserPanel_DisablePopupsCheckBox.Text = "Disable Pop-ups";
			this.browserPanel_DisablePopupsCheckBox.UseVisualStyleBackColor = true;
			this.browserPanel_DisableImagesCheckBox.Checked = true;
			this.browserPanel_DisableImagesCheckBox.CheckState = CheckState.Checked;
			this.browserPanel_DisableImagesCheckBox.Location = new Point(8, 8);
			this.browserPanel_DisableImagesCheckBox.Name = "browserPanel_DisableImagesCheckBox";
			this.browserPanel_DisableImagesCheckBox.Size = new Size(102, 24);
			this.browserPanel_DisableImagesCheckBox.TabIndex = 1;
			this.browserPanel_DisableImagesCheckBox.Text = "Disable Images";
			this.browserPanel_DisableImagesCheckBox.UseVisualStyleBackColor = true;
			this.saveButton.Location = new Point(192, 600);
			this.saveButton.Name = "saveButton";
			this.saveButton.Size = new Size(75, 23);
			this.saveButton.TabIndex = 20;
			this.saveButton.Text = "&Save";
			this.saveButton.UseVisualStyleBackColor = true;
			this.saveButton.Click += new EventHandler(this.SaveButtonClick);
			this.cancelButton.Location = new Point(288, 600);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new Size(75, 23);
			this.cancelButton.TabIndex = 21;
			this.cancelButton.Text = "&Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new EventHandler(this.CancelButtonClick);
			this.groupBox2.Controls.Add(this.vietPhrase_AlwaysWrapExceptHanVietRadioButton);
			this.groupBox2.Controls.Add(this.vietPhrase_OnlyWrapTwoMeaningRadioButton);
			this.groupBox2.Controls.Add(this.vietPhrase_AlwaysWrapRadioButton);
			this.groupBox2.Controls.Add(this.vietPhrase_NoWrapRadioButton);
			this.groupBox2.Location = new Point(8, 88);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new Size(264, 126);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "VietPhrase";
			this.vietPhrase_AlwaysWrapExceptHanVietRadioButton.AutoSize = true;
			this.vietPhrase_AlwaysWrapExceptHanVietRadioButton.Location = new Point(6, 75);
			this.vietPhrase_AlwaysWrapExceptHanVietRadioButton.Name = "vietPhrase_AlwaysWrapExceptHanVietRadioButton";
			this.vietPhrase_AlwaysWrapExceptHanVietRadioButton.Size = new Size(179, 17);
			this.vietPhrase_AlwaysWrapExceptHanVietRadioButton.TabIndex = 6;
			this.vietPhrase_AlwaysWrapExceptHanVietRadioButton.TabStop = true;
			this.vietPhrase_AlwaysWrapExceptHanVietRadioButton.Text = "Luôn sử dụng [ ... ] (trừ Hán Việt)";
			this.vietPhrase_AlwaysWrapExceptHanVietRadioButton.UseVisualStyleBackColor = true;
			this.vietPhrase_OnlyWrapTwoMeaningRadioButton.Location = new Point(6, 96);
			this.vietPhrase_OnlyWrapTwoMeaningRadioButton.Name = "vietPhrase_OnlyWrapTwoMeaningRadioButton";
			this.vietPhrase_OnlyWrapTwoMeaningRadioButton.Size = new Size(219, 24);
			this.vietPhrase_OnlyWrapTwoMeaningRadioButton.TabIndex = 5;
			this.vietPhrase_OnlyWrapTwoMeaningRadioButton.TabStop = true;
			this.vietPhrase_OnlyWrapTwoMeaningRadioButton.Text = "Chỉ sử dụng [ ... ] nếu có hơn một nghĩa";
			this.vietPhrase_OnlyWrapTwoMeaningRadioButton.UseVisualStyleBackColor = true;
			this.vietPhrase_OnlyWrapTwoMeaningRadioButton.CheckedChanged += new EventHandler(this.VietPhrase_OnlyWrapTwoMeaningRadioButtonCheckedChanged);
			this.vietPhrase_AlwaysWrapRadioButton.Location = new Point(6, 47);
			this.vietPhrase_AlwaysWrapRadioButton.Name = "vietPhrase_AlwaysWrapRadioButton";
			this.vietPhrase_AlwaysWrapRadioButton.Size = new Size(219, 24);
			this.vietPhrase_AlwaysWrapRadioButton.TabIndex = 4;
			this.vietPhrase_AlwaysWrapRadioButton.TabStop = true;
			this.vietPhrase_AlwaysWrapRadioButton.Text = "Luôn sử dụng [ ... ] (cả Hán Việt)";
			this.vietPhrase_AlwaysWrapRadioButton.UseVisualStyleBackColor = true;
			this.vietPhrase_AlwaysWrapRadioButton.CheckedChanged += new EventHandler(this.VietPhrase_AlwaysWrapRadioButtonCheckedChanged);
			this.vietPhrase_NoWrapRadioButton.Location = new Point(6, 19);
			this.vietPhrase_NoWrapRadioButton.Name = "vietPhrase_NoWrapRadioButton";
			this.vietPhrase_NoWrapRadioButton.Size = new Size(219, 24);
			this.vietPhrase_NoWrapRadioButton.TabIndex = 3;
			this.vietPhrase_NoWrapRadioButton.TabStop = true;
			this.vietPhrase_NoWrapRadioButton.Text = "Không sử dụng [ ... ]";
			this.vietPhrase_NoWrapRadioButton.UseVisualStyleBackColor = true;
			this.vietPhrase_NoWrapRadioButton.CheckedChanged += new EventHandler(this.VietPhrase_NoWrapRadioButtonCheckedChanged);
			this.groupBox3.Controls.Add(this.vietPhraseOneMeaning_AlwaysWrapExceptHanVietRadioButton);
			this.groupBox3.Controls.Add(this.vietPhraseOneMeaning_NoWrapRadioButton);
			this.groupBox3.Controls.Add(this.vietPhraseOneMeaning_AlwaysWrapRadioButton);
			this.groupBox3.Location = new Point(280, 88);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new Size(256, 126);
			this.groupBox3.TabIndex = 3;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "VietPhrase một nghĩa";
			this.vietPhraseOneMeaning_AlwaysWrapExceptHanVietRadioButton.AutoSize = true;
			this.vietPhraseOneMeaning_AlwaysWrapExceptHanVietRadioButton.Location = new Point(6, 73);
			this.vietPhraseOneMeaning_AlwaysWrapExceptHanVietRadioButton.Name = "vietPhraseOneMeaning_AlwaysWrapExceptHanVietRadioButton";
			this.vietPhraseOneMeaning_AlwaysWrapExceptHanVietRadioButton.Size = new Size(179, 17);
			this.vietPhraseOneMeaning_AlwaysWrapExceptHanVietRadioButton.TabIndex = 6;
			this.vietPhraseOneMeaning_AlwaysWrapExceptHanVietRadioButton.TabStop = true;
			this.vietPhraseOneMeaning_AlwaysWrapExceptHanVietRadioButton.Text = "Luôn sử dụng [ ... ] (trừ Hán Việt)";
			this.vietPhraseOneMeaning_AlwaysWrapExceptHanVietRadioButton.UseVisualStyleBackColor = true;
			this.vietPhraseOneMeaning_NoWrapRadioButton.Location = new Point(6, 19);
			this.vietPhraseOneMeaning_NoWrapRadioButton.Name = "vietPhraseOneMeaning_NoWrapRadioButton";
			this.vietPhraseOneMeaning_NoWrapRadioButton.Size = new Size(179, 24);
			this.vietPhraseOneMeaning_NoWrapRadioButton.TabIndex = 6;
			this.vietPhraseOneMeaning_NoWrapRadioButton.TabStop = true;
			this.vietPhraseOneMeaning_NoWrapRadioButton.Text = "Không sử dụng [ ... ]";
			this.vietPhraseOneMeaning_NoWrapRadioButton.UseVisualStyleBackColor = true;
			this.vietPhraseOneMeaning_NoWrapRadioButton.CheckedChanged += new EventHandler(this.VietPhraseOneMeaning_NoWrapRadioButtonCheckedChanged);
			this.vietPhraseOneMeaning_AlwaysWrapRadioButton.Location = new Point(6, 46);
			this.vietPhraseOneMeaning_AlwaysWrapRadioButton.Name = "vietPhraseOneMeaning_AlwaysWrapRadioButton";
			this.vietPhraseOneMeaning_AlwaysWrapRadioButton.Size = new Size(179, 24);
			this.vietPhraseOneMeaning_AlwaysWrapRadioButton.TabIndex = 7;
			this.vietPhraseOneMeaning_AlwaysWrapRadioButton.TabStop = true;
			this.vietPhraseOneMeaning_AlwaysWrapRadioButton.Text = "Luôn sử dụng [ ... ] (cả Hán Việt)";
			this.vietPhraseOneMeaning_AlwaysWrapRadioButton.UseVisualStyleBackColor = true;
			this.vietPhraseOneMeaning_AlwaysWrapRadioButton.CheckedChanged += new EventHandler(this.VietPhraseOneMeaning_AlwaysWrapRadioButtonCheckedChanged);
			this.groupBox4.Controls.Add(this.hotKeys_CopyMeaning6TextBox);
			this.groupBox4.Controls.Add(this.label23);
			this.groupBox4.Controls.Add(this.label24);
			this.groupBox4.Controls.Add(this.f9TextBox);
			this.groupBox4.Controls.Add(this.f7TextBox);
			this.groupBox4.Controls.Add(this.f5TextBox);
			this.groupBox4.Controls.Add(this.f3TextBox);
			this.groupBox4.Controls.Add(this.f8TextBox);
			this.groupBox4.Controls.Add(this.f6TextBox);
			this.groupBox4.Controls.Add(this.f4TextBox);
			this.groupBox4.Controls.Add(this.f2TextBox);
			this.groupBox4.Controls.Add(this.f1TextBox);
			this.groupBox4.Controls.Add(this.hotKeys_CopyMeaning5TextBox);
			this.groupBox4.Controls.Add(this.label21);
			this.groupBox4.Controls.Add(this.label33);
			this.groupBox4.Controls.Add(this.label32);
			this.groupBox4.Controls.Add(this.label31);
			this.groupBox4.Controls.Add(this.label30);
			this.groupBox4.Controls.Add(this.label29);
			this.groupBox4.Controls.Add(this.label28);
			this.groupBox4.Controls.Add(this.label27);
			this.groupBox4.Controls.Add(this.label26);
			this.groupBox4.Controls.Add(this.label25);
			this.groupBox4.Controls.Add(this.label22);
			this.groupBox4.Controls.Add(this.hotKeys_CopyMeaning4TextBox);
			this.groupBox4.Controls.Add(this.label19);
			this.groupBox4.Controls.Add(this.label20);
			this.groupBox4.Controls.Add(this.hotKeys_CopyMeaning3TextBox);
			this.groupBox4.Controls.Add(this.label17);
			this.groupBox4.Controls.Add(this.label18);
			this.groupBox4.Controls.Add(this.hotKeys_CopyMeaning2TextBox);
			this.groupBox4.Controls.Add(this.label15);
			this.groupBox4.Controls.Add(this.label16);
			this.groupBox4.Controls.Add(this.hotKeys_CopyHighlightedHanVietTextBox);
			this.groupBox4.Controls.Add(this.hotKeys_CopyMeaning1TextBox);
			this.groupBox4.Controls.Add(this.label35);
			this.groupBox4.Controls.Add(this.label13);
			this.groupBox4.Controls.Add(this.label34);
			this.groupBox4.Controls.Add(this.label14);
			this.groupBox4.Controls.Add(this.hotKeys_MoveUpOneParagraphTextBox);
			this.groupBox4.Controls.Add(this.label11);
			this.groupBox4.Controls.Add(this.label12);
			this.groupBox4.Controls.Add(this.hotKeys_MoveDownOneParagraphTextBox);
			this.groupBox4.Controls.Add(this.label9);
			this.groupBox4.Controls.Add(this.label10);
			this.groupBox4.Controls.Add(this.hotKeys_MoveUpOneLineTextBox);
			this.groupBox4.Controls.Add(this.label7);
			this.groupBox4.Controls.Add(this.label8);
			this.groupBox4.Controls.Add(this.hotKeys_MoveDownOneLineTextBox);
			this.groupBox4.Controls.Add(this.label5);
			this.groupBox4.Controls.Add(this.label6);
			this.groupBox4.Controls.Add(this.hotKeys_MoveLeftOneWordTextBox);
			this.groupBox4.Controls.Add(this.label3);
			this.groupBox4.Controls.Add(this.label4);
			this.groupBox4.Controls.Add(this.hotKeys_MoveRightOneWordTextBox);
			this.groupBox4.Controls.Add(this.label2);
			this.groupBox4.Controls.Add(this.label1);
			this.groupBox4.Location = new Point(8, 224);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new Size(528, 328);
			this.groupBox4.TabIndex = 4;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Hot Keys";
			this.hotKeys_CopyMeaning6TextBox.CharacterCasing = CharacterCasing.Upper;
			this.hotKeys_CopyMeaning6TextBox.Location = new Point(394, 168);
			this.hotKeys_CopyMeaning6TextBox.MaxLength = 1;
			this.hotKeys_CopyMeaning6TextBox.Name = "hotKeys_CopyMeaning6TextBox";
			this.hotKeys_CopyMeaning6TextBox.Size = new Size(32, 20);
			this.hotKeys_CopyMeaning6TextBox.TabIndex = 19;
			this.hotKeys_CopyMeaning6TextBox.Text = "6";
			this.hotKeys_CopyMeaning6TextBox.TextAlign = HorizontalAlignment.Center;
			this.label23.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.label23.Location = new Point(355, 166);
			this.label23.Name = "label23";
			this.label23.Size = new Size(44, 23);
			this.label23.TabIndex = 33;
			this.label23.Text = "Ctrl + ";
			this.label23.TextAlign = ContentAlignment.MiddleLeft;
			this.label24.Location = new Point(233, 166);
			this.label24.Name = "label24";
			this.label24.Size = new Size(116, 23);
			this.label24.TabIndex = 32;
			this.label24.Text = "Copy nghĩa thứ 6:";
			this.label24.TextAlign = ContentAlignment.MiddleRight;
			this.f9TextBox.Location = new Point(139, 296);
			this.f9TextBox.MaxLength = 0;
			this.f9TextBox.Name = "f9TextBox";
			this.f9TextBox.Size = new Size(163, 20);
			this.f9TextBox.TabIndex = 28;
			this.f7TextBox.Location = new Point(139, 275);
			this.f7TextBox.MaxLength = 0;
			this.f7TextBox.Name = "f7TextBox";
			this.f7TextBox.Size = new Size(163, 20);
			this.f7TextBox.TabIndex = 26;
			this.f5TextBox.Location = new Point(139, 252);
			this.f5TextBox.MaxLength = 0;
			this.f5TextBox.Name = "f5TextBox";
			this.f5TextBox.Size = new Size(163, 20);
			this.f5TextBox.TabIndex = 24;
			this.f3TextBox.Location = new Point(139, 229);
			this.f3TextBox.MaxLength = 0;
			this.f3TextBox.Name = "f3TextBox";
			this.f3TextBox.Size = new Size(163, 20);
			this.f3TextBox.TabIndex = 22;
			this.f8TextBox.Location = new Point(355, 275);
			this.f8TextBox.MaxLength = 0;
			this.f8TextBox.Name = "f8TextBox";
			this.f8TextBox.Size = new Size(163, 20);
			this.f8TextBox.TabIndex = 27;
			this.f6TextBox.Location = new Point(355, 252);
			this.f6TextBox.MaxLength = 0;
			this.f6TextBox.Name = "f6TextBox";
			this.f6TextBox.Size = new Size(163, 20);
			this.f6TextBox.TabIndex = 25;
			this.f4TextBox.Location = new Point(355, 229);
			this.f4TextBox.MaxLength = 0;
			this.f4TextBox.Name = "f4TextBox";
			this.f4TextBox.Size = new Size(163, 20);
			this.f4TextBox.TabIndex = 23;
			this.f2TextBox.Location = new Point(355, 204);
			this.f2TextBox.MaxLength = 0;
			this.f2TextBox.Name = "f2TextBox";
			this.f2TextBox.Size = new Size(163, 20);
			this.f2TextBox.TabIndex = 21;
			this.f1TextBox.Location = new Point(139, 205);
			this.f1TextBox.MaxLength = 0;
			this.f1TextBox.Name = "f1TextBox";
			this.f1TextBox.Size = new Size(163, 20);
			this.f1TextBox.TabIndex = 20;
			this.hotKeys_CopyMeaning5TextBox.CharacterCasing = CharacterCasing.Upper;
			this.hotKeys_CopyMeaning5TextBox.Location = new Point(175, 166);
			this.hotKeys_CopyMeaning5TextBox.MaxLength = 1;
			this.hotKeys_CopyMeaning5TextBox.Name = "hotKeys_CopyMeaning5TextBox";
			this.hotKeys_CopyMeaning5TextBox.Size = new Size(32, 20);
			this.hotKeys_CopyMeaning5TextBox.TabIndex = 18;
			this.hotKeys_CopyMeaning5TextBox.Text = "5";
			this.hotKeys_CopyMeaning5TextBox.TextAlign = HorizontalAlignment.Center;
			this.label21.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.label21.Location = new Point(136, 164);
			this.label21.Name = "label21";
			this.label21.Size = new Size(44, 23);
			this.label21.TabIndex = 30;
			this.label21.Text = "Ctrl + ";
			this.label21.TextAlign = ContentAlignment.MiddleLeft;
			this.label33.Location = new Point(233, 273);
			this.label33.Name = "label33";
			this.label33.Size = new Size(116, 23);
			this.label33.TabIndex = 29;
			this.label33.Text = "F8:";
			this.label33.TextAlign = ContentAlignment.MiddleRight;
			this.label32.Location = new Point(233, 250);
			this.label32.Name = "label32";
			this.label32.Size = new Size(116, 23);
			this.label32.TabIndex = 29;
			this.label32.Text = "F6:";
			this.label32.TextAlign = ContentAlignment.MiddleRight;
			this.label31.Location = new Point(233, 227);
			this.label31.Name = "label31";
			this.label31.Size = new Size(116, 23);
			this.label31.TabIndex = 29;
			this.label31.Text = "F4:";
			this.label31.TextAlign = ContentAlignment.MiddleRight;
			this.label30.Location = new Point(233, 204);
			this.label30.Name = "label30";
			this.label30.Size = new Size(116, 23);
			this.label30.TabIndex = 29;
			this.label30.Text = "F2:";
			this.label30.TextAlign = ContentAlignment.MiddleRight;
			this.label29.Location = new Point(14, 296);
			this.label29.Name = "label29";
			this.label29.Size = new Size(116, 23);
			this.label29.TabIndex = 29;
			this.label29.Text = "F9:";
			this.label29.TextAlign = ContentAlignment.MiddleRight;
			this.label28.Location = new Point(14, 273);
			this.label28.Name = "label28";
			this.label28.Size = new Size(116, 23);
			this.label28.TabIndex = 29;
			this.label28.Text = "F7:";
			this.label28.TextAlign = ContentAlignment.MiddleRight;
			this.label27.Location = new Point(14, 250);
			this.label27.Name = "label27";
			this.label27.Size = new Size(116, 23);
			this.label27.TabIndex = 29;
			this.label27.Text = "F5:";
			this.label27.TextAlign = ContentAlignment.MiddleRight;
			this.label26.Location = new Point(14, 227);
			this.label26.Name = "label26";
			this.label26.Size = new Size(116, 23);
			this.label26.TabIndex = 29;
			this.label26.Text = "F3:";
			this.label26.TextAlign = ContentAlignment.MiddleRight;
			this.label25.Location = new Point(14, 204);
			this.label25.Name = "label25";
			this.label25.Size = new Size(116, 23);
			this.label25.TabIndex = 29;
			this.label25.Text = "F1:";
			this.label25.TextAlign = ContentAlignment.MiddleRight;
			this.label22.Location = new Point(14, 164);
			this.label22.Name = "label22";
			this.label22.Size = new Size(116, 23);
			this.label22.TabIndex = 29;
			this.label22.Text = "Copy nghĩa thứ 5:";
			this.label22.TextAlign = ContentAlignment.MiddleRight;
			this.hotKeys_CopyMeaning4TextBox.CharacterCasing = CharacterCasing.Upper;
			this.hotKeys_CopyMeaning4TextBox.Location = new Point(394, 145);
			this.hotKeys_CopyMeaning4TextBox.MaxLength = 1;
			this.hotKeys_CopyMeaning4TextBox.Name = "hotKeys_CopyMeaning4TextBox";
			this.hotKeys_CopyMeaning4TextBox.Size = new Size(32, 20);
			this.hotKeys_CopyMeaning4TextBox.TabIndex = 17;
			this.hotKeys_CopyMeaning4TextBox.Text = "4";
			this.hotKeys_CopyMeaning4TextBox.TextAlign = HorizontalAlignment.Center;
			this.label19.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.label19.Location = new Point(355, 143);
			this.label19.Name = "label19";
			this.label19.Size = new Size(44, 23);
			this.label19.TabIndex = 27;
			this.label19.Text = "Ctrl + ";
			this.label19.TextAlign = ContentAlignment.MiddleLeft;
			this.label20.Location = new Point(233, 143);
			this.label20.Name = "label20";
			this.label20.Size = new Size(116, 23);
			this.label20.TabIndex = 26;
			this.label20.Text = "Copy nghĩa thứ 4:";
			this.label20.TextAlign = ContentAlignment.MiddleRight;
			this.hotKeys_CopyMeaning3TextBox.CharacterCasing = CharacterCasing.Upper;
			this.hotKeys_CopyMeaning3TextBox.Location = new Point(175, 143);
			this.hotKeys_CopyMeaning3TextBox.MaxLength = 1;
			this.hotKeys_CopyMeaning3TextBox.Name = "hotKeys_CopyMeaning3TextBox";
			this.hotKeys_CopyMeaning3TextBox.Size = new Size(32, 20);
			this.hotKeys_CopyMeaning3TextBox.TabIndex = 16;
			this.hotKeys_CopyMeaning3TextBox.Text = "3";
			this.hotKeys_CopyMeaning3TextBox.TextAlign = HorizontalAlignment.Center;
			this.label17.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.label17.Location = new Point(136, 141);
			this.label17.Name = "label17";
			this.label17.Size = new Size(44, 23);
			this.label17.TabIndex = 24;
			this.label17.Text = "Ctrl + ";
			this.label17.TextAlign = ContentAlignment.MiddleLeft;
			this.label18.Location = new Point(14, 141);
			this.label18.Name = "label18";
			this.label18.Size = new Size(116, 23);
			this.label18.TabIndex = 23;
			this.label18.Text = "Copy nghĩa thứ 3:";
			this.label18.TextAlign = ContentAlignment.MiddleRight;
			this.hotKeys_CopyMeaning2TextBox.CharacterCasing = CharacterCasing.Upper;
			this.hotKeys_CopyMeaning2TextBox.Location = new Point(394, 122);
			this.hotKeys_CopyMeaning2TextBox.MaxLength = 1;
			this.hotKeys_CopyMeaning2TextBox.Name = "hotKeys_CopyMeaning2TextBox";
			this.hotKeys_CopyMeaning2TextBox.Size = new Size(32, 20);
			this.hotKeys_CopyMeaning2TextBox.TabIndex = 15;
			this.hotKeys_CopyMeaning2TextBox.Text = "2";
			this.hotKeys_CopyMeaning2TextBox.TextAlign = HorizontalAlignment.Center;
			this.label15.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.label15.Location = new Point(355, 120);
			this.label15.Name = "label15";
			this.label15.Size = new Size(44, 23);
			this.label15.TabIndex = 21;
			this.label15.Text = "Ctrl + ";
			this.label15.TextAlign = ContentAlignment.MiddleLeft;
			this.label16.Location = new Point(233, 120);
			this.label16.Name = "label16";
			this.label16.Size = new Size(116, 23);
			this.label16.TabIndex = 20;
			this.label16.Text = "Copy nghĩa thứ 2:";
			this.label16.TextAlign = ContentAlignment.MiddleRight;
			this.hotKeys_CopyHighlightedHanVietTextBox.CharacterCasing = CharacterCasing.Upper;
			this.hotKeys_CopyHighlightedHanVietTextBox.Location = new Point(394, 98);
			this.hotKeys_CopyHighlightedHanVietTextBox.MaxLength = 1;
			this.hotKeys_CopyHighlightedHanVietTextBox.Name = "hotKeys_CopyHighlightedHanVietTextBox";
			this.hotKeys_CopyHighlightedHanVietTextBox.Size = new Size(32, 20);
			this.hotKeys_CopyHighlightedHanVietTextBox.TabIndex = 14;
			this.hotKeys_CopyHighlightedHanVietTextBox.Text = "0";
			this.hotKeys_CopyHighlightedHanVietTextBox.TextAlign = HorizontalAlignment.Center;
			this.hotKeys_CopyMeaning1TextBox.CharacterCasing = CharacterCasing.Upper;
			this.hotKeys_CopyMeaning1TextBox.Location = new Point(175, 120);
			this.hotKeys_CopyMeaning1TextBox.MaxLength = 1;
			this.hotKeys_CopyMeaning1TextBox.Name = "hotKeys_CopyMeaning1TextBox";
			this.hotKeys_CopyMeaning1TextBox.Size = new Size(32, 20);
			this.hotKeys_CopyMeaning1TextBox.TabIndex = 14;
			this.hotKeys_CopyMeaning1TextBox.Text = "1";
			this.hotKeys_CopyMeaning1TextBox.TextAlign = HorizontalAlignment.Center;
			this.label35.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.label35.Location = new Point(355, 96);
			this.label35.Name = "label35";
			this.label35.Size = new Size(44, 23);
			this.label35.TabIndex = 18;
			this.label35.Text = "Ctrl + ";
			this.label35.TextAlign = ContentAlignment.MiddleLeft;
			this.label13.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.label13.Location = new Point(136, 118);
			this.label13.Name = "label13";
			this.label13.Size = new Size(44, 23);
			this.label13.TabIndex = 18;
			this.label13.Text = "Ctrl + ";
			this.label13.TextAlign = ContentAlignment.MiddleLeft;
			this.label34.Location = new Point(208, 96);
			this.label34.Name = "label34";
			this.label34.Size = new Size(141, 23);
			this.label34.TabIndex = 17;
			this.label34.Text = "Copy cụm Hán Việt bôi đỏ:";
			this.label34.TextAlign = ContentAlignment.MiddleRight;
			this.label14.Location = new Point(14, 118);
			this.label14.Name = "label14";
			this.label14.Size = new Size(116, 23);
			this.label14.TabIndex = 17;
			this.label14.Text = "Copy nghĩa thứ 1:";
			this.label14.TextAlign = ContentAlignment.MiddleRight;
			this.hotKeys_MoveUpOneParagraphTextBox.CharacterCasing = CharacterCasing.Upper;
			this.hotKeys_MoveUpOneParagraphTextBox.Location = new Point(175, 60);
			this.hotKeys_MoveUpOneParagraphTextBox.MaxLength = 1;
			this.hotKeys_MoveUpOneParagraphTextBox.Name = "hotKeys_MoveUpOneParagraphTextBox";
			this.hotKeys_MoveUpOneParagraphTextBox.Size = new Size(32, 20);
			this.hotKeys_MoveUpOneParagraphTextBox.TabIndex = 12;
			this.hotKeys_MoveUpOneParagraphTextBox.Text = "U";
			this.hotKeys_MoveUpOneParagraphTextBox.TextAlign = HorizontalAlignment.Center;
			this.label11.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.label11.Location = new Point(136, 58);
			this.label11.Name = "label11";
			this.label11.Size = new Size(44, 23);
			this.label11.TabIndex = 15;
			this.label11.Text = "Ctrl + ";
			this.label11.TextAlign = ContentAlignment.MiddleLeft;
			this.label12.Location = new Point(14, 58);
			this.label12.Name = "label12";
			this.label12.Size = new Size(116, 23);
			this.label12.TabIndex = 14;
			this.label12.Text = "Move Up 1 Paragraph:";
			this.label12.TextAlign = ContentAlignment.MiddleRight;
			this.hotKeys_MoveDownOneParagraphTextBox.CharacterCasing = CharacterCasing.Upper;
			this.hotKeys_MoveDownOneParagraphTextBox.Location = new Point(394, 64);
			this.hotKeys_MoveDownOneParagraphTextBox.MaxLength = 1;
			this.hotKeys_MoveDownOneParagraphTextBox.Name = "hotKeys_MoveDownOneParagraphTextBox";
			this.hotKeys_MoveDownOneParagraphTextBox.Size = new Size(32, 20);
			this.hotKeys_MoveDownOneParagraphTextBox.TabIndex = 13;
			this.hotKeys_MoveDownOneParagraphTextBox.Text = "N";
			this.hotKeys_MoveDownOneParagraphTextBox.TextAlign = HorizontalAlignment.Center;
			this.label9.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.label9.Location = new Point(355, 62);
			this.label9.Name = "label9";
			this.label9.Size = new Size(44, 23);
			this.label9.TabIndex = 12;
			this.label9.Text = "Ctrl + ";
			this.label9.TextAlign = ContentAlignment.MiddleLeft;
			this.label10.Location = new Point(219, 62);
			this.label10.Name = "label10";
			this.label10.Size = new Size(130, 23);
			this.label10.TabIndex = 11;
			this.label10.Text = "Move Down 1 Paragraph:";
			this.label10.TextAlign = ContentAlignment.MiddleRight;
			this.hotKeys_MoveUpOneLineTextBox.CharacterCasing = CharacterCasing.Upper;
			this.hotKeys_MoveUpOneLineTextBox.Location = new Point(175, 37);
			this.hotKeys_MoveUpOneLineTextBox.MaxLength = 1;
			this.hotKeys_MoveUpOneLineTextBox.Name = "hotKeys_MoveUpOneLineTextBox";
			this.hotKeys_MoveUpOneLineTextBox.Size = new Size(32, 20);
			this.hotKeys_MoveUpOneLineTextBox.TabIndex = 10;
			this.hotKeys_MoveUpOneLineTextBox.Text = "I";
			this.hotKeys_MoveUpOneLineTextBox.TextAlign = HorizontalAlignment.Center;
			this.label7.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.label7.Location = new Point(136, 35);
			this.label7.Name = "label7";
			this.label7.Size = new Size(44, 23);
			this.label7.TabIndex = 9;
			this.label7.Text = "Ctrl + ";
			this.label7.TextAlign = ContentAlignment.MiddleLeft;
			this.label8.Location = new Point(14, 35);
			this.label8.Name = "label8";
			this.label8.Size = new Size(116, 23);
			this.label8.TabIndex = 8;
			this.label8.Text = "Move Up 1 Line:";
			this.label8.TextAlign = ContentAlignment.MiddleRight;
			this.hotKeys_MoveDownOneLineTextBox.CharacterCasing = CharacterCasing.Upper;
			this.hotKeys_MoveDownOneLineTextBox.Location = new Point(394, 39);
			this.hotKeys_MoveDownOneLineTextBox.MaxLength = 1;
			this.hotKeys_MoveDownOneLineTextBox.Name = "hotKeys_MoveDownOneLineTextBox";
			this.hotKeys_MoveDownOneLineTextBox.Size = new Size(32, 20);
			this.hotKeys_MoveDownOneLineTextBox.TabIndex = 11;
			this.hotKeys_MoveDownOneLineTextBox.Text = "M";
			this.hotKeys_MoveDownOneLineTextBox.TextAlign = HorizontalAlignment.Center;
			this.label5.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.label5.Location = new Point(355, 37);
			this.label5.Name = "label5";
			this.label5.Size = new Size(44, 23);
			this.label5.TabIndex = 6;
			this.label5.Text = "Ctrl + ";
			this.label5.TextAlign = ContentAlignment.MiddleLeft;
			this.label6.Location = new Point(233, 37);
			this.label6.Name = "label6";
			this.label6.Size = new Size(116, 23);
			this.label6.TabIndex = 5;
			this.label6.Text = "Move Down 1 Line:";
			this.label6.TextAlign = ContentAlignment.MiddleRight;
			this.hotKeys_MoveLeftOneWordTextBox.CharacterCasing = CharacterCasing.Upper;
			this.hotKeys_MoveLeftOneWordTextBox.Location = new Point(175, 13);
			this.hotKeys_MoveLeftOneWordTextBox.MaxLength = 1;
			this.hotKeys_MoveLeftOneWordTextBox.Name = "hotKeys_MoveLeftOneWordTextBox";
			this.hotKeys_MoveLeftOneWordTextBox.Size = new Size(32, 20);
			this.hotKeys_MoveLeftOneWordTextBox.TabIndex = 8;
			this.hotKeys_MoveLeftOneWordTextBox.Text = "J";
			this.hotKeys_MoveLeftOneWordTextBox.TextAlign = HorizontalAlignment.Center;
			this.label3.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.label3.Location = new Point(136, 11);
			this.label3.Name = "label3";
			this.label3.Size = new Size(44, 23);
			this.label3.TabIndex = 3;
			this.label3.Text = "Ctrl + ";
			this.label3.TextAlign = ContentAlignment.MiddleLeft;
			this.label4.Location = new Point(14, 11);
			this.label4.Name = "label4";
			this.label4.Size = new Size(116, 23);
			this.label4.TabIndex = 2;
			this.label4.Text = "Move Left 1 Word:";
			this.label4.TextAlign = ContentAlignment.MiddleRight;
			this.hotKeys_MoveRightOneWordTextBox.CharacterCasing = CharacterCasing.Upper;
			this.hotKeys_MoveRightOneWordTextBox.Location = new Point(394, 14);
			this.hotKeys_MoveRightOneWordTextBox.MaxLength = 1;
			this.hotKeys_MoveRightOneWordTextBox.Name = "hotKeys_MoveRightOneWordTextBox";
			this.hotKeys_MoveRightOneWordTextBox.Size = new Size(32, 20);
			this.hotKeys_MoveRightOneWordTextBox.TabIndex = 9;
			this.hotKeys_MoveRightOneWordTextBox.Text = "K";
			this.hotKeys_MoveRightOneWordTextBox.TextAlign = HorizontalAlignment.Center;
			this.label2.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.label2.Location = new Point(355, 12);
			this.label2.Name = "label2";
			this.label2.Size = new Size(44, 23);
			this.label2.TabIndex = 0;
			this.label2.Text = "Ctrl + ";
			this.label2.TextAlign = ContentAlignment.MiddleLeft;
			this.label1.Location = new Point(233, 12);
			this.label1.Name = "label1";
			this.label1.Size = new Size(116, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Move Right 1 Word:";
			this.label1.TextAlign = ContentAlignment.MiddleRight;
			this.translateTabPage.Controls.Add(this.tabPage1);
			this.translateTabPage.Controls.Add(this.tabPage2);
			this.translateTabPage.Dock = DockStyle.Top;
			this.translateTabPage.Location = new Point(0, 0);
			this.translateTabPage.Name = "translateTabPage";
			this.translateTabPage.SelectedIndex = 0;
			this.translateTabPage.Size = new Size(556, 584);
			this.translateTabPage.TabIndex = 22;
			this.tabPage1.Controls.Add(this.groupBox5);
			this.tabPage1.Controls.Add(this.groupBox4);
			this.tabPage1.Controls.Add(this.groupBox2);
			this.tabPage1.Controls.Add(this.groupBox3);
			this.tabPage1.Location = new Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new Padding(3);
			this.tabPage1.Size = new Size(548, 558);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Translation";
			this.tabPage1.UseVisualStyleBackColor = true;
			this.groupBox5.Controls.Add(this.prioritizedNameCheckBox);
			this.groupBox5.Controls.Add(this.algorithm_LeftToRightRadioButton);
			this.groupBox5.Controls.Add(this.algorithm_LongestVietPhraseFirstWithConditionRadioButton);
			this.groupBox5.Controls.Add(this.algorithm_LongestVietPhraseFirstRadioButton);
			this.groupBox5.Location = new Point(8, 8);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new Size(528, 72);
			this.groupBox5.TabIndex = 5;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Thuật Toán Dịch";
			this.prioritizedNameCheckBox.Location = new Point(272, 40);
			this.prioritizedNameCheckBox.Name = "prioritizedNameCheckBox";
			this.prioritizedNameCheckBox.Size = new Size(172, 24);
			this.prioritizedNameCheckBox.TabIndex = 1;
			this.prioritizedNameCheckBox.Text = "Ưu tiên Name hơn VietPhrase";
			this.prioritizedNameCheckBox.UseVisualStyleBackColor = true;
			this.prioritizedNameCheckBox.CheckedChanged += new EventHandler(this.PrioritizedNameCheckBoxCheckedChanged);
			this.algorithm_LeftToRightRadioButton.Location = new Point(272, 16);
			this.algorithm_LeftToRightRadioButton.Name = "algorithm_LeftToRightRadioButton";
			this.algorithm_LeftToRightRadioButton.Size = new Size(232, 24);
			this.algorithm_LeftToRightRadioButton.TabIndex = 0;
			this.algorithm_LeftToRightRadioButton.TabStop = true;
			this.algorithm_LeftToRightRadioButton.Text = "Dịch từ trái sang phải";
			this.algorithm_LeftToRightRadioButton.UseVisualStyleBackColor = true;
			this.algorithm_LeftToRightRadioButton.CheckedChanged += new EventHandler(this.Algorithm_LeftToRightRadioButtonCheckedChanged);
			this.algorithm_LongestVietPhraseFirstWithConditionRadioButton.Location = new Point(8, 40);
			this.algorithm_LongestVietPhraseFirstWithConditionRadioButton.Name = "algorithm_LongestVietPhraseFirstWithConditionRadioButton";
			this.algorithm_LongestVietPhraseFirstWithConditionRadioButton.Size = new Size(232, 24);
			this.algorithm_LongestVietPhraseFirstWithConditionRadioButton.TabIndex = 0;
			this.algorithm_LongestVietPhraseFirstWithConditionRadioButton.TabStop = true;
			this.algorithm_LongestVietPhraseFirstWithConditionRadioButton.Text = "Ưu tiên cụm VietPhrase dài (>= 4 từ)";
			this.algorithm_LongestVietPhraseFirstWithConditionRadioButton.UseVisualStyleBackColor = true;
			this.algorithm_LongestVietPhraseFirstWithConditionRadioButton.CheckedChanged += new EventHandler(this.Algorithm_LongestVietPhraseFirstWithConditionRadioButtonCheckedChanged);
			this.algorithm_LongestVietPhraseFirstRadioButton.Location = new Point(8, 16);
			this.algorithm_LongestVietPhraseFirstRadioButton.Name = "algorithm_LongestVietPhraseFirstRadioButton";
			this.algorithm_LongestVietPhraseFirstRadioButton.Size = new Size(232, 24);
			this.algorithm_LongestVietPhraseFirstRadioButton.TabIndex = 0;
			this.algorithm_LongestVietPhraseFirstRadioButton.TabStop = true;
			this.algorithm_LongestVietPhraseFirstRadioButton.Text = "Ưu tiên cụm VietPhrase dài";
			this.algorithm_LongestVietPhraseFirstRadioButton.UseVisualStyleBackColor = true;
			this.algorithm_LongestVietPhraseFirstRadioButton.CheckedChanged += new EventHandler(this.Algorithm_LongestVietPhraseFirstRadioButtonCheckedChanged);
			this.tabPage2.Controls.Add(this.groupBox7);
			this.tabPage2.Controls.Add(this.groupBox6);
			this.tabPage2.Location = new Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new Padding(3);
			this.tabPage2.Size = new Size(548, 558);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Panel Style & Behaviors";
			this.tabPage2.UseVisualStyleBackColor = true;
			this.groupBox7.Controls.Add(this.alwaysFocusInVietCheckBox);
			this.groupBox7.Location = new Point(8, 392);
			this.groupBox7.Name = "groupBox7";
			this.groupBox7.Size = new Size(528, 48);
			this.groupBox7.TabIndex = 3;
			this.groupBox7.TabStop = false;
			this.groupBox7.Text = "Behaviors";
			this.alwaysFocusInVietCheckBox.Location = new Point(8, 16);
			this.alwaysFocusInVietCheckBox.Name = "alwaysFocusInVietCheckBox";
			this.alwaysFocusInVietCheckBox.Size = new Size(208, 24);
			this.alwaysFocusInVietCheckBox.TabIndex = 0;
			this.alwaysFocusInVietCheckBox.Text = "Luôn focus vào ô Việt";
			this.alwaysFocusInVietCheckBox.UseVisualStyleBackColor = true;
			this.groupBox6.Controls.Add(this.nghiaBackColorButton);
			this.groupBox6.Controls.Add(this.trungLabel);
			this.groupBox6.Controls.Add(this.vietBackColorButton);
			this.groupBox6.Controls.Add(this.hanVietLabel);
			this.groupBox6.Controls.Add(this.vietPhraseOneMeaningBackColorButton);
			this.groupBox6.Controls.Add(this.vietPhraseLabel);
			this.groupBox6.Controls.Add(this.vietPhraseBackColorButton);
			this.groupBox6.Controls.Add(this.vietPhraseOneMeaningLabel);
			this.groupBox6.Controls.Add(this.hanVietBackColorButton);
			this.groupBox6.Controls.Add(this.vietLabel);
			this.groupBox6.Controls.Add(this.trungBackColorButton);
			this.groupBox6.Controls.Add(this.nghiaLabel);
			this.groupBox6.Controls.Add(this.nghiaFontButton);
			this.groupBox6.Controls.Add(this.trungFontButton);
			this.groupBox6.Controls.Add(this.vietFontButton);
			this.groupBox6.Controls.Add(this.hanVietFontButton);
			this.groupBox6.Controls.Add(this.vietPhraseOneMeaningFontButton);
			this.groupBox6.Controls.Add(this.vietPhraseFontButton);
			this.groupBox6.Location = new Point(8, 8);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new Size(528, 368);
			this.groupBox6.TabIndex = 2;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "Panel Style";
			this.nghiaBackColorButton.Location = new Point(376, 317);
			this.nghiaBackColorButton.Name = "nghiaBackColorButton";
			this.nghiaBackColorButton.Size = new Size(75, 23);
			this.nghiaBackColorButton.TabIndex = 1;
			this.nghiaBackColorButton.Text = "Back Color";
			this.nghiaBackColorButton.UseVisualStyleBackColor = true;
			this.nghiaBackColorButton.Click += new EventHandler(this.NghiaBackColorButtonClick);
			this.trungLabel.BorderStyle = BorderStyle.FixedSingle;
			this.trungLabel.Location = new Point(72, 24);
			this.trungLabel.Name = "trungLabel";
			this.trungLabel.Size = new Size(216, 48);
			this.trungLabel.TabIndex = 0;
			this.trungLabel.Text = "Trung";
			this.trungLabel.TextAlign = ContentAlignment.MiddleCenter;
			this.vietBackColorButton.Location = new Point(376, 261);
			this.vietBackColorButton.Name = "vietBackColorButton";
			this.vietBackColorButton.Size = new Size(75, 23);
			this.vietBackColorButton.TabIndex = 1;
			this.vietBackColorButton.Text = "Back Color";
			this.vietBackColorButton.UseVisualStyleBackColor = true;
			this.vietBackColorButton.Click += new EventHandler(this.VietBackColorButtonClick);
			this.hanVietLabel.BorderStyle = BorderStyle.FixedSingle;
			this.hanVietLabel.Location = new Point(72, 80);
			this.hanVietLabel.Name = "hanVietLabel";
			this.hanVietLabel.Size = new Size(216, 48);
			this.hanVietLabel.TabIndex = 0;
			this.hanVietLabel.Text = "Hán Việt";
			this.hanVietLabel.TextAlign = ContentAlignment.MiddleCenter;
			this.vietPhraseOneMeaningBackColorButton.Location = new Point(376, 205);
			this.vietPhraseOneMeaningBackColorButton.Name = "vietPhraseOneMeaningBackColorButton";
			this.vietPhraseOneMeaningBackColorButton.Size = new Size(75, 23);
			this.vietPhraseOneMeaningBackColorButton.TabIndex = 1;
			this.vietPhraseOneMeaningBackColorButton.Text = "Back Color";
			this.vietPhraseOneMeaningBackColorButton.UseVisualStyleBackColor = true;
			this.vietPhraseOneMeaningBackColorButton.Click += new EventHandler(this.VietPhraseOneMeaningBackColorButtonClick);
			this.vietPhraseLabel.BorderStyle = BorderStyle.FixedSingle;
			this.vietPhraseLabel.Location = new Point(72, 136);
			this.vietPhraseLabel.Name = "vietPhraseLabel";
			this.vietPhraseLabel.Size = new Size(216, 48);
			this.vietPhraseLabel.TabIndex = 0;
			this.vietPhraseLabel.Text = "VietPhrase";
			this.vietPhraseLabel.TextAlign = ContentAlignment.MiddleCenter;
			this.vietPhraseBackColorButton.Location = new Point(376, 149);
			this.vietPhraseBackColorButton.Name = "vietPhraseBackColorButton";
			this.vietPhraseBackColorButton.Size = new Size(75, 23);
			this.vietPhraseBackColorButton.TabIndex = 1;
			this.vietPhraseBackColorButton.Text = "Back Color";
			this.vietPhraseBackColorButton.UseVisualStyleBackColor = true;
			this.vietPhraseBackColorButton.Click += new EventHandler(this.VietPhraseBackColorButtonClick);
			this.vietPhraseOneMeaningLabel.BorderStyle = BorderStyle.FixedSingle;
			this.vietPhraseOneMeaningLabel.Location = new Point(72, 192);
			this.vietPhraseOneMeaningLabel.Name = "vietPhraseOneMeaningLabel";
			this.vietPhraseOneMeaningLabel.Size = new Size(216, 48);
			this.vietPhraseOneMeaningLabel.TabIndex = 0;
			this.vietPhraseOneMeaningLabel.Text = "VietPhrase Một Nghĩa";
			this.vietPhraseOneMeaningLabel.TextAlign = ContentAlignment.MiddleCenter;
			this.hanVietBackColorButton.Location = new Point(376, 93);
			this.hanVietBackColorButton.Name = "hanVietBackColorButton";
			this.hanVietBackColorButton.Size = new Size(75, 23);
			this.hanVietBackColorButton.TabIndex = 1;
			this.hanVietBackColorButton.Text = "Back Color";
			this.hanVietBackColorButton.UseVisualStyleBackColor = true;
			this.hanVietBackColorButton.Click += new EventHandler(this.HanVietBackColorButtonClick);
			this.vietLabel.BorderStyle = BorderStyle.FixedSingle;
			this.vietLabel.Location = new Point(72, 248);
			this.vietLabel.Name = "vietLabel";
			this.vietLabel.Size = new Size(216, 48);
			this.vietLabel.TabIndex = 0;
			this.vietLabel.Text = "Việt";
			this.vietLabel.TextAlign = ContentAlignment.MiddleCenter;
			this.trungBackColorButton.Location = new Point(376, 37);
			this.trungBackColorButton.Name = "trungBackColorButton";
			this.trungBackColorButton.Size = new Size(75, 23);
			this.trungBackColorButton.TabIndex = 1;
			this.trungBackColorButton.Text = "Back Color";
			this.trungBackColorButton.UseVisualStyleBackColor = true;
			this.trungBackColorButton.Click += new EventHandler(this.TrungBackColorButtonClick);
			this.nghiaLabel.BorderStyle = BorderStyle.FixedSingle;
			this.nghiaLabel.Location = new Point(72, 304);
			this.nghiaLabel.Name = "nghiaLabel";
			this.nghiaLabel.Size = new Size(216, 48);
			this.nghiaLabel.TabIndex = 0;
			this.nghiaLabel.Text = "Nghĩa";
			this.nghiaLabel.TextAlign = ContentAlignment.MiddleCenter;
			this.nghiaFontButton.Location = new Point(296, 317);
			this.nghiaFontButton.Name = "nghiaFontButton";
			this.nghiaFontButton.Size = new Size(75, 23);
			this.nghiaFontButton.TabIndex = 1;
			this.nghiaFontButton.Text = "Font";
			this.nghiaFontButton.UseVisualStyleBackColor = true;
			this.nghiaFontButton.Click += new EventHandler(this.NghiaFontButtonClick);
			this.trungFontButton.Location = new Point(296, 37);
			this.trungFontButton.Name = "trungFontButton";
			this.trungFontButton.Size = new Size(75, 23);
			this.trungFontButton.TabIndex = 1;
			this.trungFontButton.Text = "Font";
			this.trungFontButton.UseVisualStyleBackColor = true;
			this.trungFontButton.Click += new EventHandler(this.TrungFontButtonClick);
			this.vietFontButton.Location = new Point(296, 261);
			this.vietFontButton.Name = "vietFontButton";
			this.vietFontButton.Size = new Size(75, 23);
			this.vietFontButton.TabIndex = 1;
			this.vietFontButton.Text = "Font";
			this.vietFontButton.UseVisualStyleBackColor = true;
			this.vietFontButton.Click += new EventHandler(this.VietFontButtonClick);
			this.hanVietFontButton.Location = new Point(296, 93);
			this.hanVietFontButton.Name = "hanVietFontButton";
			this.hanVietFontButton.Size = new Size(75, 23);
			this.hanVietFontButton.TabIndex = 1;
			this.hanVietFontButton.Text = "Font";
			this.hanVietFontButton.UseVisualStyleBackColor = true;
			this.hanVietFontButton.Click += new EventHandler(this.HanVietFontButtonClick);
			this.vietPhraseOneMeaningFontButton.Location = new Point(296, 205);
			this.vietPhraseOneMeaningFontButton.Name = "vietPhraseOneMeaningFontButton";
			this.vietPhraseOneMeaningFontButton.Size = new Size(75, 23);
			this.vietPhraseOneMeaningFontButton.TabIndex = 1;
			this.vietPhraseOneMeaningFontButton.Text = "Font";
			this.vietPhraseOneMeaningFontButton.UseVisualStyleBackColor = true;
			this.vietPhraseOneMeaningFontButton.Click += new EventHandler(this.VietPhraseOneMeaningFontButtonClick);
			this.vietPhraseFontButton.Location = new Point(296, 149);
			this.vietPhraseFontButton.Name = "vietPhraseFontButton";
			this.vietPhraseFontButton.Size = new Size(75, 23);
			this.vietPhraseFontButton.TabIndex = 1;
			this.vietPhraseFontButton.Text = "Font";
			this.vietPhraseFontButton.UseVisualStyleBackColor = true;
			this.vietPhraseFontButton.Click += new EventHandler(this.VietPhraseFontButtonClick);
			this.colorDialog.FullOpen = true;
			this.fontDialog.FontMustExist = true;
			this.fontDialog.ShowColor = true;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(556, 635);
			base.Controls.Add(this.groupBox1);
			base.Controls.Add(this.cancelButton);
			base.Controls.Add(this.saveButton);
			base.Controls.Add(this.translateTabPage);
			this.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
//			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.Name = "ConfigurationPanel";
			this.Text = "ConfigurationPanel";
			base.Load += new EventHandler(this.ConfigurationPanelLoad);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.translateTabPage.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.groupBox7.ResumeLayout(false);
			this.groupBox6.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
