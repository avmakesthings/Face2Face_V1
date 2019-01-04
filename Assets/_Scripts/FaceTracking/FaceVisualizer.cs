using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class FaceVisualizer : MonoBehaviour {

	[SerializeField]
	private MeshFilter meshFilter;
	private Mesh faceDebugMesh;
	private UnityARSessionNativeInterface m_session;

    private Bounds meshbounds;
    private Bounds rendBounds;
	private Vector3 basePos;
    private Vector2 screenBasePos;
	private GameObject bBox;
    private bool FaceActive = false;

	public Material faceBoxMaterial;
    public Texture2D crosshair;
    public Rect position;

    [SerializeField]
    public UnityEvent faceAdded;
    public UnityEvent faceRemoved;
    public Text nameText;

	// Use this for initialization
	void Start () {


		m_session = UnityARSessionNativeInterface.GetARSessionNativeInterface();

		Application.targetFrameRate = 60;
		ARKitFaceTrackingConfiguration config = new ARKitFaceTrackingConfiguration();
		config.alignment = UnityARAlignment.UnityARAlignmentGravity;
		config.enableLightEstimation = true;

		if (config.IsSupported) {
			
			m_session.RunWithConfig (config);

			UnityARSessionNativeInterface.ARFaceAnchorAddedEvent += FaceAdded;
			UnityARSessionNativeInterface.ARFaceAnchorUpdatedEvent += FaceUpdated;
			UnityARSessionNativeInterface.ARFaceAnchorRemovedEvent += FaceRemoved;
		}
	}


    void Update()
    {
        
        //if(FaceActive){
        //    screenBasePos = Camera.main.WorldToScreenPoint(basePos);
        //    position = new Rect(screenBasePos.x, screenBasePos.y, 400,400); 
        //}
        //else{
        //    position = new Rect((Screen.width - crosshair.width) / 2, (Screen.height - crosshair.height) / 2, crosshair.width, crosshair.height); 
        //}
    }



	void FaceAdded (ARFaceAnchor anchorData)
	{
		gameObject.transform.localPosition = UnityARMatrixOps.GetPosition (anchorData.transform);
		gameObject.transform.localRotation = UnityARMatrixOps.GetRotation (anchorData.transform);
        //faceAdded.Invoke();

	}



	void FaceUpdated (ARFaceAnchor anchorData)
	{
        if (anchorData.isTracked)
        {
            if (!FaceActive)
            {
                FaceActive = true;
                faceAdded.Invoke();
                Debug.Log("face added");
                //FaceAdded(anchorData);
            }

            gameObject.transform.localPosition = UnityARMatrixOps.GetPosition (anchorData.transform);
    		gameObject.transform.localRotation = UnityARMatrixOps.GetRotation (anchorData.transform);

    		updateDebugFaceMesh(anchorData);



        }
        if(!anchorData.isTracked)
        {
            
            if(FaceActive){
                FaceActive = false;
                Debug.Log("face removed");
                faceRemoved.Invoke();
                //StartCoroutine(invokeEventWithDelay(faceRemoved, 3));
                //FaceRemoved(anchorData);
            }
        }
	}


	void FaceRemoved (ARFaceAnchor anchorData)
	{
        
        meshFilter.mesh = null;
		faceDebugMesh = null;
	}	



	void createDebugFaceMesh(ARFaceAnchor anchorData){
		
        faceDebugMesh = new Mesh ();
		meshFilter.mesh = faceDebugMesh;
		drawBBox();  // this is for the 3d bounding box
		updateDebugFaceMesh(anchorData);
        meshbounds = faceDebugMesh.bounds;
        rendBounds = this.GetComponent<MeshRenderer>().bounds;
	}



	void updateDebugFaceMesh(ARFaceAnchor anchorData){

		if(faceDebugMesh == null){
			createDebugFaceMesh(anchorData);
		}
		faceDebugMesh.vertices = anchorData.faceGeometry.vertices;
		faceDebugMesh.uv = anchorData.faceGeometry.textureCoordinates;
		faceDebugMesh.triangles = anchorData.faceGeometry.triangleIndices;
		faceDebugMesh.RecalculateBounds();
		faceDebugMesh.RecalculateNormals();

	}



	void drawBBox(){
	    Bounds bounds = this.GetComponent<MeshRenderer>().bounds;
		basePos = bounds.center;
		bBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
		bBox.transform.parent = transform;
		bBox.transform.position = basePos;
		bBox.transform.localScale = new Vector3(0.2f, 0.22f, 0f);
		bBox.GetComponent<Renderer>().material = faceBoxMaterial;
	}



    void OnGUI()
    {
        //GUI.DrawTexture(position, crosshair);
    }
     
    IEnumerator invokeEventWithDelay(UnityEvent myEvent, int delay){
        yield return new WaitForSeconds(delay);
        myEvent.Invoke();
    }




}
