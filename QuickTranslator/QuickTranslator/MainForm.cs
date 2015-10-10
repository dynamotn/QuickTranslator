//using ExtendedWebBrowser2;
//using QuickConverter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using TranslatorEngine;
using WeifenLuo.WinFormsUI.Docking;

namespace QuickTranslator
{
	public class MainForm : Form
	{
		private delegate void GenericDelegate();

		private DocumentPanel vietPhraseDocumentPanel = new DocumentPanel(true);

		private DocumentPanel hanVietDocumentPanel = new DocumentPanel(true);

		private DocumentPanel vietDocumentPanel = new DocumentPanel();

		private DocumentPanel chineseDocumentPanel = new DocumentPanel();

		private DocumentPanel meaningDocumentPanel = new DocumentPanel(true);

		private DocumentPanel vietPhraseOneMeaningDocumentPanel = new DocumentPanel(true);

//		private BrowserForm extendedBrowserPanel = new BrowserForm();

		private ConfigurationPanel configurationPanel = new ConfigurationPanel(MainForm.applicationConfigFilePath);

		private static string dockPanelConfigFilePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "QuickTranslatorDockPanel.config");

		private static string applicationConfigFilePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "QuickTranslatorMain.config");

		private static string shortcutDictionaryFilePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Shortcuts.txt");

		private CharRange[] chineseHanVietMappingArray;

		private CharRange[] chinesePhraseRanges;

		private CharRange[] vietPhraseRanges;

		private CharRange[] vietPhraseOneMeaningRanges;

		private string workingFilePath;

		private bool isNewTranslationWork;

		private DeserializeDockContent deserializeDockContent;

		private string hanVietContent;

		private string vietPhraseContent;

		private string vietPhraseOneMeaningContent;

		private string chineseContent;

		public static Configuration ActiveConfiguration = Configuration.LoadFromFile(MainForm.applicationConfigFilePath);

		private bool vietPhraseOneMeaningChanged;

		private string nextFilePath;

		private string backFilePath;

		private DateTime editingStartDateTime = DateTime.Now;

		private IContainer components;

		private ToolStripStatusLabel wordCountToolStripStatusLabel;

		private ToolStripStatusLabel toolStripStatusLabel3;

		private ToolStripButton backToolStripButton;

		private ToolStripButton nextToolStripButton;

		private ToolStripSeparator toolStripSeparator10;

		private ToolStripButton postTTVToolStripButton;

		private ToolStripMenuItem exportToWordToolStripMenuItem;

		private ToolStripSeparator toolStripSeparator7;

		private ToolStripSeparator toolStripSeparator4;

		private ToolStripSeparator toolStripSeparator2;

		private ToolStripMenuItem vietPhraseOneMeaningToolStripMenuItem;

		private ToolStripMenuItem vietPhraseToolStripMenuItem;

		private ToolStripMenuItem vietToolStripMenuItem;

		private ToolStripMenuItem hanVietToolStripMenuItem;

		private ToolStripMenuItem chineseToolStripMenuItem;

		private ToolStripDropDownButton layoutToolStripDropDownButton;

		private ToolStripSeparator toolStripButton10;

		private ToolStripButton retranslateToolStripButton;

		private ToolStripMenuItem saveToolStripMenuItem;

		private ToolStripMenuItem openToolStripMenuItem;

		private ToolStripMenuItem newWindowToolStripMenuItem;

		private ToolStripDropDownButton fileToolStripButton;

		private ToolStripSeparator toolStripSeparator9;

		private ToolStripButton updateNameToolStripButton;

		private ToolStripSeparator toolStripSeparator8;

		private ToolStripButton updateVietPhraseToolStripButton;

		private ToolStripButton reloadDictToolStripButton;

		private ToolStripLabel toolStripLabel1;

		private ToolStripButton autoScrollToolStripButton;

		private ToolStripLabel toolStripButton8;

		private ToolStripLabel toolStripButton7;

		private ToolStripLabel toolStripButton6;

		private ToolStripLabel toolStripButton5;

		private ToolStripLabel toolStripButton4;

		private ToolStripLabel toolStripButton3;

		private ToolStripLabel toolStripButton2;

		private ToolStripLabel toolStripButton1;

		private ToolStripSeparator toolStripSeparator6;

		private ToolStripSeparator toolStripSeparator5;

		private ToolStripSeparator toolStripSeparator1;

		private ToolStripStatusLabel toolStripStatusLabel2;

		private ToolStripSeparator toolStripSeparator3;

		private ToolStripButton translateFromClipboardToolStripButton;

		private ToolStripStatusLabel toolStripStatusLabel1;

		private StatusStrip statusStrip1;

		private ToolStrip toolStrip1;

		private DockPanel dockPanel;

		public MainForm()
		{
			this.InitializeComponent();
			this.deserializeDockContent = new DeserializeDockContent(this.GetContentFromPersistString);
		}

		private void MainFormLoad(object sender, EventArgs e)
		{
			this.hanVietDocumentPanel.Text = "Hán Việt";
			this.hanVietDocumentPanel.CloseButton = false;
			this.hanVietDocumentPanel.SetFontSize(12f);
			this.chineseDocumentPanel.Text = "Trung";
			this.chineseDocumentPanel.CloseButton = false;
			this.chineseDocumentPanel.SetFontSize(14f);
			this.vietPhraseDocumentPanel.Text = "VietPhrase";
			this.vietPhraseDocumentPanel.CloseButton = false;
			this.vietPhraseDocumentPanel.SetFontSize(12f);
			this.vietPhraseOneMeaningDocumentPanel.Text = "VietPhrase một nghĩa";
			this.vietPhraseOneMeaningDocumentPanel.CloseButton = false;
			this.vietPhraseOneMeaningDocumentPanel.SetFontSize(12f);
			this.vietPhraseOneMeaningDocumentPanel.contentRichTextBox.AcceptsTab = true;
			this.meaningDocumentPanel.Text = "Nghĩa";
			this.meaningDocumentPanel.CloseButton = false;
			this.meaningDocumentPanel.SetFontSize(10f);
			this.vietDocumentPanel.Text = "Việt";
			this.vietDocumentPanel.CloseButton = false;
			this.vietDocumentPanel.SetFontSize(12f);
			this.SetPanelStyle();
			this.configurationPanel.Text = "Configuration";
			this.configurationPanel.CloseButton = false;
//			this.extendedBrowserPanel.Text = "Quick Web Browser";
//			this.extendedBrowserPanel.CloseButton = false;
			this.hanVietDocumentPanel.EnableAddToVietPhraseContextMenuStrip();
			this.chineseDocumentPanel.EnableAddToVietPhraseContextMenuStrip();
			this.vietPhraseDocumentPanel.EnableAddToVietPhraseContextMenuStrip();
			this.vietPhraseOneMeaningDocumentPanel.EnableAddToVietPhraseContextMenuStrip();
			this.vietDocumentPanel.EnableCommentContextMenuStrip();
			this.meaningDocumentPanel.EnableAddToVietPhraseContextMenuStrip();
//			this.extendedBrowserPanel.fullScreenToolStripButton.Enabled = false;
//			this.extendedBrowserPanel.fullScreenToolStripButton.Visible = false;
			this.hanVietDocumentPanel.AllowDeletingSelectedText();
			this.chineseDocumentPanel.AllowDeletingSelectedText();
			if (File.Exists(MainForm.dockPanelConfigFilePath))
			{
				this.dockPanel.LoadFromXml(MainForm.dockPanelConfigFilePath, this.deserializeDockContent);
			}
			else
			{
				this.configurationPanel.Show(this.dockPanel, DockState.DockLeftAutoHide);
//				this.extendedBrowserPanel.Show(this.dockPanel, DockState.Document);
				this.vietDocumentPanel.Show(this.dockPanel, DockState.Document);
				this.meaningDocumentPanel.Show(this.dockPanel, DockState.Document);
				this.vietPhraseOneMeaningDocumentPanel.Show(this.dockPanel, DockState.Document);
				this.vietPhraseDocumentPanel.Show(this.dockPanel, DockState.Document);
				this.chineseDocumentPanel.Show(this.dockPanel, DockState.Document);
				this.hanVietDocumentPanel.Show(this.dockPanel, DockState.Document);
			}
			this.isNewTranslationWork = true;
			this.setFormTitle();
			this.chineseDocumentPanel.ClickHandler = new DocumentPanel.ClickDelegate(this.ChineseClick);
			this.chineseDocumentPanel.AddToVietPhraseHandler = new DocumentPanel.AddToVietPhraseDelegate(this.Chinese_AddToVietPhraseHandler);
			this.chineseDocumentPanel.SelectTextHandler = new DocumentPanel.SelectTextDelegate(this.Chinese_SelectTextHandler);
			this.chineseDocumentPanel.BaikeingHandler = new DocumentPanel.BaikeingDelegate(this.BaikeingHandler);
			this.chineseDocumentPanel.NcikuingHandler = new DocumentPanel.NcikuingDelegate(this.NcikuingHandler);
			this.chineseDocumentPanel.CopyToVietHandler = new DocumentPanel.CopyToVietDelegate(this.CopyToVietHandler);
			this.chineseDocumentPanel.DeleteSelectedTextHandler = new DocumentPanel.DeleteSelectedTextDelegate(this.DeleteSelectedTextHandler);
			this.chineseDocumentPanel.AddToPhienAmHandler = new DocumentPanel.AddToPhienAmDelegate(this.AddToPhienAmHandler);
			this.hanVietDocumentPanel.ClickHandler = new DocumentPanel.ClickDelegate(this.HanVietClick);
			this.hanVietDocumentPanel.RightClickHandler = new DocumentPanel.RightClickDelegate(this.HanVietRightClick);
			this.hanVietDocumentPanel.AddToVietPhraseHandler = new DocumentPanel.AddToVietPhraseDelegate(this.HanViet_AddToVietPhraseHandler);
			this.hanVietDocumentPanel.SelectTextHandler = new DocumentPanel.SelectTextDelegate(this.HanViet_SelectTextHandler);
			this.hanVietDocumentPanel.BaikeingHandler = new DocumentPanel.BaikeingDelegate(this.BaikeingHandler);
			this.hanVietDocumentPanel.NcikuingHandler = new DocumentPanel.NcikuingDelegate(this.NcikuingHandler);
			this.hanVietDocumentPanel.CopyToVietHandler = new DocumentPanel.CopyToVietDelegate(this.CopyToVietHandler);
			this.hanVietDocumentPanel.DeleteSelectedTextHandler = new DocumentPanel.DeleteSelectedTextDelegate(this.DeleteSelectedTextHandler);
			this.hanVietDocumentPanel.AddToPhienAmHandler = new DocumentPanel.AddToPhienAmDelegate(this.AddToPhienAmHandler);
			this.vietPhraseDocumentPanel.ClickHandler = new DocumentPanel.ClickDelegate(this.VietPhraseClick);
			this.vietPhraseDocumentPanel.RightClickHandler = new DocumentPanel.RightClickDelegate(this.VietPhraseRightClick);
			this.vietPhraseDocumentPanel.AddToVietPhraseHandler = new DocumentPanel.AddToVietPhraseDelegate(this.VietPhrase_AddToVietPhraseHandler);
			this.vietPhraseDocumentPanel.SelectTextHandler = new DocumentPanel.SelectTextDelegate(this.VietPhrase_SelectTextHandler);
			this.vietPhraseDocumentPanel.BaikeingHandler = new DocumentPanel.BaikeingDelegate(this.BaikeingHandler);
			this.vietPhraseDocumentPanel.NcikuingHandler = new DocumentPanel.NcikuingDelegate(this.NcikuingHandler);
			this.vietPhraseDocumentPanel.CopyToVietHandler = new DocumentPanel.CopyToVietDelegate(this.CopyToVietHandler);
			this.vietPhraseDocumentPanel.AddToPhienAmHandler = new DocumentPanel.AddToPhienAmDelegate(this.AddToPhienAmHandler);
			this.vietPhraseOneMeaningDocumentPanel.ClickHandler = new DocumentPanel.ClickDelegate(this.VietPhraseOneMeaningClick);
			this.vietPhraseOneMeaningDocumentPanel.RightClickHandler = new DocumentPanel.RightClickDelegate(this.VietPhraseOneMeaningRightClick);
			this.vietPhraseOneMeaningDocumentPanel.AddToVietPhraseHandler = new DocumentPanel.AddToVietPhraseDelegate(this.VietPhraseOneMeaning_AddToVietPhraseHandler);
			this.vietPhraseOneMeaningDocumentPanel.SelectTextHandler = new DocumentPanel.SelectTextDelegate(this.VietPhraseOneMeaning_SelectTextHandler);
			this.vietPhraseOneMeaningDocumentPanel.BaikeingHandler = new DocumentPanel.BaikeingDelegate(this.BaikeingHandler);
			this.vietPhraseOneMeaningDocumentPanel.NcikuingHandler = new DocumentPanel.NcikuingDelegate(this.NcikuingHandler);
			this.vietPhraseOneMeaningDocumentPanel.CopyToVietHandler = new DocumentPanel.CopyToVietDelegate(this.CopyToVietHandler);
			this.vietPhraseOneMeaningDocumentPanel.AddToPhienAmHandler = new DocumentPanel.AddToPhienAmDelegate(this.AddToPhienAmHandler);
			this.vietPhraseOneMeaningDocumentPanel.ShiftAndMouseMoveHandler = new DocumentPanel.ShiftAndMouseMoveDelegate(this.ShiftAndMouseMoveHandler);
			this.vietPhraseOneMeaningDocumentPanel.ShiftUpHandler = new DocumentPanel.ShiftUpDelegate(this.ShiftUpHandler);
			this.meaningDocumentPanel.AddToVietPhraseHandler = new DocumentPanel.AddToVietPhraseDelegate(this.Meaning_AddToVietPhraseHandler);
			this.meaningDocumentPanel.BaikeingHandler = new DocumentPanel.BaikeingDelegate(this.BaikeingHandler);
			this.meaningDocumentPanel.NcikuingHandler = new DocumentPanel.NcikuingDelegate(this.NcikuingHandler);
			this.meaningDocumentPanel.CopyToVietHandler = new DocumentPanel.CopyToVietDelegate(this.CopyToVietHandler);
			this.meaningDocumentPanel.AddToPhienAmHandler = new DocumentPanel.AddToPhienAmDelegate(this.AddToPhienAmHandler);
			TranslatorEngine.TranslatorEngine.LoadDictionaries();
			Shortcuts.LoadFromFile(MainForm.shortcutDictionaryFilePath);
			this.vietPhraseToolStripMenuItem.Checked = MainForm.ActiveConfiguration.Layout_VietPhrase;
			this.vietPhraseOneMeaningToolStripMenuItem.Checked = MainForm.ActiveConfiguration.Layout_VietPhraseOneMeaning;
			this.hanVietDocumentPanel.Activate();
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (msg.Msg == 256 || msg.Msg == 260)
			{
				if (keyData == (Keys.Control | (Keys)((byte)MainForm.ActiveConfiguration.HotKeys_MoveRightOneWord)))
				{
					this.nextHanVietWord();
				}
				else if (keyData == (Keys.Control | (Keys)((byte)MainForm.ActiveConfiguration.HotKeys_MoveLeftOneWord)))
				{
					this.previousHanVietWord();
				}
				else if (keyData == (Keys.Control | (Keys)((byte)MainForm.ActiveConfiguration.HotKeys_MoveDownOneLine)))
				{
					this.HanVietClick(this.hanVietDocumentPanel.getNextLineIndex());
				}
				else if (keyData == (Keys.Control | (Keys)((byte)MainForm.ActiveConfiguration.HotKeys_MoveUpOneLine)))
				{
					this.HanVietClick(this.hanVietDocumentPanel.getPreviousLineIndex());
				}
				else if (keyData == (Keys.Control | (Keys)((byte)MainForm.ActiveConfiguration.HotKeys_MoveDownOneParagraph)))
				{
					this.HanVietClick(this.hanVietDocumentPanel.getNextPhysicalLineIndex());
				}
				else if (keyData == (Keys.Control | (Keys)((byte)MainForm.ActiveConfiguration.HotKeys_MoveUpOneParagraph)))
				{
					this.HanVietClick(this.hanVietDocumentPanel.getPreviousPhysicalLineIndex());
				}
				else if (keyData == (Keys.Control | (Keys)((byte)MainForm.ActiveConfiguration.HotKeys_CopyHighlightedHanViet)))
				{
					this.CopyToVietHandler(this.hanVietDocumentPanel.GetHighlightText());
				}
				else if (keyData == (Keys)131142)
				{
					Form form = new FindAndReplaceForm(this.chineseDocumentPanel.contentRichTextBox);
					form.Show(this);
				}
				else if (keyData == (Keys.LButton | Keys.RButton | Keys.MButton | Keys.Space | Keys.Alt))
				{
					if (this.nextToolStripButton.Enabled)
					{
						this.OpenFile(this.nextFilePath);
					}
				}
				else if (keyData == (Keys.LButton | Keys.MButton | Keys.Space | Keys.Alt))
				{
					if (this.backToolStripButton.Enabled)
					{
						this.OpenFile(this.backFilePath);
					}
				}
				else
				{
					if (!MainForm.ActiveConfiguration.Layout_VietPhrase)
					{
						return base.ProcessCmdKey(ref msg, keyData);
					}
					if (keyData == (Keys.Control | (Keys)((byte)MainForm.ActiveConfiguration.HotKeys_CopyMeaning1)))
					{
						this.copyMeaning(1);
					}
					else if (keyData == (Keys.Control | (Keys)((byte)MainForm.ActiveConfiguration.HotKeys_CopyMeaning2)))
					{
						this.copyMeaning(2);
					}
					else if (keyData == (Keys.Control | (Keys)((byte)MainForm.ActiveConfiguration.HotKeys_CopyMeaning3)))
					{
						this.copyMeaning(3);
					}
					else if (keyData == (Keys.Control | (Keys)((byte)MainForm.ActiveConfiguration.HotKeys_CopyMeaning4)))
					{
						this.copyMeaning(4);
					}
					else if (keyData == (Keys.Control | (Keys)((byte)MainForm.ActiveConfiguration.HotKeys_CopyMeaning5)))
					{
						this.copyMeaning(5);
					}
					else
					{
						if (keyData != (Keys.Control | (Keys)((byte)MainForm.ActiveConfiguration.HotKeys_CopyMeaning6)))
						{
							return base.ProcessCmdKey(ref msg, keyData);
						}
						this.copyMeaning(6);
					}
				}
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		private void TranslateFromClipboardToolStripButtonClick(object sender, EventArgs e)
		{
			this.isNewTranslationWork = true;
			string original = "";
			try
			{
				original = Clipboard.GetText();
			}
			catch
			{
			}
			this.chineseDocumentPanel.SetTextContent(TranslatorEngine.TranslatorEngine.StandardizeInput(original));
			this.Translate(-2, -2, -2);
			this.Text = "Quick Translator - Untitled";
			this.workingFilePath = "";
			this.toggleNextBackButtons();
		}

		public void Translate(int currentHanVietDisplayedLine, int currentVietPhraseDisplayedLine, int currentVietPhraseOneMeaningDisplayedLine)
		{
			this.chineseContent = this.chineseDocumentPanel.GetTextContent();
			if (string.IsNullOrEmpty(this.chineseContent) || string.IsNullOrEmpty(this.chineseContent.Trim()))
			{
				return;
			}
			this.translateHanViet(currentHanVietDisplayedLine);
			this.translateVietPhraseOneMeaning(currentVietPhraseOneMeaningDisplayedLine);
			this.translateVietPhrase(currentVietPhraseDisplayedLine);
			this.calculateChineseWordCount();
			this.vietPhraseOneMeaningChanged = false;
		}

		private void translateHanViet(int currentDisplayedLine)
		{
			new Thread(delegate()
			{
				lock (TranslatorEngine.TranslatorEngine.LastTranslatedWord_HanViet)
				{
					if (!string.IsNullOrEmpty(this.chineseContent))
					{
						string text = TranslatorEngine.TranslatorEngine.ChineseToHanViet(this.chineseContent, out this.chineseHanVietMappingArray);
						this.updateDocumentPanel(this.hanVietDocumentPanel, text, currentDisplayedLine);
					}
				}
			})
			{
				IsBackground = true
			}.Start();
		}

		private void translateVietPhraseOneMeaning(int currentDisplayedLine)
		{
			if (!this.vietPhraseOneMeaningDocumentPanel.IsHidden)
			{
				new Thread(delegate()
				{
					lock (TranslatorEngine.TranslatorEngine.LastTranslatedWord_VietPhraseOneMeaning)
					{
						if (!string.IsNullOrEmpty(this.chineseContent))
						{
							string text;
							if (MainForm.ActiveConfiguration.Layout_VietPhrase)
							{
								CharRange[] array;
								text = TranslatorEngine.TranslatorEngine.ChineseToVietPhraseOneMeaning(this.chineseContent, MainForm.ActiveConfiguration.VietPhraseOneMeaning_Wrap, MainForm.ActiveConfiguration.TranslationAlgorithm, MainForm.ActiveConfiguration.PrioritizedName, out array, out this.vietPhraseOneMeaningRanges);
							}
							else
							{
								text = TranslatorEngine.TranslatorEngine.ChineseToVietPhraseOneMeaning(this.chineseContent, MainForm.ActiveConfiguration.VietPhraseOneMeaning_Wrap, MainForm.ActiveConfiguration.TranslationAlgorithm, MainForm.ActiveConfiguration.PrioritizedName, out this.chinesePhraseRanges, out this.vietPhraseOneMeaningRanges);
							}
							this.updateDocumentPanel(this.vietPhraseOneMeaningDocumentPanel, text, currentDisplayedLine);
						}
					}
				})
				{
					IsBackground = true
				}.Start();
			}
		}

		private void translateVietPhrase(int currentDisplayedLine)
		{
			if (!this.vietPhraseDocumentPanel.IsHidden)
			{
				new Thread(delegate()
				{
					lock (TranslatorEngine.TranslatorEngine.LastTranslatedWord_VietPhrase)
					{
						if (!string.IsNullOrEmpty(this.chineseContent))
						{
							string text = TranslatorEngine.TranslatorEngine.ChineseToVietPhrase(this.chineseContent, MainForm.ActiveConfiguration.VietPhrase_Wrap, MainForm.ActiveConfiguration.TranslationAlgorithm, MainForm.ActiveConfiguration.PrioritizedName, out this.chinesePhraseRanges, out this.vietPhraseRanges);
							this.updateDocumentPanel(this.vietPhraseDocumentPanel, text, currentDisplayedLine);
						}
					}
				})
				{
					IsBackground = true
				}.Start();
			}
		}

		private void updateDocumentPanel(DocumentPanel panel, string text, int currentDisplayedLine)
		{
			MainForm.GenericDelegate genericDelegate = delegate
			{
				panel.SetTextContent(text);
				if (currentDisplayedLine <= -2)
				{
					panel.ScrollToTop();
					return;
				}
				if (currentDisplayedLine == -1)
				{
					int physicalLine = this.countPhysicalLines(this.vietDocumentPanel.GetTextContent());
					panel.ScrollToASpecificPhysicalLine(physicalLine);
					return;
				}
				panel.ScrollToASpecificLogicalLine(currentDisplayedLine);
			};
			if (!panel.IsHandleCreated)
			{
				this.CreateHandle();
			}
			if (panel.InvokeRequired)
			{
				panel.BeginInvoke(genericDelegate);
				return;
			}
			genericDelegate();
		}

		public void SetPanelStyle()
		{
			this.chineseDocumentPanel.SetFont(MainForm.ActiveConfiguration.Style_TrungFont);
			this.chineseDocumentPanel.SetForeColor(MainForm.ActiveConfiguration.Style_TrungForeColor);
			this.chineseDocumentPanel.SetBackColor(MainForm.ActiveConfiguration.Style_TrungBackColor);
			this.hanVietDocumentPanel.SetFont(MainForm.ActiveConfiguration.Style_HanVietFont);
			this.hanVietDocumentPanel.SetForeColor(MainForm.ActiveConfiguration.Style_HanVietForeColor);
			this.hanVietDocumentPanel.SetBackColor(MainForm.ActiveConfiguration.Style_HanVietBackColor);
			this.vietPhraseDocumentPanel.SetFont(MainForm.ActiveConfiguration.Style_VietPhraseFont);
			this.vietPhraseDocumentPanel.SetForeColor(MainForm.ActiveConfiguration.Style_VietPhraseForeColor);
			this.vietPhraseDocumentPanel.SetBackColor(MainForm.ActiveConfiguration.Style_VietPhraseBackColor);
			this.vietPhraseOneMeaningDocumentPanel.SetFont(MainForm.ActiveConfiguration.Style_VietPhraseOneMeaningFont);
			this.vietPhraseOneMeaningDocumentPanel.SetForeColor(MainForm.ActiveConfiguration.Style_VietPhraseOneMeaningForeColor);
			this.vietPhraseOneMeaningDocumentPanel.SetBackColor(MainForm.ActiveConfiguration.Style_VietPhraseOneMeaningBackColor);
			this.vietDocumentPanel.SetFont(MainForm.ActiveConfiguration.Style_VietFont);
			this.vietDocumentPanel.SetForeColor(MainForm.ActiveConfiguration.Style_VietForeColor);
			this.vietDocumentPanel.SetBackColor(MainForm.ActiveConfiguration.Style_VietBackColor);
			this.meaningDocumentPanel.SetFont(MainForm.ActiveConfiguration.Style_NghiaFont);
			this.meaningDocumentPanel.SetForeColor(MainForm.ActiveConfiguration.Style_NghiaForeColor);
			this.meaningDocumentPanel.SetBackColor(MainForm.ActiveConfiguration.Style_NghiaBackColor);
		}

		private void chineseToHanVietWithNewThread(object stateInfo)
		{
			object[] array = (object[])stateInfo;
			this.hanVietContent = TranslatorEngine.TranslatorEngine.ChineseToHanViet((string)array[0], out this.chineseHanVietMappingArray);
			((ManualResetEvent)array[1]).Set();
		}

		private void chineseToVietPhraseWithNewThread(object stateInfo)
		{
			object[] array = (object[])stateInfo;
			this.vietPhraseContent = TranslatorEngine.TranslatorEngine.ChineseToVietPhrase((string)array[0], MainForm.ActiveConfiguration.VietPhrase_Wrap, MainForm.ActiveConfiguration.TranslationAlgorithm, MainForm.ActiveConfiguration.PrioritizedName, out this.chinesePhraseRanges, out this.vietPhraseRanges);
			((ManualResetEvent)array[1]).Set();
		}

		private void chineseToVietPhraseOneMeaningWithNewThread(object stateInfo)
		{
			object[] array = (object[])stateInfo;
			CharRange[] array2;
			this.vietPhraseOneMeaningContent = TranslatorEngine.TranslatorEngine.ChineseToVietPhraseOneMeaning((string)array[0], MainForm.ActiveConfiguration.VietPhraseOneMeaning_Wrap, MainForm.ActiveConfiguration.TranslationAlgorithm, MainForm.ActiveConfiguration.PrioritizedName, out array2, out this.vietPhraseOneMeaningRanges);
			((ManualResetEvent)array[1]).Set();
		}

		private void previousHanVietWord()
		{
			this.HanVietClick(this.hanVietDocumentPanel.getPreviousWordIndex());
		}

		private void nextHanVietWord()
		{
			this.HanVietClick(this.hanVietDocumentPanel.getNextWordIndex());
		}

		private void setFormTitle()
		{
			this.Text = "Quick Translator";
			this.Text += (this.isNewTranslationWork ? " - Untitled" : this.workingFilePath);
		}

		private void HanVietClick(int currentCharIndex)
		{
			try
			{
				this.HanVietClickWithoutHandlingException(currentCharIndex);
			}
			catch (Exception exception)
			{
				ApplicationLog.Log(Path.GetDirectoryName(Application.ExecutablePath), "QuickTranslator", exception);
			}
		}

		private void HanVietClickWithoutHandlingException(int currentCharIndex)
		{
			if (this.hanVietDocumentPanel.GetTextContent().Length - 1 < currentCharIndex)
			{
				return;
			}
			int num = this.findChineseCharIndexFromHanVietIndex(currentCharIndex);
			if (num < 0)
			{
				return;
			}
			int num2;
			string text = TranslatorEngine.TranslatorEngine.ChineseToMeanings(this.chineseDocumentPanel.GetTextContent().Substring(num), out num2);
			this.meaningDocumentPanel.SetTextContent(text.Replace("\\n", "\n").Replace("\\t", "\t"));
			this.meaningDocumentPanel.ScrollToTop();
			this.chineseDocumentPanel.HighlightText(num, num2, true, true);
			CharRange hanVietCharRangeFromChineseRange = this.getHanVietCharRangeFromChineseRange(num, num2);
			this.hanVietDocumentPanel.HighlightText(hanVietCharRangeFromChineseRange.StartIndex, hanVietCharRangeFromChineseRange.Length, true, false);
			if (MainForm.ActiveConfiguration.Layout_VietPhrase)
			{
				CharRange vietPhraseCharRangeFromChineseIndex = this.getVietPhraseCharRangeFromChineseIndex(num);
				this.vietPhraseDocumentPanel.HighlightText(vietPhraseCharRangeFromChineseIndex.StartIndex, vietPhraseCharRangeFromChineseIndex.Length, true, true);
			}
			if (MainForm.ActiveConfiguration.Layout_VietPhraseOneMeaning)
			{
				CharRange vietPhraseOneMeaningCharRangeFromChineseIndex = this.getVietPhraseOneMeaningCharRangeFromChineseIndex(num);
				this.vietPhraseOneMeaningDocumentPanel.HighlightText(vietPhraseOneMeaningCharRangeFromChineseIndex.StartIndex, vietPhraseOneMeaningCharRangeFromChineseIndex.Length, true, true);
			}
			if (MainForm.ActiveConfiguration.AlwaysFocusInViet)
			{
				this.vietDocumentPanel.FocusInRichTextBox();
			}
		}

		private void ChineseClick(int chineseCharIndex)
		{
			try
			{
				this.ChineseClickWithoutHandlingException(chineseCharIndex);
			}
			catch (Exception exception)
			{
				ApplicationLog.Log(Path.GetDirectoryName(Application.ExecutablePath), "QuickTranslator", exception);
			}
		}

		private void ChineseClickWithoutHandlingException(int chineseCharIndex)
		{
			if (this.chineseDocumentPanel.GetTextContent().Length - 1 < chineseCharIndex)
			{
				return;
			}
			int num;
			string text = TranslatorEngine.TranslatorEngine.ChineseToMeanings(this.chineseDocumentPanel.GetTextContent().Substring(chineseCharIndex), out num);
			this.meaningDocumentPanel.SetTextContent(text.Replace("\\n", "\n").Replace("\\t", "\t"));
			this.meaningDocumentPanel.ScrollToTop();
			this.chineseDocumentPanel.HighlightText(chineseCharIndex, num, true, false);
			CharRange hanVietCharRangeFromChineseRange = this.getHanVietCharRangeFromChineseRange(chineseCharIndex, num);
			this.hanVietDocumentPanel.HighlightText(hanVietCharRangeFromChineseRange.StartIndex, hanVietCharRangeFromChineseRange.Length, true, true);
			if (MainForm.ActiveConfiguration.Layout_VietPhrase)
			{
				CharRange vietPhraseCharRangeFromChineseIndex = this.getVietPhraseCharRangeFromChineseIndex(chineseCharIndex);
				this.vietPhraseDocumentPanel.HighlightText(vietPhraseCharRangeFromChineseIndex.StartIndex, vietPhraseCharRangeFromChineseIndex.Length, true, true);
			}
			if (MainForm.ActiveConfiguration.Layout_VietPhraseOneMeaning)
			{
				CharRange vietPhraseOneMeaningCharRangeFromChineseIndex = this.getVietPhraseOneMeaningCharRangeFromChineseIndex(chineseCharIndex);
				this.vietPhraseOneMeaningDocumentPanel.HighlightText(vietPhraseOneMeaningCharRangeFromChineseIndex.StartIndex, vietPhraseOneMeaningCharRangeFromChineseIndex.Length, true, true);
			}
			if (MainForm.ActiveConfiguration.AlwaysFocusInViet)
			{
				this.vietDocumentPanel.FocusInRichTextBox();
			}
		}

		private int findChineseCharIndexFromHanVietIndex(int hanVietCharIndex)
		{
			int result = -1;
			for (int i = 0; i < this.chineseHanVietMappingArray.Length; i++)
			{
				if (this.chineseHanVietMappingArray[i].StartIndex <= hanVietCharIndex && hanVietCharIndex <= this.chineseHanVietMappingArray[i].GetEndIndex())
				{
					result = i;
					break;
				}
			}
			return result;
		}

		private void VietPhraseClick(int currentCharIndex)
		{
			try
			{
				this.VietPhraseClickWithoutHandlingException(currentCharIndex);
			}
			catch (Exception exception)
			{
				ApplicationLog.Log(Path.GetDirectoryName(Application.ExecutablePath), "QuickTranslator", exception);
			}
		}

		private void VietPhraseClickWithoutHandlingException(int currentCharIndex)
		{
			if (this.vietPhraseDocumentPanel.GetTextContent().Length - 1 < currentCharIndex)
			{
				return;
			}
			if (this.getChineseCharRangeFromVietPhraseIndex(currentCharIndex) == null)
			{
				return;
			}
			int startIndex = this.getChineseCharRangeFromVietPhraseIndex(currentCharIndex).StartIndex;
			int num;
			string text = TranslatorEngine.TranslatorEngine.ChineseToMeanings(this.chineseDocumentPanel.GetTextContent().Substring(startIndex), out num);
			this.meaningDocumentPanel.SetTextContent(text.Replace("\\n", "\n").Replace("\\t", "\t"));
			this.meaningDocumentPanel.ScrollToTop();
			this.chineseDocumentPanel.HighlightText(startIndex, num, true, true);
			CharRange hanVietCharRangeFromChineseRange = this.getHanVietCharRangeFromChineseRange(startIndex, num);
			this.hanVietDocumentPanel.HighlightText(hanVietCharRangeFromChineseRange.StartIndex, hanVietCharRangeFromChineseRange.Length, true, true);
			CharRange vietPhraseCharRangeFromChineseIndex = this.getVietPhraseCharRangeFromChineseIndex(startIndex);
			this.vietPhraseDocumentPanel.HighlightText(vietPhraseCharRangeFromChineseIndex.StartIndex, vietPhraseCharRangeFromChineseIndex.Length, true, false);
			if (MainForm.ActiveConfiguration.Layout_VietPhraseOneMeaning)
			{
				CharRange vietPhraseOneMeaningCharRangeFromChineseIndex = this.getVietPhraseOneMeaningCharRangeFromChineseIndex(startIndex);
				this.vietPhraseOneMeaningDocumentPanel.HighlightText(vietPhraseOneMeaningCharRangeFromChineseIndex.StartIndex, vietPhraseOneMeaningCharRangeFromChineseIndex.Length, true, true);
			}
			if (MainForm.ActiveConfiguration.AlwaysFocusInViet)
			{
				this.vietDocumentPanel.FocusInRichTextBox();
			}
		}

		private void VietPhraseOneMeaningClick(int currentCharIndex)
		{
			try
			{
				this.VietPhraseOneMeaningClickWithoutHandlingException(currentCharIndex);
			}
			catch (Exception exception)
			{
				ApplicationLog.Log(Path.GetDirectoryName(Application.ExecutablePath), "QuickTranslator", exception);
			}
		}

		private void VietPhraseOneMeaningClickWithoutHandlingException(int currentCharIndex)
		{
			if (this.vietPhraseOneMeaningDocumentPanel.GetTextContent().Length - 1 < currentCharIndex)
			{
				return;
			}
			CharRange chineseCharRangeFromVietPhraseOneMeaningIndex = this.getChineseCharRangeFromVietPhraseOneMeaningIndex(currentCharIndex);
			if (chineseCharRangeFromVietPhraseOneMeaningIndex == null)
			{
				return;
			}
			int startIndex = chineseCharRangeFromVietPhraseOneMeaningIndex.StartIndex;
			int num;
			string text = TranslatorEngine.TranslatorEngine.ChineseToMeanings(this.chineseDocumentPanel.GetTextContent().Substring(startIndex), out num);
			this.meaningDocumentPanel.SetTextContent(text.Replace("\\n", "\n").Replace("\\t", "\t"));
			this.meaningDocumentPanel.ScrollToTop();
			this.chineseDocumentPanel.HighlightText(startIndex, num, true, true);
			CharRange hanVietCharRangeFromChineseRange = this.getHanVietCharRangeFromChineseRange(startIndex, num);
			this.hanVietDocumentPanel.HighlightText(hanVietCharRangeFromChineseRange.StartIndex, hanVietCharRangeFromChineseRange.Length, true, true);
			if (MainForm.ActiveConfiguration.Layout_VietPhrase)
			{
				CharRange vietPhraseCharRangeFromChineseIndex = this.getVietPhraseCharRangeFromChineseIndex(startIndex);
				this.vietPhraseDocumentPanel.HighlightText(vietPhraseCharRangeFromChineseIndex.StartIndex, vietPhraseCharRangeFromChineseIndex.Length, true, true);
			}
			CharRange vietPhraseOneMeaningCharRangeFromChineseIndex = this.getVietPhraseOneMeaningCharRangeFromChineseIndex(startIndex);
			string text2 = TranslatorEngine.TranslatorEngine.GetVietPhraseOrNameValueFromKey(this.chineseContent.Substring(startIndex, num));
			if (this.vietPhraseOneMeaningDocumentPanel.CurrentHighlightedTextStartIndex == vietPhraseOneMeaningCharRangeFromChineseIndex.StartIndex && this.vietPhraseOneMeaningDocumentPanel.CurrentHighlightedTextLength == vietPhraseOneMeaningCharRangeFromChineseIndex.Length && !"\n".Equals(this.vietPhraseOneMeaningDocumentPanel.GetHighlightText()))
			{
				if (string.IsNullOrEmpty(text2))
				{
					text2 = string.Empty;
				}
				string[] array = text2.Split("/|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				string value = this.vietPhraseOneMeaningDocumentPanel.GetTextContent().Substring(this.vietPhraseOneMeaningDocumentPanel.CurrentHighlightedTextStartIndex, this.vietPhraseOneMeaningDocumentPanel.CurrentHighlightedTextLength);
				this.vietPhraseOneMeaningDocumentPanel.chooseMeaningContextMenuStrip.Items.Clear();
				if (1 < array.Length)
				{
					string[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						string text3 = array2[i];
						if (!text3.Equals(value, StringComparison.InvariantCultureIgnoreCase))
						{
							StringBuilder stringBuilder = new StringBuilder();
							stringBuilder.AppendLine(this.findChineseMappingId(startIndex).ToString());
							stringBuilder.AppendLine(value);
							stringBuilder.AppendLine(this.chineseDocumentPanel.GetTextContent().Substring(startIndex, num));
							stringBuilder.AppendLine(text3);
							ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(text3);
							toolStripMenuItem.Tag = stringBuilder.ToString();
							toolStripMenuItem.Click += new EventHandler(this.chooseAMeaningMenuItem_Click);
							ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem("Update VietPhrase");
							toolStripMenuItem2.Tag = stringBuilder.ToString();
							toolStripMenuItem2.Click += new EventHandler(this.chooseAMeaningMenuItem_Click);
							toolStripMenuItem.DropDownItems.Add(toolStripMenuItem2);
							this.vietPhraseOneMeaningDocumentPanel.chooseMeaningContextMenuStrip.Items.Add(toolStripMenuItem);
						}
					}
					this.vietPhraseOneMeaningDocumentPanel.chooseMeaningContextMenuStrip.Items.Add(new ToolStripSeparator());
				}
				ToolStripTextBox toolStripTextBox = new ToolStripTextBox();
				toolStripTextBox.BorderStyle = BorderStyle.FixedSingle;
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder2.AppendLine(this.findChineseMappingId(startIndex).ToString());
				stringBuilder2.AppendLine(value);
				stringBuilder2.AppendLine(this.chineseDocumentPanel.GetTextContent().Substring(startIndex, num));
				toolStripTextBox.Tag = stringBuilder2.ToString();
				toolStripTextBox.KeyPress += new KeyPressEventHandler(this.newMeaningTextBox_KeyPress);
				this.vietPhraseOneMeaningDocumentPanel.chooseMeaningContextMenuStrip.Items.Add(toolStripTextBox);
				this.vietPhraseOneMeaningDocumentPanel.chooseMeaningContextMenuStrip.Show(Cursor.Position);
				if (this.vietPhraseOneMeaningDocumentPanel.chooseMeaningContextMenuStrip.Items.Count < 2)
				{
					toolStripTextBox.Focus();
				}
			}
			this.vietPhraseOneMeaningDocumentPanel.HighlightText(vietPhraseOneMeaningCharRangeFromChineseIndex.StartIndex, vietPhraseOneMeaningCharRangeFromChineseIndex.Length, true, false);
			if (MainForm.ActiveConfiguration.AlwaysFocusInViet)
			{
				this.vietDocumentPanel.FocusInRichTextBox();
			}
		}

		private void chooseAMeaningMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)sender;
			string[] array = ((string)toolStripMenuItem.Tag).Replace("\r", "").Split(new char[]
			{
				'\n'
			});
			string text = array[3];
			int num = int.Parse(array[0].Trim());
			string text2 = array[1].Trim();
			int num2 = text.Length - text2.Length;
			this.vietPhraseOneMeaningRanges[num].Length += num2;
			for (int i = 0; i < this.vietPhraseOneMeaningRanges.Length; i++)
			{
				if (this.vietPhraseOneMeaningRanges[num].StartIndex < this.vietPhraseOneMeaningRanges[i].StartIndex)
				{
					this.vietPhraseOneMeaningRanges[i].StartIndex += num2;
				}
			}
			string text3 = text;
			if (char.IsUpper(text2[0]))
			{
				text3 = char.ToUpper(text[0]) + ((text.Length <= 1) ? "" : text.Substring(1));
			}
			this.vietPhraseOneMeaningDocumentPanel.ReplaceHighlightedText(text3);
			if ("Update VietPhrase".Equals(toolStripMenuItem.Text))
			{
				string[] array2 = TranslatorEngine.TranslatorEngine.GetVietPhraseValueFromKey(array[2].Trim()).Split("/|".ToCharArray());
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(text);
				string[] array3 = array2;
				for (int j = 0; j < array3.Length; j++)
				{
					string text4 = array3[j];
					if (!text4.Equals(text))
					{
						stringBuilder.Append("/");
						stringBuilder.Append(text4);
					}
				}
				TranslatorEngine.TranslatorEngine.UpdateVietPhraseDictionary(array[2], stringBuilder.ToString(), false);
			}
			this.vietPhraseOneMeaningDocumentPanel.chooseMeaningContextMenuStrip.Hide();
		}

		private void newMeaningTextBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar != '\r')
			{
				return;
			}
			ToolStripTextBox toolStripTextBox = (ToolStripTextBox)sender;
			string[] array = ((string)toolStripTextBox.Tag).Replace("\r", "").Split(new char[]
			{
				'\n'
			});
			string text = toolStripTextBox.Text;
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			int num = int.Parse(array[0].Trim());
			string text2 = array[1].Trim();
			int num2 = text.Length - text2.Length;
			this.vietPhraseOneMeaningRanges[num].Length += num2;
			for (int i = 0; i < this.vietPhraseOneMeaningRanges.Length; i++)
			{
				if (this.vietPhraseOneMeaningRanges[num].StartIndex < this.vietPhraseOneMeaningRanges[i].StartIndex)
				{
					this.vietPhraseOneMeaningRanges[i].StartIndex += num2;
				}
			}
			string text3 = text;
			if (char.IsUpper(text2[0]) && 0 < text.Length)
			{
				text3 = char.ToUpper(text[0]) + ((text.Length <= 1) ? "" : text.Substring(1));
			}
			this.vietPhraseOneMeaningDocumentPanel.ReplaceHighlightedText(text3);
			string vietPhraseValueFromKey = TranslatorEngine.TranslatorEngine.GetVietPhraseValueFromKey(array[2].Trim());
			string[] array2 = ((vietPhraseValueFromKey == null) ? string.Empty : vietPhraseValueFromKey).Split("/|".ToCharArray());
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(text);
			string[] array3 = array2;
			for (int j = 0; j < array3.Length; j++)
			{
				string text4 = array3[j];
				if (!text4.Equals(text))
				{
					stringBuilder.Append("/");
					stringBuilder.Append(text4);
				}
			}
			TranslatorEngine.TranslatorEngine.UpdateVietPhraseDictionary(array[2], stringBuilder.ToString().TrimEnd("/".ToCharArray()), false);
			this.vietPhraseOneMeaningDocumentPanel.chooseMeaningContextMenuStrip.Hide();
		}

		private CharRange getHanVietCharRangeFromChineseRange(int chineseStartIndex, int chineseLength)
		{
			int startIndex = this.chineseHanVietMappingArray[chineseStartIndex].StartIndex;
			int length = this.chineseHanVietMappingArray[chineseStartIndex + chineseLength - 1].GetEndIndex() - startIndex + 1;
			return new CharRange(startIndex, length);
		}

		private CharRange getChineseCharRangeFromVietPhraseIndex(int vietPhraseStartIndex)
		{
			int num = this.findVietPhraseMappingId(vietPhraseStartIndex);
			if (num < 0 || this.chinesePhraseRanges == null || this.chinesePhraseRanges.Length <= num)
			{
				return null;
			}
			return this.chinesePhraseRanges[num];
		}

		private CharRange getVietPhraseCharRangeFromChineseIndex(int chineseStartIndex)
		{
			if (this.vietPhraseDocumentPanel.IsHidden)
			{
				return null;
			}
			int num = this.findChineseMappingId(chineseStartIndex);
			if (num < 0 || this.vietPhraseRanges == null || this.vietPhraseRanges.Length <= num)
			{
				return null;
			}
			return this.vietPhraseRanges[num];
		}

		private CharRange getChineseCharRangeFromVietPhraseOneMeaningIndex(int vietPhraseStartIndex)
		{
			int num = this.findVietPhraseOneMeaningMappingId(vietPhraseStartIndex);
			if (num < 0 || this.chinesePhraseRanges == null || this.chinesePhraseRanges.Length <= num)
			{
				return null;
			}
			return this.chinesePhraseRanges[num];
		}

		private CharRange getVietPhraseOneMeaningCharRangeFromChineseIndex(int chineseStartIndex)
		{
			if (this.vietPhraseOneMeaningDocumentPanel.IsHidden)
			{
				return null;
			}
			int num = this.findChineseMappingId(chineseStartIndex);
			if (num < 0 || this.vietPhraseOneMeaningRanges == null || this.vietPhraseOneMeaningRanges.Length <= num)
			{
				return null;
			}
			return this.vietPhraseOneMeaningRanges[num];
		}

		private int findVietPhraseMappingId(int currentCharIndex)
		{
			int result = -1;
			for (int i = 0; i < this.vietPhraseRanges.Length; i++)
			{
				CharRange charRange = this.vietPhraseRanges[i];
				if (charRange.StartIndex <= currentCharIndex && currentCharIndex <= charRange.GetEndIndex())
				{
					result = i;
					break;
				}
			}
			return result;
		}

		private int findVietPhraseOneMeaningMappingId(int currentCharIndex)
		{
			int result = -1;
			for (int i = 0; i < this.vietPhraseOneMeaningRanges.Length; i++)
			{
				CharRange charRange = this.vietPhraseOneMeaningRanges[i];
				if (charRange.StartIndex <= currentCharIndex && currentCharIndex <= charRange.GetEndIndex())
				{
					result = i;
					break;
				}
			}
			return result;
		}

		private int findChineseMappingId(int currentCharIndex)
		{
			int result = -1;
			for (int i = 0; i < this.chinesePhraseRanges.Length; i++)
			{
				CharRange charRange = this.chinesePhraseRanges[i];
				if (charRange.StartIndex <= currentCharIndex && currentCharIndex <= charRange.GetEndIndex())
				{
					result = i;
					break;
				}
			}
			return result;
		}

		private void VietPhraseRightClick(int currentCharIndex)
		{
			if (this.vietPhraseDocumentPanel.GetTextContent().Length - 1 < currentCharIndex)
			{
				return;
			}
			int num = this.findVietPhraseMappingId(currentCharIndex);
			if (num < 0)
			{
				return;
			}
			string textContent = this.vietPhraseDocumentPanel.GetTextContent();
			textContent.Substring(this.vietPhraseRanges[num].StartIndex, this.vietPhraseRanges[num].Length);
			int num2 = currentCharIndex;
			int num3 = this.vietPhraseRanges[num].GetEndIndex();
			int num4 = currentCharIndex;
			while (this.vietPhraseRanges[num].StartIndex <= num4)
			{
				if (textContent[num4] == '[' || textContent[num4] == '/' || textContent[num4] == '|')
				{
					num2 = num4 + 1;
					break;
				}
				num2 = num4;
				num4--;
			}
			for (int i = currentCharIndex + 1; i <= this.vietPhraseRanges[num].GetEndIndex(); i++)
			{
				if (textContent[i] == ']' || textContent[i] == '/' || textContent[i] == '|')
				{
					num3 = i - 1;
					break;
				}
				num3 = i;
			}
			string text = textContent.Substring(num2, num3 - num2 + 1);
			if (!text.Contains("[") && !text.Contains("]") && !text.Contains("/") && !text.Contains("|"))
			{
				this.appendToVietToCurrentCursor(text);
			}
		}

		private void VietPhraseOneMeaningRightClick(int currentCharIndex)
		{
			if (this.vietPhraseOneMeaningDocumentPanel.GetTextContent().Length - 1 < currentCharIndex)
			{
				return;
			}
			int num = this.findVietPhraseOneMeaningMappingId(currentCharIndex);
			if (num < 0)
			{
				return;
			}
			string textContent = this.vietPhraseOneMeaningDocumentPanel.GetTextContent();
			string text = textContent.Substring(this.vietPhraseOneMeaningRanges[num].StartIndex, this.vietPhraseOneMeaningRanges[num].Length);
			this.appendToVietToCurrentCursor(text.Trim(new char[]
			{
				'[',
				']'
			}));
		}

		private void HanVietRightClick(int currentCharIndex)
		{
			if (this.hanVietDocumentPanel.GetTextContent().Length - 1 < currentCharIndex)
			{
				return;
			}
			int num = currentCharIndex;
			int num2 = currentCharIndex;
			string textContent = this.hanVietDocumentPanel.GetTextContent();
			int num3 = currentCharIndex;
			while (0 <= num3)
			{
				if (textContent[num3] == ' ' || textContent[num3] == '\n')
				{
					num = num3 + 1;
					break;
				}
				num3--;
			}
			for (int i = currentCharIndex + 1; i < textContent.Length; i++)
			{
				if (textContent[i] == ' ' || textContent[i] == ';' || textContent[i] == '?' || textContent[i] == '!' || textContent[i] == ',' || textContent[i] == '.')
				{
					num2 = i - 1;
					break;
				}
			}
			string text = textContent.Substring(num, num2 - num + 1);
			this.appendToVietToCurrentCursor(text);
		}

		private void appendToViet(string text)
		{
			if (text.Length == 0)
			{
				return;
			}
			string textContent = this.vietDocumentPanel.GetTextContent();
			if (textContent.Length <= 1)
			{
				this.vietDocumentPanel.AppendText(this.capitalize(text));
			}
			else if (textContent.EndsWith("! ") || textContent.EndsWith(". ") || (textContent.EndsWith("? ") | textContent.EndsWith(": ")) || textContent.EndsWith("; ") || textContent.EndsWith(") ") || textContent.EndsWith("\" ") || textContent.EndsWith("\"") || textContent.EndsWith("' ") || textContent.EndsWith("\n"))
			{
				this.vietDocumentPanel.AppendText(this.capitalize(text));
			}
			else if (textContent.EndsWith("!") || textContent.EndsWith(".") || textContent.EndsWith("?"))
			{
				this.vietDocumentPanel.AppendText(" " + this.capitalize(text));
			}
			else
			{
				if (!textContent.EndsWith(" "))
				{
					this.vietDocumentPanel.AppendText(" ");
				}
				this.vietDocumentPanel.AppendText(text);
			}
			if (MainForm.ActiveConfiguration.AlwaysFocusInViet)
			{
				this.vietDocumentPanel.FocusInRichTextBox();
			}
		}

		private void appendToVietToCurrentCursor(string text)
		{
			if (text.Length == 0)
			{
				return;
			}
			string text2 = "";
			string textContent = this.vietDocumentPanel.GetTextContent();
			string twoCharsBeforeSelectedText = this.vietDocumentPanel.GetTwoCharsBeforeSelectedText();
			string twoCharsAfterSelectedText = this.vietDocumentPanel.GetTwoCharsAfterSelectedText();
			if (textContent.Length <= 1)
			{
				text2 = this.capitalize(text);
			}
			else if (twoCharsBeforeSelectedText.EndsWith("! ") || twoCharsBeforeSelectedText.EndsWith(". ") || (twoCharsBeforeSelectedText.EndsWith("? ") | twoCharsBeforeSelectedText.EndsWith(": ")) || twoCharsBeforeSelectedText.EndsWith("; ") || twoCharsBeforeSelectedText.EndsWith(") ") || twoCharsBeforeSelectedText.EndsWith("\" ") || twoCharsBeforeSelectedText.EndsWith("' ") || twoCharsBeforeSelectedText.EndsWith("\n"))
			{
				text2 = this.capitalize(text);
			}
			else if (twoCharsBeforeSelectedText.EndsWith("!") || twoCharsBeforeSelectedText.EndsWith(".") || twoCharsBeforeSelectedText.EndsWith("?") || twoCharsBeforeSelectedText.EndsWith("!\"") || twoCharsBeforeSelectedText.EndsWith(".\"") || twoCharsBeforeSelectedText.EndsWith("?\""))
			{
				text2 = " " + this.capitalize(text);
			}
			else if (twoCharsBeforeSelectedText.EndsWith("\""))
			{
				text2 = this.capitalize(text);
			}
			else if (twoCharsBeforeSelectedText.EndsWith(" "))
			{
				text2 = text.TrimStart(new char[0]);
			}
			else if (text2.TrimStart(new char[0]).StartsWith(":") || text2.TrimStart(new char[0]).StartsWith("?") || text2.TrimStart(new char[0]).StartsWith("!") || text2.TrimStart(new char[0]).StartsWith(";") || text2.TrimStart(new char[0]).StartsWith(".") || text2.TrimStart(new char[0]).StartsWith(","))
			{
				text2 = text.TrimStart(new char[0]);
			}
			else
			{
				text2 = " " + text.TrimStart(new char[0]);
			}
			if (twoCharsAfterSelectedText.StartsWith(".") || twoCharsAfterSelectedText.StartsWith("?") || twoCharsAfterSelectedText.StartsWith(" ") || twoCharsAfterSelectedText.StartsWith("!") || twoCharsAfterSelectedText.StartsWith("\n") || twoCharsAfterSelectedText.StartsWith(";") || twoCharsAfterSelectedText.StartsWith(":") || twoCharsAfterSelectedText.StartsWith("\"") || twoCharsAfterSelectedText.StartsWith(",") || twoCharsAfterSelectedText.StartsWith("'"))
			{
				text2 = text2.TrimEnd(new char[0]);
			}
			else if (twoCharsAfterSelectedText.Equals(""))
			{
				text2 = " " + text2.Trim();
			}
			else
			{
				text2 = text2.TrimEnd(new char[0]) + " ";
			}
			if (text2.Trim().StartsWith(".") || text2.Trim().StartsWith(":") || text2.Trim().StartsWith(";") || text2.Trim().StartsWith(",") || text2.Trim().StartsWith("?") || text2.Trim().StartsWith("!") || twoCharsBeforeSelectedText.EndsWith("\n") || twoCharsBeforeSelectedText.EndsWith(" ") || twoCharsBeforeSelectedText.EndsWith(" \""))
			{
				text2 = text2.TrimStart(new char[0]);
			}
			this.vietDocumentPanel.AppendTextToCurrentCursor(text2);
			if (MainForm.ActiveConfiguration.AlwaysFocusInViet)
			{
				this.vietDocumentPanel.FocusInRichTextBox();
			}
		}

		private string capitalize(string text)
		{
			if (1 >= text.Length)
			{
				return char.ToUpper(text[0]).ToString();
			}
			return char.ToUpper(text[0]) + text.Substring(1);
		}

		private void MainFormKeyDown(object sender, KeyEventArgs e)
		{
			string text = null;
			switch (e.KeyCode)
			{
			case Keys.F1:
				text = MainForm.ActiveConfiguration.HotKeys_F1;
				break;
			case Keys.F2:
				text = MainForm.ActiveConfiguration.HotKeys_F2;
				break;
			case Keys.F3:
				text = MainForm.ActiveConfiguration.HotKeys_F3;
				break;
			case Keys.F4:
				text = MainForm.ActiveConfiguration.HotKeys_F4;
				break;
			case Keys.F5:
				text = MainForm.ActiveConfiguration.HotKeys_F5;
				break;
			case Keys.F6:
				text = MainForm.ActiveConfiguration.HotKeys_F6;
				break;
			case Keys.F7:
				text = MainForm.ActiveConfiguration.HotKeys_F7;
				break;
			case Keys.F8:
				text = MainForm.ActiveConfiguration.HotKeys_F8;
				break;
			case Keys.F9:
				text = MainForm.ActiveConfiguration.HotKeys_F9;
				break;
			}
			if (text != null)
			{
				this.appendToVietToCurrentCursor(text);
				if (MainForm.ActiveConfiguration.AlwaysFocusInViet)
				{
					this.vietDocumentPanel.FocusInRichTextBox();
				}
			}
		}

		private void copyMeaning(int meaningIndex)
		{
			string highlightText = this.vietPhraseDocumentPanel.GetHighlightText();
			if (string.IsNullOrEmpty(highlightText))
			{
				return;
			}
			string[] array = highlightText.Split(new char[]
			{
				'[',
				'/',
				']'
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length == 0)
			{
				return;
			}
			int num = meaningIndex;
			if (array.Length < meaningIndex)
			{
				num = array.Length;
			}
			int currentCharIndex = this.vietPhraseDocumentPanel.CurrentHighlightedTextStartIndex + highlightText.IndexOf(array[num - 1]);
			this.VietPhraseRightClick(currentCharIndex);
		}

		private void AutoScrollToolStripButtonClick(object sender, EventArgs e)
		{
			try
			{
				int physicalLine = this.countPhysicalLines(this.vietDocumentPanel.GetTextContent());
				this.chineseDocumentPanel.ScrollToASpecificPhysicalLine(physicalLine);
				this.hanVietDocumentPanel.ScrollToASpecificPhysicalLine(physicalLine);
				this.vietPhraseDocumentPanel.ScrollToASpecificPhysicalLine(physicalLine);
				this.vietPhraseOneMeaningDocumentPanel.ScrollToASpecificPhysicalLine(physicalLine);
			}
			catch (Exception)
			{
			}
		}

		private int countPhysicalLines(string text)
		{
			return text.Split(new char[]
			{
				'\n'
			}).Length;
		}

		private void MainFormFormClosing(object sender, FormClosingEventArgs e)
		{
			string str = this.isNewTranslationWork ? "Untitled" : Path.GetFileName(this.workingFilePath);
			DialogResult dialogResult = MessageBox.Show("Do you want to save the changes to " + str + "?", "Quick Translator", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
			if (dialogResult == DialogResult.Yes)
			{
				this.SaveToolStripMenuItemClick(null, null);
				return;
			}
			if (dialogResult == DialogResult.Cancel)
			{
				e.Cancel = true;
			}
		}

		private void ReloadDictToolStripButtonClick(object sender, EventArgs e)
		{
			TranslatorEngine.TranslatorEngine.DictionaryDirty = true;
			TranslatorEngine.TranslatorEngine.LoadDictionaries();
			Shortcuts.LoadFromFile(MainForm.shortcutDictionaryFilePath);
			this.Retranslate();
		}

		private void PostTTVToolStripButtonClick(object sender, EventArgs e)
		{
			using (PostTTVForm postTTVForm = new PostTTVForm(this.vietDocumentPanel.GetTextContent(), this.hanVietDocumentPanel.GetTextContent(), this.vietPhraseOneMeaningDocumentPanel.GetTextContent(), this.chineseDocumentPanel.GetTextContent(), this.editingStartDateTime))
			{
				postTTVForm.ShowDialog();
			}
		}

		private void MainFormFormClosed(object sender, FormClosedEventArgs e)
		{
			this.dockPanel.SaveAsXml(MainForm.dockPanelConfigFilePath);
		}

		private IDockContent GetContentFromPersistString(string persistString)
		{
			if (persistString == "Hán Việt")
			{
				return this.hanVietDocumentPanel;
			}
			if (persistString == "Trung")
			{
				return this.chineseDocumentPanel;
			}
			if (persistString == "VietPhrase")
			{
				return this.vietPhraseDocumentPanel;
			}
			if (persistString == "VietPhrase một nghĩa")
			{
				return this.vietPhraseOneMeaningDocumentPanel;
			}
			if (persistString == "Nghĩa")
			{
				return this.meaningDocumentPanel;
			}
			if (persistString == "Việt")
			{
				return this.vietDocumentPanel;
			}
			if (persistString == "Configuration")
			{
				return this.configurationPanel;
			}
			if (persistString == "Quick Web Browser")
			{
//				return this.extendedBrowserPanel;
			}
			return this.vietDocumentPanel;
		}

		private void UpdateVietPhraseToolStripButtonClick(object sender, EventArgs e)
		{
			this.updateVietPhraseOrName(0);
		}

		private void updateVietPhraseOrName(int type)
		{
			int selectionStart = this.chineseDocumentPanel.getSelectionStart();
			string textContent = this.chineseDocumentPanel.GetTextContent();
			int num = 10;
			StringBuilder stringBuilder = new StringBuilder();
			int num2 = 0;
			while (num2 < num && textContent.Length - 1 >= selectionStart + num2 && textContent[selectionStart + num2] != '\n')
			{
				stringBuilder.Append(textContent[selectionStart + num2]);
				num2++;
			}
			UpdateVietPhraseForm updateVietPhraseForm = new UpdateVietPhraseForm(stringBuilder.ToString(), type);
			updateVietPhraseForm.ShowDialog();
			if (updateVietPhraseForm.NeedSurfBaike)
			{
//				string chinese = HttpUtility.UrlEncode(updateVietPhraseForm.ChinesePhraseToSurfBaike, Encoding.GetEncoding("gb2312"));
//				this.extendedBrowserPanel.Baikeing(chinese);
//				this.extendedBrowserPanel.Activate();
			}
		}

		private void UpdateNameToolStripButtonClick(object sender, EventArgs e)
		{
			this.updateVietPhraseOrName(1);
		}

		private void NewWindowToolStripMenuItemClick(object sender, EventArgs e)
		{
			Thread thread = new Thread(new ThreadStart(this.newWindow));
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
		}

		private void newWindow()
		{
			Application.Run(new MainForm());
		}

		private void OpenToolStripMenuItemClick(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Quick Translator or Word (*.qt; *.doc)|*.qt;*.doc|All (*.*)|*.*";
			DialogResult dialogResult = openFileDialog.ShowDialog();
			if (dialogResult != DialogResult.OK)
			{
				return;
			}
			string content;
			using (TextReader textReader = new StreamReader(openFileDialog.FileName, true))
			{
				content = textReader.ReadToEnd();
			}
			string fileName = openFileDialog.FileName;
			if (fileName.EndsWith(".qt") || fileName.EndsWith(".doc"))
			{
				this.OpenQtOrWordFile(content, fileName);
			}
			else
			{
				this.OpenOtherFile(fileName);
			}
			this.workingFilePath = fileName;
			this.toggleNextBackButtons();
		}

		private void OpenQtOrWordFile(string content, string filePath)
		{
			int currentHanVietDisplayedLine = -1;
			int currentVietPhraseDisplayedLine = -1;
			int currentVietPhraseOneMeaningDisplayedLine = -1;
			string original;
			string rftContent;
			if (filePath.EndsWith(".qt"))
			{
				if (content.Contains("[CurrentLines]\n"))
				{
					string text = content.Substring("[CurrentLines]\n".Length, content.IndexOf("[Chinese]\n") - "[CurrentLines]\n".Length);
					try
					{
						currentHanVietDisplayedLine = int.Parse(text.Split(new char[]
						{
							'\n'
						})[0]);
						currentVietPhraseDisplayedLine = int.Parse(text.Split(new char[]
						{
							'\n'
						})[1]);
						currentVietPhraseOneMeaningDisplayedLine = int.Parse(text.Split(new char[]
						{
							'\n'
						})[2]);
					}
					catch
					{
					}
				}
				original = content.Substring(content.IndexOf("[Chinese]\n") + "[Chinese]\n".Length, content.IndexOf("[Viet]\n") - content.IndexOf("[Chinese]\n") - "[Chinese]\n".Length);
				rftContent = content.Substring(content.IndexOf("[Viet]\n") + "[Viet]\n".Length);
			}
			else
			{
				try
				{
//					ColumnExporter.ExtractFromWord(File.ReadAllText(filePath), out original, out rftContent);
				}
				catch (Exception exception)
				{
					MessageBox.Show("Định dạng của file không đúng!");
					string application = "QuickTranslator";
					ApplicationLog.Log(Path.GetDirectoryName(Application.ExecutablePath), application, exception);
					return;
				}
			}
//			this.chineseDocumentPanel.SetTextContent(TranslatorEngine.TranslatorEngine.StandardizeInput(original));
//			this.vietDocumentPanel.SetRftContent(rftContent);
			this.vietDocumentPanel.AppendText("");
			this.vietDocumentPanel.ScrollToBottom();
			this.vietDocumentPanel.FocusInRichTextBox();
			if (string.IsNullOrEmpty(this.vietDocumentPanel.GetTextContent().Trim()))
			{
				this.Translate(currentHanVietDisplayedLine, currentVietPhraseDisplayedLine, currentVietPhraseOneMeaningDisplayedLine);
			}
			else
			{
				this.Translate(-1, -1, -1);
			}
			this.Text = "Quick Translator - " + filePath;
			this.workingFilePath = filePath;
			this.isNewTranslationWork = false;
		}

		private void SaveToolStripMenuItemClick(object sender, EventArgs e)
		{
			if (this.isNewTranslationWork)
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.CheckFileExists = false;
				saveFileDialog.Filter = "Quick Translator (*.qt)|*.qt|Microsoft Word (*.doc)|*.doc";
				DialogResult dialogResult = saveFileDialog.ShowDialog();
				if (dialogResult != DialogResult.OK)
				{
					return;
				}
				this.workingFilePath = saveFileDialog.FileName;
			}
			if (this.workingFilePath.EndsWith(".qt"))
			{
				using (TextWriter textWriter = new StreamWriter(this.workingFilePath, false, Encoding.UTF8))
				{
					textWriter.Write("[CurrentLines]\n");
					textWriter.Write(this.hanVietDocumentPanel.getCurrentLineIndex() + "\n");
					textWriter.Write(this.vietPhraseDocumentPanel.getCurrentLineIndex() + "\n");
					textWriter.Write(this.vietPhraseOneMeaningDocumentPanel.getCurrentLineIndex() + "\n");
					textWriter.Write("[Chinese]\n");
					textWriter.Write(this.chineseDocumentPanel.GetTextContent());
					if (!this.chineseDocumentPanel.GetTextContent().EndsWith("\n"))
					{
						textWriter.Write("\n");
					}
					textWriter.Write("[Viet]\n");
					textWriter.Write(this.vietDocumentPanel.GetRtfContent());
					goto IL_18B;
				}
			}
			using (ExportToWordForm exportToWordForm = new ExportToWordForm(this.chineseDocumentPanel.GetTextContent(), this.hanVietDocumentPanel.GetTextContent(), this.vietPhraseDocumentPanel.GetTextContent(), this.vietPhraseOneMeaningDocumentPanel.GetTextContent(), this.vietDocumentPanel.GetTextContent()))
			{
				exportToWordForm.PopulateControls();
				exportToWordForm.ExportToWord(this.workingFilePath);
			}
			IL_18B:
			this.Text = "Quick Translator - " + this.workingFilePath;
			this.isNewTranslationWork = false;
			this.toggleNextBackButtons();
		}

		private void ExportToWordToolStripMenuItemClick(object sender, EventArgs e)
		{
			using (ExportToWordForm exportToWordForm = new ExportToWordForm(this.chineseDocumentPanel.GetTextContent(), this.hanVietDocumentPanel.GetTextContent(), this.vietPhraseDocumentPanel.GetTextContent(), this.vietPhraseOneMeaningDocumentPanel.GetTextContent(), this.vietDocumentPanel.GetTextContent()))
			{
				exportToWordForm.ShowDialog();
			}
		}

		private void Chinese_AddToVietPhraseHandler(int type)
		{
			UpdateVietPhraseForm updateVietPhraseForm = new UpdateVietPhraseForm(this.chineseDocumentPanel.GetSelectedText(), type);
			updateVietPhraseForm.ShowDialog();
			if (updateVietPhraseForm.NeedSurfBaike)
			{
//				string chinese = HttpUtility.UrlEncode(updateVietPhraseForm.ChinesePhraseToSurfBaike, Encoding.GetEncoding("gb2312"));
//				this.extendedBrowserPanel.Baikeing(chinese);
//				this.extendedBrowserPanel.Activate();
			}
		}

		private void HanViet_AddToVietPhraseHandler(int type)
		{
			int selectionStart = this.hanVietDocumentPanel.getSelectionStart();
			int num = this.findChineseCharIndexFromHanVietIndex(selectionStart);
			if (num <= 0)
			{
				num = this.findChineseCharIndexFromHanVietIndex(selectionStart + 1);
			}
			if (num <= 0)
			{
				return;
			}
			int num2 = this.hanVietDocumentPanel.getSelectionStart() + this.hanVietDocumentPanel.getSelectionLength() - 1;
			int num3 = this.findChineseCharIndexFromHanVietIndex(num2);
			if (num3 <= 0)
			{
				num3 = this.findChineseCharIndexFromHanVietIndex(num2 - 1);
			}
			if (num3 <= 0 || num3 < num)
			{
				return;
			}
			string chineseToLookup = this.chineseDocumentPanel.GetTextContent().Substring(num, num3 - num + 1);
			UpdateVietPhraseForm updateVietPhraseForm = new UpdateVietPhraseForm(chineseToLookup, type);
			updateVietPhraseForm.ShowDialog();
			if (updateVietPhraseForm.NeedSurfBaike)
			{
//				string chinese = HttpUtility.UrlEncode(updateVietPhraseForm.ChinesePhraseToSurfBaike, Encoding.GetEncoding("gb2312"));
//				this.extendedBrowserPanel.Baikeing(chinese);
//				this.extendedBrowserPanel.Activate();
			}
		}

		private int getChineseStartIndexFromVietPhraseIndex(int vietPhraseIndex)
		{
			CharRange chineseCharRangeFromVietPhraseIndex = this.getChineseCharRangeFromVietPhraseIndex(vietPhraseIndex);
			if (chineseCharRangeFromVietPhraseIndex == null)
			{
				return 0;
			}
			return chineseCharRangeFromVietPhraseIndex.StartIndex;
		}

		private int getChineseEndIndexFromVietPhraseIndex(int vietPhraseIndex)
		{
			CharRange chineseCharRangeFromVietPhraseIndex = this.getChineseCharRangeFromVietPhraseIndex(vietPhraseIndex);
			if (chineseCharRangeFromVietPhraseIndex == null)
			{
				return 0;
			}
			return chineseCharRangeFromVietPhraseIndex.GetEndIndex();
		}

		private void VietPhrase_AddToVietPhraseHandler(int type)
		{
			int selectionStart = this.vietPhraseDocumentPanel.getSelectionStart();
			int chineseStartIndexFromVietPhraseIndex = this.getChineseStartIndexFromVietPhraseIndex(selectionStart);
			if (chineseStartIndexFromVietPhraseIndex <= 0)
			{
				chineseStartIndexFromVietPhraseIndex = this.getChineseStartIndexFromVietPhraseIndex(selectionStart + 1);
			}
			if (chineseStartIndexFromVietPhraseIndex <= 0)
			{
				return;
			}
			int num = this.vietPhraseDocumentPanel.getSelectionStart() + this.vietPhraseDocumentPanel.getSelectionLength() - 1;
			int chineseEndIndexFromVietPhraseIndex = this.getChineseEndIndexFromVietPhraseIndex(num);
			if (chineseEndIndexFromVietPhraseIndex <= 0)
			{
				chineseEndIndexFromVietPhraseIndex = this.getChineseEndIndexFromVietPhraseIndex(num - 1);
			}
			if (chineseEndIndexFromVietPhraseIndex <= 0 || chineseEndIndexFromVietPhraseIndex < chineseStartIndexFromVietPhraseIndex)
			{
				return;
			}
			string chineseToLookup = this.chineseDocumentPanel.GetTextContent().Substring(chineseStartIndexFromVietPhraseIndex, chineseEndIndexFromVietPhraseIndex - chineseStartIndexFromVietPhraseIndex + 1);
			UpdateVietPhraseForm updateVietPhraseForm = new UpdateVietPhraseForm(chineseToLookup, type);
			updateVietPhraseForm.ShowDialog();
			if (updateVietPhraseForm.NeedSurfBaike)
			{
//				string chinese = HttpUtility.UrlEncode(updateVietPhraseForm.ChinesePhraseToSurfBaike, Encoding.GetEncoding("gb2312"));
//				this.extendedBrowserPanel.Baikeing(chinese);
//				this.extendedBrowserPanel.Activate();
			}
		}

		private int getChineseStartIndexFromVietPhraseOneMeaningIndex(int vietPhraseOneMeaningIndex)
		{
			CharRange chineseCharRangeFromVietPhraseOneMeaningIndex = this.getChineseCharRangeFromVietPhraseOneMeaningIndex(vietPhraseOneMeaningIndex);
			if (chineseCharRangeFromVietPhraseOneMeaningIndex == null)
			{
				return 0;
			}
			return chineseCharRangeFromVietPhraseOneMeaningIndex.StartIndex;
		}

		private int getChineseEndIndexFromVietPhraseOneMeaningIndex(int vietPhraseOneMeaningEndIndex)
		{
			CharRange chineseCharRangeFromVietPhraseOneMeaningIndex = this.getChineseCharRangeFromVietPhraseOneMeaningIndex(vietPhraseOneMeaningEndIndex);
			if (chineseCharRangeFromVietPhraseOneMeaningIndex == null)
			{
				return 0;
			}
			return chineseCharRangeFromVietPhraseOneMeaningIndex.GetEndIndex();
		}

		private void VietPhraseOneMeaning_AddToVietPhraseHandler(int type)
		{
			int selectionStart = this.vietPhraseOneMeaningDocumentPanel.getSelectionStart();
			int chineseStartIndexFromVietPhraseOneMeaningIndex = this.getChineseStartIndexFromVietPhraseOneMeaningIndex(selectionStart);
			if (chineseStartIndexFromVietPhraseOneMeaningIndex <= 0)
			{
				chineseStartIndexFromVietPhraseOneMeaningIndex = this.getChineseStartIndexFromVietPhraseOneMeaningIndex(selectionStart + 1);
			}
			if (chineseStartIndexFromVietPhraseOneMeaningIndex <= 0)
			{
				return;
			}
			int num = this.vietPhraseOneMeaningDocumentPanel.getSelectionStart() + this.vietPhraseOneMeaningDocumentPanel.getSelectionLength() - 1;
			int chineseEndIndexFromVietPhraseOneMeaningIndex = this.getChineseEndIndexFromVietPhraseOneMeaningIndex(num);
			if (chineseEndIndexFromVietPhraseOneMeaningIndex <= 0)
			{
				chineseEndIndexFromVietPhraseOneMeaningIndex = this.getChineseEndIndexFromVietPhraseOneMeaningIndex(num - 1);
			}
			if (chineseEndIndexFromVietPhraseOneMeaningIndex <= 0 || chineseEndIndexFromVietPhraseOneMeaningIndex < chineseStartIndexFromVietPhraseOneMeaningIndex)
			{
				return;
			}
			string chineseToLookup = this.chineseDocumentPanel.GetTextContent().Substring(chineseStartIndexFromVietPhraseOneMeaningIndex, chineseEndIndexFromVietPhraseOneMeaningIndex - chineseStartIndexFromVietPhraseOneMeaningIndex + 1);
			UpdateVietPhraseForm updateVietPhraseForm = new UpdateVietPhraseForm(chineseToLookup, type);
			updateVietPhraseForm.ShowDialog();
			if (updateVietPhraseForm.NeedSurfBaike)
			{
//				string chinese = HttpUtility.UrlEncode(updateVietPhraseForm.ChinesePhraseToSurfBaike, Encoding.GetEncoding("gb2312"));
//				this.extendedBrowserPanel.Baikeing(chinese);
//				this.extendedBrowserPanel.Activate();
			}
		}

		private void Meaning_AddToVietPhraseHandler(int type)
		{
			string selectedText = this.meaningDocumentPanel.GetSelectedText();
			UpdateVietPhraseForm updateVietPhraseForm = new UpdateVietPhraseForm(selectedText, type);
			updateVietPhraseForm.ShowDialog();
			if (updateVietPhraseForm.NeedSurfBaike)
			{
//				string chinese = HttpUtility.UrlEncode(updateVietPhraseForm.ChinesePhraseToSurfBaike, Encoding.GetEncoding("gb2312"));
//				this.extendedBrowserPanel.Baikeing(chinese);
//				this.extendedBrowserPanel.Activate();
			}
		}

		private void Chinese_SelectTextHandler()
		{
			int selectionStart = this.chineseDocumentPanel.getSelectionStart();
			int selectionLength = this.chineseDocumentPanel.getSelectionLength();
			this.SelectTextInHanViet(selectionStart, selectionLength);
			this.SelectTextInVietPhrase(selectionStart, selectionLength);
			this.SelectTextInVietPhraseOneMeaning(selectionStart, selectionLength);
		}

		private void HanViet_SelectTextHandler()
		{
			int selectionStart = this.hanVietDocumentPanel.getSelectionStart();
			int num = this.findChineseCharIndexFromHanVietIndex(selectionStart);
			if (num <= 0)
			{
				num = this.findChineseCharIndexFromHanVietIndex(selectionStart + 1);
			}
			if (num <= 0)
			{
				return;
			}
			int num2 = this.hanVietDocumentPanel.getSelectionStart() + this.hanVietDocumentPanel.getSelectionLength() - 1;
			int num3 = this.findChineseCharIndexFromHanVietIndex(num2);
			if (num3 <= 0)
			{
				num3 = this.findChineseCharIndexFromHanVietIndex(num2 - 1);
			}
			if (num3 <= 0 || num3 < num)
			{
				return;
			}
			this.SelectTextInChinese(num, num3 - num + 1);
			this.SelectTextInVietPhrase(num, num3 - num + 1);
			this.SelectTextInVietPhraseOneMeaning(num, num3 - num + 1);
		}

		private void VietPhrase_SelectTextHandler()
		{
			int selectionStart = this.vietPhraseDocumentPanel.getSelectionStart();
			CharRange chineseCharRangeFromVietPhraseIndex = this.getChineseCharRangeFromVietPhraseIndex(selectionStart);
			if (chineseCharRangeFromVietPhraseIndex == null)
			{
				chineseCharRangeFromVietPhraseIndex = this.getChineseCharRangeFromVietPhraseIndex(selectionStart + 1);
			}
			if (chineseCharRangeFromVietPhraseIndex == null)
			{
				return;
			}
			int startIndex = chineseCharRangeFromVietPhraseIndex.StartIndex;
			int num = this.vietPhraseDocumentPanel.getSelectionStart() + this.vietPhraseDocumentPanel.getSelectionLength() - 1;
			if (this.getChineseCharRangeFromVietPhraseIndex(num) == null)
			{
				return;
			}
			int endIndex = this.getChineseCharRangeFromVietPhraseIndex(num).GetEndIndex();
			if (endIndex <= 0)
			{
				endIndex = this.getChineseCharRangeFromVietPhraseIndex(num - 1).GetEndIndex();
				if (this.getChineseCharRangeFromVietPhraseIndex(num - 1) == null)
				{
					return;
				}
			}
			if (endIndex <= 0 || endIndex < startIndex)
			{
				return;
			}
			this.SelectTextInChinese(startIndex, endIndex - startIndex + 1);
			this.SelectTextInHanViet(startIndex, endIndex - startIndex + 1);
			this.SelectTextInVietPhraseOneMeaning(startIndex, endIndex - startIndex + 1);
		}

		private void VietPhraseOneMeaning_SelectTextHandler()
		{
			int selectionStart = this.vietPhraseOneMeaningDocumentPanel.getSelectionStart();
			CharRange chineseCharRangeFromVietPhraseOneMeaningIndex = this.getChineseCharRangeFromVietPhraseOneMeaningIndex(selectionStart);
			if (chineseCharRangeFromVietPhraseOneMeaningIndex == null)
			{
				chineseCharRangeFromVietPhraseOneMeaningIndex = this.getChineseCharRangeFromVietPhraseOneMeaningIndex(selectionStart + 1);
			}
			if (chineseCharRangeFromVietPhraseOneMeaningIndex == null)
			{
				return;
			}
			int startIndex = chineseCharRangeFromVietPhraseOneMeaningIndex.StartIndex;
			int num = this.vietPhraseOneMeaningDocumentPanel.getSelectionStart() + this.vietPhraseOneMeaningDocumentPanel.getSelectionLength() - 1;
			CharRange chineseCharRangeFromVietPhraseOneMeaningIndex2 = this.getChineseCharRangeFromVietPhraseOneMeaningIndex(num);
			if (chineseCharRangeFromVietPhraseOneMeaningIndex2 == null)
			{
				chineseCharRangeFromVietPhraseOneMeaningIndex2 = this.getChineseCharRangeFromVietPhraseOneMeaningIndex(num - 1);
			}
			if (chineseCharRangeFromVietPhraseOneMeaningIndex2 == null)
			{
				return;
			}
			int endIndex = chineseCharRangeFromVietPhraseOneMeaningIndex2.GetEndIndex();
			if (endIndex <= 0 || endIndex < startIndex)
			{
				return;
			}
			this.SelectTextInChinese(startIndex, endIndex - startIndex + 1);
			this.SelectTextInVietPhrase(startIndex, endIndex - startIndex + 1);
			this.SelectTextInHanViet(startIndex, endIndex - startIndex + 1);
		}

		private void SelectTextInHanViet(int chineseStartIndex, int chineseLength)
		{
			CharRange hanVietCharRangeFromChineseRange = this.getHanVietCharRangeFromChineseRange(chineseStartIndex, chineseLength);
			this.hanVietDocumentPanel.SelectText(hanVietCharRangeFromChineseRange.StartIndex, hanVietCharRangeFromChineseRange.Length);
		}

		private void SelectTextInChinese(int chineseStartIndex, int chineseLength)
		{
			this.chineseDocumentPanel.SelectText(chineseStartIndex, chineseLength);
		}

		private void SelectTextInVietPhrase(int chineseStartIndex, int chineseLength)
		{
			if (this.vietPhraseDocumentPanel.IsHidden)
			{
				return;
			}
			CharRange vietPhraseCharRangeFromChineseIndex = this.getVietPhraseCharRangeFromChineseIndex(chineseStartIndex);
			CharRange vietPhraseCharRangeFromChineseIndex2 = this.getVietPhraseCharRangeFromChineseIndex(chineseStartIndex + chineseLength - 1);
			this.vietPhraseDocumentPanel.SelectText(vietPhraseCharRangeFromChineseIndex.StartIndex, vietPhraseCharRangeFromChineseIndex2.GetEndIndex() - vietPhraseCharRangeFromChineseIndex.StartIndex + 1);
		}

		private void SelectTextInVietPhraseOneMeaning(int chineseStartIndex, int chineseLength)
		{
			if (this.vietPhraseOneMeaningDocumentPanel.IsHidden)
			{
				return;
			}
			CharRange vietPhraseOneMeaningCharRangeFromChineseIndex = this.getVietPhraseOneMeaningCharRangeFromChineseIndex(chineseStartIndex);
			CharRange vietPhraseOneMeaningCharRangeFromChineseIndex2 = this.getVietPhraseOneMeaningCharRangeFromChineseIndex(chineseStartIndex + chineseLength - 1);
			this.vietPhraseOneMeaningDocumentPanel.SelectText(vietPhraseOneMeaningCharRangeFromChineseIndex.StartIndex, vietPhraseOneMeaningCharRangeFromChineseIndex2.GetEndIndex() - vietPhraseOneMeaningCharRangeFromChineseIndex.StartIndex + 1);
		}

		private void BaikeingHandler()
		{
//			string chinese = HttpUtility.UrlEncode(this.chineseDocumentPanel.GetSelectedText().Trim(), Encoding.GetEncoding("gb2312"));
//			this.extendedBrowserPanel.Baikeing(chinese);
//			this.extendedBrowserPanel.Activate();
		}

		private void NcikuingHandler()
		{
//			string chinese = HttpUtility.UrlEncode(this.chineseDocumentPanel.GetSelectedText().Trim(), Encoding.GetEncoding("utf-8"));
//			this.extendedBrowserPanel.Ncikuing(chinese);
//			this.extendedBrowserPanel.Activate();
		}

		private void CopyToVietHandler(string textToCopy)
		{
			this.appendToVietToCurrentCursor(textToCopy.Trim(new char[]
			{
				'[',
				']'
			}));
		}

		private void AddToPhienAmHandler()
		{
			string selectedText = this.chineseDocumentPanel.GetSelectedText();
			if (string.IsNullOrEmpty(selectedText))
			{
				return;
			}
			new UpdatePhienAmForm(selectedText).ShowDialog();
		}

		private void DeleteSelectedTextHandler(bool remembered)
		{
			if (remembered)
			{
				string selectedText = this.chineseDocumentPanel.GetSelectedText();
				TranslatorEngine.TranslatorEngine.AddIgnoredChinesePhrase(selectedText);
			}
			this.chineseDocumentPanel.DeleteSelectedText();
			this.Retranslate();
		}

		private void ShiftAndMouseMoveHandler(int charIndexUnderMouse)
		{
			CharRange[] array = this.vietPhraseOneMeaningRanges;
			for (int i = 0; i < array.Length; i++)
			{
				CharRange charRange = array[i];
				if (charRange.IsInRange(charIndexUnderMouse))
				{
					this.vietPhraseOneMeaningDocumentPanel.contentRichTextBox.SelectionStart = charRange.StartIndex;
					this.vietPhraseOneMeaningDocumentPanel.contentRichTextBox.SelectionLength = 0;
					return;
				}
			}
		}

		private void ShiftUpHandler()
		{
			try
			{
				this.ShiftUpHandlerWithoutExceptionHandling();
			}
			catch (Exception exception)
			{
				string application = "QuickTranslator";
				ApplicationLog.Log(Path.GetDirectoryName(Application.ExecutablePath), application, exception);
			}
		}

		private void ShiftUpHandlerWithoutExceptionHandling()
		{
			int selectionStart = this.vietPhraseOneMeaningDocumentPanel.contentRichTextBox.SelectionStart;
			int currentHighlightedTextStartIndex = this.vietPhraseOneMeaningDocumentPanel.CurrentHighlightedTextStartIndex;
			int currentHighlightedTextLength = this.vietPhraseOneMeaningDocumentPanel.CurrentHighlightedTextLength;
			if (currentHighlightedTextStartIndex == selectionStart)
			{
				return;
			}
			string text = this.vietPhraseOneMeaningDocumentPanel.contentRichTextBox.Text;
			int num = Math.Min(currentHighlightedTextStartIndex, selectionStart);
			int num2 = (currentHighlightedTextStartIndex < selectionStart) ? selectionStart : (currentHighlightedTextStartIndex + currentHighlightedTextLength);
			num2--;
			string text2 = text.Substring(num, num2 - num + 1);
			string text3 = text.Substring(currentHighlightedTextStartIndex, currentHighlightedTextLength);
			string text4 = text3;
			if (currentHighlightedTextStartIndex >= selectionStart || currentHighlightedTextStartIndex + currentHighlightedTextLength + 1 == selectionStart || currentHighlightedTextStartIndex + currentHighlightedTextLength == selectionStart)
			{
				if (selectionStart < currentHighlightedTextStartIndex)
				{
					int num3 = currentHighlightedTextLength + 1;
					text4 += " ";
					text4 += text2;
					if (text4.EndsWith(" ") || text2.Length < text4.Length)
					{
						text4 = text4.Substring(0, text2.Length);
					}
					if (char.IsUpper(text2[0]) && ("\n\t\"".Contains(text[num - 1].ToString()) || (' ' == text[num - 1] && ":.\"?!".Contains(text[num - 2].ToString()))))
					{
						text4 = char.ToUpper(text4[0]) + text4.Substring(1);
						CharRange chineseCharRangeFromVietPhraseOneMeaningIndex = this.getChineseCharRangeFromVietPhraseOneMeaningIndex(selectionStart);
						if (chineseCharRangeFromVietPhraseOneMeaningIndex != null && TranslatorEngine.TranslatorEngine.GetNameValueFromKey(this.chineseContent.Substring(chineseCharRangeFromVietPhraseOneMeaningIndex.StartIndex, chineseCharRangeFromVietPhraseOneMeaningIndex.Length)) == null)
						{
							text4 = text4.Substring(0, num3) + char.ToLower(text4[num3]) + text4.Substring(num3 + 1);
						}
					}
					this.vietPhraseOneMeaningDocumentPanel.SelectText(num, text2.Length);
					this.vietPhraseOneMeaningDocumentPanel.contentRichTextBox.SelectionColor = this.vietPhraseOneMeaningDocumentPanel.contentRichTextBox.ForeColor;
					this.vietPhraseOneMeaningDocumentPanel.contentRichTextBox.SelectedText = text4;
					this.vietPhraseOneMeaningDocumentPanel.HighlightText(num, 0);
					CharRange[] array = this.vietPhraseOneMeaningRanges;
					for (int i = 0; i < array.Length; i++)
					{
						CharRange charRange = array[i];
						if (currentHighlightedTextStartIndex == charRange.StartIndex)
						{
							charRange.StartIndex = num;
						}
						else if (num <= charRange.StartIndex && charRange.StartIndex < num2)
						{
							charRange.StartIndex += num3;
						}
					}
					this.vietPhraseOneMeaningChanged = true;
				}
				return;
			}
			int num4 = currentHighlightedTextLength + 1;
			if (!"\n\t.,:;!?".Contains(text[num2 + 1].ToString()))
			{
				text4 += " ";
			}
			else
			{
				text4 = " " + text4;
			}
			text4 = text2.Substring(num4) + text4;
			if (text2.Equals(text4) || text4.Length != text2.Length)
			{
				return;
			}
			if (char.IsUpper(text[currentHighlightedTextStartIndex]) && ("\n\t\"".Contains(text[currentHighlightedTextStartIndex - 1].ToString()) || (' ' == text[currentHighlightedTextStartIndex - 1] && ":.\"?!".Contains(text[currentHighlightedTextStartIndex - 2].ToString()))))
			{
				text4 = char.ToUpper(text4[0]) + text4.Substring(1);
				int num5 = text4.Length - num4 + (text4.EndsWith(" ") ? 0 : 1);
				if (TranslatorEngine.TranslatorEngine.GetNameValueFromKey(this.chineseDocumentPanel.GetHighlightText()) == null)
				{
					text4 = text4.Substring(0, num5) + char.ToLower(text4[num5]) + text4.Substring(num5 + 1);
				}
			}
			this.vietPhraseOneMeaningDocumentPanel.SelectText(num, text2.Length);
			this.vietPhraseOneMeaningDocumentPanel.contentRichTextBox.SelectionColor = this.vietPhraseOneMeaningDocumentPanel.contentRichTextBox.ForeColor;
			this.vietPhraseOneMeaningDocumentPanel.contentRichTextBox.SelectedText = text4;
			this.vietPhraseOneMeaningDocumentPanel.HighlightText(num, 0);
			CharRange[] array2 = this.vietPhraseOneMeaningRanges;
			for (int j = 0; j < array2.Length; j++)
			{
				CharRange charRange2 = array2[j];
				if (num < charRange2.StartIndex && charRange2.StartIndex < num2)
				{
					charRange2.StartIndex -= num4;
				}
				else if (num == charRange2.StartIndex)
				{
					charRange2.StartIndex = num2 - charRange2.Length + (text4.EndsWith(" ") ? 0 : 1);
				}
			}
			this.vietPhraseOneMeaningChanged = true;
		}

		private void RetranslateToolStripButtonClick(object sender, EventArgs e)
		{
			int currentLineIndex = this.chineseDocumentPanel.getCurrentLineIndex();
			this.updateDocumentPanel(this.chineseDocumentPanel, TranslatorEngine.TranslatorEngine.StandardizeInput(this.chineseDocumentPanel.GetTextContent()), currentLineIndex);
			this.Retranslate();
		}

		public void Retranslate()
		{
			if (this.vietPhraseOneMeaningChanged)
			{
				DialogResult dialogResult = MessageBox.Show("Ô VietPhrase một nghĩa đã được thay đổi.\nNội dung thay đổi sẽ bị mất nếu tiếp tục.", "Re-Translate?", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
				if (dialogResult == DialogResult.Cancel)
				{
					return;
				}
			}
			int currentLineIndex = this.hanVietDocumentPanel.getCurrentLineIndex();
			int currentLineIndex2 = this.vietPhraseDocumentPanel.getCurrentLineIndex();
			int currentLineIndex3 = this.vietPhraseOneMeaningDocumentPanel.getCurrentLineIndex();
			this.Translate(currentLineIndex, currentLineIndex2, currentLineIndex3);
		}

		private void MainForm_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] array = (string[])e.Data.GetData(DataFormats.FileDrop);
				string fileName = array[0];
				this.OpenFile(fileName);
			}
		}

		private void OpenFile(string fileName)
		{
			if (fileName.EndsWith(".qt"))
			{
				string text;
				using (TextReader textReader = new StreamReader(fileName, true))
				{
					text = textReader.ReadToEnd();
				}
				string original = text.Substring(text.IndexOf("[Chinese]\n") + "[Chinese]\n".Length, text.IndexOf("[Viet]\n") - text.IndexOf("[Chinese]\n") - "[Chinese]\n".Length);
				this.chineseDocumentPanel.SetTextContent(TranslatorEngine.TranslatorEngine.StandardizeInput(original));
				string rftContent = text.Substring(text.IndexOf("[Viet]\n") + "[Viet]\n".Length);
				this.vietDocumentPanel.SetRftContent(rftContent);
				this.vietDocumentPanel.AppendText("");
				this.vietDocumentPanel.ScrollToBottom();
				this.vietDocumentPanel.FocusInRichTextBox();
				this.Text = "Quick Translator - " + fileName;
				this.workingFilePath = fileName;
				this.isNewTranslationWork = false;
				int currentHanVietDisplayedLine = -1;
				int currentVietPhraseDisplayedLine = -1;
				int currentVietPhraseOneMeaningDisplayedLine = -1;
				if (text.Contains("[CurrentLines]\n"))
				{
					string text2 = text.Substring("[CurrentLines]\n".Length, text.IndexOf("[Chinese]\n") - "[CurrentLines]\n".Length);
					try
					{
						currentHanVietDisplayedLine = int.Parse(text2.Split(new char[]
						{
							'\n'
						})[0]);
						currentVietPhraseDisplayedLine = int.Parse(text2.Split(new char[]
						{
							'\n'
						})[1]);
						currentVietPhraseOneMeaningDisplayedLine = int.Parse(text2.Split(new char[]
						{
							'\n'
						})[2]);
					}
					catch
					{
					}
				}
				if (string.IsNullOrEmpty(this.vietDocumentPanel.GetTextContent().Trim()))
				{
					this.Translate(currentHanVietDisplayedLine, currentVietPhraseDisplayedLine, currentVietPhraseOneMeaningDisplayedLine);
				}
				else
				{
					this.Translate(-1, -1, -1);
				}
			}
			else if (fileName.EndsWith(".doc"))
			{
				this.openWord(fileName);
				this.Translate(-1, -1, -1);
			}
			else
			{
				this.OpenOtherFile(fileName);
			}
			this.AutoScrollToolStripButtonClick(null, null);
			this.workingFilePath = fileName;
			this.toggleNextBackButtons();
		}

		private void OpenOtherFile(string fileName)
		{
			string name = CharsetDetector.DetectChineseCharset(fileName);
			string text = File.ReadAllText(fileName, Encoding.GetEncoding(name));
			this.isNewTranslationWork = true;
			if (fileName.EndsWith("html") || fileName.EndsWith("htm") || fileName.EndsWith("asp") || fileName.EndsWith("aspx") || fileName.EndsWith("php"))
			{
				text = HtmlParser.GetChineseContent(text, false);
			}
			text = TranslatorEngine.TranslatorEngine.StandardizeInput(text);
			this.chineseDocumentPanel.SetTextContent(text);
			this.Text = "Quick Translator - " + fileName;
			this.Translate(-2, -2, -2);
		}

		private void openWord(string filePath)
		{
			string original;
			string textContent;
			try
			{
//				ColumnExporter.ExtractFromWord(File.ReadAllText(filePath), out original, out textContent);
			}
			catch (Exception exception)
			{
				MessageBox.Show("Định dạng của file không đúng!");
				string application = "QuickTranslator";
				ApplicationLog.Log(Path.GetDirectoryName(Application.ExecutablePath), application, exception);
				return;
			}
//			this.chineseDocumentPanel.SetTextContent(TranslatorEngine.StandardizeInput(original));
//			this.vietDocumentPanel.SetTextContent(textContent);
			this.vietDocumentPanel.ScrollToBottom();
			this.vietDocumentPanel.FocusInRichTextBox();
			this.Text = "Quick Translator - " + filePath;
			this.workingFilePath = filePath;
			this.isNewTranslationWork = false;
		}

		private void MainForm_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.Link;
			}
		}

		private void VietPhraseToolStripMenuItemCheckStateChanged(object sender, EventArgs e)
		{
			MainForm.ActiveConfiguration.Layout_VietPhrase = this.vietPhraseToolStripMenuItem.Checked;
			MainForm.ActiveConfiguration.SaveToFile(MainForm.applicationConfigFilePath);
			this.configurationPanel.ReloadConfiguration();
			this.ToggleDocumentPanel(this.vietPhraseDocumentPanel, MainForm.ActiveConfiguration.Layout_VietPhrase);
			int currentLineIndex = this.vietPhraseDocumentPanel.getCurrentLineIndex();
			this.translateVietPhrase(currentLineIndex);
		}

		private void VietPhraseOneMeaningToolStripMenuItemCheckStateChanged(object sender, EventArgs e)
		{
			MainForm.ActiveConfiguration.Layout_VietPhraseOneMeaning = this.vietPhraseOneMeaningToolStripMenuItem.Checked;
			MainForm.ActiveConfiguration.SaveToFile(MainForm.applicationConfigFilePath);
			this.configurationPanel.ReloadConfiguration();
			this.ToggleDocumentPanel(this.vietPhraseOneMeaningDocumentPanel, MainForm.ActiveConfiguration.Layout_VietPhraseOneMeaning);
			int currentLineIndex = this.vietPhraseOneMeaningDocumentPanel.getCurrentLineIndex();
			this.translateVietPhraseOneMeaning(currentLineIndex);
		}

		private void ToggleDocumentPanel(DocumentPanel panel, bool shown)
		{
			if (shown)
			{
				panel.Show();
				return;
			}
			panel.Hide();
		}

		private void VietPhraseToolStripMenuItemClick(object sender, EventArgs e)
		{
			this.vietPhraseToolStripMenuItem.Checked = !this.vietPhraseToolStripMenuItem.Checked;
		}

		private void VietPhraseOneMeaningToolStripMenuItemClick(object sender, EventArgs e)
		{
			this.vietPhraseOneMeaningToolStripMenuItem.Checked = !this.vietPhraseOneMeaningToolStripMenuItem.Checked;
		}

		private void ImportFromWordToolStripMenuItemClick(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Microsoft Word 2003 (*.doc)|*.doc";
			DialogResult dialogResult = openFileDialog.ShowDialog();
			if (dialogResult != DialogResult.OK)
			{
				return;
			}
			this.openWord(openFileDialog.FileName);
			this.Translate(-1, -1, -1);
		}

		private void toggleNextBackButtons()
		{
			this.editingStartDateTime = DateTime.Now;
			if (string.IsNullOrEmpty(this.workingFilePath))
			{
				this.backToolStripButton.Enabled = false;
				this.nextToolStripButton.Enabled = false;
				return;
			}
			FileInfo fileInfo = new FileInfo(this.workingFilePath);
			string[] files = Directory.GetFiles(fileInfo.Directory.FullName);
			List<string> list = new List<string>(files);
			list.Sort();
			int num = list.FindIndex((string filePath) => filePath == this.workingFilePath);
			if (num < 0)
			{
				this.backToolStripButton.Enabled = false;
				this.nextToolStripButton.Enabled = false;
				return;
			}
			this.backToolStripButton.Enabled = (0 < num);
			this.backFilePath = ((0 < num) ? list[num - 1] : "");
			this.nextToolStripButton.Enabled = (num < files.Length - 1);
			this.nextFilePath = ((num < files.Length - 1) ? list[num + 1] : "");
		}

		private void BackToolStripButtonClick(object sender, EventArgs e)
		{
			if (!File.Exists(this.backFilePath))
			{
				this.toggleNextBackButtons();
			}
			if (File.Exists(this.backFilePath))
			{
				this.OpenFile(this.backFilePath);
			}
		}

		private void NextToolStripButtonClick(object sender, EventArgs e)
		{
			if (!File.Exists(this.nextFilePath))
			{
				this.toggleNextBackButtons();
			}
			if (File.Exists(this.nextFilePath))
			{
				this.OpenFile(this.nextFilePath);
			}
		}

		private void calculateChineseWordCount()
		{
			int num = 0;
			string textContent = this.chineseDocumentPanel.GetTextContent();
			string text = textContent;
			for (int i = 0; i < text.Length; i++)
			{
				char character = text[i];
				if (TranslatorEngine.TranslatorEngine.IsChinese(character))
				{
					num++;
				}
			}
			this.wordCountToolStripStatusLabel.Text = num.ToString("N0") + " từ";
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
			DockPanelSkin dockPanelSkin = new DockPanelSkin();
			AutoHideStripSkin autoHideStripSkin = new AutoHideStripSkin();
			DockPanelGradient dockPanelGradient = new DockPanelGradient();
			TabGradient tabGradient = new TabGradient();
			DockPaneStripSkin dockPaneStripSkin = new DockPaneStripSkin();
			DockPaneStripGradient dockPaneStripGradient = new DockPaneStripGradient();
			TabGradient tabGradient2 = new TabGradient();
			DockPanelGradient dockPanelGradient2 = new DockPanelGradient();
			TabGradient tabGradient3 = new TabGradient();
			DockPaneStripToolWindowGradient dockPaneStripToolWindowGradient = new DockPaneStripToolWindowGradient();
			TabGradient tabGradient4 = new TabGradient();
			TabGradient tabGradient5 = new TabGradient();
			DockPanelGradient dockPanelGradient3 = new DockPanelGradient();
			TabGradient tabGradient6 = new TabGradient();
			TabGradient tabGradient7 = new TabGradient();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(MainForm));
			this.dockPanel = new DockPanel();
			this.toolStrip1 = new ToolStrip();
			this.fileToolStripButton = new ToolStripDropDownButton();
			this.newWindowToolStripMenuItem = new ToolStripMenuItem();
			this.toolStripSeparator4 = new ToolStripSeparator();
			this.openToolStripMenuItem = new ToolStripMenuItem();
			this.saveToolStripMenuItem = new ToolStripMenuItem();
			this.toolStripSeparator7 = new ToolStripSeparator();
			this.exportToWordToolStripMenuItem = new ToolStripMenuItem();
			this.toolStripSeparator3 = new ToolStripSeparator();
			this.layoutToolStripDropDownButton = new ToolStripDropDownButton();
			this.chineseToolStripMenuItem = new ToolStripMenuItem();
			this.hanVietToolStripMenuItem = new ToolStripMenuItem();
			this.vietToolStripMenuItem = new ToolStripMenuItem();
			this.vietPhraseToolStripMenuItem = new ToolStripMenuItem();
			this.vietPhraseOneMeaningToolStripMenuItem = new ToolStripMenuItem();
			this.toolStripSeparator2 = new ToolStripSeparator();
			this.translateFromClipboardToolStripButton = new ToolStripButton();
			this.toolStripSeparator9 = new ToolStripSeparator();
			this.retranslateToolStripButton = new ToolStripButton();
			this.toolStripSeparator6 = new ToolStripSeparator();
			this.updateVietPhraseToolStripButton = new ToolStripButton();
			this.toolStripSeparator8 = new ToolStripSeparator();
			this.updateNameToolStripButton = new ToolStripButton();
			this.toolStripSeparator1 = new ToolStripSeparator();
			this.autoScrollToolStripButton = new ToolStripButton();
			this.toolStripSeparator5 = new ToolStripSeparator();
			this.reloadDictToolStripButton = new ToolStripButton();
			this.toolStripButton10 = new ToolStripSeparator();
			this.toolStripButton1 = new ToolStripLabel();
			this.toolStripButton2 = new ToolStripLabel();
			this.toolStripButton3 = new ToolStripLabel();
			this.toolStripButton4 = new ToolStripLabel();
			this.toolStripButton7 = new ToolStripLabel();
			this.toolStripButton8 = new ToolStripLabel();
			this.toolStripButton5 = new ToolStripLabel();
			this.toolStripButton6 = new ToolStripLabel();
			this.toolStripLabel1 = new ToolStripLabel();
			this.postTTVToolStripButton = new ToolStripButton();
			this.toolStripSeparator10 = new ToolStripSeparator();
			this.nextToolStripButton = new ToolStripButton();
			this.backToolStripButton = new ToolStripButton();
			this.statusStrip1 = new StatusStrip();
			this.toolStripStatusLabel1 = new ToolStripStatusLabel();
			this.toolStripStatusLabel2 = new ToolStripStatusLabel();
			this.toolStripStatusLabel3 = new ToolStripStatusLabel();
			this.wordCountToolStripStatusLabel = new ToolStripStatusLabel();
			this.toolStrip1.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			base.SuspendLayout();
			this.dockPanel.ActiveAutoHideContent = null;
			this.dockPanel.Dock = DockStyle.Fill;
			this.dockPanel.DockBackColor = SystemColors.Control;
			this.dockPanel.Location = new Point(0, 25);
			this.dockPanel.Name = "dockPanel";
			this.dockPanel.Size = new Size(877, 367);
			dockPanelGradient.EndColor = SystemColors.ControlLight;
			dockPanelGradient.StartColor = SystemColors.ControlLight;
			autoHideStripSkin.DockStripGradient = dockPanelGradient;
			tabGradient.EndColor = SystemColors.Control;
			tabGradient.StartColor = SystemColors.Control;
			tabGradient.TextColor = SystemColors.ControlDarkDark;
			autoHideStripSkin.TabGradient = tabGradient;
			dockPanelSkin.AutoHideStripSkin = autoHideStripSkin;
			tabGradient2.EndColor = SystemColors.ControlLightLight;
			tabGradient2.StartColor = SystemColors.ControlLightLight;
			tabGradient2.TextColor = SystemColors.ControlText;
			dockPaneStripGradient.ActiveTabGradient = tabGradient2;
			dockPanelGradient2.EndColor = SystemColors.Control;
			dockPanelGradient2.StartColor = SystemColors.Control;
			dockPaneStripGradient.DockStripGradient = dockPanelGradient2;
			tabGradient3.EndColor = SystemColors.ControlLight;
			tabGradient3.StartColor = SystemColors.ControlLight;
			tabGradient3.TextColor = SystemColors.ControlText;
			dockPaneStripGradient.InactiveTabGradient = tabGradient3;
			dockPaneStripSkin.DocumentGradient = dockPaneStripGradient;
			tabGradient4.EndColor = SystemColors.ActiveCaption;
			tabGradient4.LinearGradientMode = LinearGradientMode.Vertical;
			tabGradient4.StartColor = SystemColors.GradientActiveCaption;
			tabGradient4.TextColor = SystemColors.ActiveCaptionText;
			dockPaneStripToolWindowGradient.ActiveCaptionGradient = tabGradient4;
			tabGradient5.EndColor = SystemColors.Control;
			tabGradient5.StartColor = SystemColors.Control;
			tabGradient5.TextColor = SystemColors.ControlText;
			dockPaneStripToolWindowGradient.ActiveTabGradient = tabGradient5;
			dockPanelGradient3.EndColor = SystemColors.ControlLight;
			dockPanelGradient3.StartColor = SystemColors.ControlLight;
			dockPaneStripToolWindowGradient.DockStripGradient = dockPanelGradient3;
			tabGradient6.EndColor = SystemColors.GradientInactiveCaption;
			tabGradient6.LinearGradientMode = LinearGradientMode.Vertical;
			tabGradient6.StartColor = SystemColors.GradientInactiveCaption;
			tabGradient6.TextColor = SystemColors.ControlText;
			dockPaneStripToolWindowGradient.InactiveCaptionGradient = tabGradient6;
			tabGradient7.EndColor = Color.Transparent;
			tabGradient7.StartColor = Color.Transparent;
			tabGradient7.TextColor = SystemColors.ControlDarkDark;
			dockPaneStripToolWindowGradient.InactiveTabGradient = tabGradient7;
			dockPaneStripSkin.ToolWindowGradient = dockPaneStripToolWindowGradient;
			dockPanelSkin.DockPaneStripSkin = dockPaneStripSkin;
			this.dockPanel.Skin = dockPanelSkin;
			this.dockPanel.TabIndex = 0;
			this.toolStrip1.Items.AddRange(new ToolStripItem[]
			{
				this.fileToolStripButton,
				this.toolStripSeparator3,
				this.layoutToolStripDropDownButton,
				this.toolStripSeparator2,
				this.translateFromClipboardToolStripButton,
				this.toolStripSeparator9,
				this.retranslateToolStripButton,
				this.toolStripSeparator6,
				this.updateVietPhraseToolStripButton,
				this.toolStripSeparator8,
				this.updateNameToolStripButton,
				this.toolStripSeparator1,
				this.autoScrollToolStripButton,
				this.toolStripSeparator5,
				this.reloadDictToolStripButton,
				this.toolStripButton10,
				this.toolStripButton1,
				this.toolStripButton2,
				this.toolStripButton3,
				this.toolStripButton4,
				this.toolStripButton7,
				this.toolStripButton8,
				this.toolStripButton5,
				this.toolStripButton6,
				this.toolStripLabel1,
				this.postTTVToolStripButton,
				this.toolStripSeparator10,
				this.nextToolStripButton,
				this.backToolStripButton
			});
			this.toolStrip1.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
			this.toolStrip1.Location = new Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new Size(877, 25);
			this.toolStrip1.TabIndex = 2;
			this.toolStrip1.Text = "toolStrip1";
			this.fileToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
			this.fileToolStripButton.DropDownItems.AddRange(new ToolStripItem[]
			{
				this.newWindowToolStripMenuItem,
				this.toolStripSeparator4,
				this.openToolStripMenuItem,
				this.saveToolStripMenuItem,
				this.toolStripSeparator7,
				this.exportToWordToolStripMenuItem
			});
