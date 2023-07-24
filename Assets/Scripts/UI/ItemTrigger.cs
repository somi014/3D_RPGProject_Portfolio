using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemTrigger : MonoBehaviour
{
    bool actionDone;
    float actionTime = 3f;

    InterActionObject player;

    [SerializeField] Renderer renderer;
    [SerializeField] Material meterial;
    [SerializeField] ParticleSystem particle;

    IEnumerator ieInterAction;

    private void Awake()
    {
        player = FindObjectOfType<InterActionObject>();

        actionDone = false;
    }

    private void Start()
    {
        Material mat = new Material(meterial);
        renderer.material = mat;
        renderer.material.SetFloat("_DECALEMISSIONONOFF", 0f);
    }

    private void OnEnable()
    {
        actionDone = false;
    }

    IEnumerator IEInterAction()
    {
        UIPanelManager.instance.SliderActive(true);
        UIPanelManager.instance.SetKeyUI(false);

        float time = 0f;
        float gauge = 0f;
        do
        {
            gauge = time / actionTime;
            UIPanelManager.instance.SliderGauge(gauge);

            yield return null;
            time += Time.deltaTime;

        } while (player.pickUp == true && time <= actionTime);

        if (time >= actionTime)
        {
            actionDone = true;
            renderer.material.SetFloat("_DECALEMISSIONONOFF", 1f);
            ItemEffect();

            GameEventsManager.instance.miscEvents.ItemInteracted();         //Äù½ºÆ® ÁøÇà
        }
        else
        {
            UIPanelManager.instance.SliderGauge(0f);
        }

        UIPanelManager.instance.SliderActive(false);
        ieInterAction = null;
    }

    private void ItemEffect()
    {
        particle.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Contains("Player") == true)
        {
            if (actionDone == false)
            {
                UIPanelManager.instance.SetKeyUI(true);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag.Contains("Player") == true)
        {
            if (player.pickUp == true && ieInterAction == null)
            {
                if (actionDone == false)
                {
                    ieInterAction = IEInterAction();
                    StartCoroutine(ieInterAction);
                }
            }
            else if (player.pickUp == false && actionDone == false)
            {
                UIPanelManager.instance.SetKeyUI(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Contains("Player") == true)
        {
            UIPanelManager.instance.SetKeyUI(false);

            if (ieInterAction != null)
            {
                StopCoroutine(ieInterAction);
                ieInterAction = null;
            }
        }
    }
}
