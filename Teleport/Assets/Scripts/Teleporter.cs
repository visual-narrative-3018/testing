using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour {

    public GameObject TeleportMarker;
    public Transform Player;
    public float RayLength = 50f;

    // Use this for initialization
    void Start()
    {
        Debug.Log("Started");
        transform.position = Player.position;
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, RayLength)) {
            // Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.blue, 10);
            if (hit.collider.tag == "Ground") {
                if (!TeleportMarker.activeSelf) {
                    TeleportMarker.SetActive(true);
                }
                TeleportMarker.transform.position = hit.point;
            } else {
                TeleportMarker.SetActive(false);
            }
        } else {
            // Debug.DrawRay(transform.position, transform.forward * 1000, Color.red, 10);
            TeleportMarker.SetActive(false);
        }
        if (Input.GetMouseButtonDown(0)) //OVRInput.GetDown(OVRInput.Button.One)
        {
            Debug.Log("Click");
            // check where the user is looking to teleport
            if (TeleportMarker.activeSelf)
            {
                Vector3 markerPosition = TeleportMarker.transform.position;
                Player.position = new Vector3(markerPosition.x, Player.position.y, markerPosition.z);
            }
        }
	}
}
