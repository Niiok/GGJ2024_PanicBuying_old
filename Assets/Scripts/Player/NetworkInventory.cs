using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Netcode;

namespace PanicBuying
{
    public class NetworkInventory : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsClient)
            {
                RequestInventory_ServerRpc();
            }
        }

        bool TryGetItem(ref ItemData InData)
        {
            for (int i = _inventory.Items.Length - 1; i > 0; --i)
            {
                ref ItemData ExistingItem = ref _inventory.Items[i];

                if (ExistingItem.Accumulate(ref InData) == false)
                {
                    continue;
                }
            }

            return InData.Count <= 0;
        }


        [ServerRpc]
        void RequestInventory_ServerRpc(ServerRpcParams serverRpcParams = new())
        {
            if (serverRpcParams.Receive.SenderClientId == OwnerClientId)
            {
                var Params = PanicUtil.MakeClientRpcParams(OwnerClientId);

                SendInventory_ClientRpc(_inventory, Params);
            }
        }

        [ClientRpc]
        void SendInventory_ClientRpc(InventoryStruct inventory, ClientRpcParams clientRpcParams)
        {
            _inventory = inventory;
        }


        protected InventoryStruct _inventory = new();
    }


    public struct InventoryStruct : INetworkSerializeByMemcpy
    {
        public InventoryStruct(int SlotNumber = 5)
        {
            Items = new ItemData[SlotNumber];
        }

        public ItemData[] Items;
    }
}
