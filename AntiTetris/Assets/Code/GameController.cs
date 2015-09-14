using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class GameController : MonoBehaviour
    {
      public GameObject[] Blocks;
      public Vector3 SpawnValues;
      public float Speed;
      public float FallSpeed;
      public float TurnSpeed;
      public LayerMask blocksCollisionLayer;
      public bool ShowDebug;
      public GameObject debugHelper;

      private bool _gameOver = false;
      private int _wallLength;
      private int _score;
      private Transform _wallTransform;
      private Vector3 _directionVector;
      private Text _scoreText;
      private GameObject _gameOverText;
      private GameObject _lineDebugParent;
      private GameObject _restartButton;
      private GameObject _nextBlockLocation;
      private GameObject _nextBlock;
      private List<Text> _lineDebugTexts;
      private bool _canSpawn = true;
      private int _nextBlockNumber;

      private void Awake()
      {
        var wall = GameObject.FindGameObjectWithTag("Wall");
        _wallLength = Mathf.FloorToInt(wall.transform.localScale.y);
        _wallTransform = wall.transform;
        _directionVector = Vector3.right * 11;
        _gameOverText = GameObject.FindGameObjectWithTag("GameOverText");
        _scoreText = GameObject.FindGameObjectWithTag("ScoreText").GetComponent<Text>();
        _lineDebugParent = GameObject.FindGameObjectWithTag("Player");
        _restartButton = GameObject.FindGameObjectWithTag("RestartButton");
        _nextBlockLocation = GameObject.FindGameObjectWithTag("NextBlock");
        _restartButton.GetComponent<Button>().onClick.AddListener(() => Application.LoadLevel(Application.loadedLevel));
        

        //init debug lines..
        if (debugHelper != null && ShowDebug)
        {
          _lineDebugTexts = new List<Text>();
          for (int i = 1; i < _wallLength; i++)
          {
            var newLine = Instantiate(debugHelper, new Vector3(-164, i * 37 - 310, 0), Quaternion.identity) as GameObject;
            var text = newLine.GetComponent<Text>();
            text.text = "00";
            text.gameObject.transform.SetParent(_lineDebugParent.transform, false);
            _lineDebugTexts.Add(text);
          }
        }

        _nextBlockNumber = Random.Range(0, Blocks.Length - 1);
      }
      
      private void Start ()
      {
          SpawnBlock();
      }
	
      public void SpawnBlock()
      {
        if (_gameOver || !_canSpawn)
          return;
        var spawnPosition = new Vector3(Random.Range(-SpawnValues.x, SpawnValues.x), SpawnValues.y, SpawnValues.z);
        var spawnRotation = Quaternion.identity;
        
        var spawnBlock = Instantiate(Blocks[_nextBlockNumber], spawnPosition, spawnRotation) as GameObject;
        _nextBlockNumber = Random.Range(0, Blocks.Length);
        var blockController = spawnBlock.GetComponent<BlockController>();

        blockController.RunOnCollisionEnter.AddListener(SpawnBlock);
        blockController.OnGameOver.AddListener(GameOver);

        SpawnNextBlock();
      }

      private void SpawnNextBlock()
      {
        if (_nextBlock != null)
        {
          Destroy(_nextBlock.gameObject);
        }

        _nextBlock = Instantiate(Blocks[_nextBlockNumber], _nextBlockLocation.transform.position, Quaternion.identity) as GameObject;
        _nextBlock.GetComponent<BlockController>().playerControlled = false;
      }

      private void GameOver()
      {
        if (_gameOver)
          return;
        _gameOver = true;
        _gameOverText.GetComponent<Text>().enabled = true;
        if (_score >= 100)
        {
          _gameOverText.GetComponent<Text>().text += "\n" + "You are a monster!";
        }
      }

      private void FixedUpdate()
      {
        for (int i = 1; i < _wallLength; i ++)
        {
          var checkLineVector = new Vector3(-Mathf.Abs(_wallTransform.position.x), i, 0);
          //Debug.DrawRay(checkLineVector, _directionVector, Color.red);

          var ray = new Ray(checkLineVector, _directionVector);
          var hits = Physics.RaycastAll(ray, 25f, blocksCollisionLayer);

          if(_lineDebugTexts!= null) _lineDebugTexts[i-1].text = hits.Count().ToString("00");

          if (hits.Count() >= 11 && !ContainsBlockControlledByPlayer(hits))
          {
            foreach (var hit in hits)
            {
              AddScore(1);
              //var comp = hit.collider.gameObject.com<Rigidbody>();
              foreach (Transform child in hit.transform)
              {
                child.gameObject.AddComponent<Rigidbody>();
              }
              if (hit.collider.gameObject.transform.root != null)
              {
                hit.collider.gameObject.transform.root.DetachChildren();
              }
              Destroy(hit.collider.gameObject);
            }
          }
        }
      }

      private bool ContainsBlockControlledByPlayer(RaycastHit[] hits)
      {
        foreach (var hit in hits)
        {
          var blockController = hit.collider.gameObject.GetComponentInParent<BlockController>();
          if (blockController == null)
            continue;
          if (blockController.playerControlled)
            return true;
        }

        return false;
      }

      private void AddScore(int value)
      {
        _score += value;
        _scoreText.text = string.Format("Score: {0:000}", _score);
      }
    }
}
