using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class NetworkConnection : MonoBehaviour {

    public string connectToIP = "127.0.0.1";
    public int connectPort = 9000;

    public string masterServerIP = "127.0.0.1";
    public int masterServerPort = 9000;

    string gameType = "eoc74nd0ngf9vn2df84fhfuw19mcisuh90492d87374hd";
    public string serverName = "Test server";
    public string playerName = "Player";
    public string ColorName = "White";
    public string serverDescription = "Welcome Everybody!";
    public int maxConnections = 30;
    public bool usePassword;
    public string password;
    public string sceneOnDisconnect;

    int lastLevelPrefix = 0;

    public HostData[] hostData;
    public ArrayList playerList = new ArrayList();

	void Start () {
        playerName = "OldWheels-"+UtilsC.CreateRandomString(5); //Default Random Player Name

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
