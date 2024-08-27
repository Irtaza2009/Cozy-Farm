using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int coinCount;
    public int eggCount;
    public int fruitCount;
    public int milkCount;
    public int workerCost;
    public int hatchCost;
    public int cowCost;

    private DataSaver dataSaver;

    public TextMeshProUGUI coinText;
    public TextMeshProUGUI eggText;
    public TextMeshProUGUI milkText;
    public TextMeshProUGUI fruitText;
    public TextMeshProUGUI hatchText;
    public TextMeshProUGUI workerText;
    public TextMeshProUGUI buyCowText;

    public GameObject chickPrefab;
    public GameObject eggPrefab;
    public GameObject workerPrefab;
    public GameObject cowPrefab;
    public GameObject milkPrefab;

    private List<GameObject> workers = new List<GameObject>();
    private List<GameObject> chicks = new List<GameObject>();
    private List<GameObject> cows = new List<GameObject>();

    public string fruitType = "Wheat";

    public Button wheatButton;
    public Button fruitButton;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

    }

    void Start()
    {
        dataSaver = FindObjectOfType<DataSaver>();
        if (dataSaver != null)
        {
            StartCoroutine(LoadGameStateCoroutine());
        }
        else
        {
            Debug.LogError("DataSaver instance not found in the scene.");
        }
    }

    void Update()
    {
        UpdateUI();
    }

    public void AddCoin()
    {
        coinCount++;
        SaveGameState();
    }

    public void SpendCoins(int amount)
    {
        coinCount -= amount;
        SaveGameState();
    }

    public void CollectEgg(GameObject egg)
    {
        eggCount++;
        Destroy(egg);
        SaveGameState();
    }

    public void SellEggs()
    {
        coinCount += eggCount;
        eggCount = 0;
        SaveGameState();
    }

    public void HatchEgg()
    {
        if (eggCount > 0 && coinCount >= hatchCost)
        {
            coinCount -= hatchCost;
            hatchCost += 10;
            eggCount--;
            AddHatchEgg();
            SaveGameState();
        }
    }

    public void AddHatchEgg()
    {
        Vector3 position = GetRandomPosition();
        Instantiate(eggPrefab, position, Quaternion.identity);

    }

    public void AddChickToList(GameObject chick)
    {
        chicks.Add(chick);
    }


    public void BuyWorker()
    {
        if (coinCount >= workerCost)
        {
            coinCount -= workerCost;
            workerCost += 10;
            Vector3 position = GetRandomPosition();
            GameObject worker = Instantiate(workerPrefab, position, Quaternion.identity);
            workers.Add(worker);
            SaveGameState();
        }
    }

    public void BuyCow()
    {
        if (coinCount >= cowCost)
        {
            coinCount -= cowCost;
            cowCost += 10;
            Vector3 position = GetRandomPosition();
            GameObject cow = Instantiate(cowPrefab, position, Quaternion.identity);
            cows.Add(cow);
            SaveGameState();
        }
    }

    public void AddCow()
    {
        Vector3 position = GetRandomPosition();
        GameObject cow = Instantiate(cowPrefab, position, Quaternion.identity);
        cows.Add(cow);
    }

    public void CollectMilk(GameObject milk)
    {
        milkCount++;
        Destroy(milk);
        SaveGameState();
    }

    public void CollectFruit()
    {
        fruitCount++;
        SaveGameState();
    }

    public void SellMilk()
    {
        coinCount += milkCount;
        milkCount = 0;
        SaveGameState();
    }

    public void SellFruit()
    {
        coinCount += fruitCount;
        fruitCount = 0;
        SaveGameState();
    }

    void UpdateUI()
    {
        if (coinText != null) coinText.text = coinCount.ToString();
        if (eggText != null) eggText.text = eggCount.ToString();
        if (milkText != null) milkText.text = milkCount.ToString();
        if (fruitText != null) fruitText.text = fruitCount.ToString();
        if (hatchText != null) hatchText.text = "Hatch Egg <br> Cost: " + hatchCost;
        if (workerText != null) workerText.text = "Buy Worker <br> Cost: " + workerCost;
        if (buyCowText != null) buyCowText.text = "Buy Cow <br> Cost: " + cowCost;
    }

    Vector3 GetRandomPosition()
    {
        float minX = GameObject.Find("LeftBoundary").transform.position.x + 0.5f;
        float maxX = GameObject.Find("RightBoundary").transform.position.x - 0.5f;
        float minY = GameObject.Find("BottomBoundary").transform.position.y + 0.5f;
        float maxY = GameObject.Find("TopBoundary").transform.position.y - 0.5f;

        return new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), -1);
    }

    public void SaveGameState()
    {
        PlayerPrefs.SetInt("CoinCount", coinCount);
        PlayerPrefs.SetInt("EggCount", eggCount);
        PlayerPrefs.SetInt("MilkCount", milkCount);
        PlayerPrefs.SetInt("WorkerCost", workerCost);
        PlayerPrefs.SetInt("HatchCost", hatchCost);
        PlayerPrefs.SetInt("CowCost", cowCost);
        PlayerPrefs.SetInt("FruitCount", fruitCount);

        // Save worker positions
        for (int i = 0; i < workers.Count; i++)
        {
            PlayerPrefs.SetFloat("Worker" + i + "X", workers[i].transform.position.x);
            PlayerPrefs.SetFloat("Worker" + i + "Y", workers[i].transform.position.y);
        }

        // Save chick positions
        for (int i = 0; i < chicks.Count; i++)
        {
            PlayerPrefs.SetFloat("Chick" + i + "X", chicks[i].transform.position.x);
            PlayerPrefs.SetFloat("Chick" + i + "Y", chicks[i].transform.position.y);
        }

        // Save cow positions
        for (int i = 0; i < cows.Count; i++)
        {
            PlayerPrefs.SetFloat("Cow" + i + "X", cows[i].transform.position.x);
            PlayerPrefs.SetFloat("Cow" + i + "Y", cows[i].transform.position.y);
        }

        PlayerPrefs.Save();

        #if !UNITY_WEBGL
        dataSaver.SavePlayerData(eggCount, milkCount, fruitCount, coinCount, workerCost, cowCost, hatchCost);
        #endif
    }

    IEnumerator LoadGameStateCoroutine()
    {
        #if !UNITY_WEBGL
        yield return new WaitUntil(() => dataSaver.IsInitialized);

        dataSaver.LoadPlayerData(playerData =>
        {
            if (playerData != null)
            {
                eggCount = playerData.eggCount;
                milkCount = playerData.milkCount;
                fruitCount = playerData.fruitCount;
                coinCount = playerData.coinCount;
                workerCost = playerData.workerCost;
                cowCost = playerData.cowCost;
                hatchCost = playerData.hatchCost;

                UpdateUI();
            }
        });
        #else
        LoadGameStateFromPlayerPrefs();
        yield return null;
        #endif
        
        LoadWorkerPositions();
        LoadChickPositions();
        LoadCowPositions();
    }

    void LoadGameStateFromPlayerPrefs()
    {
        coinCount = PlayerPrefs.GetInt("CoinCount", 0);
        eggCount = PlayerPrefs.GetInt("EggCount", 0);
        milkCount = PlayerPrefs.GetInt("MilkCount", 0);
        workerCost = PlayerPrefs.GetInt("WorkerCost", 10);
        hatchCost = PlayerPrefs.GetInt("HatchCost", 10);
        cowCost = PlayerPrefs.GetInt("CowCost", 10);
        fruitCount = PlayerPrefs.GetInt("FruitCount", 0);

        UpdateUI();
    }

    void LoadWorkerPositions()
    {
        for (int i = 0; ; i++)
        {
            if (!PlayerPrefs.HasKey("Worker" + i + "X"))
                break;

            float x = PlayerPrefs.GetFloat("Worker" + i + "X");
            float y = PlayerPrefs.GetFloat("Worker" + i + "Y");
            Vector3 position = new Vector3(x, y, -1);
            GameObject worker = Instantiate(workerPrefab, position, Quaternion.identity);
            workers.Add(worker);
        }
    }

    void LoadChickPositions()
    {
        for (int i = 0; ; i++)
        {
            if (!PlayerPrefs.HasKey("Chick" + i + "X"))
                break;

            float x = PlayerPrefs.GetFloat("Chick" + i + "X");
            float y = PlayerPrefs.GetFloat("Chick" + i + "Y");
            Vector3 position = new Vector3(x, y, -1);
            GameObject chick = Instantiate(chickPrefab, position, Quaternion.identity);
            chicks.Add(chick);
        }
    }

    void LoadCowPositions()
    {
        for (int i = 0; ; i++)
        {
            if (!PlayerPrefs.HasKey("Cow" + i + "X"))
                break;

            float x = PlayerPrefs.GetFloat("Cow" + i + "X");
            float y = PlayerPrefs.GetFloat("Cow" + i + "Y");
            Vector3 position = new Vector3(x, y, -1);
            GameObject cow = Instantiate(cowPrefab, position, Quaternion.identity);
            cows.Add(cow);
        }
    }

    public void FruitSwitch()
    {
        if (fruitType == "Wheat")
        {
            fruitType = "Fruit";
            wheatButton.interactable = true;
            fruitButton.interactable = false;
        }
        else
        {
            fruitType = "Wheat";
            wheatButton.interactable = false;  // Disable the button to indicate it's selected
            fruitButton.interactable = true;   // Enable the other button
        }
    }

}
