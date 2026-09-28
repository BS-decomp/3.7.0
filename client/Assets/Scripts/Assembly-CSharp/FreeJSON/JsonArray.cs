using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FreeJSON
{
	public class JsonArray
	{
		private class Parser
		{
			private static Stack<List<string>> splitArrayPool = new Stack<List<string>>();

			private static JsonArray jsonArray = new JsonArray();

			private static StringBuilder stringBuilder = new StringBuilder();

			public static JsonArray Parse(string json)
			{
				for (int i = 0; i < json.Length; i++)
				{
					char c = json[i];
					if (c == '"')
					{
						i = AppendUntilStringEnd(true, i, json);
					}
					else if (!char.IsWhiteSpace(c))
					{
						stringBuilder.Append(c);
					}
				}
				List<string> list = Split(json);
				jsonArray = new JsonArray();
				for (int j = 0; j < list.Count; j++)
				{
					if (!string.IsNullOrEmpty(list[j]))
					{
						jsonArray.values.Add(list[j]);
					}
				}
				return jsonArray;
			}

			private static int AppendUntilStringEnd(bool appendEscapeCharacter, int startIdx, string json)
			{
				stringBuilder.Append(json[startIdx]);
				for (int i = startIdx + 1; i < json.Length; i++)
				{
					if (json[i] == '\\')
					{
						if (appendEscapeCharacter)
						{
							stringBuilder.Append(json[i]);
						}
						stringBuilder.Append(json[i + 1]);
						i++;
					}
					else
					{
						if (json[i] == '"')
						{
							stringBuilder.Append(json[i]);
							return i;
						}
						stringBuilder.Append(json[i]);
					}
				}
				return json.Length - 1;
			}

			private static List<string> Split(string json)
			{
				List<string> list = ((splitArrayPool.Count <= 0) ? new List<string>() : splitArrayPool.Pop());
				list.Clear();
				int num = 0;
				stringBuilder.Length = 0;
				for (int i = 1; i < json.Length - 1; i++)
				{
					switch (json[i])
					{
					case '[':
					case '{':
						num++;
						break;
					case ']':
					case '}':
						num--;
						break;
					case '"':
						i = AppendUntilStringEnd(true, i, json);
						continue;
					case ',':
					case ':':
						if (num == 0)
						{
							list.Add(stringBuilder.ToString());
							stringBuilder.Length = 0;
							continue;
						}
						break;
					}
					stringBuilder.Append(json[i]);
				}
				list.Add(stringBuilder.ToString());
				return list;
			}
		}

		private List<string> values = new List<string>();

		public int Length
		{
			get
			{
				return values.Count;
			}
		}

		public bool HasValue(int index)
		{
			if (values.Count - 1 >= index && values[index] != string.Empty && values[index] != "null")
			{
				return true;
			}
			return false;
		}

		public bool Contains(object value)
		{
			string text = Json.ConvertType(value);
			if (string.IsNullOrEmpty(text))
			{
				Debug.LogWarning("Json does not support this type");
				return false;
			}
			return values.Contains(text);
		}

		public void RemoveAt(int index)
		{
			values.RemoveAt(index);
		}

		public bool Remove(object value)
		{
			string text = Json.ConvertType(value);
			if (string.IsNullOrEmpty(text))
			{
				Debug.LogWarning("Json does not support this type");
				return false;
			}
			return values.Remove(text);
		}

		public void Add(object value)
		{
			string text = Json.ConvertType(value);
			if (string.IsNullOrEmpty(text))
			{
				Debug.LogWarning("Json does not support this type");
			}
			else
			{
				values.Add(text);
			}
		}

		public T Get<T>(int index)
		{
			return (T)Get(index, typeof(T));
		}

		public object Get(int index, Type type)
		{
			if (type == typeof(string))
			{
				string text = string.Empty;
				if (index <= values.Count - 1)
				{
					text = values[index];
					if (text[0] == '"')
					{
						text = text.Remove(0, 1);
					}
					if (text[text.Length - 1] == '"')
					{
						text = text.Remove(text.Length - 1, 1);
					}
				}
				return text;
			}
			if (type == typeof(int))
			{
				int result = 0;
				if (index <= values.Count - 1)
				{
					int.TryParse(values[index], out result);
				}
				return result;
			}
			if (type == typeof(float))
			{
				float result2 = 0f;
				if (index <= values.Count - 1)
				{
					float.TryParse(values[index], out result2);
				}
				return result2;
			}
			if (type == typeof(bool))
			{
				bool result3 = false;
				if (index <= values.Count - 1)
				{
					bool.TryParse(values[index], out result3);
				}
				return result3;
			}
			if (type == typeof(byte))
			{
				byte result4 = 0;
				if (index <= values.Count - 1)
				{
					byte.TryParse(values[index], out result4);
				}
				return result4;
			}
			if (type == typeof(short))
			{
				short result5 = 0;
				if (index <= values.Count - 1)
				{
					short.TryParse(values[index], out result5);
				}
				return result5;
			}
			if (type == typeof(long))
			{
				long result6 = 0L;
				if (index <= values.Count - 1)
				{
					long.TryParse(values[index], out result6);
				}
				return result6;
			}
			if (type == typeof(double))
			{
				double result7 = 0.0;
				if (index <= values.Count - 1)
				{
					double.TryParse(values[index], out result7);
				}
				return result7;
			}
			if (type.IsEnum)
			{
				if (index <= values.Count - 1)
				{
					return (Enum)Enum.Parse(type, Get<string>(index));
				}
				return (Enum)Enum.ToObject(type, 0);
			}
			if (type == typeof(Vector2))
			{
				Vector2 vector = default(Vector2);
				if (index <= values.Count - 1)
				{
					JsonObject jsonObject = Get<JsonObject>(index);
					vector.x = jsonObject.Get<float>("x");
					vector.y = jsonObject.Get<float>("y");
				}
				return vector;
			}
			if (type == typeof(Vector3))
			{
				Vector3 vector2 = default(Vector3);
				if (index <= values.Count - 1)
				{
					JsonObject jsonObject2 = Get<JsonObject>(index);
					vector2.x = jsonObject2.Get<float>("x");
					vector2.y = jsonObject2.Get<float>("y");
					vector2.z = jsonObject2.Get<float>("z");
				}
				return vector2;
			}
			if (type == typeof(Vector4))
			{
				Vector4 vector3 = default(Vector4);
				if (index <= values.Count - 1)
				{
					JsonObject jsonObject3 = Get<JsonObject>(index);
					vector3.x = jsonObject3.Get<float>("x");
					vector3.y = jsonObject3.Get<float>("y");
					vector3.z = jsonObject3.Get<float>("z");
					vector3.w = jsonObject3.Get<float>("w");
				}
				return vector3;
			}
			if (type == typeof(Quaternion))
			{
				Quaternion quaternion = default(Quaternion);
				if (index <= values.Count - 1)
				{
					JsonObject jsonObject4 = Get<JsonObject>(index);
					quaternion.x = jsonObject4.Get<float>("x");
					quaternion.y = jsonObject4.Get<float>("y");
					quaternion.z = jsonObject4.Get<float>("z");
					quaternion.w = jsonObject4.Get<float>("w");
				}
				return quaternion;
			}
			if (type == typeof(Color))
			{
				Color color = default(Color);
				if (index <= values.Count - 1)
				{
					JsonObject jsonObject5 = Get<JsonObject>(index);
					color.r = jsonObject5.Get<float>("r");
					color.g = jsonObject5.Get<float>("g");
					color.b = jsonObject5.Get<float>("b");
					color.a = jsonObject5.Get<float>("a");
				}
				return color;
			}
			if (type == typeof(Color32))
			{
				Color32 color2 = default(Color32);
				if (index <= values.Count - 1)
				{
					JsonObject jsonObject6 = Get<JsonObject>(index);
					color2.r = jsonObject6.Get<byte>("r");
					color2.g = jsonObject6.Get<byte>("g");
					color2.b = jsonObject6.Get<byte>("b");
					color2.a = jsonObject6.Get<byte>("a");
				}
				return color2;
			}
			if (type == typeof(Rect))
			{
				Rect rect = default(Rect);
				if (index <= values.Count - 1)
				{
					JsonObject jsonObject7 = Get<JsonObject>(index);
					rect.x = jsonObject7.Get<float>("x");
					rect.y = jsonObject7.Get<float>("y");
					rect.width = jsonObject7.Get<float>("width");
					rect.height = jsonObject7.Get<float>("height");
				}
				return rect;
			}
			if (type.IsArray)
			{
				Array array;
				if (index <= values.Count - 1)
				{
					JsonArray jsonArray = Get<JsonArray>(index);
					array = Array.CreateInstance(type.GetElementType(), jsonArray.Length);
					for (int i = 0; i < jsonArray.Length; i++)
					{
						array.SetValue(jsonArray.Get(i, type.GetElementType()), i);
					}
				}
				else
				{
					array = Array.CreateInstance(type.GetElementType(), 0);
				}
				return array;
			}
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
			{
				IList list = null;
				if (index <= values.Count - 1)
				{
					JsonArray jsonArray2 = (JsonArray)Get(index, typeof(JsonArray));
					Type type2 = type.GetGenericArguments()[0];
					list = (IList)type.GetConstructor(new Type[1] { typeof(int) }).Invoke(new object[1] { jsonArray2.Length });
					for (int j = 0; j < jsonArray2.Length; j++)
					{
						list.Add(jsonArray2.Get(j, type2));
					}
				}
				else
				{
					list = (IList)type.GetConstructor(new Type[1] { typeof(int) }).Invoke(new object[1] { 0 });
				}
				return list;
			}
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<, >))
			{
				IDictionary dictionary = null;
				if (index <= values.Count - 1)
				{
					JsonObject jsonObject8 = Get<JsonObject>(index);
					Type type3 = type.GetGenericArguments()[0];
					Type type4 = type.GetGenericArguments()[1];
					if (type3 != typeof(string))
					{
						Debug.LogWarning("Json does not support this type");
						return "{}";
					}
					dictionary = (IDictionary)type.GetConstructor(new Type[1] { typeof(int) }).Invoke(new object[1] { jsonObject8.Length });
					for (int k = 0; k < jsonObject8.Length; k++)
					{
						string key = jsonObject8.GetKey(index);
						object value = jsonObject8.Get(key, type4);
						dictionary.Add(key, value);
					}
				}
				else
				{
					dictionary = (IDictionary)type.GetConstructor(new Type[1] { typeof(int) }).Invoke(new object[1] { 0 });
				}
				return dictionary;
			}
			if (type == typeof(JsonArray))
			{
				if (index <= values.Count - 1)
				{
					return Parse(values[index]);
				}
				return new JsonArray();
			}
			if (type == typeof(JsonObject))
			{
				if (index <= values.Count - 1)
				{
					return JsonObject.Parse(values[index]);
				}
				return new JsonObject();
			}
			if (type.IsClass)
			{
				if (index <= values.Count - 1)
				{
					JsonObject jsonObject9 = Get<JsonObject>(index);
					return JsonConvert.Deserialize(type, jsonObject9.ToString());
				}
				return Activator.CreateInstance(type);
			}
			Debug.LogWarning("Json does not support this type");
			return null;
		}

		public static JsonArray Parse(string jsonString)
		{
			return Parser.Parse(jsonString);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('[');
			foreach (string value in values)
			{
				stringBuilder.Append(value);
				stringBuilder.Append(',');
			}
			if (values.Count > 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			stringBuilder.Append(']');
			return stringBuilder.ToString();
		}
	}
}
