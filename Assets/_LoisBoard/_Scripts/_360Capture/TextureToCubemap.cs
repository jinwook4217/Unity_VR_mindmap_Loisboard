using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureToCubemap : MonoBehaviour
{
    public Cubemap myCube;
    private int size;
    public static TextureToCubemap Instance;
    private Color[] CubeMapColors;
    public Texture2D t;
    public bool CRrun;
    public Camera cam;

    public int crnum = 0;


    void Start()
    {


        if (Instance == null) Instance = this;
        size = cam.targetTexture.width;
        myCube = new Cubemap(size, TextureFormat.RGBA32, false);

    }
    public IEnumerator SetCube()

    {
        //for (int i = 0; i < cams.Length; i++) cams[i].gameObject.SetActive(true);
        CRrun = true;
        //int camNumber = 0;

        for (int i = 0; i < 6; i++)
        {

            StartCoroutine(Progress(i, cam, (CubemapFace)i));
            yield return null;
        }

        StartCoroutine(Last());

    }


    IEnumerator Progress(int num, Camera cam, CubemapFace face)
    {

        yield return null;
        while (crnum != num)
        {
            yield return null;
            Debug.Log("Wait Progress");
        }
        switch (num)
        {
            case 0:
                break;
            case 1:
                this.transform.Rotate(0f, 180f, 0f);

                break;
            case 2:
                this.transform.Rotate(0f, 90f, 0f);
                this.transform.Rotate(0f, 0f, -90f);

                break;
            case 3:
                this.transform.Rotate(0f, 180f, 0f);
                this.transform.Rotate(180f, 0f, 0f);

                break;
            case 4:
                this.transform.Rotate(0f, 0f, -90f);

                break;
            case 5:
                this.transform.Rotate(0f, 180f, 0f);

                break;

            default:
                break;
        }
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = cam.targetTexture;
        cam.Render();
        //Debug.Log(cam.targetTexture);
        Texture2D image = new Texture2D(cam.targetTexture.width, cam.targetTexture.height);

        image.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
        yield return null;
        image.Apply();
        yield return null;
        RenderTexture.active = currentRT;
        t = image;
        yield return null;




        myCube.SetPixels(t.GetPixels(), face);
        Debug.Log(face);
        //estroy(cams[crnum].gameObject);
        crnum++;
        Debug.Log(crnum);


    }
    IEnumerator Last()
    {
        while (crnum != 6)
        {
            yield return null;
            Debug.Log("Wait Last");
        }
        myCube.Apply();
        Debug.Log("Apply");
        yield return null;
        CRrun = false;
        crnum = 0;
        Destroy(cam.gameObject);
        //for (int i = 0; i < cams.Length; i++) cams[i].gameObject.SetActive(false);
    }


}
