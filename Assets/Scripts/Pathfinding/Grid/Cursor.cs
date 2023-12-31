using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Cursor : MonoBehaviour
{
    public static Cursor Instance { get; private set; }

    [SerializeField] private Plane plane = new Plane(Vector3.up, 0f);
    
    private Camera cam;
    [SerializeField] private Vector3 currentMousePos;
    [SerializeField] private Vector3 mousePosWorld;
    [SerializeField] private Vector2 visibleRegion;

    [SerializeField] private Vector3 lastmousePosWorld;
    [SerializeField] private float lerpSpeed = 0.1f;
    [SerializeField] private float lerpGradient = 0.1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        currentMousePos = Input.mousePosition;

        Ray ray = cam.ScreenPointToRay(currentMousePos);

        if (plane.Raycast(ray, out float distance))
        {
            mousePosWorld = ray.GetPoint(distance);
        }

        mousePosWorld.x = Mathf.Floor(mousePosWorld.x);
        mousePosWorld.z = Mathf.Floor(mousePosWorld.z);


        transform.position = Vector3.Lerp(transform.position, mousePosWorld, Mathf.Pow(lerpSpeed * Time.deltaTime, lerpGradient));

        if (IsOutBound || GridNodeEditor.Instance.Mode == GridNodeEditor.EditMode.NONE || !GridNodeEditor.Instance.isActive)
        {
            VisibilityOff();
        }
        else
        {
            VisibilityOn();
        }
    }

    public void VisibilityOff()
    {
        gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
    }

    public void VisibilityOn()
    {
        gameObject.GetComponentInChildren<MeshRenderer>().enabled = true;
    }

    public Vector2 VisibleRegion
    {
        set
        {
            visibleRegion = value;
        }
    }

    public bool IsOutBound
    {
        get { return mousePosWorld.x < 0 || mousePosWorld.x >= visibleRegion.x || mousePosWorld.z < 0 || mousePosWorld.z >= visibleRegion.y; }
    }

    public Vector2Int GetMouseNodePos {
        get 
        { 
            return new Vector2Int(Mathf.FloorToInt(mousePosWorld.x), Mathf.FloorToInt(mousePosWorld.z)) ; 
        } 
    }
}
