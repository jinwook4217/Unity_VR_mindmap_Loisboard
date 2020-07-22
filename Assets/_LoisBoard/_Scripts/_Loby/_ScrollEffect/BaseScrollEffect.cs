using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseScrollEffect : MonoBehaviour
{
	public struct UpdateData
	{
		public Transform page;
		public int pageIndex;
		public int pageCount;
		public float pageOffset;
		public float scrollOffset;
		public float spacing;
		public bool looping;
		public bool isInteractable;
	}

	public abstract void ApplyEffect(UpdateData updateData);
}
