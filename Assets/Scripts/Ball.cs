using System;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Adamant.Pong
{
    public class Ball : NetworkBehaviour
    {
        [SerializeField] 
        private float speed;
        private Vector3 direction;

        public Action<Direction> OnOutBoundary;
            
        public override void OnStartServer()
        {
            direction = Vector3.right ;
        }
        
        [ServerCallback]
        private void Update()
        {
            transform.position += (speed * Time.deltaTime) * direction;

            if (Mathf.Abs(transform.position.x) >= 12f)
            {
                OutOfBoundary();
            }
        }

        [ServerCallback]
        private void OutOfBoundary()
        {
            var outDirection = Direction.RIGHT;
            if (transform.position.x <= -12f)
            {
                outDirection = Direction.LEFT;
            }
            OnOutBoundary?.Invoke(outDirection);
            
            transform.position = Vector3.zero;
            direction = new Vector3(Random.Range(-1f,1f), Random.Range(-1f,1f),0f ).normalized;
        }

        private float HitFactor(Vector3 ballPos, Vector3 racketPos, float racketHeight)
        {
            return (ballPos.y - racketPos.y) / racketHeight;
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player))
            {
                var collider = player.GetComponent<Collider>();
                float y = HitFactor(transform.position, other.transform.position, collider.bounds.size.y);

                float x = direction.x *= -1;
                direction = new Vector3(x, y, 0).normalized;
            }
        }

        [ServerCallback]
        public void Bound()
        {
            direction = new Vector3(direction.x, direction.y * -1, 0);
        }
    }
}
