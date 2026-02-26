using UnityEngine;

public class NPCVoice : MonoBehaviour
{
    [Header("Ρυθμίσεις Ήχου")]
    public AudioSource voiceSound;     
    public GameObject dialoguePanel;   

    private bool wasTalking = false;   

    void Update()
    {
        if (dialoguePanel != null && voiceSound != null)
        {
            bool isTalkingNow = dialoguePanel.activeInHierarchy;

            if (isTalkingNow == true && wasTalking == false)
            {
                voiceSound.Play(); // Ξεκίνα τη φωνή
                wasTalking = true;
            }
            else if (isTalkingNow == false && wasTalking == true)
            {
                voiceSound.Stop(); // Σταμάτα τη φωνή
                wasTalking = false;
            }
        }
    }
}