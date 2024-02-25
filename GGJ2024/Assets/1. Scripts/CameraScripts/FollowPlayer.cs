using UnityEngine;

namespace CameraScripts
{
    public class FollowPlayer : MonoBehaviour
    {
        [SerializeField] private GameObject toBeFollowed;

        [SerializeField] private Vector3 offset = new(0, -3, 10.0f);

        [SerializeField] private Vector3 farOffSet = new(0, -3, 15);

        [SerializeField] private bool farOff;

        // Start is called before the first frame update

        // Update is called once per frame
        private void Update()
        {
            if (farOff)
                transform.position = toBeFollowed.transform.position - farOffSet;
            else
                transform.position = toBeFollowed.transform.position - offset;
        }

        public void ChangeTarget(GameObject target)
        {
            toBeFollowed = target;
        }
    }
}