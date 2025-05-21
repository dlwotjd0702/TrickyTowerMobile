using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public static class SurvivalEvents
{
 public static event Action<PlayerRef> BlockSpawned;
 public static event Action<PlayerRef> BlockDestroyed;
 
 public static void Spawned(PlayerRef p) => BlockSpawned?.Invoke(p);
 public static void Destroyed(PlayerRef p) => BlockDestroyed?.Invoke(p);
}
