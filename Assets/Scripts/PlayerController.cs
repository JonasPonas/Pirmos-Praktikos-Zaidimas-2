using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InventoryItem
{
    public GroundTile tile;
    public string name;
    public int amount = 1;

    public InventoryItem() { }
    public InventoryItem(string name, GroundTile tile) { this.name = name; this.tile = tile; }
}

public class PlayerController : MonoBehaviour
{

    public Sprite enhanced;
    private bool upgraded = false;

    public float speed;
    private Rigidbody2D rb;
    private Vector2 moveVelocity;

    private float timerV, timerH;

    public GameObject canvas;
    public Collider2D bottomCollider, leftCollider, rightCollider;

    private List<Collider2D> colliders;

    private bool isMoving = true;
    private bool busy = false;

    private Text invValText, messageText;
    private bool m_Fading = false;
    private float fadeOutTime = 50;
    private Coroutine fadeOutRoutine;

    public List<InventoryItem> inventory;
    public Text[] invTexts;
    private int invVal = 0;

    private GameObject invPanel;

    public GameObject shopPanel;
    private Button sellAllBtn, buyBtn, fuelBtn;

    public int fuelPrice = 100;
    public float fuelEfficency = -0.0013f; // -0.002 - hard //-0.001 - easy

    public GameObject gameOver;
    private bool gameOverB = false;

    public Shop shop;

    [System.NonSerialized]
    public int cash = 0;
    public float fuel = 1f;

    public float digSpeed = 1f;

    public int score = 0;

    public PlayerStats stats;

    // Use this for initialization
    void Start()
    {
        fuelPrice = stats.fuelPrice;
        fuelEfficency = stats.fuelEfficency;

        rb = GetComponent<Rigidbody2D>();
        timerV = Time.time;
        timerH = Time.time;

        colliders = new List<Collider2D>();
        inventory = new List<InventoryItem>();

        invValText = canvas.transform.Find("InvVal").GetComponent<Text>();
        messageText = canvas.transform.Find("NewitemMessage").GetComponent<Text>();
        messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, 0);

        invPanel = canvas.transform.Find("InventoryPanel").gameObject;

        fadeOutRoutine = StartCoroutine(FadeOutRoutine());
        StopCoroutine(fadeOutRoutine);

        buyBtn = shopPanel.transform.GetChild(0).GetComponentInChildren<Button>();
        sellAllBtn = shopPanel.transform.GetChild(1).GetComponent<Button>();
        fuelBtn = shopPanel.transform.GetChild(2).GetComponent<Button>();

