using System.Collections.Generic;
using System.IO;
using ExitGames.Client.Photon;
using UnityEngine;

internal static class CustomTypes
{
	public static readonly byte[] memVector3 = new byte[12];

	public static readonly byte[] memVector2 = new byte[8];

	public static readonly byte[] memQuarternion = new byte[16];

	public static readonly byte[] memPlayer = new byte[4];

	internal static void Register()
	{
		PhotonPeer.RegisterType(typeof(Vector2), 87, SerializeVector2, DeserializeVector2);
		PhotonPeer.RegisterType(typeof(Vector3), 86, SerializeVector3, DeserializeVector3);
		PhotonPeer.RegisterType(typeof(Quaternion), 81, SerializeQuaternion, DeserializeQuaternion);
		PhotonPeer.RegisterType(typeof(PhotonPlayer), 80, SerializePhotonPlayer, DeserializePhotonPlayer);
		PhotonPeer.RegisterType(typeof(DecalInfo), 68, SerializeDecalInfo, DeserializeDecalInfo);
		PhotonPeer.RegisterType(typeof(DamageInfo), 69, SerializeDamageInfo, DeserializeDamageInfo);
	}

	private static short SerializeVector3(MemoryStream outStream, object customobject)
	{
		Vector3 vector = (Vector3)customobject;
		int targetOffset = 0;
		lock (memVector3)
		{
			byte[] array = memVector3;
			Protocol.Serialize(vector.x, array, ref targetOffset);
			Protocol.Serialize(vector.y, array, ref targetOffset);
			Protocol.Serialize(vector.z, array, ref targetOffset);
			outStream.Write(array, 0, 12);
		}
		return 12;
	}

	private static object DeserializeVector3(MemoryStream inStream, short length)
	{
		Vector3 vector = default(Vector3);
		lock (memVector3)
		{
			inStream.Read(memVector3, 0, 12);
			int offset = 0;
			Protocol.Deserialize(out vector.x, memVector3, ref offset);
			Protocol.Deserialize(out vector.y, memVector3, ref offset);
			Protocol.Deserialize(out vector.z, memVector3, ref offset);
		}
		return vector;
	}

	private static short SerializeVector2(MemoryStream outStream, object customobject)
	{
		Vector2 vector = (Vector2)customobject;
		lock (memVector2)
		{
			byte[] array = memVector2;
			int targetOffset = 0;
			Protocol.Serialize(vector.x, array, ref targetOffset);
			Protocol.Serialize(vector.y, array, ref targetOffset);
			outStream.Write(array, 0, 8);
		}
		return 8;
	}

	private static object DeserializeVector2(MemoryStream inStream, short length)
	{
		Vector2 vector = default(Vector2);
		lock (memVector2)
		{
			inStream.Read(memVector2, 0, 8);
			int offset = 0;
			Protocol.Deserialize(out vector.x, memVector2, ref offset);
			Protocol.Deserialize(out vector.y, memVector2, ref offset);
		}
		return vector;
	}

	private static short SerializeQuaternion(MemoryStream outStream, object customobject)
	{
		Quaternion quaternion = (Quaternion)customobject;
		lock (memQuarternion)
		{
			byte[] array = memQuarternion;
			int targetOffset = 0;
			Protocol.Serialize(quaternion.w, array, ref targetOffset);
			Protocol.Serialize(quaternion.x, array, ref targetOffset);
			Protocol.Serialize(quaternion.y, array, ref targetOffset);
			Protocol.Serialize(quaternion.z, array, ref targetOffset);
			outStream.Write(array, 0, 16);
		}
		return 16;
	}

	private static object DeserializeQuaternion(MemoryStream inStream, short length)
	{
		Quaternion quaternion = default(Quaternion);
		lock (memQuarternion)
		{
			inStream.Read(memQuarternion, 0, 16);
			int offset = 0;
			Protocol.Deserialize(out quaternion.w, memQuarternion, ref offset);
			Protocol.Deserialize(out quaternion.x, memQuarternion, ref offset);
			Protocol.Deserialize(out quaternion.y, memQuarternion, ref offset);
			Protocol.Deserialize(out quaternion.z, memQuarternion, ref offset);
		}
		return quaternion;
	}

	private static short SerializePhotonPlayer(MemoryStream outStream, object customobject)
	{
		int iD = ((PhotonPlayer)customobject).ID;
		lock (memPlayer)
		{
			byte[] array = memPlayer;
			int targetOffset = 0;
			Protocol.Serialize(iD, array, ref targetOffset);
			outStream.Write(array, 0, 4);
			return 4;
		}
	}

	private static object DeserializePhotonPlayer(MemoryStream inStream, short length)
	{
		int value;
		lock (memPlayer)
		{
			inStream.Read(memPlayer, 0, length);
			int offset = 0;
			Protocol.Deserialize(out value, memPlayer, ref offset);
		}
		if (PhotonNetwork.networkingPeer.mActors.ContainsKey(value))
		{
			return PhotonNetwork.networkingPeer.mActors[value];
		}
		return null;
	}

