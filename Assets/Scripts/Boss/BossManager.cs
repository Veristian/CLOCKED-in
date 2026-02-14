using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public struct WorkGain
    {
        public int level;
        public int workGiven;

        public WorkGain(int level, int workGiven)
        {
            this.level = level;
            this.workGiven = workGiven;
        }
    }
    public List<WorkGain> workGainByLevel = 
    new List<WorkGain>
    {
        new WorkGain(1,3),
        new WorkGain(2,5),
        new WorkGain(3,7),
        new WorkGain(4,9),
        new WorkGain(5,11)
    };
    public float timeToBossAngry = 5f;
    public float timeToBossSatisfied = 10f;
    public float dialogChangeTime = 2f;
    public List<string> bossConverstation;
    // public List<string> bossListenToMe;
    public string reprimandDialog = "Hey! Look at me when im talking to you";
    public string angryDialog = "Fine! If you don't want to listen, then just take more work!";
    bool startedDialog;
    float bossAngryTimer;
    // float bossSatisfiedTimer;  
    float dialogTimer;  
    string currentDialog;
    int currentDialogIndex;
    bool readyToContinue;

    [Header("References")]
    public TextMeshProUGUI bossDialogBox;
    public Animator bossAnimator;

    private void Start()
    {
        BossStart();
        startedDialog = true;
    }
    //Boss Appeaar
    void TriggerBossAnim(string animName)
    {
        if (bossAnimator == null) return;
        bossAnimator.SetTrigger(animName);
    }
    void TriggerBossStartAnimation()
    {
        TriggerBossAnim("Enter");
    }
    //Boss Leave
    void TriggerBossLeaveAnimation()
    {
        TriggerBossAnim("Leave");
    }
    public void BossStart()
    {
        currentDialogIndex = 0;
        readyToContinue = true;
        startedDialog = true;
        bossAngryTimer = 0;
        TriggerBossStartAnimation();
    }
    public void BossLeave()
    {
        startedDialog = false;
        bossDialogBox.text = "";
        TriggerBossLeaveAnimation();
    }
    //Boss Talk Counter
    
    //Boss Dialog Spawner
    void SetDialog(string dialog)
    {
        currentDialog = dialog;
        bossDialogBox.text = "";
    }
    void Dialog()
    {
        if (!startedDialog)
        {
            if (bossAngryTimer > timeToBossAngry && dialogTimer < 0)
            {
                if (AnimateDialog(angryDialog)) {
                    bossAngryTimer = 0;
                }
            }
            else if (dialogTimer < 0)
            {
                bossDialogBox.text = "";
            }
        }
        else
        {

            if (!readyToContinue )//&& dialogTimer < 0)
            {
                AnimateDialog(reprimandDialog);
            }
            else if (dialogTimer < 0)
            {
                if (currentDialogIndex + 1 <= bossConverstation.Count)
                {
                    if (AnimateDialog(bossConverstation[currentDialogIndex])) currentDialogIndex++;
                }
                else
                {
                    BossLeave();
                }
            }
                    
        }
        dialogTimer -= Time.deltaTime;
    }
    bool AnimateDialog(string dialog)
    {
        if (bossDialogBox == null) return false;
        if (bossDialogBox.text.Length > dialog.Length) bossDialogBox.text = "";
        if (bossDialogBox.text != dialog[0..bossDialogBox.text.Length]) bossDialogBox.text = "";

        if (bossDialogBox.text != dialog)
        {
            
            bossDialogBox.text = bossDialogBox.text + dialog[bossDialogBox.text.Length];
            
        }
        else
        {
            
            dialogTimer = dialogChangeTime;
            // readyToContinue = true;
            Debug.Log(bossDialogBox.text);
            Debug.Log(dialog);
            return true;
        }
        return false;

    }
    private void FixedUpdate()
    {
        Timer();
        CallAttention();
        Dialog();
        
    }
    //Boss Gives more works
    void GiveWork(int amount)
    {
        HRMiniGameManager.Instance.SpawnPaper(amount);
    }

    void OnBossAngry()
    {
        BossLeave();
        GiveWork(workGainByLevel
        .FirstOrDefault(w => w.level == GameManager.Instance.currentLevelIndex)
        .workGiven);
    }

    void Timer()
    {
        if (!startedDialog) return;
        if (!readyToContinue)
        {
            bossAngryTimer += Time.deltaTime;
        }
        if (bossAngryTimer > timeToBossAngry)
        {
            OnBossAngry();
        }
    }
    //Boss detect ur listening
    bool CheckAttention()
    {
        return CanvasManager.Instance.activeSector == 2;
    }
    void CallAttention()
    {
        if (!startedDialog) return;
        if (!CheckAttention() && readyToContinue)
        {
            readyToContinue = false;
            Debug.Log("asdas");
            currentDialogIndex = Mathf.Clamp(currentDialogIndex - 1, 0, bossConverstation.Count);
        }
        else if (CheckAttention() && !readyToContinue)
        {
            readyToContinue = true;
        }
    }
    
}
