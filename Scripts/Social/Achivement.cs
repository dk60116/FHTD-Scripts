using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_ANDROID
public class Achivement : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI txtLog;

    public void ShowAchivementGUI()
    {
        Social.ShowAchievementsUI();
    }

    public void DoGrantAchivement(string _strAchive)
    {
        Social.ReportProgress(_strAchive, 100.00f, (bool _bSucess) =>
        {
            txtLog.text = _strAchive + " : " + _bSucess.ToString();
        });
    }

    public void DoRevealAchivement(string _strAchive)
    {
        Social.ReportProgress(_strAchive, 0.00f, (bool _bSucess) =>
        {
            txtLog.text = _strAchive + " : " + _bSucess.ToString();
        });
    }

    public void ListAchivements()
    {
        Social.LoadAchievements(achivements =>
        {
            txtLog.text = "Loaded Achivements" + achivements.Length;

            foreach (var item in achivements)
            {
                txtLog.text += "\n" + item.id + " " + item.completed;
            }
        });
    }

    public void ListDescription()
    {
        Social.LoadAchievementDescriptions(achivements =>
        {
            txtLog.text = "Loaded Achivements" + achivements.Length;

            foreach (var item in achivements)
            {
                txtLog.text += "\n" + item.id + " " + item.title;
            }
        });
    }
}
#endif
