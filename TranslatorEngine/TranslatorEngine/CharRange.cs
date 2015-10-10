using System;

namespace TranslatorEngine
{
	public class CharRange
	{
		private int startIndex;

		private int length;

		public int StartIndex
		{
			get
			{
				return this.startIndex;
			}
			set
			{
				this.startIndex = value;
			}
		}

		public int Length
		{
			get
			{
				return this.length;
			}
			set
			{
				this.length = value;
			}
		}

		public CharRange(int startIndex, int length)
		{
			this.startIndex = startIndex;
			this.length = length;
		}

		public bool IsInRange(int index)
		{
			return this.startIndex <= index && index <= this.startIndex + this.length - 1;
		}

		public int GetEndIndex()
		{
			return this.startIndex + this.length - 1;
		}
	}
}
