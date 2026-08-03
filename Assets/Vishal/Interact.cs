using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interact : MonoBehaviour
{
    [SerializeField] float interactRadius;
    [SerializeField] Collider[] nearbyItems;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] InputActionReference interactReference;
    [SerializeField] Transform stoneHoldPoint;
    [SerializeField] float interacting;
    GameObject itemInHand;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        interacting = interactReference.action.ReadValue<float>();
        nearbyItems=Physics.OverlapSphere(transform.position, interactRadius,interactableLayer);
        if (itemInHand)
        {
            itemInHand.transform.position = stoneHoldPoint.position;
        }
    }
    private void OnEnable()
    {
        interactReference.action.started += Pickup;
    }
    void Pickup(InputAction.CallbackContext obj)
    {
        if(nearbyItems.Length==0) return;
        if (itemInHand) return;
        itemInHand = nearbyItems[0].gameObject;

    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
