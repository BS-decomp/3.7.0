using System;
using CodeStage.AntiCheat.ObscuredTypes;

[Serializable]
public class StorePlayerSkinData
{
	public ObscuredInt ID;

	public ObscuredString Name;

	public MoneyType PriceType;

	public ObscuredInt Price;
}
