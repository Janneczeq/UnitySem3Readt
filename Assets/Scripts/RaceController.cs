using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class RaceController : MonoBehaviourPunCallbacks
{
    public CheckPointController[] carsController;
    public static bool racing = false;
    public static int totalLaps = 1;
    public int timer = 3;

    public Text startText;
    AudioSource audioSource;
    public AudioClip count;
    public AudioClip start;

    public GameObject endPanel;

    public GameObject carPrefab;
    public Transform[] spawnPos;
    public int playerCount;
    public GameObject startRace;
    public GameObject waitingText;

    public RawImage mirror;

    public void SetMirror(Camera backCamera)
    {
        mirror.texture = backCamera.targetTexture;

    }


    /*
    void Start()
    {
        endPanel.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        startText.gameObject.SetActive(false);
        InvokeRepeating("CountDown", 3, 1);
        for (int i = 0; i < playerCount; i++)
        {
            GameObject car = Instantiate(carPrefab);
            car.transform.position = spawnPos[i].position;
            car.transform.rotation = spawnPos[i].rotation;
            car.GetComponent<CarApperance>().playerNumber = i;
            if (i == 0)
            {
                car.GetComponent<PlayerController>().enabled = true;
                GameObject.FindObjectOfType<CameraController>().SetCameraProperties(car);
            }
        }
        GameObject[] cars = GameObject.FindGameObjectsWithTag("Car");
        carsController = new CheckPointController[cars.Length];
        for (int i = 0; i < cars.Length; i++)
        {
            carsController[i] = cars[i].GetComponent<CheckPointController>();
        }
    }
    */

    private void Start()
    {
        playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        endPanel.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        startText.gameObject.SetActive(false);

        startRace.SetActive(false);
        waitingText.gameObject.SetActive(false);

        startRace.SetActive(false );
        waitingText.SetActive(false);

        int randomStartPosition = Random.Range(0, spawnPos.Length);
        Vector3 startPos = spawnPos[randomStartPosition].position;
        Quaternion startRot = spawnPos[randomStartPosition].rotation;

        GameObject playerCar = null;
        Debug.Log("Test: "+PhotonNetwork.IsConnected);
        if (PhotonNetwork.IsConnected)
        {
            startPos = spawnPos[PhotonNetwork.CurrentRoom.PlayerCount - 1].position;
            startRot = spawnPos[PhotonNetwork.CurrentRoom.PlayerCount - 1].rotation;

            object[] instanceData = new object[4];
            instanceData[0] = (string)PlayerPrefs.GetString("PlayerName");
            instanceData[1] = (string)PlayerPrefs.GetString("Red");
            instanceData[2] = (string)PlayerPrefs.GetString("Green");
            instanceData[3] = (string)PlayerPrefs.GetString("Blue");
            Debug.Log(OnlinePlayer.LocalPlayerInstance == null);
            if(OnlinePlayer.LocalPlayerInstance == null)
            {
                playerCar = PhotonNetwork.Instantiate(carPrefab.name, startPos, startRot, 0, instanceData);
                playerCar.GetComponent<CarApperance>().SetLocalPlayer();

            }
            if (PhotonNetwork.IsMasterClient)
            {
                startRace.SetActive(true);

            }
            else
            {
                waitingText.SetActive(true);
            }
        }
        playerCar.GetComponent<DrivingScript>().enabled = true;
        playerCar.GetComponent<PlayerController>().enabled = true;
    }






    void LateUpdate()
    {
        int finishedLap = 0;

        if (racing)
        {


            foreach (CheckPointController controller in carsController)
            {
                if (controller.lap == totalLaps + 1) finishedLap++;

                if (finishedLap == carsController.Length && racing)
                {
                    endPanel.SetActive(true);
                    racing = false;
                }
            }
        }
    }

    void CountDown()
    {
        startText.gameObject.SetActive(true);
        if (timer != 0)
        {
            startText.text = timer.ToString();
            audioSource.PlayOneShot(count);
            timer--;
        }
        else
        {
            startText.text = "START!!!";
            audioSource.PlayOneShot(start);
            racing = true;
            CancelInvoke("CountDown");
            Invoke("HideStartText", 1);
        }
    }

    void HideStartText()
    {
        startText.gameObject.SetActive(false);
    }

    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }
    [PunRPC]
    public void StartGame()
    {
        InvokeRepeating("CountDown", 3, 1);
        startRace.SetActive(false);
        waitingText.SetActive(false);
        GameObject[] cars = GameObject.FindGameObjectsWithTag("Car");
        carsController = new CheckPointController[cars.Length];
        for(int i = 0; i <cars.Length; i ++)
        {
            carsController[i] = cars[i].GetComponent<CheckPointController>();
        }

    }

    public void BeginGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("StartGame", RpcTarget.All,null);
        }

    }

}
