using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{

    public static HUDManager instance;

    public Text HPText;

    public GameObject rellenoProgressBar;
    public RectTransform initialPosSlashBar;
    public RectTransform finalPosSlashBar;
    private float dRellenoPos;

    public RectTransform MissionHUD;
    public GameObject misTextPref;
    public Color missionTextColor;
    Text misTitleText;
    GameObject title;
    List<GameObject> objectives;
    List<bool> doneObjectives;

    private void Awake()
    {
        instance = this;
        objectives = new List<GameObject>();
        doneObjectives = new List<bool>();
        title = null;
    }

    private void Update()
    {
        updateHUD();
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


    public void setupMissionHUD(Mission _mis)
    {
        if (title == null)
        {
            title = Instantiate(misTextPref, MissionHUD);
        }
        title.transform.SetAsFirstSibling();
        misTitleText = title.GetComponent<Text>();
        misTitleText.text = _mis.missionName + ":";
        for (int i = 0; i < _mis.objectives.Count; i++)
        {
            GameObject aux;
            if (i > objectives.Count - 1 || objectives[i] == null)
            {
                aux = Instantiate(misTextPref, MissionHUD);
            }
            else
            {
                aux = objectives[i];
            }
            aux.transform.SetAsLastSibling();
            Text auxT = aux.GetComponent<Text>();
            auxT.text = "-" + _mis.objectives[i].ObjectiveName;
            auxT.alignment = TextAnchor.MiddleLeft;
            auxT.fontSize =misTitleText.fontSize-misTitleText.fontSize/10;
            aux.name = "Objective" + i;
            objectives.Add(aux);
            doneObjectives.Add(_mis.objectives[i].completed);
        }
        updateObjectiveCompletion();
    }

    public void updateObjectiveCompletion()
    {
        List<Objective> aux = GameController.instance.currentMission.objectives;
        for (int i = 0; i < aux.Count; i++)
        {
            objectives[i].SetActive(aux[i].active);
            if (!doneObjectives[i] && doneObjectives[i] != aux[i].completed)
            {
                doneObjectives[i] = aux[i].completed;
                Color c = new Color(missionTextColor.r, missionTextColor.g, missionTextColor.b, missionTextColor.a / 3);
                objectives[i].GetComponent<Text>().color = c;
            }
            //no meter esto dentro de !lastCompletedFound permite actualizar varios objetivos de kill a la vez
            if (aux[i].oType == Objective.ObjectiveType.kill && !aux[i].completed)
            {
                objectives[i].GetComponent<Text>().text = "-" + aux[i].ObjectiveName + " " + aux[i].enemiesKilled + "/" + aux[i].totalEnemies;
            }
        }
    }

    public void cleanMission()
    {
        int extra = objectives.Count - GameController.instance.currentMission.objectives.Count;
        for (int i = objectives.Count - 1; extra > 0; i--)
        {
            objectives.RemoveAt(i);
            extra--;
        }
    }
}
