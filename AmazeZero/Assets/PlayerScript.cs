using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerScript : MonoBehaviour
{
    public Tilemap Tilemap = null;
    private static Movements _movements;

    private Rigidbody _body;
    private Camera _camera;

    // Start is called before the first frame update
    private void Start()
    {
        _body = GetComponent<Rigidbody>();
        _camera = Camera.main;
        _body.freezeRotation = true;
    }

    private void FixedUpdate()
    {


    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            _movements.Push = true;
        }
        _movements.EastWest1 = (Input.GetKey(KeyCode.LeftArrow) ? -1 : 0) + (Input.GetKey(KeyCode.RightArrow) ? 1 : 0);
        _movements.NorthSouth1 = (Input.GetKey(KeyCode.UpArrow) ? 1 : 0) + (Input.GetKey(KeyCode.DownArrow) ? -1 : 0);


    }
}
