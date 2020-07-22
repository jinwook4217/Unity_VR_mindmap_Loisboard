using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleScrollEffect : BaseScrollEffect
{
	[Range(0.0f, 1.0f)]
	[Tooltip("The scale of the page when it is one page-length away.")]
	public float minScale;

	public override void ApplyEffect(BaseScrollEffect.UpdateData updateData) 
	{
    // Calculate the difference.
    float difference = updateData.scrollOffset - updateData.pageOffset;

    // Calculate the scale for this page.
    float ratioScrolled = Mathf.Abs(difference) / updateData.spacing;
    float scale = ((1.0f - ratioScrolled) * (1.0f - minScale)) + minScale;
    scale = Mathf.Clamp(scale, 0.0f, 1.0f);

    // Update the scale.
    updateData.page.localScale = new Vector3(scale, scale, scale);
  }
}
