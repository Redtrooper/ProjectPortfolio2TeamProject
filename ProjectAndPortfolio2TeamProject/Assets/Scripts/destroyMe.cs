using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyMe : MonoBehaviour
{
    [SerializeField] float timeToDestroy;
    [SerializeField] Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        if (anim)
        {
            StartCoroutine(AnimateOnDelay());
        }
        else
            Destroy(gameObject, timeToDestroy);
    }
    private IEnumerator AnimateOnDelay()
    {
        yield return new WaitForSeconds(timeToDestroy);
        if (anim)
            anim.SetTrigger("Close");
        StartCoroutine(DestroyOnAnimFinish());
    }

    private IEnumerator DestroyOnAnimFinish()
    {
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject);
    }
}
