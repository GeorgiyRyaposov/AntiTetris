using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Code
{
    public class BlockController : MonoBehaviour
    {
      [HideInInspector]
      public UnityEvent RunOnCollisionEnter;
      [HideInInspector]
      public UnityEvent OnGameOver;
      [HideInInspector]
      public bool playerControlled = true;

      private Rigidbody _rigidBody;
      private GameController _gameController;
      private Quaternion _turnBlock;
      private bool _collided;
      private float _timeToSpawnNext = 0.6f;
      private float _elapsedTime = 0.0f;
      private void Awake()
      {
        _rigidBody = GetComponent<Rigidbody>();
        _gameController = GameObject.FindObjectOfType<GameController>();
        _turnBlock = Quaternion.identity;
      }

      private void Update()
      {
        if (!playerControlled)
        {
          return;
        }

        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Q))
          _turnBlock *= Quaternion.Euler(0, 0, 90);

        if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.E))
        {
          _turnBlock *= Quaternion.Euler(0, 0, -90);
        }

        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
          _rigidBody.velocity = _rigidBody.velocity.WithY(Input.GetAxis("Vertical") * 20);
        }

        if(_collided)
        {
          _elapsedTime += Time.deltaTime;
          if (_elapsedTime >= _timeToSpawnNext)
          {
            playerControlled = false;
            RunOnCollisionEnter.Invoke();
          }
        }
      }

      private void FixedUpdate()
      {
          if (!playerControlled)
          {
            return;
          }

          var moveHorizontal = Input.GetAxisRaw("Horizontal");

          _rigidBody.velocity = _rigidBody.velocity.WithX(moveHorizontal * 10);

          var str = Mathf.Min(_gameController.Speed * Time.deltaTime, 1);
          transform.rotation = Quaternion.Lerp(transform.rotation, _turnBlock, str);
      }

      private void OnCollisionEnter(Collision collision)
      {
        if (transform.position.y >= _gameController.SpawnValues.y+1)
        {
          OnGameOver.Invoke();
          return;
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
          return;
        }

        if (!playerControlled)
        {
          return;
        }

      
        _collided = true;
      }
    }

  

    public static class VectorExt
    {

      public static Vector2 WithX(this Vector2 vec, float x) { return new Vector2(x, vec.y); }
      public static Vector2 WithY(this Vector2 vec, float y) { return new Vector2(vec.x, y); }
      public static Vector3 WithZ(this Vector2 vec, float z) { return new Vector3(vec.x, vec.y, z); }

      public static Vector3 WithX(this Vector3 vec, float x) { return new Vector3(x, vec.y, vec.z); }
      public static Vector3 WithY(this Vector3 vec, float y) { return new Vector3(vec.x, y, vec.z); }
      public static Vector3 WithZ(this Vector3 vec, float z) { return new Vector3(vec.x, vec.y, z); }
      public static Vector4 WithW(this Vector3 vec, float w) { return new Vector4(vec.x, vec.y, vec.z, w); }

      public static Vector3 WithXY(this Vector3 vec, float x, float y) { return new Vector3(x, y, vec.z); }
      public static Vector3 WithXZ(this Vector3 vec, float x, float z) { return new Vector3(x, vec.y, z); }
      public static Vector3 WithYZ(this Vector3 vec, float y, float z) { return new Vector3(vec.x, y, z); }

      public static Vector4 WithX(this Vector4 vec, float x) { return new Vector4(x, vec.y, vec.z, vec.w); }
      public static Vector4 WithY(this Vector4 vec, float y) { return new Vector4(vec.x, y, vec.z, vec.w); }
      public static Vector4 WithZ(this Vector4 vec, float z) { return new Vector4(vec.x, vec.y, z, vec.w); }
      public static Vector4 WithW(this Vector4 vec, float w) { return new Vector4(vec.x, vec.y, vec.z, w); }

      public static Vector4 WithXY(this Vector4 vec, float x, float y) { return new Vector4(x, y, vec.z, vec.w); }
      public static Vector4 WithXZ(this Vector4 vec, float x, float z) { return new Vector4(x, vec.y, z, vec.w); }
      public static Vector4 WithXW(this Vector4 vec, float x, float w) { return new Vector4(x, vec.y, vec.z, w); }
      public static Vector4 WithYZ(this Vector4 vec, float y, float z) { return new Vector4(vec.x, y, z, vec.w); }
      public static Vector4 WithYW(this Vector4 vec, float y, float w) { return new Vector4(vec.x, y, vec.z, w); }
      public static Vector4 WithZW(this Vector4 vec, float z, float w) { return new Vector4(vec.x, vec.y, z, w); }

      public static Vector4 WithXYZ(this Vector4 vec, float x, float y, float z) { return new Vector4(x, y, z, vec.w); }
      public static Vector4 WithXYW(this Vector4 vec, float x, float y, float w) { return new Vector4(x, y, vec.z, w); }
      public static Vector4 WithXZW(this Vector4 vec, float x, float z, float w) { return new Vector4(x, vec.y, z, w); }
      public static Vector4 WithYZW(this Vector4 vec, float y, float z, float w) { return new Vector4(vec.x, y, z, w); }

    }
}
