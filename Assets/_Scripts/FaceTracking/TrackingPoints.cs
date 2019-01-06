using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(ParticleSystem))]
public class TrackingPoints : MonoBehaviour {

    private MeshFilter filter;
    private Vector3[] vertices;
    private ParticleSystem particleSystem;
    private ParticleSystem.Particle[] particlesArray;
    private int vertexCount;
    private bool faceDetected = false;

	// Use this for initialization
	void Start () {

        particleSystem = this.GetComponent<ParticleSystem>();
        filter = this.GetComponent<MeshFilter>();

        StartCoroutine(CreateParticles());

	}


    IEnumerator CreateParticles()
    {
        while(true){

            if (filter.sharedMesh != null)
            {
                //Debug.Log("tracking points face detected");
                faceDetected = true;
                vertices = filter.sharedMesh.vertices;
                vertexCount = (filter.sharedMesh.vertices.Length)/2; //half the number of particles drawn
                particlesArray = new ParticleSystem.Particle[vertexCount];
                particleSystem.maxParticles = vertexCount;
                particleSystem.Emit(vertexCount);
                particleSystem.GetParticles(particlesArray);
            }else{
                faceDetected = false;
                particleSystem.Clear();
            }

            if (faceDetected)
            {
                for (int i = 0; i < vertexCount; i++)
                {
                    particlesArray[i].position = vertices[i*2];
                }
                particleSystem.SetParticles(particlesArray, particlesArray.Length);
            }
            yield return new WaitForSeconds(0.125f);
        }

    }



}
