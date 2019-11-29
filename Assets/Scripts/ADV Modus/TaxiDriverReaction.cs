using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class TaxiDriverReaction : MonoBehaviour
{
    // Start is called before the first frame update
    private AudioClip reactionAudio;
    public AudioMixerGroup reactionMixerOutput;
    private bool triggerLock;

    private void Awake()
    {
        triggerLock = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<EventGate>() != null && this.CompareTag("ADV") && !triggerLock)
        {
            if (WestdriveSettings.language == "DE")
            {
                reactionAudio = other.GetComponent<EventGate>().GermanReactionAudio;
            }
            else if (WestdriveSettings.language == "ENG")
            {
                reactionAudio = other.GetComponent<EventGate>().GermanReactionAudio;
            }
            //AudioSource.PlayClipAtPoint(reactionAudio,transform.position,1f);
            StartCoroutine(PlayReaction(other.GetComponent<EventGate>().reactionDelay));
        }
        
    }

    private IEnumerator PlayReaction(float audioDelay)
    {
        triggerLock = true;
        
        AudioSource reactionSource = this.gameObject.AddComponent<AudioSource>() as AudioSource;
        reactionSource.playOnAwake = false;
        reactionSource.clip = reactionAudio;
        reactionSource.outputAudioMixerGroup = reactionMixerOutput;
        yield return new WaitForSeconds(audioDelay);
        reactionSource.Play();
        EventManager.TriggerEvent("pauseTaxiAudio");
        while (reactionSource.isPlaying)
        {
            yield return null;
        }
        Destroy(reactionSource);
        EventManager.TriggerEvent("resumeTaxiAudio");
        triggerLock = false;
        yield return null;
    }
}