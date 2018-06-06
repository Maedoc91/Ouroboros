﻿// Decompiled with JetBrains decompiler
// Type: GooglePlayGames.Native.NativeQuestClient
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9BA76916-D0BD-4DB6-A90B-FE0BCC53E511
// Assembly location: C:\Users\André\Desktop\Assembly-CSharp.dll

using GooglePlayGames.BasicApi.Quests;
using GooglePlayGames.Native.Cwrapper;
using GooglePlayGames.Native.PInvoke;
using GooglePlayGames.OurUtils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GooglePlayGames.Native
{
  internal class NativeQuestClient : IQuestsClient
  {
    private readonly GooglePlayGames.Native.PInvoke.QuestManager mManager;

    internal NativeQuestClient(GooglePlayGames.Native.PInvoke.QuestManager manager)
    {
      this.mManager = Misc.CheckNotNull<GooglePlayGames.Native.PInvoke.QuestManager>(manager);
    }

    public void Fetch(GooglePlayGames.BasicApi.DataSource source, string questId, Action<GooglePlayGames.BasicApi.ResponseStatus, IQuest> callback)
    {
      Misc.CheckNotNull<string>(questId);
      Misc.CheckNotNull<Action<GooglePlayGames.BasicApi.ResponseStatus, IQuest>>(callback);
      callback = CallbackUtils.ToOnGameThread<GooglePlayGames.BasicApi.ResponseStatus, IQuest>(callback);
      this.mManager.Fetch(ConversionUtils.AsDataSource(source), questId, (Action<GooglePlayGames.Native.PInvoke.QuestManager.FetchResponse>) (response =>
      {
        GooglePlayGames.BasicApi.ResponseStatus responseStatus = ConversionUtils.ConvertResponseStatus(response.ResponseStatus());
        if (!response.RequestSucceeded())
          callback(responseStatus, (IQuest) null);
        else
          callback(responseStatus, (IQuest) response.Data());
      }));
    }

    public void FetchMatchingState(GooglePlayGames.BasicApi.DataSource source, QuestFetchFlags flags, Action<GooglePlayGames.BasicApi.ResponseStatus, List<IQuest>> callback)
    {
      Misc.CheckNotNull<Action<GooglePlayGames.BasicApi.ResponseStatus, List<IQuest>>>(callback);
      callback = CallbackUtils.ToOnGameThread<GooglePlayGames.BasicApi.ResponseStatus, List<IQuest>>(callback);
      this.mManager.FetchList(ConversionUtils.AsDataSource(source), (int) flags, (Action<GooglePlayGames.Native.PInvoke.QuestManager.FetchListResponse>) (response =>
      {
        GooglePlayGames.BasicApi.ResponseStatus responseStatus = ConversionUtils.ConvertResponseStatus(response.ResponseStatus());
        if (!response.RequestSucceeded())
          callback(responseStatus, (List<IQuest>) null);
        else
          callback(responseStatus, response.Data().Cast<IQuest>().ToList<IQuest>());
      }));
    }

    public void ShowAllQuestsUI(Action<QuestUiResult, IQuest, IQuestMilestone> callback)
    {
      Misc.CheckNotNull<Action<QuestUiResult, IQuest, IQuestMilestone>>(callback);
      callback = CallbackUtils.ToOnGameThread<QuestUiResult, IQuest, IQuestMilestone>(callback);
      this.mManager.ShowAllQuestUI(NativeQuestClient.FromQuestUICallback(callback));
    }

    public void ShowSpecificQuestUI(IQuest quest, Action<QuestUiResult, IQuest, IQuestMilestone> callback)
    {
      Misc.CheckNotNull<IQuest>(quest);
      Misc.CheckNotNull<Action<QuestUiResult, IQuest, IQuestMilestone>>(callback);
      callback = CallbackUtils.ToOnGameThread<QuestUiResult, IQuest, IQuestMilestone>(callback);
      NativeQuest quest1 = quest as NativeQuest;
      if (quest1 == null)
      {
        Logger.e("Encountered quest that was not generated by this IQuestClient");
        callback(QuestUiResult.BadInput, (IQuest) null, (IQuestMilestone) null);
      }
      else
        this.mManager.ShowQuestUI(quest1, NativeQuestClient.FromQuestUICallback(callback));
    }

    private static QuestUiResult UiErrorToQuestUiResult(CommonErrorStatus.UIStatus status)
    {
      switch (status + 12)
      {
        case ~(CommonErrorStatus.UIStatus.ERROR_INTERNAL | CommonErrorStatus.UIStatus.VALID):
          return QuestUiResult.UiBusy;
        case (CommonErrorStatus.UIStatus) 6:
          return QuestUiResult.UserCanceled;
        case (CommonErrorStatus.UIStatus) 7:
          return QuestUiResult.Timeout;
        case (CommonErrorStatus.UIStatus) 8:
          return QuestUiResult.VersionUpdateRequired;
        case (CommonErrorStatus.UIStatus) 9:
          return QuestUiResult.NotAuthorized;
        case ~(CommonErrorStatus.UIStatus.ERROR_UI_BUSY | CommonErrorStatus.UIStatus.VALID):
          return QuestUiResult.InternalError;
        default:
          Logger.e("Unknown error status: " + (object) status);
          return QuestUiResult.InternalError;
      }
    }

    private static Action<GooglePlayGames.Native.PInvoke.QuestManager.QuestUIResponse> FromQuestUICallback(Action<QuestUiResult, IQuest, IQuestMilestone> callback)
    {
      return (Action<GooglePlayGames.Native.PInvoke.QuestManager.QuestUIResponse>) (response =>
      {
        if (!response.RequestSucceeded())
        {
          callback(NativeQuestClient.UiErrorToQuestUiResult(response.RequestStatus()), (IQuest) null, (IQuestMilestone) null);
        }
        else
        {
          NativeQuest nativeQuest = response.AcceptedQuest();
          NativeQuestMilestone claim = response.MilestoneToClaim();
          if (nativeQuest != null)
          {
            callback(QuestUiResult.UserRequestsQuestAcceptance, (IQuest) nativeQuest, (IQuestMilestone) null);
            claim.Dispose();
          }
          else if (claim != null)
          {
            callback(QuestUiResult.UserRequestsMilestoneClaiming, (IQuest) null, (IQuestMilestone) response.MilestoneToClaim());
            nativeQuest.Dispose();
          }
          else
          {
            Logger.e("Quest UI succeeded without a quest acceptance or milestone claim.");
            nativeQuest.Dispose();
            claim.Dispose();
            callback(QuestUiResult.InternalError, (IQuest) null, (IQuestMilestone) null);
          }
        }
      });
    }

    public void Accept(IQuest quest, Action<GooglePlayGames.BasicApi.Quests.QuestAcceptStatus, IQuest> callback)
    {
      Misc.CheckNotNull<IQuest>(quest);
      Misc.CheckNotNull<Action<GooglePlayGames.BasicApi.Quests.QuestAcceptStatus, IQuest>>(callback);
      callback = CallbackUtils.ToOnGameThread<GooglePlayGames.BasicApi.Quests.QuestAcceptStatus, IQuest>(callback);
      NativeQuest quest1 = quest as NativeQuest;
      if (quest1 == null)
      {
        Logger.e("Encountered quest that was not generated by this IQuestClient");
        callback(GooglePlayGames.BasicApi.Quests.QuestAcceptStatus.BadInput, (IQuest) null);
      }
      else
        this.mManager.Accept(quest1, (Action<GooglePlayGames.Native.PInvoke.QuestManager.AcceptResponse>) (response =>
        {
          if (response.RequestSucceeded())
            callback(GooglePlayGames.BasicApi.Quests.QuestAcceptStatus.Success, (IQuest) response.AcceptedQuest());
          else
            callback(NativeQuestClient.FromAcceptStatus(response.ResponseStatus()), (IQuest) null);
        }));
    }

    private static GooglePlayGames.BasicApi.Quests.QuestAcceptStatus FromAcceptStatus(CommonErrorStatus.QuestAcceptStatus status)
    {
      CommonErrorStatus.QuestAcceptStatus questAcceptStatus = status;
      switch (questAcceptStatus + 5)
      {
        case ~(CommonErrorStatus.QuestAcceptStatus.ERROR_INTERNAL | CommonErrorStatus.QuestAcceptStatus.VALID):
          return GooglePlayGames.BasicApi.Quests.QuestAcceptStatus.Timeout;
        case ~CommonErrorStatus.QuestAcceptStatus.ERROR_NOT_AUTHORIZED:
          return GooglePlayGames.BasicApi.Quests.QuestAcceptStatus.NotAuthorized;
        case (CommonErrorStatus.QuestAcceptStatus) 3:
          return GooglePlayGames.BasicApi.Quests.QuestAcceptStatus.InternalError;
        case (CommonErrorStatus.QuestAcceptStatus) 6:
          return GooglePlayGames.BasicApi.Quests.QuestAcceptStatus.Success;
        default:
          if (questAcceptStatus == CommonErrorStatus.QuestAcceptStatus.ERROR_QUEST_NOT_STARTED)
            return GooglePlayGames.BasicApi.Quests.QuestAcceptStatus.QuestNotStarted;
          if (questAcceptStatus == CommonErrorStatus.QuestAcceptStatus.ERROR_QUEST_NO_LONGER_AVAILABLE)
            return GooglePlayGames.BasicApi.Quests.QuestAcceptStatus.QuestNoLongerAvailable;
          Logger.e("Encountered unknown status: " + (object) status);
          return GooglePlayGames.BasicApi.Quests.QuestAcceptStatus.InternalError;
      }
    }

    public void ClaimMilestone(IQuestMilestone milestone, Action<GooglePlayGames.BasicApi.Quests.QuestClaimMilestoneStatus, IQuest, IQuestMilestone> callback)
    {
      Misc.CheckNotNull<IQuestMilestone>(milestone);
      Misc.CheckNotNull<Action<GooglePlayGames.BasicApi.Quests.QuestClaimMilestoneStatus, IQuest, IQuestMilestone>>(callback);
      callback = CallbackUtils.ToOnGameThread<GooglePlayGames.BasicApi.Quests.QuestClaimMilestoneStatus, IQuest, IQuestMilestone>(callback);
      NativeQuestMilestone milestone1 = milestone as NativeQuestMilestone;
      if (milestone1 == null)
      {
        Logger.e("Encountered milestone that was not generated by this IQuestClient");
        callback(GooglePlayGames.BasicApi.Quests.QuestClaimMilestoneStatus.BadInput, (IQuest) null, (IQuestMilestone) null);
      }
      else
        this.mManager.ClaimMilestone(milestone1, (Action<GooglePlayGames.Native.PInvoke.QuestManager.ClaimMilestoneResponse>) (response =>
        {
          if (response.RequestSucceeded())
            callback(GooglePlayGames.BasicApi.Quests.QuestClaimMilestoneStatus.Success, (IQuest) response.Quest(), (IQuestMilestone) response.ClaimedMilestone());
          else
            callback(NativeQuestClient.FromClaimStatus(response.ResponseStatus()), (IQuest) null, (IQuestMilestone) null);
        }));
    }

    private static GooglePlayGames.BasicApi.Quests.QuestClaimMilestoneStatus FromClaimStatus(CommonErrorStatus.QuestClaimMilestoneStatus status)
    {
      CommonErrorStatus.QuestClaimMilestoneStatus claimMilestoneStatus = status;
      switch (claimMilestoneStatus + 5)
      {
        case ~(CommonErrorStatus.QuestClaimMilestoneStatus.ERROR_INTERNAL | CommonErrorStatus.QuestClaimMilestoneStatus.VALID):
          return GooglePlayGames.BasicApi.Quests.QuestClaimMilestoneStatus.Timeout;
        case ~CommonErrorStatus.QuestClaimMilestoneStatus.ERROR_NOT_AUTHORIZED:
          return GooglePlayGames.BasicApi.Quests.QuestClaimMilestoneStatus.NotAuthorized;
        case (CommonErrorStatus.QuestClaimMilestoneStatus) 3:
          return GooglePlayGames.BasicApi.Quests.QuestClaimMilestoneStatus.InternalError;
        case (CommonErrorStatus.QuestClaimMilestoneStatus) 6:
          return GooglePlayGames.BasicApi.Quests.QuestClaimMilestoneStatus.Success;
        default:
          if (claimMilestoneStatus == CommonErrorStatus.QuestClaimMilestoneStatus.ERROR_MILESTONE_CLAIM_FAILED)
            return GooglePlayGames.BasicApi.Quests.QuestClaimMilestoneStatus.MilestoneClaimFailed;
          if (claimMilestoneStatus == CommonErrorStatus.QuestClaimMilestoneStatus.ERROR_MILESTONE_ALREADY_CLAIMED)
            return GooglePlayGames.BasicApi.Quests.QuestClaimMilestoneStatus.MilestoneAlreadyClaimed;
          Logger.e("Encountered unknown status: " + (object) status);
          return GooglePlayGames.BasicApi.Quests.QuestClaimMilestoneStatus.InternalError;
      }
    }
  }
}