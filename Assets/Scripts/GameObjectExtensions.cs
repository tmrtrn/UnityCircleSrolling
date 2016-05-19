using UnityEngine;
using System.Linq;
public static class GameObjectExtensions
{
	public static void SortChildren(this GameObject gameObject)
	{
		var children = gameObject.GetComponentsInChildren<Transform>(true);
		var sorted = from child in children
			orderby child.gameObject.activeInHierarchy descending, child.localPosition.z descending
				where child != gameObject.transform
				select child;
		for (int i = 0; i < sorted.Count(); i++)
		{
			sorted.ElementAt(i).SetSiblingIndex(i);
		}
	}
}
