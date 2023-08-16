using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTrigger : MonoBehaviour
{
    private bool actionDone;
    private float actionTime = 3f;

    private InterActionObject player;

    [SerializeField]
    private Renderer renderer;
    [SerializeField]
    private Material meterial;
    [SerializeField]
    private ParticleSystem particle;

    private IEnumerator ieInterAction;

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
            renderer.material.SetFloat("_DECALEMISSIONONOFF", 1f);          //석상 쉐이더 조절
            ItemEffect();

            GameEventsManager.instance.miscEvents.ItemInteracted();         //퀘스트 진행
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
        if (other.gameObject.tag.Contains("Player") == false)
        {
            return;
        }

        if (actionDone == false)
        {
            UIPanelManager.instance.SetKeyUI(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag.Contains("Player") == false)
        {
            return;
        }

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

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Contains("Player") == false)
        {
            return;
        }

        UIPanelManager.instance.SetKeyUI(false);

        if (ieInterAction != null)
        {
            StopCoroutine(ieInterAction);
            ieInterAction = null;
        }
    }
}