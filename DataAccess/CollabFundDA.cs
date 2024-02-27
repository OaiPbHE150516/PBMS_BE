﻿using Microsoft.EntityFrameworkCore;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.Data.Auth;
using pbms_be.Data.CollabFund;
using pbms_be.Data.Custom;
using pbms_be.Data.Status;
using pbms_be.Data.Trans;
using pbms_be.DTOs;
using System.Collections.Generic;
using static Google.Cloud.DocumentAI.V1.BatchProcessMetadata.Types;

namespace pbms_be.DataAccess
{
    public class CollabFundDA
    {
        private readonly PbmsDbContext _context;
        public CollabFundDA(PbmsDbContext context)
        {
            _context = context;
        }

        internal CollabFund CreateCollabFund(CollabFund collabFund)
        {
            try
            {
                if (IsCollabFundExist(collabFund))
                {
                    throw new Exception(Message.COLLAB_FUND_ALREADY_EXIST);
                }
                _context.CollabFund.Add(collabFund);
                _context.SaveChanges();
                return GetCollabFund(collabFund.CollabFundID);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public bool IsCollabFundExist(CollabFund collabFund)
        {
            var exist = _context.CollabFund.Any(cf => cf.Name == collabFund.Name);
            return exist;
        }

        internal CollabFund GetCollabFund(string name)
        {
            try
            {
                var result = _context.CollabFund
                            .Where(cf => cf.Name == name
                                && cf.ActiveStateID == ActiveStateConst.ACTIVE)
                            .Include(cf => cf.ActiveState)
                            .FirstOrDefault();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal CollabFund GetCollabFund(int collabFundID)
        {
            try
            {
                var result = _context.CollabFund
                            .Where(cf => cf.CollabFundID == collabFundID
                                    && cf.ActiveStateID == ActiveStateConst.ACTIVE)
                            .Include(cf => cf.ActiveState)
                            .FirstOrDefault();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal CollabFund GetCollabFund(int collabFundID, string accountID)
        {
            try
            {
                var collabAccount = _context.AccountCollab
                                    .Where(ca => ca.AccountID == accountID
                                            && ca.ActiveStateID == ActiveStateConst.ACTIVE)
                                    .Select(ca => ca.CollabFundID)
                                    .ToList();

                var result = _context.CollabFund
                            .Where(cf => collabAccount.Contains(cf.CollabFundID)
                                    && cf.CollabFundID == collabFundID
                                    && cf.ActiveStateID == ActiveStateConst.ACTIVE)
                            .Include(cf => cf.ActiveState)
                            .FirstOrDefault();
                //if (result == null) throw new Exception(Message.COLLAB_FUND_NOT_EXIST);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal List<CollabFund> GetCollabFunds(string accountID)
        {
            try
            {
                var collabAccount = _context.AccountCollab
                                    .Where(ca => ca.AccountID == accountID
                                        && ca.ActiveStateID == ActiveStateConst.ACTIVE)
                                    .Select(ca => ca.CollabFundID)
                                    .ToList();

                var result = _context.CollabFund
                            .Where(cf => collabAccount.Contains(cf.CollabFundID)
                                        && cf.ActiveStateID == ActiveStateConst.ACTIVE)
                            .Include(cf => cf.ActiveState)
                            .ToList();
                foreach (var item in result)
                {
                    item.CollabFundActivities = GetAllActivityCollabFund(item.CollabFundID, accountID);
                }
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal CollabFund UpdateCollabFund(CollabFund collabFundEntity)
        {
            try
            {
                if (IsCollabFundDuplicate(collabFundEntity))
                    throw new Exception(Message.COLLAB_FUND_DUPLICATE);
                var collabFund = GetCollabFund(collabFundEntity.CollabFundID);
                if (collabFund == null) throw new Exception(Message.COLLAB_FUND_NOT_EXIST);
                collabFund.Name = collabFundEntity.Name;
                collabFund.Description = collabFundEntity.Description;
                collabFund.ActiveStateID = collabFundEntity.ActiveStateID;
                _context.SaveChanges();
                return GetCollabFund(collabFund.CollabFundID);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private bool IsCollabFundDuplicate(CollabFund collabFundEntity)
        {
            var collabFund = _context.CollabFund
                .Where(cf => cf.Name == collabFundEntity.Name && cf.CollabFundID != collabFundEntity.CollabFundID)
                .FirstOrDefault();
            return collabFund != null;
        }

        internal CollabFund ChangeCollabFundActiveState(ChangeCollabFundActiveStateDTO changeActiveStateDTO)
        {
            try
            {
                var collabFund = GetCollabFund(changeActiveStateDTO.CollabFundID);
                if (collabFund is null) throw new Exception(Message.COLLAB_FUND_NOT_EXIST);
                collabFund.ActiveStateID = changeActiveStateDTO.ActiveStateID;
                _context.SaveChanges();
                return GetCollabFund(collabFund.CollabFundID);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal List<CollabFundActivity> GetAllActivityCollabFund(int collabFundID, string accountID)
        {
            try
            {
                var collabAccount = _context.AccountCollab
                    .Where(ca => ca.AccountID == accountID
                    && ca.ActiveStateID == ActiveStateConst.ACTIVE)
                    .Select(ca => ca.CollabFundID)
                    .ToList();

                var result = _context.CollabFundActivity
                    .Where(cfa => collabAccount.Contains(cfa.CollabFundID)
                                    && cfa.CollabFundID == collabFundID
                                    && cfa.ActiveStateID == ActiveStateConst.ACTIVE)
                    .Include(cfa => cfa.ActiveState)
                    .ToList();

                var transDA = new TransactionDA(_context);

                foreach (var item in result)
                {
                    if (item.TransactionID > ConstantConfig.DEFAULT_NULL_TRANSACTION_ID)
                    {
                        item.Transaction = transDA.GetTransaction(item.TransactionID);
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal void GetDetailCollabFund(int collabFundID, string accountID, out bool isExist, out CollabFund collabFund)
        {
            try
            {
                var collabAccount = _context.AccountCollab
                                    .Where(ca => ca.AccountID == accountID
                                    && ca.ActiveStateID == ActiveStateConst.ACTIVE)
                                    .Select(ca => ca.CollabFundID)
                                    .ToList();

                var result = _context.CollabFund
                    .Where(cf => collabAccount.Contains(cf.CollabFundID)
                    && cf.CollabFundID == collabFundID
                    && cf.ActiveStateID == ActiveStateConst.ACTIVE)
                    .Include(cf => cf.ActiveState)
                    .FirstOrDefault();

                if (result is null)
                {
                    isExist = false;
                    collabFund = new CollabFund();
                }
                else
                {
                    isExist = true;
                    result.CollabFundActivities = GetAllActivityCollabFund(collabFundID, accountID);
                    collabFund = result;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal CollabFund CreateCollabFundActivity(CollabFundActivity collabFundActivityEntity)
        {
            try
            {
                if (IsCollabFundActivityDuplicate(collabFundActivityEntity))
                    throw new Exception(Message.COLLAB_FUND_ACTIVITY_DUPLICATE);
                _context.CollabFundActivity.Add(collabFundActivityEntity);
                _context.SaveChanges();
                return GetCollabFund(collabFundActivityEntity.CollabFundID);

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private bool IsCollabFundActivityDuplicate(CollabFundActivity collabFundActivityEntity)
        {
            try
            {
                // check if collab fund activity already exist, check all properties
                var collabFundActivity = _context.CollabFundActivity
                    .Where(cfa => cfa.CollabFundID == collabFundActivityEntity.CollabFundID
                                        && cfa.AccountID == collabFundActivityEntity.AccountID
                                        && cfa.Note == collabFundActivityEntity.Note
                                        && cfa.Filename == collabFundActivityEntity.Filename)
                    .FirstOrDefault();
                return collabFundActivity != null;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal List<Account> GetAllMemberCollabFund(int collabFundID, string accountID)
        {
            try
            {
                var isInFund = _context.AccountCollab
                    .Where(ca => ca.CollabFundID == collabFundID
                                            && ca.AccountID == accountID
                                            && ca.ActiveStateID == ActiveStateConst.ACTIVE)
                    .FirstOrDefault();
                if (isInFund is null) throw new Exception(Message.ACCOUNT_IS_NOT_IN_COLLAB_FUND);

                var collabAccount = _context.AccountCollab
                    .Where(ca => ca.CollabFundID == collabFundID
                        && ca.ActiveStateID == ActiveStateConst.ACTIVE)
                    .Select(ca => ca.AccountID)
                    .ToList();

                var result = _context.Account
                    .Where(a => collabAccount.Contains(a.AccountID))
                    .ToList();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // invite member to collab fund
        internal List<Account> InviteMemberCollabFund(MemberCollabFundDTO InviteMemberDTO)
        {
            try
            {
                // 1. check if account is fundholder
                var collabAccount = IsFundholderCollabFund(InviteMemberDTO.CollabFundID, InviteMemberDTO.AccountFundholderID);
                if (collabAccount) throw new Exception(Message.ACCOUNT_IS_NOT_FUNDHOLDER);

                // 2. check if account is exist
                var authDA = new AuthDA(_context);
                var account = authDA.GetAccount(InviteMemberDTO.AccountMemberID);
                if (account is null) throw new Exception(Message.ACCOUNT_NOT_FOUND);

                // 3. check if account is already invited
                var isInvited = IsAlreadyInvitedCollabFund(InviteMemberDTO.CollabFundID, InviteMemberDTO.AccountMemberID);
                if (isInvited) throw new Exception(Message.ACCOUNT_ALREADY_INVITED);

                // 4. check if account is already a member
                var isExist = IsAlreadyMemberCollabFund(InviteMemberDTO.CollabFundID, InviteMemberDTO.AccountMemberID);
                if (isExist) throw new Exception(Message.ACCOUNT_ALREADY_IS_MEMBER);

                // 5. add account as member
                var accountCollab = new AccountCollab
                {
                    AccountID = InviteMemberDTO.AccountMemberID,
                    CollabFundID = InviteMemberDTO.CollabFundID,
                    IsFundholder = false,
                    ActiveStateID = ActiveStateConst.PENDING
                };
                _context.AccountCollab.Add(accountCollab);
                _context.SaveChanges();

                // 6. return all member
                return GetAllMemberCollabFund(InviteMemberDTO.CollabFundID, InviteMemberDTO.AccountFundholderID);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //internal List<Account> AddMemberCollabFund(MemberCollabFundDTO AddMemberDTO)
        //{
        //    try
        //    {
        //        // 1. check if account is fundholder
        //        var collabAccount = IsFundholderCollabFund(AddMemberDTO.CollabFundID, AddMemberDTO.AccountFundholderID);
        //        if (collabAccount) throw new Exception(Message.ACCOUNT_IS_NOT_FUNDHOLDER);

        //        // 2. check if account is exist
        //        var authDA = new AuthDA(_context);
        //        var account = authDA.GetAccount(AddMemberDTO.AccountMemberID);
        //        if (account is null) throw new Exception(Message.ACCOUNT_NOT_FOUND);

        //        //3. check if account is already a member
        //        var isExist = IsAlreadyMemberCollabFund(AddMemberDTO.CollabFundID, AddMemberDTO.AccountMemberID);
        //        if (isExist) throw new Exception(Message.ACCOUNT_ALREADY_IS_MEMBER);

        //        // 4. add account as member
        //        var accountCollab = new AccountCollab
        //        {
        //            AccountID = AddMemberDTO.AccountMemberID,
        //            CollabFundID = AddMemberDTO.CollabFundID,
        //            IsFundholder = false,
        //            ActiveStateID = ActiveStateConst.ACTIVE
        //        };
        //        _context.AccountCollab.Add(accountCollab);
        //        _context.SaveChanges();
        //        // 5. return all member
        //        return GetAllMemberCollabFund(AddMemberDTO.CollabFundID, AddMemberDTO.AccountFundholderID);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}

        private bool IsAlreadyMemberCollabFund(int collabFundID, string accountMemberID)
        {
            try
            {
                var isExist = _context.AccountCollab
                    .Where(ca => ca.CollabFundID == collabFundID
                        && ca.AccountID == accountMemberID
                        && ca.ActiveStateID == ActiveStateConst.ACTIVE)
                    .FirstOrDefault();
                return isExist != null;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // check if an account is already invited to collab fund
        private bool IsAlreadyInvitedCollabFund(int collabFundID, string accountMemberID)
        {
            try
            {
                var isExist = _context.AccountCollab
                    .Where(ca => ca.CollabFundID == collabFundID
                                && ca.AccountID == accountMemberID
                                && ca.ActiveStateID == ActiveStateConst.PENDING)
                    .FirstOrDefault();
                return isExist != null;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // check if account is fundholder
        private bool IsFundholderCollabFund(int collabFundID, string accountID)
        {
            try
            {
                var isFundholder = _context.AccountCollab
                    .Where(ca => ca.CollabFundID == collabFundID
                    && ca.AccountID == accountID
                    && ca.IsFundholder == true
                    && ca.ActiveStateID == ActiveStateConst.ACTIVE)
                    .FirstOrDefault();
                return isFundholder != null;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // check if account is member
        private bool IsMemberCollabFund(int collabFundID, string accountID)
        {
            try
            {
                var isMember = _context.AccountCollab
                    .Where(ca => ca.CollabFundID == collabFundID
                    && ca.AccountID == accountID
                    && ca.IsFundholder == false
                    && ca.ActiveStateID == ActiveStateConst.ACTIVE)
                    .FirstOrDefault();
                return isMember != null;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal List<Account> DeleteMemberCollabFund(MemberCollabFundDTO deleteMemberCollabFundDTO)
        {
            try
            {
                // 1. check if account is fundholder
                var isFundholder = IsFundholderCollabFund(deleteMemberCollabFundDTO.CollabFundID, deleteMemberCollabFundDTO.AccountFundholderID);
                if (!isFundholder) throw new Exception(Message.ACCOUNT_IS_NOT_FUNDHOLDER);

                // 2. check if account is a member
                var isMember = IsMemberCollabFund(deleteMemberCollabFundDTO.CollabFundID, deleteMemberCollabFundDTO.AccountMemberID);
                if (!isMember) throw new Exception(Message.ACCOUNT_IS_NOT_MEMBER);

                // 3. delete account as member
                var accountCollab = _context.AccountCollab
                    .Where(ca => ca.CollabFundID == deleteMemberCollabFundDTO.CollabFundID && ca.AccountID == deleteMemberCollabFundDTO.AccountMemberID)
                    .FirstOrDefault();
                if (accountCollab is null) throw new Exception(Message.ACCOUNT_NOT_FOUND);
                accountCollab.ActiveStateID = ActiveStateConst.INACTIVE;
                accountCollab.LastTime = DateTime.UtcNow;
                _context.SaveChanges();

                // 4. return all member
                return GetAllMemberCollabFund(deleteMemberCollabFundDTO.CollabFundID, deleteMemberCollabFundDTO.AccountFundholderID);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal object AcceptMemberCollabFund(MemberCollabFundDTO acceptMemberCollabFundDTO)
        {
            try
            {
                // 1. check if account is fundholder
                var isFundholder = IsFundholderCollabFund(acceptMemberCollabFundDTO.CollabFundID, acceptMemberCollabFundDTO.AccountFundholderID);
                if (!isFundholder) throw new Exception(Message.ACCOUNT_IS_NOT_FUNDHOLDER);

                // 2. check if account is already invited
                var isInvited = IsAlreadyInvitedCollabFund(acceptMemberCollabFundDTO.CollabFundID, acceptMemberCollabFundDTO.AccountMemberID);
                if (!isInvited) throw new Exception(Message.ACCOUNT_WAS_NOT_INVITED);

                // 3. accept account as member
                var accountCollab = _context.AccountCollab
                    .Where(ca => ca.CollabFundID == acceptMemberCollabFundDTO.CollabFundID
                                       && ca.AccountID == acceptMemberCollabFundDTO.AccountMemberID)
                    .FirstOrDefault();
                if (accountCollab is null) throw new Exception(Message.ACCOUNT_NOT_FOUND);
                accountCollab.ActiveStateID = ActiveStateConst.ACTIVE;
                accountCollab.LastTime = DateTime.UtcNow;
                _context.SaveChanges();

                // 4. return all member
                return GetAllMemberCollabFund(acceptMemberCollabFundDTO.CollabFundID, acceptMemberCollabFundDTO.AccountFundholderID);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal object DeclineMemberCollabFund(MemberCollabFundDTO declineMemberCollabFundDTO)
        {
            try
            {
                // 2. check if account is already invited
                var isInvited = IsAlreadyInvitedCollabFund(declineMemberCollabFundDTO.CollabFundID, declineMemberCollabFundDTO.AccountMemberID);
                if (!isInvited) throw new Exception(Message.ACCOUNT_WAS_NOT_INVITED);

                // 3. decline account as member
                var accountCollab = _context.AccountCollab
                    .Where(ca => ca.CollabFundID == declineMemberCollabFundDTO.CollabFundID
                                                          && ca.AccountID == declineMemberCollabFundDTO.AccountMemberID)
                    .FirstOrDefault();
                if (accountCollab is null) throw new Exception(Message.ACCOUNT_NOT_FOUND);
                accountCollab.ActiveStateID = ActiveStateConst.INACTIVE;
                accountCollab.LastTime = DateTime.UtcNow;
                _context.SaveChanges();

                // 4. return all member
                return GetAllMemberCollabFund(declineMemberCollabFundDTO.CollabFundID, declineMemberCollabFundDTO.AccountFundholderID);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal object DeleteInvitationCollabFund(MemberCollabFundDTO deleteInvitationCollabFundDTO)
        {
            try
            {
                // 1. check if account is fundholder
                var isFundholder = IsFundholderCollabFund(deleteInvitationCollabFundDTO.CollabFundID, deleteInvitationCollabFundDTO.AccountFundholderID);
                if (!isFundholder) throw new Exception(Message.ACCOUNT_IS_NOT_FUNDHOLDER);

                // 2. check if account is already invited
                var isInvited = IsAlreadyInvitedCollabFund(deleteInvitationCollabFundDTO.CollabFundID, deleteInvitationCollabFundDTO.AccountMemberID);
                if (!isInvited) throw new Exception(Message.ACCOUNT_WAS_NOT_INVITED);

                // 3. delete account as member
                var accountCollab = _context.AccountCollab
                    .Where(ca => ca.CollabFundID == deleteInvitationCollabFundDTO.CollabFundID
                                                                             && ca.AccountID == deleteInvitationCollabFundDTO.AccountMemberID)
                    .FirstOrDefault();
                if (accountCollab is null) throw new Exception(Message.ACCOUNT_NOT_FOUND);
                accountCollab.ActiveStateID = ActiveStateConst.INACTIVE;
                accountCollab.LastTime = DateTime.UtcNow;
                _context.SaveChanges();

                // 4. return all member
                return GetAllMemberCollabFund(deleteInvitationCollabFundDTO.CollabFundID, deleteInvitationCollabFundDTO.AccountFundholderID);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal List<Account> GetAccountByEmail(string email)
        {
            try
            {
                var result = _context.Account
                    .Where(a => a.EmailAddress.Contains(email) || a.AccountName.Contains(email))
                    .ToList();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal List<CollabFundAccountActiveStateDTO> GetAccountByEmailAndCollabFundID(int collabfundID, List<Account> accountEmail)
        {
            try
            {
                var result = new List<CollabFundAccountActiveStateDTO>();
                foreach (var item in accountEmail)
                {
                    var accountCollab = _context.AccountCollab
                        .Where(ca => ca.CollabFundID == collabfundID && ca.AccountID == item.AccountID)
                        .Include(ca => ca.ActiveState)
                        .FirstOrDefault();
                    if (accountCollab != null)
                    {
                        var account = new CollabFundAccountActiveStateDTO
                        {
                            AccountID = item.AccountID,
                            AccountName = item.AccountName,
                            EmailAddress = item.EmailAddress,
                            CFA_ActiveStateID = accountCollab.ActiveStateID,
                            CFA_ActiveState = accountCollab.ActiveState
                        };
                        result.Add(account);
                        continue;
                    }
                    // if not found, add account with active state is inactive ( can invite to join collab fund)
                    var newaccount = new CollabFundAccountActiveStateDTO
                    {
                        AccountID = item.AccountID,
                        AccountName = item.AccountName,
                        EmailAddress = item.EmailAddress,
                        CFA_ActiveStateID = ActiveStateConst.INACTIVE,
                        CFA_ActiveState = new ActiveState
                        {
                            ActiveStateID = ActiveStateConst.INACTIVE,
                            Name = ActiveStateConst.INACTIVE_NAME
                        }
                    };
                    result.Add(newaccount);
                }
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal object GetAllDividingMoneyWithDetail(int collabFundID, string accountID)
        {
            try
            {
                var dividingMoney = _context.CF_DividingMoney
                    .Where(cfdm => cfdm.CollabFundID == collabFundID
                                           && cfdm.ActiveStateID == ActiveStateConst.ACTIVE)
                    .Include(cfdm => cfdm.ActiveState)
                    .ToList();

                var dividingMoneyDetail = new List<CF_DividingMoneyDetail>();
                var _authDA = new AuthDA(_context);
                foreach (var item in dividingMoney)
                {
                    var detail = _context.CF_DividingMoneyDetail
                                .Where(cfdmd => cfdmd.CF_DividingMoneyID == item.CF_DividingMoneyID)
                                .ToList();
                    foreach (var itemDetail in detail)
                    {
                        itemDetail.FromAccount = _authDA.GetAccount(itemDetail.FromAccountID);
                        itemDetail.ToAccount = _authDA.GetAccount(itemDetail.ToAccountID);
                        var time = ConvertTimeFromUtc(itemDetail.LastTime);
                        itemDetail.LastTime = time;
                    }
                    dividingMoneyDetail.AddRange(detail);
                }
                return dividingMoney;
                //return new { result, dividingMoney, dividingMoneyDetail };
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal bool IsAccountInCollabFund(string accountID, int collabFundID)
        {
            try
            {
                var result = _context.AccountCollab
                    .Where(ca => ca.AccountID == accountID
                                && ca.CollabFundID == collabFundID
                                && ca.ActiveStateID == ActiveStateConst.ACTIVE)
                    .FirstOrDefault();
                return result != null;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // a function to add system location utc time to input time
        internal DateTime ConvertTimeFromUtc(DateTime time)
        {
            //return TimeZoneInfo.ConvertTimeFromUtc(time, TimeZoneInfo.Local);
            //var timeZone = TimeZoneInfo.Local.GetUtcOffset(time).Hours;
            //Console.WriteLine("timeZone: " + timeZone);
            //return time.AddHours(timeZone);
            return time.AddHours(ConstantConfig.VN_TIMEZONE_UTC);
        }

        //internal object GetAllAmountContributed(int collabFundID, string accountID)
        //{
        //    try
        //    {
        //        var result = GetDivideMoneyInfo(collabFundID, accountID).Find(p => p.AccountID == accountID);
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}

        internal object GetDivideMoneyInfo(int collabFundID, string accountID)
        {
            try
            {
                var divideMoneyInfor = GetDivideMoneyCollabFund(collabFundID);
                var totalAmount = divideMoneyInfor.Sum(p => p.TotalAmount);
                var numberParticipant = divideMoneyInfor.Count;
                // cast totalAmount to int, chỗ này phải xử lý đề tiền chẵn.
                var totalAmountInt = (int)totalAmount;
                int averageAmount = totalAmountInt / numberParticipant;
                var remainAmount = totalAmount - averageAmount * numberParticipant;
                var cf_dividingmoney = new CF_DividingMoney
                {
                    CollabFundID = collabFundID,
                    TotalAmount = totalAmount,
                    NumberParticipant = numberParticipant,
                    AverageAmount = averageAmount,
                    RemainAmount = remainAmount
                };

                var listDetail = new List<DivideMoneyExecute>();
                foreach (var item in divideMoneyInfor)
                {
                    var dividingAmount = item.TotalAmount - averageAmount;
                    switch (dividingAmount)
                    {
                        case 0:
                            var detail0 = new DivideMoneyExecute
                            {
                                FromAccountID = item.AccountID,
                                ToAccountID = item.AccountID,
                                ActualAmount = dividingAmount,
                                State = 0
                            };
                            break;
                        case > 0:
                            var detail1 = new DivideMoneyExecute
                            {
                                ToAccountID = item.AccountID,
                                ActualAmount = dividingAmount,
                                State = 1
                            };
                            listDetail.Add(detail1);
                            break;
                        case < 0:
                            var detail2 = new DivideMoneyExecute
                            {
                                FromAccountID = item.AccountID,
                                ActualAmount = dividingAmount,
                                State = -1
                            };
                            listDetail.Add(detail2);
                            break;
                    }
                }
                // sort listDetail by ActualAmount
                listDetail.Sort((x, y) => x.ActualAmount.CompareTo(y.ActualAmount));

                //var listDetail2 = new List<DivideMoneyExecute>();
                var listDetail3 = new List<DivideMoneyExecute>();
                bool shouldLoop = true;
                while (true)
                {
                    var listDetail2 = new List<DivideMoneyExecute>(listDetail);
                    // remove all item with actualAmount = 0
                    listDetail2.RemoveAll(p => p.ActualAmount == 0);
                    foreach (var item in listDetail2)
                    {
                        var toAccount = listDetail2.Find(p => p.ActualAmount > 0 && p.FromAccountID == null);
                        if (toAccount != null)
                        {
                            item.ToAccountID = toAccount.ToAccountID;
                            var actual = Math.Abs(item.ActualAmount) - toAccount.ActualAmount;
                            item.ActualAmount = actual;
                            if (actual > 0)
                            {
                                continue;
                            }
                            else break;
                        }

                    }
                }

                //if (item.State < 0)
                //{
                //    while (item.RemainAmount > 0)
                //    {
                //        // find the first account with state = 1, fromAccountID = null, then set fromAccountID = item.ToAccountID
                //        var toAccount = listDetail.Find(p => p.State == 1 && p.FromAccountID == null);
                //        if (toAccount != null)
                //        {
                //            item.ToAccountID = toAccount.ToAccountID;
                //            item.State = 0;
                //            var actual = Math.Abs(item.ActualAmount) - toAccount.ActualAmount;
                //            item.ActualAmount = actual;
                //            if (actual > 0)
                //            {
                //                continue;
                //            }
                //            else break;
                //        }
                //    }
                //    listDetail2.Add(item);
                //}

                //return new { cf_dividingmoney, listDetail, listDetail2 };
                //return cf_dividingmoney;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal List<DivideMoneyInfo> GetDivideMoneyCollabFund(int collabFundID)
        {
            try
            {
                /* câu query lấy ra tất cả thông tin về số tiền đã đóng góp của mỗi thành viên trong collab fund
                 * điều kiện: 
                 * - kể từ lần chia tiền cuối cùng (isBeforeDivide = true) hoặc từ đầu nếu ko có lần chia tiền đầu tiên
                 * - id của các activity ghi lại transaction phải lớn hơn id của lần chia tiền cuối cùng
                 * 
                     WITH FirstTrue AS (
                         SELECT account_id, collab_fun_activity_id, MIN(collab_fun_activity_id) as min_id
                         FROM collab_fun_activity
                         WHERE isBeforeDivide = true
                         GROUP BY account_id, collab_fun_activity_id
                     )
                     SELECT cf.account_id, COALESCE(SUM(total_amount), 0) as total_amount, COALESCE(COUNT(t.transaction_id), 0) as transaction_count
                     FROM collab_fun_activity as cf
                     LEFT JOIN transaction as t ON cf.transaction_id = t.transaction_id
                     LEFT JOIN FirstTrue as ft ON cf.account_id = ft.account_id
                     WHERE cf.collabfund_id = 2
                         AND cf.isBeforeDivide = false
                         AND (cf.collab_fun_activity_id > ft.min_id OR ft.min_id IS NULL)
                     GROUP BY cf.account_id;
                 */
                var rawQuery = $"WITH FirstTrue AS (" +
                    $"SELECT account_id, collab_fun_activity_id, MIN(collab_fun_activity_id) as min_id " +
                    $"FROM collab_fun_activity " +
                    $"WHERE isBeforeDivide = true " +
                    $"GROUP BY account_id, collab_fun_activity_id " +
                    $") " +
                    $"SELECT cf.account_id, COALESCE(SUM(total_amount), 0) as total_amount, COALESCE(COUNT(t.transaction_id), 0) as transaction_count " +
                    $"FROM collab_fun_activity as cf " +
                    $"LEFT JOIN transaction as t ON cf.transaction_id = t.transaction_id " +
                    $"LEFT JOIN FirstTrue as ft ON cf.account_id = ft.account_id " +
                    $"WHERE cf.collabfund_id = {collabFundID} " +
                    $"AND cf.isBeforeDivide = false " +
                    $"AND (cf.collab_fun_activity_id > ft.min_id OR ft.min_id IS NULL) " +
                    $"GROUP BY cf.account_id;";
                var result = _context.DivideMoneyInfo.FromSqlRaw(rawQuery).ToList();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
