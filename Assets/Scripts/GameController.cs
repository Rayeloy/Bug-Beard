using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public static GameController instance;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        instance = this;
    }
    // Update is called once per frame
    void Update () {
		
	}

    public void GameOver()
    {

    }
}
