using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace QuickTranslator
{
	[GeneratedCode("ICSharpCode.SettingsEditor.SettingsCodeGeneratorTool", "4.3.1.9430"), CompilerGenerated]
	internal sealed class PostTTVFormSettings : ApplicationSettingsBase
	{
		private static PostTTVFormSettings defaultInstance = (PostTTVFormSettings)SettingsBase.Synchronized(new PostTTVFormSettings());

		public static PostTTVFormSettings Default
		{
			get
			{
				return PostTTVFormSettings.defaultInstance;
			}
		}

		[DefaultSettingValue(""), UserScopedSetting, DebuggerNonUserCode]
		public string discussionUrl
		{
			get
			{
				return (string)this["discussionUrl"];
			}
			set
			{
				this["discussionUrl"] = value;
			}
		}

		[DefaultSettingValue(""), UserScopedSetting, DebuggerNonUserCode]
		public string line1
		{
			get
			{
				return (string)this["line1"];
			}
			set
			{
				this["line1"] = value;
			}
		}

		[DefaultSettingValue(""), UserScopedSetting, DebuggerNonUserCode]
		public string line2
		{
			get
			{
				return (string)this["line2"];
			}
			set
			{
				this["line2"] = value;
			}
		}

		[DefaultSettingValue(""), UserScopedSetting, DebuggerNonUserCode]
		public string line3
		{
			get
			{
				return (string)this["line3"];
			}
			set
			{
				this["line3"] = value;
			}
		}

		[DefaultSettingValue(""), UserScopedSetting, DebuggerNonUserCode]
		public string line4
		{
			get
			{
				return (string)this["line4"];
			}
			set
			{
				this["line4"] = value;
			}
		}

		[DefaultSettingValue("True"), UserScopedSetting, DebuggerNonUserCode]
		public bool spoilerHanViet
		{
			get
			{
				return (bool)this["spoilerHanViet"];
			}
			set
			{
				this["spoilerHanViet"] = value;
			}
		}

		[DefaultSettingValue("True"), UserScopedSetting, DebuggerNonUserCode]
		public bool spoilerTrung
		{
			get
			{
				return (bool)this["spoilerTrung"];
			}
			set
			{
				this["spoilerTrung"] = value;
			}
		}

		[DefaultSettingValue("False"), UserScopedSetting, DebuggerNonUserCode]
		public bool spoilerViet
		{
			get
			{
				return (bool)this["spoilerViet"];
			}
			set
			{
				this["spoilerViet"] = value;
			}
		}

		[DefaultSettingValue("True"), UserScopedSetting, DebuggerNonUserCode]
		public bool spoilerVietPhraseOneMeaning
		{
			get
			{
				return (bool)this["spoilerVietPhraseOneMeaning"];
			}
			set
			{
				this["spoilerVietPhraseOneMeaning"] = value;
			}
		}
	}
}
