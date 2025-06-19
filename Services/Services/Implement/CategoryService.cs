using AutoMapper;
using Azure.Core;
using Services.ModelView;
using Repository.UnitOfWork.Interface;
using Repository.Models;
using Services.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service.Implement
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task CreateCategory(CategoryDtoRequest request)
        {
            try
            {
                var category = _mapper.Map<Category>(request);
                category.Status = 1;
                await _unitOfWork.CategoryRepository.InsertAsync(category);
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteCategory(int CategoryId)
        {
            try
            {
                var category = (await _unitOfWork.CategoryRepository.FindAsync(c => c.CategoryId == CategoryId)).FirstOrDefault();
                if (category == null)
                {
                    throw new Exception("No category match this id");
                }
                else
                {
                    await _unitOfWork.CategoryRepository.DeleteAsync(category);
                    await _unitOfWork.SaveAsync();
                }
            }
            catch (Exception ex) { 
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<CategoryDtoResponse>> GetAllCategories()
        {
            try
            {
                var categoryes = await _unitOfWork.CategoryRepository.FindAsync(c => c.Status == 1);
                if (categoryes.Count() == 0)
                {
                    return null;
                }
                List<CategoryDtoResponse> categoryViews = new List<CategoryDtoResponse>();
                foreach (var Type in categoryes)
                {
                    var categoryView = _mapper.Map<CategoryDtoResponse>(Type);
                    categoryViews.Add(categoryView);
                }
                return categoryViews;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<CategoryDtoResponse> GetCategoryById(int id)
        {
            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByIDAsync(id);
                if (category == null)
                {
                    return null;
                }
                var categoryView = _mapper.Map<CategoryDtoResponse>(category);
                return categoryView;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateCategory(int CategoryId, CategoryDtoRequest request)
        {
            try
            {
                var category = (await _unitOfWork.CategoryRepository.FindAsync(c => c.CategoryId == CategoryId)).FirstOrDefault();
                if (category == null) {
                    throw new Exception("No category match this id");
                } else
                {
                    category.Name = request.Name;
                    category.Description = request.Description;
                    await _unitOfWork.CategoryRepository.UpdateAsync(category);
                    await _unitOfWork.SaveAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
