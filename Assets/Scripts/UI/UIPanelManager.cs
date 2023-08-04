using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPanelManager : MonoBehaviour
{
    public static UIPanelManager instance;

    private PlayerStateManager player;
    private StatAttribute stats;

    [SerializeField] private Slider interactSlider;             //아이템 상호작용 슬라이더
    [SerializeField] private Image playerHPSlider;
    [SerializeField] private TextMeshProUGUI keyUI;
    [SerializeField] private TextMeshProUGUI qeustTxt;

    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject restartPanel;

    [SerializeField] private CanvasGroup levelUp_cg;
    [SerializeField] private TextMeshProUGUI levelTxt;

    private CanvasGroup restart_cg;
    private Button restart_btn;

    private bool inventoryOpen;
    private bool shopOpen;

    private void Awake()
    {
        restart_cg = restartPanel.GetComponent<CanvasGroup>();
        restart_cg.alpha = 0f;
        restart_btn = restartPanel.GetComponentInChildren<Button>();
        restart_btn.enabled = false;

        player = FindObjectOfType<PlayerStateManager>();

        instance = this;

        player = FindObjectOfType<PlayerStateManager>();
        stats = player.GetComponent<StatAttribute>();

        Initialize();
    }

    private void Initialize()
    {
        SetKeyUI(false);
        SliderActive(false);
        SliderGauge(0f);

        levelUp_cg.alpha = 0f;
    }

    private void Update()
    {
        playerHPSlider.fillAmount = Mathf.InverseLerp(0f, stats.lifePool.maxValue.integer_value, stats.lifePool.currentValue);
    }

    public void SetQuestText(string questID)
    {
        qeustTxt.text = questID;
    }

    #region Item Inter Action UI
    public void SetKeyUI(bool _on)
    {
        keyUI.enabled = _on;
    }

    public void SliderActive(bool _on)
    {
        interactSlider.gameObject.SetActive(_on);
    }

    public void SliderGauge(float _value)
    {
        interactSlider.value = _value;
    }
    #endregion

    #region UI Open Close
    bool CheckUI()
    {
        if (inventoryOpen == true || shopOpen == true)
        {
            player.cursorLocked = false;
            player.SetCursorState(player.cursorLocked);
            return true;
        }

        player.cursorLocked = true;
            player.SetCursorState(player.cursorLocked);
        return false;
    }

    public void OpenInventory()
    {
        if (player.stats.isDead == true)
            return;

        inventoryPanel.SetActive(!inventoryPanel.activeInHierarchy);

        statsPanel.SetActive(!statsPanel.activeInHierarchy);
        if (statsPanel.activeInHierarchy == true)
        {
            player.GetComponent<StatAttribute>().StatAttributeUIUpdate();
        }

        inventoryOpen = inventoryPanel.activeInHierarchy;

        player.uiOpen = CheckUI();
    }

    public void OpenShop()
    {
        if (player.stats.isDead == true)
            return;

        shopPanel.SetActive(!shopPanel.activeInHierarchy);

        shopOpen = shopPanel.activeInHierarchy;

        player.uiOpen = CheckUI();
    }

    public void OpenLevelUpPanel(int level)
    {
        player.LevelEffect();
        levelTxt.text = level.ToString();

        StartCoroutine(IELevelUp());
    }

    IEnumerator IELevelUp()
    {
        StartCoroutine(IEAlpha(levelUp_cg, 2f));
        yield return new WaitForSeconds(1f);
        StartCoroutine(IEAlpha(levelUp_cg, 2f, false));
    }

    public void OpenRestart()
    {
        restartPanel.SetActive(true);

        player.cursorLocked = false;
        player.SetCursorState(player.cursorLocked);

        StartCoroutine(IEAlpha(restart_cg, 0.2f));
    }

    /// <summary>
    /// Restart Button Event
    /// </summary>
    public void RestartButton()
    {
        restartPanel.SetActive(false);

        player.gameObject.SetActive(false);

        player.cursorLocked = true;
        player.SetCursorState(player.cursorLocked);

        player.gameObject.SetActive(true);
    }

    IEnumerator IEAlpha(CanvasGroup cg, float speed, bool on = true)
    {
        float time = 0f;
        do
        {
            if (on == true)
            {
                cg.alpha += time;
            }
            else
            {
                cg.alpha -= time;
            }
            yield return null;

            time += Time.deltaTime * speed;
        } while (time >= 1f);

        cg.alpha = on == true ? 1f : 0f;
        restart_btn.enabled = true;
    }
    #endregion
}