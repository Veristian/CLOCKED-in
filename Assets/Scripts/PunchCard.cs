using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(AudioSource))]
public class PunchCard : MonoBehaviour
{
    public UIDraggable draggable;
    public RectTransform cardRectTransform;
    public AudioClip paperRustleClip;
    public AudioClip clankClip;
    public AudioSource audioSource;

    [Header("Drop Slots")]
    [SerializeField] private RectTransform startSlot;     
    [SerializeField] private RectTransform settingSlot;   
    [SerializeField] private RectTransform exitSlot;     

    [SerializeField] private string startSceneName = "YourSceneNameHere"; 
    [SerializeField] private GameObject settingMenu;

    [Header("Overlap Threshold")]
    //how much overlap is required (0 = any touch, 1 = full containment)
    [SerializeField, Range(0f, 1f)] private float overlapThreshold = 0.3f;

    private Vector2 originalPosition;

    private void Awake()
    {
        if (draggable == null)
            draggable = GetComponent<UIDraggable>();
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        if (cardRectTransform == null)
            cardRectTransform = GetComponent<RectTransform>();

        if (settingMenu == null)
            settingMenu = GameObject.Find("setting menu"); // Fallback

        if (draggable != null)
        {
            draggable.onEndDrag.AddListener(CheckDrop);
            if (paperRustleClip != null)
            {
                draggable.onBeginDrag.AddListener(() => PlayRustle());
                draggable.onEndDrag.AddListener(() => PlayRustle());
            }
        }
        
        originalPosition = cardRectTransform.anchoredPosition;
    }

    void PlayRustle()
    {
        if (paperRustleClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(paperRustleClip);
        }
    }
    void PlayClank()
    {
        if (clankClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clankClip);
        }
    }

    private void CheckDrop()
    {
        if (cardRectTransform == null) return;

        Rect cardRect = GetScreenRect(cardRectTransform);


        if (startSlot != null && GetOverlapRatio(cardRect, GetScreenRect(startSlot)) >= overlapThreshold)
        {
            if (!string.IsNullOrEmpty(startSceneName))
            {
                PlayerPrefs.SetInt("CurrentLevel", 1);
                SceneManager.LoadScene(startSceneName);
                PlayClank();
            }
            return;
        }

        if (settingSlot != null && GetOverlapRatio(cardRect, GetScreenRect(settingSlot)) >= overlapThreshold)
        {
            if (settingMenu != null)
            {
                settingMenu.SetActive(true);
                cardRectTransform.anchoredPosition = originalPosition;
                PlayClank();
            }
            return; // remove if you want to allow multiple actions
        }

        if (exitSlot != null && GetOverlapRatio(cardRect, GetScreenRect(exitSlot)) >= overlapThreshold)
        {
            PlayClank();
            QuitApplication();
        }

        // reset position if no valid drop
        // cardRectTransform.anchoredPosition = originalPosition;
    }

    private void QuitApplication()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Returns a value 0-1 indicating how much of the card overlaps the slot (0 = none, 1 = fully inside)
    private float GetOverlapRatio(Rect card, Rect slot)
    {
        if (!card.Overlaps(slot, true)) return 0f;

        Rect overlap = Rect.MinMaxRect(
            Mathf.Max(card.xMin, slot.xMin),
            Mathf.Max(card.yMin, slot.yMin),
            Mathf.Min(card.xMax, slot.xMax),
            Mathf.Min(card.yMax, slot.yMax)
        );

        float overlapArea = overlap.width * overlap.height;
        float cardArea = card.width * card.height;

        return overlapArea / cardArea;
    }

    private Rect GetScreenRect(RectTransform rt)
    {
        if (rt == null) return new Rect();

        Vector3[] worldCorners = new Vector3[4];
        rt.GetWorldCorners(worldCorners);

        float minX = float.MaxValue, minY = float.MaxValue;
        float maxX = float.MinValue, maxY = float.MinValue;

        for (int i = 0; i < 4; i++)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, worldCorners[i]);
            minX = Mathf.Min(minX, screenPoint.x);
            maxX = Mathf.Max(maxX, screenPoint.x);
            minY = Mathf.Min(minY, screenPoint.y);
            maxY = Mathf.Max(maxY, screenPoint.y);
        }

        return new Rect(minX, minY, maxX - minX, maxY - minY);
    }
}