//			this.fileToolStripButton.Image = (Image)componentResourceManager.GetObject("fileToolStripButton.Image");
			this.fileToolStripButton.ImageTransparentColor = Color.Magenta;
			this.fileToolStripButton.Name = "fileToolStripButton";
			this.fileToolStripButton.Size = new Size(38, 22);
			this.fileToolStripButton.Text = "&File";
//			this.newWindowToolStripMenuItem.Image = (Image)componentResourceManager.GetObject("newWindowToolStripMenuItem.Image");
			this.newWindowToolStripMenuItem.Name = "newWindowToolStripMenuItem";
			this.newWindowToolStripMenuItem.ShortcutKeys = (Keys)131159;
			this.newWindowToolStripMenuItem.Size = new Size(196, 22);
			this.newWindowToolStripMenuItem.Text = "New &Window";
			this.newWindowToolStripMenuItem.Click += new EventHandler(this.NewWindowToolStripMenuItemClick);
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new Size(193, 6);
//			this.openToolStripMenuItem.Image = (Image)componentResourceManager.GetObject("openToolStripMenuItem.Image");
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.ShortcutKeys = (Keys)131153;
			this.openToolStripMenuItem.Size = new Size(196, 22);
			this.openToolStripMenuItem.Text = "Open...";
			this.openToolStripMenuItem.Click += new EventHandler(this.OpenToolStripMenuItemClick);
