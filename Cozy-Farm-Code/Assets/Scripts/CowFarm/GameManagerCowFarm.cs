using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManagerCowFarm : MonoBehaviour
{
    public static GameManagerCowFarm Instance; // Singleton instance
    public int coinCount = 0;


     public int milkCount = 0;
    public int workerCost = 20; // Cost to buy a worker
    public int cowCost = 10;

    public TextMeshProUGUI coinText;

    public TextMeshProUGUI buyText;

    public TextMeshProUGUI milkText;

     public TextMeshProUGUI workerText;

    public GameObject cowPrefab;
    public GameObject milkPrefab;
    public GameObject workerPrefab; // Reference to the worker chicken prefab

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        coinText.text = "Coins: " + coinCount;
        milkText.text = "Milk Bottles: " + milkCount;
    }

    public void AddCoin()
    {
        coinCount++;
        UpdateCoinUI();
    }

    public void SpendCoins(int amount)
    {
        coinCount -= amount;
        UpdateCoinUI();
    }

    void UpdateCoinUI()
    {
        coinText.text = "Coins: " + coinCount;
    }

    

     public void CollectMilk(GameObject milk)
    {
        milkCount++;
        UpdateMilkUI();
        Destroy(milk);
    }

  

    void UpdateMilkUI()
    {
        milkText.text = "Milk Bottles: " + milkCount;
    }

    public void SellMilk()
    {
        coinCount += milkCount;
        milkCount = 0;
        UpdateCoinUI();
        UpdateMilkUI();
    }

    

    public void BuyCow()
    {
        
            if (coinCount >= cowCost)
            {
                coinCount -= cowCost;
                cowCost += 10;

                buyText.text = "Buy Cow <br> Cost: " + cowCost;

               
               
                AddCow();
            }
       
    }

    public void AddCow()
    {
        Vector3 cowPosition = GetRandomPosition();
        GameObject cow = Instantiate(cowPrefab, cowPosition, Quaternion.identity);
    }

    public void BuyWorker()
    {
        if (coinCount >= workerCost)
        {
            coinCount -= workerCost;
            workerCost += 10;

                workerText.text = "Buy Worker <br> Cost: " + workerCost;
            UpdateCoinUI();
            // Instantiate the worker chicken at a designated position
            Instantiate(workerPrefab, GetRandomPosition(), Quaternion.identity);
        }
    }

    Vector3 GetRandomPosition()
    {
        float minX = GameObject.Find("LeftBoundary").transform.position.x + 0.5f;
        float maxX = GameObject.Find("RightBoundary").transform.position.x - 0.5f;
        float minY = GameObject.Find("BottomBoundary").transform.position.y + 0.5f;
        float maxY = GameObject.Find("TopBoundary").transform.position.y - 0.5f;

        return new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), -1);
    }
}
