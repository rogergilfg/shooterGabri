using Photon.Pun;
using UnityEngine;

public class LevelMultiplayerManager : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PhotonNetwork.Instantiate("MultiplayerPlayer", spawnPoints[0].position, spawnPoints[0].rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
