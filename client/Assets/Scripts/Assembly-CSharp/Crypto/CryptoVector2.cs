using System;
using UnityEngine;

namespace Crypto
{
	[Serializable]
	public class CryptoVector2 : IEquatable<CryptoVector2>
	{
		[SerializeField]
		private Vector2 cryptoKey;

		[SerializeField]
		private Vector2 hiddenValue;

		[SerializeField]
		private Vector2 roundValue;

		[SerializeField]
		private Vector2 fakeValue;

		private CryptoVector2(Vector2 value)
		{
			SetValue(value);
		}

		public void SetValue(Vector2 value)
		{
			System.Random random = new System.Random();
			cryptoKey = new Vector2(random.Next(-5000, 5000), random.Next(-5000, 5000));
			roundValue = new Vector2(value.x - (float)(int)value.x, value.y - (float)(int)value.y);
			fakeValue = value;
			hiddenValue = new Vector2(cryptoKey.x - (float)(int)value.x, cryptoKey.y - (float)(int)value.y);
		}

		private Vector2 GetValue()
		{
			Vector2 vector = new Vector2(cryptoKey.x - hiddenValue.x + roundValue.x, cryptoKey.y - hiddenValue.y + roundValue.y);
			if (vector != fakeValue)
			{
				CryptoDetector.Detected();
			}
			return vector;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is CryptoVector2))
			{
				return false;
			}
			return Equals((CryptoVector2)obj);
		}

		public bool Equals(CryptoVector2 obj)
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

		public static implicit operator CryptoVector2(Vector2 value)
		{
			return new CryptoVector2(value);
		}

		public static implicit operator Vector2(CryptoVector2 value)
		{
			return value.GetValue();
		}
	}
}
