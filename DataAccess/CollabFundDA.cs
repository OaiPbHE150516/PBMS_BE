using Microsoft.EntityFrameworkCore;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.Data.Auth;
using pbms_be.Data.CollabFund;
using pbms_be.Data.Trans;
using pbms_be.DTOs;
using System.Collections.Generic;

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

        internal List<Account> AddMemberCollabFund(MemberCollabFundDTO AddMemberDTO)
        {
            try
            {
                // 1. check if account is fundholder
                var collabAccount = IsFundholderCollabFund(AddMemberDTO.CollabFundID, AddMemberDTO.AccountFundholderID);
                if (collabAccount) throw new Exception(Message.ACCOUNT_IS_NOT_FUNDHOLDER);

                // 2. check if account is exist
                var authDA = new AuthDA(_context);
                var account = authDA.GetAccount(AddMemberDTO.AccountMemberID);
                if (account is null) throw new Exception(Message.ACCOUNT_NOT_FOUND);

                //3. check if account is already a member
                var isExist = IsAlreadyMemberCollabFund(AddMemberDTO.CollabFundID, AddMemberDTO.AccountMemberID);
                if (isExist) throw new Exception(Message.ACCOUNT_ALREADY_IS_MEMBER);

                // 4. add account as member
                var accountCollab = new AccountCollab
                {
                    AccountID = AddMemberDTO.AccountMemberID,
                    CollabFundID = AddMemberDTO.CollabFundID,
                    IsFundholder = false,
                    ActiveStateID = ActiveStateConst.ACTIVE
                };
                _context.AccountCollab.Add(accountCollab);
                _context.SaveChanges();
                // 5. return all member
                return GetAllMemberCollabFund(AddMemberDTO.CollabFundID, AddMemberDTO.AccountFundholderID);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

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
                _context.SaveChanges();

                // 4. return all member
                return GetAllMemberCollabFund(deleteMemberCollabFundDTO.CollabFundID, deleteMemberCollabFundDTO.AccountFundholderID);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
