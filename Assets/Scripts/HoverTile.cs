using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverTile : MonoBehaviour

{   //the prestige (bale version)
    private Camera cameraComponent;
    public GameObject HoverTileprefab;
    private GameObject instantiatedObject;


    private void Awake()
    {
        instantiatedObject =  Instantiate(HoverTileprefab, new Vector3(0, -0.01f, 0), Quaternion.identity);
        cameraComponent = GameObject.Find("Main Camera").GetComponent<Camera>();
    }
    void Update()
    {
        Ray ray = cameraComponent.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100)
            && !EventSystem.current.IsPointerOverGameObject()
            )
        {
            Vector3 hhc = new Vector3(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.y), Mathf.RoundToInt(hit.point.z));
            instantiatedObject.transform.position = hhc + new Vector3(0, 0.02f, 0);
        }
    }
}




