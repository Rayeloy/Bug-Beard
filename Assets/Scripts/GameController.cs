using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public static GameController instance;
    public Text HPText;
    public bool CheatsOn;

    public static List<EnemyHP> enemyList;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        instance = this;
        if (enemyList == null)
        {
            enemyList = new List<EnemyHP>();
        }
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
    public GameObject GetChild(GameObject padre, string childName)//Busca recursivamente un hijo con nombre childName
    {
        GameObject findResult;
        for(int i = 0; i < padre.transform.childCount; i++)
        {
            if (padre.transform.GetChild(i).name == childName)
            {
                return padre.transform.GetChild(i).gameObject;
            }
            findResult = GetChild(padre.transform.GetChild(i).gameObject, childName);
            if(findResult != null)//Recursivo
                return findResult;
        }

        return null;
    }
}
