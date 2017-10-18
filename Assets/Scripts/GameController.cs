using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public static GameController instance;
    public Text HPText;
    public bool CheatsOn;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        instance = this;
    }
    // Update is called once per frame

    void Update () {
        Cheats();

    }

    public void updateHUD()
    {
        HPText.text =PlayerHP.instance.HitPoints+ " HP";
    }

    public void GameOver()
    {

    }
    void Cheats()
    {
        if (CheatsOn)
        {
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                if (Time.timeScale == 0.25f)
                    Time.timeScale = 1;
                else
                Time.timeScale = 0.25f;
            }
        }
    }
    public GameObject GetChild(GameObject padre, string childName)
    {
        for(int i = 0; i <= padre.transform.childCount; i++)
        {
            if (padre.transform.GetChild(i).name == childName)
            {
                return padre.transform.GetChild(i).gameObject;
            }
        }

        return null;
    }
}
