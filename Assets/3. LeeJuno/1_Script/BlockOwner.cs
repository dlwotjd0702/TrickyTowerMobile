using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

[RequireComponent(typeof(NetworkObject))]
public class BlockOwner : NetworkBehaviour
{
 public PlayerRef Owner => Object.InputAuthority;
}