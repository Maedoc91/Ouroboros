﻿// Decompiled with JetBrains decompiler
// Type: SRPG.DirectionArrow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 85BFDF7F-5712-4D45-9CD6-3465C703DFDF
// Assembly location: S:\Desktop\Assembly-CSharp.dll

using UnityEngine;

namespace SRPG
{
  public class DirectionArrow : MonoBehaviour
  {
    public DirectionArrow.ArrowStates State;
    public EUnitDirection Direction;
    private Animator mAnimator;
    [HelpBox("方向の選択状態にあわせてAnimatorのStateNameを変更します (0=Normal,1=Press,2=Hilit,3=Close)。矢印はアニメーションが停止したら破棄されるので、PressとClose状態以外はループアニメーションにしてください。")]
    public string StateName;

    public DirectionArrow()
    {
      base.\u002Ector();
    }

    private void Start()
    {
      this.mAnimator = (Animator) ((Component) this).GetComponent<Animator>();
    }

    private void Update()
    {
      if (Object.op_Equality((Object) this.mAnimator, (Object) null))
        return;
      this.mAnimator.SetInteger(this.StateName, (int) this.State);
      AnimatorStateInfo animatorStateInfo1 = this.mAnimator.GetCurrentAnimatorStateInfo(0);
      // ISSUE: explicit reference operation
      if (((AnimatorStateInfo) @animatorStateInfo1).get_loop())
        return;
      AnimatorStateInfo animatorStateInfo2 = this.mAnimator.GetCurrentAnimatorStateInfo(0);
      // ISSUE: explicit reference operation
      if ((double) ((AnimatorStateInfo) @animatorStateInfo2).get_normalizedTime() < 1.0 || this.mAnimator.IsInTransition(0))
        return;
      Object.Destroy((Object) ((Component) this).get_gameObject());
    }

    public enum ArrowStates
    {
      Normal,
      Press,
      Hilit,
      Close,
    }
  }
}