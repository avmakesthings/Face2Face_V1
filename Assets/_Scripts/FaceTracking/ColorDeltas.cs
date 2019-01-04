using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorDeltas : MonoBehaviour {

    public float m_Threshold;
    public Color m_Color;

    private MeshFilter filter;
    private Vector3[] vertices;
    private Vector3[] lastVertices;

    private int[] thresholdDebounce;
    private Color[] colors;

    void Update(){
        
		ColorMesh ();
    }

    void ColorMesh(){
        filter = GetComponent<MeshFilter>();
		if(filter.sharedMesh!=null){

			vertices = filter.sharedMesh.vertices;

			if (colors == null) {
				colors = new Color[vertices.Length];
			}
			if (thresholdDebounce == null) {
				thresholdDebounce = new int[vertices.Length];
			}
			if (lastVertices == null) {
				lastVertices = (Vector3[])vertices.Clone ();
				return;
			}

			float delta;
			Vector3 lastPos;
			Vector3 curPos;
			for (int i = 0; i < vertices.Length; i++) {
				curPos = vertices [i];
				lastPos = lastVertices [i];

				delta = (curPos - lastPos).magnitude;

				if (delta > m_Threshold) {
					thresholdDebounce [i] += 1;
				} else {
					if(thresholdDebounce[i]>0){
						thresholdDebounce[i] -= 1;
					}
				}

				if (thresholdDebounce [i] > 5) {
					colors [i] = m_Color;
				}
				else {
					colors[i] = Color.clear;
				}

				lastVertices [i] = new Vector3(curPos.x, curPos.y, curPos.z);
			}

			filter.sharedMesh.colors = colors;
			filter.sharedMesh.RecalculateBounds();
		}
    }
}

