using System;
using UnityEngine;

namespace Crypto
{
	[Serializable]
	public class CryptoFloat : IFormattable, IEquatable<CryptoFloat>
	{
		[SerializeField]
		private int cryptoKey;

		[SerializeField]
		private int hiddenValue;

		[SerializeField]
		private float roundValue;

		[SerializeField]
		private float fakeValue;

		private CryptoFloat(float value)
		{
			SetValue(value);
		}

		public void SetValue(float value)
		{
			System.Random random = new System.Random();
			cryptoKey = random.Next(-5000, 5000);
			roundValue = value - (float)(int)value;
			fakeValue = value;
			hiddenValue = cryptoKey - (int)value;
		}

		private float GetValue()
		{
			float num = (float)(cryptoKey - hiddenValue) + roundValue;
			if (num != fakeValue)
			{
				CryptoDetector.Detected();
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is CryptoFloat))
			{
				return false;
			}
			return Equals((CryptoFloat)obj);
		}

		public bool Equals(CryptoFloat obj)
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

		public static implicit operator CryptoFloat(float value)
		{
			return new CryptoFloat(value);
		}

		public static implicit operator float(CryptoFloat value)
		{
			if (value == null)
			{
				value = new CryptoFloat(0f);
			}
			return value.GetValue();
		}

		public static CryptoFloat operator ++(CryptoFloat value)
		{
			if (value == null)
			{
				value = new CryptoFloat(1f);
			}
			else
			{
				value.SetValue(value.GetValue() + 1f);
			}
			return value;
		}

		public static CryptoFloat operator --(CryptoFloat value)
		{
			if (value == null)
			{
				value = new CryptoFloat(-1f);
			}
			else
			{
				value.SetValue(value.GetValue() - 1f);
			}
			return value;
		}
	}
}
