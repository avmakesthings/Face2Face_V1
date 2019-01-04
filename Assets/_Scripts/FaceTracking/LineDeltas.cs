using System.Collections;


using System.Collections.Generic;


using UnityEngine;



public class LineDeltas : MonoBehaviour

{
    public float cutoff;
    private MeshFilter filter;
    private LineRenderer lineRenderer;
    private Vector3[] vertices;
    private Vector3[] lastVertices;
    private bool[] changed;
    private int vertexCount;


    void Start()

    {
        StartCoroutine(DrawDeltas());
    }


    IEnumerator DrawDeltas()

    {
        while (true)

        {
            filter = GetComponent<MeshFilter>();
            lineRenderer = GetComponent<LineRenderer>();
            if (filter.sharedMesh != null && lineRenderer)

            {
                vertices = filter.sharedMesh.vertices;

                if (lastVertices == null || changed == null)

                {
                    vertexCount = filter.sharedMesh.vertices.Length;
                    lastVertices = new Vector3[vertexCount];
                    changed = new bool[vertexCount];

                }


                Vector3 v;
                Vector3 nextV;
                int changedCount = 0;
                for (int i = 0; i < vertexCount; ++i)

                {

                    v = lastVertices[i];
                    nextV = vertices[i];
                    if (v != nextV && Vector3.Distance(v, nextV) > cutoff)

                    {

                        changed[i] = true;
                        changedCount++;

                    }

                    else

                    {
                        changed[i] = false;
                    }

                }


                lineRenderer.SetVertexCount(changedCount);

                int j = 0;
                for (int i = 0; i < vertexCount; i++)

                {

                    if (changed[i])
                    {
                        lineRenderer.SetPosition(j, vertices[i]);
                        j++;
                    }

                }

                lastVertices = vertices;

            }
            yield return new WaitForSeconds(0.125f);

        }

    }

}