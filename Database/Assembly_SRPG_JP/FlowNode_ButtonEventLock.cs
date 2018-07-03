﻿// Decompiled with JetBrains decompiler
// Type: SRPG.FlowNode_ButtonEventLock
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 85BFDF7F-5712-4D45-9CD6-3465C703DFDF
// Assembly location: S:\Desktop\Assembly-CSharp.dll

using UnityEngine;

namespace SRPG
{
  [FlowNode.Pin(10, "Out", FlowNode.PinTypes.Output, 10)]
  [AddComponentMenu("")]
  [FlowNode.NodeType("Event/ButtonEventLock", 13156327)]
  [FlowNode.Pin(1, "Lock", FlowNode.PinTypes.Input, 1)]
  [FlowNode.Pin(2, "UnLock", FlowNode.PinTypes.Input, 2)]
  [FlowNode.Pin(3, "Reset", FlowNode.PinTypes.Input, 3)]
  [FlowNode.Pin(4, "AllReset", FlowNode.PinTypes.Input, 3)]
  public class FlowNode_ButtonEventLock : FlowNode
  {
    public string LockKey;

    public override void OnActivate(int pinID)
    {
      switch (pinID)
      {
        case 1:
          ButtonEvent.Lock(this.LockKey);
          break;
        case 2:
          ButtonEvent.UnLock(this.LockKey);
          break;
        case 3:
          ButtonEvent.ResetLock(this.LockKey);
          break;
        case 4:
          ButtonEvent.Reset();
          break;
      }
      this.ActivateOutputLinks(10);
    }
  }
}