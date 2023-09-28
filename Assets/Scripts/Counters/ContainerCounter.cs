using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ContainerCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabbedObject;

    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            if (!player.HasKitchenObject())
            {
                KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);

                InteractLogicServerRpc();
            }
            else if(player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                KitchenObject.SpawnKitchenObject(kitchenObjectSO, this);
                if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                {
                    KitchenObject.DestroyKitchenObject(GetKitchenObject());

                    InteractLogicServerRpc();
                }
                else
                {
                    KitchenObject.DestroyKitchenObject(GetKitchenObject());
                }
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }
    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
    }
}
