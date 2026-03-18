using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;  

[RequireComponent(typeof(AudioSource))]
public class BossManager : MonoBehaviour
{
    [Serializable]
    public struct BossSettingPerLevel
    {
        public int level;
        public int workGiven;
        public float minReentryInterval;
        public float maxReentryInterval;

        public BossSettingPerLevel(int level, int workGiven, float minReentryInterval, float maxReentryInterval)
        {
            this.level = level;
            this.workGiven = workGiven;
            this.minReentryInterval = minReentryInterval;
            this.maxReentryInterval = maxReentryInterval;
        }
    }
    public List<BossSettingPerLevel> bossSettingPerLevel =
    new List<BossSettingPerLevel>
    {
        new BossSettingPerLevel(1,3, 20f, 30f),
        new BossSettingPerLevel(2,5, 20f, 30f),
        new BossSettingPerLevel(3,7, 20f, 30f),
        new BossSettingPerLevel(4,9, 20f, 30f),
        new BossSettingPerLevel(5,11, 20f, 30f)
    };
    public float timeToBossAngry = 5f;
    public float timeToBossSatisfied = 10f;
    public float dialogChangeTime = 2f;
    public List<string> bossConverstation;
    public string reprimandDialog = "Hey! Look at me when im talking to you";
    public string angryDialog = "Fine! If you don't want to listen, then just take more work!";
    bool startedDialog;
    float bossAngryTimer;
    float dialogTimer;
    string currentDialog;
    int currentDialogIndex;
    bool readyToContinue;

    private float minReentryInterval = 10f;
    private float maxReentryInterval = 20f;
    private float reentryTimer;
    AudioSource audioSource;
    public AudioClip[] bossAudioClips;

    [Header("References")]
    public TextMeshProUGUI bossDialogBox;
    public Animator bossAnimator;

    [Header("Warning Timeline")]
    public PlayableDirector bossWarningDirector;  

    private List<string> selectedConversations; // holds the 6 random line for this boss visit
    private bool hasPlayedWarning;   // ← ONLY NEW FIELD

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        SetBossReentryInterval();
        hasPlayedWarning = false;   
        // BossStart();
    }

    void SetBossReentryInterval()
    {
        BossSettingPerLevel boss = bossSettingPerLevel.FirstOrDefault(w => w.level == GameManager.Instance.currentLevelIndex + 1);
        minReentryInterval = boss.minReentryInterval;
        maxReentryInterval = boss.maxReentryInterval;
        reentryTimer = UnityEngine.Random.Range(minReentryInterval, maxReentryInterval);
    }

    void TriggerBossAnim(string animName)
    {
        if (bossAnimator == null) return;
        bossAnimator.SetTrigger(animName);
    }
    void TriggerBossStartAnimation()
    {
        TriggerBossAnim("Enter");
    }
    void TriggerBossLeaveAnimation()
    {
        TriggerBossAnim("Leave");
    }

    // play the boss warning timeline
    void PlayBossWarning()
    {
        if (bossWarningDirector != null)
            bossWarningDirector.Play();
    }

    public void BossStart()
    {
        reentryTimer = UnityEngine.Random.Range(minReentryInterval, maxReentryInterval);

        // Pick 6 random conversations every time the boss appears
        selectedConversations = bossConverstation.OrderBy(x => UnityEngine.Random.value).Take(6).ToList();

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
        hasPlayedWarning = false;   // reset for next countdown
        TriggerBossLeaveAnimation();
    }

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
                if (AnimateDialog(angryDialog))
                {
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
            if (!readyToContinue)
            {
                AnimateDialog(reprimandDialog);
            }
            else if (dialogTimer < 0)
            {
                if (currentDialogIndex + 1 <= selectedConversations.Count)
                {
                    if (AnimateDialog(selectedConversations[currentDialogIndex])) currentDialogIndex++;
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
            audioSource.PlayOneShot(bossAudioClips[UnityEngine.Random.Range(0, bossAudioClips.Length)]);
        }
        else
        {
            dialogTimer = dialogChangeTime;
            return true;
        }
        return false;
    }

    private void FixedUpdate()
    {
        Timer();
        CallAttention();
        Dialog();
        BossReentry();
    }

    void GiveWork(int amount)
    {
        HRMiniGameManager.Instance.SpawnPaper(amount);
    }

    void OnBossAngry()
    {
        BossLeave();
        GiveWork(bossSettingPerLevel
        .FirstOrDefault(w => w.level == GameManager.Instance.currentLevelIndex + 1)
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
            currentDialogIndex = Mathf.Clamp(currentDialogIndex - 1, 0, selectedConversations.Count);
        }
        else if (CheckAttention() && !readyToContinue)
        {
            readyToContinue = true;
        }
    }

    void BossReentry()
    {
        if (!startedDialog)
        {
            reentryTimer -= Time.fixedDeltaTime;

            // 3 seconds before reentry play warning
            if (reentryTimer <= 3f && !hasPlayedWarning)
            {
                PlayBossWarning();
                hasPlayedWarning = true;
            }

            if (reentryTimer <= 0)
            {
                BossStart();
            }
        }
    }
}