using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Crypto
{
	public static class CryptoPrefs
	{
		private static string cryptokey;

		private static UTF8Encoding encoder;

		private static AesManaged aes;

		static CryptoPrefs()
		{
			cryptokey = "defaultKeyString";
			SetCryptoKey(cryptokey);
		}

		public static void SetCryptoKey(string key)
		{
			cryptokey = GetDeviceId(key);
			encoder = new UTF8Encoding();
			AesManaged aesManaged = new AesManaged();
			aesManaged.Key = encoder.GetBytes(cryptokey).Take(16).ToArray();
			aes = aesManaged;
			aes.BlockSize = 128;
		}

		public static string GetDeviceId(string key)
		{
			string empty = string.Empty;
			empty = ((Application.platform != RuntimePlatform.Android) ? (SystemInfo.deviceUniqueIdentifier + key) : (AndroidNativeFunctions.GetAndroidID() + key));
			return empty.Substring(0, 16);
		}

		private static byte[] GenerateIV()
		{
			aes.GenerateIV();
			return aes.IV;
		}

		private static byte[] Encrypt(byte[] buffer)
		{
			aes.GenerateIV();
			using (ICryptoTransform cryptoTransform = aes.CreateEncryptor())
			{
				byte[] second = cryptoTransform.TransformFinalBlock(buffer, 0, buffer.Length);
				return aes.IV.Concat(second).ToArray();
			}
		}

		private static byte[] EncryptKeyIV(byte[] buffer, byte[] IV)
		{
			using (ICryptoTransform cryptoTransform = aes.CreateEncryptor(aes.Key, IV))
			{
				return cryptoTransform.TransformFinalBlock(buffer, 0, buffer.Length);
			}
		}

		private static byte[] Decrypt(byte[] buffer)
		{
			byte[] rgbIV = buffer.Take(16).ToArray();
			using (ICryptoTransform cryptoTransform = aes.CreateDecryptor(aes.Key, rgbIV))
			{
				return cryptoTransform.TransformFinalBlock(buffer, 16, buffer.Length - 16);
			}
		}

		private static string GetEncryptPrefsKey(string key)
		{
			return Convert.ToBase64String(EncryptKeyIV(encoder.GetBytes(key), encoder.GetBytes(cryptokey)));
		}

		public static void Save()
		{
			PlayerPrefs.Save();
		}

		public static bool HasKey(string key)
		{
			return PlayerPrefs.HasKey(GetEncryptPrefsKey(key));
		}

		public static void DeleteKey(string key)
		{
			PlayerPrefs.DeleteKey(GetEncryptPrefsKey(key));
		}

		public static void DeleteAll()
		{
			PlayerPrefs.DeleteAll();
		}

		public static void SetString(string key, string value)
		{
			key = GetEncryptPrefsKey(key);
			value = Convert.ToBase64String(Encrypt(encoder.GetBytes(value)));
			PlayerPrefs.SetString(key, value);
		}

		public static string GetString(string key)
		{
			return GetString(key, string.Empty);
		}

		public static string GetString(string key, string defautValue)
		{
			key = GetEncryptPrefsKey(key);
			string text = PlayerPrefs.GetString(key, defautValue);
			if (text == defautValue)
			{
				return defautValue;
			}
			try
			{
				byte[] array = Decrypt(Convert.FromBase64String(text));
				return encoder.GetString(array, 0, array.Length);
			}
			catch
			{
				return defautValue;
			}
		}

		public static void SetInt(string key, int value)
		{
			PlayerPrefs.SetString(GetEncryptPrefsKey(key), Convert.ToBase64String(Encrypt(BitConverter.GetBytes(value))));
		}

		public static int GetInt(string key)
		{
			return GetInt(key, 0);
		}

		public static int GetInt(string key, int defautValue)
		{
			key = GetEncryptPrefsKey(key);
			string text = PlayerPrefs.GetString(key, defautValue.ToString());
			if (text == defautValue.ToString())
			{
				return defautValue;
			}
			try
			{
				return BitConverter.ToInt32(Decrypt(Convert.FromBase64String(text)), 0);
			}
			catch
			{
				return defautValue;
			}
		}

		public static void SetFloat(string key, float value)
		{
			PlayerPrefs.SetString(GetEncryptPrefsKey(key), Convert.ToBase64String(Encrypt(BitConverter.GetBytes(value))));
		}

		public static float GetFloat(string key)
		{
			return GetFloat(key, 0f);
		}

		public static float GetFloat(string key, float defautValue)
		{
			key = GetEncryptPrefsKey(key);
			string text = PlayerPrefs.GetString(key, defautValue.ToString());
			if (text == defautValue.ToString())
			{
				return defautValue;
			}
			try
			{
				return BitConverter.ToSingle(Decrypt(Convert.FromBase64String(text)), 0);
			}
			catch
			{
				return defautValue;
			}
		}

		public static void SetBool(string key, bool value)
		{
			PlayerPrefs.SetString(GetEncryptPrefsKey(key), Convert.ToBase64String(Encrypt(BitConverter.GetBytes(value))));
		}

		public static bool GetBool(string key)
		{
			return GetBool(key, false);
		}

		public static bool GetBool(string key, bool defautValue)
		{
			key = GetEncryptPrefsKey(key);
			string text = PlayerPrefs.GetString(key, defautValue.ToString());
			if (text == defautValue.ToString())
			{
				return defautValue;
			}
			try
			{
				return BitConverter.ToBoolean(Decrypt(Convert.FromBase64String(text)), 0);
			}
			catch
			{
				return defautValue;
			}
		}

		public static void SetVector2(string key, Vector2 value)
		{
			string s = value.x + "|" + value.y;
			PlayerPrefs.SetString(GetEncryptPrefsKey(key), Convert.ToBase64String(Encrypt(encoder.GetBytes(s))));
		}

		public static Vector2 GetVector2(string key)
		{
			return GetVector2(key, Vector2.zero);
		}

		public static Vector2 GetVector2(string key, Vector2 defautValue)
		{
			key = GetEncryptPrefsKey(key);
			string text = PlayerPrefs.GetString(key, defautValue.ToString());
			if (text == defautValue.ToString())
			{
				return defautValue;
			}
			try
			{
				byte[] array = Decrypt(Convert.FromBase64String(text));
				string[] array2 = encoder.GetString(array, 0, array.Length).Split("|"[0]);
				return new Vector2(float.Parse(array2[0]), float.Parse(array2[1]));
			}
			catch
			{
				return defautValue;
			}
		}

		public static void SetVector3(string key, Vector3 value)
		{
			string s = value.x + "|" + value.y + "|" + value.z;
			PlayerPrefs.SetString(GetEncryptPrefsKey(key), Convert.ToBase64String(Encrypt(encoder.GetBytes(s))));
		}

		public static Vector2 GetVector3(string key)
		{
			return GetVector2(key, Vector3.zero);
		}

		public static Vector2 GetVector3(string key, Vector3 defautValue)
		{
			key = GetEncryptPrefsKey(key);
			string text = PlayerPrefs.GetString(key, defautValue.ToString());
			if (text == defautValue.ToString())
			{
				return defautValue;
			}
			try
			{
				byte[] array = Decrypt(Convert.FromBase64String(text));
				string[] array2 = encoder.GetString(array, 0, array.Length).Split("|"[0]);
				return new Vector3(float.Parse(array2[0]), float.Parse(array2[1]), float.Parse(array2[2]));
			}
			catch
			{
				return defautValue;
			}
		}
	}
}
