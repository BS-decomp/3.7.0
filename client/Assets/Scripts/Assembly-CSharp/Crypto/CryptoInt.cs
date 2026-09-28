using System;
using UnityEngine;

namespace Crypto
{
	[Serializable]
	public class CryptoInt : IFormattable, IEquatable<CryptoInt>
	{
		[SerializeField]
		private int cryptoKey;

		[SerializeField]
		private int hiddenValue;

		[SerializeField]
		private int fakeValue;

		private CryptoInt(int value)
		{
			SetValue(value);
		}

		public void SetValue(int value)
		{
			System.Random random = new System.Random();
			cryptoKey = random.Next(-50000, 50000);
			fakeValue = value;
			hiddenValue = cryptoKey - value;
		}

		private int GetValue()
		{
			int num = cryptoKey - hiddenValue;
			if (num != fakeValue)
			{
				CryptoDetector.Detected();
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is CryptoInt))
			{
				return false;
			}
			return Equals((CryptoInt)obj);
		}

		public bool Equals(CryptoInt obj)
		{
			return GetValue() == obj.GetValue();
		}

		public override int GetHashCode()
		{
			return GetValue().GetHashCode();
		}

		public override string ToString()
		{
			return GetValue().ToString();
		}

		public string ToString(string format)
		{
			return GetValue().ToString(format);
		}

		public string ToString(IFormatProvider provider)
		{
			return GetValue().ToString(provider);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			return GetValue().ToString(format, provider);
		}

		public static implicit operator CryptoInt(int value)
		{
			return new CryptoInt(value);
		}

		public static implicit operator int(CryptoInt value)
		{
			if (value == null)
			{
				value = new CryptoInt(0);
			}
			return value.GetValue();
		}

		public static CryptoInt operator ++(CryptoInt value)
		{
			if (value == null)
			{
				value = new CryptoInt(1);
			}
			else
			{
				value.SetValue(value.GetValue() + 1);
			}
			return value;
		}

		public static CryptoInt operator --(CryptoInt value)
		{
			if (value == null)
			{
				value = new CryptoInt(-1);
			}
			else
			{
				value.SetValue(value.GetValue() - 1);
			}
			return value;
		}
	}
}
