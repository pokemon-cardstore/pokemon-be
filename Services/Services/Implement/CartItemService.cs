using AutoMapper;
using Services.ModelView;
using Services.Service.Interface;
using Repository.Models;
using Repository.UnitOfWork.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RolexApplication_BAL.Service.Implement
{
    public class CartItemService : ICartItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CartItemService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> AddToCart(CartItemDtoRequest request)
        {
            try
            {
                var existedItemInCart = (await _unitOfWork.CartItemRepository.GetAsync(filter: c => c.ProductId == request.ProductId && c.CustomerId == request.CustomerId, includeProperties: "Product")).FirstOrDefault();
                if (existedItemInCart == null)
                {
                    var cartItem = _mapper.Map<CartItem>(request);
                    await _unitOfWork.CartItemRepository.InsertAsync(cartItem);
                    await _unitOfWork.SaveAsync();
                    return true;
                }
                else {
                    var totalItem = existedItemInCart.Quantity + request.Quantity;
                    if (existedItemInCart.Product.Quantity > totalItem)
                    {
                        existedItemInCart.Quantity = totalItem;
                        await _unitOfWork.CartItemRepository.UpdateAsync(existedItemInCart);
                        await _unitOfWork.SaveAsync();
                        return true;
                    } else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteItemInCart(int id)
        {
            try
            {
                var cartItemToDelete = await _unitOfWork.CartItemRepository.GetByIDAsync(id);
                if (cartItemToDelete != null) { 
                    await _unitOfWork.CartItemRepository.DeleteAsync(cartItemToDelete);
                    await _unitOfWork.SaveAsync();
                    return true;
                } else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetAllCart()
        {
            return  await _unitOfWork.cartRepository.GetCount();
        }

        public async Task<List<CartItemDtoResponse>> GetCartItemsByCustomerId(int CustomerId)
        {
            try
            {
                var response = new List<CartItemDtoResponse>();
                var cartItems = await _unitOfWork.CartItemRepository.GetAsync(c => c.CustomerId == CustomerId, includeProperties: "Product");
                foreach (var item in cartItems)
                {
                    var itemView = _mapper.Map<CartItemDtoResponse>(item);
                    itemView.ProductVIew = _mapper.Map<ProductDtoResponse>(item.Product);
                    var productImages = (await _unitOfWork.ProductImageRepository.GetAsync(p => p.ProductId == item.Product.ProductId)).FirstOrDefault();
                    if (productImages != null)
                    {
                        var imageView = new ProductImageView
                        {
                            Base64StringImage = productImages.ImagePath
                        };
                        itemView.ProductVIew.Images.Add(imageView);
                    }
                    response.Add(itemView);
                }
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> UpdateItemQuantityInCart(int id, int quantity)
        {
            try
            {
                var cartItem = await _unitOfWork.CartItemRepository.GetByIDAsync(id);
                if (cartItem != null) { 
                    if(quantity == 0)
                    {
                        await _unitOfWork.CartItemRepository.DeleteAsync(cartItem);
                        await _unitOfWork.SaveAsync();
                        return 3;
                    }
                    var product = await _unitOfWork.ProductRepository.GetByIDAsync(cartItem.ProductId);
                    if (product != null)
                    {
                        if (product.Quantity < quantity)
                        {
                            return 2;
                        }
                        else
                        {
                            cartItem.Quantity = quantity;
                            await _unitOfWork.CartItemRepository.UpdateAsync(cartItem);
                            await _unitOfWork.SaveAsync();
                            return 1;
                        }
                    }
                    else
                    {
                        return -1;
                    }
                } else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
