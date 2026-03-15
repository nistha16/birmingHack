using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

public class StoryManager : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI answer1Text;
    public TextMeshProUGUI answer2Text;
    public TextMeshProUGUI answer3Text;
    public GameObject questionPanel;
    public float baseQuestionDistance = 50f;

    private StoryData storyData;
    private int currentScenario = 0;
    private float nextQuestionX;
    private bool showingQuestion = false;
    private bool showingIntro = true;
    private int introLine = 0;
    private GameObject currentCloud;
    private List<int> answerOrder = new List<int>();
    private bool waitingToContinue = false;
    private bool cloudTriggered = false;

    private InputAction key1;
    private InputAction key2;
    private InputAction key3;
    private InputAction spaceKey;

    void Start()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("storyline");
        storyData = JsonUtility.FromJson<StoryData>(jsonFile.text);

        key1 = new InputAction("Key1", binding: "<Keyboard>/1");
        key2 = new InputAction("Key2", binding: "<Keyboard>/2");
        key3 = new InputAction("Key3", binding: "<Keyboard>/3");
        spaceKey = new InputAction("Space", binding: "<Keyboard>/space");
        key1.Enable();
        key2.Enable();
        key3.Enable();
        spaceKey.Enable();

        questionPanel.SetActive(false);
        nextQuestionX = baseQuestionDistance;
        ShowIntro();
    }

    void ShowIntro()
    {
        showingIntro = true;
        Time.timeScale = 0.001f;
        questionPanel.SetActive(true);
        questionText.text = storyData.storyline[introLine] + "\n\n[Space to continue]";
        answer1Text.text = "";
        answer2Text.text = "";
        answer3Text.text = "";
    }

    void Update()
    {
        // use Keyboard.current for paused state (works when timeScale = 0)
        var kb = UnityEngine.InputSystem.Keyboard.current;
        if (kb == null) return;

        // intro sequence
        if (showingIntro)
        {
            if (kb.spaceKey.wasPressedThisFrame)
            {
                introLine++;
                if (introLine < storyData.storyline.Length)
                {
                    questionText.text = storyData.storyline[introLine] + "\n\n[Space to continue]";
                }
                else
                {
                    showingIntro = false;
                    questionPanel.SetActive(false);
                    Time.timeScale = 1f;
                    SpawnCloud();
                }
            }
            return;
        }

        // waiting after answering
        if (waitingToContinue)
        {
            if (kb.spaceKey.wasPressedThisFrame)
            {
                waitingToContinue = false;

                if (currentScenario >= storyData.scenarios.Length)
                {
                    questionText.text = "Alex reaches the edge of the city.\nThe truth was never a destination.\nIt was the journey of questioning everything.";
                    answer1Text.text = "";
                    answer2Text.text = "";
                    answer3Text.text = "";
                    return;
                }

                questionPanel.SetActive(false);
                Time.timeScale = 1f;
                SpawnCloud();
            }
            return;
        }

        // handle answer input when question is showing
        if (showingQuestion)
        {
            if (kb.digit1Key.wasPressedThisFrame) PickAnswer(0);
            if (kb.digit2Key.wasPressedThisFrame) PickAnswer(1);
            if (kb.digit3Key.wasPressedThisFrame) PickAnswer(2);
        }
    }

    void SpawnCloud()
    {
        if (currentScenario >= storyData.scenarios.Length) return;

        float spawnX = player.position.x + nextQuestionX;
        float spawnY = player.position.y - 1.5f;
        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0f);

        cloudTriggered = false;

        // create small "?" circle
        currentCloud = new GameObject("QuestionCloud");
        currentCloud.transform.position = spawnPos;
        currentCloud.tag = "QuestionCloud";

        // balloon circle
        SpriteRenderer sr = currentCloud.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.85f, 0.85f, 0.9f, 1f);
        Texture2D tex = new Texture2D(64, 64);
        for (int x = 0; x < 64; x++)
        {
            for (int y = 0; y < 64; y++)
            {
                float dx = x - 32f;
                float dy = y - 32f;
                if (dx * dx + dy * dy < 30f * 30f)
                    tex.SetPixel(x, y, Color.white);
                else
                    tex.SetPixel(x, y, Color.clear);
            }
        }
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64f);
        currentCloud.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        sr.sortingOrder = 5;

        // string hanging down from balloon
        GameObject stringObj = new GameObject("BalloonString");
        stringObj.transform.SetParent(currentCloud.transform);
        stringObj.transform.localPosition = new Vector3(0f, -1.5f, 0f);
        LineRenderer lr = stringObj.AddComponent<LineRenderer>();
        lr.startWidth = 0.03f;
        lr.endWidth = 0.03f;
        lr.positionCount = 2;
        lr.useWorldSpace = false;
        lr.SetPosition(0, new Vector3(0f, 1.5f, 0f));
        lr.SetPosition(1, new Vector3(0f, -0.5f, 0f));
        lr.startColor = Color.black;
        lr.endColor = Color.black;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.sortingOrder = 5;

        // "Ready for Q?" text
        GameObject textObj = new GameObject("CloudLabel");
        textObj.transform.SetParent(currentCloud.transform);
        textObj.transform.localPosition = Vector3.zero;
        TextMesh tm = textObj.AddComponent<TextMesh>();
        tm.text = "Ready\nfor Q?";
        tm.characterSize = 0.03f;
        tm.fontSize = 40;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = Color.black;

        // trigger collider - bigger so easier to hit
        BoxCollider2D col = currentCloud.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(3f, 3f);
    }

    // called from PlayerMovement when player hits the cloud trigger
    public void OnCloudTriggered()
    {
        if (cloudTriggered || currentScenario >= storyData.scenarios.Length) return;
        cloudTriggered = true;
        ShowQuestion();
    }

    void ShowQuestion()
    {
        showingQuestion = true;
        Time.timeScale = 0.001f;

        Scenario s = storyData.scenarios[currentScenario];

        // shuffle answers
        answerOrder.Clear();
        answerOrder.Add(0);
        answerOrder.Add(1);
        answerOrder.Add(2);
        for (int i = answerOrder.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = answerOrder[i];
            answerOrder[i] = answerOrder[j];
            answerOrder[j] = temp;
        }

        questionPanel.SetActive(true);
        questionText.text = s.scenario + "\n\n" + s.question;
        answer1Text.text = "1: " + s.possible_answers[answerOrder[0]].response;
        answer2Text.text = "2: " + s.possible_answers[answerOrder[1]].response;
        answer3Text.text = "3: " + s.possible_answers[answerOrder[2]].response;

        if (currentCloud != null) Destroy(currentCloud);
    }

    void PickAnswer(int choice)
    {
        Scenario s = storyData.scenarios[currentScenario];
        int actualIndex = answerOrder[choice];
        bool correct = s.possible_answers[actualIndex].correct;

        if (correct)
        {
            questionText.text = "Correct! You saw through the misinformation.\n\n[Space to continue]";
            nextQuestionX = baseQuestionDistance * 1.5f;
        }
        else
        {
            questionText.text = "Wrong! The misinformation got to you.\n\n[Space to continue]";
            nextQuestionX = baseQuestionDistance * 0.5f;
        }

        answer1Text.text = "";
        answer2Text.text = "";
        answer3Text.text = "";

        currentScenario++;
        showingQuestion = false;
        waitingToContinue = true;
    }

    void OnDisable()
    {
        key1.Disable();
        key2.Disable();
        key3.Disable();
        spaceKey.Disable();
        Time.timeScale = 1f;
    }
}

[System.Serializable]
public class StoryData
{
    public string[] storyline;
    public Scenario[] scenarios;
}

[System.Serializable]
public class Scenario
{
    public string scenario;
    public string question;
    public PossibleAnswer[] possible_answers;
}

[System.Serializable]
public class PossibleAnswer
{
    public bool correct;
    public string response;
}