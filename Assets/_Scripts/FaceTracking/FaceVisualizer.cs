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

	private Vector3 basePos;
    private Vector2 screenBasePos;
	private GameObject bBox;
    private bool FaceActive = false;

	public Material faceBoxMaterial;


    [SerializeField]
    public UnityEvent faceAdded;
    public UnityEvent faceRemoved;


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


	void FaceAdded (ARFaceAnchor anchorData)
	{
        Debug.Log("face added");
        gameObject.transform.localPosition = UnityARMatrixOps.GetPosition (anchorData.transform);
		gameObject.transform.localRotation = UnityARMatrixOps.GetRotation (anchorData.transform);
	}



	void FaceUpdated (ARFaceAnchor anchorData)
	{
        if(anchorData.isTracked)
        {

            if (!FaceActive)
            {
                Debug.Log("updated - anchor data is tracked and not faceActive");
                FaceActive = true;
                faceAdded.Invoke();
            }

            gameObject.transform.localPosition = UnityARMatrixOps.GetPosition (anchorData.transform);
    		gameObject.transform.localRotation = UnityARMatrixOps.GetRotation (anchorData.transform);
    		updateDebugFaceMesh(anchorData);

        }else{
            if (FaceActive)
            {
                Debug.Log("updated - anchor data not tracked and faceActive");
                FaceActive = false;
                faceRemoved.Invoke();
            } 
        }
	}


	void FaceRemoved (ARFaceAnchor anchorData)
	{
        Debug.Log("face removed");
        meshFilter.mesh = null;
		faceDebugMesh = null;
	}	



	void createDebugFaceMesh(ARFaceAnchor anchorData){
		
        faceDebugMesh = new Mesh ();
		meshFilter.mesh = faceDebugMesh;
		drawBBox();  // this is for the 3d bounding box
		updateDebugFaceMesh(anchorData);

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


     
    IEnumerator invokeEventWithDelay(UnityEvent myEvent, int delay){
        yield return new WaitForSeconds(delay);
        myEvent.Invoke();
    }




}