//			this.saveToolStripMenuItem.Image = (Image)componentResourceManager.GetObject("saveToolStripMenuItem.Image");
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.ShortcutKeys = (Keys)131155;
			this.saveToolStripMenuItem.Size = new Size(196, 22);
			this.saveToolStripMenuItem.Text = "&Save...";
			this.saveToolStripMenuItem.Click += new EventHandler(this.SaveToolStripMenuItemClick);
			this.toolStripSeparator7.Name = "toolStripSeparator7";
			this.toolStripSeparator7.Size = new Size(193, 6);
//			this.exportToWordToolStripMenuItem.Image = (Image)componentResourceManager.GetObject("exportToWordToolStripMenuItem.Image");
			this.exportToWordToolStripMenuItem.Name = "exportToWordToolStripMenuItem";
			this.exportToWordToolStripMenuItem.ShortcutKeys = (Keys)131141;
			this.exportToWordToolStripMenuItem.Size = new Size(196, 22);
			this.exportToWordToolStripMenuItem.Text = "&Export To Word";
			this.exportToWordToolStripMenuItem.Click += new EventHandler(this.ExportToWordToolStripMenuItemClick);
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new Size(6, 25);
			this.layoutToolStripDropDownButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
			this.layoutToolStripDropDownButton.DropDownItems.AddRange(new ToolStripItem[]
			{
				this.chineseToolStripMenuItem,
				this.hanVietToolStripMenuItem,
				this.vietToolStripMenuItem,
				this.vietPhraseToolStripMenuItem,
				this.vietPhraseOneMeaningToolStripMenuItem
			});
