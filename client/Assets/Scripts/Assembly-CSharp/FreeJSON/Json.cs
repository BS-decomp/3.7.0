using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeJSON
{
	public class Json
	{
		public static string ConvertType(object value)
		{
			if (value == null)
			{
				return "null";
			}
			if (value is string)
			{
				return "\"" + value.ToString() + "\"";
			}
			if (value is int)
			{
				return value.ToString();
			}
			if (value is float)
			{
				return value.ToString();
			}
			if (value is bool)
			{
				return value.ToString().ToLower();
			}
			if (value is byte)
			{
				return value.ToString();
			}
			if (value is short)
			{
				return value.ToString();
			}
			if (value is long)
			{
				return value.ToString();
			}
			if (value is char)
			{
				return value.ToString();
			}
			if (value is double)
			{
				return value.ToString();
			}
			if (value is Enum)
			{
				return value.ToString();
			}
			if (value is Vector2)
			{
				JsonObject jsonObject = new JsonObject();
				Vector2 vector = (Vector2)value;
				jsonObject.Add("x", vector.x);
				jsonObject.Add("y", vector.y);
				return jsonObject.ToString();
			}
			if (value is Vector3)
			{
				JsonObject jsonObject2 = new JsonObject();
				Vector3 vector2 = (Vector3)value;
				jsonObject2.Add("x", vector2.x);
				jsonObject2.Add("y", vector2.y);
				jsonObject2.Add("z", vector2.z);
				return jsonObject2.ToString();
			}
			if (value is Vector4)
			{
				JsonObject jsonObject3 = new JsonObject();
				Vector4 vector3 = (Vector4)value;
				jsonObject3.Add("x", vector3.x);
				jsonObject3.Add("y", vector3.y);
				jsonObject3.Add("z", vector3.z);
				jsonObject3.Add("w", vector3.w);
				return jsonObject3.ToString();
			}
			if (value is Quaternion)
			{
				JsonObject jsonObject4 = new JsonObject();
				Quaternion quaternion = (Quaternion)value;
				jsonObject4.Add("x", quaternion.x);
				jsonObject4.Add("y", quaternion.y);
				jsonObject4.Add("z", quaternion.z);
				jsonObject4.Add("w", quaternion.w);
				return jsonObject4.ToString();
			}
			if (value is Color)
			{
				JsonObject jsonObject5 = new JsonObject();
				Color color = (Color)value;
				jsonObject5.Add("r", color.r);
				jsonObject5.Add("g", color.g);
				jsonObject5.Add("b", color.b);
				jsonObject5.Add("a", color.a);
				return jsonObject5.ToString();
			}
			if (value is Color32)
			{
				JsonObject jsonObject6 = new JsonObject();
				Color32 color2 = (Color32)value;
				jsonObject6.Add("r", color2.r);
				jsonObject6.Add("g", color2.g);
				jsonObject6.Add("b", color2.b);
				jsonObject6.Add("a", color2.a);
				return jsonObject6.ToString();
			}
			if (value is Rect)
			{
				JsonObject jsonObject7 = new JsonObject();
				Rect rect = (Rect)value;
				jsonObject7.Add("x", rect.x);
				jsonObject7.Add("y", rect.y);
				jsonObject7.Add("width", rect.width);
				jsonObject7.Add("height", rect.height);
				return jsonObject7.ToString();
			}
			if (value is IList)
			{
				JsonArray jsonArray = new JsonArray();
				IList list = value as IList;
				for (int i = 0; i < list.Count; i++)
				{
					jsonArray.Add(list[i]);
				}
				return jsonArray.ToString();
			}
			if (value.GetType().IsGenericType && value.GetType().GetGenericTypeDefinition() == typeof(Dictionary<, >))
			{
				if (value.GetType().GetGenericArguments()[0] != typeof(string))
				{
					Debug.LogWarning("Json does not support this type");
					return "{}";
				}
				JsonObject jsonObject8 = new JsonObject();
				IDictionary dictionary = value as IDictionary;
				foreach (object key in dictionary.Keys)
				{
					jsonObject8.Add((string)key, dictionary[key]);
				}
				return jsonObject8.ToString();
			}
			if (value is JsonArray)
			{
				return value.ToString();
			}
			if (value is JsonObject)
			{
				return value.ToString();
			}
			if (value.GetType().IsClass)
			{
				return JsonConvert.Serialize(value);
			}
			return string.Empty;
		}
	}
}
