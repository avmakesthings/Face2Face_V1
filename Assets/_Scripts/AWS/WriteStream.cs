
//#define DEBUG_Webcam
#define AR_camera

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.iOS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Utils;


[RequireComponent(typeof(AWSClient))]
[RequireComponent(typeof(AWSConfig))]
public class WriteStream : MonoBehaviour
{

    #region Webcam_fields
#if DEBUG_Webcam
    private bool camAvailable;
    private bool isInitialized = false;
    private WebCamTexture webCam;
    private Texture defaultBackground;

    Color32[] data;

    public RawImage background;
    public AspectRatioFitter fit;
#endif
    #endregion


    public int captureRate;
    public bool sendToAWS;
    private int frames = 0;
    private RenderTexture arCamTexture;
    Texture2D tex;


    WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();

    static private AWSClient _C;

    [System.Serializable]
    struct FramePackage
    {
        public System.DateTime ApproximateCaptureTime;
        public int FrameCount;
        public byte[] ImageBytes;


        public FramePackage(System.DateTime time, int count, byte[] data)
        {
            this.ApproximateCaptureTime = time;
            this.FrameCount = count;
            this.ImageBytes = data;
        }

        public string serialize(){
            return JsonConvert.SerializeObject(this);
        }

    }


    // Use this for initialization
    void Start()
    {
        _C = GetComponent<AWSClient>();
        Camera arCam = this.GetComponent<Camera>();
        arCamTexture = new RenderTexture(arCam.pixelWidth, arCam.pixelHeight, 16, RenderTextureFormat.ARGB32);
        arCamTexture.Create();
        arCam.targetTexture = arCamTexture;

        tex = new Texture2D(arCamTexture.width, arCamTexture.height);


    #region Webcam
#if DEBUG_Webcam
        defaultBackground = background.texture;
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0){
            Debug.Log("No camera detected");
            return;
        }

        for (int i = 0; i < devices.Length; i++){
            if(devices[i].isFrontFacing){
                webCam = new WebCamTexture(devices[i].name, Screen.width/100, Screen.height/100);
            } 
        }

        if(webCam == null){
            Debug.Log("Unable to find webcam");
            return;
        }

        webCam.Play();
        background.texture = webCam;
        camAvailable = true;
#endif
    #endregion
    
    }


    void Update()
    {
        frames++;

        #region webcam
#if DEBUG_Webcam

        Texture2D t = new Texture2D(1280, 720);

        if (!camAvailable)
        {
            return;
        }

        if (!isInitialized && webCam.didUpdateThisFrame) {

            data = new Color32[webCam.width * webCam.height];
            print(webCam.width + "height" + webCam.height);
            isInitialized = true;
        }
        else if (isInitialized && webCam.didUpdateThisFrame && frames % captureRate == 0){
            
            webCam.GetPixels32(data);
            t.SetPixels32(data);
            t.Resize(128, 72);
            t.Apply();
            byte[] b = Color32ArrayToByteArray(t.GetPixels32());
            
            //File.WriteAllBytes(Application.dataPath + "/StreamingAssets/testbytefile.txt", b );

#endif
        #endregion

        if(frames % captureRate == 0 && sendToAWS ){
            StartCoroutine(ExportFrame());
        }
    }


    public IEnumerator ExportFrame()
    {
        yield return frameEnd;
        tex.Resize(arCamTexture.width, arCamTexture.height);
        tex.ReadPixels(new Rect(0, 0, arCamTexture.width, arCamTexture.height), 0, 0);
        tex.Apply();

        TextureScale.Bilinear(tex, tex.width / 2, tex.height / 2);
        Texture2D croppedTex = TextureTools.ResampleAndCrop(tex,tex.width,tex.height/2+100);
        byte[] frameBytes = croppedTex.EncodeToJPG();

        //Debug to write texture into PNG
        //File.WriteAllBytes(Application.dataPath + System.String.Format( "/../SavedScreen{0}.jpg", frames), frameBytes);

        FramePackage dataToStream = new FramePackage(System.DateTime.UtcNow, frames, frameBytes);
        string JSONdataToStream = dataToStream.serialize();


        Debug.Log("Sending image to Kinesis");
        _C.PutRecord(JSONdataToStream, "FrameStream", (response) => { }); 

    }

    public void activate(){
        sendToAWS = true;
    }

    public void deactive(){
        sendToAWS = false;
    }

}
