using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BossStartTrigger : MonoBehaviour
{
    [SerializeField] BossAI boss;

    [SerializeField] PlayableDirector director;

    private void StartTimeline()
    {
        director.Play();

        StartCoroutine(IETimelinePlay());
    }

    IEnumerator IETimelinePlay()
    {
        yield return new WaitUntil(()=> director.state == PlayState.Paused);
        
        boss.bossStart = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Contains("Player") == true)
        {
            StartTimeline();

        }
    }
}