        sellAllBtn.onClick.AddListener(AddCash);
        buyBtn.onClick.AddListener(BuyUpgrade);
        fuelBtn.onClick.AddListener(BuyFuel);
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameOverB)
            InputHandler();

        if (!busy)
        {
            Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (fuel <= 0 && moveInput.y > 0) moveInput.y = 0;
            moveVelocity = moveInput.normalized * speed;

            if (moveInput.y > 0)
            {
                fuel += fuelEfficency;
                if (fuel < 0)
                {
                    fuel = 0;
                    gameOver.SetActive(true);
                    gameOverB = true;
                    busy = true;
                    if (score > PlayerPrefs.GetInt("HighScore", 0))
                        PlayerPrefs.SetInt("HighScore", score);
                }
            }
            fuelBtn.GetComponentInChildren<Text>().text = "Buy fuel(" + (int)Mathf.Lerp(fuelPrice, 0, fuel) + "$)";

            if (isMoving)
            {
                timerV = Time.time;
                timerH = Time.time;
            }
            else
            {
                if (Input.GetAxisRaw("Horizontal") == 0)
                    timerH = Time.time;
                if (Input.GetAxisRaw("Vertical") >= 0)
                    timerV = Time.time;
            }

            //Debug.Log(Time.time - timerV);
            if (Time.time - timerV >= digSpeed || Time.time - timerH >= digSpeed)
            {
                GroundDigging();
                timerV = Time.time;
                timerH = Time.time;
            }
        }
    }

    void FixedUpdate()
    {
        if (!busy)
        {
            rb.isKinematic = false;
        }
        else
        {
            moveVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        Vector2 prevPos = rb.position;
        rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);

        if (rb.position == prevPos)
            isMoving = false;
        else
            isMoving = true;
    }

    void GroundDigging()
    {
        foreach (Collider2D collider in colliders)
        {
            Collider2D current = null;
            if (Input.GetAxisRaw("Vertical") < 0 && collider.IsTouching(bottomCollider) && Time.time - timerV >= digSpeed)
                current = collider;
            else if (Input.GetAxisRaw("Horizontal") < 0 && collider.IsTouching(leftCollider) && Time.time - timerH >= digSpeed)
                current = collider;
            else if (Input.GetAxisRaw("Horizontal") > 0 && collider.IsTouching(rightCollider) && Time.time - timerH >= digSpeed)
                current = collider;

            if (current != null)
            {
                GroundTile colidedTile = null;
                if (collider.gameObject.GetComponent<Tile>() != null)
                    colidedTile = collider.gameObject.GetComponent<Tile>().tile;
                else break;

                if (colidedTile.name == "BedRock" && !upgraded) break;
                if (colidedTile.name == "Lava")
                {
                    gameOverB = true;
                    gameOver.SetActive(true);
                    gameOver.GetComponentInChildren<Text>().text = "GameOver\n(Victory!)";
                    busy = true;
                    break;
                }
                // cash += colidedTile.value;
                collider.gameObject.SetActive(false);
                //invValText.text = "Inv Value: " + score;
                messageText.text = "+ " + colidedTile.name;
                colliders.Remove(collider);
                timerV = Time.time;
                timerH = Time.time;

                messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b);
                //Debug.Log("Fad out started!");
                StopCoroutine(fadeOutRoutine);
                fadeOutRoutine = StartCoroutine(FadeOutRoutine());

                if (inventory.Exists(x => x.name == colidedTile.name))
                    inventory.Find(x => x.name == colidedTile.name).amount++;
                else
                    inventory.Add(new InventoryItem(colidedTile.name, colidedTile));

                invVal = 0;
                for (int i = 0; i < inventory.Count; i++)
                {
                    invTexts[i].gameObject.SetActive(true);
                    invTexts[i].text = inventory[i].name + ": " + inventory[i].amount + " (" + inventory[i].amount * inventory[i].tile.value + "$)";
                    invVal += inventory[i].amount * inventory[i].tile.value;
                    /*Debug.Log(invPanel.transform.GetChild(invPanel.transform.childCount - 1));
                    GameObject txG = Instantiate(invPanel.transform.GetChild(invPanel.transform.childCount - 1).gameObject);
                    txG.SetActive(true);
                    Text myText = txG.GetComponent<Text>();
                    myText.text = "Ta-dah!";
                    if (i != 0)
                    {
                        txG.transform.SetParent(invPanel.transform, false);
                        txG.transform.localPosition -= new Vector3(0, i * 57, 0);

                        //Debug.Log(txG.transform.lossyScale.y);
                    }*/
                }
                invPanel.transform.GetChild(invPanel.transform.childCount - 1).GetComponent<Text>().text = "Total Value: " + invVal + "$";

                //foreach (InventoryItem item in inventory)
                //Debug.Log(item.name + ": " + item.amount);

                fuel += fuelEfficency * 30;
                score++;
                canvas.transform.Find("Score").GetComponent<Text>().text = score.ToString();

                break;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D Other)
    {
        colliders.Add(Other);
        timerV = Time.time;
        timerH = Time.time;
        //Debug.Log(colliders.Count);
        //Debug.Log(Other.gameObject.GetComponent<Tile>().tile.name);
    }
    void OnTriggerExit2D(Collider2D Other)
    {
        colliders.Remove(Other);
        timerV = Time.time;
        timerH = Time.time;
        //Debug.Log(colliders.Count);
    }

    void InputHandler()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!SceneManager.GetSceneByName("Menu").isLoaded)
            {
                SceneManager.LoadScene("Menu", LoadSceneMode.Additive);
                busy = true;
            }
            else
            {
                SceneManager.UnloadSceneAsync("Menu");
                busy = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.F) && shop.openText.activeSelf)
        {
            busy = !busy;
            shopPanel.SetActive(!shopPanel.activeSelf);
        }
    }
    private IEnumerator FadeOutRoutine()
    {
        Color originalColor = messageText.color;
        for (float t = 0.01f; t < fadeOutTime; t += Time.deltaTime)
        {
            messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, Mathf.Lerp(messageText.color.a, 0f, Mathf.Min(1, t / fadeOutTime)));
            //messageText.color = Color.Lerp(originalColor, Color.clear, Mathf.Min(1, t / fadeOutTime));
            //Debug.Log(m_Fading);
            yield return null;
        }
        //yield break;
    }

    void AddCash()
    {
        foreach (InventoryItem item in inventory)
        {
            cash += item.amount * item.tile.value;
        }
        inventory = new List<InventoryItem>();
        invValText.text = "Cash: " + cash + "$";
        invPanel.transform.GetChild(invPanel.transform.childCount - 1).GetComponent<Text>().text = "Total Value: " + 0 + "$";

        foreach (Text text in invTexts)
            text.gameObject.SetActive(false);
    }

    void BuyUpgrade()
    {
        if (cash >= 200)
        {
            digSpeed = 0.5f;
            fuelEfficency /= 2;

            cash -= 200;
            invValText.text = "Cash: " + cash + "$";

            buyBtn.transform.parent.gameObject.SetActive(false);

            GetComponent<SpriteRenderer>().sprite = enhanced;
            upgraded = true;
        }
    }

    void BuyFuel()
    {
        int price = (int)Mathf.Lerp(fuelPrice, 0, fuel);
        if (cash >= price)
        {
            cash -= price;
            fuel = 1;
        }
        else
        {
            //Debug.Log(cash / (float)fuelPrice);
            fuel += cash / (float)fuelPrice;
            cash = 0;
        }
        invValText.text = "Cash: " + cash + "$";
        fuelBtn.GetComponentInChildren<Text>().text = "Buy fuel(" + (int)Mathf.Lerp(fuelPrice, 0, fuel) + "$)";
    }
}
