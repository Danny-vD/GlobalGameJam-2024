    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private GameObject toBeFollowed;

    [SerializeField] private Vector3 offset = new Vector3(0, -3, 10.0f);

    [SerializeField] private Vector3 farOffSet = new Vector3(0, -3, 15);

    [SerializeField] private bool farOff;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (farOff)
        {
            transform.position = toBeFollowed.transform.position - farOffSet;
        }
        else
        {
            transform.position = toBeFollowed.transform.position - offset;
        }
        
    }

    public void ChangeTarget(GameObject target)
    {
        toBeFollowed = target;
    }
}