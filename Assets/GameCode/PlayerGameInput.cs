using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGameInput : MonoBehaviour
{
    [SerializeField]
    Camera mainCam;
    [SerializeField]
    string maskLayerTarget;
    int layerId;
    RaycastHit hitInfo;
    Ray ray;
    Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        layerId = LayerMask.GetMask(maskLayerTarget);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            ray = mainCam.ScreenPointToRay(Input.mousePosition);
            pos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            if (Physics.Raycast(ray, out hitInfo, layerId))
            {
                if (hitInfo.collider.gameObject.TryGetComponent(out GameInputHelper iOut))
                {
                    iOut.Interact(pos);
                }
            }
        }
    }
}
