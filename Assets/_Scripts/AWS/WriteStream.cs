
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
public class WriteStream : MonoBehaviour
{
    
    public float frameExportInterval;
    public bool sendToAWS;
    Texture2D tex;
    Texture2D croppedTex;
    Camera arCam;
    Rect screenRect;

    private byte[] frameBytes;
    FramePackage dataToStream;
    string JSONdataToStream;

    bool isRunning = false;
    WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();
    WaitForSeconds exportInterval;
    private Coroutine co;

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
        arCam = this.GetComponent<Camera>();
        frameBytes = new byte[5];
        tex = new Texture2D(arCam.pixelWidth, arCam.pixelHeight);
        screenRect = new Rect(0, 0, arCam.pixelWidth, arCam.pixelHeight);
        exportInterval = new WaitForSeconds(2.0f);
    }


    public IEnumerator ExportFrame()
    {
        while(true){
                
            if(sendToAWS && !isRunning){
                isRunning = true;
                yield return frameEnd;

                tex.Resize(arCam.pixelWidth, arCam.pixelHeight);
                tex.ReadPixels(screenRect, 0, 0);
                tex.Apply();

                TextureScale.Bilinear(tex, tex.width / 2, tex.height / 2);
                croppedTex = TextureTools.ResampleAndCrop(tex, tex.width, tex.height / 2 + 100);
                //frameBytes = croppedTex.EncodeToJPG();

                dataToStream = new FramePackage(System.DateTime.UtcNow, Time.frameCount, frameBytes);

                JSONdataToStream = dataToStream.serialize();

                //Debug.Log("Sending image to Kinesis >>" + Time.frameCount );
                _C.PutRecord(JSONdataToStream, "FrameStream", (response) => { });


                frameBytes = null;
                JSONdataToStream = null;
                isRunning = false;

            }
            yield return exportInterval;
        }

    }


    public void activate(){
        sendToAWS = true;
        co = StartCoroutine(ExportFrame());
    }


    public void deactivate(){
        sendToAWS = false;
        StopCoroutine(co);
        Resources.UnloadUnusedAssets();
    }

}