//			this.layoutToolStripDropDownButton.Image = (Image)componentResourceManager.GetObject("layoutToolStripDropDownButton.Image");
			this.layoutToolStripDropDownButton.ImageTransparentColor = Color.Magenta;
			this.layoutToolStripDropDownButton.Name = "layoutToolStripDropDownButton";
			this.layoutToolStripDropDownButton.Size = new Size(56, 22);
			this.layoutToolStripDropDownButton.Text = "&Layout";
			this.chineseToolStripMenuItem.Checked = true;
			this.chineseToolStripMenuItem.CheckState = CheckState.Checked;
			this.chineseToolStripMenuItem.Enabled = false;
			this.chineseToolStripMenuItem.Name = "chineseToolStripMenuItem";
			this.chineseToolStripMenuItem.Size = new Size(189, 22);
			this.chineseToolStripMenuItem.Text = "Trung (required)";
			this.hanVietToolStripMenuItem.Checked = true;
			this.hanVietToolStripMenuItem.CheckState = CheckState.Checked;
			this.hanVietToolStripMenuItem.Enabled = false;
			this.hanVietToolStripMenuItem.Name = "hanVietToolStripMenuItem";
			this.hanVietToolStripMenuItem.Size = new Size(189, 22);
			this.hanVietToolStripMenuItem.Text = "Hán Việt (required)";
			this.vietToolStripMenuItem.Checked = true;
			this.vietToolStripMenuItem.CheckState = CheckState.Checked;
			this.vietToolStripMenuItem.Enabled = false;
			this.vietToolStripMenuItem.Name = "vietToolStripMenuItem";
			this.vietToolStripMenuItem.Size = new Size(189, 22);
			this.vietToolStripMenuItem.Text = "Việt (required)";
			this.vietPhraseToolStripMenuItem.Checked = true;
			this.vietPhraseToolStripMenuItem.CheckState = CheckState.Checked;
			this.vietPhraseToolStripMenuItem.Name = "vietPhraseToolStripMenuItem";
			this.vietPhraseToolStripMenuItem.Size = new Size(189, 22);
			this.vietPhraseToolStripMenuItem.Text = "&VietPhrase";
			this.vietPhraseToolStripMenuItem.CheckStateChanged += new EventHandler(this.VietPhraseToolStripMenuItemCheckStateChanged);
			this.vietPhraseToolStripMenuItem.Click += new EventHandler(this.VietPhraseToolStripMenuItemClick);
			this.vietPhraseOneMeaningToolStripMenuItem.Checked = true;
			this.vietPhraseOneMeaningToolStripMenuItem.CheckState = CheckState.Checked;
			this.vietPhraseOneMeaningToolStripMenuItem.Name = "vietPhraseOneMeaningToolStripMenuItem";
			this.vietPhraseOneMeaningToolStripMenuItem.Size = new Size(189, 22);
			this.vietPhraseOneMeaningToolStripMenuItem.Text = "VietPhrase &Một Nghĩa";
			this.vietPhraseOneMeaningToolStripMenuItem.CheckStateChanged += new EventHandler(this.VietPhraseOneMeaningToolStripMenuItemCheckStateChanged);
			this.vietPhraseOneMeaningToolStripMenuItem.Click += new EventHandler(this.VietPhraseOneMeaningToolStripMenuItemClick);
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new Size(6, 25);
			this.translateFromClipboardToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
