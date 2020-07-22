using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeScrollEffect : BaseScrollEffect
{
    [Range(0.0f, 1.0f)]
    [Tooltip("The alpha of the page when it is one page-length away.")]
    public float minAlpha = 0.0f;

    public override void ApplyEffect(BaseScrollEffect.UpdateData updateData)
    {
		Renderer pageRenderer = updateData.page.GetComponentInChildren<Renderer>();
        CanvasGroup textCanvas = updateData.page.GetComponentInChildren<CanvasGroup>();

        /// All pages require a CanvasGroup for manipulating Alpha.
        if (pageRenderer == null)
        {
            Debug.LogError("Cannot adjust alpha for page " + updateData.page.name + ", missing Renderer");
            return;
        }

        // Calculate the difference
        float difference = updateData.scrollOffset - updateData.pageOffset;

        /// Calculate the alpha for this page.
        float alpha = 1.0f - (Mathf.Abs(difference) / updateData.spacing);
        alpha = (alpha * (1.0f - minAlpha)) + minAlpha;
        alpha = Mathf.Clamp(alpha, 0.0f, 1.0f);

        /// If this is the last page or the first page,
        /// Then we clamp the alpha to 1 when dragging past the edge
        /// Of the scrolling region.
        if (!updateData.looping)
        {
            if (updateData.pageIndex == 0 && difference < 0)
            {
                alpha = 1.0f;
            }
            else if (updateData.pageIndex == updateData.pageCount - 1 && difference > 0)
            {
                alpha = 1.0f;
            }
        }

		pageRenderer.material.color = new Color(1f, 1f, 1f, alpha);
        textCanvas.alpha = alpha;
    }
}
