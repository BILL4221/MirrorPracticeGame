using System;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

namespace Adamant.Pong
{
    public enum Direction
    {
        LEFT,
        RIGHT,
    }

    public class NetworkManagerPong : NetworkManager
    {
        [SerializeField] 
        private Transform[] spawnPoints;
        private Dictionary<NetworkIdentity, Player> players;
        private Dictionary<Direction, NetworkIdentity> playerDirections;

        private Ball ball;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            players = new Dictionary<NetworkIdentity, Player>();
            playerDirections = new Dictionary<Direction, NetworkIdentity>();
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient networkConnection)
        {
            var spawnPoint = numPlayers == 0 ? spawnPoints[0] : spawnPoints[1];
            var direction = numPlayers == 0 ? Direction.LEFT : Direction.RIGHT;
            GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            NetworkServer.AddPlayerForConnection(networkConnection, player);
            players.Add(networkConnection.identity, player.GetComponent<Player>());
            playerDirections.Add(direction, networkConnection.identity);

            if (numPlayers == 2)
            {
                var ballObj = Instantiate(spawnPrefabs.Find(prefab => prefab.name.Equals("Ball")));
                NetworkServer.Spawn(ballObj);
                ball = ballObj.GetComponent<Ball>();
                ball.OnOutBoundary += BallOutBoundary;
            }
        }
        
        private void BallOutBoundary(Direction direction)
        {
            var scoreDirection = direction == Direction.RIGHT ? Direction.LEFT : Direction.RIGHT;
            var playerIdentify = playerDirections[scoreDirection];
            players[playerIdentify].AddScore(1f);
        }

        public override void OnServerDisconnect(NetworkConnectionToClient networkConnection)
        {
            if (ball != null)
            {
                NetworkServer.Destroy(ball.gameObject);
            }

            players.Remove(networkConnection.identity);
            foreach (var direction in playerDirections)
            {
                if (direction.Value == networkConnection.identity)
                {
                    playerDirections.Remove(direction.Key);
                    break;
                }
            }
            
            base.OnServerDisconnect(networkConnection);
        }
    }
}
