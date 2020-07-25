using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    bool up;
    bool down;

    // Start is called before the first frame update
    void Start()
    {
        up = true;
        down = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (up || down)
        {
            StartCoroutine(Rotate(1f, up.Equals(true)));
            up = down = false;
        }
    }

    IEnumerator Rotate(float duration, bool position)
    {
        float angle;
        if (position)
        {
            angle = 20;
        }
        else
        {
            angle = -20;
        }
        float startRotation = transform.eulerAngles.y;
        float endRotation = startRotation + angle;
        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float xRotation = Mathf.Lerp(startRotation, endRotation, t / duration);
            transform.eulerAngles = new Vector3(xRotation, transform.eulerAngles.y, 
            transform.eulerAngles.z); 
            yield return null;
        }
    }
}
