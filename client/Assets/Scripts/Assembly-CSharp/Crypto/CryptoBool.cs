using System;
using UnityEngine;

namespace Crypto
{
	[Serializable]
	public class CryptoBool : IEquatable<CryptoBool>
	{
		[SerializeField]
		private int hiddenValue = 531;

		[SerializeField]
		private bool fakeValue;

		private CryptoBool(bool value)
		{
			SetValue(value);
		}

		public void SetValue(bool value)
		{
			fakeValue = value;
			hiddenValue = GetRandomValue(value);
		}

		private int GetRandomValue(bool even)
		{
			System.Random random = new System.Random();
			int num = random.Next(0, 5000);
			if (num % 2 == ((!even) ? 1 : 0))
			{
				return num;
			}
			return num + 1;
		}

		private bool GetValue()
		{
			bool flag = hiddenValue % 2 == 0;
			if (flag != fakeValue)
			{
				CryptoDetector.Detected();
			}
			return flag;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is CryptoBool))
			{
				return false;
			}
			return Equals((CryptoBool)obj);
		}

		public bool Equals(CryptoBool obj)
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

		public static implicit operator CryptoBool(bool value)
		{
			return new CryptoBool(value);
		}

		public static implicit operator bool(CryptoBool value)
		{
			if (value == null)
			{
				value = new CryptoBool(false);
			}
			return value.GetValue();
		}
	}
}
