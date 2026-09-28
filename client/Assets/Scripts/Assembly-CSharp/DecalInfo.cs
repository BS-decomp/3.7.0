using System.Collections.Generic;
using UnityEngine;

public class DecalInfo
{
	public int BloodDecal = -1;

	public bool isKnife;

	public List<Vector3> Points = new List<Vector3>();

	public List<Vector3> Normals = new List<Vector3>();

	public static DecalInfo Create(int bloodDecal, List<Vector3> points, List<Vector3> normals)
	{
		DecalInfo decalInfo = new DecalInfo();
		decalInfo.BloodDecal = bloodDecal;
		decalInfo.Points = points;
		decalInfo.Normals = normals;
		return decalInfo;
	}
}