//			this.translateFromClipboardToolStripButton.Image = (Image)componentResourceManager.GetObject("translateFromClipboardToolStripButton.Image");
			this.translateFromClipboardToolStripButton.ImageTransparentColor = Color.Magenta;
			this.translateFromClipboardToolStripButton.Name = "translateFromClipboardToolStripButton";
			this.translateFromClipboardToolStripButton.Size = new Size(145, 22);
			this.translateFromClipboardToolStripButton.Text = "&Translate From Clipboard";
			this.translateFromClipboardToolStripButton.Click += new EventHandler(this.TranslateFromClipboardToolStripButtonClick);
			this.toolStripSeparator9.Name = "toolStripSeparator9";
			this.toolStripSeparator9.Size = new Size(6, 25);
			this.retranslateToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
//			this.retranslateToolStripButton.Image = (Image)componentResourceManager.GetObject("retranslateToolStripButton.Image");
			this.retranslateToolStripButton.ImageTransparentColor = Color.Magenta;
			this.retranslateToolStripButton.Name = "retranslateToolStripButton";
			this.retranslateToolStripButton.Size = new Size(77, 22);
			this.retranslateToolStripButton.Text = "&Re-Translate";
			this.retranslateToolStripButton.Click += new EventHandler(this.RetranslateToolStripButtonClick);
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new Size(6, 25);
			this.updateVietPhraseToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
			this.updateVietPhraseToolStripButton.Enabled = false;
