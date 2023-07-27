
using Mirror;
using TMPro;
using UnityEngine;

namespace Adamant.Pong
{
    public class Player : NetworkBehaviour
    {
        private const float minYSize = -3.7f;
        private const float maxYSize = 5f;
        
        [SerializeField] 
        private float speed;
        [SerializeField] 
        private TMP_Text scoreUITemplete;
        private TMP_Text scoreUI;
        [SyncVar] 
        private float score;

        public override void OnStartClient()
        {
            base.OnStartClient();
            scoreUI = Instantiate(scoreUITemplete, GameObject.Find("Tabs").transform);
        }

        [ClientCallback]
        private void Update()
        {
            if (isLocalPlayer)
            {
                CmdMove(Input.GetAxisRaw("Vertical"));
            }

            scoreUI.text = $"Player {score}";
        }
        
        [Command]
        public void CmdMove(float value)
        {   
            transform.position += Vector3.up * (value * speed * Time.deltaTime);
            var yPos = Mathf.Clamp(transform.position.y, minYSize, maxYSize);
            transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
        }

        public void AddScore(float score)
        {
            this.score += score;
        }
        
    }
}
