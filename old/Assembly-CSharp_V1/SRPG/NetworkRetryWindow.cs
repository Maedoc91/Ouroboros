﻿// Decompiled with JetBrains decompiler
// Type: SRPG.NetworkRetryWindow
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9BA76916-D0BD-4DB6-A90B-FE0BCC53E511
// Assembly location: C:\Users\André\Desktop\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SRPG
{
  public class NetworkRetryWindow : MonoBehaviour
  {
    public NetworkRetryWindow.RetryWindowEvent Delegate;
    public Text Title;
    public Text Message;
    public Button ButtonOk;
    public Button ButtonCancel;

    public NetworkRetryWindow()
    {
      base.\u002Ector();
    }

    private void Awake()
    {
      if (Object.op_Inequality((Object) this.ButtonOk, (Object) null))
      {
        // ISSUE: method pointer
        ((UnityEvent) this.ButtonOk.get_onClick()).AddListener(new UnityAction((object) this, __methodptr(OnOk)));
      }
      if (!Object.op_Inequality((Object) this.ButtonCancel, (Object) null))
        return;
      // ISSUE: method pointer
      ((UnityEvent) this.ButtonCancel.get_onClick()).AddListener(new UnityAction((object) this, __methodptr(OnCancel)));
    }

    private void Start()
    {
      if (!Object.op_Inequality((Object) this.Title, (Object) null))
        return;
      this.Title.set_text(LocalizedText.Get("embed.CONN_RETRY"));
    }

    public string Body
    {
      set
      {
        this.Message.set_text(value);
      }
      get
      {
        return this.Message.get_text();
      }
    }

    private void OnOk()
    {
      this.Delegate(true);
    }

    private void OnCancel()
    {
      this.Delegate(false);
    }

    public delegate void RetryWindowEvent(bool retry);
  }
}