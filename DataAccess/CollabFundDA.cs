﻿using Microsoft.EntityFrameworkCore;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.Data.CollabFund;
using pbms_be.Data.Trans;
using pbms_be.DTOs;

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
                    .Where(cf => cf.Name == name)
                    .Include(cf => cf.ActiveState)
                    .FirstOrDefault();
                return result;
            } catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal CollabFund GetCollabFund(int collabFundID)
        {
            try
            {
                var result = _context.CollabFund
                    .Where(cf => cf.CollabFundID == collabFundID)
                    .Include(cf => cf.ActiveState)
                    .FirstOrDefault();
                return result;
            } catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal CollabFund GetCollabFund(int collabFundID, string accountID)
        {
            try
            {
                var collabAccount = _context.AccountCollab
                .Where(ca => ca.AccountID == accountID)
                .Select(ca => ca.CollabFundID)
                .ToList();

                var result = _context.CollabFund
                    .Where(cf => collabAccount.Contains(cf.CollabFundID) && cf.CollabFundID == collabFundID)
                    .Include(cf => cf.ActiveState)
                    .FirstOrDefault();
                //if (result == null) throw new Exception(Message.COLLAB_FUND_NOT_EXIST);
                return result;
            } catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal List<CollabFund> GetCollabFunds(string accountID)
        {
            try
            {
                var collabAccount = _context.AccountCollab
                .Where(ca => ca.AccountID == accountID)
                .Select(ca => ca.CollabFundID)
                .ToList();

                var result = _context.CollabFund
                    .Where(cf => collabAccount.Contains(cf.CollabFundID))
                    .Include(cf => cf.ActiveState)
                    .ToList();
                return result;
            } catch (Exception e)
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
            } catch (Exception e)
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
                if (collabFund == null) throw new Exception(Message.COLLAB_FUND_NOT_EXIST);
                collabFund.ActiveStateID = changeActiveStateDTO.ActiveStateID;
                _context.SaveChanges();
                return GetCollabFund(collabFund.CollabFundID);
            } catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal void GetDetailCollabFund(int collabFundID, string accountID, out bool isExist, out CollabFund collabFund)
        {
            try
            {
                var collabAccount = _context.AccountCollab
                .Where(ca => ca.AccountID == accountID)
                .Select(ca => ca.CollabFundID)
                .ToList();

                // get list collabFundActivity by collabFundID
                var collabFundActivity = _context.CollabFundActivity
                    .Where(cfa => collabAccount.Contains(cfa.CollabFundID) && cfa.CollabFundID == collabFundID)
                    .Include(cfa => cfa.ActiveState)
                    .ToList();

                // get list collabfundacttransaction by collabFundActivityID
                var collabFundActTransaction = _context.CollabFundActTransaction
                    .Where(cfat => collabFundActivity.Select(cfa => cfa.CollabFundActivityID).Contains(cfat.CollabFundActivityID))
                    .Include(cfat => cfat.ActiveState)
                    .ToList();

                // get list transaction by collabFundActTransactionID
                //var transaction = _context.Transaction
                //    .Where(t => collabFundActTransaction.Select(cfat => cfat.TransactionID).Contains(t.TransactionID))
                //    .Include(t => t.ActiveState)
                //    .ToList();

                // get var result collabFund by collabFundID
                var result = _context.CollabFund
                    .Where(cf => collabAccount.Contains(cf.CollabFundID) && cf.CollabFundID == collabFundID)
                    .Include(cf => cf.ActiveState)
                    .FirstOrDefault();



                //foreach (var cfa in collabFundActivity)
                //{
                //    cfa.Transactions = transaction.Where(t => collabFundActTransaction.Where(cfat => cfat.CollabFundActivityID == cfa.CollabFundActivityID).Select(cfat => cfat.TransactionID).Contains(t.TransactionID)).ToList();
                //}

                if (result is null)
                {
                    isExist = false;
                    collabFund = new CollabFund();
                }
                else
                {
                    isExist = true;
                    collabFund = result;
                }

                //var result = _context.CollabFund
                //    .Where(cf => collabAccount.Contains(cf.CollabFundID) && cf.CollabFundID == collabFundID)
                //    .Include(cf => cf.ActiveState)
                //    .FirstOrDefault();

               

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
