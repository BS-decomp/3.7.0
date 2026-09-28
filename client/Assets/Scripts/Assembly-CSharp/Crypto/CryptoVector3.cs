using System;
using UnityEngine;

namespace Crypto
{
	[Serializable]
	public class CryptoVector3 : IEquatable<CryptoVector3>
	{
		[SerializeField]
		private Vector3 cryptoKey;

		[SerializeField]
		private Vector3 hiddenValue;

		[SerializeField]
		private Vector3 roundValue;

		[SerializeField]
		private Vector3 fakeValue;

		private CryptoVector3(Vector3 value)
		{
			SetValue(value);
		}

		public void SetValue(Vector3 value)
		{
			System.Random random = new System.Random();
			cryptoKey = new Vector3(random.Next(-5000, 5000), random.Next(-5000, 5000), random.Next(-5000, 5000));
			roundValue = new Vector3(value.x - (float)(int)value.x, value.y - (float)(int)value.y, value.z - (float)(int)value.z);
			fakeValue = value;
			hiddenValue = new Vector3(cryptoKey.x - (float)(int)value.x, cryptoKey.y - (float)(int)value.y, cryptoKey.z - (float)(int)value.z);
		}

		private Vector3 GetValue()
		{
			Vector3 vector = new Vector3(cryptoKey.x - hiddenValue.x + roundValue.x, cryptoKey.y - hiddenValue.y + roundValue.y, cryptoKey.z - hiddenValue.z + roundValue.z);
			if (vector != fakeValue)
			{
				CryptoDetector.Detected();
			}
			return vector;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is CryptoVector3))
			{
				return false;
			}
			return Equals((CryptoVector3)obj);
		}

		public bool Equals(CryptoVector3 obj)
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

		public static implicit operator CryptoVector3(Vector3 value)
		{
			return new CryptoVector3(value);
		}

		public static implicit operator Vector3(CryptoVector3 value)
		{
			return value.GetValue();
		}
	}
}
