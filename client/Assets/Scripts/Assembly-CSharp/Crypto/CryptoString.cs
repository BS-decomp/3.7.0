using System;
using UnityEngine;

namespace Crypto
{
	[Serializable]
	public class CryptoString : IEquatable<CryptoString>
	{
		[SerializeField]
		private int cryptoKey;

		[SerializeField]
		private int[] hiddenValue;

		private CryptoString(string value)
		{
			SetValue(value);
		}

		public void SetValue(string value)
		{
			System.Random random = new System.Random();
			cryptoKey = random.Next(-500, 500);
			hiddenValue = Encrypt(value, cryptoKey);
		}

		public static int[] Encrypt(string text, int key)
		{
			int[] array = new int[text.Length];
			for (int i = 0; i < text.Length; i++)
			{
				array[i] = text[i] + key + i;
			}
			return array;
		}

		public static string Decrypt(int[] value, int key)
		{
			string text = string.Empty;
			for (int i = 0; i < value.Length; i++)
			{
				text += (char)(value[i] - key - i);
			}
			return text;
		}

		private string GetValue()
		{
			string result = string.Empty;
			if (hiddenValue != null && hiddenValue.Length != 0)
			{
				result = Decrypt(hiddenValue, cryptoKey);
			}
			return result;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is CryptoString))
			{
				return false;
			}
			return Equals((CryptoString)obj);
		}

		public bool Equals(CryptoString obj)
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

		public static implicit operator CryptoString(string value)
		{
			return new CryptoString(value);
		}

		public static implicit operator string(CryptoString value)
		{
			if (value == null)
			{
				value = new CryptoString(string.Empty);
			}
			return value.GetValue();
		}
	}
}
