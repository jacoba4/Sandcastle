using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleCollision : MonoBehaviour
{
    private bool destroyed = false;
    public GameObject visuals; // disable the visuals when destroyed

    private void Start()
    {
        // play a sound!
    }

    private void OnTriggerEnter(Collider other)
    {
        Pop();
    }

    public void Pop()
    {
        if (!destroyed)
        {
            destroyed = true;
            visuals.SetActive(false);
            Destroy(gameObject, 5f); // destroy yourself! possibly create a particle system! play a sound

            // play a sound when popped!
            //Debug.Log("Popped!");
        }
    }

    public IEnumerator PopAfterTime(float t)
    {
        yield return new WaitForSeconds(t);
        if (gameObject != null)
        {
            Pop();
        }
    }
}