//			this.updateVietPhraseToolStripButton.Image = (Image)componentResourceManager.GetObject("updateVietPhraseToolStripButton.Image");
			this.updateVietPhraseToolStripButton.ImageTransparentColor = Color.Magenta;
			this.updateVietPhraseToolStripButton.Name = "updateVietPhraseToolStripButton";
			this.updateVietPhraseToolStripButton.Size = new Size(107, 22);
			this.updateVietPhraseToolStripButton.Text = "Update &VietPhrase";
			this.updateVietPhraseToolStripButton.Visible = false;
			this.updateVietPhraseToolStripButton.Click += new EventHandler(this.UpdateVietPhraseToolStripButtonClick);
			this.toolStripSeparator8.Name = "toolStripSeparator8";
			this.toolStripSeparator8.Size = new Size(6, 25);
			this.toolStripSeparator8.Visible = false;
			this.updateNameToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
			this.updateNameToolStripButton.Enabled = false;
//			this.updateNameToolStripButton.Image = (Image)componentResourceManager.GetObject("updateNameToolStripButton.Image");
			this.updateNameToolStripButton.ImageTransparentColor = Color.Magenta;
			this.updateNameToolStripButton.Name = "updateNameToolStripButton";
			this.updateNameToolStripButton.Size = new Size(84, 22);
			this.updateNameToolStripButton.Text = "Update &Name";
			this.updateNameToolStripButton.Visible = false;
			this.updateNameToolStripButton.Click += new EventHandler(this.UpdateNameToolStripButtonClick);
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new Size(6, 25);
			this.toolStripSeparator1.Visible = false;
			this.autoScrollToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
