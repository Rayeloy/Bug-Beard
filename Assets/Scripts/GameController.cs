using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public static GameController instance;
    public bool CheckPointOrder;
    public Text HPText;
    public bool CheatsOn;
    public GameObject rellenoProgressBar;
    public RectTransform initialPosSlashBar;
    public RectTransform finalPosSlashBar;
    private float dRellenoPos;

    public static List<EnemyHP> enemyList;
    public CheckPoint lastCheckPoint;
    private List<CheckPoint> checkPoints;


    private void Awake()
    {
        DontDestroyOnLoad(this);
        instance = this;
        if (enemyList == null)
        {
            enemyList = new List<EnemyHP>();
        }
        setupHUD();
        checkPoints = new List<CheckPoint>();
        checkPoints.Add(lastCheckPoint);//añadimos el primer checkPoint (la salida)
    }
    // Update is called once per frame

    void Update()
    {
        Cheats();
        updateHUD();

    }
    void setupHUD()
    {
        //SlashCDBar.maxValue = PlayerSlash.instance.cdTime;
        //SlashCDBar.interactable = false;
    }
    private float newRellenoProgressBarPos;
    public void updateHUD()
    {
        dRellenoPos = finalPosSlashBar.localPosition.x - initialPosSlashBar.localPosition.x;
        newRellenoProgressBarPos = initialPosSlashBar.localPosition.x + ((PlayerSlash.instance.cd * dRellenoPos) / PlayerSlash.instance.cdTime);
        rellenoProgressBar.transform.localPosition = new Vector2(newRellenoProgressBarPos, rellenoProgressBar.transform.localPosition.y);
    }

    public void updateHUDHP()
    {
        HPText.text = PlayerHP.instance.HitPoints + " HP";
        //SlashCDBar.value = PlayerSlash.instance.cd;
    }
    /*public float getSlashCD()
    {
        return PlayerSlash.instance.cd;
    }*/
    public void Respawn(GameObject player)
    {
        if (!CheckPointOrder)
            player.transform.position = lastCheckPoint.spawnPos.position;
        else
        {
            Debug.Log("checkPoints.Count== " + checkPoints.Count);
            if (checkPoints.Count == 1)
            {
                player.transform.position = checkPoints[0].spawnPos.position;
            }
            else if (checkPoints.Count > 1)
            {
                player.transform.position = lastCheckPoint.spawnPos.position;
            }
        }
        PlayerHP.instance.HitPoints = PlayerHP.instance.MaxHitPoints;
        updateHUDHP();
        PlayerSlash.instance.cd = 2;
    }
    public void GameOver(GameObject player)
    {
        Respawn(player);
    }

    public void AddCheckPoint(CheckPoint newCP)
    {
        bool valido = false;
        for (int i = 0; i < checkPoints.Count; i++)
        {
            if (newCP == checkPoints[i])
                return;
            if (newCP.order >= checkPoints[i].order)
            {
                valido = true;
                if (newCP.order > checkPoints[i].order)
                {
                    checkPoints.Remove(checkPoints[i]);
                }
            }
            
        }
        if (valido && CheckPointOrder)
        {
            checkPoints.Add(newCP);
            lastCheckPoint = newCP;
        }
        else if(!CheckPointOrder)
        {
            lastCheckPoint = newCP;
        }
        for (int i = 0; i < checkPoints.Count; i++)
        {
            Debug.Log("checkPoint " + i + "= " + checkPoints[i].ToString());
        }
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
        for (int i = 0; i < padre.transform.childCount; i++)
        {
            if (padre.transform.GetChild(i).name == childName)
            {
                return padre.transform.GetChild(i).gameObject;
            }
            findResult = GetChild(padre.transform.GetChild(i).gameObject, childName);
            if (findResult != null)//Recursivo
                return findResult;
        }

        return null;
    }
}
