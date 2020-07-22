using System;
using UnityEngine;
using System.Collections;
using System.IO;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public class I360Render : MonoBehaviour
    {
        public static I360Render Instance;

        // private static Material equirectangularConverter = null;
        private Material equirectangularConverter = null;
        public Material equirectangularConverterExample;

        byte[] image;
        public bool CRrun = true;

        public Borodar.FarlandSkies.CloudyCrownPro.SkyboxController sky;
        void Awake()
        {
            if (Instance == null) Instance = this;
            sky = GameObject.FindGameObjectWithTag("SkyBox").GetComponent<Borodar.FarlandSkies.CloudyCrownPro.SkyboxController>();
        }

        public IEnumerator Capture(int width = 1024, Camera renderCam = null)
        {
            yield return null;
            CRrun = true;

            sky.CloudsRotationSpeed = 0f;
            if (renderCam == null)
            {
                renderCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
                if (renderCam == null)
                {
                    Debug.LogError("Error: no camera detected");
                    //return null;
                }
            }
            Cubemap myCube;

            RenderTexture camTarget = renderCam.targetTexture;

            if (equirectangularConverter == null)
                // equirectangularConverter = new Material(Shader.Find("Hidden/CubemapToEquirectangular"));
                equirectangularConverter = new Material(equirectangularConverterExample);


            RenderTexture cubemap = null, equirectangularTexture = null;
            Texture2D output = null;
            yield return null;
            try
            {


                //myCube = TextureToCubemap.Instance.myCube;
                Debug.Log("myCube");

                myCube = TextureToCubemap.Instance.myCube;
                StartCoroutine(TextureToCubemap.Instance.SetCube());

                StartCoroutine(Cap(width));




            }
            catch (Exception e)
            {

                Debug.LogException(e);

                //return null;
            }
            finally
            {
                renderCam.targetTexture = camTarget;

                if (cubemap != null)
                    RenderTexture.ReleaseTemporary(cubemap);

                if (equirectangularTexture != null)
                    RenderTexture.ReleaseTemporary(equirectangularTexture);

                if (output != null)

                    Destroy(output);
            }
            yield return null;

        }
        IEnumerator Cap(int width)
        {
            while (TextureToCubemap.Instance.CRrun)
            {
                yield return null;
                Debug.Log("Wait in Cap");
            }
            yield return null;
            Debug.Log("set Cube");
            Cubemap myCube = TextureToCubemap.Instance.myCube;
            yield return null;

            int cubemapSize = Mathf.Min(Mathf.NextPowerOfTwo(width), 8192);
            RenderTexture equirectangularTexture = RenderTexture.GetTemporary(cubemapSize, cubemapSize / 2, 0);
            equirectangularTexture.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;

            yield return null;
            Graphics.Blit(myCube, equirectangularTexture, equirectangularConverter);
            Debug.Log("blit");
            yield return null;

            RenderTexture temp = RenderTexture.active;
            RenderTexture.active = equirectangularTexture;
            Texture2D output = new Texture2D(equirectangularTexture.width, equirectangularTexture.height, TextureFormat.RGB24, false);


            output.ReadPixels(new Rect(0, 0, equirectangularTexture.width, equirectangularTexture.height), 0, 0);
            RenderTexture.active = temp;
            yield return null;
            //return null;
            //int timer = 0;
            //int seg = 64;
            //Texture2D newTexture = new Texture2D(equirectangularTexture.width, equirectangularTexture.height);
            //for (int i = 0; i < equirectangularTexture.width; i++)
            //{
            //    if (timer++ > seg)
            //    {
            //        yield return null;
            //        timer = 0;
            //    }
            //    for (int j = 0; j < equirectangularTexture.height; j++)
            //        newTexture.SetPixel(i, equirectangularTexture.height - j - 1, output.GetPixel(i, j));
            //}

            image = InsertXMPIntoTexture2D_JPEG(output);
            yield return null;


            if (equirectangularTexture != null)
                RenderTexture.ReleaseTemporary(equirectangularTexture);

            if (output != null)
                Destroy(output);
            CRrun = false;



        }



        #region JPEG XMP Injection
        private const string XMP_NAMESPACE_JPEG = "http://ns.adobe.com/xap/1.0/";
        private const string XMP_CONTENT_TO_FORMAT_JPEG = "<x:xmpmeta xmlns:x=\"adobe:ns:meta/\" x:xmptk=\"Adobe XMP Core 5.1.0-jc003\"> <rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\"> <rdf:Description rdf:about=\"\" xmlns:GPano=\"http://ns.google.com/photos/1.0/panorama/\" GPano:UsePanoramaViewer=\"True\" GPano:CaptureSoftware=\"Unity3D\" GPano:StitchingSoftware=\"Unity3D\" GPano:ProjectionType=\"equirectangular\" GPano:PoseHeadingDegrees=\"180.0\" GPano:InitialViewHeadingDegrees=\"0.0\" GPano:InitialViewPitchDegrees=\"0.0\" GPano:InitialViewRollDegrees=\"0.0\" GPano:InitialHorizontalFOVDegrees=\"{0}\" GPano:CroppedAreaLeftPixels=\"0\" GPano:CroppedAreaTopPixels=\"0\" GPano:CroppedAreaImageWidthPixels=\"{1}\" GPano:CroppedAreaImageHeightPixels=\"{2}\" GPano:FullPanoWidthPixels=\"{1}\" GPano:FullPanoHeightPixels=\"{2}\"/></rdf:RDF></x:xmpmeta>";


        public static byte[] InsertXMPIntoTexture2D_JPEG(Texture2D image)
        {
            byte[] fileBytes = image.EncodeToJPG(100);

            return DoTheHardWork_JPEG(fileBytes, image.width, image.height);
        }

        private static byte[] DoTheHardWork_JPEG(byte[] fileBytes, int imageWidth, int imageHeight)
        {
            int xmpIndex = 0, xmpContentSize = 0;
            while (!SearchChunkForXMP_JPEG(fileBytes, ref xmpIndex, ref xmpContentSize))
            {
                if (xmpIndex == -1)
                    break;
            }

            int copyBytesUntil, copyBytesFrom;
            if (xmpIndex == -1)
            {
                copyBytesUntil = copyBytesFrom = FindIndexToInsertXMPCode_JPEG(fileBytes);
            }
            else
            {
                copyBytesUntil = xmpIndex;
                copyBytesFrom = xmpIndex + 2 + xmpContentSize;
            }

            string xmpContent = string.Concat(XMP_NAMESPACE_JPEG, "\0", string.Format(XMP_CONTENT_TO_FORMAT_JPEG, 75f.ToString("F1"), imageWidth, imageHeight));
            int xmpLength = xmpContent.Length + 2;
            xmpContent = string.Concat((char)0xFF, (char)0xE1, (char)(xmpLength / 256), (char)(xmpLength % 256), xmpContent);

            byte[] result = new byte[copyBytesUntil + xmpContent.Length + (fileBytes.Length - copyBytesFrom)];

            Array.Copy(fileBytes, 0, result, 0, copyBytesUntil);

            for (int i = 0; i < xmpContent.Length; i++)
            {
                result[copyBytesUntil + i] = (byte)xmpContent[i];
            }

            Array.Copy(fileBytes, copyBytesFrom, result, copyBytesUntil + xmpContent.Length, fileBytes.Length - copyBytesFrom);

            return result;
        }

        private static bool CheckBytesForXMPNamespace_JPEG(byte[] bytes, int startIndex)
        {
            for (int i = 0; i < XMP_NAMESPACE_JPEG.Length; i++)
            {
                if (bytes[startIndex + i] != XMP_NAMESPACE_JPEG[i])
                    return false;
            }

            return true;
        }

        private static bool SearchChunkForXMP_JPEG(byte[] bytes, ref int startIndex, ref int chunkSize)
        {
            if (startIndex + 4 < bytes.Length)
            {
                //Debug.Log( startIndex + " " + System.Convert.ToByte( bytes[startIndex] ).ToString( "x2" ) + " " + System.Convert.ToByte( bytes[startIndex+1] ).ToString( "x2" ) + " " +
                //           System.Convert.ToByte( bytes[startIndex+2] ).ToString( "x2" ) + " " + System.Convert.ToByte( bytes[startIndex+3] ).ToString( "x2" ) );

                if (bytes[startIndex] == 0xFF)
                {
                    byte secondByte = bytes[startIndex + 1];
                    if (secondByte == 0xDA)
                    {
                        startIndex = -1;
                        return false;
                    }
                    else if (secondByte == 0x01 || (secondByte >= 0xD0 && secondByte <= 0xD9))
                    {
                        startIndex += 2;
                        return false;
                    }
                    else
                    {
                        chunkSize = bytes[startIndex + 2] * 256 + bytes[startIndex + 3];

                        if (secondByte == 0xE1 && chunkSize >= 31 && CheckBytesForXMPNamespace_JPEG(bytes, startIndex + 4))
                        {
                            return true;
                        }

                        startIndex = startIndex + 2 + chunkSize;
                    }
                }
            }

            return false;
        }

        private static int FindIndexToInsertXMPCode_JPEG(byte[] bytes)
        {
            int chunkSize = bytes[4] * 256 + bytes[5];
            return chunkSize + 4;
        }
        #endregion

        public IEnumerator ShareScreenshot(string destination)
        {
            //isProcessing = true;

            yield return new WaitForEndOfFrame();

            yield return new WaitForSecondsRealtime(0.3f);

            if (!Application.isEditor)
            {
                AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
                AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
                intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
                AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
                AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + destination);
                intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"),
                    uriObject);
                intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"),
                    "Can you beat my score?");
                intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
                AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser",
                    intentObject, "Share your new score");
                currentActivity.Call("startActivity", chooser);

                yield return new WaitForSecondsRealtime(1);
            }
            Destroy(this.gameObject);


        }


        public IEnumerator SaveShare()
        {
            while (CRrun)
            {
                yield return null;
                Debug.Log("Wait in SaveShare");
            }

            byte[] jpegBytes = image;
            Debug.Log("Capture");
            //yield return new WaitForSeconds(10f);
            yield return null;
            if (jpegBytes != null)
            {
                string pathStart = "/storage/emulated/0/DCIM/Loisboard/";
                if (Directory.Exists(pathStart) == false)
                {
                    Directory.CreateDirectory(pathStart);
                }
                string path = Path.Combine(pathStart, System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_") + "360_capture.jpeg");
                //yield return new WaitForSeconds(0.1f);
                File.WriteAllBytes(path, jpegBytes);
                Debug.Log("FileWrite");
                yield return null;
                Debug.Log("360 render saved to " + path);
                if (!Application.isEditor)
                {
                    //REFRESHING THE ANDROID PHONE PHOTO GALLERY IS BEGUN
                    AndroidJavaClass classPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    AndroidJavaObject objActivity = classPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                    AndroidJavaClass classUri = new AndroidJavaClass("android.net.Uri");
                    AndroidJavaObject objIntent = new AndroidJavaObject("android.content.Intent", new object[2] { "android.intent.action.MEDIA_SCANNER_SCAN_FILE", classUri.CallStatic<AndroidJavaObject>("parse", "file://" + path) });
                    objActivity.Call("sendBroadcast", objIntent);
                    //REFRESHING THE ANDROID PHONE PHOTO GALLERY IS COMPLETE
                }

                GameObject[] allNodes = GameObject.FindGameObjectsWithTag("Node");

                for (int i = 0; i < allNodes.Length; i++)
                {
                    Debug.Log("DOWN");
                    allNodes[i].GetComponentInChildren<NodePannel>().ToBasicTextMode();
                }
                CRrun = true;
                sky.CloudsRotationSpeed = 1f;
                string popupMessage = "EXPORT" + " Success !";
                PopupTextManager.Instance.OnPopupText(popupMessage, 2f);
                if (TutorialManager.Instance.isFirst)
                {
                    yield return new WaitForSeconds(3f);
                    TutorialManager.Instance.DoMission(12);
                }

                if(GvrViewer.Instance.VRModeEnabled) Destroy(this.gameObject);
                else StartCoroutine(I360Render.Instance.ShareScreenshot(path));
            }
        }
    }
}