	private static byte[] SerializeDecalInfo(object customobject)
	{
		DecalInfo decalInfo = (DecalInfo)customobject;
		byte[] array = new byte[12 + decalInfo.Points.Count * 12 + 4 + decalInfo.Normals.Count * 12];
		int targetOffset = 0;
		Protocol.Serialize(decalInfo.BloodDecal, array, ref targetOffset);
		Protocol.Serialize(decalInfo.isKnife ? 1 : 0, array, ref targetOffset);
		Protocol.Serialize(decalInfo.Points.Count, array, ref targetOffset);
		for (int i = 0; i < decalInfo.Points.Count; i++)
		{
			Protocol.Serialize(decalInfo.Points[i].x, array, ref targetOffset);
			Protocol.Serialize(decalInfo.Points[i].y, array, ref targetOffset);
			Protocol.Serialize(decalInfo.Points[i].z, array, ref targetOffset);
		}
		Protocol.Serialize(decalInfo.Normals.Count, array, ref targetOffset);
		for (int j = 0; j < decalInfo.Normals.Count; j++)
		{
			Protocol.Serialize(decalInfo.Normals[j].x, array, ref targetOffset);
			Protocol.Serialize(decalInfo.Normals[j].y, array, ref targetOffset);
			Protocol.Serialize(decalInfo.Normals[j].z, array, ref targetOffset);
		}
		return array;
	}

	private static object DeserializeDecalInfo(byte[] bytes)
	{
		DecalInfo decalInfo = new DecalInfo();
		int offset = 0;
		Protocol.Deserialize(out decalInfo.BloodDecal, bytes, ref offset);
		int value = 0;
		Protocol.Deserialize(out value, bytes, ref offset);
		decalInfo.isKnife = value == 1;
		int value2 = 0;
		Protocol.Deserialize(out value2, bytes, ref offset);
		for (int i = 0; i < value2; i++)
		{
			Vector3 zero = Vector3.zero;
			Protocol.Deserialize(out zero.x, bytes, ref offset);
			Protocol.Deserialize(out zero.y, bytes, ref offset);
			Protocol.Deserialize(out zero.z, bytes, ref offset);
			decalInfo.Points.Add(zero);
		}
		int value3 = 0;
		Protocol.Deserialize(out value3, bytes, ref offset);
		for (int j = 0; j < value3; j++)
		{
			Vector3 zero2 = Vector3.zero;
			Protocol.Deserialize(out zero2.x, bytes, ref offset);
			Protocol.Deserialize(out zero2.y, bytes, ref offset);
			Protocol.Deserialize(out zero2.z, bytes, ref offset);
			decalInfo.Normals.Add(zero2);
		}
		return decalInfo;
	}

	private static byte[] SerializeDamageInfo(object customobject)
	{
		DamageInfo damageInfo = (DamageInfo)customobject;
		byte[] array = new byte[36];
		int targetOffset = 0;
		Protocol.Serialize(damageInfo.Damage, array, ref targetOffset);
		Protocol.Serialize(damageInfo.AttackPosition.x, array, ref targetOffset);
		Protocol.Serialize(damageInfo.AttackPosition.y, array, ref targetOffset);
		Protocol.Serialize(damageInfo.AttackPosition.z, array, ref targetOffset);
		Protocol.Serialize((int)damageInfo.AttackerTeam, array, ref targetOffset);
		Protocol.Serialize(damageInfo.WeaponID, array, ref targetOffset);
		Protocol.Serialize(damageInfo.WeaponSkinID, array, ref targetOffset);
		Protocol.Serialize(damageInfo.PlayerID, array, ref targetOffset);
		Protocol.Serialize(damageInfo.HeadShot ? 1 : 0, array, ref targetOffset);
		return array;
	}

	private static object DeserializeDamageInfo(byte[] bytes)
	{
		DamageInfo damageInfo = new DamageInfo();
		int offset = 0;
		Protocol.Deserialize(out damageInfo.Damage, bytes, ref offset);
		Protocol.Deserialize(out damageInfo.AttackPosition.x, bytes, ref offset);
		Protocol.Deserialize(out damageInfo.AttackPosition.y, bytes, ref offset);
		Protocol.Deserialize(out damageInfo.AttackPosition.z, bytes, ref offset);
		int value = 0;
		Protocol.Deserialize(out value, bytes, ref offset);
		damageInfo.AttackerTeam = (Team)value;
		Protocol.Deserialize(out damageInfo.WeaponID, bytes, ref offset);
		Protocol.Deserialize(out damageInfo.WeaponSkinID, bytes, ref offset);
		Protocol.Deserialize(out damageInfo.PlayerID, bytes, ref offset);
		int value2 = 0;
		Protocol.Deserialize(out value2, bytes, ref offset);
		damageInfo.HeadShot = value2 == 1;
		return damageInfo;
	}

	private static byte[] SerializeListVector3(object customobject)
	{
		List<Vector3> list = (List<Vector3>)customobject;
		byte[] array = new byte[3 * list.Count * 4 + 4];
		int targetOffset = 0;
		Protocol.Serialize(list.Count, array, ref targetOffset);
		for (int i = 0; i < list.Count; i++)
		{
			Protocol.Serialize(list[i].x, array, ref targetOffset);
			Protocol.Serialize(list[i].y, array, ref targetOffset);
			Protocol.Serialize(list[i].z, array, ref targetOffset);
		}
		return array;
	}

	private static object DeserializeListVector3(byte[] bytes)
	{
		List<Vector3> result = new List<Vector3>();
		int offset = 0;
		int value = 0;
		Protocol.Deserialize(out value, bytes, ref offset);
		for (int i = 0; i < value; i++)
		{
			Vector3 vector = default(Vector3);
			Protocol.Deserialize(out vector.x, bytes, ref offset);
			Protocol.Deserialize(out vector.y, bytes, ref offset);
			Protocol.Deserialize(out vector.z, bytes, ref offset);
		}
		return result;
	}
}
