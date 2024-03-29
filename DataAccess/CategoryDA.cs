﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.Data.Filter;
using pbms_be.DTOs;

namespace pbms_be.DataAccess
{
    public class CategoryDA
    {
        private readonly PbmsDbContext _context;

        public CategoryDA(PbmsDbContext context)
        {
            _context = context;
        }

        // get all category by account id
        public List<Category> GetCategories(string AccountID)
        {
            try
            {
                var result = _context.Category
                            .Where(c => c.AccountID == AccountID)
                            .Include(c => c.ActiveState)
                            .Include(c => c.CategoryType)
                            .OrderBy(c => c.CategoryID)
                            .ToList();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }



        internal object GenerateDefaultCategories(string accountID)
        {
            try
            {
                // get default categories
                var defaultCategories = _context.DefaultCategory.ToList();
                var list = new List<Category>();
                var listParentCategory = new List<Category>();
                foreach (var defaultCategory in defaultCategories)
                {
                    var category = new Category
                    {
                        AccountID = accountID,
                        ParentCategoryID = defaultCategory.ParentCategoryID,
                        NameVN = defaultCategory.NameVN,
                        NameEN = defaultCategory.NameEN,
                        ActiveStateID = ActiveStateConst.ACTIVE,
                        CategoryTypeID = defaultCategory.CategoryTypeID,
                        IsRoot = defaultCategory.IsRoot
                    };
                    if (defaultCategory.IsRoot)
                    {
                        listParentCategory.Add(category);
                    }
                    list.Add(category);
                }
                // add parent category to db
                _context.Category.AddRange(listParentCategory);
                _context.SaveChanges();

                var listRootCategory = _context.Category.Where(c => c.AccountID == accountID && c.IsRoot).ToList();
                var parentIncomeCategory = listRootCategory.FirstOrDefault(c => c.NameEN == ConstantConfig.DEFAULT_CATEGORY_NAME_EN_INCOME && c.AccountID == accountID && c.IsRoot);
                var parentExpenseCategory = listRootCategory.FirstOrDefault(c => c.NameEN == ConstantConfig.DEFAULT_CATEGORY_NAME_EN_EXPENSE && c.AccountID == accountID && c.IsRoot);
                var parentOtherCategory = listRootCategory.FirstOrDefault(c => c.NameEN == ConstantConfig.DEFAULT_CATEGORY_NAME_EN_OTHER && c.AccountID == accountID && c.IsRoot);
                foreach (var category in list)
                {
                    if (!category.IsRoot)
                    {
                        switch (category.ParentCategoryID)
                        {
                            case 1:
                                if (parentIncomeCategory is null) continue;
                                category.ParentCategoryID = parentIncomeCategory.CategoryID;
                                break;
                            case 2:
                                if (parentExpenseCategory is null) continue;
                                category.ParentCategoryID = parentExpenseCategory.CategoryID;
                                break;
                            case 16:
                                if (parentOtherCategory is null) continue;
                                category.ParentCategoryID = parentOtherCategory.CategoryID;
                                break;
                        }
                        _context.Category.Add(category);
                        _context.SaveChanges();
                    }
                }
                return GetCategories(accountID);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //internal object GetAllCategories(string accountID)
        //{
        //    try
        //    {
        //        var result = _context.Category
        //                    .Where(c => c.AccountID == accountID)
        //                    .Include(c => c.ActiveState)
        //                    .Include(c => c.CategoryType)
        //                    .ToList();
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}

        internal CategoryType GetCategoryType(int categoryTypeID)
        {
            try
            {
                var result = _context.CategoryType.Where(c => c.CategoryTypeID == categoryTypeID).FirstOrDefault();
                if (result is null) throw new Exception(categoryTypeID + Message.CATEGORY_TYPE_NOT_FOUND);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal List<CategoryType> GetCategoryTypes()
        {
            try
            {
                var result = _context.CategoryType.ToList();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal object GetDefaultCategories()
        {
            try
            {
                var result = _context.DefaultCategory.ToList();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal bool IsCategoryExist(Category category)
        {
            try
            {
                var result = _context.Category.Any(c => c.NameEN == category.NameEN && c.AccountID == category.AccountID && c.ParentCategoryID == category.ParentCategoryID
                                                    || c.NameVN == category.NameVN && c.AccountID == category.AccountID && c.ParentCategoryID == category.ParentCategoryID);
                return result;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal object CreateCategory(Category category)
        {
            try
            {
                var parent = GetCategory(category.ParentCategoryID, category.AccountID);
                if (parent is null) throw new Exception(Message.CATEGORY_PARENT_NOT_FOUND);
                category.CategoryTypeID = parent.CategoryTypeID;
                category.ActiveStateID = ActiveStateConst.ACTIVE;
                category.IsRoot = false;
                _context.Category.Add(category);
                _context.SaveChanges();
                return GetCategory(category.CategoryID, category.AccountID);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Category GetCategory(int categoryID, string accountID)
        {
            try
            {
                var result = _context.Category
                            .Where(c => c.CategoryID == categoryID && c.AccountID == accountID)
                            .Include(c => c.ActiveState)
                            .Include(c => c.CategoryType)
                            .FirstOrDefault();
                if (result is null) throw new Exception(Message.CATEGORY_NOT_FOUND);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal object UpdateCategory(Category category)
        {
            try
            {
                var categoryUpdate = GetCategory(category.CategoryID, category.AccountID);
                if (categoryUpdate is null) throw new Exception(Message.CATEGORY_NOT_FOUND);
                categoryUpdate.NameEN = category.NameEN;
                categoryUpdate.NameVN = category.NameVN;
                categoryUpdate.ParentCategoryID = category.ParentCategoryID;
                var parent = GetCategory(category.ParentCategoryID, category.AccountID);
                if (parent is null) throw new Exception(Message.CATEGORY_PARENT_NOT_FOUND);
                categoryUpdate.CategoryTypeID = parent.CategoryTypeID;
                _context.SaveChanges();
                return GetCategory(category.CategoryID, category.AccountID);
            } catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal object DeleteCategory(int categoryID, string accountID)
        {
            try
            {
                var category = GetCategory(categoryID, accountID);
                if (category is null) throw new Exception(Message.CATEGORY_NOT_FOUND);
                category.ActiveStateID = ActiveStateConst.DELETED;
                _context.SaveChanges();
                return category;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal object GetCategoriesTypeByType(string accountID, AutoMapper.IMapper? _mapper)
        {
            try
            {
                //var categoriesDict = new Dictionary<string, List<Category>>();
                var categories = GetCategories(accountID);
                if (_mapper is null) throw new Exception(Message.MAPPER_IS_NULL);
                var categoriesDTO = _mapper.Map<List<CategoryTree_VM_DTO>>(categories);

                var categoryMap = categoriesDTO.ToDictionary(c => c.CategoryID, c => c);
                var rootCategories = new List<CategoryTree_VM_DTO>();

                // add children to parent category
                foreach (var category in categoriesDTO)
                {
                    if (category.ParentCategoryID == 0)
                    {
                        rootCategories.Add(category);
                    }
                    else
                    {
                        if (categoryMap.ContainsKey(category.ParentCategoryID))
                        {
                            categoryMap[category.ParentCategoryID].Children.Add(category);
                        }
                    }
                }
                return rootCategories;




                //var categoryTypes = GetCategoryTypes();
                //foreach (var categoryType in categoryTypes)
                //{
                //    var list = categories.Where(c => c.CategoryTypeID == categoryType.CategoryTypeID).ToList();
                //    categoriesDict.Add(categoryType.Name, list);
                //}

                //var newCateDict = new Dictionary<string, List<CategoryTree_VM_DTO>>();
                //foreach (var categoryType in categoryTypes)
                //{
                //    var list = categories.Where(c => c.CategoryTypeID == categoryType.CategoryTypeID).ToList();
                //    var listTree = new List<CategoryTree_VM_DTO>();
                //    foreach (var category in list)
                //    {
                //        //var categoryTree = new CategoryTree_VM_DTO
                //        //{
                //        //    CategoryID = category.CategoryID,
                //        //    NameVN = category.NameVN,
                //        //    ParentCategoryID = category.ParentCategoryID,
                //        //    IsRoot = category.IsRoot
                //        //};
                //        //listTree.Add(categoryTree);

                //        // if category have parent category id in list, add to parent category children
                //        if (listTree.Any(c => c.CategoryID == category.ParentCategoryID))
                //        {
                //            var parentCategory = listTree.FirstOrDefault(c => c.CategoryID == category.ParentCategoryID);
                //            var categoryTree = new CategoryTree_VM_DTO
                //            {
                //                CategoryID = category.CategoryID,
                //                NameVN = category.NameVN,
                //                ParentCategoryID = category.ParentCategoryID,
                //                IsRoot = category.IsRoot
                //            };
                //            parentCategory.Children.Add(categoryTree);
                //        }
                //        else
                //        {
                //            var categoryTree = new CategoryTree_VM_DTO
                //            {
                //                CategoryID = category.CategoryID,
                //                NameVN = category.NameVN,
                //                ParentCategoryID = category.ParentCategoryID,
                //                IsRoot = category.IsRoot
                //            };
                //            listTree.Add(categoryTree);
                //        }
                //    }
                //    newCateDict.Add(categoryType.Name, listTree);
                //}
                //return newCateDict;











            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
