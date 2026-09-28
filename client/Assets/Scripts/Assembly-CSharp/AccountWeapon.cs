using System;
using System.Collections.Generic;
using Crypto;

[Serializable]
public class AccountWeapon
{
	public CryptoInt ID = 0;

	public CryptoBool Buy = false;

	public CryptoInt Upgrade = 0;

	public List<CryptoInt> Skins = new List<CryptoInt>();

	public List<CryptoInt> FireStats = new List<CryptoInt>();

	public CryptoInt SelectedSkin = 0;
}
