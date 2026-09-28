using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DevelopersNames : MonoBehaviour
{
	[ContextMenu("Check")]
	private void Check()
	{
		UILabel component = GetComponent<UILabel>();
		List<string> source = component.text.Split("\n"[0]).ToList();
		source = source.Distinct().ToList();
		component.text = string.Join("\n", source.ToArray());
	}
}
