using UnityEngine;

namespace DefaultNamespace
{
    public class LeasCarController : MonoBehaviour
    {
        public void UpdatePosition()
        {
            var leasGameObject = gameObject.transform;
            leasGameObject.position = leasGameObject.position + new Vector3(1, 0, 1)*Time.deltaTime;
        }
    }
}