using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateScrollEffect : BaseScrollEffect
{
    [Tooltip("Determines the percentage of the page's offset that is applied to each axis.")]
    public Vector3 Weights = new Vector3(1.0f, 0.0f, 0.0f);

    [Tooltip("Determines if the absolute offset will be used for the X axis.")]
    public bool mirrorX;

    [Tooltip("Determines if the absolute offset will be used for the Y axis.")]
    public bool mirrorY;

    [Tooltip("Determines if the absolute offset will be used for the Z axis.")]
    public bool mirrorZ;

    public override void ApplyEffect(BaseScrollEffect.UpdateData updateData)
    {
        float distance = updateData.pageOffset - updateData.scrollOffset;
        float absDistance = Mathf.Abs(distance);
        updateData.page.localPosition = new Vector3(
          (mirrorX ? absDistance : distance) * Weights.x,
          (mirrorY ? absDistance : distance) * Weights.y,
          (mirrorZ ? absDistance : distance) * Weights.z);
    }
}
