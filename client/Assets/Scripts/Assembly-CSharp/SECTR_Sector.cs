using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("SECTR/Core/SECTR Sector")]
[ExecuteInEditMode]
public class SECTR_Sector : SECTR_Member
{
	private List<SECTR_Portal> portals = new List<SECTR_Portal>(8);

	private List<SECTR_Member> members = new List<SECTR_Member>(32);

	private bool visited;

	private static List<SECTR_Sector> allSectors = new List<SECTR_Sector>(128);

	[SECTR_ToolTip("The terrain Sector attached on the top side of this Sector.")]
	public SECTR_Sector TopTerrain;

	[SECTR_ToolTip("The terrain Sector attached on the bottom side of this Sector.")]
	public SECTR_Sector BottomTerrain;

	[SECTR_ToolTip("The terrain Sector attached on the left side of this Sector.")]
	public SECTR_Sector LeftTerrain;

	[SECTR_ToolTip("The terrain Sector attached on the right side of this Sector.")]
	public SECTR_Sector RightTerrain;

	public new static List<SECTR_Sector> All
	{
		get
		{
			return allSectors;
		}
	}

	public bool Visited
	{
		get
		{
			return visited;
		}
		set
		{
			visited = value;
		}
	}

	public List<SECTR_Portal> Portals
	{
		get
		{
			return portals;
		}
	}

	public List<SECTR_Member> Members
	{
		get
		{
			return members;
		}
	}

	public bool IsConnectedTerrain
	{
		get
		{
			return (bool)LeftTerrain || (bool)RightTerrain || (bool)TopTerrain || (bool)BottomTerrain;
		}
	}

	private SECTR_Sector()
	{
		isSector = true;
	}

	public static void GetContaining(ref List<SECTR_Sector> sectors, Vector3 position)
	{
		sectors.Clear();
		int count = allSectors.Count;
		for (int i = 0; i < count; i++)
		{
			SECTR_Sector sECTR_Sector = allSectors[i];
			if (sECTR_Sector.TotalBounds.Contains(position))
			{
				sectors.Add(sECTR_Sector);
			}
		}
	}

	public static void GetContaining(ref List<SECTR_Sector> sectors, Bounds bounds)
	{
		sectors.Clear();
		int count = allSectors.Count;
		for (int i = 0; i < count; i++)
		{
			SECTR_Sector sECTR_Sector = allSectors[i];
			if (sECTR_Sector.TotalBounds.Intersects(bounds))
			{
				sectors.Add(sECTR_Sector);
			}
		}
	}

	public void ConnectTerrainNeighbors()
	{
		Terrain componentInChildren = GetComponentInChildren<Terrain>();
		if ((bool)componentInChildren)
		{
			componentInChildren.SetNeighbors((!LeftTerrain) ? null : LeftTerrain.GetComponentInChildren<Terrain>(), (!TopTerrain) ? null : TopTerrain.GetComponentInChildren<Terrain>(), (!RightTerrain) ? null : RightTerrain.GetComponentInChildren<Terrain>(), (!BottomTerrain) ? null : BottomTerrain.GetComponentInChildren<Terrain>());
		}
	}

	public void DisonnectTerrainNeighbors()
	{
		Terrain componentInChildren = GetComponentInChildren<Terrain>();
		if ((bool)componentInChildren)
		{
			componentInChildren.SetNeighbors(null, null, null, null);
		}
		if ((bool)TopTerrain)
		{
			Terrain componentInChildren2 = TopTerrain.GetComponentInChildren<Terrain>();
			if ((bool)componentInChildren2)
			{
				componentInChildren2.SetNeighbors((!TopTerrain.LeftTerrain) ? null : TopTerrain.LeftTerrain.GetComponentInChildren<Terrain>(), (!TopTerrain.TopTerrain) ? null : TopTerrain.TopTerrain.GetComponentInChildren<Terrain>(), (!TopTerrain.RightTerrain) ? null : TopTerrain.RightTerrain.GetComponentInChildren<Terrain>(), null);
			}
		}
		if ((bool)BottomTerrain)
		{
			Terrain componentInChildren3 = BottomTerrain.GetComponentInChildren<Terrain>();
			if ((bool)componentInChildren3)
			{
				componentInChildren3.SetNeighbors((!BottomTerrain.LeftTerrain) ? null : BottomTerrain.LeftTerrain.GetComponentInChildren<Terrain>(), null, (!BottomTerrain.RightTerrain) ? null : BottomTerrain.RightTerrain.GetComponentInChildren<Terrain>(), (!BottomTerrain.BottomTerrain) ? null : BottomTerrain.BottomTerrain.GetComponentInChildren<Terrain>());
			}
		}
		if ((bool)LeftTerrain)
		{
			Terrain componentInChildren4 = LeftTerrain.GetComponentInChildren<Terrain>();
			if ((bool)componentInChildren4)
			{
				componentInChildren4.SetNeighbors((!LeftTerrain.LeftTerrain) ? null : LeftTerrain.LeftTerrain.GetComponentInChildren<Terrain>(), (!LeftTerrain.TopTerrain) ? null : LeftTerrain.TopTerrain.GetComponentInChildren<Terrain>(), null, (!LeftTerrain.BottomTerrain) ? null : LeftTerrain.BottomTerrain.GetComponentInChildren<Terrain>());
			}
		}
		if ((bool)RightTerrain)
		{
			Terrain componentInChildren5 = RightTerrain.GetComponentInChildren<Terrain>();
			if ((bool)componentInChildren5)
			{
				componentInChildren5.SetNeighbors(null, (!RightTerrain.TopTerrain) ? null : RightTerrain.TopTerrain.GetComponentInChildren<Terrain>(), (!RightTerrain.RightTerrain) ? null : RightTerrain.RightTerrain.GetComponentInChildren<Terrain>(), (!RightTerrain.BottomTerrain) ? null : RightTerrain.BottomTerrain.GetComponentInChildren<Terrain>());
			}
		}
	}

	public void Register(SECTR_Portal portal)
	{
		if (!portals.Contains(portal))
		{
			portals.Add(portal);
		}
	}

	public void Deregister(SECTR_Portal portal)
	{
		portals.Remove(portal);
	}

	public void Register(SECTR_Member member)
	{
		members.Add(member);
	}

	public void Deregister(SECTR_Member member)
	{
		members.Remove(member);
	}

	protected override void OnEnable()
	{
		allSectors.Add(this);
		if ((bool)TopTerrain || (bool)BottomTerrain || (bool)RightTerrain || (bool)LeftTerrain)
		{
			ConnectTerrainNeighbors();
		}
		base.OnEnable();
	}

	protected override void OnDisable()
	{
		List<SECTR_Member> list = new List<SECTR_Member>(members);
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			SECTR_Member sECTR_Member = list[i];
			if ((bool)sECTR_Member)
			{
				sECTR_Member.SectorDisabled(this);
			}
		}
		allSectors.Remove(this);
		base.OnDisable();
	}
}
