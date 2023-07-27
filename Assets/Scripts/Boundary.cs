using Mirror;
using UnityEngine;

namespace Adamant.Pong
{
    public class Boundary : NetworkBehaviour
    {
        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Ball ball))
            {
                ball.Bound();
            }
        }
    }
}
