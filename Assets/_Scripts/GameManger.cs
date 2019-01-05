using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManger : MonoBehaviour {

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Use this for initialization
    void Start () {
        StartCoroutine(restartScene());
	}

    public IEnumerator restartScene(){
        yield return new WaitForSeconds(10);
        Debug.Log("Scene restart");
        SceneManager.LoadSceneAsync("Face2Face_V1");
    }

}
