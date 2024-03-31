using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.Data.Auth;
using pbms_be.Data.CollabFund;
using pbms_be.Data.Custom;
using pbms_be.Data.Status;
using pbms_be.Data.Trans;
using pbms_be.Data.WalletF;
using pbms_be.DTOs;
using pbms_be.Library;
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

        internal CollabFund CreateCollabFund(CollabFund collabFund, string accountID, List<string> accountIDs)
        {
            try
            {
                if (IsCollabFundExist(collabFund))
                {
                    throw new Exception(Message.COLLAB_FUND_ALREADY_EXIST);
                }
                collabFund.ActiveStateID = ActiveStateConst.ACTIVE;
                _context.CollabFund.Add(collabFund);
                _context.SaveChanges();
                var result = GetCollabFund(collabFund.CollabFundID);
                var collabAccount = new AccountCollab
                {
                    AccountID = accountID,
                    CollabFundID = result.CollabFundID,
                    IsFundholder = true,
                    ActiveStateID = ActiveStateConst.ACTIVE
                };
                _context.AccountCollab.Add(collabAccount);
                var accountCollabs = new List<AccountCollab>();
                foreach (var item in accountIDs)
                {
                    // continue if accountID is fundholder
                    if (item == accountID) continue;
                    var accountCollab = new AccountCollab
                    {
                        AccountID = item,
                        CollabFundID = result.CollabFundID,
                        IsFundholder = false,
                        ActiveStateID = ActiveStateConst.ACTIVE
                    };
                    accountCollabs.Add(accountCollab);
                }
                _context.AccountCollab.AddRange(accountCollabs);
                _context.SaveChanges();
                return result;
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
                if (result is not null )
                {
                    var divideMoneyInfor = GetDivideMoneyCollabFund(result.CollabFundID);
                    result.TotalAmount = divideMoneyInfor.Sum(p => p.TotalAmount);
                }
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

                foreach(var item in result)
                {
                    var divideMoneyInfor = GetDivideMoneyCollabFund(item.CollabFundID);
                    item.TotalAmount = divideMoneyInfor.Sum(p => p.TotalAmount);
                }

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public bool IsFundholderEasy(int collabFundID, string accountID)
        {
            try
            {
                var isFundholder = _context.AccountCollab.Any(ca => ca.CollabFundID == collabFundID
                    && ca.AccountID == accountID
                    && ca.IsFundholder == true
                    && ca.ActiveStateID == ActiveStateConst.ACTIVE);
                return isFundholder;
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
                    .Include(cfa => cfa.Account)
                    .ToList();

                var transDA = new TransactionDA(_context);

                foreach (var item in result)
                {
                    if (item.TransactionID > ConstantConfig.DEFAULT_NULL_TRANSACTION_ID)
                    {
                        item.Transaction = transDA.GetTransaction(item.TransactionID);
                    }
                    else
                    {
                        item.TransactionID = ConstantConfig.DEFAULT_ZERO_VALUE;
                    }

                }
                // sort result by createTime
                result.Sort((x, y) => y.CreateTime.CompareTo(x.CreateTime));
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

        //internal List<Account> GetAllMemberCollabFund(int collabFundID, string accountID)
        //{
        //    try
        //    {
        //        if (!IsMemberCollabFund(collabFundID, accountID)) throw new Exception(Message.ACCOUNT_IS_NOT_IN_COLLAB_FUND);
        //        var collabAccount = _context.AccountCollab
        //            .Where(ca => ca.CollabFundID == collabFundID
        //                && ca.ActiveStateID == ActiveStateConst.ACTIVE)
        //            .Select(ca => ca.AccountID)
        //            .ToList();

        //        var result = _context.Account
        //            .Where(a => collabAccount.Contains(a.AccountID))
        //            .ToList();
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}

        internal List<AccountDetailInCollabFundDTO> GetAllMemberWithDetailCollabFund(int collabFundID, string accountID)
        {
            try
            {
                if (!IsAccountInCollabFund(collabFundID, accountID)) throw new Exception(Message.ACCOUNT_IS_NOT_IN_COLLAB_FUND);
                var collabAccount = _context.AccountCollab
                    .Where(ca => ca.CollabFundID == collabFundID
                        && ca.ActiveStateID == ActiveStateConst.ACTIVE)
                    .Select(ca => ca.AccountID)
                    .ToList();
                var result = _context.Account
                    .Where(a => collabAccount.Contains(a.AccountID))
                    .ToList();
                var accountDetailInCollabFundDTOs = new List<AccountDetailInCollabFundDTO>();
                foreach (var item in result)
                {
                    var inCollab = _context.AccountCollab
                                            .Where(ca => ca.CollabFundID == collabFundID
                                             && ca.AccountID == item.AccountID
                                             && ca.ActiveStateID == ActiveStateConst.ACTIVE)
                                            .Include(ca => ca.ActiveState)
                                            .FirstOrDefault();
                    if (inCollab is null) throw new Exception(Message.ACCOUNT_NOT_FOUND);
                    var accountDetailInCollabFundDTO = new AccountDetailInCollabFundDTO
                    {
                        AccountID = item.AccountID,
                        ClientID = item.ClientID,
                        AccountName = item.AccountName,
                        EmailAddress = item.EmailAddress,
                        PictureURL = item.PictureURL,
                        IsFundholder = inCollab.IsFundholder,
                        ActiveStateID = inCollab.ActiveStateID,
                        ActiveState = inCollab.ActiveState,
                        LastTime = LConvertVariable.ConvertUtcToLocalTime(inCollab.LastTime)
                    };
                    accountDetailInCollabFundDTOs.Add(accountDetailInCollabFundDTO);
                }
                return accountDetailInCollabFundDTOs;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        internal object GetAllMemberWithDetailCollabFundTypeByType(int collabFundID, string accountID)
        {
            try
            {
                if (!IsAccountInCollabFund(collabFundID, accountID)) throw new Exception(Message.ACCOUNT_IS_NOT_IN_COLLAB_FUND);
                var collabAccount = _context.AccountCollab
                    .Where(ca => ca.CollabFundID == collabFundID)
                    .Select(ca => ca.AccountID)
                    .ToList();
                var result = _context.Account
                    .Where(a => collabAccount.Contains(a.AccountID))
                    .ToList();
                var accountDetailInCollabFundDTOs = new List<AccountDetailInCollabFundDTO>();
                foreach (var item in result)
                {
                    var inCollab = _context.AccountCollab
                                            .Where(ca => ca.CollabFundID == collabFundID
                                             && ca.AccountID == item.AccountID)
                                            .Include(ca => ca.ActiveState)
                                            .FirstOrDefault();
                    if (inCollab is null) throw new Exception(Message.ACCOUNT_NOT_FOUND);
                    var accountDetailInCollabFundDTO = new AccountDetailInCollabFundDTO
                    {
                        AccountID = item.AccountID,
                        ClientID = item.ClientID,
                        AccountName = item.AccountName,
                        EmailAddress = item.EmailAddress,
                        PictureURL = item.PictureURL,
                        IsFundholder = inCollab.IsFundholder,
                        ActiveStateID = inCollab.ActiveStateID,
                        ActiveState = inCollab.ActiveState,
                        LastTime = LConvertVariable.ConvertUtcToLocalTime(inCollab.LastTime)
                    };
                    accountDetailInCollabFundDTOs.Add(accountDetailInCollabFundDTO);
                }

                // split account by activeStateID (active, pending, inactive)
                var active = accountDetailInCollabFundDTOs.FindAll(a => a.ActiveStateID == ActiveStateConst.ACTIVE);
                var pending = accountDetailInCollabFundDTOs.FindAll(a => a.ActiveStateID == ActiveStateConst.PENDING);
                var inactive = accountDetailInCollabFundDTOs.FindAll(a => a.ActiveStateID == ActiveStateConst.INACTIVE);

                return new { active, pending, inactive };
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // invite member to collab fund
        internal List<AccountDetailInCollabFundDTO> InviteMemberCollabFund(MemberCollabFundDTO InviteMemberDTO)
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
                return GetAllMemberWithDetailCollabFund(InviteMemberDTO.CollabFundID, InviteMemberDTO.AccountFundholderID);
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
        internal bool IsAlreadyInvitedCollabFund(int collabFundID, string accountMemberID)
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
        internal bool IsFundholderCollabFund(int collabFundID, string accountID)
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

        // check if account is in collab fund
        private bool IsAccountInCollabFund(int collabFundID, string accountID)
        {
            try
            {
                var isMember = _context.AccountCollab
                    .Any(ca => ca.CollabFundID == collabFundID
                    && ca.AccountID == accountID
                    && ca.ActiveStateID == ActiveStateConst.ACTIVE);
                return isMember;
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
                    .Any(ca => ca.CollabFundID == collabFundID
                    && ca.AccountID == accountID
                    && ca.IsFundholder == false
                    && ca.ActiveStateID == ActiveStateConst.ACTIVE);
                return isMember;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal List<AccountDetailInCollabFundDTO> DeleteMemberCollabFund(MemberCollabFundDTO deleteMemberCollabFundDTO)
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
                return GetAllMemberWithDetailCollabFund(deleteMemberCollabFundDTO.CollabFundID, deleteMemberCollabFundDTO.AccountFundholderID);
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
                return GetAllMemberWithDetailCollabFund(acceptMemberCollabFundDTO.CollabFundID, acceptMemberCollabFundDTO.AccountFundholderID);
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
                return GetAllMemberWithDetailCollabFund(declineMemberCollabFundDTO.CollabFundID, declineMemberCollabFundDTO.AccountFundholderID);
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
                return GetAllMemberWithDetailCollabFund(deleteInvitationCollabFundDTO.CollabFundID, deleteInvitationCollabFundDTO.AccountFundholderID);
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
                        var time = LConvertVariable.ConvertUtcToLocalTime(itemDetail.LastTime);
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

        internal void GetDivideMoneyInfo(int collabFundID, string accountID, out CF_DividingMoney cfdividingmoney_result, 
            out List<CF_DividingMoneyDetail> cfdm_detail_result, out List<DivideMoneyInfoWithAccount> divideMoneyInfos)
        {
            try
            {
                var divideMoneyInfor = GetDivideMoneyCollabFund(collabFundID);
                var totalAmount = divideMoneyInfor.Sum(p => p.TotalAmount);
                var numberParticipant = divideMoneyInfor.Count;
                var averageAmount = (totalAmount / ConstantConfig.DEFAULT_VND_THOUSAND_SEPARATOR / numberParticipant) * ConstantConfig.DEFAULT_VND_THOUSAND_SEPARATOR;
                var remainAmount = totalAmount - averageAmount * numberParticipant;
                var cf_dividingmoney = new CF_DividingMoney
                {
                    CollabFundID = collabFundID,
                    TotalAmount = totalAmount,
                    NumberParticipant = numberParticipant,
                    AverageAmount = averageAmount,
                    RemainAmount = remainAmount
                };

                var listDetail = CalculateTheAdditionalAmount(divideMoneyInfor, averageAmount);
                var listDetailResult = DivideMoney(listDetail);
                var listDividingMoneyDetail = CalculateTheDividingMoneyDetail(divideMoneyInfor, listDetailResult);
                cfdividingmoney_result = cf_dividingmoney;
                cfdm_detail_result = listDividingMoneyDetail;

                var divideMoneyInforWithAccount = new List<DivideMoneyInfoWithAccount>();
                //var count_DMI_WithAccount = 1;

                var authDA = new AuthDA(_context);
                foreach (var item in divideMoneyInfor)
                {
                    //count_DMI_WithAccount++;
                    var account = authDA.GetAccount(item.AccountID);
                    var remain = item.TotalAmount - averageAmount;
                    var divideMoneyInfoWithAccount = new DivideMoneyInfoWithAccount
                    {
                        AccountID = item.AccountID,
                        Account = account,
                        TotalAmount = item.TotalAmount,
                        TotalAmountStr = LConvertVariable.ConvertToMoneyFormat(item.TotalAmount),
                        RemainAmount = remain,
                        RemainAmountStr = LConvertVariable.ConvertToMoneyFormat(remain),
                        TransactionCount = item.TransactionCount,
                        MoneyActionStr = LConvertVariable.ConvertToMoneyFormat(item.TotalAmount) +
                        " - " + LConvertVariable.ConvertToMoneyFormat(averageAmount) +
                        " = " + LConvertVariable.ConvertToMoneyFormat(remain),
                    };
                    divideMoneyInforWithAccount.Add(divideMoneyInfoWithAccount);
                }
                divideMoneyInfos = divideMoneyInforWithAccount;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal object DivideMoneyCollabFund(CollabAccountDTO collabAccountDTO)
        {
            try
            {
                var dividingmoney = new CF_DividingMoney();
                var dividingmoneydetail = new List<CF_DividingMoneyDetail>();
                var divideMoneyInfor = new List<DivideMoneyInfoWithAccount>();
                GetDivideMoneyInfo(collabAccountDTO.CollabFundID, collabAccountDTO.AccountID, out dividingmoney, out dividingmoneydetail, out divideMoneyInfor);

                var cf_activity = new CollabFundActivity
                {
                    CollabFundID = collabAccountDTO.CollabFundID,
                    AccountID = collabAccountDTO.AccountID,
                    Note = "Test chia tiền",
                    TransactionID = ConstantConfig.DEFAULT_NULL_TRANSACTION_ID,
                    ActiveStateID = ActiveStateConst.ACTIVE,
                };

                _context.CollabFundActivity.Add(cf_activity);
                _context.SaveChanges();
                var cfa_id = cf_activity.CollabFundActivityID;

                var cf_dividingmoney = new CF_DividingMoney
                {
                    CollabFundID = dividingmoney.CollabFundID,
                    CollabFunActivityID = cfa_id,
                    TotalAmount = dividingmoney.TotalAmount,
                    NumberParticipant = dividingmoney.NumberParticipant,
                    AverageAmount = dividingmoney.AverageAmount,
                    RemainAmount = dividingmoney.RemainAmount,
                    ActiveStateID = ActiveStateConst.ACTIVE
                };

                _context.CF_DividingMoney.Add(cf_dividingmoney);
                _context.SaveChanges();
                var cfdm_id = cf_dividingmoney.CF_DividingMoneyID;

                var list_cfdm_detail = new List<CF_DividingMoneyDetail>();
                foreach (var item in dividingmoneydetail)
                {
                    var cfdm_detail = new CF_DividingMoneyDetail
                    {
                        CF_DividingMoneyID = cfdm_id,
                        FromAccountID = item.FromAccountID,
                        FromAccountTotalAmount = item.FromAccountTotalAmount,
                        FromAccountTransactionCount = item.FromAccountTransactionCount,
                        ToAccountID = item.ToAccountID,
                        DividingAmount = item.DividingAmount,
                        LastTime = LConvertVariable.ConvertUtcToLocalTime(DateTime.UtcNow)
                    };
                    list_cfdm_detail.Add(cfdm_detail);
                }
                _context.CF_DividingMoneyDetail.AddRange(list_cfdm_detail);
                _context.SaveChanges();
                return new { cf_dividingmoney, list_cfdm_detail };

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private List<CF_DividingMoneyDetail> CalculateTheDividingMoneyDetail(List<DivideMoneyInfo> divideMoneyInfor, List<DivideMoneyExecute> listDetailResult)
        {
            var authDA = new AuthDA(_context);
            var listDividingMoneyDetail = new List<CF_DividingMoneyDetail>();
            var count = 1;
            foreach (var item in listDetailResult)
            {
                var dmInfor = divideMoneyInfor.Find(p => p.AccountID == item.FromAccountID);
                if (dmInfor is null) continue;
                var detail = new CF_DividingMoneyDetail
                {
                    CF_DividingMoneyDetailID = count,
                    FromAccountTotalAmount = dmInfor.TotalAmount,
                    FromAccountTransactionCount = dmInfor.TransactionCount,
                    FromAccountID = item.FromAccountID,
                    FromAccount = authDA.GetAccount(item.FromAccountID),
                    ToAccountID = item.ToAccountID,
                    ToAccount = authDA.GetAccount(item.ToAccountID),
                    DividingAmount = Math.Abs(item.ActualAmount),
                    LastTime = LConvertVariable.ConvertUtcToLocalTime(DateTime.UtcNow)
                };
                listDividingMoneyDetail.Add(detail);
                count++;
            }
            return listDividingMoneyDetail;
        }

        private List<DivideMoneyExecute> CalculateTheAdditionalAmount(List<DivideMoneyInfo> listinfor, long averageAmount)
        {
            var listDetail = new List<DivideMoneyExecute>();
            foreach (var item in listinfor)
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
                        };
                        break;
                    case > 0:
                        var detail1 = new DivideMoneyExecute
                        {
                            ToAccountID = item.AccountID,
                            ActualAmount = dividingAmount,
                        };
                        listDetail.Add(detail1);
                        break;
                    case < 0:
                        var detail2 = new DivideMoneyExecute
                        {
                            FromAccountID = item.AccountID,
                            ActualAmount = dividingAmount,
                        };
                        listDetail.Add(detail2);
                        break;
                }
            }
            listDetail.Sort((x, y) => x.ActualAmount.CompareTo(y.ActualAmount));
            return listDetail;
        }

        private List<DivideMoneyExecute> DivideMoney(List<DivideMoneyExecute> listDetail)
        {
            var listDetailResult = new List<DivideMoneyExecute>();
            bool isContinue = true;
            int breakcount = 0;
            while (isContinue)
            {
                breakcount++;
                var listDetail2 = new List<DivideMoneyExecute>(listDetail);
                listDetail2.RemoveAll(p => p.ActualAmount == 0);
                foreach (var item in listDetail2)
                {
                    if (item.ActualAmount < 0)
                    {
                        var toAccount = listDetail2.Find(p => p.ActualAmount > 0 && p.FromAccountID == "");
                        if (toAccount != null)
                        {
                            item.ToAccountID = toAccount.ToAccountID;
                            var actual = Math.Abs(toAccount.ActualAmount) - Math.Abs(item.ActualAmount);
                            if (actual < 0)
                            {
                                // case from > to => from = from - to, to = 0
                                var itemResult0 = new DivideMoneyExecute
                                {
                                    FromAccountID = item.FromAccountID,
                                    ToAccountID = item.ToAccountID,
                                    ActualAmount = toAccount.ActualAmount,
                                };
                                listDetailResult.Add(itemResult0);

                                toAccount.ActualAmount = 0;
                                item.ActualAmount = actual;
                                item.ToAccountID = toAccount.ToAccountID;
                            }
                            else if (actual > 0)
                            {
                                // case from < to => from = 0, to = to - from

                                var itemResult1 = new DivideMoneyExecute
                                {
                                    FromAccountID = item.FromAccountID,
                                    ToAccountID = item.ToAccountID,
                                    ActualAmount = item.ActualAmount,
                                };
                                listDetailResult.Add(itemResult1);

                                toAccount.ActualAmount = actual;
                                item.ActualAmount = 0;
                                item.ToAccountID = toAccount.ToAccountID;
                            }
                            else if (actual == 0)
                            {
                                // case from = to => from = 0, to = 0

                                var itemResult2 = new DivideMoneyExecute
                                {
                                    FromAccountID = item.FromAccountID,
                                    ToAccountID = item.ToAccountID,
                                    ActualAmount = item.ActualAmount,
                                };
                                listDetailResult.Add(itemResult2);

                                toAccount.ActualAmount = 0;
                                item.ActualAmount = 0;
                                item.ToAccountID = toAccount.ToAccountID;
                            }
                        }
                    }
                    else continue;
                }
                // nếu listDetail2 chỉ còn 1 phần tử thì thoát khỏi vòng lặp while
                if (listDetail2.Count == 1)
                {
                    listDetail2[0].FromAccountID = listDetail2[0].ToAccountID;
                    listDetailResult.Add(listDetail2[0]);
                    isContinue = false;
                    break;
                }
                if (breakcount > 1000) // tránh trường hợp vòng lặp vô hạn nếu có lỗi
                {
                    isContinue = false;
                    break;
                }
            }
            return listDetailResult;
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

        // this function is used to test delete newest activity
        internal object DeleteCFActivity()
        {
            try
            {
                // delete activity has newest id
                var activity = _context.CollabFundActivity
                     .OrderByDescending(cfa => cfa.CollabFundActivityID)
                     .FirstOrDefault();
                if (activity is null) throw new Exception(Message.COLLAB_FUND_ACTIVITY_NOT_FOUND);

                var cfdm = _context.CF_DividingMoney
                    .Where(cfdm => cfdm.CollabFunActivityID == activity.CollabFundActivityID)
                    .FirstOrDefault();
                if (cfdm is null) throw new Exception(Message.COLLAB_FUND_ACTIVITY_NOT_FOUND);

                var listdetail = _context.CF_DividingMoneyDetail
                    .Where(cfdmd => cfdmd.CF_DividingMoneyID == cfdm.CF_DividingMoneyID)
                    .ToList();
                _context.CF_DividingMoneyDetail.RemoveRange(listdetail);
                _context.SaveChanges();

                _context.CF_DividingMoney.Remove(cfdm);
                _context.SaveChanges();

                _context.CollabFundActivity.Remove(activity);
                _context.SaveChanges();
                return null;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal object DeleteCollabFund(CollabAccountDTO deleteCollabFundDTO)
        {
            try
            {
                var collabFund = GetCollabFund(deleteCollabFundDTO.CollabFundID);
                if (collabFund is null) throw new Exception(Message.COLLAB_FUND_NOT_EXIST);
                collabFund.ActiveStateID = ActiveStateConst.DELETED;
                _context.SaveChanges();
                // return list collab fund
                return GetCollabFunds(deleteCollabFundDTO.AccountID);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal void GetPersonalDivideMoneyInfor(int cfdm_detailID, string fromAccountID, string toAccountID, out string totalAmountString, out List<Wallet> listWallet, out Account toAccount)
        {
            try
            {
                var totalAmount = _context.CF_DividingMoneyDetail
                    .Where(cfdmd => cfdmd.CF_DividingMoneyDetailID == cfdm_detailID
                                    && cfdmd.FromAccountID == fromAccountID
                                    && cfdmd.ToAccountID == toAccountID).FirstOrDefault();
                if (totalAmount is null) throw new Exception(Message.CFDM_DETAIL_NOT_FOUND);

                totalAmountString = LConvertVariable.ConvertToMoneyFormat(totalAmount.DividingAmount);

                var walletDA = new WalletDA(_context);
                listWallet = walletDA.GetWallets(toAccountID);

                var authDa = new AuthDA(_context);
                toAccount = authDa.GetAccount(toAccountID);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal object ChangeIsDoneDividingMoneyDetail(CF_DividingMoneyDetail_Wallet_DTO cfdm_detail)
        {
            try
            {
                var result = _context.CF_DividingMoneyDetail
                   .Where(cfdmd => cfdmd.CF_DividingMoneyDetailID == cfdm_detail.CF_DividingMoneyDetailID
                                   && cfdmd.FromAccountID == cfdm_detail.FromAccountID
                                   && cfdmd.ToAccountID == cfdm_detail.ToAccountID).FirstOrDefault();
                if (result is null) throw new Exception(Message.CFDM_DETAIL_NOT_FOUND);
                result.IsDone = true;
                _context.SaveChanges();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal List<AccountInCollabFundDTO> GetAccountInCollabFunds(int collabFundID)
        {
            try
            {
                var result = _context.AccountCollab
                    .Where(ca => ca.CollabFundID == collabFundID
                                                   && ca.ActiveStateID == ActiveStateConst.ACTIVE)
                    .ToList();
                var listAccount = new List<AccountInCollabFundDTO>();
                var authDA = new AuthDA(_context);
                foreach (var item in result)
                {
                    var account = _context.Account
                        .Where(a => a.AccountID == item.AccountID)
                        .FirstOrDefault();
                    if (account is null) continue;
                    var accountInCollabFund = new AccountInCollabFundDTO
                    {
                        AccountID = account.AccountID,
                        AccountName = account.AccountName,
                        EmailAddress = account.EmailAddress,
                        PictureURL = account.PictureURL,
                    };
                    listAccount.Add(accountInCollabFund);
                }
                return listAccount;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