//			this.autoScrollToolStripButton.Image = (Image)componentResourceManager.GetObject("autoScrollToolStripButton.Image");
			this.autoScrollToolStripButton.ImageTransparentColor = Color.Magenta;
			this.autoScrollToolStripButton.Name = "autoScrollToolStripButton";
			this.autoScrollToolStripButton.Size = new Size(69, 22);
			this.autoScrollToolStripButton.Text = "&Auto Scroll";
			this.autoScrollToolStripButton.Click += new EventHandler(this.AutoScrollToolStripButtonClick);
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new Size(6, 25);
			this.reloadDictToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
//			this.reloadDictToolStripButton.Image = (Image)componentResourceManager.GetObject("reloadDictToolStripButton.Image");
			this.reloadDictToolStripButton.ImageTransparentColor = Color.Magenta;
			this.reloadDictToolStripButton.Name = "reloadDictToolStripButton";
			this.reloadDictToolStripButton.Size = new Size(76, 22);
			this.reloadDictToolStripButton.Text = "Reload &Dicts";
			this.reloadDictToolStripButton.Click += new EventHandler(this.ReloadDictToolStripButtonClick);
			this.toolStripButton10.Name = "toolStripButton10";
			this.toolStripButton10.Size = new Size(6, 25);
			this.toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.None;
