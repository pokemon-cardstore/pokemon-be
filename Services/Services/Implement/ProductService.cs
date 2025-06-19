using AutoMapper;
using Repository.Models;
using Repository.UnitOfWork.Interface;
using Services.ModelView;
using Services.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Implement
{
    public class ProductService:  IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> AddNewProduct(Product product, List<string> imagePaths)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    bool status = false;
                    var checkCategory = _unitOfWork.CategoryRepository.GetByIDAsync(product.CategoryId);
                    if (checkCategory != null)
                    {
                        product.Status = 1;
                        await _unitOfWork.ProductRepository.InsertAsync(product);
                        await _unitOfWork.SaveAsync();

                        if (imagePaths.Any())
                        {
                            foreach (var imagePath in imagePaths)
                            {
                                if (!String.IsNullOrEmpty(imagePath))
                                {
                                    var image = new ProductImage
                                    {
                                        ProductId = product.ProductId,
                                        ImagePath = imagePath
                                    };
                                    await _unitOfWork.ProductImageRepository.InsertAsync(image);
                                    await _unitOfWork.SaveAsync();
                                }
                            }
                        }

                        status = true;
                        await transaction.CommitAsync();
                        return status;
                    }
                    else
                    {
                        return status;
                    }

            }
                catch (Exception ex)
                {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }
        }

        public async Task<bool> DeleteProduct(int id)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var checkProduct = await _unitOfWork.ProductRepository.GetByIDAsync(id);
                    if (checkProduct != null)
                    {
                        var Images = (await _unitOfWork.ProductImageRepository.GetAsync(p => p.ProductId == checkProduct.ProductId)).ToList();
                        if (Images.Any())
                        {
                            foreach (var image in Images)
                            {
                                await _unitOfWork.ProductImageRepository.DeleteAsync(image);
                                await _unitOfWork.SaveAsync();
                            }
                        }
                        await _unitOfWork.ProductRepository.DeleteAsync(checkProduct);
                        await _unitOfWork.SaveAsync();
                        await transaction.CommitAsync();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
            }
                catch (Exception ex)
                {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }
        }

        public async Task<List<ProductDtoResponse>> GetAllProducts(int CategoryId, string? sortBy, string? sortOrder)
        {
            try
            {
                var products = new List<Product>();
                if (CategoryId == 0)
                {
                    products = (await _unitOfWork.ProductRepository.GetAsync(p => p.Status == 1)).ToList();
                }
                else
                {
                    products = (await _unitOfWork.ProductRepository.GetAsync(p => p.Status == 1 && p.CategoryId == CategoryId)).ToList();
                }

                if (!string.IsNullOrEmpty(sortBy))
                {
                    bool descending = sortOrder?.Equals("desc", StringComparison.OrdinalIgnoreCase) ?? false;

                    if (sortBy.Equals("Price", StringComparison.OrdinalIgnoreCase))
                    {
                        products = descending ? products.OrderByDescending(p => p.Price).ToList() : products.OrderBy(p => p.Price).ToList();
                    }
                    else if (sortBy.Equals("Quantity", StringComparison.OrdinalIgnoreCase))
                    {
                        products = descending ? products.OrderByDescending(p => p.Quantity).ToList() : products.OrderBy(p => p.Quantity).ToList();
                    }
                }
                if (products.Any())
                {
                    List<ProductDtoResponse> list = new List<ProductDtoResponse>();
                    foreach (var product in products)
                    {
                        var productView = _mapper.Map<ProductDtoResponse>(product);
                        var productImages = (await _unitOfWork.ProductImageRepository.GetAsync(p => p.ProductId == product.ProductId)).FirstOrDefault();
                        if (productImages != null)
                        {
                            var imageView = new ProductImageView
                            {
                                Base64StringImage = productImages.ImagePath
                            };
                            productView.Images.Add(imageView);
                        }
                        list.Add(productView);
                    }
                    return list;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<ProductDtoResponse> GetProductByID(int id)
        {
            try
            {
                var product = (await _unitOfWork.ProductRepository.GetAsync(filter: p => p.ProductId == id, includeProperties: "Category")).FirstOrDefault();
                if (product != null)
                {
                    var productView = _mapper.Map<ProductDtoResponse>(product);
                    var productImages = await _unitOfWork.ProductImageRepository.GetAsync(p => p.ProductId == product.ProductId);
                    if (productImages.Any())
                    {
                        foreach (var image in productImages)
                        {
                            var imageView = new ProductImageView
                            {
                                Base64StringImage = image.ImagePath
                            };
                            productView.Images.Add(imageView);
                        }
                    }
                    return productView;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<List<ProductDtoResponse>> Search(string searchInput)
        {
            try
            {
                var products = (await _unitOfWork.ProductRepository.FindAsync(p => searchInput != null && p.Name.Contains(searchInput))).ToList();
                if (products.Any())
                {
                    List<ProductDtoResponse> list = new List<ProductDtoResponse>();
                    foreach (var product in products)
                    {
                        var productView = _mapper.Map<ProductDtoResponse>(product);
                        var productImages = (await _unitOfWork.ProductImageRepository.GetAsync(p => p.ProductId == product.ProductId)).FirstOrDefault();
                        if (productImages != null)
                        {
                            var imageView = new ProductImageView
                            {
                                Base64StringImage = productImages.ImagePath
                            };
                            productView.Images.Add(imageView);
                        }
                        list.Add(productView);
                    }
                    return list;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        public async Task<int> StatusProduct(int id)
        {
            try
            {

                var checkProduct = await _unitOfWork.ProductRepository.GetByIDAsync(id);
                if (checkProduct != null)
                {
                    if (checkProduct.Status == 1)
                    {
                        checkProduct.Status = 0;
                        await _unitOfWork.ProductRepository.UpdateAsync(checkProduct);
                        await _unitOfWork.SaveAsync();
                        return 1;
                    }
                    else if (checkProduct.Status == 0)
                    {
                        if (checkProduct.Quantity == 0)
                        {
                            return 2;
                        }
                        checkProduct.Status = 1;
                        await _unitOfWork.ProductRepository.UpdateAsync(checkProduct);
                        await _unitOfWork.SaveAsync();
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }

                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<(bool check, List<string>? oldImagePaths)> UpdateProduct(ProductDtoRequest request, List<string> imagePaths, int id)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    bool status = false;
                    var checkCategory = _unitOfWork.CategoryRepository.GetByIDAsync(request.CategoryId);
                    if (checkCategory != null)
                    {
                        var checkProduct = await _unitOfWork.ProductRepository.GetByIDAsync(id);
                        if (checkProduct != null)
                        {
                            var product = _mapper.Map(request, checkProduct);
                            await _unitOfWork.ProductRepository.UpdateAsync(product);
                            await _unitOfWork.SaveAsync();
                            var currentImagePaths = new List<string>();

                            var currentImages = await _unitOfWork.ProductImageRepository.GetAsync(p => p.ProductId == checkProduct.ProductId);
                            if (currentImages.Any())
                            {
                                foreach (var image in currentImages)
                                {
                                    await _unitOfWork.ProductImageRepository.DeleteAsync(image);
                                    await _unitOfWork.SaveAsync();
                                    currentImagePaths.Add(image.ImagePath);
                                }
                            }

                            if (imagePaths.Any())
                            {
                                foreach (var imagePath in imagePaths)
                                {
                                    if (!String.IsNullOrEmpty(imagePath))
                                    {
                                        var image = new ProductImage
                                        {
                                            ProductId = checkProduct.ProductId,
                                            ImagePath = imagePath
                                        };
                                        await _unitOfWork.ProductImageRepository.InsertAsync(image);
                                        await _unitOfWork.SaveAsync();
                                    }
                                }
                            }

                            status = true;
                            await transaction.CommitAsync();
                            return (status, currentImagePaths);
                        }
                        else
                        {
                            return (status, null);
                        }
                    }
                    else
                    {
                        return (status, null);
                    }

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}
