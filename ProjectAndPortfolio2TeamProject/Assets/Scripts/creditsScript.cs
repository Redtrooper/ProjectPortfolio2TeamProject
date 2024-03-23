using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class creditsScript : MonoBehaviour
{
    [SerializeField] Animator creditsAnimController;

    public void closeCredits()
    {
        StartCoroutine(closeCreditsOnDelay());
    }

    private IEnumerator closeCreditsOnDelay()
    {
        if (creditsAnimController)
            creditsAnimController.SetTrigger("Close");
        yield return new WaitForSeconds(1.0f);
        gameObject.SetActive(false);
        GetComponentInParent<menuControls>().freezeInput = false;
    }
}