//			this.toolStripButton1.Image = (Image)componentResourceManager.GetObject("toolStripButton1.Image");
			this.toolStripButton1.ImageTransparentColor = Color.Magenta;
			this.toolStripButton1.Name = "toolStripButton1";
			this.toolStripButton1.Size = new Size(0, 22);
			this.toolStripButton1.Text = "F1 = : \"";
			this.toolStripButton2.DisplayStyle = ToolStripItemDisplayStyle.None;
//			this.toolStripButton2.Image = (Image)componentResourceManager.GetObject("toolStripButton2.Image");
			this.toolStripButton2.ImageTransparentColor = Color.Magenta;
			this.toolStripButton2.Name = "toolStripButton2";
			this.toolStripButton2.Size = new Size(0, 22);
			this.toolStripButton2.Text = "F2 = .\"";
			this.toolStripButton3.DisplayStyle = ToolStripItemDisplayStyle.None;
//			this.toolStripButton3.Image = (Image)componentResourceManager.GetObject("toolStripButton3.Image");
			this.toolStripButton3.ImageTransparentColor = Color.Magenta;
			this.toolStripButton3.Name = "toolStripButton3";
			this.toolStripButton3.Size = new Size(0, 22);
			this.toolStripButton3.Text = "F3 = ?\"";
			this.toolStripButton4.DisplayStyle = ToolStripItemDisplayStyle.None;
//			this.toolStripButton4.Image = (Image)componentResourceManager.GetObject("toolStripButton4.Image");
			this.toolStripButton4.ImageTransparentColor = Color.Magenta;
			this.toolStripButton4.Name = "toolStripButton4";
			this.toolStripButton4.Size = new Size(0, 22);
			this.toolStripButton4.Text = "F4 = !\"";
			this.toolStripButton7.DisplayStyle = ToolStripItemDisplayStyle.None;
//			this.toolStripButton7.Image = (Image)componentResourceManager.GetObject("toolStripButton7.Image");
			this.toolStripButton7.ImageTransparentColor = Color.Magenta;
			this.toolStripButton7.Name = "toolStripButton7";
			this.toolStripButton7.Size = new Size(0, 22);
			this.toolStripButton7.Text = "F5 = , ";
			this.toolStripButton8.DisplayStyle = ToolStripItemDisplayStyle.None;
//			this.toolStripButton8.Image = (Image)componentResourceManager.GetObject("toolStripButton8.Image");
			this.toolStripButton8.ImageTransparentColor = Color.Magenta;
			this.toolStripButton8.Name = "toolStripButton8";
			this.toolStripButton8.Size = new Size(0, 22);
			this.toolStripButton8.Text = "F6 = . ";
			this.toolStripButton5.DisplayStyle = ToolStripItemDisplayStyle.None;
//			this.toolStripButton5.Image = (Image)componentResourceManager.GetObject("toolStripButton5.Image");
			this.toolStripButton5.ImageTransparentColor = Color.Magenta;
			this.toolStripButton5.Name = "toolStripButton5";
			this.toolStripButton5.Size = new Size(0, 22);
			this.toolStripButton5.Text = "F7 = hay không";
			this.toolStripButton6.DisplayStyle = ToolStripItemDisplayStyle.None;
//			this.toolStripButton6.Image = (Image)componentResourceManager.GetObject("toolStripButton6.Image");
			this.toolStripButton6.ImageTransparentColor = Color.Magenta;
			this.toolStripButton6.Name = "toolStripButton6";
			this.toolStripButton6.Size = new Size(0, 22);
			this.toolStripButton6.Text = "F8 = mà";
			this.toolStripLabel1.BackgroundImageLayout = ImageLayout.Center;
			this.toolStripLabel1.DisplayStyle = ToolStripItemDisplayStyle.None;
			this.toolStripLabel1.Name = "toolStripLabel1";
			this.toolStripLabel1.Size = new Size(0, 22);
			this.toolStripLabel1.Text = "F9 = của";
			this.postTTVToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
//			this.postTTVToolStripButton.Image = (Image)componentResourceManager.GetObject("postTTVToolStripButton.Image");
			this.postTTVToolStripButton.ImageTransparentColor = Color.Magenta;
			this.postTTVToolStripButton.Name = "postTTVToolStripButton";
			this.postTTVToolStripButton.Size = new Size(58, 22);
			this.postTTVToolStripButton.Text = "&Post TTV";
			this.postTTVToolStripButton.Click += new EventHandler(this.PostTTVToolStripButtonClick);
			this.toolStripSeparator10.Name = "toolStripSeparator10";
			this.toolStripSeparator10.Size = new Size(6, 25);
			this.nextToolStripButton.Alignment = ToolStripItemAlignment.Right;
			this.nextToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
			this.nextToolStripButton.Enabled = false;
//			this.nextToolStripButton.Image = (Image)componentResourceManager.GetObject("nextToolStripButton.Image");
			this.nextToolStripButton.ImageTransparentColor = Color.Magenta;
			this.nextToolStripButton.Name = "nextToolStripButton";
			this.nextToolStripButton.Size = new Size(35, 22);
			this.nextToolStripButton.Text = "Next";
			this.nextToolStripButton.ToolTipText = "Next (Alt + Right)";
			this.nextToolStripButton.Click += new EventHandler(this.NextToolStripButtonClick);
			this.backToolStripButton.Alignment = ToolStripItemAlignment.Right;
			this.backToolStripButton.BackColor = SystemColors.Control;
			this.backToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
			this.backToolStripButton.Enabled = false;
//			this.backToolStripButton.Image = (Image)componentResourceManager.GetObject("backToolStripButton.Image");
			this.backToolStripButton.ImageTransparentColor = Color.Magenta;
			this.backToolStripButton.Name = "backToolStripButton";
			this.backToolStripButton.Size = new Size(36, 22);
			this.backToolStripButton.Text = "Back";
			this.backToolStripButton.ToolTipText = "Back (Alt + Left)";
			this.backToolStripButton.Click += new EventHandler(this.BackToolStripButtonClick);
			this.statusStrip1.Items.AddRange(new ToolStripItem[]
			{
				this.toolStripStatusLabel1,
				this.toolStripStatusLabel2,
				this.toolStripStatusLabel3,
				this.wordCountToolStripStatusLabel
			});
			this.statusStrip1.Location = new Point(0, 392);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new Size(877, 22);
			this.statusStrip1.TabIndex = 3;
			this.statusStrip1.Text = "statusStrip1";
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new Size(0, 17);
			this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
			this.toolStripStatusLabel2.Size = new Size(384, 17);
			this.toolStripStatusLabel2.Text = "© 2009-2013 ngoctay@TangThuVien.com - Quick Translator 2013.07.08";
			this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
			this.toolStripStatusLabel3.Size = new Size(447, 17);
			this.toolStripStatusLabel3.Spring = true;
			this.wordCountToolStripStatusLabel.Name = "wordCountToolStripStatusLabel";
			this.wordCountToolStripStatusLabel.Size = new Size(0, 17);
			this.wordCountToolStripStatusLabel.TextAlign = ContentAlignment.MiddleRight;
			this.AllowDrop = true;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(877, 414);
			base.Controls.Add(this.dockPanel);
			base.Controls.Add(this.statusStrip1);
			base.Controls.Add(this.toolStrip1);
//			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.IsMdiContainer = true;
			base.KeyPreview = true;
			base.Name = "MainForm";
			this.Text = "Quick Translator";
			base.WindowState = FormWindowState.Maximized;
			base.FormClosing += new FormClosingEventHandler(this.MainFormFormClosing);
			base.FormClosed += new FormClosedEventHandler(this.MainFormFormClosed);
			base.Load += new EventHandler(this.MainFormLoad);
			base.DragDrop += new DragEventHandler(this.MainForm_DragDrop);
			base.DragEnter += new DragEventHandler(this.MainForm_DragEnter);
			base.KeyDown += new KeyEventHandler(this.MainFormKeyDown);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
